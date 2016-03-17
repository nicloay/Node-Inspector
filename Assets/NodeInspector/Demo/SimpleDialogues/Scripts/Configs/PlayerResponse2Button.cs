using UnityEngine;
using System.Collections;

namespace NodeInspector.Demo.Dialogue{       
    public class PlayerResponse2Button : DialogueNode {
        public string Caption1;
        [OneWay]
        public DialogueNode NextStepOn1;

        public string Caption2;
        [OneWay]
        public DialogueNode NextStepOn2;

        public override void Execute(GameObject actor, GameObject opponent)
        {
            throw new System.NotImplementedException();
        }            
    }

}