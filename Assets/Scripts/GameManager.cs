using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    private float time = 5.0f;
    public bool isPlaying = false;
    public TMP_Text timeTxt;
    public TMP_Text scoreTxt;
    public GameObject gameOverBoard;
    public int score;

    private NewBoard board;

    void Start()
    {
        board = FindObjectOfType<NewBoard>();
        board.currentState = GameState.wait;
        Time.timeScale = 1.0f; 
        score = 0;
    }

    void Update()
    {
        if(isPlaying){
            if(time < 0){
                isPlaying = false;
                TimeIsUp();
            }
            time -= Time.deltaTime;
            timeTxt.text = "Time : " + time.ToString("N2");
            scoreTxt.text = "Score : " + score.ToString();

        }
        
    }
    void TimeIsUp(){
        gameOverBoard.SetActive(true);
        Time.timeScale = 0f;
        board.currentState = GameState.wait;
    }

    private void GameStart()
    {
        //시간 리셋
        time = 60.0f;
        gameOverBoard.SetActive(false);
        board.currentState = GameState.move;
    }
    
    
}
