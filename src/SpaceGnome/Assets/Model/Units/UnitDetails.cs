using UnityEngine;

namespace Assets.Model.Units
{
    [CreateAssetMenu(menuName = "Units/Model/Unit Details")]
    public class UnitDetails : ScriptableObject
    {
        public string UnitName;
        public float MovementSpeed;
    }
}
