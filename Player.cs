using System.Collections.Generic;

public class Player
{
    public Player()
    {
        this.ArmyList = new List<Army>();
    }

    private List<Army> ArmyList { get; set; }

    public void AddArmy(Army army, Pos pos)
    {
        // create new army object
        this.ArmyList.Add(new Army(pos, 100));
    }

    public void MoveArmy(Army army, Pos toPos)
    {
        // TODO: Change
    }
}