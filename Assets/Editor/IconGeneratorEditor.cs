using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IconGenerator))]
public class IconGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        IconGenerator generator = (IconGenerator)target;
        if (GUILayout.Button("Generate Icon"))
        {
            generator.GenerateIcon();
        }
    }
}
