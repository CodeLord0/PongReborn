using System.Data.Common;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace PongReborn;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D background;
    private Texture2D playerOneSprite;
    private Texture2D playerTwoSprite;
    public Player playerOne;
    public Player playerTwo;
    public PlayerController playerController;
    public Texture2D ballSprite;
    Ball ball;


    
    //private Player playerTwo;


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.PreferredBackBufferWidth = 802;
        _graphics.PreferredBackBufferHeight = 455;
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

        //instanciations
        playerOne = new Player(playerOneSprite, new Vector2(67, 117));
        playerTwo = new Player(playerTwoSprite, new Vector2(769, 220));
        playerController = new PlayerController(playerOne, playerTwo);
        ball = new Ball(ballSprite);
        

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        playerController.playerConUpdate(gameTime);
        ball.BallUpdate(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        _spriteBatch.Draw(background,Vector2.Zero, Color.White);
        playerOne.Draw(_spriteBatch);
        playerTwo.Draw(_spriteBatch);
        ball.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
