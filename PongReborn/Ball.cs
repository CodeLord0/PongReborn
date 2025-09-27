using System;
using System.Dynamic;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace PongReborn;

public class Ball
{
    private Vector2 speed = new Vector2(180,180);
    
    private Vector2 position;
    //static  public Vector2 defaultPosition = new Vector2(401,228);
    private Texture2D ballTexture;
    
    const int radius = 15;
    bool state = true;
    private bool hasRun = true;
    private static Random rand = new Random();

    
    public Ball(Texture2D _ballSprite)
    {
        ballTexture = _ballSprite;
    }
    

    public void BallUpdate(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (hasRun)
        {
            position.X = rand.Next(100, 400);
            position.Y = rand.Next(100, 300);
            hasRun = false;
        }
   
        position += speed * dt;
        if (position.Y <= 0 || position.Y >= 455 || position.X <= 0 || position.X >= 802)
        {
            speed *= -2;
            

            

        }
    
    

    }



    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(ballTexture, new Vector2(position.X - radius,position.Y - radius), Color.White);
    }



}
