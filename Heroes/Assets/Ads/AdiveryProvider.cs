using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdiveryUnity;
using System;
using UnityEngine.UI;

public class AdiveryProvider : IAdProvider
{
    string _appID = "";
    string _rewardedPlacement = "";
    string _interstitialPlacement = "";
    string _bannerPlacement = "";
    string _mediumBannerPlacement = "";
    string _largeBannerPlacement = "";
    string _nativePlacement = "";

    NativeAd _nativeAd;
    BannerAd _bannerAd, _mediumBannerAd, _largeBannerAd;
    AdiveryListener _rewardedListener;
    AdiveryListener _interstitialListener;

    private void OnDestroy()
    {
        Adivery.RemoveListener(_rewardedListener);
        Adivery.RemoveListener(_interstitialListener);
    }

    public void Initialize(string appID, string rewardedPlacement, string interstitialPlacement, string bannerPlacement, string mediumBannerPlacement, string largeBannerPlacement, string nativePlacement)
    {
        Debug.Log("Initialize Adivery");
        Adivery.SetLoggingEnabled(true);

        _appID = appID;
        _rewardedPlacement = rewardedPlacement;
        _interstitialPlacement = interstitialPlacement;
        _bannerPlacement = bannerPlacement;
        _mediumBannerPlacement = mediumBannerPlacement;
        _largeBannerPlacement = largeBannerPlacement;
        _nativePlacement = nativePlacement;

        Adivery.Configure(_appID);

        InitRewardedAd();
        InitInterstitialAd();
        InitBannerAd();
        InitMediumBannerAd();
        InitLargeBannerAd();
        InitNativeAd();
    }

    private void InitRewardedAd()
    {
        _rewardedListener = new AdiveryListener();

        _rewardedListener.OnRewardedAdLoaded += OnRewardedAdLoaded;
        _rewardedListener.OnRewardedAdClicked += OnRewardedClicked;
        _rewardedListener.OnRewardedAdClosed += OnRewardedClosed;

        Adivery.AddListener(_rewardedListener);
        Adivery.AddPlacementListener(_rewardedPlacement, _rewardedListener);
    }

    private void InitInterstitialAd()
    {
        _interstitialListener = new AdiveryListener();

        _interstitialListener.OnInterstitialAdLoaded += OnInterstitialLoaded;
        _interstitialListener.OnInterstitialAdClicked += OnInterstitialAdClicked;
        _interstitialListener.OnInterstitialAdClosed += OnInterstitialAdClosed;

        Adivery.AddListener(_interstitialListener);
    }

    private void InitBannerAd()
    {
        _bannerAd = new BannerAd(_bannerPlacement, BannerAd.TYPE_BANNER, BannerAd.POSITION_BOTTOM);
        _bannerAd.OnAdLoaded += OnBannerAdLoaded;
        _bannerAd.OnAdClicked += OnBannerAdClicked;
    }

    private void InitMediumBannerAd()
    {
        _mediumBannerAd = new BannerAd(_mediumBannerPlacement, BannerAd.TYPE_MEDIUM_RECTANGLE, BannerAd.POSITION_BOTTOM);
        _bannerAd.OnAdLoaded += OnMediumRectangleAdLoaded;
        _bannerAd.OnAdClicked += OnMediumBannerAdClicked;
    }

    private void InitLargeBannerAd()
    {
        _largeBannerAd = new BannerAd(_largeBannerPlacement, BannerAd.TYPE_LARGE_BANNER, BannerAd.POSITION_BOTTOM);
        _bannerAd.OnAdLoaded += OnLargeBannerLoaded;
        _bannerAd.OnAdClicked += OnLargeBannerAdClicked;
        _largeBannerAd.LoadAd();
    }

    private void InitNativeAd()
    {
        _nativeAd = new NativeAd(_nativePlacement);
        _nativeAd.OnAdLoaded += OnNativeAdLoaded;
        _nativeAd.LoadAd();
    }

    public void LoadRewardedAd()
    {
        Adivery.PrepareRewardedAd(_rewardedPlacement);
    }

    public void LoadInterstitialAd()
    {
        Adivery.PrepareInterstitialAd(_interstitialPlacement);
    }

