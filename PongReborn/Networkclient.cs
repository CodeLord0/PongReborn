using System;
using System.Net;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;
using PongReborn.Shared;

namespace PongReborn;

public class NetworkClient
{
    private NetManager client;
    
    private readonly EventBasedNetListener listener = new();
    private NetPeer? serverPeer;

    public int MyPlayerNumber { get; private set; } = 0; // 0 = not assigned yet

    // Latest state received from server - Game1 reads these each frame.
    public float BallX, BallY, BallVelX, BallVelY;
    public float Player1Y, Player2Y;
    public int Player1Score, Player2Score;

    // Raised when a goal packet arrives, so Game1 can play a sound / reset visuals if needed.
    public event Action<int>? OnGoalScored;

    public double time;
    public double ClockTime;

    public bool IsConnected => serverPeer != null && serverPeer.ConnectionState == ConnectionState.Connected;
    public bool isGamePlaying = false;

    public void Connect(string ip, int port)
    {
        listener.NetworkReceiveEvent += OnNetworkReceive;
        listener.PeerConnectedEvent += peer =>
        {
            serverPeer = peer;
            Console.WriteLine("Connected to server: " + peer);
        };
        listener.PeerDisconnectedEvent += (peer, info) =>
        {
            Console.WriteLine("Disconnected from server: " + info.Reason);
            serverPeer = null;
        };

        client = new NetManager(listener);
        client.Start();
        client.Connect(ip, port, "PongRebornKey");
    }

    // Call this once per frame from Game1.Update, before reading state.
    public void PollEvents()
    {
        client?.PollEvents();
    }

    // Call this once per frame from Game1.Update, with the local player's current input.
    public void SendInput(bool moveUp, bool moveDown)
    {
        if (serverPeer == null) return;

        var packet = new PlayerInputPacket { MoveUp = moveUp, MoveDown = moveDown };
        var writer = new NetDataWriter();
        writer.Put((byte)PacketType.PlayerInput);
        packet.Serialize(writer);
        serverPeer.Send(writer, DeliveryMethod.Sequenced); // only the latest input matters
        
    }

    private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod method)
    {
        var type = (PacketType)reader.GetByte();

        switch (type)
        {
            case PacketType.BallState:
                var ball = new BallStatePacket();
                ball.Deserialize(reader);
                BallX = ball.PositionX;
                BallY = ball.PositionY;
                BallVelX = ball.VelocityX;
                BallVelY = ball.VelocityY;
                break;

            case PacketType.PaddleState:
                var paddle = new PaddleStatePacket();
                paddle.Deserialize(reader);
                Player1Y = paddle.Player1Y;
                Player2Y = paddle.Player2Y;
                break;

            case PacketType.GoalScored:
                var goal = new GoalScoredPacket();
                goal.Deserialize(reader);
                Player1Score = goal.Player1Score;
                Player2Score = goal.Player2Score;
                OnGoalScored?.Invoke(goal.ScoringPlayer);
                break;

            case PacketType.AssignPlayer:
                var assign = new AssignPlayerPacket();
                assign.Deserialize(reader);
                MyPlayerNumber = assign.PlayerNumber;
                Console.WriteLine("Assigned as player " + MyPlayerNumber);
                break;

            case PacketType.CountdownPacket:
                var timer = new CountdownPacket();
                timer.Deserialize(reader);
                time = timer.SecondsRemaining;
                Console.WriteLine(time);
                break;

            // Inside your packet-reading switch or router on the client:
            case PacketType.GameStartPacket:
                var gameStartPacket = new GameStartPacket();
                gameStartPacket.Deserialize(reader);
                isGamePlaying = true; // Set the flag to indicate the game has started

                // Switch your game state to active play
                Console.WriteLine("Game has officially started!");
                break;

            case PacketType.ClockPacket:
                var clockPacket = new ClockPacket();
                clockPacket.Deserialize(reader);
                ClockTime = clockPacket.Time;
                break;
                // Update your game clock or timer with clockPacket.Time
        }

        reader.Recycle();
    }

    public void Disconnect()
    {
        client?.Stop();
    }
}