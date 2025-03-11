using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    public GameObject[] pieces;//퍼즐조각
    [SerializeField]
    private Transform pieceParents; //보드

    public int[,] puzzleBoard = new int[8,5];
    private GameObject now;
    public Vector2Int puzzleSize = new Vector2Int(5,8);  //퍼즐판 크기
    public HashSet<(int,int)> killList = new HashSet<(int,int)> ();  //삭제할 좌표목록
    private void Start()
    {
        for(int y = 0; y < puzzleSize.y; ++y){
            for(int x = 0; x < puzzleSize.x; ++x){
                SpawnPiece(x, y);
            }
        }
        gameManager.CheckingBoard();
    }

    void Update()
    {
        
    }

    void SpawnPiece(int x, int y)
    {
        int num = Random.Range(1, pieces.Length);
        puzzleBoard[y,x] = num;
        now = Instantiate(pieces[num], pieceParents);
        now.GetComponent<PuzzlePiece>().color = num;  
        now.GetComponent<PuzzlePiece>().y = y;
        now.GetComponent<PuzzlePiece>().x = x; 

        //gameManager.puzzle[y,x] = now;

    }
    public bool CallMatching(int col, int row, int cur)
    {
        // 현재 좌표기준 상하 혹은 좌우가 현재의 컬러와 같은 값이면 디스트로이
        int[] dirY = {0, 1, 0, -1, 0, 2, 0, -2};
        int[] dirX = {1, 0, -1, 0, 2, 0, -2, 0};
        bool matching = false;

        for(int i = 0; i < 4; ++i){
            if(col+dirY[i] < 0 || col + dirY[i] >= puzzleBoard.GetLength(0) || row + dirX[i] < 0 || row + dirX[i] >= puzzleBoard.GetLength(1))  continue;
            if(puzzleBoard[col +dirY[i], row+ dirX[i]] == cur){
                if(col+dirY[i+2] >= 0 && col + dirY[i+2] < puzzleBoard.GetLength(0) && row + dirX[i+2] >= 0 && row + dirX[i+2] < puzzleBoard.GetLength(1)){ //맞은편 검사

                    if(puzzleBoard[col + dirY[i+2], row + dirX[i+2]] == cur){
                        killList.Add((col+dirY[i], row+dirX[i]));
                        killList.Add((col+dirY[i+2], row + dirX[i+2]));
                        matching = true;
                    }
                }
                if(col+dirY[i+4] >= 0 && col + dirY[i+4] < puzzleBoard.GetLength(0) && row + dirX[i+4] >= 0 && row + dirX[i+4] < puzzleBoard.GetLength(1)){ //한칸더 검사

                    if(puzzleBoard[col + dirY[i+4],row+ dirX[i+4]] == cur){
                        killList.Add((col+dirY[i], row+dirX[i]));
                        killList.Add((col+dirY[i+4], row + dirX[i+4]));
                        matching = true;
                    }
                }
            }
        }
        return matching;

    }
    
    void DestroyPuzzle(){
        foreach(var puzzle in killList){
            //Debug.Log(gameManager.puzzle[puzzle.Item1, puzzle.Item2].GetComponent<PuzzlePiece>().color);
            //gameManager.puzzle[puzzle.Item1, puzzle.Item2] = pieces[0];
        }
    }
    
    
}
