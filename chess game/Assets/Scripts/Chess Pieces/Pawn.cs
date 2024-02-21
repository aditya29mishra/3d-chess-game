using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPieces
{
    public override List<Vector2Int> GetAvilableMove(ref ChessPieces[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int direction = (team == 0) ? 1 : -1;

        // one in front

        if (board[currentX, currentY + direction] == null)
        {
            r.Add(new Vector2Int(currentX, currentY + direction));
        }
        // two in front
        if (board[currentX, currentY + direction] == null)
        {

            if (team == 0 && currentY == 1 && board[currentX, currentY + (direction * 2)] == null)
            {
                r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
            }
            else if (team == 1 && currentY == 6 && board[currentX, currentY + (direction * 2)] == null)
            {
                r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
            }
        }

        // take kill

        if (currentX != tileCountX - 1)
        {
            if (board[currentX + 1, currentY + direction] != null && board[currentX + 1, currentY + direction].team != team)
            {
                r.Add(new Vector2Int(currentX + 1, currentY + direction));
            }
        }
        if (currentX != 0)
        {
            if (board[currentX - 1, currentY + direction] != null && board[currentX - 1, currentY + direction].team != team)
            {
                r.Add(new Vector2Int(currentX - 1, currentY + direction));
            }
        }
        return r;
    }

    public override SpecialMove GetSpecialMove(ref ChessPieces[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> avliableMove)
    {
        int direction = (team == 0) ? 1 : -1;

        // Promotion 
        if ((team == 0 && currentY == 6) || (team == 1 && currentY == 1))
        {
            return SpecialMove.Promotion;
        }
        // En Passant
        if (moveList.Count > 0)
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1]; // get last move
            if (board[lastMove[1].x, lastMove[1].y].type == ChessPieceType.Pawn) // if last move was a pawn and it moved 2 tiles
            {
                if (Mathf.Abs(lastMove[0].y - lastMove[1].y) == 2)
                {
                    if (board[lastMove[1].x, lastMove[1].y].team != team) // if the pawn is not on the same team
                    {
                        if (lastMove[1].y == currentY) // if the pawn is on the same y axis
                        {
                            if (lastMove[1].x == currentX - 1) // if the pawn is next to this pawn
                            {
                                avliableMove.Add(new Vector2Int(currentX - 1, currentY + direction));
                                return SpecialMove.EnPassant;
                            }
                            if (lastMove[1].x == currentX + 1) // if the pawn is next to this pawn
                            {
                                avliableMove.Add(new Vector2Int(currentX + 1, currentY + direction));
                                return SpecialMove.EnPassant;
                            }
                        }
                    }
                }
            }
        }
        return SpecialMove.None;
    }
}
