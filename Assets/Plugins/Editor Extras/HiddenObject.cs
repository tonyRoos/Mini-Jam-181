#if UNITY_EDITOR
    using UnityEngine;

    [AddComponentMenu("Extras/HiddenObject")]
    public class HiddenObject : MonoBehaviour
    {
        [SerializeField] private bool isHidden = true;

        private void OnValidate() {
            RefreshHiddenState();
        }

        public void RefreshHiddenState() {
            gameObject.hideFlags = isHidden && UnityEditor.EditorPrefs.GetBool("EditorExtrasHideFlag") ? HideFlags.HideInHierarchy : HideFlags.None;
        }
    }
#endif