using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAdExample : MonoBehaviour
{
    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER; // 배너 위치
    [SerializeField] string _androidAdUnitId = "Banner_Android"; // Android 광고 ID
    [SerializeField] string _iOSAdUnitId = "Banner_iOS";         // iOS 광고 ID

    private string _adUnitId = null; // 현재 플랫폼에 맞는 광고 ID 저장

    void Start()
    {
        // 현재 플랫폼에 맞는 Ad Unit ID 설정
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif

        // 배너 위치 설정
        Advertisement.Banner.SetPosition(_bannerPosition);

        // 배너 광고를 자동으로 로드하고 표시
        LoadAndShowBanner();
    }

    private void LoadAndShowBanner()
    {
        // 배너 로드 옵션 설정 (콜백 포함)
        BannerLoadOptions loadOptions = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,  // 로드 성공 시 호출될 메서드
            errorCallback = OnBannerError  // 로드 실패 시 호출될 메서드
        };

        // 배너 광고 로드 시작
        Advertisement.Banner.Load(_adUnitId, loadOptions);
    }

    private void OnBannerLoaded()
    {
        Debug.Log("배너 광고가 성공적으로 로드되었습니다.");

        // 배너 광고 표시
        Advertisement.Banner.Show(_adUnitId);
    }

    private void OnBannerError(string message)
    {
        Debug.LogError($"배너 광고 로드 실패: {message}");
    }
}
