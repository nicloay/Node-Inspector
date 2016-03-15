using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace NodeInspector.Editor{	
	public class ConnectionsCollection {
		public List<Connection> allConnections;

        public ConnectionsCollection(List<Node> allNodes, NodeInspector parentWindow){
			allConnections = new List<Connection> ();        

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


            Stack<Joint> UsedByLineJoints = new Stack<Joint>(); // all joints used by line would be store here
            bool handleJointDragging = true;
            Connection lastDraggedConnection = null;
			//connect them to fields		
			foreach (Node node in allNodes) {
                foreach (Joint joint in node.Joints) {
					if (joint.JointType == JointType.OneToOne_Incognito_OUT || joint.JointType == JointType.ManyToOne_Incognito_OUT) {
                        if (joint.ObjectRefferenceValue != null){
                            Connection connection;
                            if (incognitoInConnections.ContainsKey(joint.ObjectRefferenceValue)){
                                connection = incognitoInConnections[joint.ObjectRefferenceValue];
                                if (connection.OutputJoint != null){                                    
                                    //need to clone this connection because one already connected to something
                                    connection = GetNewConnectionGUI(connection.InputJoint);
                                }
                                connection.OutputJoint = joint;
                                allConnections.Add(connection);
                                UsedByLineJoints.Push(connection.OutputJoint);
                                UsedByLineJoints.Push(connection.InputJoint);

                                if (handleJointDragging){                                    
                                    if (connection.InputJoint == parentWindow.StartDraggJoint || connection.OutputJoint == parentWindow.StartDraggJoint){                                                                               

                                        if (connection.Focused || connection.ConnectionType != ConnectionRenderType.OutputToInput){                                            
                                            handleJointDragging = false;
                                            //it's possible that several line use this knob and we will drag last one or selected if some of lines in focus
                                        }

                                        connection.ConnectionType = connection.InputJoint == parentWindow.StartDraggJoint 
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

            if (lastDraggedConnection == null){                
                //lets check maybe we clicked some joints and want to connect it to something
                foreach (Node node in allNodes) {
                    foreach (Joint joint in node.Joints) {
                        if (!UsedByLineJoints.Contains(joint) && joint == parentWindow.StartDraggJoint){
                            //here is clicked node so lets create new connection with anonimouse target or source
                            Connection connection = Connection.GetInstance();
                            switch (joint.JointType){
                                case JointType.Incognito_In:
                                    connection.InputJoint = joint;
                                    connection.ConnectionType = ConnectionRenderType.MouseToInputNode;
                                    break;
                                case JointType.OneToOne_Incognito_OUT:
                                    connection.OutputJoint = joint;
                                    connection.ConnectionType = ConnectionRenderType.OutputNodeToMouse;
                                    break;
                                default:
                                    Debug.LogWarning("unsuported joint type"+ joint.JointType) ;
                                    break;
                            }
                            allConnections.Add(connection);
                            lastDraggedConnection = connection;
                        } 
                    }
                }
            }


            if (lastDraggedConnection != null){                
                switch (lastDraggedConnection.ConnectionType){
                    case ConnectionRenderType.MouseToInputNode:                        
                        parentWindow.JointHighlight.JointType = JointType.ManyToOne_Incognito_OUT;
                        break;
                    case ConnectionRenderType.OutputNodeToMouse:
                        parentWindow.JointHighlight.JointType = JointType.Incognito_In;
                        break;
                    default:
                        Debug.LogError("FIXME: must not be here");
                        break;
                }
                parentWindow.Repaint();
            } else {
                parentWindow.JointHighlight.JointType = JointType.Nan;
            }
        }

        Connection GetNewConnectionGUI( Joint jointData){
            Connection connectionData = Connection.GetInstance();
            connectionData.InputJoint = jointData;
            return connectionData;
        }

	}
}