using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using VE = UnityEngine.UIElements.VisualElementExtensions;
using VEDirection = UnityEditor.Experimental.GraphView.Direction;

public class LevelTreeGraph : GraphView
{
    public LevelTreeGraph()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        VE.AddManipulator(this, new ContentDragger());
        VE.AddManipulator(this, new SelectionDragger());
        VE.AddManipulator(this, new RectangleSelector());
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach(port =>
        {
            if (startPort != port && startPort.node != port.node && startPort.direction != port.direction)
                compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    public Port GeneratePort(string name, LevelTreeNode node, Orientation orientation, VEDirection direction, Port.Capacity capacity, System.Type type)
    {
        var port = node.InstantiatePort(orientation, direction, capacity, type);
        port.portName = name;

        return port;
    }
}
