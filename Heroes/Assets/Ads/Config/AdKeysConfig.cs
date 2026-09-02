using System;
using System.Collections;
using System.Collections.Generic;
using CoreConfigs.Configs;
using UnityEditor;
using UnityEngine;

public class AdKeysConfig : ConfigBase
{
    [SerializeField] public string _appID;
    [SerializeField] public string _rewardedPlacement;
    [SerializeField] public string _interstitialPlacement;
    [SerializeField] public string _bannerPlacement;
    [SerializeField] public string _mediumBannerPlacement;
    [SerializeField] public string _largeBannerPlacement;
    [SerializeField] public string _nativePlacement;

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Configs/AdKeysConfig")]
    private static void Create()
    {
        CreateAsset<AdKeysConfig>();
    }
#endif
}
