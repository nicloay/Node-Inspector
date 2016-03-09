using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace NodeInspector.Editor{	
	public class ConnectionsCollection {
		public List<ConnectionGUI> allConnections;

        public ConnectionsCollection(List<NodeGUI> allNodes){
			allConnections = new List<ConnectionGUI> ();



			Dictionary<Object, ConnectionGUI> incognitoInConnections = new Dictionary<Object, ConnectionGUI> ();


			//collect all incognito inputconnections
			foreach (NodeGUI node in allNodes) {
				foreach (JointData jointData in node.Joints) {
					if (jointData.JointType == JointType.Incognito_In) {
                        
                        if (incognitoInConnections.ContainsKey(jointData.ObjectRefferenceValue))
                        {
                            Debug.Log("we already have this value " + jointData.ObjectRefferenceValue);
                        }
                        ConnectionGUI connectionData = GetNewConnectionGUI(jointData);
                        incognitoInConnections.Add(jointData.ObjectRefferenceValue, connectionData);
					} 
				}
			}




			//connect them to fields		
			foreach (NodeGUI node in allNodes) {
				foreach (JointData jointData in node.Joints) {
					if (jointData.JointType == JointType.OneToOne_Incognito_OUT || jointData.JointType == JointType.ManyToOne_Incognito_OUT) {
                        if (jointData.ObjectRefferenceValue != null){
							ConnectionGUI connectionData;
                            if (incognitoInConnections.ContainsKey(jointData.ObjectRefferenceValue)){
                                connectionData = incognitoInConnections[jointData.ObjectRefferenceValue];
                                if (connectionData.OutputJoint != null){                                    
                                    //need to clone this connection because one already connected to something
                                    connectionData = GetNewConnectionGUI(connectionData.InputJoint);
                                }
                                connectionData.OutputJoint = jointData;
                                allConnections.Add(connectionData);
							} 
						}						
					}
				}
			}
        }

        ConnectionGUI GetNewConnectionGUI( JointData jointData){
            ConnectionGUI connectionData = ConnectionGUI.GetInstance(GUIUtility.GetControlID(FocusType.Passive));
            connectionData.InputJoint = jointData;
            return connectionData;
        }

	}
}