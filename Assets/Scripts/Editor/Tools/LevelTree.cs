using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelTree : EditorWindow
{
    [SerializeField]
    string fileName = "File Name";
    private static bool _close;

    LevelTreeGraph _graphView;

    [MenuItem("Tools/Level Tools/Level Tree View/Open")]
    public static void ShowLevelTree()
    {
        _close = false;
        // Get existing open window or if none, make a new one:
        var window = GetWindow<LevelTree>("Level Tree View");
        //window.Show();
    }

    [MenuItem("Tools/Level Tools/Level Tree View/Close")]
    public static void CloseAllWindows()
    {
        _close = true;
    }

    private void CreateGUI()
    {
        // Each editor window contains a root VisualElement object.
        bool t = false;
        VisualElement root = rootVisualElement;
        VisualElement sub = new VisualElement { style = { flexDirection = FlexDirection.Row, alignContent = Align.Auto } };
        sub.Add(new Button(delegate() { AddNode(); }){ text = "Add Node", style = { maxWidth = 125 } });
        sub.Add(new Button { text = "Test Button 2", style = { maxWidth = 125 } });
        root.Add(sub);
        root.Bind(new SerializedObject(this));
    }

    private void Update()
    {
        if (_close)
            Close();
    }

    public LevelTreeNode AddNode()
    {
        Debug.Log("Add Node");
        Rect pos = new Rect(_graphView.contentRect.xMax / 2, _graphView.contentRect.yMax / 2, 100, 100);
        var node = new LevelTreeNode("Level0x0", pos);
        var port = _graphView.GeneratePort("Exit 0", node, UnityEditor.Experimental.GraphView.Orientation.Horizontal, UnityEditor.Experimental.GraphView.Direction.Output,
            UnityEditor.Experimental.GraphView.Port.Capacity.Multi, typeof(LevelTreeNode));
        node.outputContainer.Add(port);

        var enterPort = _graphView.GeneratePort("Entrance 0", node, UnityEditor.Experimental.GraphView.Orientation.Horizontal, UnityEditor.Experimental.GraphView.Direction.Input,
            UnityEditor.Experimental.GraphView.Port.Capacity.Multi, typeof(LevelTreeNode));
        node.inputContainer.Add(enterPort);

        node.RefreshExpandedState();
        node.RefreshPorts();
        _graphView.AddElement(node);
        return node;
    }

    private void OnEnable()
    {
        _graphView = new LevelTreeGraph { name = "Graph Test" };
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }

    private void OnGUI()
    {
        //The rectangle is drawn in the Editor (when MyScript is attached) with the width depending on the value of the Slider
        //EditorGUI.DrawRect(new Rect(50, 350, 100, 70), Color.green);
    }
}
