#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

[CustomEditor(typeof(Intro))]
public class SpriteVideoPlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Intro player = (Intro)target;

        if (GUILayout.Button("Load Sprites from Folder"))
        {
            string path = EditorUtility.OpenFolderPanel("Select Sprite Folder", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                LoadSpritesFromFolder(player, path);
            }
        }
    }

    private void LoadSpritesFromFolder(Intro player, string folderPath)
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