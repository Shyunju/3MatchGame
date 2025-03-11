using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    //고유의의 값을 져 색을 판별한다.
    //상하좌우의 퍼즐들의 값이 같으면 파괴한다. 배열에 값을 전달할까?
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    public float swipeAngle = 0;
    private Camera cam;

    private void Start()
    {
        cam = GameObject.Find("MainCamera").GetComponent<Camera>();
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
        Debug.Log(swipeAngle);
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
