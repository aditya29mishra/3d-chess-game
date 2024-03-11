using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessPieces
{
    public override List<Vector2Int> GetAvilableMove(ref ChessPieces[,] board, int tileCountX, int tileCountY){

        List<Vector2Int> r = new List<Vector2Int>();

        // up
        for (int i = currentY + 1; i < tileCountY; i++){
            if (board[currentX, i] == null){
                r.Add(new Vector2Int(currentX, i));
            }
            else{
                if(board[currentX, i].team != team){
                    r.Add(new Vector2Int(currentX,i));
                }
                break;
            }
        }
        // down
        for (int i = currentY - 1; i >= 0; i--){
            if (board[currentX, i] == null){
                r.Add(new Vector2Int(currentX, i));
            }
            else{
                if(board[currentX, i].team != team){
                    r.Add(new Vector2Int(currentX,i));
                }
                break;
            }
        }
        // right
        for (int i = currentX + 1; i < tileCountX; i++){
            if (board[i, currentY] == null){
                r.Add(new Vector2Int(i, currentY));
            }
            else{
                if(board[i, currentY].team != team){
                    r.Add(new Vector2Int(i,currentY));
                }
                break;
            }
        }
        // left
        for (int i = currentX - 1; i >= 0; i--){
            if (board[i, currentY] == null){
                r.Add(new Vector2Int(i, currentY));
            }
            else{
                if(board[i, currentY].team != team){
                    r.Add(new Vector2Int(i,currentY));
                }
                break;
            }
        }
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
