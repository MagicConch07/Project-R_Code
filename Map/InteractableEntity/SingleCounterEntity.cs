using System.Collections.Generic;
using UnityEngine;

namespace GM.InteractableEntities
{
    public class SingleCounterEntity : InteractableEntity
    {
        private List<Customer> lineCustomerList = new();

        public Transform SenderTransform => _senderTransform;
        [SerializeField] protected Transform _senderTransform;

        [SerializeField] private List<Transform> lineTrmList;

        int lineIdx = 0;

        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < 3; ++i)
            {
                lineCustomerList.Add(null);
            }
        }

        public Transform GetLineTrm(Customer customer)
        {
            if (lineIdx >= lineCustomerList.Count)
            {
                return null;
            }

            lineCustomerList[lineIdx] = customer;
            return lineTrmList[lineIdx++];
        }

        public void ExitLine(Customer customer)
        {
            int idx = lineCustomerList.IndexOf(customer);

            if (idx >= 0)
                lineCustomerList[idx] = null;
            lineIdx--;
        }

        public Transform CheckEmptyFront(Customer customer)
        {
            int idx = lineCustomerList.IndexOf(customer);
            if (idx <= 0)
            {
                return null;
            }

            if (lineCustomerList[idx - 1] == null)
            {
                lineCustomerList[idx - 1] = customer;
                lineCustomerList[idx] = null;
                return lineTrmList[idx - 1];
            }
            else
            {
                return null;
            }
        }

        public bool IsCustomerFirst(Customer customer) => lineCustomerList.IndexOf(customer) == 0;
    }
}
