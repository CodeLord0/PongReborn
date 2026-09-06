using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace PongRebornCopy;

public class Player(Texture2D _paddleSprite, Vector2 _position)
{
    public const float speed = 200f;
    public Vector2 position = _position;
    public Texture2D paddleSprite = _paddleSprite;
    public Rectangle Hitbox => new((int)position.X - 17, (int)position.Y - 120, 17, 120);
    public double playerScore = 0D;

    public void PlayerUpdate(GameTime gameTime)
    {
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(paddleSprite, new Vector2(position.X - 17, position.Y - 120), Color.White);
    }
}