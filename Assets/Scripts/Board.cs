using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    public GameObject[] pieces;//퍼즐조각
    [SerializeField]
    private Transform pieceParents; //보드

    private int[,] puzzleBoard = new int[8,5];

    private Vector2Int puzzleSize = new Vector2Int(5,8);  //퍼즐판 크기
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        SpawnPiece();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnPiece()
    {
        GameObject now;
        for(int y = 0; y < puzzleSize.y; ++y){
            for(int x = 0; x < puzzleSize.x; ++x){
                int num = Random.Range(0, pieces.Length);
                puzzleBoard[y,x] = num;
                now = Instantiate(pieces[num], pieceParents);
                now.GetComponent<PuzzlePiece>().color = num;
            }
        }
    }
    public void CallMatching(int col, int row, int cur)
    {
        // 현재 좌표기준 상하 혹은 좌우가 현재의 컬러와 같은 값이면 디스트로이
        if(HorizomMatching()) 
        {}
    }
    private bool HorizomMatching() //가로검사
    {
        return false;
    }
    private bool VirticalMatching()//세로검사
    {
        return false;
    }
}
