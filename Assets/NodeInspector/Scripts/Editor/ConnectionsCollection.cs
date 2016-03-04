using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace NodeInspector.Editor{	
	public class ConnectionsCollection {
		public List<ConnectionData> allConnections;

		public ConnectionsCollection(List<NodeGUI> allNodes){
			allConnections = new List<ConnectionData> ();



			Dictionary<Object, ConnectionData> incognitoInConnections = new Dictionary<Object, ConnectionData> ();


			//collect all incognito inputconnections
			foreach (NodeGUI node in allNodes) {
				foreach (JointData jointData in node.Joints) {
					if (jointData.JointType == JointType.Incognito_In) {
						ConnectionData connectionData = new ConnectionData ();
						connectionData.InputJoint = jointData;
                        incognitoInConnections.Add (jointData.ObjectRefferenceValue, connectionData);
					} 
				}
			}


			//connect them to fields		
			foreach (NodeGUI node in allNodes) {
				foreach (JointData jointData in node.Joints) {
					if (jointData.JointType == JointType.OneToOne_Incognito_OUT || jointData.JointType == JointType.ManyToOne_Incognito_OUT) {
                        if (jointData.ObjectRefferenceValue != null){
							ConnectionData connectionData;
                            if (incognitoInConnections.ContainsKey(jointData.ObjectRefferenceValue)){
                                connectionData = incognitoInConnections[jointData.ObjectRefferenceValue];
								connectionData.OutputJoint = jointData;
								allConnections.Add(connectionData);
							} 
						}						
					}
				}
			}
		}
	}
}