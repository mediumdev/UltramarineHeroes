using System.Net;
using Enums;
using SoundPool;
using UnityEngine;

namespace Game.Units
{
    public class EntryFX : MonoBehaviour
    {
        [SerializeField] private bool _needCheckPlayerType;
        [SerializeField] private FxPlayer _entryFxPlayer;
        [SerializeField] private FxPlayer _entryFxEnemy;
        [SerializeField] protected Vector3 _fxPosition = Vector3.zero;
        [SerializeField] protected Vector3 _fxScale = Vector3.one;
        [SerializeField] private SoundConfig _soundFx;
        private FxPlayer _fx;

        private void OnEnable()
        {
            if (_entryFxPlayer is null) return;
            
            var rotation = _entryFxPlayer.transform.rotation.eulerAngles;
            if (_needCheckPlayerType)
            {
                var unitController = GetComponent<UnitController>();
                if (unitController is null) return;
                
                if (unitController.PlayerType == PlayerType.Player)
                    _fx = _entryFxPlayer.Create(transform, _fxPosition, _fxScale, rotation);
                else if (unitController.PlayerType == PlayerType.Enemy)
                    _fx = _entryFxEnemy.Create(transform, _fxPosition, _fxScale, rotation);
            }
            else
            {
                _fx = _entryFxPlayer.Create(transform, _fxPosition, _fxScale, rotation);
            }
            SoundManager.Instance.Play(_soundFx);
        }

        private void OnDisable()
        {
            if (_fx != null)
                _fx.Stop();
        }

        public void StopFx()
        {
            if (_fx != null)
                _fx.Stop();
        }
    }
}
