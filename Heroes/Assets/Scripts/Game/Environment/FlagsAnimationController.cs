using UnityEngine;

public class FlagsAnimationController : MonoBehaviour
{
    [SerializeField, Tooltip("Разница во времени активации анимаций")]
    private float _delay = .25f;
    [SerializeField, Tooltip("Компоненты анимации на флажках")]
    private Animation[] _animations;

    void OnEnable()
    {
        if (_animations.Length < 1)
        {
            Debug.LogWarning($"Can't control flags animations. Add animation components to script on object {name}");
        }
        else
        {
            var clip = _animations[0].clip;
            if (clip == null)
            {
                Debug.LogError($"There is no clip in animation component! Object {name}");
                return;
            }
            
            var animationName = clip.name;
            var timer = 0f;
        
            foreach (var a in _animations)
            {
                if (a.clip.name != animationName)
                {
                    Debug.LogWarning($"Animation clip on {a.name} is different. Please check");
                }
                else
                {
                    var startTime = timer % clip.length;
                    a[animationName].time = startTime;
                    timer += _delay;
                }
            }
        }
    }
}
