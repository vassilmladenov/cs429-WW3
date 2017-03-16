using System;

public class Army
{
    public const int DefaultMoveRange = 20;
    public const int DefaultRange = 50;

    private readonly int maxHealth = 100;

    public Army(Pos position, int health)
    {
        Position = position;
        Health = health;
        Range = DefaultRange;
        MoveRange = DefaultMoveRange;
    }

    public bool Moved { get; set; }

    public Pos Position { get; set; }

    public int Range { get; private set; }

    public int MoveRange { get; private set; }

    public int Health { get; private set; }

    public void FeedArmy(int food)
    {
        this.Health += food;
        this.Health = Math.Min(this.Health, maxHealth);
    }

    public void MoveTo(Pos target)
    {
        Position = target;
    }

    public bool CanMoveTo(Pos target)
    {
        return !Moved && DistanceTo(target) <= MoveRange;
    }

    public int DistanceTo(Pos target)
    {
        return Math.Abs(target.X - Position.X) + Math.Abs(target.Y - Position.Y);
    }

    public void Tick()
    {
        Moved = false;
        /* Just restore Moved for now, may change into attack code in the future (i.e. armies attack enemies and capture territory at the end of their turn) */
    }
}
