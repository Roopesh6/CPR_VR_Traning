using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CPRSceneUI : MonoBehaviour
{
    private int tutorialStep;
    private Text status;
    private Text metrics;
    private Button actionButton;
    private CPRSession session;
    private float tutorialStart;
    private bool tutorialStarted;

    private readonly string[] steps = {
        "Check that the scene is safe. Tap Next when ready.",
        "Check responsiveness and call for emergency help. Tap Next.",
        "Expose the chest and locate the center of the chest. Tap Next.",
        "Place both hands on the center of the chest. Click Start compressions, then press Space steadily.",
        "Continue compressions. Aim for a steady 100–120 per minute in this practice demo."
    };

    private void Start()
    {
        session = FindObjectOfType<CPRSession>();
        BuildUI();
        Refresh();
    }

    private void Update()
    {
        if (tutorialStarted && session != null && session.IsActive && Time.time - tutorialStart > 60f)
        {
            session.FinishTest(); tutorialStarted = false;
            status.text = "Tutorial practice complete. Review your rhythm, then continue to Testing.";
            actionButton.GetComponentInChildren<Text>().text = "Continue to Testing";
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() => Load("Testing"));
        }
        if (metrics != null && session != null && session.IsActive)
            metrics.text = $"Compressions: {session.CompressionCount}\nRate: {session.CurrentRate:0} / min\nTime: {session.SessionSeconds:0}s";
    }

    private void BuildUI()
    {
        var canvasObject = new GameObject("Training Dashboard", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasObject.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObject.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        if (SceneManager.GetActiveScene().name == "MainMenu") { BuildMenu(canvas.transform); return; }

        var panel = MakePanel(canvas.transform, new Vector2(0.5f, 0.82f), new Vector2(0.62f, 0.26f));
        status = MakeText(panel, "", 24, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.70f), new Vector2(0.94f, 0.48f));
        metrics = MakeText(panel, "", 20, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.32f), new Vector2(0.94f, 0.16f));
        actionButton = MakeButton(panel, "Next", new Vector2(0.5f, 0.08f), new Vector2(0.34f, 0.18f), OnAction);
        if (SceneManager.GetActiveScene().name == "Progress") BuildProgress(panel);
        else if (SceneManager.GetActiveScene().name == "Tutorial") Refresh();
        else { status.text = "60-second CPR practice\nClick Start test, then press Space to register each compression."; actionButton.GetComponentInChildren<Text>().text = "Start test"; actionButton.onClick.AddListener(() => { session.Begin(); status.text = "Test in progress. Keep a steady rhythm."; }); }
        MakeButton(canvas.transform, "Main Menu", new Vector2(0.11f, 0.08f), new Vector2(0.16f, 0.08f), () => Load("MainMenu"));
    }

    private void BuildMenu(Transform parent)
    {
        var panel = MakePanel(parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.55f));
        MakeText(panel, "CPR TRAINING\nDesktop Practice Prototype", 34, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.82f), new Vector2(0.92f, 0.3f));
        MakeButton(panel, "Guided Tutorial", new Vector2(0.5f, 0.55f), new Vector2(0.66f, 0.14f), () => Load("Tutorial"));
        MakeButton(panel, "Start Test", new Vector2(0.5f, 0.35f), new Vector2(0.66f, 0.14f), () => Load("Testing"));
        MakeButton(panel, "Progress", new Vector2(0.5f, 0.15f), new Vector2(0.66f, 0.14f), () => Load("Progress"));
    }

    private void BuildProgress(Transform panel)
    {
        int practices = PlayerPrefs.GetInt("CPR_Practices", 0);
        float best = PlayerPrefs.GetFloat("CPR_BestScore", 0f);
        status.text = $"Your practice progress\n\nCompleted tests: {practices}\nBest rhythm score: {best:0}/100";
        metrics.text = "Score is a simple rhythm-only demo metric.";
        actionButton.GetComponentInChildren<Text>().text = "Practice again";
        actionButton.onClick.AddListener(() => Load("Testing"));
    }

    private void OnAction()
    {
        if (SceneManager.GetActiveScene().name != "Tutorial") return;
        if (tutorialStep < steps.Length - 1) { tutorialStep++; Refresh(); }
        else if (!tutorialStarted) { tutorialStarted = true; tutorialStart = Time.time; session.Begin(); status.text = steps[tutorialStep]; }
    }

    private void Refresh()
    {
        if (status == null || SceneManager.GetActiveScene().name != "Tutorial") return;
        status.text = $"GUIDED TUTORIAL  •  Step {tutorialStep + 1}/{steps.Length}\n\n{steps[tutorialStep]}";
        actionButton.GetComponentInChildren<Text>().text = tutorialStep < steps.Length - 1 ? "Next" : (tutorialStarted ? "Practice running" : "Start practice");
    }

    private void Load(string scene) => SceneManager.LoadScene(scene);

    private static RectTransform MakePanel(Transform parent, Vector2 anchor, Vector2 size)
    {
        var go = new GameObject("Dashboard Panel", typeof(Image)); go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>(); rt.anchorMin = rt.anchorMax = anchor; rt.sizeDelta = new Vector2(1280f * size.x, 720f * size.y); rt.anchoredPosition = Vector2.zero;
        go.GetComponent<Image>().color = new Color(0.04f, 0.09f, 0.14f, 0.92f); return rt;
    }
    private static Text MakeText(Transform parent, string value, int size, TextAnchor align, Vector2 anchor, Vector2 dimensions)
    {
        var go = new GameObject("Dashboard Text", typeof(Text)); go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>(); rt.anchorMin = rt.anchorMax = anchor; rt.sizeDelta = ParentSize(parent, dimensions); rt.anchoredPosition = Vector2.zero;
        var text = go.GetComponent<Text>(); text.text = value; text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); text.fontSize = size; text.alignment = align; text.color = Color.white; text.horizontalOverflow = HorizontalWrapMode.Wrap; text.verticalOverflow = VerticalWrapMode.Overflow; return text;
    }
    private static Button MakeButton(Transform parent, string label, Vector2 anchor, Vector2 dimensions, UnityEngine.Events.UnityAction click)
    {
        var go = new GameObject(label, typeof(Image), typeof(Button)); go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>(); rt.anchorMin = rt.anchorMax = anchor; rt.sizeDelta = ParentSize(parent, dimensions); rt.anchoredPosition = Vector2.zero;
        go.GetComponent<Image>().color = new Color(0.08f, 0.48f, 0.65f); var b = go.GetComponent<Button>(); b.onClick.AddListener(click);
        var text = MakeText(go.transform, label, 20, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.92f, 0.85f)); text.raycastTarget = false; return b;
    }

    private static Vector2 ParentSize(Transform parent, Vector2 dimensions)
    {
        var parentRect = parent as RectTransform;
        Vector2 size = parentRect != null ? parentRect.rect.size : new Vector2(1280f, 720f);
        return new Vector2(size.x * dimensions.x, size.y * dimensions.y);
    }
}
