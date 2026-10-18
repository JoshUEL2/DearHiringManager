using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class SnatchBriefcaseMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;

    private VisualElement container;
    private VisualElement[] sbDialStrips = new VisualElement[3];
    private Label[] sbDialsOut = new Label[3];
    private Label[] sbDialsTop = new Label[3];
    private Label[] sbDialsMid = new Label[3];
    private Label[] sbDialsBot = new Label[3];
    private Label sbTargetText;
    private Button sbStopBtn;

    private int sbCurrentDial = 0;
    private int[] sbTargetNums = new int[3];
    private int[] sbCurrentDialNums = new int[3];
    private float sbSpinTimer = 0f;
    private float timer = 0f;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;

        container = root.Q<VisualElement>("SnatchBriefcaseContainer");
        for (int i = 0; i < 3; i++)
        {
            sbDialStrips[i] = root.Q<VisualElement>($"SbDial{i}_Strip");
            sbDialsOut[i] = root.Q<Label>($"SbDial{i}_Out");
            sbDialsTop[i] = root.Q<Label>($"SbDial{i}_Top");
            sbDialsMid[i] = root.Q<Label>($"SbDial{i}_Mid");
            sbDialsBot[i] = root.Q<Label>($"SbDial{i}_Bot");
        }
        sbTargetText = root.Q<Label>("SbTargetText");
        sbStopBtn = root.Q<Button>("BtnSbStop");

        if (sbStopBtn != null)
        {
            sbStopBtn.clicked += OnSbStopClicked;
        }
    }

    public void StartGame()
    {
        if (container != null) container.style.display = DisplayStyle.Flex;

        timer = config.timeLimit;
        sbCurrentDial = 0;
        sbSpinTimer = 0f;
        if (sbStopBtn != null) sbStopBtn.text = "STOP DIAL 1";

        for (int i = 0; i < 3; i++)
        {
            sbTargetNums[i] = Random.Range(0, 10);
            sbCurrentDialNums[i] = Random.Range(0, 10);

            if (sbDialsOut[i] != null) sbDialsOut[i].text = ((sbCurrentDialNums[i] + 2) % 10).ToString();
            if (sbDialsTop[i] != null) sbDialsTop[i].text = ((sbCurrentDialNums[i] + 1) % 10).ToString();
            if (sbDialsMid[i] != null) sbDialsMid[i].text = sbCurrentDialNums[i].ToString();
            if (sbDialsBot[i] != null) sbDialsBot[i].text = ((sbCurrentDialNums[i] - 1 + 10) % 10).ToString();

            if (sbDialsMid[i] != null) sbDialsMid[i].style.color = Color.black;
            if (sbDialStrips[i] != null) sbDialStrips[i].style.top = -75f;
        }
        if (sbTargetText != null) sbTargetText.text = $"TARGET: {sbTargetNums[0]} - {sbTargetNums[1]} - {sbTargetNums[2]}";

        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        if (container != null) container.style.display = DisplayStyle.None;
        if (sbStopBtn != null) sbStopBtn.clicked -= OnSbStopClicked;
    }

    private void OnSbStopClicked()
    {
        if (!manager.isPlaying || sbCurrentDial >= 3) return;

        int currentNum = sbCurrentDialNums[sbCurrentDial];
        if (currentNum == sbTargetNums[sbCurrentDial])
        {
            AudioManager.Instance?.PlayScorePop();
            MiniGameFeedback.PlaySuccess(sbStopBtn, "CLICK! 🔓", container);
            if (sbDialsMid[sbCurrentDial] != null) sbDialsMid[sbCurrentDial].style.color = new Color(0.18f, 0.8f, 0.44f); // Green
            if (sbDialStrips[sbCurrentDial] != null) sbDialStrips[sbCurrentDial].style.top = -75f; // Snap to perfect center
            sbCurrentDial++;
            if (sbCurrentDial >= 3)
            {
                if (sbStopBtn != null) sbStopBtn.text = "UNLOCKED!";
                manager.MarkWinReady();
            }
            else
            {
                if (sbStopBtn != null) sbStopBtn.text = $"STOP DIAL {sbCurrentDial + 1}";
            }
        }
        else
        {
            AudioManager.Instance?.PlayBuzzer();
            MiniGameFeedback.PlayError(sbStopBtn, "JAMMED!", container);
            if (sbDialsMid[sbCurrentDial] != null) sbDialsMid[sbCurrentDial].style.color = new Color(0.9f, 0.3f, 0.23f); // Red
            if (sbStopBtn != null) sbStopBtn.text = "WRONG PIN!";
            manager.AbortGame();
        }
    }

    private IEnumerator GameRoutine()
    {
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            sbSpinTimer += Time.deltaTime;

            // Speed starts at 0.12. Gets faster with each lock cracked.
            float currentSpeed = 0.35f - (sbCurrentDial * 0.08f);
            if (currentSpeed < 0.15f) currentSpeed = 0.15f;

            float progress = sbSpinTimer / currentSpeed;
            if (progress > 1f) progress = 1f;
            float currentTop = -75f + (progress * 75f);

            for (int i = sbCurrentDial; i < 3; i++)
            {
                if (sbDialStrips[i] != null)
                {
                    sbDialStrips[i].style.top = currentTop;
                }
            }

            if (sbSpinTimer >= currentSpeed)
            {
                sbSpinTimer = 0f;
                for (int i = sbCurrentDial; i < 3; i++)
                {
                    sbCurrentDialNums[i] = (sbCurrentDialNums[i] + 1) % 10;

                    if (sbDialsOut[i] != null) sbDialsOut[i].text = ((sbCurrentDialNums[i] + 2) % 10).ToString();
                    if (sbDialsTop[i] != null) sbDialsTop[i].text = ((sbCurrentDialNums[i] + 1) % 10).ToString();
                    if (sbDialsMid[i] != null) sbDialsMid[i].text = sbCurrentDialNums[i].ToString();
                    if (sbDialsBot[i] != null) sbDialsBot[i].text = ((sbCurrentDialNums[i] - 1 + 10) % 10).ToString();

                    if (sbDialStrips[i] != null) sbDialStrips[i].style.top = -75f;
                }
            }

            if (timer <= 0f)
            {
                manager.AbortGame();
                yield break;
            }
            yield return null;
        }
    }
}
