using System.Collections;
using System.Collections.Generic;
using System;

namespace NanoEcs
{
    [Serializable]
    public class Storage
    {
        public string Name;

        public Type type;
        object[] items;
        bool[] hasItem;

        ComponentEcs _component;


        public Storage(int capacity, Type type)
        {
            items = new object[capacity];
            hasItem = new bool[capacity];
            this.type = type;

            for (int i = 0; i < capacity; i++)
            {
                CreateNewItemAtIndex(i);
            }

#if NANOECS_VERBOSE_DEBUG
        Name = type.ToString();
#endif
        }

        public object GetAtIndex(int id)
        {
            return items[id];
        }

        public object ActivateAtIndex(int id)
        {
            CheckID(id);

            hasItem[id] = true;

#if NANOECS_VERBOSE_DEBUG
        Debug.Log(Name + " added to entity-: " + id);
#endif

            return items[id];
        }

        public void DeactivateAtIndex(int id)
        {

            CheckID(id);

            if (hasItem[id] == false)
            {
                throw new Exception("Unable to remove " + Name + "from entity-" + id + ". Component doesn't exist");
            }
            else
            {
                hasItem[id] = false;
#if NANOECS_VERBOSE_DEBUG
            Debug.Log(Name + " removed from entity-" + id); 
#endif
            }
        }

        public bool HasComponentAtIndex(int id)
        {
            if (id >= hasItem.Length) return false;
            return hasItem[id] == true;
        }

        void CheckID(int id)
        {
            if (id >= items.Length)
            {
                var prevSize = items.Length;
                var newSize = IntExtensions.GetPowerOfTwoSize(id + 1);

                Array.Resize(ref items, newSize);
                Array.Resize(ref hasItem, newSize);

                for (int i = prevSize; i < newSize; i++)
                {
                    CreateNewItemAtIndex(i);
                }
            }
        }

        void CreateNewItemAtIndex(int id)
        {
            items[id] = System.Activator.CreateInstance(type);
        }
    }

}