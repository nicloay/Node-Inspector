using UnityEngine;
using System.Collections;
namespace NodeInspector.Demo.Dialogue{
    public class PlayAnimation : DialogueNode {
        public AnimationClip Animation;
        [OneWay]
        public DialogueNode NextStep;

        public override void Execute(GameObject actor, GameObject opponent)
        {
            throw new System.NotImplementedException();
        }
    }
}
