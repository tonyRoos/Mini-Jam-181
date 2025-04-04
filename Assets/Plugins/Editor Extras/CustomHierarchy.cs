#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class CustomHierarchy
{
    static bool _hierarchyHasFocus = false;
    static EditorWindow _hierarchyEditorWindow;

    static CustomHierarchy() {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemOnGUI;
        EditorApplication.update += OnEditorUpdate;
    }

    private static void OnEditorUpdate() {
        if(_hierarchyEditorWindow == null)
            _hierarchyEditorWindow = EditorWindow.GetWindow(System.Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor"));

        _hierarchyHasFocus = EditorWindow.focusedWindow != null &&
            EditorWindow.focusedWindow == _hierarchyEditorWindow;
    }

    private static void OnHierarchyItemOnGUI(int instanceID, Rect selectionRect) {
        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if(obj == null) return;

        Texture icon = EditorGUIUtility.ObjectContent(obj, obj.GetType()).image;

        if(PrefabUtility.GetCorrespondingObjectFromOriginalSource(obj) != null) return;

        Component[] components = obj.GetComponents<Component>();
        if(components == null || components.Length == 0) return;

        Component component = components.Length > 1 ? components[1] : components[0];

        Type type = component.GetType();

        GUIContent content = EditorGUIUtility.ObjectContent(component, type);
        content.text = null;
        content.tooltip = type.Name;

        content.image = (icon.name == "GameObject Icon" || icon.name == "d_GameObject Icon") ? content.image : icon;

        bool isSelected = Selection.instanceIDs.Contains(instanceID);
        bool isHovering = selectionRect.Contains(Event.current.mousePosition);

        Color color = UnityEditorBackgroundColor.Get(isSelected, isHovering, _hierarchyHasFocus);
        Rect backgroundRect = selectionRect;
        backgroundRect.width = 18.5f;
        EditorGUI.DrawRect(backgroundRect, color);

        EditorGUI.LabelField(selectionRect, content);
    }
}
#endif