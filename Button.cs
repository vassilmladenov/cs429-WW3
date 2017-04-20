public class Button
{
    public Button(int x, int y, int w, int h, string text, Color c)
    {
        X = x;
        Y = y;
        Width = w;
        Height = h;
        Text = text;
        Color = c;
    }

    public int X { get; set; }

    public int Y { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public string Text { get; set; }

    public Color Color { get; set; }

    public bool IsCursorInside(int cursorX, int cursorY)
    {
        return cursorX >= X && cursorY >= Y && cursorX <= X + Width && cursorY <= Y + Height;
    }
}