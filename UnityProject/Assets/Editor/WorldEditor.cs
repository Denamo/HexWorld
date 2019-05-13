using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(World))]
public class WorldEditor : Editor
{
    World world;

    void OnEnable()
    {
        world = target as World;
    }

         
    public override void OnInspectorGUI()
    {

        //EditorGUI.BeginChangeCheck();
        bool change = DrawDefaultInspector();
        //bool change = EditorGUI.EndChangeCheck();

        if (change)
        {
            world.UpdateWorld();
        }

    }

}
