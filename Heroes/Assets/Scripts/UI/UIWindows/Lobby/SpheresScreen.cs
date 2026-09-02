using System.Collections;
using System.Collections.Generic;
using Configs;
using CoreUtils.Utils;
using Enums;
using Game.Controllers;
using JetBrains.Annotations;
using UI.UIWindows.Lobby;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpheresScreen : MonoBehaviour
{
    [SerializeField] private RectTransform _container;
    [SerializeField] private GameObject _screen;
    [SerializeField] private SphereItem _item;

    private bool _forceDisabled;
    private readonly List<CurrencyConfig> _list = new List<CurrencyConfig>();

    private void OnEnable()
    {
        CreateList();
        CreateSectionPanel();
    }

    private void CreateList()
    {
        _list.Clear();
        
        foreach (var sphere  in CurrencyManager.Instance.CurrencyConfigsWithType(CurrencyType.Sphere))
        {
            _list.Add(sphere);
        }
    }

    private void CreateSectionPanel()
    {
        _container.Clear();
        
        foreach (var sphere in _list)
        {
            var value = CurrencyManager.Instance.CurrencyValue(sphere);
            var item = Instantiate(_item, _container);
            item.Init(sphere, value);
        }
    }

    private bool IsFocused
    {
        get
        {
            var obj = GetSelectedObject();
            if (obj == null) return false;
            
            var objTransform = obj.transform;
            while (objTransform.parent != null)
            {
                if (obj == gameObject) return true;
                
                objTransform = objTransform.parent;
                obj = objTransform != null ? objTransform.gameObject : null;
            }
            return false;
        }
    }

    private GameObject GetSelectedObject()
    {
        var pointerData = new PointerEventData(EventSystem.current);
        pointerData.pointerId = -1;
        pointerData.position = Input.mousePosition;
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData,results);
        return results.Count > 0 ? results[0].gameObject : null;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsFocused)
        {
            _forceDisabled = true;
            StartCoroutine(DisableCoroutine());
        }
    }

    [UsedImplicitly]
    public void SetScreenActive()
    {
        if (_forceDisabled) return;
        
        _screen.SetActive(!_screen.activeSelf);
    }

    private IEnumerator DisableCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        _screen.SetActive(false);
        _forceDisabled = false;
    }
}
