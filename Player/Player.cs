using UnityEngine;
using GM.Entities;

namespace GM.Players
{
    public class Player : Entity
    {
        public InputReaderSO Input => _input;
        [SerializeField] private InputReaderSO _input;
    }
}
