using UnityEngine;

namespace Game.Environment
{
    public class SkyScroll : MonoBehaviour
    {
        [SerializeField] private SkyLayer[] _skyLayer;

        private void Update()
        {
            foreach (var skyLayer in _skyLayer)
            {
                var offset = Time.time * skyLayer.ScrollSpeed;
                skyLayer.Renderer.material.SetTextureOffset(skyLayer.MainTex, new Vector2(offset, 0));
            }
        }
    }
    
    [System.Serializable]
    public struct SkyLayer
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private float _scrollSpeed;
        private static readonly int mainTex = Shader.PropertyToID("_MainTex");
        public Renderer Renderer => _renderer;
        public float ScrollSpeed => _scrollSpeed;
        public int MainTex => mainTex;
    }
}