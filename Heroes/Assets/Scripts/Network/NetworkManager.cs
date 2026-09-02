using Photon.Pun;
using Photon.Realtime;
using PhotonUtils;
using UnityEngine;

namespace Network
{
    public class NetworkManager : MonoPunSingleton<NetworkManager>
    {
        protected override void Init()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            DontDestroyOnLoad(this);
        }

        public void Connect()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = Application.version;
            }
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
            Debug.LogWarningFormat("Init server {0}", RoomServer.Instance);
        }

        public override void OnJoinedLobby()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogErrorFormat("Error {0} message {1}", returnCode, message);
        }

        public override void OnJoinedRoom()
        {
            if (!PhotonNetwork.IsMasterClient)
                PhotonSingleton.Instance.RaiseEvent((byte)NetworkEvents.PlayerTwoJoined, null);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            PhotonSingleton.Instance.RaiseEvent((byte) NetworkEvents.PlayerDisconnected, null);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.LogWarningFormat("Can't join {0}", message);
            PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = 2, IsVisible = true});
        }
    }
}
