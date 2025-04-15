using UnityEngine;
using UnityEngine.InputSystem.Controls;
using Image = UnityEngine.UI.Image;
public class AudioManager : MonoBehaviour
{
    [SerializeField] GameObject bgmObj;
    [SerializeField] AudioClip[] audioClips;
    private AudioSource bgmSound;
    public Sprite[] soundBtnImg;
    public GameObject soundBtn;

    private AudioSource audioSource;

    void Start()
    {
        bgmSound = bgmObj.GetComponent<AudioSource>();
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayMatchedSound()
    {
        audioSource.PlayOneShot(audioClips[0]);
    }
    public void PlayCorrectSound()
    {
        audioSource.PlayOneShot(audioClips[1]);
    }
    public void PlayWrongSound()
    {
        audioSource.PlayOneShot(audioClips[2]);
    }
    public void PlayNewRecordSound()
    {
        audioSource.PlayOneShot(audioClips[3]);
    }
    public void PlayBGM()
    {
        bgmSound.mute = false;
        soundBtn.GetComponent<Image>().sprite = soundBtnImg[0];
        
    }
    public void StopBGM()
    {
        bgmSound.mute=true;
        soundBtn.GetComponent<Image>().sprite = soundBtnImg[1];
    }
    public void PressMuteButton()
    {
        if(bgmSound.mute)
        {
            PlayBGM();
        }
        else
        {
            StopBGM();
        }
    }
    public void GameMusicStart()
    {
        bgmSound.Play();
    }
}
