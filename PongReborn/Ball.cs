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

    
    public Vector2 position;
    //Vector2 direction;
    public Vector2 velocity = new(250,250);
    private readonly Texture2D ballTexture;
    
    const int radius = 15;
    
    private bool hasRun = true;
    private static Random rand = new Random();
    public Rectangle Hitbox => new Rectangle((int)position.X - radius, (int)position.Y - radius, 30, 30);



    
    public Ball(Texture2D _ballSprite)
    {
        ballTexture = _ballSprite;
    }


    public void BallUpdate(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (hasRun)
        {
            position.X = 401;//rand.Next(802); // sets x vector to a position betwen -1 or 1
            position.Y = 227;//rand.Next(0,455);
            hasRun = false;
        }



        // invert the axis of the ball
        position.X += velocity.X *dt;
        position.Y += velocity.Y* dt;

        //top screen
        if (position.Y <= 57)
        {
            velocity.Y = -velocity.Y;
            
        }

        //bottom screen
        if (position.Y >= 455)
        {
            velocity.Y = -velocity.Y;
        
        }
        //left screen
        if (position.X <= -100 ||position.X >= 900)
        {
            position.X  = 401;
        }


    }
    public void InvertAxis()
    {
                
        velocity.X = -velocity.X;
        
    }


    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(ballTexture, new Vector2(position.X - radius,position.Y - radius), Color.White);
    }



}
