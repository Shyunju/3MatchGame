using UnityEngine;
using Image = UnityEngine.UI.Image;
public class AudioManager : MonoBehaviour
{
    [SerializeField] GameObject correctObj;
    [SerializeField] GameObject wrongObj;
    [SerializeField] GameObject matchedObj;
    [SerializeField] GameObject bgmObj;
    [SerializeField] GameObject newRecordObj;
    private AudioSource correctSound;
    private AudioSource wrongSound;
    private AudioSource matchedSound;
    private AudioSource bgmSound;
    private AudioSource newRecordSound;
    public Sprite[] soundBtnImg;
    public GameObject soundBtn;
    private bool isMute = true;

    void Start()
    {
        correctSound = correctObj.GetComponent<AudioSource>();
        wrongSound = wrongObj.GetComponent<AudioSource>();
        matchedSound = matchedObj.GetComponent<AudioSource>();
        bgmSound = bgmObj.GetComponent<AudioSource>();
        newRecordSound = newRecordObj.GetComponent<AudioSource>();
    }

    public void PlayCorrectSound()
    {
        correctSound.Play();
    }
    public void PlayWrongSound()
    {
        wrongSound.Play();
    }
    public void PlayMatchedSound()
    {
        matchedSound.Play();
    }
    public void PlayBGM()
    {
        if(isMute)
        {
            bgmSound.Play();
            soundBtn.GetComponent<Image>().sprite = soundBtnImg[0];
            isMute = false;
        }else{
            bgmSound.Stop();
            soundBtn.GetComponent<Image>().sprite = soundBtnImg[1];
            isMute = true;
        }
    }
    public void NewRecord()
    {
        newRecordSound.Play();
    }
}
