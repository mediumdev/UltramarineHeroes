using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Configs;
using CoreUtils.Utils;
using DG.Tweening;
using Enums;
using Game.Controllers;
using Packages.CoreUtils.Utils;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
   public class UnitCardMenu : MonoBehaviour
   {
      [SerializeField] private UnitIconItem _unitIconItem;
      [SerializeField] private TowerAbilityIconItem _towerAbilityIconItem;
      [SerializeField] private Transform _container;
      [SerializeField] private CanvasGroup _group;
      [SerializeField] private HorizontalLayoutGroup _horizontal;
      [SerializeField] private float _offset;

      private bool _locked = true;
      private readonly List<UnitIconItem> _items = new List<UnitIconItem>();
      private OutlineActivator _towerOutline;
      private Sequence _seq;

      public void CreateMenu(int i)
      {
         if (!GameController.Instance.GameMachine.IsBattleActive && !GameController.Instance.GameMachine.IsPaused) return;
         
         _locked = true;

         if (_group.alpha > 0)
         {
            DisableTab();
            StartCoroutine(WaitSequence(i));
         }
         
         if (_seq != null)
         {
            StartCoroutine(WaitSequence(i));
         }
         else
         {
            EnableTab(i);
         }
      }

      private IEnumerator WaitSequence(int i)
      {
         yield return new WaitUntil(() => !_seq.IsPlaying());
         EnableTab(i);
      }

      private void EnableTab(int i)
      {
         var position =  _offset * Screen.width/10f;
         var seq = DOTween.Sequence();
         seq.Append(_group.DOFade(1f, 0.3f));
         seq.Insert(0, gameObject.transform.DOMoveY(position, 0.2f));
         
         _container.Clear();
         _items.Clear();
         var list = new List<UnitConfig>();

         var controller = PhotonNetwork.IsMasterClient
            ? GameController.Instance.Player
            : GameController.Instance.Enemy;
         foreach (var unit in controller.Collection)
         {
            if (unit.SetupProperties.Any(x=> x.SetupType==(LineType)i))
               list.Add(unit);
         }

         foreach (var towerVisual in controller.TowerVisual)
         {
            towerVisual.SetOutline(false);
         }
         _towerOutline = controller.TowerVisual[i];
         _towerOutline.SetOutline(true);

         var abilityItem = Instantiate(_towerAbilityIconItem, _container);
         abilityItem.Init(controller.Towers[i].Faction.ActiveAbilityConfig, (LineType) i, controller.Towers[i].Faction.ActiveAbilityManaCost);
         
         var sorted = list.ToList();
         sorted.Sort(Comparison);

         switch (sorted.Count)
         {
            case int n when n < 3:
               _horizontal.spacing = -650;
               break;
            
            case int n when n == 3:
               _horizontal.spacing = -450;
               break;
            
            case int n when n > 3:
               _horizontal.spacing = -285;
               break;
         }

         foreach (var unit in sorted)
         {
            var item = Instantiate(_unitIconItem, _container);
            item.Init(unit, (LineType) i);
            item.SpawnEvent += GlobalCooldown;
            _items.Add(item);
         }

         StartCoroutine(WaitUnlock());
      }

      private void DisableTab()
      {
         var position =  _offset * Screen.width/10f;
         _seq = DOTween.Sequence();
         _seq.Append(_group.DOFade(0, 0.2f));
         _seq.Insert(0, gameObject.transform.DOMoveY(-position, 0.3f));
         _towerOutline.SetOutline(false);
      }

      private void GlobalCooldown()
      {
         foreach (var item in _items)
            item.Cooldown();
      }

      private IEnumerator WaitUnlock()
      {
         yield return new WaitForEndOfFrame();
         _locked = !gameObject.activeSelf;
      }
      
      private int Comparison(UnitConfig x, UnitConfig y)
      {
         return x.Cost > y.Cost ? 1 : -1;
      }
   
      private void Update()
      {
         if (!_locked && Input.GetMouseButtonDown(0) && !ObjectButton.IsPointerOverUiElement())
         {
            DisableTab();
         }
      }
   }
}
