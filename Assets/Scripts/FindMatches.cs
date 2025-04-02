using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    [SerializeField] Board board;
    private GameManager gameManager;
    public List<GameObject> currentMatches = new List<GameObject>();
    void Start()
    {
        //board = FindObjectOfType<Board>();   
    }
    public void FindAllMatches(){
        StartCoroutine(FindAllMatchesCo());
    }
    private IEnumerator FindAllMatchesCo(){
        yield return new WaitForSeconds(.2f);
        for(int i = 0; i < board.Width; ++i){
            for(int j = 0; j < board.Height; ++j){
                GameObject currentDot = board.PuzzleBoard[i,j];
                if(currentDot != null){
                    if(i > 0 && i < board.Width-1){
                        GameObject leftDot = board.PuzzleBoard[i-1,j];
                        GameObject rightDot = board.PuzzleBoard[i+1,j];
                        if(leftDot != null && rightDot != null){
                            if(leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag){
                                if(!currentMatches.Contains(leftDot)){
                                    currentMatches.Add(leftDot);
                                }
                                leftDot.GetComponent<PuzzlePiece>().IsMatched = true;
                                if(!currentMatches.Contains(rightDot)){
                                    currentMatches.Add(rightDot);
                                }
                                rightDot.GetComponent<PuzzlePiece>().IsMatched = true;
                                if(!currentMatches.Contains(currentDot)){
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<PuzzlePiece>().IsMatched = true;
                            }
                        }
                    }
                    if(j > 0 && j < board.Height-1){
                        GameObject upDot = board.PuzzleBoard[i,j-1];
                        GameObject downDot = board.PuzzleBoard[i,j+1];
                        if(upDot != null && downDot != null){
                            if(upDot.tag == currentDot.tag && downDot.tag == currentDot.tag){
                                if(!currentMatches.Contains(upDot)){
                                    currentMatches.Add(upDot);
                                }
                                upDot.GetComponent<PuzzlePiece>().IsMatched = true;
                                if(!currentMatches.Contains(downDot)){
                                    currentMatches.Add(downDot);
                                }
                                downDot.GetComponent<PuzzlePiece>().IsMatched = true;
                                if(!currentMatches.Contains(currentDot)){
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<PuzzlePiece>().IsMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
