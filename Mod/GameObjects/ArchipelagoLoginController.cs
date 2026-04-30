using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Mod.Helpers;

public enum DialogState
{
    Login,
    Connecting,
    Error
}

public class ArchipelagoLoginController : MonoBehaviour
{
    public string Host
    { 
        get => _host.text;
    }

    public string Slot
    {
        get => _slot.text;
    }

    public string Password
    {
        get => _password.text;
    }

    public bool Cancelled
    {
        get => _cancelled;
    }

    // Events
    public Action OnConnect;
    public Action OnCancel;

    // Private
    private bool _finished;
    private bool _cancelled;

    private GameObject _loginState;
    private GameObject _connectingState;
    private GameObject _errorState;

    private Text _errorText;

    private InputField _host;
    private InputField _slot;
    private InputField _password;

    public static ArchipelagoLoginController Create(ArchipelagoData data)
    {
        GameObject go = new GameObject("ArchipelagoLoginController");
        ArchipelagoLoginController dialog = go.AddComponent<ArchipelagoLoginController>();

        dialog.Build(data.Host, data.Slot, data.Password);
        dialog.SetState(DialogState.Login, null);

        return dialog;
    }

    public IEnumerator WaitForFinish()
    {
        yield return new WaitUntil(delegate
        {
            return _finished;
        });
    }

    public void SetState(DialogState state, string error)
    {
        _loginState.SetActive(state == DialogState.Login);
        _connectingState.SetActive(state == DialogState.Connecting);
        _errorState.SetActive(state == DialogState.Error);

        if (state == DialogState.Error && _errorText != null)
        {
            _errorText.text = error != null ? error : "Connection failed.";
        }
    }

    public void Close()
    {
        _finished = true;
        Destroy(this.gameObject);
    }

    private void Build(string host, string slot, string password)
    {
        Canvas canvas = this.gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        this.gameObject.AddComponent<CanvasScaler>();
        this.gameObject.AddComponent<GraphicRaycaster>();

        GameObject blocker = CreateFullscreenBlocker(this.transform);

        _loginState = BuildLogin(blocker.transform, host, slot, password);
        _connectingState = BuildConnecting(blocker.transform);
        _errorState = BuildError(blocker.transform);

        EnsureEventSystem();
    }

    #region States

    private GameObject BuildLogin(Transform parent, string host, string slot, string password)
    {
        GameObject panel = CreateCenteredPanel(parent, new Vector2(520f, 340f));

        AddText(panel.transform, "Archipelago", 26, Color.white, new Vector2(0f, 135f), TextAnchor.MiddleCenter);

        // Add host input
        _host = AddInput(panel.transform, "Host", "archipelago.gg:38281", 65f, false);
        _host.text = host;

        // Add slot input
        _slot = AddInput(panel.transform, "Slot", "Player1", 5f, false);
        _slot.text = slot;

        // Add password input
        _password = AddInput(panel.transform, "Password", "Optional, if required", -55f, true);
        _password.text = password;

        AddButton(panel.transform, "Connect", new Vector2(-75f, -125f), OnConnectClicked);
        AddButton(panel.transform, "Cancel", new Vector2(75f, -125f), OnCancelClicked);

        return panel;
    }

    private GameObject BuildConnecting(Transform parent)
    {
        GameObject panel = CreateCenteredPanel(parent, new Vector2(420f, 180f));

        AddText(panel.transform, "Connecting...", 26, Color.white, Vector2.zero);

        return panel;
    }

    private GameObject BuildError(Transform parent)
    {
        GameObject panel = CreateCenteredPanel(parent, new Vector2(520f, 260f));

        AddText(panel.transform, "Error", 26, Color.white, new Vector2(0f, 85f), TextAnchor.MiddleCenter);

        _errorText = AddText(panel.transform, "", 18, Color.white, new Vector2(0f, 15f), new Vector2(440f, 90f), TextAnchor.MiddleCenter);

        AddButton(panel.transform, "Back", new Vector2(-75f, -90f), BackToLogin);
        AddButton(panel.transform, "Cancel", new Vector2(75f, -90f), OnCancelClicked);

        return panel;
    }

    #endregion

    #region UI

    private GameObject CreateFullscreenBlocker(Transform parent)
    {
        GameObject go = new GameObject("Blocker");
        go.transform.SetParent(parent, false);

        Image img = go.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0.65f);
        img.raycastTarget = true;

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        return go;
    }

    private GameObject CreateCenteredPanel(Transform parent, Vector2 size)
    {
        GameObject go = new GameObject("Panel");
        go.transform.SetParent(parent, false);

        Image img = go.AddComponent<Image>();
        img.color = new Color(0.08f, 0.08f, 0.08f, 0.96f);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;

        return go;
    }

    private InputField AddInput(Transform parent, string label, string title, float y, bool password)
    {
        AddText(parent, label, 18, Color.white, new Vector2(-120f, y));

        GameObject go = CreateCenteredPanel(parent, new Vector2(280f, 35f));
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(80f, y);

        Image bg = go.GetComponent<Image>();
        bg.color = Color.white;

        InputField input = go.AddComponent<InputField>();
        input.contentType = password ? InputField.ContentType.Password : InputField.ContentType.Standard;

        Text text = AddText(go.transform, "", 18, Color.black, Vector2.zero);
        Stretch(text.GetComponent<RectTransform>(), 8f, 4f, 8f, 4f);
        input.textComponent = text;

        Text placeholder = AddText(go.transform, title, 18, Color.gray, Vector2.zero);
        Stretch(placeholder.GetComponent<RectTransform>(), 8f, 4f, 8f, 4f);
        input.placeholder = placeholder;

        return input;
    }

    private void AddButton(Transform parent, string label, Vector2 pos, Action onClick)
    {
        GameObject go = CreateCenteredPanel(parent, new Vector2(120f, 40f));
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = pos;

        Image img = go.GetComponent<Image>();
        img.color = Color.gray;

        Button button = go.AddComponent<Button>();
        button.onClick.AddListener(delegate { onClick(); });

        Text text = AddText(go.transform, label, 20, Color.black, Vector2.zero, TextAnchor.MiddleCenter);
        Stretch(text.GetComponent<RectTransform>(), 0f, 0f, 0f, 0f);
    }

    private Text AddText(Transform parent, string value, int size, Color color, Vector2 pos, TextAnchor anchor = TextAnchor.MiddleLeft)
    {
        return AddText(parent, value, size, color, pos, new Vector2(200f, 40f), anchor);
    }

    private Text AddText(Transform parent, string value, int size, Color color, Vector2 pos, Vector2 sizeDelta, TextAnchor anchor)
    {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(parent, false);

        Text text = go.AddComponent<Text>();
        text.text = value;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = size;
        text.color = color;
        text.alignment = anchor;
        text.raycastTarget = false;

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = sizeDelta;
        rect.anchoredPosition = pos;

        return text;
    }

    private void Stretch(RectTransform rect, float left, float top, float right, float bottom)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, -top);
    }

    #endregion

    private void OnConnectClicked()
    {
        if (OnConnect != null)
        {
            OnConnect();
        }
    }

    private void OnCancelClicked()
    {
        _cancelled = true;
        _finished = true;

        if (OnCancel != null)
        {
            OnCancel();
        }

        Destroy(this.gameObject);
    }

    private void BackToLogin()
    {
        SetState(DialogState.Login, null);
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