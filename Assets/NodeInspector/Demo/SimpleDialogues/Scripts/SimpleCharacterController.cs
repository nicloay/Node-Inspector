using UnityEngine;
using System.Collections;

namespace NodeInspector.Demo.Dialogue{    
    public class SimpleCharacterController : MonoBehaviour {
        public float Force = 0.5f;
        float xInput = 0.0f;
        
        void Update () {
            xInput = Input.GetAxis("Horizontal");
        }
        
        void FixedUpdate(){        
            GetComponent<Rigidbody>().AddForce(new Vector3(xInput*Force, 0 ,0), ForceMode.VelocityChange);        
        }
    }
}
