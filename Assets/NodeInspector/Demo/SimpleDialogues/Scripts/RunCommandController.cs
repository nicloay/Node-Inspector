using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace NodeInspector.Demo.Dialogue{    
    [System.Serializable]
    public class CommandWithKey{
        public string Key;
        public UnityEvent UnityEvent;
    }
    public class RunCommandController : MonoBehaviour {
        public List<CommandWithKey> Commands;

        public void RunCommand(string key){
            CommandWithKey  commandWithKey = Commands.SingleOrDefault((arg) => arg.Key.ToLower() == key.ToLower());
            if (commandWithKey == null){
                Debug.LogError("can't find command with key " + key, gameObject);
                return;
            }
            commandWithKey.UnityEvent.Invoke();
        }

    }
    
}