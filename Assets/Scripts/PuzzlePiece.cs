using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    //고유의의 값을 져 색을 판별한다.
    //상하좌우의 퍼즐들의 값이 같으면 파괴한다. 배열에 값을 전달할까?
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int column;
    public int row;
    public int targetX;
    public int targetY;

    private NewBoard board;
    private GameObject otherDot;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    public float swipeAngle = 0;
    private Camera cam;
    private Vector2 tempPositon;

    private void Start()
    {
        cam = GameObject.Find("MainCamera").GetComponent<Camera>();
        board = FindObjectOfType<NewBoard>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        row = targetY;
        column = targetX;
    }

    private void Update()
    {
        targetX = column; 
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            tempPositon = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPositon, .4f);
        }
        else
        {
            tempPositon = new Vector2(targetX, transform.position.y);
            transform.position = tempPositon;
            board.puzzleBoard[column, row] = this.gameObject;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tempPositon = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPositon, .4f);
        }
        else
        {
            tempPositon = new Vector2(transform.position.x, targetY);
            transform.position = tempPositon;
            board.puzzleBoard[column, row] = this.gameObject;
        }
    }
    private void OnMouseDown()
    {
        firstTouchPosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));  //모바일 주의(?)
        //Debug.Log(firstTouchPosition);

    }
    private void OnMouseUp()
    {
        finalTouchPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }

    void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180/ Mathf.PI;
        MovePieces();
    }
    void MovePieces()
    {
        if(swipeAngle > - 45 && swipeAngle <= 45 && column < board.width)
        {
            //right swipe
            otherDot = board.puzzleBoard[column + 1, row];
            otherDot.GetComponent<PuzzlePiece>().column -= 1;
            column += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.heigth)
        {
            //up swipe
            otherDot = board.puzzleBoard[column, row+1];
            otherDot.GetComponent<PuzzlePiece>().row -= 1;
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            //left swipe
            otherDot = board.puzzleBoard[column - 1, row];
            otherDot.GetComponent<PuzzlePiece>().column += 1;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //down swipe
            otherDot = board.puzzleBoard[column, row-1];
            otherDot.GetComponent<PuzzlePiece>().row += 1;
            row -= 1;
        }

    }
    /*public enum Colors
    {
        red,
        blue,
        green,
        yelow,
        pink,
        sky
    }*/
    //public Colors color;

    //스왚하는 두개의 좌표로 매칭검사 (두번 호출)

}
