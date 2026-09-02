using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using CoreConfigs.Configs;
using System.Linq;

public static class AdManager
{
    private static IAdProvider _adProvider = null;
    
    public static void Initialize()
    {
        AdKeysConfig _adKeysConfig = null;
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        _adKeysConfig = ConfigBase.LoadAll<AdKeysConfig>().First();

        string _appID = _adKeysConfig._appID;
        string _rewardedPlacement = _adKeysConfig._rewardedPlacement;
        string _interstitialPlacement = _adKeysConfig._interstitialPlacement;
        string _bannerPlacement = _adKeysConfig._bannerPlacement;
        string _mediumBannerPlacement = _adKeysConfig._mediumBannerPlacement;
        string _largeBannerPlacement = _adKeysConfig._largeBannerPlacement;
        string _nativePlacement = _adKeysConfig._nativePlacement;

        _adProvider = new AdiveryProvider(); // or UnityAdsProvider, etc.
        _adProvider.Initialize(_appID, _rewardedPlacement, _interstitialPlacement, _bannerPlacement, _mediumBannerPlacement, _largeBannerPlacement, _nativePlacement);
    }

    public static void LoadRewardedAd()
    {
       _adProvider.LoadRewardedAd();
    }

    public static void LoadInterstitialAd()
    {
        _adProvider.LoadInterstitialAd();
    }

    public static void LoadBannerAd()
    {
        _adProvider.LoadBannerAd();
    }

    public static void LoadMediumBannerAd()
    {
        _adProvider.LoadMediumBannerAd();
    }

    public static void LoadLargeBannerAd()
    {
        _adProvider.LoadLargeBannerAd();
    }

    public static void LoadNativeAd()
    {
        _adProvider.LoadLargeBannerAd();
    }

    public static void ShowRewardedAd()
    {
        if (_adProvider.RewardedAdIsLoaded())
        {
            _adProvider.ShowRewardedAd();
        }
    }

    public static void ShowInterstitialAd()
    {
        if (_adProvider.InterstitialAdIsLoaded())
        {
            _adProvider.ShowInterstitialAd();
        }
    }

    public static void ShowBannerAd()
    {
        if (_adProvider.BannerAdIsLoaded())
        {
            _adProvider.ShowBannerAd();
        }
    }

    public static void ShowMediumBannerAd()
    {
        if (_adProvider.MediumBannerAdIsLoaded())
        {
            _adProvider.ShowMediumBannerAd();
        }
    }

    public static void ShowLargeBannerAd()
    {
        if (_adProvider.LargeBannerAdIsLoaded())
        {
            _adProvider.ShowLargeBannerAd();
        }
    }

    public static void ShowNativeAd()
    {
        _adProvider.ShowLargeBannerAd();
    }

    public static bool RewardedAdIsLoaded()
    {
        return _adProvider.RewardedAdIsLoaded();
    }

    public static bool InterstitialAdIsLoaded()
    {
        return _adProvider.InterstitialAdIsLoaded();
    }

    public static bool BannerAdIsLoaded()
    {
        return _adProvider.BannerAdIsLoaded();
    }

    public static bool MediumBannerAdIsLoaded()
    {
        return _adProvider.MediumBannerAdIsLoaded();
    }

    public static bool LargeBannerAdIsLoaded()
    {
        return _adProvider.LargeBannerAdIsLoaded();
    }
}
