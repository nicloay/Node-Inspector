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
            UIController.Instance.ShowTwoButtons(
                Caption1, ()=>ExecuteNodeIfNotNull(NextStepOn1, actor, opponent),
                Caption2, ()=>ExecuteNodeIfNotNull(NextStepOn2, actor, opponent)
            );
        }       

        void ExecuteNodeIfNotNull(DialogueNode node, GameObject actor, GameObject opponent){
            if (node!=null){
                node.Execute(actor, opponent);
            }
        }
    }

}