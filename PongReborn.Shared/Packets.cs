using LiteNetLib.Utils;

namespace PongReborn.Shared;

// Sent from server -> client, every tick (or every N ticks).
// Unreliable/Sequenced delivery — a dropped one doesn't matter, next one supersedes it.
public struct BallStatePacket : INetSerializable
{
    public float PositionX;
    public float PositionY;
    public float VelocityX;
    public float VelocityY;

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(PositionX);
        writer.Put(PositionY);
        writer.Put(VelocityX);
        writer.Put(VelocityY);
    }

    public void Deserialize(NetDataReader reader)
    {
        PositionX = reader.GetFloat();
        PositionY = reader.GetFloat();
        VelocityX = reader.GetFloat();
        VelocityY = reader.GetFloat();
    }
}

// Sent from server -> client, every tick. Paddle Y positions for both players.
public struct PaddleStatePacket : INetSerializable
{
    public float Player1Y;
    public float Player2Y;

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Player1Y);
        writer.Put(Player2Y);
    }

    public void Deserialize(NetDataReader reader)
    {
        Player1Y = reader.GetFloat();
        Player2Y = reader.GetFloat();
    }
}

// Sent from client -> server, every tick. Just raw input, never trust the client's position.
public struct PlayerInputPacket : INetSerializable
{
    public bool MoveUp;
    public bool MoveDown;

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(MoveUp);
        writer.Put(MoveDown);
    }

    public void Deserialize(NetDataReader reader)
    {
        MoveUp = reader.GetBool();
        MoveDown = reader.GetBool();
    }
}

// Sent from server -> client, ReliableOrdered. One-shot events, not per-frame.
public struct GoalScoredPacket : INetSerializable
{
    public int ScoringPlayer; // 1 or 2
    public int Player1Score;
    public int Player2Score;

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(ScoringPlayer);
        writer.Put(Player1Score);
        writer.Put(Player2Score);
    }

    public void Deserialize(NetDataReader reader)
    {
        ScoringPlayer = reader.GetInt();
        Player1Score = reader.GetInt();
        Player2Score = reader.GetInt();
    }
}

// Sent from server -> client once, when a client connects and is assigned a side.
public struct AssignPlayerPacket : INetSerializable
{
    public int PlayerNumber; // 1 or 2

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(PlayerNumber);
    }

    public void Deserialize(NetDataReader reader)
    {
        PlayerNumber = reader.GetInt();
    }
}

// Packet type IDs so both sides agree on how to interpret the first byte.
public enum PacketType : byte
{
    BallState = 1,
    PaddleState = 2,
    PlayerInput = 3,
    GoalScored = 4,
    AssignPlayer = 5
}