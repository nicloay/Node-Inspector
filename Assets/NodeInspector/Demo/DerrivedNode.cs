using UnityEngine;
using System.Collections;


namespace NodeInspector.Demo{
    [NodeMenuItem("SubNode/SubNode/DerrivedNode")]
    public class DerrivedNode : CustomNode {
        public float DerivedFloatProperty;
    }
}