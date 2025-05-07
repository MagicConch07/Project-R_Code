using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MKDir;
using UnityEngine;

namespace GM.Managers
{
    [DefaultExecutionOrder(-1)]
    public class ManagerHub : MonoSingleton<ManagerHub>
    {
        public PoolManagerSO Pool;

        private Dictionary<Type, IManagerable> _managers;
        private List<IManagerUpdateable> _updateManagers;

        protected override void Initialized()
        {
            base.Initialized();

            _managers = new Dictionary<Type, IManagerable>();
            _updateManagers = new List<IManagerUpdateable>();


            Pool.InitializePool(transform);

            GetComponents<IManagerable>().ToList().ForEach(manager => _managers.Add(manager.GetType(), manager));
            FindAndAddManagerToDictionary();
            ManagerInitialize();
            AddUpdateManager();
        }

        private void Update()
        {
            UpdateManager();
        }

        private void OnDestroy()
        {
            ManagerAllClear();
        }

        public T GetManager<T>() where T : IManagerable
        {
            if (_managers.TryGetValue(typeof(T), out IManagerable manager))
            {
                return (T)manager;
            }

            Debug.LogError("Manger Not Found");
            return default;
        }

        /// <summary>
        /// Find all classes that implement IManagerable and add them to the dictionary
        /// </summary>
        private void FindAndAddManagerToDictionary()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => typeof(IManagerable).IsAssignableFrom(type) && type.IsClass);

            foreach (var type in types)
            {
                try
                {
                    IManagerable manager;
                    if (typeof(MonoBehaviour).IsAssignableFrom(type))
                    {
                        manager = GetComponent(type) as IManagerable;
                    }
                    else
                    {
                        manager = Activator.CreateInstance(type) as IManagerable;
                    }

                    if (manager != null)
                    {
                        _managers.TryAdd(type, manager);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"{ex} createInstance Error, Message : {ex.Message}");
                }
            }
        }

        private void ManagerInitialize()
        {
            foreach (var manager in _managers.Values)
            {
                manager.Initialized();
            }
        }

        private void AddUpdateManager()
        {
            foreach (var manager in _managers.Values)
            {
                if (manager is IManagerUpdateable updateable)
                {
                    _updateManagers.Add(updateable);
                }
            }
        }

        private void UpdateManager()
        {
            if (_updateManagers.Count < 0)
            {
                return;
            }

            foreach (var manager in _updateManagers)
            {
                manager.Update();
            }
        }

        private void ManagerAllClear()
        {
            foreach (var manager in _managers.Values)
            {
                if (manager == null) continue;
                manager.Clear();
            }
        }
    }
}