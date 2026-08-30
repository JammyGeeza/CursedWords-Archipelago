using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mod.Helpers
{
    public class NotificationHelper : MonoBehaviour
    {
        private static NotificationHelper _instance;
        public static NotificationHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Create();
                }

                return _instance;
            }
        }

        private CanvasGroup _canvasGroup;

        private float _fadeInSpeed = 0.2f;

        private float _fadeOutSpeed = 0.2f;

        private RectTransform _panelRectTransform;

        private TextMeshProUGUI _textTMP;

        private Coroutine _processRoutine;

        /// <summary>
        /// Queue of notifications to be displayed.
        /// </summary>
        private Queue<(string text, float displaySeconds)> NotificationQueue { get; set; } = new();

        private static NotificationHelper Create()
        {
            // Create canvas
            GameObject canvasGameObject = new GameObject("NotificationCanvas");
            Canvas canvas = canvasGameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32760;

            // Scaler to scale canvas with screen
            CanvasScaler canvasScaler = canvasGameObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
            canvasGameObject.AddComponent<GraphicRaycaster>();
            DontDestroyOnLoad(canvasGameObject);

            // Create panel
            GameObject panelGameObject = new GameObject("Panel", typeof(RectTransform));
            panelGameObject.transform.SetParent(canvasGameObject.transform, false);
            panelGameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);

            // Set panel position
            RectTransform panelRectTransform = panelGameObject.GetComponent<RectTransform>();
            panelRectTransform.anchorMin = new Vector2(0.25f, 1f);
            panelRectTransform.anchorMax = new Vector2(0.75f, 1f);
            panelRectTransform.pivot = new Vector2(0.5f, 1f);
            panelRectTransform.anchoredPosition = new Vector2(0f, -20f);
            panelRectTransform.sizeDelta = new Vector2(0f, 90f);

            // Create canvas group
            CanvasGroup canvasGroup = panelGameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            // Create text area
            GameObject textGameObject = new GameObject("Text", typeof(RectTransform));
            textGameObject.transform.SetParent(panelGameObject.transform, false);
            RectTransform textRectTransform = textGameObject.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.offsetMin = new Vector2(16f, 8f);
            textRectTransform.offsetMax = new Vector2(-16f, -8f);

            // Create text
            TextMeshProUGUI tmp = textGameObject.AddComponent<TextMeshProUGUI>();
            tmp.font = TMP_Settings.defaultFontAsset;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 24f;
            tmp.color = Color.white;
            tmp.richText = true;

            NotificationHelper controller = canvasGameObject.AddComponent<NotificationHelper>();
            controller._canvasGroup = canvasGroup;
            controller._textTMP = tmp;
            controller._panelRectTransform = panelRectTransform;

            return controller;
        }

        /// <summary>
        /// Queue a notification.
        /// </summary>
        /// <param name="text">The text of the notification</param>
        /// <param name="displaySeconds">The amount of seconds to display for.</param>
        public void Enqueue(string text, float displaySeconds = 2f)
        {
            NotificationQueue.Enqueue((text, displaySeconds));

            if (_processRoutine == null)
            {
                _processRoutine = StartCoroutine(ProcessQueue());
            }
        }

        /// <summary>
        /// Process the current queue.
        /// </summary>
        private IEnumerator ProcessQueue()
        {
            while (NotificationQueue.Count > 0)
            {
                (string text, float displaySeconds) = NotificationQueue.Dequeue();
                _textTMP.SetText(text);

                yield return Show(displaySeconds);
            }
            _processRoutine = null;
        }

        /// <summary>
        /// Show the notification control.
        /// </summary>
        /// <param name="displaySeconds">The amount of seconds to display for.</param>
        private IEnumerator Show(float displaySeconds)
        {
            // Linear fade-in
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / _fadeInSpeed;
                _canvasGroup.alpha = Mathf.Min(1f, t);
                yield return null;
            }

            // Fix at 100%
            _canvasGroup.alpha = 1f;

            // Wait for display seconds
            yield return new WaitForSeconds(displaySeconds);

            // Linear fade-out
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / _fadeOutSpeed;
                _canvasGroup.alpha = 1f - Mathf.Min(1f, t);
                yield return null;
            }

            // Fix at 0%
            _canvasGroup.alpha = 0f;
        }
    }
}
