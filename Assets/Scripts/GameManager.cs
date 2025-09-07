using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Button = UnityEngine.UI.Button;

public class GameManager : MonoBehaviour
{
    private static GameManager gameManager;
    [SerializeField] GameObject cam;
    public GameObject Cam {get { return cam;} }
    [SerializeField] private float time;
    [SerializeField] bool isPlaying = false;
    public bool IsPlaying {get{return isPlaying;} set{isPlaying = value;}}
    [SerializeField] bool isTuched = false;
    public bool IsTuched {get{return isTuched;} set{isTuched = value;}}
    [SerializeField] float comboTime = 3.0f;
    public float ComboTime { get { return comboTime; } }
    [SerializeField] AudioManager _audioManager;
    [SerializeField] GameObject _selectLevelBoard;
    [SerializeField] GameObject _firstDisplay;
    [SerializeField] TMP_Text _timeTxt;                //시간
    [SerializeField] TMP_Text _scoreTxt;               //실시간 점수
    [SerializeField] TMP_Text _bestScoreTxt;           //최고점수
    [SerializeField] TMP_Text _number1Txt;             //피연산자1
    [SerializeField] TMP_Text _number2Txt;             //피연산자2
    [SerializeField] TMP_Text _operatorTxt;            //연산자
    [SerializeField] TMP_Text _answerTxt;              //비교될 값
    [SerializeField] TMP_Text _comparatorTxt;          //비교연산자
    [SerializeField] TMP_Text _currentScoreTxt;        //현재점수
    [SerializeField] TMP_Text _curGoalScoreTxt;        //현재 레벨 목표 점수
    [SerializeField] GameObject _gameOverBoard;        //제한시간이 끝난후
    [SerializeField] GameObject _gameStartBoard;       //게임 시작 전
    [SerializeField] GameObject _inGameCanvas;         //게임 진행 중
    [SerializeField] GameObject _pauseBoard;           //게임 일시정지
    [SerializeField] GameObject _protectBoard;
    [SerializeField] int _score;
    [SerializeField] Queue<int> _numQueue = new Queue<int>();      //피연산자 큐
    [SerializeField] int _successScore;
    [SerializeField] GameObject _howToBoard;
    [SerializeField] GameObject _bestImage;
    [SerializeField] Board _board;
    [SerializeField] GameObject[] _levelArr;
    private int[] _operatorArr = new int[2];             //연산자 배열
    private int _answer;
    private bool _isShowingHowToBoard = false;
    private int[] _goalScores = {0, 900, 900, 1500, 1800, 2000, 3000};
    private int _curLevel;
    private int _answerRangeMin = 3;
    private int _answerRangeMax;
    private int _operRange;
    private int _curGoalScore;
    private int _curNumberRange;
    public int CurLevel {get{return _curLevel;} set{_curLevel = value;}}    


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
    private void Awake()
    {
        Screen.SetResolution(1080, 1920, true);
    }
    void Start()
    {   
        if (!PlayerPrefs.HasKey("ClearedLevels"))
        {
            PlayerPrefs.SetInt("ClearedLevel", 0);            
        }
        Time.timeScale = 1.0f; 
        _score = 0;
    }

