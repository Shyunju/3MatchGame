using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    public GameObject test;
    public NewBoard board;
    //public GameObject[,] puzzle = new GameObject[8,5];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CheckingBoard()
    {
        for(int y = 0; y < board.width; ++y){
            for(int x = 0; x < board.heigth; ++x){
                int num = board.colorBoard[y,x];
                if(board.CallMatching(y,x,num)){}
                    DestroyPuzzle();
                //Debug.Log("체킹실행");
            }
        }
    }
    void DestroyPuzzle(){
        foreach(var i in board.killList){
            board.colorBoard[i.Item1, i.Item2] = -1;
            board.puzzleBoard[i.Item1, i.Item2].SetActive(false);
            Destroy(board.puzzleBoard[i.Item1, i.Item2]);
        }

        //Vector2 tempPosition = new Vector2(i,j); //     생성 스크립트
        //GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
    }
}
