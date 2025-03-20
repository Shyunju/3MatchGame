using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private NewBoard board;
    private GameManager gameManager;
    public List<GameObject> currentMatches = new List<GameObject>();
    void Start()
    {
        board = FindObjectOfType<NewBoard>();   
    }
    public void FindAllMatches(){
        StartCoroutine(FindAllMatchesCo());
    }
    private IEnumerator FindAllMatchesCo(){
        yield return new WaitForSeconds(.2f);
        for(int i = 0; i < board.width; ++i){
            for(int j = 0; j < board.height; ++j){
                GameObject currentDot = board.puzzleBoard[i,j];
                if(currentDot != null){
                    if(i > 0 && i < board.width-1){
                        GameObject leftDot = board.puzzleBoard[i-1,j];
                        GameObject rightDot = board.puzzleBoard[i+1,j];
                        if(leftDot != null && rightDot != null){
                            if(leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag){
                                if(!currentMatches.Contains(leftDot)){
                                    currentMatches.Add(leftDot);
                                }
                                leftDot.GetComponent<PuzzlePiece>().isMatched = true;
                                if(!currentMatches.Contains(rightDot)){
                                    currentMatches.Add(rightDot);
                                }
                                rightDot.GetComponent<PuzzlePiece>().isMatched = true;
                                if(!currentMatches.Contains(currentDot)){
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<PuzzlePiece>().isMatched = true;
                            }
                        }
                    }
                    if(j > 0 && j < board.height-1){
                        GameObject upDot = board.puzzleBoard[i,j-1];
                        GameObject downDot = board.puzzleBoard[i,j+1];
                        if(upDot != null && downDot != null){
                            if(upDot.tag == currentDot.tag && downDot.tag == currentDot.tag){
                                if(!currentMatches.Contains(upDot)){
                                    currentMatches.Add(upDot);
                                }
                                upDot.GetComponent<PuzzlePiece>().isMatched = true;
                                if(!currentMatches.Contains(downDot)){
                                    currentMatches.Add(downDot);
                                }
                                downDot.GetComponent<PuzzlePiece>().isMatched = true;
                                if(!currentMatches.Contains(currentDot)){
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<PuzzlePiece>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
