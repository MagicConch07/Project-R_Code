using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GM.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        protected Dictionary<Type, IEntityComponent> _components;

        protected virtual void Awake()
        {
            _components = new Dictionary<Type, IEntityComponent>();
            AddComponentToDictionary(gameObject, _components);
            ComponentInitialize();
            AfterInitialize();
        }

        #region Entity Component Structure

        public static void AddComponentToDictionary<T>(GameObject targetObject, Dictionary<Type, T> targetDictionary, bool includeInactive = true)
        {
            targetObject.GetComponentsInChildren<T>(includeInactive)
                .ToList()
                .ForEach(component => targetDictionary.Add(component.GetType(), component));
        }

        protected void ComponentInitialize()
        {
            _components.Values.ToList().ForEach(component => component.Initialize(this));
        }

        protected virtual void AfterInitialize()
        {
            _components.Values.OfType<IAfterInitable>()
                .ToList().ForEach(afterInitCompo => afterInitCompo.AfterInit());
        }

        public T GetCompo<T>(bool isDerived = false) where T : IEntityComponent
        {
            if (_components.TryGetValue(typeof(T), out IEntityComponent component))
            {
                return (T)component;
            }

            if (isDerived == false) return default;

            Type findType = _components.Keys.FirstOrDefault(type => type.IsSubclassOf(typeof(T)));
            if (findType != null)
                return (T)_components[findType];

            return default;
        }

        #endregion
    }
}
