using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Net;
using LiteNetLib;
using LiteNetLib.Utils;
using PongReborn.Shared;
using System;
using System.Threading;


namespace PongRebornCopy;

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
    public double timer = 60;
    public double countdown;
    public Song backSong;
    public SoundEffect effect;
    public bool gamePaused;

    // Networking
    public NetworkClient networkClient;
    private const string ServerIp = "ao-wat.tun.ply.gg"; // swap for playit.gg address when hosting remotely
    private const int ServerPort = 31414;
    private bool wasConnected;

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
        MediaPlayer.Volume = 1.0f;

        // instantiations
        playerOne = new Player(playerOneSprite, new Vector2(67, 117));
        playerTwo = new Player(playerTwoSprite, new Vector2(769, 220));
        playerController = new PlayerController(playerOne, playerTwo);
        ball = new Ball(ballSprite);

        networkClient = new NetworkClient();
        networkClient.OnGoalScored += _ => effect.Play();
        networkClient.Connect(ServerIp, ServerPort);
    }

    protected override void Update(GameTime gameTime)
    {
        networkClient.PollEvents();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Wait until the server has told us which side we're playing before doing anything else.
        if (networkClient.MyPlayerNumber == 0)
        {
            base.Update(gameTime);
            return;
        }

        if (timer >= 1)
        {
            var kState = Keyboard.GetState();
            bool moveUp, moveDown;

            // Read the correct key pair depending on which side the server assigned us.
            if (networkClient.MyPlayerNumber == 1)
            {
                moveUp = kState.IsKeyDown(Keys.W);
                moveDown = kState.IsKeyDown(Keys.S);
            }
            else
            {
                moveUp = kState.IsKeyDown(Keys.I);
                moveDown = kState.IsKeyDown(Keys.K);
            }

            // Send our input to the server every frame - the server is the source of truth.
            networkClient.SendInput(moveUp, moveDown);

            // Local prediction: move our own paddle immediately so it feels responsive,
            // instead of waiting for the server's round-trip confirmation.
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (networkClient.MyPlayerNumber == 1)
            {
                if (moveUp && playerOne.position.Y >= 167) playerOne.position.Y -= Player.speed * dt;
                if (moveDown && playerOne.position.Y <= 455) playerOne.position.Y += Player.speed * dt;

                // Opponent (player two) is not predicted - always trust the server for them.
                playerTwo.position.Y = networkClient.Player2Y;
            }
            else
            {
                if (moveUp && playerTwo.position.Y >= 167) playerTwo.position.Y -= Player.speed * dt;
                if (moveDown && playerTwo.position.Y <= 455) playerTwo.position.Y += Player.speed * dt;

                playerOne.position.Y = networkClient.Player1Y;
            }

            // Ball is always server-driven - we never simulate it locally anymore.
            ball.position.X = networkClient.BallX;
            ball.position.Y = networkClient.BallY;
            ball.velocity.X = networkClient.BallVelX;
            ball.velocity.Y = networkClient.BallVelY;

            timer -= gameTime.ElapsedGameTime.TotalSeconds;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        _spriteBatch.Draw(background, new Vector2(0, 47), Color.White);
        playerOne.Draw(_spriteBatch);
        playerTwo.Draw(_spriteBatch);

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

        ball.Draw(_spriteBatch);

        string statusText = networkClient.MyPlayerNumber == 0 ? "CONNECTING..." : "TIME";
        _spriteBatch.DrawString(gameFont, statusText, new Vector2(370, 0), Color.DarkGray, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
        _spriteBatch.DrawString(gameFont, Math.Floor(timer).ToString(), new Vector2(382, 20), Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
        _spriteBatch.DrawString(gameFont, networkClient.Player1Score.ToString(), new Vector2(170, 10), Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
        _spriteBatch.DrawString(gameFont, networkClient.Player2Score.ToString(), new Vector2(610, 10), Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        networkClient?.Disconnect();
        base.UnloadContent();
    }
}