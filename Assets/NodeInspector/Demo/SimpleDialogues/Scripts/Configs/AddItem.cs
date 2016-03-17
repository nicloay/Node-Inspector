using UnityEngine;
using System.Collections;

namespace NodeInspector.Demo.Dialogue{
    public class AddItem : DialogueNode {
        public Owner    Owner;
        public ItemType ItemType;
        public int      ItemNumber;
        [OneWay]
        public DialogueNode NextStep;

        public override void Execute(GameObject actor, GameObject opponent)
        {
            GameObject actualOwner = Owner == Owner.Actor ? actor : opponent;
            Inventory inventory = actualOwner.GetComponent<Inventory>();
            if (inventory == null){
                Debug.LogErrorFormat("can't find inventory on {0} gameObject ", actualOwner.name, actualOwner);
            } else {
                inventory.AddItem(ItemType, ItemNumber);
            }
            NextStep.Execute(actor, opponent);
        }
    }
}
