using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public enum GameState{
    wait,
    move
}
public class Board : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] AudioManager audioManager;
    [SerializeField] FindMatches findMatches;
    public GameState currentState = GameState.move;
    [SerializeField] int height;
    public int Height { get { return height; } }
    [SerializeField] int width;
    public int Width { get { return width; } }
    [SerializeField] int offSet;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject[] dots;                                 //생성가능한 퍼즐 종류
    private BackgroundTile[,] allTiles;                                 //보드 기판
    GameObject[,] puzzleBoard;                                   //실제 오브젝트가 들어있는 배열
    private int basicScore = 10;
    private int comboScore = 20;
    private int curLevel;
    private int dotRange;
    private int resetPenalty = -100;
    private float[] curLevelPositionX = {2f, 1f, 0f};
    private float[] curLevelPositionY = {2.2f, 1.1f, 0f};
    private (int, int)[] levelSize = new (int, int)[]
    {
        (4, 6),
        (6, 9),
        (8, 12)
    };

    public GameObject[,] PuzzleBoard { get {return puzzleBoard;}}                                   
    
    void Start()
    {
        //findMatches = FindObjectOfType<FindMatches>();
    }

    public void SetUp()
    {
        width = levelSize[curLevel].Item1;
        height = levelSize[curLevel].Item2;
        puzzleBoard = new GameObject[width,height];
        allTiles = new BackgroundTile[width, height];
        gameManager.GetComponent<GameManager>().IsPlaying = true;
        for(int i = 0; i < width; ++i){
            for(int j = 0; j < height; ++j){
                //Vector3 tempPosition = new Vector3(i+1,j + offSet + 0.6f, this.transform.position.z); //@@@@@@@@@@@@@@@@@@@@@
                Vector3 tempPosition = new Vector3(i,j + offSet, this.transform.position.z); 
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                int dotToUse = Random.Range(0, dotRange);
                
                int maxIterations = 0;
                //초기 퍼즐 보드 노 매칭 적합성 판단(현재 퍼즐로 생성되어 매칭된다면 다른 퍼즐로 변경 후 생성) + 무한루프 방지
                while(MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100){  
                    dotToUse = Random.Range(0, dotRange);
                    maxIterations++;
                }
                //maxIterations = 0;
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                PuzzlePiece curDot = dot.GetComponent<PuzzlePiece>();
                curDot.LevelPositionX = curLevelPositionX[curLevel];
                curDot.LevelPositionY = curLevelPositionY[curLevel];
                curDot.Row = j;
                curDot.Column = i;
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
        if (puzzleBoard[column,row].GetComponent<PuzzlePiece>().IsMatched) {
            findMatches.currentMatches.Remove(puzzleBoard[column,row]);
            if(findMatches.currentMatches.Count == 0) // 계산할 값(피연산자) 전달
            {
                audioManager.PlayMatchedSound();
                if(gameManager.curQState == GameManager.QueueState.empty && gameManager.IsTuched)
                {
                    //gameManager.numQueue.Enqueue(puzzleBoard[column,row].GetComponent<PuzzlePiece>().Number);
                    gameManager.FillNumberText(puzzleBoard[column,row].GetComponent<PuzzlePiece>().Number);
                }
            }
            Destroy(puzzleBoard[column, row]);
            puzzleBoard[column, row] = null;

            if(currentState == GameState.wait)
            {
                if(gameManager.ComboTime > 0.0f){
                    gameManager.ChaingeScore(comboScore);
                }else
                    gameManager.ChaingeScore(basicScore);
            }

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
        gameManager.IsTuched = false;
        StartCoroutine(DecreaseRowCo());
    }
    private IEnumerator DecreaseRowCo(){ //몇개를 채워야할지 파악하기
        int nullCount = 0;
        for(int i = 0; i < width; ++i){
            for(int j = 0; j < height; ++j){
                if(puzzleBoard[i,j] == null){
                    nullCount++;
                }else if(nullCount > 0){
                    puzzleBoard[i,j].GetComponent<PuzzlePiece>().Row -= nullCount;
                    puzzleBoard[i,j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.0f);
        StartCoroutine(FillBoardCo());
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
    private void RefillBoard(){
        for(int i = 0; i < width; ++i){
            for(int j = 0; j < height; ++j){
                if(puzzleBoard[i,j] == null){
                    Vector3 tempPosition = new Vector3(i,j+offSet, 10f);
                    int dotToUse =  Random.Range(0, dots.Length);
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity) as GameObject;
                    puzzleBoard[i,j] = piece;
                    piece.GetComponent<PuzzlePiece>().Row = j;
                    piece.GetComponent<PuzzlePiece>().Column = i;
                    piece.transform.parent = this.transform;

                }
            }
        }
    }
    private bool MatchesOnBoard(){ //매칭된것이 있는지 확인
        for(int i = 0; i < width; ++i){
            for(int j = 0; j < height; ++j){
                if(puzzleBoard[i,j] != null){
                    if(puzzleBoard[i,j].GetComponent<PuzzlePiece>().IsMatched){
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void DestroyAll()
    {
        if(currentState == GameState.move)
        {
            if(gameManager.CurLevel % 10 == 6){
                gameManager.ChaingeScore(resetPenalty);
            }
            for(int i = 0; i < width; ++i)
            {
                for(int j = 0; j<height; ++j)
                {
                    Destroy(puzzleBoard[i, j]);
                    puzzleBoard[i, j] = null;
                }
            }
            StartCoroutine(FillBoardCo());
        }
    }
    public void SettingPosition()
    {
        curLevel = gameManager.CurLevel / 100 -1;
        switch (gameManager.CurLevel / 100)
        {
            case 1:
                this.transform.position = new Vector3(this.transform.position.x, -2.4f, 1.6f);
                break;
            case 2:
                this.transform.position = new Vector3(0f, 0f, 5.6f);
                break;
            case 3:
                this.transform.position = new Vector3(this.transform.position.x, -2.2f, 10f);
                break;
            default:
                this.transform.position = new Vector3(this.transform.position.x, -2.2f, 10f);
                break;

        }
        SettingNumbers();
        SetUp();
    }
    private void SettingNumbers()
    {
        dotRange = (gameManager.CurLevel / 10) % 10;
    }
}
