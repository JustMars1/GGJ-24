using UnityEngine;

namespace NPC
{
    [CreateAssetMenu(fileName = "Color Palette", menuName = "Color Palette", order = 0)]
    public class ColorPalette : ScriptableObject
    {
        [SerializeField] Color _hat = Color.white;
        public Color hat => _hat;
        
        [SerializeField] Color _neckThing = Color.white;
        public Color neckThing => _neckThing;
        
        [SerializeField] Color _coat =  Color.white;
        public Color coat => _coat;
        
        [SerializeField] Color _gloves =  Color.white;
        public Color gloves => _gloves;
        
        [SerializeField] Color _pants =  Color.white;
        public Color pants => _pants;
        
        [SerializeField] Color _boots =  Color.white;
        public Color boots => _boots;
        
        [SerializeField] Color _scarf =  Color.white;
        public Color scarf => _scarf;
    }
}