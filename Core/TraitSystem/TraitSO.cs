using UnityEngine;
using UnityEngine.UI;

namespace GM.Traits
{
    public enum TraitType
    {
        SpeedDemon,
        Snail,
    }

    [CreateAssetMenu(fileName = "TraitDataSO", menuName = "SO/TraitDataSO")]
    public class TraitDataSO : ScriptableObject
    {
        public TraitType Type;
        public string Name;
        public Trait Trait;
        public Image Image;
        public bool Negative;
    }
}
