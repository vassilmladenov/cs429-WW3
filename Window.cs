using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

public class Window : GameWindow
{
    public const int HUDPIXELHEIGHT = 100;
    public static readonly Color BACKGROUND = new Color(0.5f, 0.5f, 0.5f);
    public static readonly Color HUDBACKGROUND = new Color(0.09375f, 0.27f, 0.4f);
    public static readonly int BORDER = 2;

    private static string fontBitmapFilename = "font.bmp";
    private static int glyphsPerLine = 16;

    private static int glyphWidth = 11;
    private static int glyphHeight = 22;

    private static int charXSpacing = 11;

    private Game game;
    private int playerID;
    private Army army;
    private int clickFlag = 0; // 0: initial state, 1: army clicked, 2: Confirmation step, clicking on the same spot will decrement it
    private Pos pos;
    private float centerX;
    private float centerY;
    private float scale; // scale = pixels per world square

    private int fontTextureID;
    private int textureWidth;
    private int textureHeight;

    private Button endTurnButton;
    private Button makeArmyButton;

    public Window(int width, int height, Game game)
        : base(width, height, GraphicsMode.Default, "WW3")
    {
        this.game = game;
        scale = 15;
        centerX = World.WIDTH / 2;
        centerY = World.HEIGHT / 2;
        VSync = VSyncMode.On;
        endTurnButton = new Button(750, 30, 200, 50, "End Turn", Color.RED);
        makeArmyButton = new Button(400, 30, 150, 45, "Make Army", Color.BLUE);
    }

    public void Render(Button button)
    {
        button.Color.Use();
        GL.Rect(button.X, button.Y, button.X + button.Width, button.Y + button.Height);
        var textX = button.X + (button.Width / 2) - (button.Text.Length * glyphWidth / 2);
        var textY = button.Y + (button.Height / 2) - (glyphHeight / 2);
        Color.WHITE.Use();
        DrawText(textX, textY, button.Text);
    }

    public void Render(Army army)
    {
        float left = 0.25f;
        float right = 0.75f;
        float top = 0.75f;
        float bottom = 0.25f;

        var pos = game.Manager.ArmyPosition(army);
        GL.MatrixMode(MatrixMode.Modelview);
        GL.PushMatrix();
        GL.Translate(pos.X, pos.Y, 0);
        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

        DrawArmyTriangle(left, right, bottom, top);

        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
        GL.LineWidth(2.0f);
        Color.BLACK.Use();
        DrawArmyTriangle(left, right, bottom, top);

        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        GL.LineWidth(1.0f);
        GL.PopMatrix();
    }

    public void DrawText(int x, int y, string text)
    {
        GL.Enable(EnableCap.Blend);
        GL.Enable(EnableCap.Texture2D);
        GL.Begin(PrimitiveType.Quads);

        float u_step = (float)glyphWidth / (float)textureWidth;
        float v_step = (float)glyphHeight / (float)textureHeight;

        for (int n = 0; n < text.Length; n++)
        {
            char idx = text[n];
            float u = (float)(idx % glyphsPerLine) * u_step;
            float v = (float)(idx / glyphsPerLine) * v_step;

            GL.TexCoord2(u, v + v_step);
            GL.Vertex2(x, y);
            GL.TexCoord2(u + u_step, v + v_step);
            GL.Vertex2(x + glyphWidth, y);
            GL.TexCoord2(u + u_step, v);
            GL.Vertex2(x + glyphWidth, y + glyphHeight);
            GL.TexCoord2(u, v);
            GL.Vertex2(x, y + glyphHeight);

            x += charXSpacing;
        }

        GL.End();
        GL.Disable(EnableCap.Texture2D);
        GL.Disable(EnableCap.Blend);
    }

    public void Render(City city)
    {
        Color.GREEN.Use();
        float border = (1.0f - City.SIZE) / 2;
        GL.Rect(border, border, 1.0f - border, 1.0f - border);
    }

    public void Render(Province province)
    {
        Color c = province.Owner?.Color ?? new Color(0.5f, 0.5f, 0.5f);

        c.Use();
        GL.Rect(0.0f, 0.0f, 1.0f, 1.0f);

        if (province.City != null)
        {
            Render(province.City);
        }
    }

    public void Render(World world)
    {
        // render provinces
        for (int x = 0; x < World.WIDTH; x++)
        {
            for (int y = 0; y < World.HEIGHT; y++)
            {
                GL.MatrixMode(MatrixMode.Modelview);
                GL.PushMatrix();
                GL.Translate(x, y, 0);
                Render(world.GetProvinceAt(new Pos(x, y)));
                GL.PopMatrix();
            }
        }

        // render gridlines
        GL.Begin(PrimitiveType.Lines);
        Color.BLACK.Use();
        for (int x = 0; x < World.WIDTH; x++)
        {
            GL.Vertex2(x, 0);
            GL.Vertex2(x, World.HEIGHT);
        }

        for (int y = 0; y < World.HEIGHT; y++)
        {
            GL.Vertex2(0, y);
            GL.Vertex2(World.WIDTH, y);
        }

        GL.End();

        // render selected province
        GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
        GL.Color3(1.0, 0.5, 0.0);
        GL.Rect(pos.X, pos.Y, pos.X + 1, pos.Y + 1);
        GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
    }

