using System;
using System.Runtime.InteropServices;
using System.Text;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using PongReborn.Shared;

namespace PongReborn;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private RenderManager _renderManager;

    private bool countdownActive = true; // prevents nuormal game from running during count down

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

    public double timer = 120;

    public Song backSong;
    public SoundEffect effect;

    public bool gamePaused;

    // Internal game resolution
    private const int nativeWidth = 802;
    private const int nativeHeight = 502;

    // Networking
    public NetworkClient networkClient;

    private const string ServerIp = "ao-wat.tun.ply.gg";
    private const int ServerPort = 31414;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        // Internal game resolution
        _graphics.PreferredBackBufferWidth = nativeWidth;
        _graphics.PreferredBackBufferHeight = nativeHeight;

        _graphics.ApplyChanges();


        // Allow the player to resize the window
        Window.AllowUserResizing = true;

        // Recalculate the render destination whenever the window changes size
        Window.ClientSizeChanged += OnClientSizeChanged;
    }

    private void OnClientSizeChanged(object sender, EventArgs e)
    {
        if (_renderManager != null &&
            Window.ClientBounds.Width > 0 &&
            Window.ClientBounds.Height > 0)
        {
            _renderManager.CalculateRenderDestination();
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
    }



    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Create the render manager after GraphicsDevice exists
        _renderManager = new RenderManager(
            GraphicsDevice,
            nativeWidth,
            nativeHeight
        );

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
        MediaPlayer.Volume = 1.0f;

        // Instantiations
        playerOne = new Player(
            playerOneSprite,
            new Vector2(67, 117)
        );

        playerTwo = new Player(
            playerTwoSprite,
            new Vector2(769, 220)
        );

        playerController = new PlayerController(
            playerOne,
            playerTwo
        );

        ball = new Ball(ballSprite);

        networkClient = new NetworkClient();

        networkClient.OnGoalScored += _ => effect.Play();

        networkClient.Connect(
            ServerIp,
            ServerPort
        );
    }

    protected override void Update(GameTime gameTime)
    {
        networkClient.PollEvents();
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }
        // 2. If we are still waiting or counting down, stop here


        if (networkClient.MyPlayerNumber == 0)
        {
            base.Update(gameTime);
            return;
        }

        if (timer >= 1)
        {
            var kState = Keyboard.GetState();

            bool moveUp;
            bool moveDown;

            // Read the correct key pair depending on which side
            // the server assigned us.
            if (networkClient.MyPlayerNumber == 1)
            {
                moveUp = kState.IsKeyDown(Keys.Up);
                moveDown = kState.IsKeyDown(Keys.Down);
            }
            else
            {
                moveUp = kState.IsKeyDown(Keys.Up);
                moveDown = kState.IsKeyDown(Keys.Down);
            }

            // Send our input to the server every frame.
            networkClient.SendInput(moveUp, moveDown);

            // Local prediction
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (networkClient.MyPlayerNumber == 1)
            {
                if (moveUp && playerOne.position.Y >= 167)
                    playerOne.position.Y -= Player.speed * dt;

                if (moveDown && playerOne.position.Y <= 455)
                    playerOne.position.Y += Player.speed * dt;

                // Opponent is server-driven.
                playerTwo.position.Y = networkClient.Player2Y;
            }
            else
            {
                if (moveUp && playerTwo.position.Y >= 167)
                    playerTwo.position.Y -= Player.speed * dt;

                if (moveDown && playerTwo.position.Y <= 455)
                    playerTwo.position.Y += Player.speed * dt;

                playerOne.position.Y = networkClient.Player1Y;
            }

            // Ball is server-driven.
            ball.position.X = networkClient.BallX;
            ball.position.Y = networkClient.BallY;

            ball.velocity.X = networkClient.BallVelX;
            ball.velocity.Y = networkClient.BallVelY;

        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // Start rendering to the 802 × 502 render target.
        _renderManager.BeginRenderTarget();

        // Clear the internal game resolution.
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        // Background
        _spriteBatch.Draw(
            background,
            new Vector2(0, 47),
            Color.White
        );

        // Players
        playerOne.Draw(_spriteBatch);
        playerTwo.Draw(_spriteBatch);

        // Score bars
        _spriteBatch.Draw(
            scoreBar,
            Vector2.Zero,
            null,
            Color.White
        );

        _spriteBatch.Draw(
            scoreBarTwo,
            new Vector2(461, 0),
            null,
            Color.Blue,
            0f,
            Vector2.Zero,
            1f,
            SpriteEffects.FlipHorizontally,
            0f
        );

        // Ball
        ball.Draw(_spriteBatch);

        // Timer/status
        string statusText =
            networkClient.MyPlayerNumber == 0
                ? "CONNECTING..."
                : "TIME";

        _spriteBatch.DrawString(
            gameFont,
            statusText,
            new Vector2(370, 0),
            Color.DarkGray,
            0f,
            Vector2.Zero,
            0.3f,
            SpriteEffects.None,
            0f
        );

        //clock
        _spriteBatch.DrawString(
            gameFont,
            Math.Floor(networkClient.ClockTime).ToString(),
            new Vector2(382, 20),
            Color.White,
            0f,
            Vector2.Zero,
            0.4f,
            SpriteEffects.None,
            0f
        );

        if (!networkClient.isGamePlaying)
        {
            // couuntdown at start
            _spriteBatch.DrawString(
                gameFont,
                Math.Floor(networkClient.time).ToString(),
                new Vector2(401, 251),
                Color.Gold,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0f
            );
        }

        

        // Scores
        _spriteBatch.DrawString(
            gameFont,
            networkClient.Player1Score.ToString(),
            new Vector2(170, 10),
            Color.White,
            0f,
            Vector2.Zero,
            0.4f,
            SpriteEffects.None,
            0f
        );

        _spriteBatch.DrawString(
            gameFont,
            networkClient.Player2Score.ToString(),
            new Vector2(610, 10),
            Color.White,
            0f,
            Vector2.Zero,
            0.4f,
            SpriteEffects.None,
            0f
        );

        _spriteBatch.End();

        // Stop rendering to the render target.
        _renderManager.EndRenderTarget();

        // Scale the 802 × 502 game to the actual window.
        _renderManager.DrawToScreen(_spriteBatch);

        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        networkClient?.Disconnect();

        base.UnloadContent();
    }
}