#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ExploreTogether{
    [InitializeOnLoad]
    public class EditorSaveHandler
    {
        static EditorSaveHandler()
        {
            EditorApplication.quitting += () =>
            {
                Debug.Log("Editor is closing, saving game...");
                // Call your save function here
            };
        }
    }
}
#endif
