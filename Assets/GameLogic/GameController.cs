using System;
using UnityEngine;

public class GameController
{
    private GoBoard _board;
    private GnuGoPlayerController _blackPlayer;
    private GnuGoPlayerController _whitePlayer;

    public Player CurrentPlayer { get; private set; }
    public int CurrentTurn { get; internal set; }

    public GameController(GoBoard board)
    {
        _board = board;
        CurrentTurn = 0;
    }
    
    public void PlayMove(Move move)
    {
        if (move.player != CurrentPlayer)
            throw new System.Exception("Hold up mate, it isn't your turn!");

        _board.ShowMove(move);

        _blackPlayer.NotifyMove(move, CurrentTurn);
        _whitePlayer.NotifyMove(move, CurrentTurn);

        SwapTurn();
    }

    public void SetPlayer(GnuGoPlayerController controller, Player player)
    {
        if (player == Player.Black)
        {
            GameObject.Destroy(_blackPlayer);
            _blackPlayer = controller;
        }
        else
        {
            GameObject.Destroy(_whitePlayer);
            _whitePlayer = controller;
        }

        controller.GameController = this;
    }

    private void SwapTurn()
    {
        CurrentPlayer = CurrentPlayer == Player.Black ? Player.White : Player.Black;
        CurrentTurn++;
    }
}