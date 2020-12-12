using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Skeleton))]
public class SkeletonInspector : Editor
{
    public override void OnInspectorGUI()
    {
        Skeleton s = (Skeleton)target;
        if (GUILayout.Button((s.GetIsShowingSkeleton() ? "Hide" : "Show") + " skeleton"))
        {
            s.SetIsShowingSkeleton(!s.GetIsShowingSkeleton());
        }
        DrawDefaultInspector();
    }
}
