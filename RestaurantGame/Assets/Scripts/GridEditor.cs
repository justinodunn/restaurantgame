using UnityEngine;
[UnityEditor.CustomEditor(typeof(Navigation))]
public class GridEditor : UnityEditor.Editor {
    Navigation grid;
    private void OnEnable() {
        grid = target as Navigation;
    }

    public override void OnInspectorGUI() {
        if (GUILayout.Button("Create New")) {
            grid.CreateGrid();
        }

        base.OnInspectorGUI();
    }
}

