using System;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PongReborn;

public class PlayerController
{
    private Player player1;
    private Player player2;
    private float dt;
    private KeyboardState kState;

    public PlayerController(Player p1, Player p2)
    {
        player1 = p1;
        player2 = p2;
    }

    public void playerConUpdate(GameTime gameTime)
    {
        dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        kState = Keyboard.GetState();
        PlayerOneDirection();
        PlayerTwoDirection();

    }

    // input handler for player 1
    public void PlayerOneDirection()
    {

        if (kState.IsKeyDown(Keys.W) && player1.position.Y >= 120)
        {
            player1.position.Y -= Player.speed * dt;
            System.Console.WriteLine(player2.position.Y );

        }
        if (kState.IsKeyDown(Keys.S) && player1.position.Y<= 455)
        {
            player1.position.Y += Player.speed * dt;
        }

    }
    // input handler for player 2
    public void PlayerTwoDirection()
    {

        if (kState.IsKeyDown(Keys.I) && player2.position.Y >= 120)
        {
            player2.position.Y -= Player.speed * dt;
        }
        if (kState.IsKeyDown(Keys.K)&& player2.position.Y <= 455)
        {
            player2.position.Y += Player.speed * dt;
        }
        
    }

}
    
    

    



