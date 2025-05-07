using UnityEngine;

namespace GM.Players.Pickers.Maps
{
    public interface IMouseMapEditable
    {
        public void EditMapWithMouse(Vector2Int position);
    }
}
