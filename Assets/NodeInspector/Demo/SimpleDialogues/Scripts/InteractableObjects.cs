using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace NodeInspector.Demo.Dialogue{
    [System.Serializable]
    public class InteractableObject{
        public string Key;
        public GameObject GameObject;
    }

    public class InteractableObjects : MonoBehaviour {
        public List<InteractableObject> Objects;

        public GameObject GetObjectBykey(string key){
            return Objects.First((arg) => arg.Key.ToLower() == key.ToLower()).GameObject;
        }
    }    
}
