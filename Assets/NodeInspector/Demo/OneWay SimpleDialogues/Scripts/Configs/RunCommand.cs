using UnityEngine;
using System.Collections;
using NodeInspector.Demo.Dialogue;

namespace NodeInspector.Demo.Dialogue {
    public class RunCommand : DialogueNode {
        public Owner        Owner;
        public string       CommandKey;
        [OneWay]
        public DialogueNode NextStep;

        public override void Execute(GameObject actor, GameObject opponent)
        {
            GameObject actualOwner = Owner == Owner.Actor ? actor : opponent;
            RunCommandController commands = actualOwner.GetComponent<RunCommandController>();
            if (commands == null){
                Debug.LogError("Cant find InteractableObjects on the object", actualOwner);
            } else {
                commands.RunCommand(CommandKey);
                if (NextStep != null){
                    NextStep.Execute(actor, opponent);
                }
            }
        }
    }
    
}
