using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(WorldGenerator))]
public class TextureCreatorINspector : Editor {

    private WorldGenerator worldGenerator;

    private void OnEnable() {
        worldGenerator = target as WorldGenerator;
        Undo.undoRedoPerformed += RefreshNoise;
    }

    private void OnDisable() {
        Undo.undoRedoPerformed -= RefreshNoise;
    }

    private void RefreshNoise() {
        if (Application.isPlaying) {
            foreach(GameObject chunk in GameObject.FindGameObjectsWithTag("Chunk"))
                GameObject.Destroy(chunk);
            worldGenerator.Start();
        }
    }

    public override void OnInspectorGUI() {
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        if (EditorGUI.EndChangeCheck()) {
            RefreshNoise();
        }
    }
}
