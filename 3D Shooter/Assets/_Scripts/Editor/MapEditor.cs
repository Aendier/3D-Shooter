using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator map = target as MapGenerator;

        //如果签的值发生改变
        if (DrawDefaultInspector())
        {
            //如果自动更新
            if (map.autoUpdate)
            {
                //当值发生改变时，自动更新
                map.GenerateMap(); 
            }
        }

        //如果点击了Generate按钮
        if (GUILayout.Button("Generate"))
        {
            //手动更新
            map.GenerateMap();
        }
    }
}
