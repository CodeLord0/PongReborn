using System.Net;
using LiteNetLib;
using LiteNetLib.Utils;
using PongReborn.Shared;
using System;
using System.Threading;

namespace PongReborn.Server;

public class Program
{
    private const int Port = 31414;
    private const float BaseBallSpeed = 250f;
    private const float MaxBallSpeed = 700f;
    private const float PaddleSpeed = 200f;
    private const float BallRadius = 15f;

    private static NetManager server = null!;
    private static readonly EventBasedNetListener listener = new();

    // Authoritative game state
    private static float ballX = 401f, ballY = 227f;
    private static float ballVelX = BaseBallSpeed, ballVelY = BaseBallSpeed;
    private static float p1Y = 117f, p2Y = 220f;
    private static int p1Score, p2Score;
    private static bool wasCollidingP1, wasCollidingP2;

    private static NetPeer? player1Peer;
    private static NetPeer? player2Peer;
    private static PlayerInputPacket p1Input;
    private static PlayerInputPacket p2Input;

    public static void Main(string[] args)
    {
        Console.WriteLine("Starting Pong server on port " + Port);

        listener.ConnectionRequestEvent += request => request.AcceptIfKey("PongRebornKey");

        listener.PeerConnectedEvent += peer =>
        {
            if (player1Peer == null)
            {
                player1Peer = peer;
                SendAssignPlayer(peer, 1);
                Console.WriteLine($"Player 1 connected: {peer}");
            }
            else if (player2Peer == null)
            {
                player2Peer = peer;
                SendAssignPlayer(peer, 2);
                Console.WriteLine($"Player 2 connected: {peer}");
            }
            else
            {
                Console.WriteLine($"Extra connection rejected: {peer}");
                peer.Disconnect();
            }
        };

        listener.PeerDisconnectedEvent += (peer, info) =>
        {
            if (peer == player1Peer) player1Peer = null;
            if (peer == player2Peer) player2Peer = null;
            Console.WriteLine($"Player disconnected: {peer}, reason: {info.Reason}");
        };

        listener.NetworkReceiveEvent += (peer, reader, channel, method) =>
        {
            var type = (PacketType)reader.GetByte();

            if (type == PacketType.PlayerInput)
            {
                var packet = new PlayerInputPacket();
                packet.Deserialize(reader);
                OnPlayerInput(packet, peer);
            }

            reader.Recycle();
        };

        server = new NetManager(listener);
        server.Start(Port);

        ResetBall();

        var lastTime = DateTime.UtcNow;
        while (true)
        {
            server.PollEvents();

            var now = DateTime.UtcNow;
            float dt = (float)(now - lastTime).TotalSeconds;
            lastTime = now;

            Tick(dt);

            Thread.Sleep(15); // ~66 ticks/sec
        }
    }

    private static void OnPlayerInput(PlayerInputPacket packet, NetPeer peer)
    {
        if (peer == player1Peer) p1Input = packet;
        else if (peer == player2Peer) p2Input = packet;
    }

    private static void SendAssignPlayer(NetPeer peer, int playerNumber)
    {
        var packet = new AssignPlayerPacket { PlayerNumber = playerNumber };
        var writer = new NetDataWriter();
        writer.Put((byte)PacketType.AssignPlayer);
        packet.Serialize(writer);
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    private static void Tick(float dt)
    {
        if (dt <= 0 || dt > 0.25f) return; // guard against huge/negative dt spikes (e.g. debugger pause)

        // Apply input -> paddle movement (server is authoritative, never trusts client position)
        if (p1Input.MoveUp && p1Y >= 167) p1Y -= PaddleSpeed * dt;
        if (p1Input.MoveDown && p1Y <= 455) p1Y += PaddleSpeed * dt;
        if (p2Input.MoveUp && p2Y >= 167) p2Y -= PaddleSpeed * dt;
        if (p2Input.MoveDown && p2Y <= 455) p2Y += PaddleSpeed * dt;

        // Ball physics (mirrors Ball.cs rules)
        ballX += ballVelX * dt;
        ballY += ballVelY * dt;

        if (ballY <= 57) ballVelY = -ballVelY;
        if (ballY >= 455) ballVelY = -ballVelY;

        // Paddle hitboxes: paddle1 at x=67, paddle2 at x=769, same dimensions as Player.cs (17 wide, 120 tall)
        bool collidingP1 = RectsIntersect(67 - 17, p1Y - 120, 17, 120, ballX - BallRadius, ballY - BallRadius, BallRadius * 2, BallRadius * 2);
        bool collidingP2 = RectsIntersect(769 - 17, p2Y - 120, 17, 120, ballX - BallRadius, ballY - BallRadius, BallRadius * 2, BallRadius * 2);

        if (collidingP1 && !wasCollidingP1) Bounce();
        if (collidingP2 && !wasCollidingP2) Bounce();
        wasCollidingP1 = collidingP1;
        wasCollidingP2 = collidingP2;

        // Goals
        if (ballX <= 0)
        {
            p2Score++;
            BroadcastGoal(2);
            ResetBall();
        }
        else if (ballX >= 802)
        {
            p1Score++;
            BroadcastGoal(1);
            ResetBall();
        }

        BroadcastState();
    }

    private static void Bounce()
    {
        ballVelX = -ballVelX;
        ballVelX += MathF.Sign(ballVelX) * 15f;
        ballVelY += MathF.Sign(ballVelY) * 15f;

        float speed = MathF.Sqrt(ballVelX * ballVelX + ballVelY * ballVelY);
        if (speed > MaxBallSpeed)
        {
            float scale = MaxBallSpeed / speed;
            ballVelX *= scale;
            ballVelY *= scale;
        }
    }

    private static bool RectsIntersect(float ax, float ay, float aw, float ah, float bx, float by, float bw, float bh)
    {
        return ax < bx + bw && ax + aw > bx && ay < by + bh && ay + ah > by;
    }

    private static void ResetBall()
    {
        ballX = 401f;
        ballY = 227f;
        float xDir = Random.Shared.Next(2) == 0 ? 1f : -1f;
        float yDir = Random.Shared.Next(2) == 0 ? 1f : -1f;
        ballVelX = BaseBallSpeed * xDir;
        ballVelY = BaseBallSpeed * yDir;
        wasCollidingP1 = false;
        wasCollidingP2 = false;
    }

    private static void BroadcastState()
    {
        var ballPacket = new BallStatePacket { PositionX = ballX, PositionY = ballY, VelocityX = ballVelX, VelocityY = ballVelY };
        var ballWriter = new NetDataWriter();
        ballWriter.Put((byte)PacketType.BallState);
        ballPacket.Serialize(ballWriter);
        server.SendToAll(ballWriter, DeliveryMethod.Sequenced);

        var paddlePacket = new PaddleStatePacket { Player1Y = p1Y, Player2Y = p2Y };
        var paddleWriter = new NetDataWriter();
        paddleWriter.Put((byte)PacketType.PaddleState);
        paddlePacket.Serialize(paddleWriter);
        server.SendToAll(paddleWriter, DeliveryMethod.Sequenced);
    }

    private static void BroadcastGoal(int scoringPlayer)
    {
        var packet = new GoalScoredPacket { ScoringPlayer = scoringPlayer, Player1Score = p1Score, Player2Score = p2Score };
        var writer = new NetDataWriter();
        writer.Put((byte)PacketType.GoalScored);
        packet.Serialize(writer);
        server.SendToAll(writer, DeliveryMethod.ReliableOrdered);
    }
}