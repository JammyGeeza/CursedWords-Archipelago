using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Mod.GameObjects
{
    public class ArchipelagoDialog : MonoBehaviour
    {
        public class DialogResult
        {
            public bool Cancelled;
            public string Host;
            public string Slot;
            public string Password;
        }

        public DialogResult Result { get; private set; } = new DialogResult();

        private bool _isDone;

        /// <summary>
        /// Create an instance of the dialog window
        /// </summary>
        /// <param name="host">The archipelago host url</param>
        /// <param name="slotName">The archipelago slot name</param>
        /// <param name="password">The archipelago password</param>
        /// <returns></returns>
        public static ArchipelagoDialog Create(string host = "", string slotName = "", string password = "")
        {
            GameObject go = new GameObject("ArchipelagoDialog");
            ArchipelagoDialog dialog = go.AddComponent<ArchipelagoDialog>();

            dialog.Build(host, slotName, password);
            return dialog;
        }

        public IEnumerator WaitForResult()
        {
            yield return new WaitUntil(() => _isDone);
        }

        /// <summary>
        /// Build the dialog window
        /// </summary>
        /// <param name="host">The archipelago host url</param>
        /// <param name="slotName">The archipelago slot name</param>
        /// <param name="password">The archipelago password</param>
        private void Build(string host, string slotName, string password)
        {
            // Create canvas and place at front
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999;

            gameObject.AddComponent<CanvasScaler>();
            gameObject.AddComponent<GraphicRaycaster>();

            // Prevent user clicking outside of dialog
            GameObject blockerGO = new GameObject("Blocker");
            blockerGO.transform.SetParent(transform, false);

            Image blocker = blockerGO.AddComponent<Image>();
            blocker.color = new Color(0, 0, 0, 0.65f);
            blocker.raycastTarget = true;

            RectTransform blockerRect = blockerGO.GetComponent<RectTransform>();
            blockerRect.anchorMin = Vector2.zero;
            blockerRect.anchorMax = Vector2.one;
            blockerRect.offsetMin = Vector2.zero;
            blockerRect.offsetMax = Vector2.zero;

            // Create panel for input elements
            GameObject panelGO = new GameObject("Panel");
            panelGO.transform.SetParent(blockerGO.transform, false);

            Image panel = panelGO.AddComponent<Image>();
            panel.color = new Color(0.08f, 0.08f, 0.08f, 0.96f);

            RectTransform panelRect = panelGO.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(520, 340);
            panelRect.anchoredPosition = Vector2.zero;

            // Insert header text
            AddText(panelGO.transform, "Archipelago", 26, Color.white, TextAnchor.MiddleCenter,
                new Vector2(0, 135), new Vector2(460, 40));

            // Insert input fields
            InputField hostInput = AddLabelledInput(panelGO.transform, "Host", 65, placeholderText: "e.g. archipelago.gg:38281");
            hostInput.text = host;

            InputField slotInput = AddLabelledInput(panelGO.transform, "Slot", 5, placeholderText: "e.g. Player1");
            slotInput.text = slotName;

            InputField passwordInput = AddLabelledInput(panelGO.transform, "Password", -55, isPassword: true);
            passwordInput.text = password;

            // Insert 'Connect' button
            AddButton(panelGO.transform, "Connect", new Vector2(-75, -125), () =>
            {
                Result.Cancelled = false;
                Result.Host = hostInput.text;
                Result.Slot = slotInput.text;
                Result.Password = passwordInput.text;

                _isDone = true;
                Destroy(gameObject);
            });

            // Insert 'Cancel' button
            AddButton(panelGO.transform, "Cancel", new Vector2(75, -125), () =>
            {
                Result.Cancelled = true;

                _isDone = true;
                Destroy(gameObject);
            });

            EnsureEventSystem();
        }

        private InputField AddLabelledInput(Transform parent, string label, float y, string placeholderText = "", bool isPassword = false)
        {
            // Insert label text
            AddText(parent, label, 18, Color.white, TextAnchor.MiddleLeft,
                new Vector2(-170, y), new Vector2(130, 35));

            // Create input object
            GameObject inputGO = new GameObject($"{label}_Input");
            inputGO.transform.SetParent(parent, false);

            Image bg = inputGO.AddComponent<Image>();
            bg.color = Color.white;

            InputField input = inputGO.AddComponent<InputField>();
            input.contentType = isPassword
                ? InputField.ContentType.Password
                : InputField.ContentType.Standard;

            RectTransform rect = inputGO.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(280, 35);
            rect.anchoredPosition = new Vector2(80, y);

            // Insert text
            Text text = AddText(inputGO.transform, "", 18, Color.black, TextAnchor.MiddleLeft);
            Stretch(text.GetComponent<RectTransform>(), 8, 4, 8, 4);
            input.textComponent = text;

            // Insert placeholder
            Text placeholder = AddText(inputGO.transform, placeholderText, 18, Color.gray, TextAnchor.MiddleLeft);
            Stretch(placeholder.GetComponent<RectTransform>(), 8, 4, 8, 4);
            input.placeholder = placeholder;

            return input;
        }

        private void AddButton(Transform parent, string label, Vector2 pos, UnityAction onClick)
        {
            GameObject buttonGO = new GameObject($"{label}_Button");
            buttonGO.transform.SetParent(parent, false);

            Image image = buttonGO.AddComponent<Image>();
            image.color = Color.gray;

            Button button = buttonGO.AddComponent<Button>();
            button.onClick.AddListener(onClick);

            RectTransform rect = buttonGO.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(120, 40);
            rect.anchoredPosition = pos;

            Text text = AddText(buttonGO.transform, label, 20, Color.black, TextAnchor.MiddleCenter);
            Stretch(text.GetComponent<RectTransform>());
        }

        private Text AddText(Transform parent, string value, int size, Color color, TextAnchor alignment, Vector2? pos = null, Vector2? sizeDelta = null)
        {
            GameObject go = new GameObject("Text");
            go.transform.SetParent(parent, false);

            Text text = go.AddComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = size;
            text.color = color;
            text.alignment = alignment;
            text.raycastTarget = false;

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = sizeDelta ?? new Vector2(100, 30);
            rect.anchoredPosition = pos ?? Vector2.zero;

            return text;
        }

        private static void Stretch(RectTransform rect, float left = 0, float top = 0, float right = 0, float bottom = 0)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(left, bottom);
            rect.offsetMax = new Vector2(-right, -top);
        }

        private static void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null)
                return;

            GameObject go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            go.AddComponent<StandaloneInputModule>();
        }
    }
}