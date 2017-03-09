using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OpenTK;

public class REPL
{
    private const string Commands = @"help|end|mv|quit|print|capture";
    private static readonly Regex Command = new Regex(@"(" + Commands + @")");
    private static readonly Regex Move = new Regex(@"mv (\d+) (\d+),(\d+)");
    private static readonly Regex Capture = new Regex(@"capture (\d+)");
    private readonly Game game;
    private readonly Window window;

    public REPL(Game game, Window window)
    {
        this.game = game;
        this.window = window;
    }

    public void Launch()
    {
        Console.WriteLine("Welcome to WW3 - type 'help' for instructions.");
        var originals = new Dictionary<Army, Pos>();

        bool running = true;
        do
        {
            Console.Write(game.CurrentPlayerIndex + "> ");
            var player = game.CurrentPlayer;

            var input = Console.ReadLine();
            var match = Command.Match(input);
            if (match.Success)
            {
                if (match.Value == "help")
                {
                    Console.WriteLine("fuck you");
                }
                else if (match.Value == "end")
                {
                    game.AdvancePlayer();
                    originals = new Dictionary<Army, Pos>();
                }
                else if (match.Value == "mv")
                {
                    var mv = Move.Match(input);
                    if (mv.Success)
                    {
                        var index = int.Parse(mv.Groups[1].Value);

                        var x = int.Parse(mv.Groups[2].Value);
                        var y = int.Parse(mv.Groups[3].Value);
                        var target = new Pos(x, y);

                        Console.WriteLine(" " + index + " " + x + " " + y);

                        if (player.CanMoveArmy(index, target))
                        {
                            player.MoveArmy(index, target);
                        }
                        else
                        {
                            Console.WriteLine("Illegal Movement");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Command must match: mv [0-9]+ [0-9]+,[0-9]+");
                    }
                }
                else if (match.Value == "capture")
                {
                    var capture = Capture.Match(input);
                    if (capture.Success)
                    {
                        var index = int.Parse(capture.Groups[1].Value);

                        if (player.ArmyExists(index))
                        {
                            var armyPosition = player.ArmyPosition(index);
                            var armyProvince = game.World.GetProvinceAt(armyPosition);
                            if (armyProvince.Owner != player)
                            {
                                armyProvince.Owner = player;
                                Console.WriteLine("Territory captured");
                            }
                            else
                            {
                                Console.WriteLine("Territory already controlled");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid Army Index");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Command must match: capture [0-9]+");
                    }
                }
                else if (match.Value == "quit")
                {
                    running = false;

                    try
                    {
                        window.Exit();
                    }
                    catch (System.ObjectDisposedException)
                    {
                        Console.WriteLine("Window already closed externally");
                    }
                }
                else if (match.Value == "print")
                {
                    game.Print();
                }
            }
            else
            {
                Console.WriteLine("Command must match: " + Commands);
            }
        }
        while (running);
    }
}