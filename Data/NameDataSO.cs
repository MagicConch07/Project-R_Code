using UnityEngine;

namespace GM.Data
{
    [CreateAssetMenu(fileName = "NameDataSO", menuName = "SO/Data/NameDataSO")]
    public class NameDataSO : ScriptableObject
    {
        public string[] FirstNames;
        public string[] LastNames;

        public string GetRandomFirstName()
        {
            return FirstNames[Random.Range(0, FirstNames.Length)];
        }

        public string GetRandomLastName()
        {
            return LastNames[Random.Range(0, LastNames.Length)];
        }
    }
}
