using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.SceneManagement; // Для работы с переходами между сценами

public class AdmobAdsManager : MonoBehaviour
{
    private static AdmobAdsManager Instance { get; set; }

    private BannerView bannerView;
    private InterstitialAd interstitialAd;

    // Счётчик для отслеживания числа сцен
    private int sceneCount = 0;
    private int sceneThreshold = 3; // Порог в 3 сцены перед показом рекламы

    // Укажите ваши рекламные ID для Android и iOS
#if UNITY_ANDROID
    private string bannerAdUnitId = "ca-app-pub-1951500700100725/5325885462";
    private string interstitialAdUnitId = "ca-app-pub-1951500700100725/5010853406";
#elif UNITY_IPHONE
    private string bannerAdUnitId = "ca-app-pub-3940256099942544/2934735716";
    private string interstitialAdUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
    private string bannerAdUnitId = "unused";
    private string interstitialAdUnitId = "unused";
#endif

    private void Awake()
    {
        if(Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        // Инициализация SDK
        MobileAds.Initialize(initStatus => {
            Debug.Log("Google Mobile Ads SDK Initialized");
        });

        // Подписываемся на событие смены сцены
        SceneManager.activeSceneChanged += OnSceneChanged;

        // Запрос на загрузку баннера и межстраничной рекламы
        RequestBannerAd();
        RequestInterstitialAd();
    }

    // Метод, вызываемый при смене сцены
    private void OnSceneChanged(Scene current, Scene next)
    {
        sceneCount++;

        if (sceneCount >= sceneThreshold)
        {
            sceneCount = 0; // Сбрасываем счётчик
            ShowInterstitialAd(); // Показываем рекламу
        }
    }

    // Запрос баннерной рекламы
    private void RequestBannerAd()
    {
        // Уничтожаем старый баннер, если он существует
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }

        // Создаем новый баннер внизу экрана
        bannerView = new BannerView(bannerAdUnitId, AdSize.Banner, AdPosition.Bottom);

        // Подписка на события
        bannerView.OnBannerAdLoaded += OnBannerAdLoaded;
        bannerView.OnBannerAdLoadFailed += OnBannerAdLoadFailed;
        bannerView.OnAdClicked += OnBannerAdClicked;
        bannerView.OnAdImpressionRecorded += OnBannerAdImpressionRecorded;

        // Создаем и загружаем рекламный запрос
        AdRequest request = new AdRequest();
        bannerView.LoadAd(request);
    }

    // Запрос межстраничной рекламы
    private void RequestInterstitialAd()
    {
        // Уничтожаем старую рекламу, если она существует
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        // Загружаем межстраничную рекламу
        InterstitialAd.Load(interstitialAdUnitId, new AdRequest(),
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Interstitial ad failed to load with error: " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded successfully.");
                interstitialAd = ad;

                // Подписываемся на события межстраничной рекламы
                interstitialAd.OnAdFullScreenContentClosed += OnInterstitialAdClosed;
                interstitialAd.OnAdFullScreenContentFailed += OnInterstitialAdFailed;
                interstitialAd.OnAdPaid += OnInterstitialAdPaid;
            });
    }

    // Показ межстраничной рекламы
    public void ShowInterstitialAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }

    // Обработчики событий баннерной рекламы
    private void OnBannerAdLoaded()
    {
        Debug.Log("Banner ad loaded successfully.");
    }

    private void OnBannerAdLoadFailed(LoadAdError error)
    {
        Debug.LogError("Banner ad failed to load with error: " + error);
    }

    private void OnBannerAdClicked()
    {
        Debug.Log("Banner ad was clicked.");
    }

    private void OnBannerAdImpressionRecorded()
    {
        Debug.Log("Banner ad impression recorded.");
    }

    // Обработчики событий межстраничной рекламы
    private void OnInterstitialAdClosed()
    {
        Debug.Log("Interstitial ad closed.");
        RequestInterstitialAd();  // Загружаем следующую рекламу
    }

    private void OnInterstitialAdFailed(AdError error)
    {
        Debug.LogError("Interstitial ad failed with error: " + error);
        RequestInterstitialAd();  // Загружаем следующую рекламу
    }

    private void OnInterstitialAdPaid(AdValue adValue)
    {
        Debug.Log($"Interstitial ad paid {adValue.Value} {adValue.CurrencyCode}");
    }

    private void OnDestroy()
    {
        // Отписываемся от события смены сцены
        SceneManager.activeSceneChanged -= OnSceneChanged;

        // Уничтожаем рекламу, чтобы избежать утечек памяти
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }
    }
}