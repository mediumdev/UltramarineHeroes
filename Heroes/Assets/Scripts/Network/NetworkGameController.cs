using System;
using com.adjust.sdk;
using CoreConfigs.Configs;
using JetBrains.Annotations;
using Photon.Pun;
using PhotonUtils;
using UnityEngine;
using Utils;
using Utils.SaveManager;

namespace Network
{
    public class NetworkGameController : MonoBehaviour
    {
        [SerializeField] private GameObject _readyPanel;
        [SerializeField] private ConfigLibrary _library;

        private void Awake()
        {
            if (!ConfigLibrary.Loaded)
                ConfigLibrary.LoadLibrary(_library);
        }

        private void Start()
        {
            //ADJUSTEVENT
            //AdjustEvent app_load = new AdjustEvent("s7qw53");
            //Adjust.trackEvent(app_load);

            var collectionString = SaveManager.GetValue(SavedDataManager.PlayerCollectionKey, string.Empty);
            var factionString = SaveManager.GetValue(SavedDataManager.PlayerFactionsKey, string.Empty);
            
            object[] data = { PhotonNetwork.IsMasterClient, collectionString.Split(';'), factionString.Split(';') };
            PhotonSingleton.Instance.RaiseEvent((byte) NetworkEvents.CollectionLoaded, data);
            
            PhotonSingleton.Instance.RaiseEvent((byte)NetworkEvents.PlayerReady, null);
        }

        /*[UsedImplicitly]
        public void OnReadyClick()
        {
            _readyPanel.gameObject.SetActive(false);
            var collectionString = SaveManager.GetValue(SavedDataManager.PlayerCollectionKey, string.Empty);
            var factionString = SaveManager.GetValue(SavedDataManager.PlayerFactionsKey, string.Empty);
            
            object[] data = { PhotonNetwork.IsMasterClient, collectionString.Split(';'), factionString.Split(';') };
            PhotonSingleton.Instance.RaiseEvent((byte) NetworkEvents.CollectionLoaded, data);
            
            PhotonSingleton.Instance.RaiseEvent((byte)NetworkEvents.PlayerReady, null);
        }*/
    }
}
