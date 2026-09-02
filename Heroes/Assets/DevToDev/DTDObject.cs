using DevToDev.Analytics;
using UnityEngine;

public class DTDObject : MonoBehaviour
{
        public static DTDObject Instance;
        
        private void Awake()
        {
                if (Instance != null)
                        Destroy(gameObject);
                else
                        Instance = this;
                
                DontDestroyOnLoad(this);
        }

        void Start()
    {

#if UNITY_ANDROID
        DTDAnalytics.Initialize("3e9eb4c8-4705-0a01-9686-9044bc3a3681");
#elif UNITY_IOS
        DTDAnalytics.Initialize("iosAppID");
#elif UNITY_WEBGL
        DTDAnalytics.Initialize("webglAppID");
            
#elif UNITY_STANDALONE_WIN
            var winPlatformConfig = new DTDAnalyticsConfiguration
            {
                    ApplicationVersion = "0.1",
                    LogLevel = DTDLogLevel.Debug,
                    TrackingAvailability = DTDTrackingStatus.Disable, // Disable для ПК, чтобы не загружать статистику с запусков в юнити
                    CurrentLevel = 1,
                    UserId = "unique_userId"
            };
            DTDAnalytics.Initialize("3e9eb4c8-4705-0a01-9686-9044bc3a3681", winPlatformConfig);
            
#elif UNITY_STANDALONE_OSX
        DTDAnalytics.Initialize("osxAppID");
#elif UNITY_WSA
        DTDAnalytics.Initialize("wsaAppID");
#endif
    }
}