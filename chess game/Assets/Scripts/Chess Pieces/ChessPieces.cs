using System;
using System.Collections.Generic;
using UnityEngine;

public enum ChessPieceType
{
    None = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6
}

public class ChessPieces : MonoBehaviour
{

    public int team;
    public int currentX;
    public int currentY;
    public ChessPieceType type;

    private Vector3 DesiredPosition;
    private Vector3 DesiredScale = Vector3.one * 0.9f;

    private void Start()
    {
        transform.rotation = Quaternion.Euler((team == 0) ? Vector3.one * -90 : new Vector3(-90, 0, 0));
    }
    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, DesiredPosition, Time.deltaTime * 10);
        transform.localScale = Vector3.Lerp(transform.localScale, DesiredScale, Time.deltaTime * 10);
    }
    public virtual List<Vector2Int> GetAvilableMove(ref ChessPieces[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        r.Add(new Vector2Int(3, 3));
        r.Add(new Vector2Int(3, 4));
        r.Add(new Vector2Int(4, 3));
        r.Add(new Vector2Int(4, 4));
        return r;
    }
    public virtual SpecialMove GetSpecialMove(ref ChessPieces[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> avliableMove)
    {
        return SpecialMove.None;

    }
    public virtual void SetPosition(Vector3 position, bool force = false)
    {
        DesiredPosition = position;
        if (force)
        {
            transform.position = DesiredPosition;
        }
    }
    public virtual void setScale(Vector3 scale, bool force = false)
    {
        DesiredScale = scale;
        if (force)
        {
            transform.localScale = DesiredScale;
        }
    }

}
