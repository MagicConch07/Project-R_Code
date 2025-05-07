using System;
using UnityEngine;

namespace GM.Maps
{
    public class MapObjectPart : MonoBehaviour
    {
        public MapObject Owner => _owner;
        private MapObject _owner;

        public event Action OnDestoryObjectEvent;
        public event Action<bool> OnChangeTempObject;

        public virtual void Initialize(MapObject owner)
        {
            _owner = owner;
        }

        public void DestoryPart()
        {
            OnDestoryObjectEvent?.Invoke();

            OnDestoryObjectEvent = null;
            OnChangeTempObject = null;
        }

        public void SetTemp(bool isTemp)
        {
            OnChangeTempObject?.Invoke(isTemp);
        }

        public virtual void ShowInfo(bool isShow)
        {
            // TODO : 오브젝트 정보 추가하기(보류) / 기획 이슈
            Debug.Log($"[Name : {_owner.gameObject.name}] / [Active : {isShow}]");
        }
    }
}
