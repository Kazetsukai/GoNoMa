using UnityEngine;
using System.Threading;
using System.Diagnostics;

public class GoBoard : MonoBehaviour
{

    public GameObject BoardLine;
    public int GridWidth = 19;
    public int GridHeight = 19;

    public GnuGoPlayerController BlackPlayer;
    public GnuGoPlayerController WhitePlayer;

    public GameObject BlackPiece;
    public GameObject WhitePiece;

    private double GridScaleZ = 0.2;
    private double GridScaleX = 0.2;

    private float BoardHeight;

    private GameController _gameController;
    public GameController GameController { get { return _gameController; } }
    
    public GoBoard()
    {
    }

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

        _gameController = new GameController(this);
        _gameController.SetPlayer(BlackPlayer, Player.Black);
        _gameController.SetPlayer(WhitePlayer, Player.White);
    }
    
    void Update()
    {
    }

    public void ShowMove(Move move)
    {
        var piece = (GameObject)Instantiate(move.player == Player.Black ? BlackPiece : WhitePiece, Vector3.zero, BlackPiece.transform.rotation, transform);
        piece.transform.parent = transform;
        // Make black stones slightly thicker
        piece.transform.localScale = new Vector3(1, move.player == Player.Black ? 0.53f : 0.5f, 1) * (float)GridScaleX;
        piece.transform.localPosition = GetPosition(move.x, move.y) + Vector3.up * piece.transform.localScale.y / 2.4f;
    }

    private Vector3 GetPosition(int x, int y)
    {
        return new Vector3((float)((x + 0.5 - GridWidth / 2.0) * GridScaleX), BoardHeight, (float)((y + 0.5 - GridHeight / 2.0) * GridScaleZ));
    }
}
