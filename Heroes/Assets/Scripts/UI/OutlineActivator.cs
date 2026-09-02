using UnityEngine;

namespace UI
{
    public class OutlineActivator : MonoBehaviour
    {
        [SerializeField] private Renderer _mesh;
        [SerializeField] private Material _outlineMaterial;
        [SerializeField] private GameObject _background;
        [SerializeField] private Material _oldMaterial;
        [SerializeField] private Texture _towerTexture;

        private void OnValidate()
        {
            _oldMaterial = _mesh.sharedMaterial;
            _towerTexture = _oldMaterial.mainTexture;
        }

        public void SetOutline(bool condition)
        {
            _mesh.material = condition ? _outlineMaterial : _oldMaterial;
            _mesh.material.mainTexture = _towerTexture;
            _background.SetActive(condition);
        }
    }
}