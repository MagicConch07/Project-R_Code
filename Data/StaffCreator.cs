using GM.Data;
using GM.Staffs;
using UnityEngine;

namespace GM
{
    public class StaffCreator : MonoBehaviour
    {
        [SerializeField] private StaffProfileGeneratorSO _staffInfoGenerator;
        [SerializeField] private StaffController _staffHandlerPrefab;

        public StaffInfo CreateStaff()
        {
            StaffProfile staffProfile = _staffInfoGenerator.GetRandomStaffInfo();
            StaffInfo staffInfo = new StaffInfo(staffProfile, Random.Range(10, 100));

            return staffInfo;
        }
    }
}
