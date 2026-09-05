using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System;

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
    public double timer = 60;
    public double countdown;
    public int playerOneScore;
    public int playerTwoScore;
    public Song backSong;
    public SoundEffect effect;
    public bool gamePaused;

    // Tracks whether the ball was overlapping each paddle last frame, so a
    // bounce only fires once per contact instead of every frame of overlap.
    private bool wasCollidingP1;
    private bool wasCollidingP2;

    // Amount added to ball velocity on each paddle bounce (clamped in Ball.AddBounceSpeed).
    private static readonly Vector2 BounceSpeedBoost = new(15, 15);

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
    }

    protected override void Update(GameTime gameTime)
    {
        if (timer >= 1)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            playerController.playerConUpdate(gameTime);
            ball.BallUpdate(gameTime);

            // Discrete, one-shot scoring: award exactly one point per goal,
            // then reset the ball. No more fractional per-frame score growth.
            if (ball.position.X <= 0)
            {
                playerTwo.playerScore += 1;
                ball.Reset();
                ResetCollisionState();
            }
            else if (ball.position.X >= 802)
            {
                playerOne.playerScore += 1;
                ball.Reset();
                ResetCollisionState();
            }

            HandleBallCollisions();

            timer -= gameTime.ElapsedGameTime.TotalSeconds;
        }
        else
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
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

        _spriteBatch.DrawString(gameFont, "TIME", new Vector2(370, 0), Color.DarkGray, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);
        _spriteBatch.DrawString(gameFont, Math.Floor(timer).ToString(), new Vector2(382, 20), Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
        _spriteBatch.DrawString(gameFont, Math.Floor(playerOne.playerScore).ToString(), new Vector2(170, 10), Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
        _spriteBatch.DrawString(gameFont, Math.Floor(playerTwo.playerScore).ToString(), new Vector2(610, 10), Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    // Edge-triggered collision handling: only bounces the ball on the frame
    // contact *begins*, instead of every frame the hitboxes overlap. This
    // stops the velocity explosion that happened when InvertAxis() and the
    // speed boost fired repeatedly during a single overlap.
    private void HandleBallCollisions()
    {
        bool collidingP1 = playerOne.Hitbox.Intersects(ball.Hitbox);
        bool collidingP2 = playerTwo.Hitbox.Intersects(ball.Hitbox);

        if (collidingP1 && !wasCollidingP1)
        {
            ball.InvertAxis();
            ball.AddBounceSpeed(BounceSpeedBoost);
            effect.Play();
        }

        if (collidingP2 && !wasCollidingP2)
        {
            ball.InvertAxis();
            ball.AddBounceSpeed(BounceSpeedBoost);
            effect.Play();
        }

        wasCollidingP1 = collidingP1;
        wasCollidingP2 = collidingP2;
    }

    private void ResetCollisionState()
    {
        wasCollidingP1 = false;
        wasCollidingP2 = false;
    }
}