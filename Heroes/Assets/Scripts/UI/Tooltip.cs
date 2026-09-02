using System;
using Configs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _damage;
    [SerializeField] private TextMeshProUGUI _range;
    [SerializeField] private TextMeshProUGUI _hp;
    [SerializeField] private TextMeshProUGUI _speed;
    [SerializeField] private TextMeshProUGUI _type;
    [SerializeField] private TextMeshProUGUI _loreText;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _gems;
    [SerializeField] private TextMeshProUGUI _count;
    [SerializeField] private Image[] _lines;

    [Header("Visual Part")]
    [SerializeField] private Image _border;
    [SerializeField] private Image _titleBack;
    [SerializeField] private Image _bottom;
    [SerializeField] private Image _crystallFrame;
    [SerializeField] private Image _linesFrame;
    [SerializeField] private Image _limitFrame;
    [Space]
    [SerializeField] private Sprite _usualBorder;
    [SerializeField] private Sprite _usualTitleBack;
    [SerializeField] private Sprite _usualBottom;
    [SerializeField] private Sprite _usualCrystallFrame;
    [SerializeField] private Sprite _usualLinesFrame;
    [SerializeField] private Sprite _usualLimitFrame;
    [SerializeField] private Sprite _mercenaryBorder;
    [SerializeField] private Sprite _mercenaryTitleBack;
    [SerializeField] private Sprite _mercenaryBottom;
    [SerializeField] private Sprite _mercenaryCrystallFrame;
    [SerializeField] private Sprite _mercenaryLinesFrame;
    [SerializeField] private Sprite _mercenaryLimitFrame;
    public void init(UnitConfig config)
    {
        
        if (!string.IsNullOrEmpty(config.Name))
        {
            _title.gameObject.SetActive(true);
            _title.text = config.Name;
        }

        if (config.Icon != null)
        {
            _icon.gameObject.SetActive(true);
            _icon.sprite = config.Icon;
        }
        
        if (!string.IsNullOrEmpty(config.Damage.ToString()))
        {
            _damage.gameObject.SetActive(true);
            _damage.text = config.Damage.ToString();
        }
        
        if (!string.IsNullOrEmpty(config.AttackRange.ToString()))
        {
            _range.gameObject.SetActive(true);
            _range.text = config.AttackRange.ToString();
        }
        
        if (!string.IsNullOrEmpty(config.HitPoints.ToString()))
        {
            _hp.gameObject.SetActive(true);
            _hp.text = config.HitPoints.ToString();
        }
        
        if (!string.IsNullOrEmpty(config.MoveSpeed.ToString()))
        {
            _speed.gameObject.SetActive(true);
            _speed.text = config.MoveSpeed.ToString();
        }
        
        if (!string.IsNullOrEmpty(config.AbilityDescription))
        {
            _type.gameObject.SetActive(true);
            _type.text = config.AbilityDescription;
        }
        
        if (!string.IsNullOrEmpty(config.LoreText))
        {
            _loreText.gameObject.SetActive(true);
            _loreText.text = config.LoreText;
        }
        
        if (!string.IsNullOrEmpty(config.MaxCount.ToString()))
        {
            _count.gameObject.SetActive(true);
            _count.text = config.MaxCount.ToString();
        }
        
        if (!string.IsNullOrEmpty(config.Cost.ToString()))
        {
            _gems.gameObject.SetActive(true);
            _gems.text = config.Cost.ToString();
        }

        foreach (var image in _lines)
        {
            image.gameObject.SetActive(false);
            
            foreach (var setupProperty in config.SetupProperties)
            {
                _lines[(int) setupProperty.SetupType].gameObject.SetActive(true);
            }
        }

        _border.sprite = config.IsMercenary ? _mercenaryBorder : _usualBorder;
        _bottom.sprite = config.IsMercenary ? _mercenaryBottom : _usualBottom;
        _titleBack.sprite = config.IsMercenary ? _mercenaryTitleBack : _usualTitleBack;
        _crystallFrame.sprite = config.IsMercenary ? _mercenaryCrystallFrame : _usualCrystallFrame;
        _linesFrame.sprite = config.IsMercenary ? _mercenaryLinesFrame : _usualLinesFrame;
        _limitFrame.sprite = config.IsMercenary ? _mercenaryLimitFrame : _usualLimitFrame;
    }

    public void RepaintBorders()
    {
        
    }
}
