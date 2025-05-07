using System;

namespace GM.Data
{
    public enum OrderType
    {
        Null,
        Order,
        Count,
        Serving,
        Cook
    }

    public struct OrderData
    {
        public uint orderTableID;
        public Customer orderCustomer;
        public OrderType type;
        public RecipeSO recipe;
        public bool isCustomerOut;

        public static bool operator ==(OrderData a, OrderData b)
        {
            return a.orderTableID == b.orderTableID &&
                    a.orderCustomer == b.orderCustomer &&
                    a.type == b.type &&
                    a.recipe == b.recipe &&
                    a.isCustomerOut == b.isCustomerOut;
        }

        public static bool operator !=(OrderData a, OrderData b) => !(a == b);

        public override bool Equals(object obj)
        {
            if (obj is OrderData data)
            {
                return this == data;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(orderTableID, orderCustomer, type, recipe, isCustomerOut);
        }
    }
}