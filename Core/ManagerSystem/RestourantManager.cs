using GM.Entities;
using GM.InteractableEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GM.Managers
{
    public class RestourantManager : IManagerable
    {
        public bool IsSeatFull => isSeatFull;
        private bool isSeatFull = false;

        private uint tableId = 0;

        private Dictionary<Enums.InteractableEntityType, List<InteractableEntity>> _interactableEntityDictionary;

        public void Initialized()
        {
            _interactableEntityDictionary = new Dictionary<Enums.InteractableEntityType, List<InteractableEntity>>();

            // TODO : 저장 로직 추가시 변경
            foreach (var interactableTable in GameObject.FindObjectsByType<InteractableEntity>(FindObjectsSortMode.None))
            {
                if (_interactableEntityDictionary.ContainsKey(interactableTable.Type) == false)
                {
                    _interactableEntityDictionary[interactableTable.Type] = new List<InteractableEntity>();
                }

                if (interactableTable is Table table)
                {
                    table.SetID(tableId++);
                }
                _interactableEntityDictionary[interactableTable.Type].Add(interactableTable);
            }
        }

        public void Clear()
        {
            _interactableEntityDictionary.Clear();
        }

        public List<InteractableEntity> GetAllInteractableEntitiesList()
        {
            List<InteractableEntity> allInteractableEntities = new List<InteractableEntity>();
            foreach (var interactableEntityList in _interactableEntityDictionary.Values)
            {
                allInteractableEntities.AddRange(interactableEntityList);
            }
            return allInteractableEntities;
        }

        public List<InteractableEntity> GetInteractableEntitiesList(Enums.InteractableEntityType type)
        {
            if (_interactableEntityDictionary.TryGetValue(type, out List<InteractableEntity> tables) && tables.Count > 0)
            {
                return tables;
            }

            return null;
        }

        public void AddMapInteractable(InteractableEntity entity)
        {
            if (_interactableEntityDictionary.ContainsKey(entity.Type) == false)
            {
                _interactableEntityDictionary[entity.Type] = new List<InteractableEntity>();
            }
            _interactableEntityDictionary[entity.Type].Add(entity);

            if (entity is Table table)
            {
                table.SetID(tableId++);
            }
        }

        public void RemoveMapInteractable(InteractableEntity entity)
        {
            if (_interactableEntityDictionary.ContainsKey(entity.Type))
            {
                _interactableEntityDictionary[entity.Type].Remove(entity);
            }
        }

        public Table GetTable(uint id = uint.MaxValue)
        {
            List<InteractableEntity> tableList;
            _interactableEntityDictionary.TryGetValue(Enums.InteractableEntityType.Table, out tableList);

            if (id < uint.MaxValue)
            {
                foreach (Table table in tableList)
                {
                    if (table.ID == id)
                    {
                        return table;
                    }
                }
            }

            // any Table
            List<Table> nullValueList = new();
            foreach (Table table in tableList)
            {
                if (table.HasEmptyChair())
                {
                    nullValueList.Add(table);
                }
            }

            if (nullValueList.Count == 0)
            {
                isSeatFull = true;
                return default;
            }
            else
            {
                isSeatFull = false;
            }

            int randIdx = Random.Range(0, nullValueList.Count);
            return nullValueList[randIdx];
        }

        /// <summary>
        /// Get the nearest interactable entity
        /// </summary>
        /// <param name="type">InteractableEntity type</param>
        /// <param name="tableEntity">InteractableEntity</param>
        /// <param name="owner"></param>
        /// <returns>The nearest interactable entity</returns>
        public bool GetInteractableEntity(Enums.InteractableEntityType type, out InteractableEntity tableEntity, Entity owner)
        {
            tableEntity = null;

            var tables = GetInteractableEntitiesTryGetValue(type, table => table.InUse == false);
            if (tables != null)
            {
                tableEntity = CalculateMinimumDistanceEntity(tables, owner);
            }

            return tableEntity != null;
        }

        public bool GetStaticFirstInteractableEntity(Enums.InteractableEntityType type, out InteractableEntity tableEntity)
        {
            tableEntity = null;

            var tables = GetInteractableEntitiesTryGetValue(type);

            tableEntity = tables?.First();
            return tableEntity != null;
        }

        public bool GetStaticFirstInteractableEntites(Enums.InteractableEntityType type, out List<InteractableEntity> listTableEntites)
        {
            listTableEntites = null;

            listTableEntites = GetInteractableEntitiesTryGetValue(type);

            return listTableEntites != null;
        }

        public bool GetGenericEntityList<T>(Enums.InteractableEntityType type, out List<T> listTableEntites) where T : InteractableEntity
        {
            var entityList = GetInteractableEntitiesTryGetValue(type);

            listTableEntites = entityList?.OfType<T>().ToList() ?? null;

            return listTableEntites.Any();
        }

        private List<InteractableEntity> GetInteractableEntitiesTryGetValue(Enums.InteractableEntityType type, Func<InteractableEntity, bool> predicate = null)
        {
            if (_interactableEntityDictionary.TryGetValue(type, out List<InteractableEntity> tables) && tables.Count > 0)
            {
                if (predicate == null)
                {
                    return tables;
                }
                return tables.Where(predicate).ToList();
            }

            return null;
        }

        private InteractableEntity CalculateMinimumDistanceEntity(List<InteractableEntity> tables, Entity owner)
        {
            float minimumDistance = float.MaxValue;
            InteractableEntity minimumEntity = null;

            foreach (var table in tables)
            {
                float distance = Vector3.Distance(owner.transform.position, table.transform.position);
                if (distance < minimumDistance)
                {
                    minimumDistance = distance;
                    minimumEntity = table;
                }
            }

            return minimumEntity;
        }
    }
}