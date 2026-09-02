using UnityEngine;

namespace Game.Environment
{
    public class TowerVisualChanger : MonoBehaviour
    {
        [SerializeField] private GameObject _visual;
        [SerializeField] private GameObject _fxBlue;
        [SerializeField] private GameObject _fxRed;
        [SerializeField] private Animator _animator;
        [SerializeField] private ParticleSystem _towerDamagedFx;
        public Animator Animator => _animator;
        public void Swap()
        {
            _visual.transform.RotateAround(_visual.transform.position, _visual.transform.up, 180);
            var scale = _visual.transform.localScale;
           // _visual.transform.localScale = new Vector3(-1 * scale.x, scale.y, scale.z);
            _fxBlue.SetActive(false);
            _fxRed.SetActive(true);
        }

        public void PlayTowerFX()
        {
            if(_towerDamagedFx == null) return;
            _towerDamagedFx.gameObject.SetActive(true);
            _towerDamagedFx.Play();
        }
    }
}