#NodeInspector
If you like to use scriptable object as config files you probably know how hard is maintain references from one object to another. This project would help you to organize your objects.

this is what you will get at the end of this simple tutorial 

![Simple nodes](https://cloud.githubusercontent.com/assets/1671030/13919456/ff03db6e-ef86-11e5-93e2-e9dcf3c753e1.png)

#Graph
Graph is a set of nodes connected by references. In our case we have **ScriptableObject** as a graph nodes.
To start work what we need is  to define Graph scriptable object which will keep links to all nodes. and all nodes just need to be inherited from **ScriptableObjectNode** class. If you open this class you will find that it just have one more property WindowRect. this property used in inspector to store information about node position.

It's possible that you want to store information about start node you can use it as well using directives. so let's start implement this

#MyConfig.cs
I'll create test class which will keep some properties and have a link to another object.

```C#
using UnityEngine;
using NodeInspector;

[NodeMenuItem("MyConfigNode")] //This directive create option menu in the inspector
[OneWay]       // If you won't provide this attribute you won't be abble to link to this instance as inputnode
public class MyConfigNode : ScriptableObjectNode {
    [OneWay]  //next property will have output link to another node
    public MyConfigNode LinkToNode;
    public int          TestInt;
    public LayerMask    TestLayerMask;
    [OneWay]
    public MyConfigNode LinkToAdditionalNode;
}
```

As you see it's really simple class you just  keep some properties here.


#MyConfigNodeHolder
```C#
using UnityEngine;
using System.Collections.Generic;
using NodeInspector;

[CreateAssetMenu] //This is standard unity feature. Create instance of this object in assets
public class MyConfigHolder : ScriptableObject { 
    [Graph("StartNode")] //this is a name of the property of this class
    public List<MyConfigNode> Nodes;

    public MyConfigNode StartNode; //if you don't need a start node you don't need to use Graph parameter
}
```

you can use several graphs at one scriptable object. but you graph must be a List of ScriptableObjectNode type or classes deprived from it.

So this is it. now you can create instance of your ConfigHodler object and start work with you graph.
Right click at your project window and select **Create/My Config Holder** after that you will have just asset in the folder. to start working with it you need to open node inspector it located at the top unity bar **Window/Node Inspector**

At the top left corner of the inspector you will find menu which create instances of your node. 
This is it. now you can freely connect your nodes as you wish. and if you want to create some node as start node just use context menu or toplight menu of the nodes.


