using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace NanoEcs
{
    public interface IContext
    {
        void HandleDalayedOperations();
    }

    public enum DelayedOperationType : byte
    {
        Add,
        Remove,
        ValueChange
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
    public struct DelayedOperation
    {
        public DelayedOperationType Type;
        public int EntityID;
        public int ComponentFieldID;
        public int ComponentIndex;

        public DelayedOperation(DelayedOperationType operationType, int entityID)
        {
            Type = operationType;
            EntityID = entityID;
            ComponentFieldID = 0;
            ComponentIndex = 0;
        }

        public DelayedOperation(DelayedOperationType operationType, int entityID, int componentFieldID, int componentIndex)
        {
            Type = operationType;
            EntityID = entityID;
            ComponentFieldID = componentFieldID;
            ComponentIndex = componentIndex;
        }
    }

    [Serializable]
    public partial class Context<T> : IDisposable, IContext where T : Entity, new()
    {
        T[] _entities;
        public int entitiesCount;
        int[] _reservedEntities;
        int _reservedEntitiesCount;
        Storage[] _storages;
        Groups<T> _groups = new Groups<T>();

        DelayedOperation[] delayedOperations = new DelayedOperation[1024];
        int delayedOperationsCount;

        public Group<T> CommonGroup = new Group<T>();

#if UNITY_EDITOR && NANOECS_DEBUG
        ContextObserver<T> _observer;
        Dictionary<string, int> _componentsLookup = new Dictionary<string, int>();
#endif

        public Context(Type[] componentTypes)
        {
            _entities = new T[64];

            _reservedEntities = new int[8];

            _storages = new Storage[componentTypes.Length];
            for (int i = 0; i < _storages.Length; i++)
            {
                _storages[i] = new Storage(64, componentTypes[i]);
            }

#if UNITY_EDITOR && NANOECS_DEBUG
            for (int i = 0; i < _storages.Length; i++)
            {
                _componentsLookup.Add(componentTypes[i].ToString(), i);
            }

            _observer = new ContextObserver<T>();
            _observer.Initialize(this, _componentsLookup);
#endif
        }

        public void Dispose()
        {
            foreach (var entity in _entities)
            {
                Destroy(entity);
            }
        }

        protected Group<T> CreateGroupInternal(Group<T> group)
        {
            _groups.Values.Add(group);
            if (entitiesCount > 0)
            {
                //HandleDelayedGroup(group);
            }
            return group;
        }

        void HandleDelayedGroup(Group<T> group)
        {
            for (int i = 0; i < entitiesCount; i++)
            {
                if (_entities[i].IsReserved) continue;

                TryAddEntityToGroup(_entities[i], _groups.Values.Count - 1, group);
            }
        }

        public T CreateEntity()
        {
            int id;
            if (_reservedEntitiesCount > 0)
            {
                _reservedEntitiesCount--;
                id = _reservedEntities[_reservedEntitiesCount];
            }
            else
            {
                id = entitiesCount;
            }
            entitiesCount++;

            var entity = new T();
            entity.InternalInitialize(id,
                _storages,
                OnComponentValueChange,
                OnComponentAdd,
                OnComponentRemove);

            if (id == _entities.Length)
            {
                Array.Resize(ref _entities, entitiesCount << 1);
            }

            _entities[id] = entity;


#if UNITY_EDITOR && NANOECS_DEBUG
            entity.componentsLookup = _componentsLookup;
            _observer.OnEntityCreated(entity);
#endif
            entity.IsReserved = false;

            CommonGroup.Add(entity);

            return entity;
        }

        public void Destroy(int id)
        {
            if (entitiesCount == 0) return;
            Destroy(_entities[id]);
        }

        public void Destroy(T entity)
        {
            entitiesCount--;

            if (entity.OnDestroy != null)
            {
                entity.OnDestroy();
                entity.OnDestroy = null;
            }

            if (entity.IsReserved) throw new System.Exception("Can not destroy reserved entity");

            entity.IsReserved = true;

            CallGroupsOnDestroy(entity.ID);

            CommonGroup.Remove(entity);

            entity.RemoveAllComponents();

            _reservedEntities[_reservedEntitiesCount++] = entity.ID;

            if (_reservedEntitiesCount == _reservedEntities.Length)
            {
                Array.Resize(ref _reservedEntities, _reservedEntitiesCount << 1);
            }

            entity.InternalReset();

#if UNITY_EDITOR && NANOECS_DEBUG
            _observer.OnEntityRemoved(entity);
#endif
        }

        public T GetEntity(int id)
        {
            if (id < 0) return null;
            var entity = _entities[id];
            if (entity.IsReserved) return null;
            return entity;
        }

        /// <summary>
        /// Returns all enities in the context. Rather slow, don't use it every tick.
        /// </summary>
        /// <returns></returns>
        public T[] GetAllEntities()
        {
            if (entitiesCount == 0) return new T[0];
            return _entities.Take(entitiesCount).Where(e => !e.IsReserved).ToArray();
        }

        public void OnComponentAdd(int id)
        {
            AddDelayedOperation(DelayedOperationType.Add, id);
        }

        public void OnComponentRemove(int id)
        {
            AddDelayedOperation(DelayedOperationType.Remove, id);
        }

        void RefreshGroups(int id)
        {
            var entity = _entities[id];

            for (int i = 0; i < _groups.Values.Count; i++)
            {
                var group = _groups.Values[i];
                TryAddEntityToGroup(entity, i, group);
            }


            var groupCountCached = entity.GroupsCount;
            for (int i = groupCountCached - 1; i >= 0; i--)
            {
                var groupID = entity.GroupsIDs[i];
                var group = _groups.Values[groupID];
                TryRemoveEntityFromGroup(entity, i, group);
            }

#if UNITY_EDITOR && NANOECS_DEBUG
            _observer.OnEntityComponentChange(entity);
#endif
        }

        private static void TryRemoveEntityFromGroup(T entity, int i, Group<T> group)
        {
            if (!group.IsMatching(entity))
            {
                var res = group.Remove(entity);
                if (res)
                {
                    entity.GroupsCount--;
                    Array.Copy(entity.GroupsIDs, i + 1, entity.GroupsIDs, i, entity.GroupsCount - i);
                }
            }
        }

        static void TryAddEntityToGroup(T entity, int groupIndex, Group<T> group)
        {
            if (group.IsMatching(entity))
            {
                if (group.Add(entity))
                {
                    entity.GroupsIDs[entity.GroupsCount++] = groupIndex;

                    if (entity.GroupsIDs.Length == entity.GroupsCount)
                    {
                        Array.Resize(ref entity.GroupsIDs, entity.GroupsCount << 1);
                    }
                }
            }
        }



        public void OnComponentValueChange(int entityId, int componentFieldId, int componentIndex)
        {
            AddDelayedOperation(DelayedOperationType.ValueChange, entityId, componentFieldId, componentIndex);
        }

        private void TryAddToCollectors(int entityId, int componentFieldId, int componentIndex)
        {
            var entity = _entities[entityId];
            for (int i = entity.GroupsCount - 1; i >= 0; i--)
            {
                var groupID = entity.GroupsIDs[i];
                var group = _groups.Values[groupID];

                if (group.HasCollectorNode[componentIndex])
                {
                    var node = group.CollectorNodes[componentIndex];
                    if (node.hasCollector[componentFieldId])
                    {
                        node.collectors[componentFieldId].Add(entity);
                    }
                }
            }
        }

        void CallGroupsOnDestroy(int entityId)
        {
            var entity = _entities[entityId];
            for (int i = entity.GroupsCount - 1; i >= 0; i--)
            {
                var groupID = entity.GroupsIDs[i];
                var group = _groups.Values[groupID];
                group.InternalTryAddToOnDestroy(entity);
            }
        }

        public void HandleDalayedOperations()
        {
            if (delayedOperationsCount == 0) return;

            for (int i = 0; i < delayedOperationsCount; i++)
            {
                var operation = delayedOperations[i];

                if (operation.Type == DelayedOperationType.Add)
                {
                    RefreshGroups(operation.EntityID);
                }
                else if (operation.Type == DelayedOperationType.Remove)
                {
                    RefreshGroups(operation.EntityID);
                }
                else if (operation.Type == DelayedOperationType.ValueChange)
                {
                    TryAddToCollectors(operation.EntityID, operation.ComponentFieldID, operation.ComponentIndex);
                }
            }
            delayedOperationsCount = 0;
        }

        void AddDelayedOperation(DelayedOperationType type, int entityID, int componentFieldId = 0, int componentIndex = 0)
        {
            if (delayedOperationsCount == delayedOperations.Length)
            {
                Array.Resize(ref delayedOperations, delayedOperationsCount << 1);
            }
            delayedOperations[delayedOperationsCount++] = new DelayedOperation(type, entityID, componentFieldId, componentIndex);
        }
    }
}