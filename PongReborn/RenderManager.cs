using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PongReborn;

public class RenderManager
{
    private readonly GraphicsDevice graphics;
    private readonly int screenWidth;
    private readonly int screenHeight;

    private readonly RenderTarget2D renderTarget;
    private Rectangle renderDestination;

    public RenderManager(GraphicsDevice graphics, int screenWidth, int screenHeight)
    {
        this.graphics = graphics;
        this.screenWidth = screenWidth;
        this.screenHeight = screenHeight;

        renderTarget = new RenderTarget2D(
            graphics,
            screenWidth,
            screenHeight
        );

        CalculateRenderDestination();
    }

    public void CalculateRenderDestination()
    {
        Point windowSize = graphics.Viewport.Bounds.Size;

        float scaleX = (float)windowSize.X / renderTarget.Width;
        float scaleY = (float)windowSize.Y / renderTarget.Height;

        // Use the smaller scale so the entire game remains visible.
        float scale = Math.Min(scaleX, scaleY);

        renderDestination.Width = (int)(renderTarget.Width * scale);
        renderDestination.Height = (int)(renderTarget.Height * scale);

        // Centre the game inside the window.
        renderDestination.X =
            (windowSize.X - renderDestination.Width) / 2;

        renderDestination.Y =
            (windowSize.Y - renderDestination.Height) / 2;
    }

    public void BeginRenderTarget()
    {
        graphics.SetRenderTarget(renderTarget);
    }

    public void EndRenderTarget()
    {
        graphics.SetRenderTarget(null);
    }

    public void DrawToScreen(SpriteBatch spriteBatch)
    {
        graphics.Clear(Color.Black);

        spriteBatch.Begin(
            samplerState: SamplerState.PointClamp
        );

        spriteBatch.Draw(
            renderTarget,
            renderDestination,
            Color.White
        );

        spriteBatch.End();
    }
}