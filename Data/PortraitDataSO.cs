using UnityEngine;

namespace GM.Data
{
    [CreateAssetMenu(fileName = "PortraitDataSO", menuName = "SO/Data/PortraitDataSO")]
    public class PortraitDataSO : ScriptableObject
    {
        public Sprite[] Portraits;

        public Sprite GetRandomPortrait()
        {
            return Portraits[Random.Range(0, Portraits.Length)];
        }
    }
}
