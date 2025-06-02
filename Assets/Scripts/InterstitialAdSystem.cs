using UnityEngine;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;
public class InterstitialAdSystem : MonoBehaviour
{
    private int retryCount = 0;
    private const int MAX_RETRY = 3;
    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-4556004795553060/7469177892";
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
    private string _adUnitId = "unused";
#endif

    private InterstitialAd _interstitialAd;
    void Start(){
        LoadInterstitialAd();
    }

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
    public void PressedRetryButton()
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
}
