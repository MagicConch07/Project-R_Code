using UnityEngine;

namespace GM.Data
{
    public enum LanguageType
    {
        Korean,
        English
    }

    [CreateAssetMenu(fileName = "StaffInfoGenerator", menuName = "SO/Data/StaffInfoGenerator")]
    public class StaffProfileGeneratorSO : ScriptableObject
    {
        public LanguageType LanguageType;
        public NameDataSO NameContainer;
        public PortraitDataSO PortraitContainer;

        public StaffProfile GetRandomStaffInfo()
        {
            StaffProfile staffInfo = new StaffProfile
            {
                Name = $"{NameContainer.GetRandomFirstName()}{NameContainer.GetRandomLastName()}",
                Portrait = PortraitContainer.GetRandomPortrait()
            };

            if (LanguageType == LanguageType.Korean)
            {
                return staffInfo;
            }
            else if (LanguageType == LanguageType.English)
            {
                staffInfo.Name = $"{NameContainer.GetRandomLastName()} {NameContainer.GetRandomFirstName()}";
            }

            return staffInfo;
        }
    }
}
