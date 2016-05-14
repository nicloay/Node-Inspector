using System;
using UnityEngine;
using System.Collections.Generic;


namespace NodeInspector.Demo.OneToMany{    
    
    public class ComplexItem : SimpleItem, ISerializationCallbackReceiver {
        public List<ItemPack> Items;

        #region ISerializationCallbackReceiver implementation
        public void OnBeforeSerialize()
        {
            if (Items != null) {                
                Items.ForEach((obj) => {
                    if (obj != null) { 
                        obj.Parent = this;
                    }   
                });
            }
        }

        public void OnAfterDeserialize()
        {            
        }
        #endregion
        
    }
}