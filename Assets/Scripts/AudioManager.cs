using UnityEngine;

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
        bgmSound.Play();
    }
}
