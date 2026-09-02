using System;
using System.Collections.Generic;
using Enums;
using Game.Controllers;
using Photon.Pun;
using UnityEngine;

namespace Game.Flags
{
    public class FlagUnit : MonoBehaviourPun, IPunObservable
    {
        [SerializeField] private PlayerType _player = PlayerType.None;
        [SerializeField] private GameObject _fxNeutral;
        [SerializeField] private GameObject _fxPlayer;
        [SerializeField] private GameObject _fxEnemy;
        [SerializeField] private LineType _line;
        [SerializeField] private int _resourcePerCapture = 1;

        private List<GameObject> _fxList;

        public PlayerType Player => _player;
        public LineType Line => _line;

        private void OnEnable()
        {
            _fxList = new List<GameObject> {_fxNeutral, _fxPlayer, _fxEnemy};
            Repaint();
        }

        private void SetFx(GameObject fx)
        {
            foreach (var fxObject in _fxList)
                fxObject.SetActive(fxObject == fx);
        }

        private void Repaint()
        {
            switch (_player)
            {
                case PlayerType.None:
                    SetFx(_fxNeutral);
                    break;
                case PlayerType.Player:
                    SetFx(_fxPlayer);
                    break;
                case PlayerType.Enemy:
                    SetFx(_fxEnemy);
                    break;
                default:
                    Debug.LogError($"Unknown type of player <<{_player}>>");
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Capture(PlayerType player)
        {
            if (player == _player) return;
            _player = player;
            
            switch (_player)
            {
                case PlayerType.Player:
                    GameController.Instance.Player.ChangeMana(_resourcePerCapture);
                    break;
                case PlayerType.Enemy:
                    GameController.Instance.Enemy.ChangeMana(_resourcePerCapture);
                    break;
            }
            Repaint();
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting && PhotonNetwork.IsMasterClient)
            {
                stream.SendNext(_player);
            }

            if (stream.IsReading)
            {
                _player = (PlayerType) stream.ReceiveNext();
                Repaint();
            }
        }
    }
}