    void Update()
    {
        CheckPlayTime();
    }
    void CheckPlayTime()
    {
        if (isPlaying)
        {
            if (comboTime >= 0.0f)
            {
                comboTime -= Time.deltaTime;
            }
            time -= Time.deltaTime;
            _timeTxt.text = time.ToString("N2");
            _scoreTxt.text = _score.ToString();
            if (time <= 0.0f)
            {
                _timeTxt.text = "0.00";
                isPlaying = false;
                TimeIsUp();
            }
        }
    }
    public void ChaingeScore(int num)
    {
        _score += num;
        if(_score < 0)
        {
            _score = 0;
        }
    }
    private void TimeIsUp() //제한시간 종료
    {
        _audioManager.StopBGM();
        _audioManager.PlayMatchedSound();
        if(_score >= _curGoalScore){
            _audioManager.PlayNewRecordSound();
            _bestImage.SetActive(true);
            int clearedLevels = PlayerPrefs.GetInt("ClearedLevels", 0);
            clearedLevels |= 1 << _curLevel % 10; //레벨 클리어
            PlayerPrefs.SetInt("ClearedLevels", clearedLevels);
            PlayerPrefs.Save();
        }
        _currentScoreTxt.text = _score.ToString();
        _bestScoreTxt.text = _curGoalScore.ToString();
        _gameOverBoard.SetActive(true);
        Time.timeScale = 0.0f;
        _board.currentState = GameState.wait;
    }    
    public void GameStart()
    {
        _board.SettingPosition();
        _gameStartBoard.SetActive(false);
        _inGameCanvas.SetActive(true);
        _audioManager.GameMusicStart();
        _curGoalScoreTxt.text = "목표점수 : " + _curGoalScore.ToString();
        MakeFormula();
    }
    private void MakeFormula()
    {
        //수연산자와 비교연산지 채우기
        int oper = UnityEngine.Random.Range(1, _operRange);
        _operatorArr[0] = oper;
        switch(oper)
        {
            case 1:
                _operatorTxt.text = "+";
                _answerRangeMax = _curNumberRange * 2 - 1;
                break;
            case 2:
                _operatorTxt.text = "-";
                _answerRangeMax = _curNumberRange - 2;
                break;
            case 3:
                _operatorTxt.text = "x";
                _answerRangeMax = (int)Math.Pow(_curNumberRange, 2) -1;
                break;
            case 4:
                _operatorTxt.text = "%";
                _answerRangeMax = _curNumberRange - 1;
                break;
        }
        int comp = UnityEngine.Random.Range(1, 4);
        _operatorArr[1] = comp;
        switch(comp)
        {
            case 1:
                _comparatorTxt.text = "<";
                break;
            case 2:
                _comparatorTxt.text = "<=";
                break;
            case 3:
                _comparatorTxt.text = ">";
                break;
            case 4:
                _comparatorTxt.text = ">=";
                break;
        }
        _answer = UnityEngine.Random.Range(_answerRangeMin, _answerRangeMax);
        _answerTxt.text = _answer.ToString();
    }
    public void FillNumberText(int num)
    {
        _numQueue.Enqueue(num);
        if(_numQueue.Count == 1)
        {
            _number1Txt.text = num.ToString();
        }else{
            _number2Txt.text = num.ToString();
            curQState = QueueState.full;
            StartCoroutine(CheckFormulaCo());
        }
    }
    private IEnumerator CheckFormulaCo()
    {
        yield return new WaitForSeconds(.8f);
        if(CheckFormula())
        {
            _audioManager.PlayCorrectSound();
            ChaingeScore(_successScore);
        }else{
            _audioManager.PlayWrongSound();
        }
        _number2Txt.text = "_";
        _number1Txt.text = "_";
        curQState = QueueState.empty;
        MakeFormula();
    }
    
    private bool CheckFormula()
    {
        //식 계산 확인하기 
        int result = 0;
        int num1 = _numQueue.Dequeue();
        int num2 = _numQueue.Dequeue();
        switch(_operatorArr[0])
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
        switch(_operatorArr[1])
        {
            case 1:
                if(result < _answer)     return true;
                break;        
            case 2:
                if(result <= _answer)     return true;
                break; 
            case 3:
                if(result > _answer)     return true;
                break; 
            case 4:
                if(result >= _answer)     return true;
                break; 
            default: return false;
        }
        return false;
        
    }

    public void PauseGame()
    {
        if(isPlaying){
            _board.currentState = GameState.wait;
            isPlaying = false;
            _pauseBoard.SetActive(true);
            _audioManager.StopBGM();
        }else{
            _board.currentState = GameState.move;
            _pauseBoard.SetActive(false);
            isPlaying = true;
            _audioManager.PlayBGM();
        }

    }
    public void ShowHowToBoard()
    {
        if(_isShowingHowToBoard)
        {
            _isShowingHowToBoard = false;
        }else{
            _isShowingHowToBoard = true;
        }
        _howToBoard.SetActive(_isShowingHowToBoard);
    }
    public void SettingLevel(int level){
        CurLevel = level;
        _operRange = level > 200 ? 4 : 2;
        _curGoalScore = _goalScores[level % 10];
        _curNumberRange = level % 100 / 10;
        time = level > 200 ? 90 : 60;
        GameStart();
    }
    public void GoToSelectLevel()
    {
        _firstDisplay.SetActive(false);
        _selectLevelBoard.SetActive(true);
        StartCoroutine(ProtectButtonCo());
        CheckLevelState();
    }
    public void GoToFirstDisplay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void CheckLevelState()
    {
        int clearedLevels = PlayerPrefs.GetInt("ClearedLevels", 0);
        
        for(int i = 1; i < 6; i++)
        {
            if((clearedLevels & (1 << i)) == 0) //i레벨이 클리어 되어있지 않음
            {
                //버튼 배열에서 아이번째 버튼 활성화
                Button tempButton = _levelArr[i].GetComponent<Button>();
                tempButton.interactable = false;
            }
        }
    }

    IEnumerator ProtectButtonCo()
    {
        yield return new WaitForSeconds(0.5f);
        _protectBoard.SetActive(false);
    }
    public void GameExit()
{
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
}
}
