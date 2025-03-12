using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
public class NewBoard : MonoBehaviour
{
    public int heigth;
    public int width;
    public GameObject tilePrefab;
    public GameObject[] dots;                                           //생성가능한 퍼즐 종류
    private BackgroundTile[,] allTiles;                                 //보드 기판
    public GameObject[,] puzzleBoard;                                   //실제 오브젝트가 들어있는 배열

    void Start()
    {
        puzzleBoard = new GameObject[width,heigth];
        allTiles = new BackgroundTile[width, heigth];
        SetUp();
    }

    private void SetUp()
    {
        for(int i = 0; i < width; ++i){
            for(int j = 0; j < heigth; ++j){
                Vector2 tempPosition = new Vector2(i,j);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition,Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                int dotToUse = Random.Range(0, dots.Length);
                
                int maxIterations = 0;
                //초기 퍼즐 보드 노 매칭 적합성 판단(현재 퍼즐로 생성되어 매칭된다면 다른 퍼즐로 변경 후 생성) + 무한루프 방지
                while(MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100){  
                    dotToUse = Random.Range(0, dots.Length);
                    maxIterations++;
                }
                //maxIterations = 0;
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                puzzleBoard[i,j] = dot;
                dot.transform.parent = this.transform;
            }
        }
    }
    private bool MatchesAt(int column, int row, GameObject piece) //왼쪽과 위쪽 기준 매칭이 되는지 검사
    {
        if(column > 1 && row > 1){
            if(puzzleBoard[column-1, row].tag == piece.tag && puzzleBoard[column-2, row].tag == piece.tag){
                return true;
            }
            if(puzzleBoard[column, row-1].tag == piece.tag && puzzleBoard[column, row-2].tag == piece.tag){
                return true;
            }
        }else if(column <= 1 || row <= 1){
            if(column > 1){
                if(puzzleBoard[column-1, row].tag == piece.tag && puzzleBoard[column-2, row].tag == piece.tag){
                    return true;
                }
            }
            if(row > 1){
                if(puzzleBoard[column, row -1].tag == piece.tag && puzzleBoard[column, row -2].tag == piece.tag){
                    return true;
                }
            }
        }
        
        return false;
    }
   private void DestroyMatchesAt(int column, int row)
    {
        if (puzzleBoard[column,row].GetComponent<PuzzlePiece>().isMatched) {
            Destroy(puzzleBoard[column, row]);
            puzzleBoard[column, row] = null;
        }
    }
    public void DestroyMatches()
    {
        for(int i = 0; i < width; ++i)
        {
            for(int j = 0; j < heigth; ++j)
            {
                if (puzzleBoard[i,j]  != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
    }
}
