using ExitGames.Client.Photon;
using Network;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace PhotonUtils
{
    public class PhotonSingleton : MonoPunSingleton<PhotonSingleton>
    {
        public void RaiseEvent(byte code, object content, bool allPlayers = true)
        {
            Debug.LogWarningFormat($"Send code {code} {(NetworkEvents) code} with content {content}");
            var raiseOptions = new RaiseEventOptions {Receivers = allPlayers ? ReceiverGroup.All : ReceiverGroup.Others};
            var sendOptions = new SendOptions {Reliability = true};
            PhotonNetwork.RaiseEvent(code, content, raiseOptions, sendOptions);
        }
    }
}
