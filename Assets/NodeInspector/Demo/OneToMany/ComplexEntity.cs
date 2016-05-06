using System;
using UnityEngine;
using System.Collections.Generic;


namespace NodeInspector.Demo.OneToMany{    
    [Serializable]
    public class Part{
        [OneToMany(DirectionType.Input, "entity")]
        public SimpleEntity Source ;
        public int Quantity;
        public ComplexEntity Parent;
    }

    public class ComplexEntity : SimpleEntity, ISerializationCallbackReceiver {
        public List<Part> Parts;

        #region ISerializationCallbackReceiver implementation
        public void OnBeforeSerialize()
        {
            Parts.ForEach((obj) => {
                if (obj != null) { 
                    obj.Parent = this;
                }   
            });
        }

        public void OnAfterDeserialize()
        {            
        }
        #endregion
        
    }
}