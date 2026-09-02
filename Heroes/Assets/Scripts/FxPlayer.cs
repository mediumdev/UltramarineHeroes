using System.Collections;
using UnityEngine;

public class FxPlayer : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private bool _destroyAfterDuration = true;
    [SerializeField] private float _customDuration = 0f;
    [SerializeField] private float _length = 8f;

    public float Duration => _particleSystem.main.duration;
    public float Length => _length;

    private void OnValidate()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    public FxPlayer Create(Transform parent, Vector3 position, Vector3 scale, Vector3 rotation)
    {
        var fx = Instantiate(this, parent);
        var fxTransform = fx.transform;
        fxTransform.localPosition = position;
        fxTransform.localRotation = Quaternion.Euler(rotation);
        fxTransform.localScale = scale;
        Play();
        return fx;
    }

    public void Play()
    {
        gameObject.SetActive(true);
    }

    public void Stop()
    {
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        if (_destroyAfterDuration)
            StartCoroutine(EndEffect());
    }

    private IEnumerator EndEffect()
    {
        var main = _particleSystem.main;
        var duration = _customDuration > 0 ? _customDuration : main.duration;
        yield return new WaitForSeconds(duration);
        Stop();
    }
}