using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;

public class GameManager : MonoBehaviour
{
    private static GameManager gameManager;
    [SerializeField] GameObject cam;
    public GameObject Cam {get { return cam;} }
    [SerializeField] private float time = 60.0f;
    [SerializeField] bool isPlaying = false;
    public bool IsPlaying {get{return isPlaying;} set{isPlaying = value;}}
    [SerializeField] bool isTuched = false;
    public bool IsTuched {get{return isTuched;} set{isTuched = value;}}
    [SerializeField] float comboTime = 3.0f;
    public float ComboTime { get { return comboTime; } }
    [SerializeField] AudioManager audioManager;
    [SerializeField] TMP_Text timeTxt;                //시간
    [SerializeField] TMP_Text scoreTxt;               //실시간 점수
    [SerializeField] TMP_Text bestScoreTxt;           //최고점수
    [SerializeField] TMP_Text number1Txt;             //피연산자1
    [SerializeField] TMP_Text number2Txt;             //피연산자2
    [SerializeField] TMP_Text operatorTxt;            //연산자
    [SerializeField] TMP_Text answerTxt;              //비교될 값
    [SerializeField] TMP_Text comparatorTxt;          //비교연산자
    [SerializeField] TMP_Text currentScoreTxt;        //현재점수
    [SerializeField] GameObject gameOverBoard;        //제한시간이 끝난후
    [SerializeField] GameObject gameStartBoard;       //게임 시작 전
    [SerializeField] GameObject inGameCanvas;         //게임 진행 중
    [SerializeField] GameObject pauseBoard;           //게임 일시정지
    [SerializeField] private int score;
    [SerializeField] Queue<int> numQueue = new Queue<int>();      //피연산자 큐
    [SerializeField] private int successScore;
    [SerializeField] private GameObject howToBoard;
    [SerializeField] private GameObject bestImage;
    [SerializeField] private Board board;
    private int[] operatorArr = new int[2];             //연산자 배열
    private int bestScore;
    private int answer;
    private bool isShowingHowToBoard = false;
    private CameraSetting cm;

    private int retryCount = 0;
    private const int MAX_RETRY = 3;
    public float lerpValueTest;
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
        Screen.SetResolution(750, 1334, true);
    }
    void Start()
    {
        LoadInterstitialAd();
        cm = cam.GetComponent<CameraSetting>();
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
            timeTxt.text = time.ToString("N2");
            scoreTxt.text = score.ToString();
            if (time <= 0.0f)
            {
                isPlaying = false;
                TimeIsUp();
            }
        }
    }
    public void ChaingeScore(int num)
    {
        score += num;
    }
    private void TimeIsUp() //제한시간 종료
    {
        audioManager.StopBGM();
        if(score > bestScore){
            audioManager.PlayNewRecordSound();
            PlayerPrefs.SetInt("BestScore", score);
            bestImage.SetActive(true);
        }
        audioManager.PlayMatchedSound();
        currentScoreTxt.text = score.ToString();
        bestScoreTxt.text = bestScore.ToString();
        gameOverBoard.SetActive(true);
        Time.timeScale = 0.0f;
        board.currentState = GameState.wait;
    }
    public void GoToStartScreen()
    {
        if (this._interstitialAd != null && this._interstitialAd.CanShowAd())
        {
            ShowInterstitialAd();
            retryCount = 0;
        }
        else
        {
            ShowToast("광고를 불러오는 중입니다. 잠시 후 다시 시도해주세요.");

            // 재시도 제한
            if (retryCount < MAX_RETRY)
            {
                retryCount++;
                // 5초 후에 다시 광고 로드 시도
                Invoke(nameof(LoadInterstitialAd), 5f);
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }


    }
    public void ShowToast(string message)   //화면에 메세지띄우기
    {
#if UNITY_ANDROID
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
            AndroidJavaObject javaString = new AndroidJavaObject("java.lang.String", message);
            AndroidJavaObject toast = toastClass.CallStatic<AndroidJavaObject>(
                "makeText", context, javaString, toastClass.GetStatic<int>("LENGTH_SHORT"));
            toast.Call("show");
        }));
#endif
    }
    public void GameStart()
    {
        board.SettingPosition(20);
        gameStartBoard.SetActive(false);
        inGameCanvas.SetActive(true);
        audioManager.GameMusicStart();
        //board.SetUp(); 
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
                operatorTxt.text = "x";
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
        numQueue.Enqueue(num);
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
            ChaingeScore(successScore);
        }else{
            audioManager.PlayWrongSound();
        }
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
            board.currentState = GameState.wait;
            isPlaying = false;
            pauseBoard.SetActive(true);
            audioManager.StopBGM();
        }else{
            board.currentState = GameState.move;
            pauseBoard.SetActive(false);
            isPlaying = true;
            audioManager.PlayBGM();
        }

    }
    public void ShowHowToBoard()
    {
        if(isShowingHowToBoard)
        {
            isShowingHowToBoard = false;
        }else{
            isShowingHowToBoard = true;
        }
        howToBoard.SetActive(isShowingHowToBoard);
    }

    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
  private string _adUnitId = "unused";
#endif

    private InterstitialAd _interstitialAd;

    /// <summary>
    /// Loads the interstitial ad.
    /// </summary>
    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        //Debug.Log("Loading the interstitial ad.");
        // create our request used to load the ad.
        var adRequest = new AdRequest();

        InterstitialAd.Load(_adUnitId, adRequest,
        // send the request to load the ad.
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                //Debug.Log("Interstitial ad loaded with response : "
                          //+ ad.GetResponseInfo());

                _interstitialAd = ad;
                RegisterEventHandlers(_interstitialAd);
            });
    }

    public void ShowInterstitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }
    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            // Debug.Log("Interstitial ad full screen content closed.");
            // 원하는 동작 실행
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            LoadInterstitialAd(); // 광고 다시 로드
        };

        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content with error : " + error);
            LoadInterstitialAd();
        };
    }


}
