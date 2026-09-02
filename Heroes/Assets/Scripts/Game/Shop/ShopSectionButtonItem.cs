using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
    public class ShopSectionButtonItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _sectionTitle;
        [SerializeField] private GameObject _sectionButtonObj;
        private int _sectionNumber;
        private ShopSection _shopSection;
        private bool _active;
        private bool _needShow;
        private ShopWindow _shopWindow;

        public void Init(int sectionNumber, ShopSection shopSection, bool active, bool needShow, ShopWindow shopWindow)
        {
            _shopWindow = shopWindow;
            _sectionNumber = sectionNumber;
            _shopSection = shopSection;
            _active = active;
            _needShow = needShow;
            
            Repaint();
        }

        private void Repaint()
        {
            if (_needShow)
            {
                _sectionTitle.text = _shopSection.SectionTitle;
                _sectionButtonObj.GetComponentInChildren<Mask>().showMaskGraphic = !_active;
                if(_active)
                    _sectionTitle.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 1), 1f, 3);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void OpenSection()
        {
            _shopWindow.OpenSection(_sectionNumber);
        }

        public void PlayButtonSoundFromParent()
        {
            var buttonSound = GetComponentInParent<ButtonSound>();
            buttonSound.PlayButtonSound();
        }
    }
}