using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;

public enum GameState{
    wait,
    move
}
public class NewBoard : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int height;
    public int width;
    public int offSet;
    public GameObject tilePrefab;
    public GameObject[] dots;                                           //생성가능한 퍼즐 종류
    private BackgroundTile[,] allTiles;                                 //보드 기판
    public GameObject[,] puzzleBoard;                                   //실제 오브젝트가 들어있는 배열
    private FindMatches findMatches;

    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        puzzleBoard = new GameObject[width,height];
        allTiles = new BackgroundTile[width, height];
        SetUp();
    }

    private void SetUp()
    {
        for(int i = 0; i < width; ++i){
            for(int j = 0; j < height; ++j){
                Vector2 tempPosition = new Vector2(i,j + offSet);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
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
                dot.GetComponent<PuzzlePiece>().row = j;
                dot.GetComponent<PuzzlePiece>().column = i;
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
   private void DestroyMatchesAt(int column, int row) //좌표 받아서 매칭이 성공한 퍼즐이라면 파괴하고 배열에서 null처리
    {
        if (puzzleBoard[column,row].GetComponent<PuzzlePiece>().isMatched) {
            findMatches.currentMatches.Remove(puzzleBoard[column,row]);
            Destroy(puzzleBoard[column, row]);
            puzzleBoard[column, row] = null;
        }
    }
    public void DestroyMatches()
    {
        for(int i = 0; i < width; ++i)
        {
            for(int j = 0; j < height; ++j)
            {
                if (puzzleBoard[i,j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCo());
    }
    private IEnumerator DecreaseRowCo(){ //몇개를 채워야할지 파악하기
        int nullCount = 0;
        for(int i = 0; i < width; ++i){
            for(int j = 0; j < height; ++j){
                if(puzzleBoard[i,j] == null){
                    nullCount++;
                }else if(nullCount > 0){
                    puzzleBoard[i,j].GetComponent<PuzzlePiece>().row -= nullCount;
                    puzzleBoard[i,j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.5f);
        StartCoroutine(FillBoardCo());
    }
    private void RefillBoard(){
        for(int i = 0; i < width; ++i){
            for(int j = 0; j < height; ++j){
                if(puzzleBoard[i,j] == null){
                    Vector3 tempPosition = new Vector3(i,j+offSet, 10f);
                    int dotToUse =  Random.Range(0, dots.Length);
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity) as GameObject;
                    puzzleBoard[i,j] = piece;
                    piece.GetComponent<PuzzlePiece>().row = j;
                    piece.GetComponent<PuzzlePiece>().column = i;
                }
            }
        }
    }
    private bool MatchesOnBoard(){ //매칭된것이 있는지 확인
        for(int i = 0; i < width; ++i){
            for(int j = 0; j < height; ++j){
                if(puzzleBoard[i,j] != null){
                    if(puzzleBoard[i,j].GetComponent<PuzzlePiece>().isMatched){
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private IEnumerator FillBoardCo(){ 
        RefillBoard(); //빈자리 채우고
        yield return new WaitForSeconds(.5f);

        while(MatchesOnBoard()){ //새로 생서된게 바로 매칭이 되면 파괴하는 과정
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        yield return new WaitForSeconds(.5f);
        currentState = GameState.move;
    }
}
