using System.Collections.Generic;
using UnityEngine;

public class King : ChessPieces
{
    public override List<Vector2Int> GetAvilableMove(ref ChessPieces[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        // up
        if (currentY + 1 < tileCountY)
        {
            if (board[currentX, currentY + 1] == null)
            {
                r.Add(new Vector2Int(currentX, currentY + 1));
            }
            else
            {
                if (board[currentX, currentY + 1].team != team)
                {
                    r.Add(new Vector2Int(currentX, currentY + 1));
                }
            }
        }
        // down
        if (currentY - 1 >= 0)
        {
            if (board[currentX, currentY - 1] == null)
            {
                r.Add(new Vector2Int(currentX, currentY - 1));
            }
            else
            {
                if (board[currentX, currentY - 1].team != team)
                {
                    r.Add(new Vector2Int(currentX, currentY - 1));
                }
            }
        }
        // right
        if (currentX + 1 < tileCountX)
        {
            if (board[currentX + 1, currentY] == null)
            {
                r.Add(new Vector2Int(currentX + 1, currentY));
            }
            else
            {
                if (board[currentX + 1, currentY].team != team)
                {
                    r.Add(new Vector2Int(currentX + 1, currentY));
                }
            }
        }
        // left
        if (currentX - 1 >= 0)
        {
            if (board[currentX - 1, currentY] == null)
            {
                r.Add(new Vector2Int(currentX - 1, currentY));
            }
            else
            {
                if (board[currentX - 1, currentY].team != team)
                {
                    r.Add(new Vector2Int(currentX - 1, currentY));
                }
            }
        }
        // up right
        if (currentX + 1 < tileCountX && currentY + 1 < tileCountY)
        {
            if (board[currentX + 1, currentY + 1] == null)
            {
                r.Add(new Vector2Int(currentX + 1, currentY + 1));
            }
            else
            {
                if (board[currentX + 1, currentY + 1].team != team)
                {
                    r.Add(new Vector2Int(currentX + 1, currentY + 1));
                }
            }
        }
        // up left
        if (currentX - 1 >= 0 && currentY + 1 < tileCountY)
        {
            if (board[currentX - 1, currentY + 1] == null)
            {
                r.Add(new Vector2Int(currentX - 1, currentY + 1));
            }
            else
            {
                if (board[currentX - 1, currentY + 1].team != team)
                {
                    r.Add(new Vector2Int(currentX - 1, currentY + 1));
                }
            }
        }
        // down right
        if (currentX + 1 < tileCountX && currentY - 1 >= 0)
        {
            if (board[currentX + 1, currentY - 1] == null)
            {
                r.Add(new Vector2Int(currentX + 1, currentY - 1));
            }
            else
            {
                if (board[currentX + 1, currentY - 1].team != team)
                {
                    r.Add(new Vector2Int(currentX + 1, currentY - 1));
                }
            }
        }
        // down left
        if (currentX - 1 >= 0 && currentY - 1 >= 0)
        {
            if (board[currentX - 1, currentY - 1] == null)
            {
                r.Add(new Vector2Int(currentX - 1, currentY - 1));
            }
            else
            {
                if (board[currentX - 1, currentY - 1].team != team)
                {
                    r.Add(new Vector2Int(currentX - 1, currentY - 1));
                }
            }
        }
        return r;
    }

    public override SpecialMove GetSpecialMove(ref ChessPieces[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> avliableMove)
    {
        // Castling
        SpecialMove r = SpecialMove.None;

        var KingMove = moveList.FindAll(m => m[1].x == 4 && m[1].y == ((team == 0) ? 0 : 7));
        var leftRookMove = moveList.FindAll(m => m[1].x == 0 && m[1].y == ((team == 0) ? 0 : 7));
        var rightRookMove = moveList.FindAll(m => m[1].x == 7 && m[1].y == ((team == 0) ? 0 : 7));

        if (KingMove == null && currentX == 4)
        {
            //white team
            if (team == 0)
            {
                //left rook
                if (leftRookMove == null)
                {
                    if (board[0, 0].type == ChessPieceType.Rook)
                    {
                        if (board[0, 0].team == 0)
                        {
                            if (board[1, 0] == null && board[2, 0] == null && board[3, 0] == null)
                            {
                                avliableMove.Add(new Vector2Int(2, 0));
                                r = SpecialMove.Castling;
                            }
                        }

                    }
                }
                //right rook
                if (rightRookMove == null)
                {
                    if (board[7, 0].type == ChessPieceType.Rook)
                    {
                        if (board[7, 0].team == 0)
                        {
                            if (board[5, 0] == null && board[6, 0] == null)
                            {
                                avliableMove.Add(new Vector2Int(6, 0));
                                r = SpecialMove.Castling;
                            }
                        }

                    }
                }
            }
            //black team
            else
            {
                //left rook
                if (leftRookMove == null)
                {
                    if (board[0, 7].type == ChessPieceType.Rook)
                    {
                        if (board[0, 7].team == 1)
                        {
                            if (board[1, 7] == null && board[2, 7] == null && board[3, 7] == null)
                            {
                                avliableMove.Add(new Vector2Int(2, 7));
                                r = SpecialMove.Castling;
                            }
                        }

                    }
                }
                //right rook
                if (rightRookMove == null)
                {
                    if (board[7, 7].type == ChessPieceType.Rook)
                    {
                        if (board[7, 7].team == 1)
                        {
                            if (board[5, 7] == null && board[6, 7] == null)
                            {
                                avliableMove.Add(new Vector2Int(6, 7));
                                r = SpecialMove.Castling;
                            }
                        }

                    }
                }
            }
        }

        return r;
    }
}
