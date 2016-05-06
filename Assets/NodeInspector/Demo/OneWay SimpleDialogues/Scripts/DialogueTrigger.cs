using UnityEngine;
using System.Collections;

namespace NodeInspector.Demo.Dialogue{    
    public class DialogueTrigger : MonoBehaviour {
        public LayerMask OpponentLayerMask;
        public DialogueSequence Dialogue;

        void OnTriggerEnter(Collider other){            
            if (((1 << other.gameObject.layer) & OpponentLayerMask.value ) != 0){                
                Dialogue.StartItem.Execute(gameObject, other.gameObject);
            }
        }

        void OnTriggerExit(Collider other){            
            if (((1 << other.gameObject.layer) & OpponentLayerMask.value ) != 0){                
                UIController.Instance.CloseDialogue();
            }
        }
    }
}