    public void LoadBannerAd()
    {
        _bannerAd.LoadAd();
    }

    public void LoadMediumBannerAd()
    {
        _mediumBannerAd.LoadAd();
    }

    public void LoadLargeBannerAd()
    {
        _largeBannerAd.LoadAd();
    }

    public void LoadNativeAd()
    {
        _nativeAd.LoadAd();
    }

    public void ShowRewardedAd()
    {
        if (Adivery.IsLoaded(_rewardedPlacement))
        {
            Adivery.Show(_rewardedPlacement);
        }
    }

    public void ShowInterstitialAd()
    {
        if (Adivery.IsLoaded(_interstitialPlacement))
        {
            Adivery.Show(_interstitialPlacement);
        }
    }

    public void ShowBannerAd()
    {
        if (_bannerAd.IsLoaded())
        {
            _largeBannerAd.Hide();
            _mediumBannerAd.Hide();
            _bannerAd.Show();
        }
    }

    public void ShowMediumBannerAd()
    {
        if (_mediumBannerAd.IsLoaded())
        {
            Debug.Log("show medium rectangle");
            _bannerAd.Hide();
            _largeBannerAd.Hide();
            _mediumBannerAd.Show();
        }
    }

    public void ShowLargeBannerAd()
    {
        if (_largeBannerAd.IsLoaded())
        {
            _bannerAd.Hide();
            _mediumBannerAd.Hide();
            _largeBannerAd.Show();
        }
    }

    public void ShowNativeAd()
    {
        _nativeAd.RecordImpression();
    }

    public bool RewardedAdIsLoaded()
    {
        return Adivery.IsLoaded(_rewardedPlacement);
    }

    public bool InterstitialAdIsLoaded()
    {
        return Adivery.IsLoaded(_interstitialPlacement);
    }

    public bool BannerAdIsLoaded()
    {
        return _bannerAd.IsLoaded();
    }

    public bool MediumBannerAdIsLoaded()
    {
        return _mediumBannerAd.IsLoaded();
    }

    public bool LargeBannerAdIsLoaded()
    {
        return _largeBannerAd.IsLoaded();
    }

    private void OnRewardedAdLoaded(object caller, string placement)
    {
        Debug.Log("Rewarded loaded");
    }

    private void OnRewardedClicked(object caller, string placement)
    {
        Debug.Log("Rewarded ad clicked " + placement);
    }

    private void OnRewardedClosed(object caller, AdiveryReward reward)
    {
        Debug.Log("Rewarded ad closed " + reward.PlacementId);
    }

    private void OnInterstitialLoaded(object caller, string placement)
    {
        Debug.Log("Interstitial ad loaded");
    }

    private void OnInterstitialAdClicked(object caller, string placement)
    {
        Debug.Log("Interstitial ad clicked " + placement);
    }

    private void OnInterstitialAdClosed(object caller, string placement)
    {
        Debug.Log("Interstitial ad closed " + placement);
    }

    private void OnBannerAdLoaded(object caller, EventArgs args)
    {
        _bannerAd.Hide();

        Debug.Log("Banner ad loaded");
    }

    private void OnBannerAdClicked(object caller, EventArgs args)
    {
        Debug.Log("Banner ad clicked");
    }

    private void OnMediumRectangleAdLoaded(object caller, EventArgs args)
    {
        _mediumBannerAd.Hide();

        Debug.Log("Medium Banner ad loaded");
    }

    private void OnMediumBannerAdClicked(object caller, EventArgs args)
    {
        Debug.Log("Medium Banner ad clicked");
    }

    private void OnLargeBannerLoaded(object caller, EventArgs args)
    {
        _largeBannerAd.Hide();

        Debug.Log("Large Banner ad loaded");
    }

    private void OnLargeBannerAdClicked(object caller, EventArgs args)
    {
        Debug.Log("Large Banner ad clicked");
    }

    private void OnNativeAdLoaded(object caller, EventArgs args)
    {
        ShowNativeAd();
    }

    private void OnError(object caller, AdiveryError error)
    {
        Debug.Log("Placement: " + error.PlacementId + " error: " + error.Reason);
    }
}
