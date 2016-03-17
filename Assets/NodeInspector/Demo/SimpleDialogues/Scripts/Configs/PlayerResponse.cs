using UnityEngine;
using System.Collections;

namespace NodeInspector.Demo.Dialogue{
    public class PlayerResponse : DialogueNode {
        public string Caption;
        [OneWay]
        public DialogueNode NextStep;

        public override void Execute(GameObject actor, GameObject opponent)
        {
            throw new System.NotImplementedException();
        }            
    }          
}