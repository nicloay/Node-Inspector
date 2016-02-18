using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace NodeInspector.Demo{
    [NodeMenuItem("CustomNode")]
    public class CustomNode : ScriptableObject{
        public int          IntProperty ;
        public List<string> ListProperty;
        public float        FloatProperty;
       
    }    
}
