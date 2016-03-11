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

            bool handleJointDragging = true;
            Connection lastDraggedConnection = null;
			//connect them to fields		
			foreach (Node node in allNodes) {
				foreach (Joint jointData in node.Joints) {
					if (jointData.JointType == JointType.OneToOne_Incognito_OUT || jointData.JointType == JointType.ManyToOne_Incognito_OUT) {
                        if (jointData.ObjectRefferenceValue != null){
                            Connection connection;
                            if (incognitoInConnections.ContainsKey(jointData.ObjectRefferenceValue)){
                                connection = incognitoInConnections[jointData.ObjectRefferenceValue];
                                if (connection.OutputJoint != null){                                    
                                    //need to clone this connection because one already connected to something
                                    connection = GetNewConnectionGUI(connection.InputJoint);
                                }
                                connection.OutputJoint = jointData;
                                allConnections.Add(connection);

                                if (handleJointDragging){                                    
                                    if (connection.InputJoint.MouseDrag || connection.OutputJoint.MouseDrag){                                                                               

                                        if (connection.Focused || connection.ConnectionType != ConnectionRenderType.OutputToInput){                                            
                                            handleJointDragging = false;
                                            //it's possible that several line use this knob and we will drag last one or selected if some of lines in focus
                                        }

                                        connection.ConnectionType = connection.InputJoint.MouseDrag 
                                            ? ConnectionRenderType.OutputNodeToMouse : ConnectionRenderType.MouseToInputNode;
                                        if (lastDraggedConnection != null){
                                            lastDraggedConnection.ConnectionType = ConnectionRenderType.OutputToInput;
                                        }
                                        lastDraggedConnection = connection;

                                    } else {
                                        connection.ConnectionType = ConnectionRenderType.OutputToInput;
                                    }
                                }
							} 
						}						
					}
				}
			}

            if (lastDraggedConnection != null){
                parentWindow.Repaint();
            }
        }

        Connection GetNewConnectionGUI( Joint jointData){
            Connection connectionData = Connection.GetInstance();
            connectionData.InputJoint = jointData;
            return connectionData;
        }

	}
}