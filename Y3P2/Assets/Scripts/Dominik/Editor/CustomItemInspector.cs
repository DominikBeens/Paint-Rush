using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Item), true)]
public class CustomItemInspector : Editor 
{

    public Item item;

    private void OnEnable()
    {
        item = (Item)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(20);

        #region Save Asset Button
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save Asset"))
        {
            SaveAssetsDirty();
        }

        GUILayout.EndHorizontal();
        #endregion
    }

    private void SaveAssetsDirty()
    {
        EditorUtility.SetDirty(target);
        AssetDatabase.SaveAssets();
    }
}
