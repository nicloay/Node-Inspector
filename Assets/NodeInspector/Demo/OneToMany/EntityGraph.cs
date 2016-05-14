using UnityEngine;
using System.Collections.Generic;

namespace NodeInspector.Demo.OneToMany{ 
    [CreateAssetMenu]
    public class EntityGraph : ScriptableObject {
        [Graph]
        public List<SimpleItem> Entities;
    }
}