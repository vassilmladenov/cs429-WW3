using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

public class Window : GameWindow
{
    private Game game;

    public Window(int width, int height, Game game)
        : base(width, height, GraphicsMode.Default, "WW3")
    {
        this.game = game;
        VSync = VSyncMode.On;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
        // GL.Enable(EnableCap.DepthTest);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadMatrix(ref projection);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        // not strictly necessary
        if (Keyboard[Key.Escape])
        {
            Exit();
        }
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GL.Ortho(0.0, World.WIDTH, 0.0, World.HEIGHT, 1.0, -1.0);

        game.Render();
        /*
        GL.MatrixMode(MatrixMode.Modelview);
        GL.Begin(PrimitiveType.Quads);

        // draw logic goes here
        GL.Color3(1.0f, 1.0f, 0.0f);
        GL.Vertex3(-1.0f, -1.0f, 4.0f);
        GL.Color3(1.0f, 0.0f, 0.0f);
        GL.Vertex3(1.0f, -1.0f, 4.0f);
        GL.Color3(0.2f, 0.9f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 4.0f);
        GL.Vertex3(-1.0f, 1.0f, 4.0f);

        GL.End();
        */
        SwapBuffers();
    }
}