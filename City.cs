using OpenTK.Graphics.OpenGL;

public class City
{
    public const float SIZE = 0.7f;

    public City(string name, int points)
    {
        Name = name;
        Points = points;
    }

    public string Name { get; }

    public int Points { get; }

    public void Render()
    {
        Color.GREEN.Use();
        float border = (1.0f - SIZE) / 2;
        GL.Rect(border, border, 1.0f - border, 1.0f - border);
    }
}