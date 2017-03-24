using UnityEditor.Animations;
using UnityEngine;

namespace Assets.Views.Units
{
    [CreateAssetMenu(menuName = "Units/Views/Unit View Details")]
    public class UnitViewDetails : ScriptableObject
    {
        public Sprite Sprite;
        public AnimatorController Animator;

        public Material DefaultMaterial;
        public Material HighlightedMaterial;
        public Material SelectedMaterial;
    }
}
