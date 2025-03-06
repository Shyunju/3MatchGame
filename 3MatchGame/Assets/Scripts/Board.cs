using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    private GameObject piecePrefeb;//퍼즐조각
    [SerializeField]
    private Transform pieceParents; //보드

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
        for(int y = 0; y < puzzleSize.y; ++y){
            for(int x = 0; x < puzzleSize.x; ++x){
                Instantiate(piecePrefeb, pieceParents);
            }
        }
    }
}
