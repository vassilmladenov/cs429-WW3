using System;

public class Army
{
    public const int DefaultMoveRange = 20;
    public const int DefaultRange = 50;

    private readonly int ammoToUpgradeRange = 50;
    private readonly int maxFoodCarried = 50;
    private readonly int maxHealth = 100;
    private int food;
    private int weapons;

    public Army(Pos position, int health)
    {
        Position = position;
        Health = health;
        Range = DefaultRange;
        MoveRange = DefaultMoveRange;
    }

    public Pos Position { get; set; }

    public int Range { get; private set; }

    public int MoveRange { get; private set; }

    public int Health { get; set; }

    public void FeedArmy()
    {
        // take from food stores to feed army/increase health
        int healthNeeded = this.maxHealth - this.Health;

        // increase health as much as possible
        if (this.food < healthNeeded)
        {
            this.Health = this.food;
            this.food = 0;
        }

        // max out health
        else
        {
            this.Health = this.maxHealth;
            this.food -= healthNeeded;
        }
    }

    public void UpdateResources(Province armyOn)
    {
        // call harvest from Province class based on position for food resources
        int harvested = armyOn.Gather(ResourceType.Food);
        int healthNeeded = this.maxHealth - this.Health;

        // everyhing we harvested is going to Health
        if (harvested <= healthNeeded)
        {
            this.Health = harvested;
        }

        // surplus harvest gets saved in Food stores
        else
        {
            this.Health = healthNeeded;

            // if we can't carry any more, only take what we can
            if ((this.food + (harvested - healthNeeded)) >= this.maxFoodCarried)
            {
                this.food = this.maxFoodCarried;
            }
            else
            {
                this.food = harvested - healthNeeded;
            }
        }

        // call harvest from Province class based on position for ammo resources
        this.weapons += armyOn.Gather(ResourceType.Weapons);

        // if enough Weapons has been acquired, upgrade range
        if (this.weapons >= this.ammoToUpgradeRange)
        {
            this.Range++;
            this.weapons -= this.ammoToUpgradeRange;
        }
    }

    public int DistanceTo(Pos target)
    {
        return Math.Abs(target.X - Position.X) + Math.Abs(target.Y - Position.Y);
    }
}
