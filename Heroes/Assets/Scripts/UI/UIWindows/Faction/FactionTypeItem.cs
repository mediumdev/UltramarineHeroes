using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Configs;
using CoreUtils.Utils;
using Enums;
using JetBrains.Annotations;
using RTLTMPro;
using TMPro;
using Ui.Utils;
using UnityEngine;

namespace UI.UIWindows.Faction
{
    public class FactionTypeItem : MonoBehaviour
    {
        [SerializeField] private Ui3DMesh _item;
        [SerializeField] private RTLTextMeshPro _towerName;

        private List<FactionConfig> _factions;
        private FactionWindow _parentWindow;
        private int _currentIndex;
        private LineType _lineType;
        private Transform _container;
        
        public void Init(List<FactionConfig> factions, FactionWindow parent, string[] loadedUids, LineType lineType)
        {
            _factions = factions;
            _parentWindow = parent;
            _lineType = lineType;
            
            var targetUid = loadedUids.FirstOrDefault(x => _factions.Any(y => string.Equals(x, y.Uid)));
            if (!string.IsNullOrEmpty(targetUid))
            {
                var item = _factions.FirstOrDefault(x => string.Equals(x.Uid, targetUid));
                if (item != null)
                    _currentIndex = _factions.IndexOf(item);
            }

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
            _container.transform.position = screenPos;
            _container.Rotate(0, 180, 0);

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
            var obj = Instantiate(_factions[_currentIndex].TowerObject, _container);
            obj.transform.localPosition = new Vector3(0, -0.5f, 0.15f);
            obj.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            _parentWindow.ChangeFaction(_lineType, _factions[_currentIndex]);
            _towerName.text = _factions[_currentIndex].FactionName;
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
