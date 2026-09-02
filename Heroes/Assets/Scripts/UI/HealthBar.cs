using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

        public void SetHealth(int health)
        {
            _slider.value = health;
        }

        public void SetMaxHealth(int health)
        {
            _slider.maxValue = health;
            _slider.value = health;
        }
    }
}
