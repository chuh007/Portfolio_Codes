using UnityEngine;

namespace Work.CUH.Code.SwitchSystem
{
    [DefaultExecutionOrder(-1)]
    public class ColorLinkObject : MonoBehaviour
    {
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        
        private Material _renderMaterial;

        private void Awake()
        {
            _renderMaterial = GetComponent<Renderer>().material;
        }

        public void SetLinkColor(Color color)
            => _renderMaterial.SetColor(BaseColor, color);

        public Color GetLinkColor() => _renderMaterial.color;
    }
}