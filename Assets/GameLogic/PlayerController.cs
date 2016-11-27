using UnityEngine;

public abstract class PlayerController : MonoBehaviour
{
    public Player ControlledPlayer;

    public GameController GameController { get; set; }

    public abstract void NotifyMove(Move move, int currentTurn);
    public abstract void Destroy();
}