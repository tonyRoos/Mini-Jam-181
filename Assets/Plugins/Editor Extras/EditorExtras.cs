#if UNITY_EDITOR
    using UnityEditor;
using UnityEngine;

public static class EditorExtras {
            const string HiddenGameObjects = "Extras/Show Hidden Objects";


            [MenuItem(HiddenGameObjects)] 
            static void ToggleShowHiddenObjects() { 
                EditorPrefs.SetBool("EditorExtrasHideFlag", !EditorPrefs.GetBool("EditorExtrasHideFlag", false));
                foreach(HiddenObject hiddenObject in GameObject.FindObjectsByType<HiddenObject>(FindObjectsSortMode.None)) {
                    hiddenObject.RefreshHiddenState();
                }
            }

            [MenuItem(HiddenGameObjects, true)] 
            static bool ToggleShowHiddenObjectsValidation() {
        UnityEditor.Menu.SetChecked(HiddenGameObjects, !EditorPrefs.GetBool("EditorExtrasHideFlag", false));
                return true;
            }

        }
#endif