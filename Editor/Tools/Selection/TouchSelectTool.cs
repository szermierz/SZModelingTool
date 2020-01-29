using UnityEditor;
using UnityEngine;


namespace SZ.ModelingTool
{
    public class ModelingWindow : EditorWindow
    {
        private const string c_windowTitle = "3D Modeling tool";
        private const string c_windowMenuItem = "Tools/SZ/3D Modeling tool";

        private void OnEnable()
        {
            Debug.Log("enab");
        }

        private void OnDisable()
        {

            Debug.Log("ds");
        }

        [MenuItem(c_windowMenuItem)]
        private static void Create()
        {
            var window = GetWindow<ModelingWindow>();

            window.titleContent.text = c_windowTitle;

            window.Show();
        }

        private void OnInspectorUpdate() => Repaint();

        private void OnGUI()
        {

        }

        private void OnFocus() =>
            OnSelectionChange();

        private void OnSelectionChange()
        {

        }
    }
}