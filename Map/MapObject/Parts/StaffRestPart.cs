using GM.Staffs;
using UnityEngine;

namespace GM.Maps
{
    public class StaffRestPart : MapObjectPart
    {
        public StaffType StaffType => _staffType;
        [SerializeField] private StaffType _staffType;

        [SerializeField] private Transform _staffVisual;

        public override void Initialize(MapObject owner)
        {
            base.Initialize(owner);
        }

        public override void ShowInfo(bool isShow)
        {
            // TODO : Create Info Trm Class
        }

        public void SetStaffVisual(bool isActive)
        {
            _staffVisual.gameObject.SetActive(isActive);
        }
    }
}
