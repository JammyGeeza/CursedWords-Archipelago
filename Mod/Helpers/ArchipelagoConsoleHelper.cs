using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.MessageLog.Parts;
using Archipelago.MultiClient.Net.Models;
using Mod.Mappings;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Mod.Helpers
{
    public class ArchipelagoConsoleHelper : MonoBehaviour
    {
        private readonly float _buttonHeight = 30f;

        private readonly float _buttonWidth = 150f;

        private readonly object _lock = new();

        private readonly int _maxLogSize = 250;

        private readonly float _panelHeight = 150f;

        /// <summary>
        /// The currently inputted string by the user.
        /// </summary>
        private string InputText { get; set; } = string.Empty;

        /// <summary>
        /// The message label style.
        /// </summary>
        private GUIStyle MessageStyle { get; set; }

        /// <summary>
        /// The log of all messages pending being displayed.
        /// </summary>
        private Queue<string> MessageQueue { get; set; } = new();

        /// <summary>
        /// The log of all displayed messages.
        /// </summary>
        private List<string> MessageLog { get; set; } = new();

        /// <summary>
        /// The scroll position of the message log.
        /// </summary>
        private Vector2 ScrollPosition { get; set; }

        /// <summary>
        /// Whether the log should scroll to the bottom
        /// </summary>
        private bool ScrollToBottom { get; set; }

        /// <summary>
        /// Whether or not to show the message log.
        /// </summary>
        private bool Show { get; set; } = false;


        private static ArchipelagoConsoleHelper _instance;
        public static ArchipelagoConsoleHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject gameObject = new GameObject("ArchipelagoConsole");
                    _instance = gameObject.AddComponent<ArchipelagoConsoleHelper>();

                    DontDestroyOnLoad(gameObject);
                }

                return _instance;
            }
        }

        // Event(s)
        public delegate void UserInputEvent(string message);
        public event UserInputEvent OnUserInput;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Show = !Show;
            }

            lock (_lock)
            {
                // Work through message queue
                while (MessageQueue.Count > 0)
                {
                    MessageLog.Add(MessageQueue.Dequeue());
                    ScrollToBottom = true;
                }

                // Clear messages older than 250 messages ago
                while (MessageLog.Count > _maxLogSize)
                {
                    MessageLog.RemoveAt(0);
                }
            }
        }

        void OnGUI()
        {
            // Set panel width to a third of screen size
            float panelWidth = (Screen.width / 3);
            Rect panelRect = new Rect((Screen.width - panelWidth) / 2f, 0, panelWidth, _panelHeight);
            Rect buttonRect = new Rect((Screen.width - _buttonWidth) / 2f, Show ? panelRect.yMax : 0, _buttonWidth, _buttonHeight);

            // The 'show / hide' toggle button
            if (GUI.Button(buttonRect, Show ? "Hide Console (F1)" : "Show Console (F1)"))
            {
                Show = !Show;
            }

            // Skip the rest if not showing
            if (!Show)
            {
                return;
            }

            // Set message style
            if (MessageStyle == null)
            {
                MessageStyle = new GUIStyle(GUI.skin.label)
                {
                    richText = true
                };
            }
            
            // Create panel
            GUI.Box(panelRect, GUIContent.none);
            GUILayout.BeginArea(panelRect);

            // If should scroll to bottom, do so
            if (ScrollToBottom)
            {
                ScrollPosition = new Vector2(ScrollPosition.x, float.MaxValue);
                ScrollToBottom = false;
            }

            // Set scroll view (bottom-aligned)
            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.Height(_panelHeight - 30));
            GUILayout.BeginVertical(GUILayout.MinHeight(_panelHeight - 30));
            GUILayout.FlexibleSpace();
            foreach(string message in MessageLog.ToArray())
            {
                GUILayout.Label(message, MessageStyle);
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            // Check if return has been pressed
            bool returnPressed = Event.current.type == EventType.KeyDown && (Event.current.keyCode is KeyCode.Return or KeyCode.KeypadEnter);

            // Create input field
            GUI.SetNextControlName("ArchipelagoConsoleInput");
            InputText = GUILayout.TextField(InputText);

            // If return pressed on input field, submit it
            if (returnPressed && GUI.GetNameOfFocusedControl() == "ArchipelagoConsoleInput")
            {
                OnUserInput?.Invoke(InputText);
                InputText = "";
                Event.current.Use();
            }

            GUILayout.EndArea();
        }

        /// <summary>
        /// Add a message to the message log.
        /// </summary>
        /// <param name="logMessage">The archipelago log message to add to the message log.</param>
        public void AddMessage(LogMessage logMessage)
        {
            string parsedMessage = Parse(logMessage);
            AddMessage(parsedMessage);
        }

        /// <summary>
        /// Add a message to the message log.
        /// </summary>
        /// <param name="message">The text to add to the message log.</param>
        public void AddMessage(string message)
        {
            lock (_lock)
            {
                MessageQueue.Enqueue($"[{DateTime.Now.ToString("HH:mm:ss")}]\t{message}");
            }
        }

        /// <summary>
        /// Parse an archipelago log message to formatted text.
        /// </summary>
        /// <param name="logMessage">The archipelago log message to parse.</param>
        /// <returns>The parsed log message string.</returns>
        private string Parse(LogMessage logMessage)
        {
            StringBuilder stringBuilder = new StringBuilder();
            
            foreach(MessagePart part in logMessage.Parts)
            {
                stringBuilder.Append($"<color=#{Colours.GetColourHex(part.Color)}>{part.Text}</color>");
            }
            
            return stringBuilder.ToString();
        }
    }
}
