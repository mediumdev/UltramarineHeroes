using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ui.Utils
{
    [ExecuteInEditMode]
    public class Ui3DMesh : MonoBehaviour
    {
        [SerializeField] private List<MeshFilter> _meshFilters = new List<MeshFilter>();

        public List<MeshFilter> Filters => _meshFilters;

        public void ResetData ()
        {
            _meshFilters.ForEach (delegate (MeshFilter meshFilter)
            {
                var cr = meshFilter.transform.GetComponent<CanvasRenderer>();
                cr.SetMesh (meshFilter.sharedMesh);

                var materials = new List<Material>();
                var meshRenderer = meshFilter.transform.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                    materials = meshRenderer.sharedMaterials.ToList();
                else
                {
                    var skinRenderer = meshFilter.transform.GetComponent<SkinnedMeshRenderer>();
                    materials = skinRenderer.sharedMaterials.ToList();
                }
 
                for (var i = 0; i < materials.Count; i++)
                {
                    cr.materialCount = materials.Count;
                    cr.SetMaterial  (materials[i], i);
                }
            });
        }

        private void OnEnable()
        {
            ResetData();
        }

        private void OnValidate()
        {
            ResetData();
        }
    }
}