using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

public class GoBoard : MonoBehaviour
{

    public GameObject BoardLine;
    public int GridWidth = 19;
    public int GridHeight = 19;

    public GameObject BlackPiece;
    public GameObject WhitePiece;

    private double GridScaleZ = 0.2;
    private double GridScaleX = 0.2;

    private float BoardHeight;

    Thread _engineThread;

    Queue<Move> _moveList = new Queue<Move>();

    // Use this for initialization
    void Start()
    {
        var gobanMesh = transform.FindChild("goban");
        GridScaleZ = gobanMesh.localScale.z / 5f;
        GridScaleX = gobanMesh.localScale.x / 5f;
        BoardHeight = gobanMesh.GetComponent<MeshRenderer>().bounds.size.y + 0.22f * (float)GridScaleZ;
        for (int x = 0; x < GridWidth; x++)
        {
            var line = Instantiate(BoardLine);
            line.transform.parent = transform;
            line.transform.localPosition = Vector3.zero;
            var lineRenderer = line.GetComponent<LineRenderer>();

            lineRenderer.SetWidth((float)GridScaleZ * 0.1f, (float)GridScaleZ * 0.1f);
            lineRenderer.SetPosition(0, GetPosition(x, 0) - Vector3.up * 0.04f * (float)GridScaleZ);
            lineRenderer.SetPosition(1, GetPosition(x, GridHeight - 1) - Vector3.up * 0.04f * (float)GridScaleX);
        }
        for (int y = 0; y < GridHeight; y++)
        {
            var line = Instantiate(BoardLine);
            line.transform.parent = transform;
            line.transform.localPosition = Vector3.zero;
            var lineRenderer = line.GetComponent<LineRenderer>();

            lineRenderer.SetWidth((float)GridScaleZ*0.1f, (float)GridScaleZ*0.1f);
            lineRenderer.SetPosition(0, GetPosition(0, y) - Vector3.up * 0.04f * (float)GridScaleZ);
            lineRenderer.SetPosition(1, GetPosition(GridHeight - 1, y) - Vector3.up * 0.04f * (float)GridScaleX);
        }

        _engineThread = new Thread(EngineLoop);
        _engineThread.Start();
    }

    private void EngineLoop()
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

            var stdout = process.StandardOutput;
            var stdin = process.StandardInput;

            stdin.WriteLine();
            stdout.ReadLine();
            stdout.ReadLine();

            while (true)
            {
                stdin.WriteLine("genmove b");
                var black = TrimStuff(stdout.ReadLine());
                stdout.ReadLine();

                lock(_moveList)
                {
                    _moveList.Enqueue(ParseMove(black, Player.Black));
                }

                Thread.Sleep(500);

                stdin.WriteLine("genmove w");
                var white = TrimStuff(stdout.ReadLine());
                stdout.ReadLine();

                lock (_moveList)
                {
                    _moveList.Enqueue(ParseMove(white, Player.White));
                }
                Thread.Sleep(500);
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

    private string TrimStuff(string v)
    {
        return v.Trim('=').Trim();
    }

    private Vector3 GetPosition(int x, int y)
    {
        return new Vector3((float)((x + 0.5 - GridWidth / 2.0) * GridScaleX), BoardHeight, (float)((y + 0.5 - GridHeight / 2.0) * GridScaleZ));
    }

    // Update is called once per frame
    void Update()
    {
        lock (_moveList)
        {
            while (_moveList.Count > 0)
            {
                var move = _moveList.Dequeue();

                var piece = (GameObject)Instantiate(move.player == Player.Black ? BlackPiece : WhitePiece, Vector3.zero, BlackPiece.transform.rotation, transform);
                piece.transform.parent = transform;
                // Make black stones slightly thicker
                piece.transform.localScale = new Vector3(1, move.player == Player.Black ? 0.53f : 0.5f, 1) * (float)GridScaleX;
                piece.transform.localPosition = GetPosition(move.x, move.y) + Vector3.up * piece.transform.localScale.y / 2.4f;

            }
        }
    }

    private struct Move
    {
        public int x;
        public int y;
        public Player player;
    }

    private enum Player
    {
        Black,
        White
    }
}
