using UnityEngine;
using System.Collections;

namespace NodeInspector.Demo.Dialogue{    
    public class HasItem : DialogueNode {
        public Owner Owner;
        public ItemType Item;

        [OneWay]
        public DialogueNode Yes;
        [OneWay]
        public DialogueNode No;


        public override void Execute(GameObject actor, GameObject opponent)
        {
            GameObject actualOwner = Owner == Owner.Actor ? actor : opponent;
            Inventory inventory = actualOwner.GetComponent<Inventory>();
            if (inventory == null){
                Debug.LogErrorFormat("can't find inventory on {0} gameObject ", actualOwner.name, actualOwner);
            } else {
                if (inventory.HasItem(Item)){
                    if (Yes != null){                        
                        Yes.Execute(actor, opponent);
                    }
                } else {
                    if (No != null){                        
                        No.Execute(actor, opponent);
                    }
                }
            }
        }
    }
}
