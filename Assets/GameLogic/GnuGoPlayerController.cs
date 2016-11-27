using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;

public class GnuGoPlayerController : PlayerController
{
    static Thread sEngineThread;
    static Queue<Move> sMoveList = new Queue<Move>();
    static Queue<Action> sRequestList = new Queue<Action>();
    private int sLastPlayedTurn = -1;
    private int sLastNotifiedTurn = -1;

    private static StreamReader _stdout;
    private static StreamWriter _stdin;
    
    bool _moveRequested;

    public GnuGoPlayerController()
    {
        if (sEngineThread == null)
        {
            sEngineThread = new Thread(EngineLoop);
            sEngineThread.Start();
        }
    }

    public override void NotifyMove(Move move, int moveNumber)
    {
        if (sLastNotifiedTurn < moveNumber)
        {
            sLastNotifiedTurn = moveNumber;
            lock (sRequestList)
            {
                sRequestList.Enqueue(() =>
                {
                    _stdin.WriteLine("play " + (move.player == Player.Black ? 'b' : 'w') + " " + MoveCoords(move));
                    _stdout.ReadLine();
                    _stdout.ReadLine();
                });
            }
        }
    }

    public void Start()
    {
    }

    public void Update()
    {
        if (GameController == null)
            return;

        // Pick up moves that have been played.
        lock (sMoveList)
        {
            while (sMoveList.Count > 0)
            {
                var move = sMoveList.Dequeue();
                GameController.PlayMove(move);
            }
        }

        // Request a move for current player
        if (GameController.CurrentTurn > sLastPlayedTurn && GameController.CurrentPlayer == ControlledPlayer)
        {
            lock (sRequestList)
            {
                sLastPlayedTurn = GameController.CurrentTurn;

                sRequestList.Enqueue(() =>
                {
                    _stdin.WriteLine("reg_genmove " + (ControlledPlayer == Player.Black ? 'b' : 'w'));
                    var move = TrimStuff(_stdout.ReadLine());
                    _stdout.ReadLine();

                    lock (sMoveList)
                    {
                        sMoveList.Enqueue(ParseMove(move, ControlledPlayer));
                    }

                });
            }
        }
    }

    private static void EngineLoop()
    {
        using (var process = Process.Start(new ProcessStartInfo()
        {
            Arguments = "--mode gtp",
            CreateNoWindow = true,
            FileName = "GnuGo/gnugo.exe",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
        }))
        {
            _stdout = process.StandardOutput;
            _stdin = process.StandardInput;

            _stdin.WriteLine();
            _stdout.ReadLine();
            _stdout.ReadLine();

            while (true)
            {
                Action a = null;
                lock (sRequestList)
                {
                    if (sRequestList.Count > 0)
                        a = sRequestList.Dequeue();
                }

                if (a != null) a();

                Thread.Sleep(10);
            }
        }
    }

    private static Move ParseMove(string coord, Player player)
    {
        return new Move()
        {
            x = coord[0] < 'I' ? coord[0] - 'A' : coord[0] - 'B',
            y = int.Parse(coord.Substring(1)) - 1,
            player = player
        };
    }

    private const string COORD_LETTERS = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
    private static string MoveCoords(Move move)
    {
        return COORD_LETTERS[move.x] + (move.y+1).ToString();
    }

    private static string TrimStuff(string v)
    {
        return v.Trim('=').Trim();
    }

    public override void Destroy()
    {
        sEngineThread.Abort();
        Destroy(gameObject);
    }
}