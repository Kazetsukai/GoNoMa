using System;
using UnityEngine;

public class GameController
{
    private GoBoard _board;
    private PlayerController _blackPlayer;
    private PlayerController _whitePlayer;

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

    public void SetPlayer(PlayerController controller, Player player)
    {
        if (player == Player.Black)
        {
            if (_blackPlayer != null) _blackPlayer.Destroy();
            _blackPlayer = controller;
        }
        else
        {
            if (_whitePlayer != null) _whitePlayer.Destroy();
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