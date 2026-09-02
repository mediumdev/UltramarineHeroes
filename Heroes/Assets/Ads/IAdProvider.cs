public interface IAdProvider
{
    public void Initialize(string appID, string rewardedPlacement, string interstitialPlacement, string bannerPlacement, string mediumBannerPlacement, string largeBannerPlacement, string nativePlacement);
    public void LoadRewardedAd();
    public void ShowRewardedAd();
    public bool RewardedAdIsLoaded();
    public void LoadInterstitialAd();
    public void ShowInterstitialAd();
    public bool InterstitialAdIsLoaded();
    public void LoadBannerAd();
    public void ShowBannerAd();
    public bool BannerAdIsLoaded();
    public void LoadMediumBannerAd();
    public void ShowMediumBannerAd();
    public bool MediumBannerAdIsLoaded();
    public void LoadLargeBannerAd();
    public void ShowLargeBannerAd();
    public bool LargeBannerAdIsLoaded();
    public void LoadNativeAd();
    public void ShowNativeAd();
}
