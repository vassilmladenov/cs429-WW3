using System;
using System.Collections.Generic;

public class Game
{
    public Game()
    {
        // initialize World
        World = new World("maps.csv");

        // set up players list. for now, just add two players.
        Players = new List<Player>();
        Players.Add(new Player(new Color(1.0f, 0.0f, 0.0f)));
        Players.Add(new Player(new Color(0.0f, 0.0f, 1.0f)));
        Players[0].AddArmy(new Army(new Pos(0, 0), 10));
        Players[1].AddArmy(new Army(new Pos(1, 1), 10));
        CurrentPlayerIndex = 0;
    }

    public World World { get; private set; }

    public List<Player> Players { get; private set; }

    public Player CurrentPlayer
    {
        get
        {
            return Players[CurrentPlayerIndex];
        }
    }

    public int CurrentPlayerIndex { get; private set; }

    public void AdvancePlayer()
    {
        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;
    }

    public void Tick()
    {
        World.Tick();
        foreach (var player in Players)
        {
            player.Tick();
        }
    }

    public void Print()
    {
        for (int x = 0; x < World.WIDTH; x++)
        {
            for (int y = 0; y < World.HEIGHT; y++)
            {
                Province p = World.GetProvinceAt(new Pos(x, y));
                bool printed = false;
                foreach (var player in Players)
                {
                    var armies = player.ArmyList;
                    for (int j = 0; j < armies.Count; ++j)
                    {
                        if (armies[j].Position.Equals(new Pos(x, y)))
                        {
                            Console.Write(j);
                            printed = true;
                        }
                    }
                }

                if (!printed)
                {
                    Console.Write(p.City?.Name.Substring(0, 1) ?? "*");
                }
            }

            Console.WriteLine(string.Empty);
        }
    }
}