using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Configs;
using CoreUtils.Utils;
using Enums;
using JetBrains.Annotations;
using Ui.Utils;
using UnityEngine;

namespace UI.UIWindows.Lobby
{
    public class LobbyTower : MonoBehaviour
    {
        [SerializeField] private Ui3DMesh _item;

        private List<FactionConfig> _factions;
        private LobbyWindow _window;
        private int _currentIndex;
        private LineType _type;
        private Transform _container;
        
        public void Init(List<FactionConfig> factions, LobbyWindow parent, string[] loadedUids, LineType type)
        {
            _factions = factions;
            _window = parent;
            _type = type;
            var targetUid = loadedUids.FirstOrDefault(x => _factions.Any(y => string.Equals(x, y.Uid)));
            if (!string.IsNullOrEmpty(targetUid))
            {
                var item = _factions.FirstOrDefault(x => string.Equals(x.Uid, targetUid));
                if (item != null)
                    _currentIndex = _factions.IndexOf(item);
            }

            if (_item == null)
                _item = GetComponentInChildren<Ui3DMesh>();

            StartCoroutine(CreateContainer());
        }

        private IEnumerator CreateContainer()
        {
            yield return new WaitForEndOfFrame();

            var position = _item.transform.position;
            position.z = -Camera.main.transform.position.z;
            var screenPos = Camera.main.ScreenToWorldPoint(position);
            
            if (_container != null)
                Destroy(_container.gameObject);
            
            _container = new GameObject("Container").transform;
            _container.transform.position = new Vector3(0,0,0);
            _container.Rotate(0, 0, 0);

            Repaint(); 
        }

        private void OnDisable()
        {
            if (_container != null)
                Destroy(_container.gameObject);
        }


        private void Repaint()
        {
            _container.Clear();
            var obj = Instantiate(_factions[_currentIndex].TownObject, _container);
            //obj.transform.localPosition = new Vector3(0, -0.5f, 0.15f);
            //obj.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            _window.ChangeFaction(_type, _factions[_currentIndex]);
        }

        [UsedImplicitly]
        public void ChangeFaction(int value)
        {
            _currentIndex += value;
            if (_currentIndex < 0)
                _currentIndex = _factions.Count - 1;
            if (_currentIndex >= _factions.Count)
                _currentIndex = 0;
            
            Repaint();
        }
    }
}
