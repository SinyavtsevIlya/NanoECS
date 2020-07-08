using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NanoEcs
{
    [System.Serializable]
    public class Groups<T> where T : Entity
    {
        public List<Group<T>> Values = new List<Group<T>>();
    }
}
