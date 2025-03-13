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
    public GameObject gameOverBtn;
    public int score;

    void Start()
    {
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
        gameOverBtn.SetActive(true);
        Time.timeScale = 0f;
    }
    
    
}
