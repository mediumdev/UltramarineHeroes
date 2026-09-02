using Configs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UIWindows.Lobby
{
    public class UpgradeItem : MonoBehaviour
    {
        [Header("Available Item")] 
        [SerializeField] private Image _iconAv;
        [SerializeField] private TextMeshProUGUI _maxCount;

        [Header("Unavailable Item")]
        [SerializeField] private GameObject _unavailable;
        [SerializeField] private Image _iconUn;
        [SerializeField] private TextMeshProUGUI _infoText;

        [Header("Progress Bar")] 
        [SerializeField] private GameObject _progreeBar;
        [SerializeField] private Image _fill;
        [SerializeField] private TextMeshProUGUI _timer;
        [SerializeField] private TextMeshProUGUI _plusCount;
    
        private UpgradeWindow _parent;
        private UnitWithLimits _unitData;

        private UnitConfig Config => _unitData.config;
        private Sprite Icon => Config.Icon;
        private int Limit => _unitData.limit;
        private int GenerationAmount => _unitData.generationAmount;
        private int GenerationSeconds => _unitData.generationSeconds;
        
        private int _targetGenMinutes;
        private int _targetGenAmount;

        public void Init(UnitWithLimits unitData)
        {
            _unitData = unitData;
        
            _iconAv.sprite = Icon;
            _iconUn.sprite = Icon;
            _maxCount.text = "max count: " + Limit;
            var genMinutes = (float) GenerationSeconds / 60 / GenerationAmount;
            if (genMinutes > 1)
            {
                _targetGenMinutes = (int) genMinutes;
                _targetGenAmount = GenerationAmount;
            }
            else
            {
                _targetGenMinutes = 1;
                _targetGenAmount = (int) (1 / genMinutes);
            }
            
            _timer.text = $"{_targetGenMinutes} Min";
            _plusCount.text = $"+{_targetGenAmount}";
        }

        public void CompareWithOldUnit(UnitWithLimits oldUnitData)
        {
            if (oldUnitData.limit == 0)
            {
                _maxCount.color = Color.green;
                _timer.color = Color.green;
                _plusCount.color = Color.green;
                return;
            }
            
            if (_unitData.limit > oldUnitData.limit)
                _maxCount.color = Color.green;
            
            var oldGenMinutes = (float) oldUnitData.generationSeconds / 60 / oldUnitData.generationAmount;
            int oldTargetGenMinutes;
            int oldTargetGenAmount;
            if (oldGenMinutes > 1)
            {
                oldTargetGenMinutes = (int) oldGenMinutes;
                oldTargetGenAmount = GenerationAmount;
            }
            else
            {
                oldTargetGenMinutes = 1;
                oldTargetGenAmount = (int) (1 / oldGenMinutes);
            }
            
            if (oldTargetGenMinutes > _targetGenMinutes || oldTargetGenAmount < _targetGenAmount)
            {
                _timer.color = Color.green;
                _plusCount.color = Color.green;
            }
        }
    }
}
