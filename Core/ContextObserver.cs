#if UNITY_EDITOR && NANOECS_DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NanoEcs
{

    public class ContextObserver<T> where T : Entity, new()
    {
        Context<T> _context;
        Dictionary<string, int> _componentsLookup = new Dictionary<string, int>();
        Dictionary<int, EntityObserver> _entityObserverByID = new Dictionary<int, EntityObserver>();

        GameObject _contextObserverGO;

        public void Initialize(Context<T> context, Dictionary<string, int> componentsLookup)
        {
            var rawName = typeof(T).ToString();
            var baseName = rawName.Substring(0, rawName.Length - "Entity".Length);
            var fineName = string.Format("[{0}Context]", baseName);
            _contextObserverGO = new GameObject(fineName);
            _contextObserverGO.AddComponent<ContextObserverBehaviour>().Initialize(CreateEntity, baseName);
            _contextObserverGO.transform.SetAsFirstSibling();

            _context = context;
            _componentsLookup = componentsLookup;
        }

        public void CreateEntity()
        {
            var e = _context.CreateEntity();
        }

        public void DestroyEntity(Entity entity)
        {
            if (_entityObserverByID.Count == 0) return;
            _context.Destroy(entity.ID);
        }

        public void OnEntityComponentChange(Entity entity)
        {
            if (entity.IsReserved) return;

            var components = entity.ComponentObservers
               .Select(x => x.Component.GetType().ToString().Replace("Component", ""));

            var componentsSequence = components.Count() > 0 ? string.Format(" <{0}>", components.Aggregate((a, b) => a + ", " + b)) : string.Empty;

            var go = _entityObserverByID[entity.ID].gameObject;
            go.name = "Entity_" + entity.ID + componentsSequence;
        }

        public void OnEntityComponentAdd()
        {

        }

        public void OnEntityComponentRemove()
        {

        }

        public void OnEntityCreated(Entity entity)
        {
            var eGO = new GameObject("Entity_" + entity.ID);
            eGO.transform.SetParent(_contextObserverGO.transform);
            var observer = eGO.AddComponent<EntityObserver>();
            observer.Initialize(entity, DestroyEntity, _componentsLookup);

            if (!_entityObserverByID.ContainsKey(entity.ID))
            {
                _entityObserverByID.Add(entity.ID, observer);
            }
            else
            {
                Debug.LogError("entity-" + entity.ID + "is already added");
            }
        }

		public void OnEntityRemoved(Entity entity)
        {
            if (_entityObserverByID.ContainsKey(entity.ID))
            {
                var observer = _entityObserverByID[entity.ID];
                if (observer != null)
                {
                    _entityObserverByID.Remove(entity.ID);
                    Object.Destroy(observer.gameObject);
                }
            }

        }
    }
}

#endif