    public void RenderHealth()
    {
        if (army != null)
        {
            var x = 30;
            var y = 30;
            var size = 45;
            var thickness = 10;
            Color.RED.Use();
            GL.Rect(x + (size / 2) - (thickness / 2), y, x + (size / 2) + (thickness / 2), y + size);
            GL.Rect(x, y + (size / 2) - (thickness / 2), x + size, y + (size / 2) + (thickness / 2));
            Color.BLACK.Use();

            var barX = 100;
            var barLength = 100;
            var barFilled = army.Health;
            GL.Rect(barX - BORDER, y - BORDER, barX + barLength + BORDER, y + size + BORDER);
            HUDBACKGROUND.Use();
            GL.Rect(barX, y, barX + barLength, y + size);
            Color.GREEN.Use();
            GL.Rect(barX, y, barX + barFilled, y + size);
        }
    }

    public void RenderHUD(World world)
    {
        float squareSize = 40;
        int foodOffset = 100;
        int gunOffset = 200;
        int cityTextOffset = 200;
        int resourceTextOffset = 350;

        // render HUD background
        GL.Viewport(0, 0, Width, HUDPIXELHEIGHT);
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GL.Ortho(0.0, Width, 0.0, HUDPIXELHEIGHT, 1, -1);
        HUDBACKGROUND.Use();
        GL.Rect(0, 0, Width, HUDPIXELHEIGHT);

        // render health bar
        RenderHealth();

        // render buttons
        Render(makeArmyButton);
        Render(endTurnButton);

        // render current player info
        Color.WHITE.Use();
        DrawText(0, 0, "Current Player: ");
        game.CurrentPlayer.Color.Use();
        DrawText("Current Player: ".Length * glyphWidth, 0, (game.CurrentPlayerIndex + 1).ToString());

        // render province info
        Province province = world.GetProvinceAt(pos);

        int passiveFood = province.PassiveResources.GetAmountOf(ResourceType.Food);
        int passiveWeapons = province.PassiveResources.GetAmountOf(ResourceType.Weapons);
        int activeFood = province.ActiveResources.GetAmountOf(ResourceType.Food);
        int activeWeapons = province.ActiveResources.GetAmountOf(ResourceType.Weapons);

        int food = passiveFood;
        int weapons = passiveWeapons;
        var right = Width;
        var foodX = right - foodOffset;
        var gunX = right - gunOffset;

        var squareY = (HUDPIXELHEIGHT / 2) - (squareSize / 2);
        int foodSideNum = (int)Math.Sqrt(food) + 1;
        int weaponsSideNum = (int)Math.Sqrt(weapons) + 1;
        float foodSize = squareSize / foodSideNum;
        float weaponSize = squareSize / weaponsSideNum;
        if (province != null && province.City != null)
        {
            Color.WHITE.Use();
            DrawText((Width / 2) + cityTextOffset, 30, province.City?.Name);
        }

        // food
        Color.BLACK.Use();
        GL.Rect(foodX - BORDER, squareY - BORDER, foodX + squareSize + BORDER, squareY + squareSize + BORDER);
        HUDBACKGROUND.Use();
        GL.Rect(foodX, squareY, foodX + squareSize, squareY + squareSize);

        Color.RED.Use();
        for (int i = 0; i < food; i++)
        {
            var x2 = foodX + ((i % foodSideNum) * foodSize);
            var y2 = squareY + ((i / foodSideNum) * foodSize);
            GL.Rect(x2 + 1, y2 + 1, x2 + foodSize - 1, y2 + foodSize - 1);
        }

        // weapons
        Color.BLACK.Use();
        GL.Rect(gunX - BORDER, squareY - BORDER, gunX + squareSize + BORDER, squareY + squareSize + BORDER);
        HUDBACKGROUND.Use();
        GL.Rect(gunX, squareY, gunX + squareSize, squareY + squareSize);
        new Color(0.8f, 0.6f, 0.0f).Use();
        for (int i = 0; i < weapons; i++)
        {
            var x2 = gunX + ((i % weaponsSideNum) * weaponSize);
            var y2 = squareY + ((i / weaponsSideNum) * weaponSize);
            GL.Rect(x2 + 1, y2 + 1, x2 + weaponSize - 1, y2 + weaponSize - 1);
        }

        // text
        Color.WHITE.Use();
        DrawText(right - resourceTextOffset, 30, "Resources");
    }

    public float GetLeft()
    {
        return centerX - (Width / 2 / scale);
    }

    public float GetRight()
    {
        return centerX + (Width / 2 / scale);
    }

    public float GetBottom()
    {
        return centerY - ((Height - HUDPIXELHEIGHT) / 2 / scale);
    }

