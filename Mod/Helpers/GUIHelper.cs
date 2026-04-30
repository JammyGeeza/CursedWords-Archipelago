using BepInEx;
using UnityEngine;
using UnityEngine.Windows;

namespace Mod.Helpers
{
    internal static class GUIHelper
    {
        private static GameObject Root;
        private static GameObject Panel;

        static GUIHelper()
        {
            Root = new GameObject("ConfigUI_Root");
            Object.DontDestroyOnLoad(Root);

            Root.AddComponent<GUIRunner>().CreateConfigGUI();
        }

        private class GUIRunner : MonoBehaviour
        {
            public void CreateConfigGUI()
            {
                // TODO: Create a config menu - ideally something that appears once a user selects a game save.
                //       It either shows the last connection data for that save or prompts if there is none.
            }

            private void Update()
            {
                if (UnityInput.Current.GetKeyDown(KeyCode.F1))
                {
                    Panel.SetActive(!Panel.activeSelf);
                }
            }
        }
    }

}
