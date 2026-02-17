using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private bool isVRMode;
    [SerializeField] private NetworkPrefabRef vrPlayerPrefab;
    [SerializeField] private NetworkPrefabRef desktopPlayerPrefab;

    private GameObject _startUI;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            var prefab = isVRMode ? vrPlayerPrefab : desktopPlayerPrefab;
            var networkObject = Runner.Spawn(prefab, Vector3.zero, Quaternion.identity);
            DontDestroyOnLoad(networkObject.gameObject);
            Debug.Log($"[NetworkSpawner] Spawned {(isVRMode ? "VR" : "Desktop")} player for {player}");
        }

        int playerCount = 0;
        foreach (var _ in Runner.ActivePlayers)
            playerCount++;

        if (playerCount >= 2)
            ShowStartUI();
    }

    private void ShowStartUI()
    {
        if (_startUI != null)
            return;

        // --- World Space Canvas ---
        _startUI = new GameObject("StartUI");
        var canvas = _startUI.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        var rt = canvas.GetComponent<RectTransform>();
        rt.position = new Vector3(0f, 1.4f, 3.0f);
        rt.sizeDelta = new Vector2(400f, 150f);
        rt.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        // --- テキスト (上半分) ---
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(_startUI.transform, false);

        var tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "Press Trigger to Start";
        tmp.fontSize = 48;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        var textRt = tmp.rectTransform;
        textRt.anchorMin = new Vector2(0f, 0.5f);
        textRt.anchorMax = new Vector2(1f, 1f);
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        // --- ゲージ背景 (下半分) ---
        var bgObj = new GameObject("GaugeBackground");
        bgObj.transform.SetParent(_startUI.transform, false);

        var bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        var bgRt = bgImage.rectTransform;
        bgRt.anchorMin = new Vector2(0.05f, 0.1f);
        bgRt.anchorMax = new Vector2(0.95f, 0.4f);
        bgRt.offsetMin = Vector2.zero;
        bgRt.offsetMax = Vector2.zero;

        // --- ゲージ Fill ---
        var fillObj = new GameObject("GaugeFill");
        fillObj.transform.SetParent(bgObj.transform, false);

        var fillImage = fillObj.AddComponent<Image>();
        fillImage.color = new Color(0.2f, 0.8f, 0.2f, 1f);

        // fillAmount を機能させるために白スプライトを生成
        var tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        fillImage.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));

        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillAmount = 0f;

        var fillRt = fillImage.rectTransform;
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = Vector2.one;
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;

        // --- ゲージ表示を更新するコンポーネント ---
        var display = _startUI.AddComponent<StartGaugeDisplay>();
        display.gaugeFill = fillImage;

        Debug.Log("[NetworkSpawner] ShowStartUI: 2+ players joined");
    }

    public void PlayerLeft(PlayerRef player)
    {
    }
}
