using System.Collections;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

public class PuzzlePiece : MonoBehaviour
{
    //고유의의 값을 져 색을 판별한다.
    //상하좌우의 퍼즐들의 값이 같으면 파괴한다. 배열에 값을 전달할까?
    [Header("Board Variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    private FindMatches findMatches;
    private NewBoard board;
    private GameObject otherDot;
    private Vector3 firstTouchPosition;
    private Vector3 finalTouchPosition;
    public float swipeAngle = 0;
    public float swipeResist = .5f;
    private Camera cam;
    private Vector3 tempPositon;

    private void Start()
    {
        cam = GameObject.Find("MainCamera").GetComponent<Camera>();
        board = FindObjectOfType<NewBoard>();
        findMatches = FindObjectOfType<FindMatches>();
    }

    private void Update()
    {
        FindMatches();
        if(isMatched){ //색 바꾸기
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, .2f);
        }
        targetX = column; 
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            tempPositon = new Vector3(targetX, transform.position.y, 10f);
            transform.position = Vector3.Lerp(transform.position, tempPositon, .1f);
            if(board.puzzleBoard[column,row] != this.gameObject){
                board.puzzleBoard[column,row]=  this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            tempPositon = new Vector3(targetX, transform.position.y, 10f);
            transform.position = tempPositon;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tempPositon = new Vector3(transform.position.x, targetY, 10f);
            transform.position = Vector3.Lerp(transform.position, tempPositon, .1f);
            if(board.puzzleBoard[column,row] != this.gameObject){
                board.puzzleBoard[column,row]=  this.gameObject;
            }
            findMatches.FindAllMatches();

        }
        else
        {
            tempPositon = new Vector3(transform.position.x, targetY, 10f);
            transform.position = tempPositon;
        }
    }
    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.3f);
        if(otherDot != null){
             //두 퍼즐 모두 매칭되지 않으면 원래의 좌표로 되돌림
            if(!isMatched && !otherDot.GetComponent<PuzzlePiece>().isMatched){ 
                otherDot.GetComponent<PuzzlePiece>().row = row; 
                otherDot.GetComponent<PuzzlePiece>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(.3f);
                board.currentState = GameState.move;
            }
            else
            {
                board.DestroyMatches();
            }
            otherDot = null;
        }

    }
    private void OnMouseDown()
    {
        if(board.currentState == GameState.move)
            firstTouchPosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));  //모바일 주의(?)

    }
    private void OnMouseUp()
    {
        if(board.currentState == GameState.move){
            finalTouchPosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
            CalculateAngle();
        }
    }

    void CalculateAngle() //터치 유효검사
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist){ 
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180/ Mathf.PI;
            MovePieces();
            board.currentState = GameState.wait;
        }else{
            board.currentState = GameState.move;
        }
    }
    void MovePieces()
    {
        //스와이프한 기울기에 따라 방향을 판정하고 해당 방향의 퍼즐과 과표를 바꿈
        if(swipeAngle > - 45 && swipeAngle <= 45 && column < board.width-1)
        {
            //right swipe
            otherDot = board.puzzleBoard[column + 1, row];
            otherDot.GetComponent<PuzzlePiece>().column -= 1;
            previousRow = row;
            previousColumn = column;
            column += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height-1)
        {
            //up swipe
            otherDot = board.puzzleBoard[column, row+1];
            otherDot.GetComponent<PuzzlePiece>().row -= 1;
            previousRow = row;
            previousColumn = column;
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            //left swipe
            otherDot = board.puzzleBoard[column - 1, row];
            otherDot.GetComponent<PuzzlePiece>().column += 1;
            previousRow = row;
            previousColumn = column;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //down swipe
            otherDot = board.puzzleBoard[column, row-1];
            otherDot.GetComponent<PuzzlePiece>().row += 1;
            previousRow = row;
            previousColumn = column;
            row -= 1;
        }
        StartCoroutine(CheckMoveCo()); //매칭확인 코루틴

    }
    void FindMatches() //좌우상하 비교 
    {
        if(column > 0 && column < board.width -1){
            GameObject leftDot1 = board.puzzleBoard[column-1, row];
            GameObject rightDot1 = board.puzzleBoard[column+1, row];
            if(leftDot1 != null && rightDot1 != null){
                if(leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag){
                    leftDot1.GetComponent<PuzzlePiece>().isMatched = true;
                    rightDot1.GetComponent<PuzzlePiece>().isMatched=true;
                    isMatched = true;
                }
            }
        }
        if(row > 0 && row < board.height -1){
            GameObject upDot1 = board.puzzleBoard[column, row +1];
            GameObject downDot1 = board.puzzleBoard[column, row -1];
            if(upDot1 != null && downDot1 != null){
                if(upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag){
                    upDot1.GetComponent<PuzzlePiece>().isMatched = true;
                    downDot1.GetComponent<PuzzlePiece>().isMatched=true;
                    isMatched = true;
                }
            }
        }
    }

}
