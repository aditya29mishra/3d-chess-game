using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPieces
{
    public override List<Vector2Int> GetAvilableMove(ref ChessPieces[,] board, int tileCountX, int tileCountY){
        
        List<Vector2Int> r = new List<Vector2Int>();

        // up right
        for (int i = 1; currentX + i < tileCountX && currentY + i < tileCountY; i++){
            if (board[currentX + i, currentY + i] == null){
                r.Add(new Vector2Int(currentX + i, currentY + i));
            }
            else{
                if(board[currentX + i, currentY + i].team != team){
                    r.Add(new Vector2Int(currentX + i, currentY + i));
                }
                break;
            }
        }
        // up left
        for (int i = 1; currentX - i >= 0 && currentY + i < tileCountY; i++){
            if (board[currentX - i, currentY + i] == null){
                r.Add(new Vector2Int(currentX - i, currentY + i));
            }
            else{
                if(board[currentX - i, currentY + i].team != team){
                    r.Add(new Vector2Int(currentX - i, currentY + i));
                }
                break;
            }
        }
        // down right
        for (int i = 1; currentX + i < tileCountX && currentY - i >= 0; i++){
            if (board[currentX + i, currentY - i] == null){
                r.Add(new Vector2Int(currentX + i, currentY - i));
            }
            else{
                if(board[currentX + i, currentY - i].team != team){
                    r.Add(new Vector2Int(currentX + i, currentY - i));
                }
                break;
            }
        }
        // down left
        for (int i = 1; currentX - i >= 0 && currentY - i >= 0; i++){
            if (board[currentX - i, currentY - i] == null){
                r.Add(new Vector2Int(currentX - i, currentY - i));
            }
            else{
                if(board[currentX - i, currentY - i].team != team){
                    r.Add(new Vector2Int(currentX - i, currentY - i));
                }
                break;
            }
        }
        return r;
    }
}
