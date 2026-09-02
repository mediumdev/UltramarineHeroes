using JetBrains.Annotations;
using Photon.Pun;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.SaveManager;

namespace Network
{
    public class NetworkLobbyController : MonoBehaviour
    {
        [SerializeField] private Button _connectButton;
        [SerializeField] private Button _botButton;
        [SerializeField] private GameObject _searchPanel;
        [SerializeField] private TMP_InputField _inputField;

        [UsedImplicitly]
        public void OnConnectClick()
        {
            PhotonNetwork.NickName = _inputField.text;
            NetworkManager.Instance.Connect();
            _connectButton.gameObject.SetActive(false);
            _botButton.gameObject.SetActive(false);
            _searchPanel.gameObject.SetActive(true);
        }

        private static void StartOfflineRoom()
        {
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.CreateRoom(null);
        }
        
        public static void LoadGameSceneBot(int level)
        {
            StartOfflineRoom();
            SaveManager.Add(SavedDataManager.BotLevelKey, level);
            LoadGameScene(SavedDataManager.GameModeSingle);
        }

        public static void LoadGameScene(string mode)
        {
            SaveManager.Add(SavedDataManager.GameModeKey, mode);
            PhotonNetwork.LoadLevel("ServerScene");
        }

        public static void LoadLobbyScene()
        {
            GoToScene.LoadScene("Lobby");
        }
        
        public static void LoadBriefingScene()
        {
            GoToScene.LoadScene("Briefing");
        }
    }
}
