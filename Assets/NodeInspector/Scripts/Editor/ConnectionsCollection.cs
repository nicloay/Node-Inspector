using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace NodeInspector.Editor{	
	public class ConnectionsCollection {
		public List<Connection> allConnections;
        NodeInspector parentWindow;

        public ConnectionsCollection(List<Node> allNodes, NodeInspector parentWindow){
			allConnections = new List<Connection> ();
            this.parentWindow = parentWindow;


			Dictionary<Object, Connection> incognitoInConnections = new Dictionary<Object, Connection> ();

            			
			foreach (Node node in allNodes) {
				foreach (Joint jointData in node.Joints) {
					if (jointData.JointType == JointType.Incognito_In) {
                        
                        if (incognitoInConnections.ContainsKey(jointData.ObjectRefferenceValue))
                        {
                            Debug.LogError("FIXME: we already have this value " + jointData.ObjectRefferenceValue);
                        } else {                            
                            Connection connectionData = GetNewConnectionGUI(jointData);
                            incognitoInConnections.Add(jointData.ObjectRefferenceValue, connectionData);
                        }
					} 
				}
			}

            bool HandleMouseDragNDrop = true;
			//connect them to fields		
			foreach (Node node in allNodes) {
				foreach (Joint jointData in node.Joints) {
					if (jointData.JointType == JointType.OneToOne_Incognito_OUT || jointData.JointType == JointType.ManyToOne_Incognito_OUT) {
                        if (jointData.ObjectRefferenceValue != null){
							Connection connectionData;
                            if (incognitoInConnections.ContainsKey(jointData.ObjectRefferenceValue)){
                                connectionData = incognitoInConnections[jointData.ObjectRefferenceValue];
                                if (connectionData.OutputJoint != null){                                    
                                    //need to clone this connection because one already connected to something
                                    connectionData = GetNewConnectionGUI(connectionData.InputJoint);
                                }
                                connectionData.OutputJoint = jointData;
                                allConnections.Add(connectionData);
                                if (HandleMouseDragNDrop){
                                    if (connectionData.InputJoint.MouseDrag){
                                        connectionData.ConnectionType = ConnectionRenderType.OutputNodeToMouse;
                                        HandleMouseDragNDrop = false;
                                        parentWindow.Repaint();
                                    } else if (connectionData.OutputJoint.MouseDrag){
                                        connectionData.ConnectionType = ConnectionRenderType.MouseToInputNode;
                                        HandleMouseDragNDrop = false;
                                        parentWindow.Repaint();
                                    } else {
                                        connectionData.ConnectionType = ConnectionRenderType.OutputToInput;
                                    }
                                }
							} 
						}						
					}
				}
			}
        }

        Connection GetNewConnectionGUI( Joint jointData){
            Connection connectionData = Connection.GetInstance(GUIUtility.GetControlID(FocusType.Passive));
            connectionData.InputJoint = jointData;
            return connectionData;
        }

	}
}