    public float GetTop()
    {
        return centerY + ((Height - HUDPIXELHEIGHT) / 2 / scale);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        fontTextureID = LoadTexture(fontBitmapFilename);
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        endTurnButton.X = (Width / 2) - (endTurnButton.Width / 2);
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

        var left = GetLeft();
        var right = GetRight();
        var bottom = GetBottom();
        var top = GetTop();

        GL.Viewport(0, HUDPIXELHEIGHT, Width, Height - HUDPIXELHEIGHT);
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GL.Ortho(left, right, bottom, top, 1.0, -1.0);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        World world = game.GameWorld;
        Render(world);
        foreach (var player in game.Players)
        {
            player.Color.Use();
            foreach (var army in player.ArmyList)
            {
                // if an army's health is 0, they are dead. Remove the army and don't render
                if (army.Health == 0)
                {
                    player.RemoveArmy(army);
                }

                Render(army);
            }

            // RenderResources(player.ResourcesString())
        }

        // render HUD
        RenderHUD(world);

        SwapBuffers();
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        int mouseX = e.Position.X;
        int mouseY = Height - e.Position.Y - 1;
        Tuple<int, int> clickedPosition = CalculatePosition(e);
        int x = clickedPosition.Item1;
        int y = clickedPosition.Item2;
        Console.WriteLine("x is: " + x + " y is: " + y);
        Player player = game.CurrentPlayer;

        // check if the end turn button was clicked
        if (endTurnButton.IsCursorInside(mouseX, mouseY))
        {
            Console.WriteLine("Turn Ended");
            game.EndTurn();
            clickFlag = 0;
        }

        if (clickFlag == 0 && World.IsInBounds(new Pos(x, y)))
        {
            playerID = game.CurrentPlayerIndex;
            var prevPos = new Pos(pos.X, pos.Y);
            pos = new Pos(x, y);
            army = game.Manager.ArmyAt(pos);
            if (army != null && player.ArmyList.Contains(army))
            {
                Console.WriteLine("Army clicked.");
                clickFlag = 1;
            }
        }
        else if (clickFlag == 0 && makeArmyButton.IsCursorInside(mouseX, mouseY))
        {
            var prevPos = new Pos(pos.X, pos.Y);
            pos = new Pos(prevPos.X, prevPos.Y);
            if (game.Manager.ArmyAt(pos) == null)
            {
                Console.WriteLine("Army created.");
                player.AddArmy(new Army(Army.InitialHealth), pos);
            }
            else
            {
                Console.WriteLine("Army already exists at position");
            }
        }
        else if ((clickFlag == 1 || clickFlag == 2) && World.IsInBounds(new Pos(x, y)))
        {
            pos = new Pos(x, y);
            if (game.Manager.ArmyPosition(army).Equals(pos))
            {
                Console.WriteLine("Army unselected");
                clickFlag = 0;
            }
            else if (game.Manager.CanMoveTo(army, pos) == true)
            {
                Console.WriteLine("Press 'y' now to confirm move or 'u' to undo the move");
                clickFlag = 2;
            }
            else
            {
                Console.WriteLine("Invalid move. Try again.");
            }
        }
    }

    protected override void OnKeyPress(OpenTK.KeyPressEventArgs e)
    {
        switch (e.KeyChar)
        {
            case 'y':
                if (clickFlag == 2)
                {
                    game.Manager.MoveArmy(army, pos);
                    Console.WriteLine("Army has moved.");
                    clickFlag = 0;
                }

                break;
            case 'u':
                if (clickFlag == 2)
                {
                    Console.WriteLine("Army has been moved back to its previous position");
                    clickFlag = 0;
                }

                break;
            case '+':
                scale *= 1.1f;
                break;
            case '-':
                scale /= 1.1f;
                break;
            case 'w':
                centerY += 1;
                break;
            case 'a':
                centerX -= 1;
                break;
            case 'd':
                centerX += 1;
                break;
            case 's':
                centerY -= 1;
                break;
        }
    }

    private Tuple<int, int> CalculatePosition(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        var a = Mouse.GetCursorState();
        int mouseX = e.Position.X;
        int mouseY = Height - e.Position.Y - 1;

        float left = GetLeft();
        float right = GetRight();
        float bottom = GetBottom();
        float top = GetTop();

        float worldX = left + (((float)mouseX / Width) * (right - left));
        float worldY = bottom + (((float)(mouseY - HUDPIXELHEIGHT) / (Height - HUDPIXELHEIGHT)) * (top - bottom));

        int x = (int)worldX;
        int y = (int)worldY;

        return Tuple.Create(x, y);
    }

    private void DrawArmyTriangle(float left, float right, float bottom, float top)
    {
        GL.Begin(PrimitiveType.Triangles);
        GL.Vertex2(right, bottom);
        GL.Vertex2(0.5f, top);
        GL.Vertex2(left, bottom);
        GL.End();
    }

    private int LoadTexture(string filename)
    {
        using (var bitmap = new System.Drawing.Bitmap(filename))
        {
            var texId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, fontTextureID);
            System.Drawing.Imaging.BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            textureWidth = bitmap.Width;
            textureHeight = bitmap.Height;
            return texId;
        }
    }
}