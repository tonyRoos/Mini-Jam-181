#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

/* Esse código preenche os frames na classe "FrameByFrameVideo", ele só roda no editor, graças a notação #if UNITY_EDITOR e #endif */

[CustomEditor(typeof(FrameByFrameVideo))]
public class FrameFiller : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FrameByFrameVideo player = (FrameByFrameVideo)target;

        if (GUILayout.Button("Load Sprites from Folder"))
        {
            string path = EditorUtility.OpenFolderPanel("Select Sprite Folder", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                LoadSpritesFromFolder(player, path);
            }
        }
    }

    private void LoadSpritesFromFolder(FrameByFrameVideo player, string folderPath)
    {
        string relativePath = "Assets" + folderPath.Substring(Application.dataPath.Length);
        string[] files = Directory.GetFiles(relativePath, "*.png");

        Sprite[] sprites = files.Select(file => AssetDatabase.LoadAssetAtPath<Sprite>(file)).Where(sprite => sprite != null).ToArray();

        if (sprites.Length > 0)
        {
            Undo.RecordObject(player, "Load Sprites into SpriteVideoPlayer");
            player.populateFrames(sprites);
            EditorUtility.SetDirty(player);
        }
        else
        {
            Debug.LogWarning("No sprites found in the selected folder!");
        }
    }
}
#endif