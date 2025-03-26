using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    private static GameManager gameManager;
    public AudioManager audioManager;
    private float time = 60.0f;
    public bool isPlaying = false;
    public bool isTuched = false;
    public TMP_Text timeTxt;                //시간
    public TMP_Text scoreTxt;               //실시간 점수
    public TMP_Text bestScoreTxt;           //최고점수
    public TMP_Text number1Txt;             //피연산자1
    public TMP_Text number2Txt;             //피연산자2
    public TMP_Text operatorTxt;            //연산자
    public TMP_Text answerTxt;              //비교될 값
    public TMP_Text comparatorTxt;          //비교연산자
    public TMP_Text currentScoreTxt;        //현재점수
    public GameObject gameOverBoard;        //제한시간이 끝난후
    public GameObject gameStartBoard;       //게임 시작 전
    public GameObject inGameCanvas;         //게임 진행 중
    public GameObject pauseBoard;           //게임 일시정지
    public int score;
    public Queue<int> numQueue = new Queue<int>();      //피연산자 큐
    private int[] operatorArr = new int[2];             //연산자 배열
    private int bestScore;
    private int answer;

    public Board board;
    public float comboTime = 3.0f;
        public enum QueueState
    {
        empty,
        full
    }
    public QueueState curQState = QueueState.empty;
    private GameManager()
    {
    }
    public static GameManager GetGameManager()
    {
        if (gameManager == null)
        {
            gameManager = new GameManager();
        }
        return gameManager;
    }
    void Start()
    {
        //int asdf = Enum.GetValues(typeof(Operators)).Length;
        //board = FindObjectOfType<NewBoard>();
        if (PlayerPrefs.HasKey("BestScore"))
        {
            // 최고 기록을 저장된 값으로 초기화
            bestScore = PlayerPrefs.GetInt("BestScore");
        }
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
    private void TimeIsUp(){ //제한시간 종료
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
        audioManager.PlayBGM();
        board.SetUp(); 
        MakeFormula();
    }
    private void MakeFormula()
    {
        //수연산자와 비교연산지 채우기
        int oper = UnityEngine.Random.Range(1, 4);
        operatorArr[0] = oper;
        switch(oper)
        {
            case 1:
                operatorTxt.text = "+";
                break;
            case 2:
                operatorTxt.text = "-";
                break;
            case 3:
                operatorTxt.text = "*";
                break;
            case 4:
                operatorTxt.text = "%";
                break;
        }
        int comp = UnityEngine.Random.Range(1, 4);
        operatorArr[1] = comp;
        switch(comp)
        {
            case 1:
                comparatorTxt.text = "<";
                break;
            case 2:
                comparatorTxt.text = "<=";
                break;
            case 3:
                comparatorTxt.text = ">";
                break;
            case 4:
                comparatorTxt.text = ">=";
                break;
        }
        answer = UnityEngine.Random.Range(10, 82);
        answerTxt.text = answer.ToString();
    }
    public void FillNumberText(int num)
    {
        if(numQueue.Count == 1)
        {
            number1Txt.text = num.ToString();
        }else{
            number2Txt.text = num.ToString();
            curQState = QueueState.full;
            StartCoroutine(CheckFormulaCo());
        }
    }
    private IEnumerator CheckFormulaCo()
    {
        yield return new WaitForSeconds(.8f);
        if(CheckFormula())
        {
            audioManager.PlayCorrectSound();
            score += 500;
        }else{
            audioManager.PlayWrongSound();
        }
        //numQueue.Clear();
        number2Txt.text = "_";
        number1Txt.text = "_";
        curQState = QueueState.empty;
        MakeFormula();
    }
    
    private bool CheckFormula()
    {
        //식 계산 확인하기 
        int result = 0;
        int num1 = numQueue.Dequeue();
        int num2 = numQueue.Dequeue();
        switch(operatorArr[0])
        {
            case 1:
                result = num1 + num2;
                break;
            case 2:
                result = num1 - num2;
                break;
            case 3:
                result = num1 * num2;
                break;
            case 4:
                result = num1 / num2;
                break;
        }
        switch(operatorArr[1])
        {
            case 1:
                if(result < answer)     return true;
                break;        
            case 2:
                if(result <= answer)     return true;
                break; 
            case 3:
                if(result > answer)     return true;
                break; 
            case 4:
                if(result >= answer)     return true;
                break; 
            default: return false;
        }
        return false;
        
    }

    public void PauseGame()
    {
        if(isPlaying){
            isPlaying = false;
            pauseBoard.SetActive(true);
        }else{
            pauseBoard.SetActive(false);
            isPlaying = true;
        }

    }
    
    
}
