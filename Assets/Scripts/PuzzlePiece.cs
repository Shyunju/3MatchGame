using System.Collections;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UIElements.Experimental;

public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    
    //고유의의 값을 져 색을 판별한다.
    //상하좌우의 퍼즐들의 값이 같으면 파괴한다. 배열에 값을 전달할까?
    [Header("Board Variables")]
    [SerializeField] int column;
    public int Column {get {return column;} set {column = value;} }
    [SerializeField] int row;
    public int Row {get {return row;} set{row = value;} }
    [SerializeField] int previousColumn;
    [SerializeField] int previousRow;
    [SerializeField] float targetX;
    [SerializeField] float targetY;
    [SerializeField] bool isMatched = false;
    [SerializeField] float lerpValue;
    public bool IsMatched {get {return isMatched;} set {isMatched = value;} }

    [SerializeField] float swipeAngle = 0;
    [SerializeField] float swipeResist = .3f;
    [SerializeField] private int number;
    public int Number {get {return number;} }
    private GameManager gameManager;
    private FindMatches findMatches;
    private Board board;
    private GameObject otherDot;
    private Vector3 firstTouchPosition;
    public Vector3 FirstTouchPosition { get { return firstTouchPosition; } set { firstTouchPosition = value; } }

    private Vector3 finalTouchPosition;
    public Vector3 FinalTouchPosition { get { return finalTouchPosition; } set { finalTouchPosition = value; } }
    private float levelPositionX;
    public float LevelPositionX { get{return levelPositionX;}set { levelPositionX = value; } }
    private float levelPositionY;
    public float LevelPositionY {get{return levelPositionY;} set { levelPositionY = value; } }
    private Camera cam;
    private Vector3 tempPositon;

    private bool isPressed = false;

    private void Start()
    {
        board = FindFirstObjectByType<Board>();
        findMatches = FindFirstObjectByType<FindMatches>();
        gameManager = FindFirstObjectByType<GameManager>();
        cam = gameManager.Cam.GetComponent<Camera>();
        lerpValue = gameManager.lerpValueTest;
    }

    private void Update()
    {
        //FindMatches();
        ChaingeMatchedColor();
        MoveToTargetPosition();
    }
    void ChaingeMatchedColor()
    {
        if (isMatched)
        { //색 바꾸기
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, .2f);
        }
    }
    void MoveToTargetPosition()
    {
        //level1 = 2, 2.2         level2 = +1, 1,1            level3 = 0,0
        targetX = column + levelPositionX;
        targetY = row + levelPositionY;
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            tempPositon = new Vector3(targetX, transform.position.y, board.transform.position.z); //10f   +가중치를 주는 식
            transform.position = Vector3.Lerp(transform.position, tempPositon, lerpValue);
            if (board.PuzzleBoard[column, row] != this.gameObject)
            {
                board.PuzzleBoard[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            tempPositon = new Vector3(targetX, transform.position.y, board.transform.position.z);
            transform.position = tempPositon;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tempPositon = new Vector3(transform.position.x, targetY, board.transform.position.z);
            transform.position = Vector3.Lerp(transform.position, tempPositon, lerpValue);
            if (board.PuzzleBoard[column, row] != this.gameObject)
            {
                board.PuzzleBoard[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();

        }
        else
        {
            tempPositon = new Vector3(transform.position.x, targetY, board.transform.position.z);
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

    public void CalculateAngle() //터치 유효검사
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist){ 
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180/ Mathf.PI;
            MovePieces();
            gameManager.IsTuched = true;
            board.currentState = GameState.wait;
        }else{
            board.currentState = GameState.move;
        }
    }
    void MovePieces()
    {
        //스와이프한 기울기에 따라 방향을 판정하고 해당 방향의 퍼즐과 좌표를 바꿈
        if(swipeAngle > - 45 && swipeAngle <= 45 && column < board.Width-1)
        {
            //right swipe
            otherDot = board.PuzzleBoard[column + 1, row];
            otherDot.GetComponent<PuzzlePiece>().column -= 1;
            previousRow = row;
            previousColumn = column;
            column += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.Height-1)
        {
            //up swipe
            otherDot = board.PuzzleBoard[column, row+1];
            otherDot.GetComponent<PuzzlePiece>().row -= 1;
            previousRow = row;
            previousColumn = column;
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            //left swipe
            otherDot = board.PuzzleBoard[column - 1, row];
            otherDot.GetComponent<PuzzlePiece>().column += 1;
            previousRow = row;
            previousColumn = column;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //down swipe
            otherDot = board.PuzzleBoard[column, row-1];
            otherDot.GetComponent<PuzzlePiece>().row += 1;
            previousRow = row;
            previousColumn = column;
            row -= 1;
        }
        StartCoroutine(CheckMoveCo()); //매칭확인 코루틴

    }
    //void FindMatches() //좌우상하 비교 
    //{
    //    if(column > 0 && column < board.Width -1){
    //        GameObject leftDot1 = board.PuzzleBoard[column-1, row];
    //        GameObject rightDot1 = board.PuzzleBoard[column+1, row];
    //        if(leftDot1 != null && rightDot1 != null){
    //            if(leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag){
    //                leftDot1.GetComponent<PuzzlePiece>().isMatched = true;
    //                rightDot1.GetComponent<PuzzlePiece>().isMatched=true;
    //                isMatched = true;
    //            }
    //        }
    //    }
    //    if(row > 0 && row < board.Height -1){
    //        GameObject upDot1 = board.PuzzleBoard[column, row +1];
    //        GameObject downDot1 = board.PuzzleBoard[column, row -1];
    //        if(upDot1 != null && downDot1 != null){
    //            if(upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag){
    //                upDot1.GetComponent<PuzzlePiece>().isMatched = true;
    //                downDot1.GetComponent<PuzzlePiece>().isMatched=true;
    //                isMatched = true;
    //            }
    //        }
    //    }
    //}
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (board.currentState == GameState.move)
        {
            firstTouchPosition = cam.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 10f));
        }
    }

    

    public void OnEndDrag(PointerEventData eventData)
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPosition = cam.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 10f));
            CalculateAngle();
        }
    }



    public void OnDrag(PointerEventData eventData)
    {
        isPressed = true;
    }
}
