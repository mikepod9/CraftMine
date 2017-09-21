using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(WorldGenerator))]
public class TextureCreatorINspector : Editor {

    private WorldGenerator noise;

    private void OnEnable() {
        noise = target as WorldGenerator;
        Undo.undoRedoPerformed += RefreshNoise;
    }

    private void OnDisable() {
        Undo.undoRedoPerformed -= RefreshNoise;
    }

    private void RefreshNoise() {
        if (Application.isPlaying) {
            //noise.FillTexture();
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
