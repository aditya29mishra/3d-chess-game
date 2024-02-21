using System.Collections.Generic;
using UnityEngine;

public class knight : ChessPieces
{
    public override List<Vector2Int> GetAvilableMove(ref ChessPieces[,] board, int tileCountX, int tileCountY){
        List<Vector2Int> r = new List<Vector2Int>();

        // up right

        if (currentX + 1 < tileCountX && currentY + 2 < tileCountY){
            if (board[currentX + 1, currentY + 2] == null){
                r.Add(new Vector2Int(currentX + 1, currentY + 2));
            }
            else{
                if(board[currentX + 1, currentY + 2].team != team){
                    r.Add(new Vector2Int(currentX + 1, currentY + 2));
                }
            }
        }
        // up left
        if (currentX - 1 >= 0 && currentY + 2 < tileCountY){
            if (board[currentX - 1, currentY + 2] == null){
                r.Add(new Vector2Int(currentX - 1, currentY + 2));
            }
            else{
                if(board[currentX - 1, currentY + 2].team != team){
                    r.Add(new Vector2Int(currentX - 1, currentY + 2));
                }
            }
        }
        // down right
        if (currentX + 1 < tileCountX && currentY - 2 >= 0){
            if (board[currentX + 1, currentY - 2] == null){
                r.Add(new Vector2Int(currentX + 1, currentY - 2));
            }
            else{
                if(board[currentX + 1, currentY - 2].team != team){
                    r.Add(new Vector2Int(currentX + 1, currentY - 2));
                }
            }
        }
        // down left
        if (currentX - 1 >= 0 && currentY - 2 >= 0){
            if (board[currentX - 1, currentY - 2] == null){
                r.Add(new Vector2Int(currentX - 1, currentY - 2));
            }
            else{
                if(board[currentX - 1, currentY - 2].team != team){
                    r.Add(new Vector2Int(currentX - 1, currentY - 2));
                }
            }
        }
        // right up
        if (currentX + 2 < tileCountX && currentY + 1 < tileCountY){
            if (board[currentX + 2, currentY + 1] == null){
                r.Add(new Vector2Int(currentX + 2, currentY + 1));
            }
            else{
                if(board[currentX + 2, currentY + 1].team != team){
                    r.Add(new Vector2Int(currentX + 2, currentY + 1));
                }
            }
        }
        // right down
        if (currentX + 2 < tileCountX && currentY - 1 >= 0){
            if (board[currentX + 2, currentY - 1] == null){
                r.Add(new Vector2Int(currentX + 2, currentY - 1));
            }
            else{
                if(board[currentX + 2, currentY - 1].team != team){
                    r.Add(new Vector2Int(currentX + 2, currentY - 1));
                }
            }
        }
        // left up
        if (currentX - 2 >= 0 && currentY + 1 < tileCountY){
            if (board[currentX - 2, currentY + 1] == null){
                r.Add(new Vector2Int(currentX - 2, currentY + 1));
            }
            else{
                if(board[currentX - 2, currentY + 1].team != team){
                    r.Add(new Vector2Int(currentX - 2, currentY + 1));
                }
            }
        }
        // left down
        if (currentX - 2 >= 0 && currentY - 1 >= 0){
            if (board[currentX - 2, currentY - 1] == null){
                r.Add(new Vector2Int(currentX - 2, currentY - 1));
            }
            else{
                if(board[currentX - 2, currentY - 1].team != team){
                    r.Add(new Vector2Int(currentX - 2, currentY - 1));
                }
            }
        }
        return r;       
    }
}
