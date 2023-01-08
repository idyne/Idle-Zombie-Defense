using UnityEditor;
using FateGames;
public abstract class HideScriptInEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        DrawPropertiesExcluding(serializedObject, "m_Script");
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }
}
