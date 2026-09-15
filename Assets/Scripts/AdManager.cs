using UnityEngine;
using GoogleMobileAds.Api;
using System;
using MyGame;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    // Use test IDs in editor/builds for development to avoid invalid traffic
#if UNITY_EDITOR
    private const string InterstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
    private const string RewardedAdUnitId = "ca-app-pub-3940256099942544/2247696110";
    private const string BannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
    private const string RewardedInterstitialAdUnitId = "ca-app-pub-3940256099942544/5224354917";
#else
    private const string InterstitialAdUnitId = "ca-app-pub-7301408190687161/2164767633";
    private const string RewardedAdUnitId = "ca-app-pub-7301408190687161/3841555270";
    private const string BannerAdUnitId = "ca-app-pub-7301408190687161/8066895882";
    private const string RewardedInterstitialAdUnitId = "ca-app-pub-7301408190687161/1257944952";
#endif

    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;
    private RewardedInterstitialAd rewardedInterstitialAd;
    private BannerView bannerView;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        MobileAds.Initialize(status =>
        {
            Debug.Log("Ads Initialized: " + status);
            LoadInterstitialAd();
            LoadRewardedAd();
            LoadRewardedInterstitialAd();
            LoadBannerAd();
        });
    }

    private void OnDestroy()
    {
        bannerView?.Destroy();
        interstitialAd?.Destroy();
    }

    #region Interstitial
    private void LoadInterstitialAd()
    {
        interstitialAd?.Destroy();
        
        var request = new AdRequest();
        InterstitialAd.Load(InterstitialAdUnitId, request,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError("Interstitial load failed: " + error.GetMessage());
                    return;
                }
                if (ad == null) return;

                interstitialAd = ad;
                interstitialAd.OnAdFullScreenContentClosed += HandleAdClosed;
                interstitialAd.OnAdFullScreenContentFailed += (adError) => Debug.LogError("Interstitial failed to show: " + adError.GetMessage());
            });
    }

    public void ShowInterstitialAd(Action onAdClosed = null)
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            PauseGame();
            interstitialAd.OnAdFullScreenContentClosed -= HandleAdClosed;
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                HandleAdClosed();
                onAdClosed?.Invoke();
                LoadInterstitialAd();
            };
            interstitialAd.Show();
        }
        else
        {
            Debug.LogWarning("Interstitial not ready.");
            onAdClosed?.Invoke();
            LoadInterstitialAd();
        }
    }

    private void HandleAdClosed()
    {
        ResumeGame();
    }
    #endregion

    #region Rewarded
    private void LoadRewardedAd()
    {
        rewardedAd = null;
        
        var request = new AdRequest();
        RewardedAd.Load(RewardedAdUnitId, request,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError("Rewarded load failed: " + error.GetMessage());
                    return;
                }
                if (ad == null) return;

                rewardedAd = ad;
                rewardedAd.OnAdFullScreenContentClosed += HandleAdClosed;
                rewardedAd.OnAdFullScreenContentFailed += (adError) => Debug.LogError("Rewarded failed to show: " + adError.GetMessage());
            });
    }

  public void ShowRewardedAd(Action onUserRewarded = null, Action onClosed = null)
{
    Debug.Log("[AdManager] محاولة عرض Rewarded Ad... CanShowAd = " + (rewardedAd?.CanShowAd() ?? false));

    if (rewardedAd != null && rewardedAd.CanShowAd())
    {
        PauseGame();

        // نظف + أضف handlers جديدة
        rewardedAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("[Rewarded] → OnClosed");
            ResumeGame();
            onClosed?.Invoke();
            LoadRewardedAd();           // ← إعادة تحميل ضرورية كل مرة
        };

        rewardedAd.OnAdFullScreenContentFailed += (error) =>
        {
            Debug.LogError("[Rewarded] فشل العرض: " + error.GetMessage());
            ResumeGame();
            onClosed?.Invoke();
            LoadRewardedAd();
        };

        rewardedAd.Show(reward =>
        {
            Debug.Log("[Rewarded] → مكافأة منحت! Amount: " + reward.Amount);
            onUserRewarded?.Invoke();
        });
    }
    else
    {
        Debug.LogWarning("[AdManager] Rewarded غير جاهز → fallback (اختبار فقط)");
        // للاختبار: اعطِ المكافأة مباشرة
        // onUserRewarded?.Invoke();
        onClosed?.Invoke();
        LoadRewardedAd();
    }
}    private void LoadRewardedInterstitialAd()
    {
        rewardedInterstitialAd = null;
        
        var request = new AdRequest();
        RewardedInterstitialAd.Load(RewardedInterstitialAdUnitId, request,
            (RewardedInterstitialAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError("Rewarded Interstitial load failed: " + error.GetMessage());
                    return;
                }
                if (ad == null) return;

                rewardedInterstitialAd = ad;
                rewardedInterstitialAd.OnAdFullScreenContentClosed += HandleAdClosed;
                rewardedInterstitialAd.OnAdFullScreenContentFailed += (adError) => Debug.LogError("Rewarded Interstitial failed to show: " + adError.GetMessage());
            });
    }

    public void ShowRewardedInterstitialAd(Action onUserRewarded = null, Action onClosed = null)
    {
        if (rewardedInterstitialAd != null && rewardedInterstitialAd.CanShowAd())
        {
            PauseGame();
            rewardedInterstitialAd.OnAdFullScreenContentClosed -= HandleAdClosed;
            rewardedInterstitialAd.OnAdFullScreenContentClosed += () =>
            {
                HandleAdClosed();
                onClosed?.Invoke();
                LoadRewardedInterstitialAd();
            };
            rewardedInterstitialAd.Show(reward =>
            {
                onUserRewarded?.Invoke();
                Debug.Log("User rewarded in interstitial!");
            });
        }
        else
        {
            onUserRewarded?.Invoke();
            onClosed?.Invoke();
            LoadRewardedInterstitialAd();
        }
    }
    #endregion

    #region Banner
    private void LoadBannerAd()
    {
        bannerView?.Destroy();
        bannerView = new BannerView(BannerAdUnitId, AdSize.Banner, AdPosition.Bottom);
        var request = new AdRequest();
        bannerView.LoadAd(request);
    }

    public void HideBanner() => bannerView?.Hide();
    public void ShowBanner() => bannerView?.Show();
    #endregion

    #region Pause / Resume
    private void PauseGame()
    {
        Time.timeScale = 0f;
        var audioListener = UnityEngine.Object.FindFirstObjectByType<AudioListener>();
        if (audioListener != null)
            AudioListener.pause = true;
        else
            Debug.LogWarning("No AudioListener found for pausing.");
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (LevelManager.Instance?.player != null)
        {
            var player = LevelManager.Instance.player;
            player.gameObject.SetActive(true);
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            LevelManager.Instance.RestorePlayerInput();
            LevelManager.Instance.ReLinkMovementButtons();
        }
        else
        {
            Debug.LogWarning("Player not found in LevelManager during resume.");
        }
    }
    #endregion
}