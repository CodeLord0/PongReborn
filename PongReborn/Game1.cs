using System;
using System.Data.Common;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
namespace PongReborn;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D background;
    private Texture2D playerOneSprite;
    private Texture2D playerTwoSprite;
    private Texture2D scoreBar;
    private Texture2D scoreBarTwo;
    public Player playerOne;
    public Player playerTwo;
    public PlayerController playerController;
    public Texture2D ballSprite;
    public Ball ball;
    public SpriteFont gameFont;
    public double timer = 80;
    public double countdown;
    public int playerOneScore;
    public int playerTwoScore;
    public Song backSong;
    public SoundEffect effect;
    public bool gamePaused;


    
    //private Player playerTwo;


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.PreferredBackBufferWidth = 802;
        _graphics.PreferredBackBufferHeight = 502;
        _graphics.ApplyChanges();

    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here


        //playerTwo = new Player()

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        background = Content.Load<Texture2D>("Board");
        playerOneSprite = Content.Load<Texture2D>("Player");
        playerTwoSprite = Content.Load<Texture2D>("Computer");
        ballSprite = Content.Load<Texture2D>("Ball");
        scoreBar = Content.Load<Texture2D>("ScoreBar");
        scoreBarTwo = Content.Load<Texture2D>("ScoreBar");
        gameFont = Content.Load<SpriteFont>("gameFont");
        effect = Content.Load<SoundEffect>("paddlesound");
        backSong = Content.Load<Song>("gameSong");
        MediaPlayer.Play(backSong);
        MediaPlayer.Volume = 0.4f;
        //instanciations
        playerOne = new Player(playerOneSprite, new Vector2(67, 117));
        playerTwo = new Player(playerTwoSprite, new Vector2(769, 220));
        playerController = new PlayerController(playerOne, playerTwo);
        ball = new Ball(ballSprite);
        

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        while(timer == 0)
        {
            
        }


        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        playerController.playerConUpdate(gameTime);
        ball.BallUpdate(gameTime);

        //player scores
        if (ball.position.X <= 0)
        {
            playerTwo.playerScore += gameTime.ElapsedGameTime.TotalSeconds*2;
        }

        if(ball.position.X >= 802)
        {
            playerOne.playerScore += gameTime.ElapsedGameTime.TotalSeconds*2;
        }
        if(OnBallCollsiion() == true)
        {
            ball.InvertAxis();

        }
        timer -= gameTime.ElapsedGameTime.TotalSeconds;




        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        
        _spriteBatch.Draw(background,new Vector2(0,47), Color.White);
        playerOne.Draw(_spriteBatch);
        playerTwo.Draw(_spriteBatch);


        _spriteBatch.Draw(
            scoreBar,
            Vector2.Zero,
            null,                 // source rectangle
            Color.White                   // layerDepth
        );

        _spriteBatch.Draw(
            scoreBarTwo,
            new Vector2(461, 0),
            null,                 // source rectangle
            Color.Blue,          // color
            0f,                   // rotation
            Vector2.Zero,         // origin
            1f,                   // scale
            SpriteEffects.FlipHorizontally, // effects
            0f                    // layerDepth
        );


        ball.Draw(_spriteBatch);

        _spriteBatch.DrawString(gameFont, "TIME",new Vector2(370,0),Color.DarkGray,0f,Vector2.Zero,0.3f,SpriteEffects.None,0f);
        _spriteBatch.DrawString(gameFont, Math.Floor(timer).ToString(),new Vector2(382,20),Color.White,0f,Vector2.Zero,0.4f,SpriteEffects.None,0f);
        _spriteBatch.DrawString(gameFont, Math.Floor(playerOne.playerScore).ToString(),new Vector2(170,10),Color.White,0f,Vector2.Zero,0.4f,SpriteEffects.None,0f);
        _spriteBatch.DrawString(gameFont, Math.Floor(playerTwo.playerScore).ToString(),new Vector2(610,10),Color.White,0f,Vector2.Zero,0.4f,SpriteEffects.None,0f);

        _spriteBatch.End();


        base.Draw(gameTime);
    }
    
    public bool OnBallCollsiion()
    {
        
        if(playerOne.Hitbox.Intersects(ball.Hitbox))
        {
            Console.WriteLine("true");
            ball.velocity += new Vector2(15,15);
            effect.Play();
            return true;

        }
        if(playerTwo.Hitbox.Intersects(ball.Hitbox))
        {
            Console.WriteLine("true");
            ball.velocity += new Vector2(15,15);
            effect.Play();
            return true;

        }
        return false;
    }
    
}
