using UnityEngine;
using System.Collections;
namespace NodeInspector.Demo.Dialogue{
    public class PlayAnimation : DialogueNode {
        public Owner         Owner;
        public string        ObjectKey;
        public AnimationClip Animation;
        [OneWay]
        public DialogueNode NextStep;

        public override void Execute(GameObject actor, GameObject opponent)
        {
            GameObject actualOwner = Owner == Owner.Actor ? actor : opponent;
            InteractableObjects objects = actualOwner.GetComponent<InteractableObjects>();
            if (objects == null){
                Debug.LogError("Cant find InteractableObjects on the object", actualOwner);
            } else {
                GameObject actualObject = objects.GetObjectBykey(ObjectKey);
                if (actualObject == null){
                    Debug.LogErrorFormat("Can't find object with key {0} on the object {1}", ObjectKey, actualObject.name );
                } else {
                    Animation animation = actualObject.GetComponent<Animation>();
                    animation.clip = Animation;
                    animation.wrapMode = WrapMode.Once;
                    animation.Play();
                }
            }
        }
    }
}
