using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LevelTreeNode : Node
{
    public int Number;
    public bool IsFoo;
    public LevelTreeNode(string name, Rect position)
    {
        title = name;
        SetPosition(position);
    }
}
