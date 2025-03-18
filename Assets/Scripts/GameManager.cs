using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    private float time = 60.0f;
    public bool isPlaying = false;
    public TMP_Text timeTxt;
    public TMP_Text scoreTxt;
    public TMP_Text bestScoreTxt;
    public TMP_Text currentScoreTxt;
    public GameObject gameOverBoard;
    public GameObject gameStartBoard;
    public GameObject inGameCanvas;
    public int score;
    private int bestScore;

    private NewBoard board;
    public float comboTime = 3.0f;

    void Start()
    {
        board = FindObjectOfType<NewBoard>();
        if (PlayerPrefs.HasKey("BestScore"))
        {
            // 최고 기록을 저장된 값으로 초기화
            bestScore = PlayerPrefs.GetInt("BestScore");
        }
        // 없으면 999로 하기
        else
        {
            bestScore = 0;
        }
        Time.timeScale = 1.0f; 
        score = 0;
    }

    void Update()
    {
        if(isPlaying){
            if(time <= 0.0f){
                isPlaying = false;
                TimeIsUp();
            }
            if(comboTime >= 0.0f){
                comboTime -= Time.deltaTime;
            }
            time -= Time.deltaTime;
            timeTxt.text = "Time : " + time.ToString("N2");
            scoreTxt.text = "Score : " + score.ToString();

        }
        
    }
    private void TimeIsUp(){
        if(score > bestScore){
            PlayerPrefs.SetInt("BestScore", score);
        }
        currentScoreTxt.text = "score : " + score.ToString();
        bestScoreTxt.text = "best : " + bestScore.ToString();
        gameOverBoard.SetActive(true);
        Time.timeScale = 0.0f;
        board.currentState = GameState.wait;
    }
    public void GoToStartScreen()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void GameStart()
    {
        gameStartBoard.SetActive(false);
        inGameCanvas.SetActive(true);
        board.SetUp(); 
    }
    
    
}
