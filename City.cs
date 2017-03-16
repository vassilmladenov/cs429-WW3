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
}