using OpenTK.Graphics.OpenGL;

public struct Color
{
    public static readonly Color RED = new Color(1.0f, 0.0f, 0.0f);
    public static readonly Color BLUE = new Color(0.0f, 0.0f, 1.0f);
    public static readonly Color GREEN = new Color(0.0f, 1.0f, 0.0f);
    public static readonly Color BLACK = new Color(0.0f, 0.0f, 0.0f);
    public static readonly Color WHITE = new Color(1.0f, 1.0f, 1.0f);

    public float R;
    public float G;
    public float B;

    public Color(float r, float g, float b)
    {
        R = r;
        G = g;
        B = b;
    }

    public void Use()
    {
        GL.Color3(R, G, B);
    }
}