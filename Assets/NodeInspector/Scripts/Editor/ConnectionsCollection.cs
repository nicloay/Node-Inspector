using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace NodeInspector.Editor{	
	public class ConnectionsCollection {
        public Connection LastDraggedConnection {get; private set;}
        public List<Connection> allConnections {get; set;}

        public ConnectionsCollection(List<Node> allNodes, NodeInspector parentWindow){
			allConnections = new List<Connection> ();        

            Dictionary<Object, Connection> oneWayIncomingConnections = new Dictionary<Object, Connection> ();

			foreach (Node node in allNodes) {
				foreach (Joint jointData in node.Joints) {
					if (jointData.JointType == JointType.OneWay_IN) {
                        
                        if (oneWayIncomingConnections.ContainsKey(jointData.ObjectRefferenceValue))
                        {
                            Debug.LogError("FIXME: we already have this value " + jointData.ObjectRefferenceValue);
                        } else {                            
                            Connection connectionData = GetNewConnectionGUI(jointData);
                            oneWayIncomingConnections.Add(jointData.ObjectRefferenceValue, connectionData);
                        }
					} 
				}
			}


            Stack<Joint> UsedByLineJoints = new Stack<Joint>(); // all joints used by line would be store here
            bool handleJointDragging = true;
            LastDraggedConnection = null;
			//connect them to fields		
			foreach (Node node in allNodes) {
                foreach (Joint joint in node.Joints) {
					if (joint.JointType == JointType.OneWay_OUT) {
                        if (joint.ObjectRefferenceValue != null){
                            Connection connection;
                            if (oneWayIncomingConnections.ContainsKey(joint.ObjectRefferenceValue)){
                                connection = oneWayIncomingConnections[joint.ObjectRefferenceValue];
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
                                        if (LastDraggedConnection != null){
                                            LastDraggedConnection.ConnectionType = ConnectionRenderType.OutputToInput;
                                        }
                                        LastDraggedConnection = connection;

                                    } else {
                                        connection.ConnectionType = ConnectionRenderType.OutputToInput;
                                    }
                                }
							} 
						}						
					}
				}
			}

            if (LastDraggedConnection == null){                
                //lets check maybe we clicked some joints and want to connect it to something
                foreach (Node node in allNodes) {
                    foreach (Joint joint in node.Joints) {
                        if (!UsedByLineJoints.Contains(joint) && joint == parentWindow.StartDraggJoint){
                            //here is clicked node so lets create new connection with anonimouse target or source
                            Connection connection = Connection.GetInstance();
                            switch (joint.JointType){
                                case JointType.OneWay_IN:
                                    connection.InputJoint = joint;
                                    connection.OutputJoint = null;
                                    connection.ConnectionType = ConnectionRenderType.MouseToInputNode;
                                    break;
                                case JointType.OneWay_OUT:
                                    connection.OutputJoint = joint;
                                    connection.InputJoint = null;
                                    connection.ConnectionType = ConnectionRenderType.OutputNodeToMouse;
                                    break;
                                default:
                                    Debug.LogWarning("unsuported joint type"+ joint.JointType) ;
                                    break;
                            }
                            allConnections.Add(connection);
                            LastDraggedConnection = connection;
                        } 
                    }
                }
            }


            if (LastDraggedConnection != null){                
                switch (LastDraggedConnection.ConnectionType){
                    case ConnectionRenderType.MouseToInputNode:                        
                        parentWindow.JointHighlight.JointType = JointType.OneWay_OUT;
                        break;
                    case ConnectionRenderType.OutputNodeToMouse:
                        parentWindow.JointHighlight.JointType = JointType.OneWay_IN;
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