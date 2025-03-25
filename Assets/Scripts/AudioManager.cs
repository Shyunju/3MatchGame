using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
public class AudioManager : MonoBehaviour
{
    public GameObject correctObj;
    public GameObject wrongObj;
    public GameObject matchedObj;
    public GameObject bgmObj;
    private AudioSource correctSound;
    private AudioSource wrongSound;
    private AudioSource matchedSound;
    private AudioSource bgmSound;
    public Sprite[] soundBtnImg;
    public GameObject soundBtn;
    private bool isMute = true;

    void Start()
    {
        correctSound = correctObj.GetComponent<AudioSource>();
        wrongSound = wrongObj.GetComponent<AudioSource>();
        matchedSound = matchedObj.GetComponent<AudioSource>();
        bgmSound = bgmObj.GetComponent<AudioSource>();
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
}
