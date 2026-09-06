using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace PongRebornCopy;

public class Ball
{
    public Vector2 position;
    public Vector2 velocity;
    private readonly Texture2D ballTexture;

    private const int Radius = 15;
    private const float StartX = 401f;
    private const float StartY = 227f;
    private const float BaseSpeed = 250f;
    private const float MaxSpeed = 700f; // cap so rallies don't accelerate forever
    //private Random random;

    public Rectangle Hitbox => new((int)position.X - Radius, (int)position.Y - Radius, Radius * 2, Radius * 2);

    public Ball(Texture2D _ballSprite)
    {
        ballTexture = _ballSprite;
        Reset();
    }

    // Centers the ball and gives it a fresh, randomized-direction starting velocity.
    // Call this on construction and whenever a point is scored.
    public void Reset()
    {
        position.X = StartX;
        position.Y = StartY;

        float xDir = Random.Shared.Next(2) == 0 ? 1f : -1f;
        float yDir = Random.Shared.Next(2) == 0 ? 1f : -1f;
        velocity = new Vector2(BaseSpeed * xDir, BaseSpeed * yDir);
    }

    public void BallUpdate(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        position.X += velocity.X * dt;
        position.Y += velocity.Y * dt;

        // top screen
        if (position.Y <= 57)
        {
            velocity.Y = -velocity.Y;
        }

        // bottom screen
        if (position.Y >= 455)
        {
            velocity.Y = -velocity.Y;
        }

        // Note: goal detection + reset now lives in Game1, since scoring is a
        // game-rule concern, not a ball-physics concern. Ball no longer
        // teleports itself off-screen.
    }

    public void InvertAxis()
    {
        velocity.X = -velocity.X;
    }

    // Adds a speed bump on paddle bounce, clamped so speed can't grow unbounded.
    public void AddBounceSpeed(Vector2 amount)
    {
        velocity += amount;

        float speed = velocity.Length();
        if (speed > MaxSpeed)
        {
            velocity = velocity * (MaxSpeed / speed);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(ballTexture, new Vector2(position.X - Radius, position.Y - Radius), Color.White);
    }
}