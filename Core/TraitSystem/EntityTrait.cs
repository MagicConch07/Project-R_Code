using GM.Staffs;
using UnityEngine;

namespace GM.Entities
{
    public class EntityTrait : MonoBehaviour, IEntityComponent, IAfterInitable
    {
        public Staff _staff;

        public void Initialize(Entity entity)
        {
            _staff = entity as Staff;
        }

        public void AfterInit()
        {

        }
    }
}
