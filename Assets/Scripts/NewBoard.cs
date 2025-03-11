using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
public class NewBoard : MonoBehaviour
{
    public GameManager gameManager;
    public int heigth;
    public int width;
    public GameObject tilePrefab;
    public GameObject[] dots;                                           //생성가능한 퍼즐 종류
    private BackgroundTile[,] allTiles;                                 //보드 기판
    public GameObject[,] puzzleBoard;                                   //실제 오브젝트가 들어있는 배열
    public int[,] colorBoard;                                           //컬러값이 들어가있는 배열
    public HashSet<(int,int)> killList = new HashSet<(int,int)> ();     //삭제할 좌표목록

    void Start()
    {
        puzzleBoard = new GameObject[width,heigth];
        colorBoard = new int[width,heigth];
        allTiles = new BackgroundTile[width, heigth];
        SetUp();
        gameManager.CheckingBoard();
    }

    private void SetUp()
    {
        for(int i = 0; i < width; ++i){
            for(int j = 0; j < heigth; ++j){
                Vector2 tempPosition = new Vector2(i,j);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition,Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                int dotToUse = Random.Range(0, dots.Length);
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                //dot.GetComponent<PuzzlePiece>().color = dotToUse;
                puzzleBoard[i,j] = dot;
                dot.transform.parent = this.transform;
            }
        }
    }
    public bool CallMatching(int col, int row, int cur)
    {
        // 현재 좌표기준 상하 혹은 좌우가 현재의 컬러와 같은 값이면 디스트로이
        int[] dirY = {0, 1, 0, -1, 0, 2, 0, -2};
        int[] dirX = {1, 0, -1, 0, 2, 0, -2, 0};
        bool matching = false;

        for(int i = 0; i < 4; ++i){
            if(col+dirY[i] < 0 || col + dirY[i] >= puzzleBoard.GetLength(0) || row + dirX[i] < 0 || row + dirX[i] >= puzzleBoard.GetLength(1))  continue;
            if(colorBoard[col +dirY[i], row+ dirX[i]] == cur){
                if(col+dirY[i+2] >= 0 && col + dirY[i+2] < puzzleBoard.GetLength(0) && row + dirX[i+2] >= 0 && row + dirX[i+2] < puzzleBoard.GetLength(1)){ //맞은편 검사

                    if(colorBoard[col + dirY[i+2], row + dirX[i+2]] == cur){
                        killList.Add((col+dirY[i], row+dirX[i]));
                        killList.Add((col+dirY[i+2], row + dirX[i+2]));
                        matching = true;
                    }
                }
                if(col+dirY[i+4] >= 0 && col + dirY[i+4] < puzzleBoard.GetLength(0) && row + dirX[i+4] >= 0 && row + dirX[i+4] < puzzleBoard.GetLength(1)){ //한칸더 검사

                    if(colorBoard[col + dirY[i+4],row+ dirX[i+4]] == cur){
                        killList.Add((col+dirY[i], row+dirX[i]));
                        killList.Add((col+dirY[i+4], row + dirX[i+4]));
                        matching = true;
                    }
                }
            }
        }
        return matching;

    }
}
