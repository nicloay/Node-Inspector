using UnityEngine;
using System.Collections;

namespace NodeInspector.Demo.Dialogue{
    public enum Owner{
        Actor,
        Opponent
    }

    public class ShowPhrase : DialogueNode {
        public Owner Owner;
        [Multiline]
        public string Text;
        [OneWay]
        public DialogueNode NextStep;

        public override void Execute(GameObject actor, GameObject opponent)
        {
            throw new System.NotImplementedException();
        }
    }

}