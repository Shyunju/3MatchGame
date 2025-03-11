using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    //고유의의 값을 져 색을 판별한다.
    //상하좌우의 퍼즐들의 값이 같으면 파괴한다. 배열에 값을 전달할까?
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Board  board = new Board();

    public int color=0;
    public int y;
    public int x;
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
