using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace NodeInspector.Demo.Dialogue{
    [Serializable]
    public class Slot{
        public ItemType Item;
        public int  Count;
    }

    public class Inventory : MonoBehaviour {
        public List<Slot> Slots;

        //Could accept negative value
        public void AddItem(ItemType itemType, int count){
            if (count > 0){
                IncreaseItemNumberOn(itemType, count);
            } else {
                DecreaseItemNumberOn(itemType, -count);
            }
        }

        void IncreaseItemNumberOn(ItemType itemType, int count){
            Slot slot = GetSlotForType(itemType);
            if (slot == null){
                slot = new Slot(){
                    Item = itemType,
                    Count = count
                };
                Slots.Add(slot);
            } else {
                slot.Count+= count;
            }
        }

        void DecreaseItemNumberOn(ItemType itemType, int count){
            Slot slot = GetSlotForType(itemType);
            if (slot == null){
               //good we don't have this item, can't remove anything
            } else {
                slot.Count -= count;
                if (slot.Count < 0){
                    Slots.Remove(slot);
                }
            }
        }

        public bool HasItem(ItemType item){
            Slot slot = GetSlotForType(item);
            return slot != null && slot.Count > 0;
        }

        // On later game we can return collection because one item could be stored at several slots
        Slot GetSlotForType(ItemType item){            
            return Slots.SingleOrDefault((arg) => arg.Item == item);
        }
    }    
}
