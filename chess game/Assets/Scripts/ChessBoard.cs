using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpecialMove
{
    None = 0,
    EnPassant,
    Castling,
    Promotion
}

public class ChessBoard : MonoBehaviour
{
    [Header("Art Stuff")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private float YOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private float deadSize = .5f;
    [SerializeField] private float deadSizing = 1f;
    [SerializeField] private float dragHeight = 0.1f;
    [Header("Prefabs and Materials")]
    [SerializeField] private GameObject[] Prefabs;
    [SerializeField] private Material[] teamMaterials;
    [SerializeField] private GameObject VictoryScreen;
    // logic

    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private ChessPieces currentlyDraging;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;
    private ChessPieces[,] chessPieces;
    private List<Vector2Int> avliableMove = new List<Vector2Int>();
    private List<ChessPieces> DeadWhiteTeam = new List<ChessPieces>();
    private List<ChessPieces> DeadBlackTeam = new List<ChessPieces>();
    private List<Vector2Int[]> moveList = new List<Vector2Int[]>();
    private bool isWhiteTurn;
    private SpecialMove specialMove;
    private void Awake()
    {
        isWhiteTurn = true;
        GenrateALLTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPiece();
        PositionAllPieces();
    }
    private void Update()
    {
        if (!currentCamera)
        {    // if we dont have a camera
            currentCamera = Camera.main;
            return;
        }
        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Higtlight")))
        {  // if we are hovering over a tile

            Vector2Int hitPosition = LookUpTileIndex(info.transform.gameObject);    // get the index of the tile we are hovering over

            if (currentHover == -Vector2Int.one)
            {   // if we were not hovering over a tile
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
            // if we were hovering over a tile , change the previous one 
            if (currentHover != hitPosition)
            {
                tiles[currentHover.x, currentHover.y].layer = (CantainValidMove(ref avliableMove, currentHover)) ? LayerMask.NameToLayer("Higtlight") : LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
            // if we press the mouse button
            if (Input.GetMouseButtonDown(0))
            {
                if (chessPieces[hitPosition.x, hitPosition.y] != null)
                {
                    // is it our turn
                    if ((chessPieces[hitPosition.x, hitPosition.y].team == 0 && isWhiteTurn) || (chessPieces[hitPosition.x, hitPosition.y].team == 1 && !isWhiteTurn))
                    {
                        currentlyDraging = chessPieces[hitPosition.x, hitPosition.y];
                        // get the list of where we can move , and highlight them
                        avliableMove = currentlyDraging.GetAvilableMove(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                        // get the  list of the Special moves
                        specialMove = currentlyDraging.GetSpecialMove(ref chessPieces, ref moveList, ref avliableMove);
                        PreventCheck();
                        HighLightTiles();
                    }
                }
            }
            // if we are releasing the mouse button
            if (currentlyDraging != null && Input.GetMouseButtonUp(0))
            {

                Vector2Int privosPosition = new Vector2Int(currentlyDraging.currentX, currentlyDraging.currentY);
                bool validMove = MoveTo(currentlyDraging, hitPosition.x, hitPosition.y);
                if (!validMove)  // if we cant move to the target position
                    currentlyDraging.SetPosition(GetTileCenter(privosPosition.x, privosPosition.y));
                currentlyDraging = null;

                currentlyDraging = null;
                RemoveHighLightTiles();
            }

        }

        else
        {
            if (currentHover != -Vector2Int.one)
            {   // if we were hovering over a tile
                tiles[currentHover.x, currentHover.y].layer = (CantainValidMove(ref avliableMove, currentHover)) ? LayerMask.NameToLayer("Higtlight") : LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }
            if (currentlyDraging != null && Input.GetMouseButtonUp(0))
            { // if we are releasing the mouse button 
                currentlyDraging.SetPosition(GetTileCenter(currentlyDraging.currentX, currentlyDraging.currentY));
                currentlyDraging = null;
                RemoveHighLightTiles();

            }
        }
        // if we are dragging a piece
        if (currentlyDraging)
        {
            Plane hroziontalPlane = new Plane(Vector3.up, Vector3.up * YOffset);
            float distance = 0.0f;
            if (hroziontalPlane.Raycast(ray, out distance))
            { // if we are hovering over the board
                currentlyDraging.SetPosition(ray.GetPoint(distance) + Vector3.up * dragHeight);
            }
        }

    }
    // genrate the board
    private void GenrateALLTiles(float tileSize, int TILE_COUNT_X, int TILE_COUNT_Y)
    {

        YOffset += transform.position.y;
        bounds = new Vector3((TILE_COUNT_X / 2) * tileSize, 0, (TILE_COUNT_Y / 2) * tileSize) + boardCenter;

        tiles = new GameObject[TILE_COUNT_X, TILE_COUNT_Y];
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                tiles[x, y] = GenrateSingleTile(tileSize, x, y);
            }
        }
    }
    private GameObject GenrateSingleTile(float tileSize, int x, int y)
    {

        GameObject tileObject = new GameObject(string.Format("X:{1} , Y:{1}", x, y));

        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        Vector3[] vertices = new Vector3[4];

        vertices[0] = new Vector3(x * tileSize, YOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, YOffset, (y + 1) * tileSize) - bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, YOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, YOffset, (y + 1) * tileSize) - bounds;

        int[] triangles = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");

        tileObject.AddComponent<BoxCollider>();

        return tileObject;

    }
    // spawning of the pieces
    private void SpawnAllPiece()
    {
        chessPieces = new ChessPieces[TILE_COUNT_X, TILE_COUNT_Y];

        int whiteTeam = 0, blackTeam = 1;

        chessPieces[0, 0] = spawningSinglePieces(ChessPieceType.Rook, whiteTeam);
        chessPieces[1, 0] = spawningSinglePieces(ChessPieceType.Knight, whiteTeam);
        chessPieces[2, 0] = spawningSinglePieces(ChessPieceType.Bishop, whiteTeam);
        chessPieces[3, 0] = spawningSinglePieces(ChessPieceType.Queen, whiteTeam);
        chessPieces[4, 0] = spawningSinglePieces(ChessPieceType.King, whiteTeam);
        chessPieces[5, 0] = spawningSinglePieces(ChessPieceType.Bishop, whiteTeam);
        chessPieces[6, 0] = spawningSinglePieces(ChessPieceType.Knight, whiteTeam);
        chessPieces[7, 0] = spawningSinglePieces(ChessPieceType.Rook, whiteTeam);

        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            chessPieces[x, 1] = spawningSinglePieces(ChessPieceType.Pawn, whiteTeam);
        }

        chessPieces[0, 7] = spawningSinglePieces(ChessPieceType.Rook, blackTeam);
        chessPieces[1, 7] = spawningSinglePieces(ChessPieceType.Knight, blackTeam);
        chessPieces[2, 7] = spawningSinglePieces(ChessPieceType.Bishop, blackTeam);
        chessPieces[3, 7] = spawningSinglePieces(ChessPieceType.Queen, blackTeam);
        chessPieces[4, 7] = spawningSinglePieces(ChessPieceType.King, blackTeam);
        chessPieces[5, 7] = spawningSinglePieces(ChessPieceType.Bishop, blackTeam);
        chessPieces[6, 7] = spawningSinglePieces(ChessPieceType.Knight, blackTeam);
        chessPieces[7, 7] = spawningSinglePieces(ChessPieceType.Rook, blackTeam);

        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            chessPieces[x, 6] = spawningSinglePieces(ChessPieceType.Pawn, blackTeam);
        }

    }
    private ChessPieces spawningSinglePieces(ChessPieceType type, int team)
    {
        ChessPieces cp = Instantiate(Prefabs[(int)type - 1], transform).GetComponent<ChessPieces>();

        cp.type = type;
        cp.team = team;
        cp.GetComponent<MeshRenderer>().material = teamMaterials[team];

        return cp;
    }
    // position of the pieces
    private void PositionAllPieces()
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {

                if (chessPieces[x, y] != null)
                {
                    PositionSinglePiece(x, y, true);
                }
            }
        }

    }
    private void PositionSinglePiece(int x, int y, bool force = false)
    {
        chessPieces[x, y].currentX = x;
        chessPieces[x, y].currentY = y;
        chessPieces[x, y].SetPosition(GetTileCenter(x, y), force);
    }
    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, YOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }
    // highliting of the pieces
    private void HighLightTiles()
    {
        for (int i = 0; i < avliableMove.Count; i++)
        {
            tiles[avliableMove[i].x, avliableMove[i].y].layer = LayerMask.NameToLayer("Higtlight");
        }
    }
    private void RemoveHighLightTiles()
    {
        for (int i = 0; i < avliableMove.Count; i++)
        {
            tiles[avliableMove[i].x, avliableMove[i].y].layer = LayerMask.NameToLayer("Tile");
        }
        avliableMove.Clear();
    }
    //cheakmate
    private void cheakmate(int team)
    {
        DisplayVictory(team);
    }
    private void DisplayVictory(int WinningTeam)
    {
        VictoryScreen.SetActive(true);
        VictoryScreen.transform.GetChild(WinningTeam).gameObject.SetActive(true);
    }
    public void OnResetbutton()
    {

        //UI
        VictoryScreen.transform.GetChild(0).gameObject.SetActive(false);
        VictoryScreen.transform.GetChild(1).gameObject.SetActive(false);
        VictoryScreen.SetActive(false);

        // Reset the board

        currentlyDraging = null;
        avliableMove.Clear();
        moveList.Clear();

        // Clean up

        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            for (int j = 0; j < TILE_COUNT_Y; j++)
            {
                if (chessPieces[i, j] != null)
                {
                    Destroy(chessPieces[i, j].gameObject);
                }
                chessPieces[i, j] = null;
            }
        }

        for (int i = 0; i < DeadWhiteTeam.Count; i++)
        {
            Destroy(DeadWhiteTeam[i].gameObject);
        }
        for (int i = 0; i < DeadBlackTeam.Count; i++)
        {
            Destroy(DeadBlackTeam[i].gameObject);
        }

        DeadWhiteTeam.Clear();
        DeadBlackTeam.Clear();

        SpawnAllPiece();
        PositionAllPieces();

        isWhiteTurn = true;
    }
    public void OnExitbutton()
    {
        Application.Quit();
    }
    // special moves
    private void ProcessSpecialMove()
    {
        //En Passant
        if (specialMove == SpecialMove.EnPassant)
        {
            var newmove = moveList[moveList.Count - 1];
            ChessPieces myPawn = chessPieces[newmove[1].x, newmove[1].y];
            var targetPawnPosition = moveList[moveList.Count - 2];
            ChessPieces enemyPawn = chessPieces[targetPawnPosition[1].x, targetPawnPosition[1].y];

            if (myPawn.currentX == enemyPawn.currentX)
            {
                if (myPawn.currentY == enemyPawn.currentY + 1 || myPawn.currentY == enemyPawn.currentY - 1)
                {
                    if (enemyPawn.team == 0)
                    {
                        DeadWhiteTeam.Add(enemyPawn);
                        enemyPawn.setScale(Vector3.one * deadSize);
                        enemyPawn.SetPosition(new Vector3(8 * tileSize, .25f, -1 * tileSize)
                            - bounds
                            + new Vector3(tileSize / 2, 0, tileSize / 2)
                            + (Vector3.forward * deadSizing) * DeadWhiteTeam.Count);
                    }
                    else if (enemyPawn.team == 1)
                    {
                        DeadBlackTeam.Add(enemyPawn);
                        enemyPawn.setScale(Vector3.one * deadSize);
                        enemyPawn.SetPosition(new Vector3(-1 * tileSize, .25f, 8 * tileSize)
                            - bounds
                            + new Vector3(tileSize / 2, 0, tileSize / 2)
                            + (Vector3.back * deadSizing) * DeadBlackTeam.Count);
                    }
                    chessPieces[enemyPawn.currentX, enemyPawn.currentY] = null;
                }
            }
        }
        // Promotion
        if (specialMove == SpecialMove.Promotion)
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];
            ChessPieces targetPawn = chessPieces[lastMove[1].x, lastMove[1].y];

            if (targetPawn.type == ChessPieceType.Pawn)
            {
                if (targetPawn.team == 0 && lastMove[1].y == 7)
                {    // white team
                    ChessPieces newQueen = spawningSinglePieces(ChessPieceType.Queen, 0);
                    newQueen.transform.position = chessPieces[lastMove[1].x, lastMove[1].y].transform.position;
                    Destroy(chessPieces[lastMove[1].x, lastMove[1].y].gameObject);
                    chessPieces[lastMove[1].x, lastMove[1].y] = newQueen;
                    PositionSinglePiece(lastMove[1].x, lastMove[1].y);
                }
                else if (targetPawn.team == 1 && lastMove[1].y == 0)
                {   // black team
                    ChessPieces newQueen = spawningSinglePieces(ChessPieceType.Queen, 1);
                    Destroy(chessPieces[lastMove[1].x, lastMove[1].y].gameObject);
                    chessPieces[lastMove[1].x, lastMove[1].y] = newQueen;
                    PositionSinglePiece(lastMove[1].x, lastMove[1].y);
                }

            }
        }
        // Castling
        if (specialMove == SpecialMove.Castling)
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];

            // left rook
            if (lastMove[1].x == 2)
            {
                if (lastMove[1].y == 0)
                { // white team
                    ChessPieces rook = chessPieces[0, 0];
                    chessPieces[3, 0] = rook;
                    PositionSinglePiece(3, 0);
                    chessPieces[0, 0] = null;
                }
                else if (lastMove[1].y == 7)
                { // black team
                    ChessPieces rook = chessPieces[0, 7];
                    chessPieces[3, 7] = rook;
                    PositionSinglePiece(3, 7);
                    chessPieces[0, 7] = null;
                }
            }
            else if (lastMove[1].x == 6)
            {
                if (lastMove[1].y == 0)
                { // white team
                    ChessPieces rook = chessPieces[7, 0];
                    chessPieces[5, 0] = rook;
                    PositionSinglePiece(5, 0);
                    chessPieces[7, 0] = null;
                }
                else if (lastMove[1].y == 7)
                { // black team
                    ChessPieces rook = chessPieces[7, 7];
                    chessPieces[5, 7] = rook;
                    PositionSinglePiece(5, 7);
                    chessPieces[7, 7] = null;
                }
            }
        }
    }
    private void PreventCheck()
    {
        ChessPieces tagetKing = null;
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null)
                {
                    if (chessPieces[x, y].type == ChessPieceType.King)
                    {
                        if (chessPieces[x, y].team == currentlyDraging.team)
                        {
                            tagetKing = chessPieces[x, y];
                        }
                    }
                }

            }
        }
        // Since we are sending the ref aviavle move , we will delete the moves that we putting us in check 
        SimilateMoveForSinglePiece(currentlyDraging, ref avliableMove, tagetKing);
    }
    private void SimilateMoveForSinglePiece(ChessPieces cp, ref List<Vector2Int> moves, ChessPieces targetKing)
    {
        // save the current value of the piece to reset the function call 
        int actualX = cp.currentX;
        int actualY = cp.currentY;
        List<Vector2Int> movesToRemove = new List<Vector2Int>();

        // Going throught all the moves , simulating them and checking if the king is in check

        for (int i = 0; i < moves.Count; i++)
        {
            int simX = moves[i].x;
            int simY = moves[i].y;

            Vector2Int kingpositionThisSim = new Vector2Int(targetKing.currentX, targetKing.currentY);
            // Did we simulate the king move 

            if (cp.type == ChessPieceType.King)
            {
                kingpositionThisSim = new Vector2Int(simX, simY);
            }

            // copy the [,] and not a ref
            ChessPieces[,] simulation = new ChessPieces[TILE_COUNT_X, TILE_COUNT_Y];
            List<ChessPieces> simAttackingPieces = new List<ChessPieces>();
            for (int x = 0; x < TILE_COUNT_X; x++)
            {
                for (int y = 0; y < TILE_COUNT_Y; y++)
                {
                    if (chessPieces[x, y] != null)
                    {
                        simulation[x, y] = chessPieces[x, y];
                        if (simulation[x, y].team != cp.team)
                        {
                            simAttackingPieces.Add(simulation[x, y]);
                        }
                    }
                }
            }

            // simulate the move 

            simulation[actualX, actualY] = null;
            cp.currentX = simX;
            cp.currentY = simY;

            simulation[simX, simY] = cp;

            //did one the piece got taken down during the sim

            var deadPiece = simAttackingPieces.Find(c => c.currentX == simX && c.currentY == simY);
            if (deadPiece != null)
            {
                simAttackingPieces.Remove(deadPiece);
            }

            // Get all the simulaterd attacking pieces moves

            List<Vector2Int> simMove = new List<Vector2Int>();
            for (int a = 0; a < simAttackingPieces.Count; a++)
            {
                var pieceMove = simAttackingPieces[a].GetAvilableMove(ref simulation, TILE_COUNT_X, TILE_COUNT_Y);
                for (int b = 0; b < pieceMove.Count; b++)
                {
                    simMove.Add(pieceMove[b]);
                }
            }

            // is the King in troble

            if (CantainValidMove(ref simMove, kingpositionThisSim))
            {
                movesToRemove.Add(moves[i]);
            }

            cp.currentX = actualX;
            cp.currentY = actualY;
        }

        // Remove form the current avliable move list
        for (int i = 0; i < movesToRemove.Count; i++)
        {
            moves.Remove(movesToRemove[i]);
        }
    }
    private bool checkForCheckmate()
    {
        var lastMove = moveList[moveList.Count - 1];
        int targetTeam = (chessPieces[lastMove[1].x, lastMove[1].y].team == 0) ? 1 : 0;

        List<ChessPieces> attackingPieces = new List<ChessPieces>();
        List<ChessPieces> defendingPieces = new List<ChessPieces>();

        ChessPieces tagetKing = null;
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null)
                {

                    if (chessPieces[x, y].team == targetTeam)
                    {
                        defendingPieces.Add(chessPieces[x, y]);
                        if(chessPieces[x,y].type == ChessPieceType.King)
                        {
                            tagetKing = chessPieces[x, y];
                        }
                    }
                    else
                    {
                        attackingPieces.Add(chessPieces[x, y]);
                    }

                }

            }

        }

        // is the king attacked right now?

        List<Vector2Int> currentAvilableMoves = new List<Vector2Int>();
        for (int a = 0; a < attackingPieces.Count; a++)
        {
            var pieceMove = attackingPieces[a].GetAvilableMove(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
            for (int b = 0; b < pieceMove.Count; b++)
            {
                currentAvilableMoves.Add(pieceMove[b]);
            }
        }
        // Are we in check now ?
        if(CantainValidMove(ref currentAvilableMoves , new Vector2Int(tagetKing.currentX, tagetKing.currentY)))
        {
            // king is under attack ? , can we move ? 

            for (int i = 0; i < defendingPieces.Count; i++)
            {
                List<Vector2Int> defendingMoves = defendingPieces[i].GetAvilableMove(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                // Since we are sending the ref aviavle move , we will delete the moves that we putting us in check 
                SimilateMoveForSinglePiece(defendingPieces[i], ref defendingMoves, tagetKing);

                if(defendingMoves.Count != 0)
                {
                    return false;
                }
            }
            return true; // checkMate Exit
        }

        return false;

    }
    // opration of the pieces
    private bool CantainValidMove(ref List<Vector2Int> moves, Vector2Int pos)
    {
        for (int i = 0; i < moves.Count; i++)
        {
            if (moves[i].x == pos.x && moves[i].y == pos.y)
            {
                return true;
            }
        }
        return false;
    }
    private bool MoveTo(ChessPieces cp, int x, int y)
    {
        if (!CantainValidMove(ref avliableMove, new Vector2Int(x, y)))
        {
            return false;
        }

        Vector2Int privosPosition = new Vector2Int(cp.currentX, cp.currentY);
        // is there another piece on the target position
        if (chessPieces[x, y] != null)
        {
            ChessPieces opponet = chessPieces[x, y];

            if (cp.team == opponet.team)
            {
                return false;
            }
            //if its the enemy team
            if (opponet.team == 0)
            {
                if (opponet.type == ChessPieceType.King)
                {
                    cheakmate(1);
                    Debug.Log("Black Team Wins");
                }
                DeadWhiteTeam.Add(opponet);
                opponet.setScale(Vector3.one * deadSize);
                opponet.SetPosition(new Vector3(8 * tileSize, .25f, -1 * tileSize) -
                bounds
                + new Vector3(tileSize / 2, 0, tileSize / 2)
                + (Vector3.forward * deadSizing) * DeadWhiteTeam.Count);
            }
            else
            {
                if (opponet.type == ChessPieceType.King)
                {
                    cheakmate(0);
                    Debug.Log("White Team Wins");
                }
                DeadBlackTeam.Add(opponet);
                opponet.setScale(Vector3.one * deadSize);
                opponet.SetPosition(new Vector3(-1 * tileSize, .25f, 8 * tileSize) -
                bounds
                + new Vector3(tileSize / 2, 0, tileSize / 2)
                + (Vector3.back * deadSizing) * DeadBlackTeam.Count);

            }

        }

        chessPieces[x, y] = cp;
        chessPieces[privosPosition.x, privosPosition.y] = null;

        PositionSinglePiece(x, y);

        isWhiteTurn = !isWhiteTurn;
        moveList.Add(new Vector2Int[] { privosPosition, new Vector2Int(x, y) });

        ProcessSpecialMove();

        if (checkForCheckmate())
        {
            cheakmate(cp.team);
        }

        return true;
    }
    private Vector2Int LookUpTileIndex(GameObject HitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (tiles[x, y] == HitInfo)
                {
                    return new Vector2Int(x, y);
                }

            }
        }
        return -Vector2Int.one;
    }
}
