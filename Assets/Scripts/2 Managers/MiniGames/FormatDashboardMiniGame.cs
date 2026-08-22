using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class FormatDashboardMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;
    private VisualElement formatDashboardContainer;
    
    private float timer;
    private Label fdTimerText;
    private VisualElement[] fdHandles = new VisualElement[3];
    private float[] fdValues = new float[3];
    private VisualElement fdProgressFill;
    private VisualElement[] fdBars = new VisualElement[4];
    private float fdHoldTimer = 0f;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;
        formatDashboardContainer = root.Q<VisualElement>("FormatDashboardContainer");
    }

    public void StartGame()
    {
        formatDashboardContainer.style.display = DisplayStyle.Flex;
        SetupFormatDashboard();
    }

    public void AbortGame()
    {
        if (formatDashboardContainer != null)
        {
            formatDashboardContainer.style.display = DisplayStyle.None;
        }
    }

    private void SetupFormatDashboard()
    {
        timer = config.timeLimit;
        fdHoldTimer = 0f;
        
        formatDashboardContainer.Clear();
        formatDashboardContainer.style.flexDirection = FlexDirection.Column;
        formatDashboardContainer.style.alignItems = Align.Center;
        formatDashboardContainer.style.justifyContent = Justify.FlexStart;
        formatDashboardContainer.style.paddingTop = 20;

        Label title = new Label("Format Summary Dashboards");
        title.style.fontSize = 24;
        title.style.color = Color.white;
        title.style.marginBottom = 10;
        formatDashboardContainer.Add(title);

        fdTimerText = new Label($"{timer:F1}s");
        fdTimerText.style.fontSize = 32;
        fdTimerText.style.color = Color.white;
        fdTimerText.style.marginBottom = 20;
        formatDashboardContainer.Add(fdTimerText);

        VisualElement slidersRow = new VisualElement();
        slidersRow.style.flexDirection = FlexDirection.Row;
        slidersRow.style.justifyContent = Justify.Center;
        slidersRow.style.alignItems = Align.Center;
        slidersRow.style.width = new Length(100, LengthUnit.Percent);
        slidersRow.style.height = 300;
        
        string[] labels = { "Revenue", "Growth", "Retention" };
        Color[] colors = { Color.blue, Color.yellow, Color.red };

        for(int i = 0; i < 3; i++)
        {
            fdValues[i] = 1f;

            VisualElement sliderCol = new VisualElement();
            sliderCol.style.alignItems = Align.Center;
            sliderCol.style.marginRight = 20;
            sliderCol.style.marginLeft = 20;

            Label slLabel = new Label(labels[i]);
            slLabel.style.fontSize = 20;
            slLabel.style.color = Color.white;
            slLabel.style.marginBottom = 10;
            sliderCol.Add(slLabel);

            VisualElement track = new VisualElement();
            track.style.width = 40;
            track.style.height = 200;
            track.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            track.style.borderTopLeftRadius = 20;
            track.style.borderTopRightRadius = 20;
            track.style.borderBottomLeftRadius = 20;
            track.style.borderBottomRightRadius = 20;
            track.style.overflow = Overflow.Hidden;

            // Green zone (85% to 100%) -> Top 15%
            VisualElement targetZone = new VisualElement();
            targetZone.style.position = Position.Absolute;
            targetZone.style.top = 0;
            targetZone.style.width = new Length(100, LengthUnit.Percent);
            targetZone.style.height = new Length(15, LengthUnit.Percent);
            targetZone.style.backgroundColor = new Color(0f, 1f, 0f, 0.3f);
            track.Add(targetZone);

            fdHandles[i] = new VisualElement();
            fdHandles[i].style.width = 40;
            fdHandles[i].style.height = 40;
            fdHandles[i].style.backgroundColor = colors[i];
            fdHandles[i].style.borderTopLeftRadius = 20;
            fdHandles[i].style.borderTopRightRadius = 20;
            fdHandles[i].style.borderBottomLeftRadius = 20;
            fdHandles[i].style.borderBottomRightRadius = 20;
            fdHandles[i].style.position = Position.Absolute;
            fdHandles[i].style.top = 0;
            track.Add(fdHandles[i]);

            // Interaction
            int index = i;
            track.RegisterCallback<PointerMoveEvent>(evt => {
                if (!manager.isPlaying) return;
                if(evt.pressedButtons != 0)
                {
                    float localY = evt.localPosition.y;
                    float pct = 1f - Mathf.Clamp01(localY / 200f);
                    fdValues[index] = pct;
                }
            });
            track.RegisterCallback<PointerDownEvent>(evt => {
                if (!manager.isPlaying) return;
                float localY = evt.localPosition.y;
                float pct = 1f - Mathf.Clamp01(localY / 200f);
                fdValues[index] = pct;
            });

            sliderCol.Add(track);
            slidersRow.Add(sliderCol);
        }

        formatDashboardContainer.Add(slidersRow);

        VisualElement progressContainer = new VisualElement();
        progressContainer.style.width = 300;
        progressContainer.style.height = 30;
        progressContainer.style.backgroundColor = Color.black;
        progressContainer.style.marginTop = 20;
        progressContainer.style.borderTopWidth = 2;
        progressContainer.style.borderBottomWidth = 2;
        progressContainer.style.borderLeftWidth = 2;
        progressContainer.style.borderRightWidth = 2;
        progressContainer.style.borderTopColor = Color.white;
        progressContainer.style.borderBottomColor = Color.white;
        progressContainer.style.borderLeftColor = Color.white;
        progressContainer.style.borderRightColor = Color.white;

        fdProgressFill = new VisualElement();
        fdProgressFill.style.height = new Length(100, LengthUnit.Percent);
        fdProgressFill.style.width = new Length(0, LengthUnit.Percent);
        fdProgressFill.style.backgroundColor = Color.green;
        progressContainer.Add(fdProgressFill);

        Label holdLabel = new Label("HOLD STEADY!");
        holdLabel.style.position = Position.Absolute;
        holdLabel.style.width = new Length(100, LengthUnit.Percent);
        holdLabel.style.height = new Length(100, LengthUnit.Percent);
        holdLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        holdLabel.style.fontSize = 20;
        holdLabel.style.color = Color.white;
        progressContainer.Add(holdLabel);

        formatDashboardContainer.Add(progressContainer);

        // Chart bars (decorative)
        VisualElement chartArea = new VisualElement();
        chartArea.style.flexDirection = FlexDirection.Row;
        chartArea.style.height = 100;
        chartArea.style.marginTop = 20;
        chartArea.style.alignItems = Align.FlexEnd;
        chartArea.style.justifyContent = Justify.Center;
        for(int i = 0; i < 4; i++)
        {
            fdBars[i] = new VisualElement();
            fdBars[i].style.width = 30;
            fdBars[i].style.backgroundColor = Color.gray;
            fdBars[i].style.marginLeft = 5;
            fdBars[i].style.marginRight = 5;
            chartArea.Add(fdBars[i]);
        }
        formatDashboardContainer.Add(chartArea);

        manager.StartCoroutine(FormatDashboardRoutine());
    }
    
    private IEnumerator FormatDashboardRoutine()
    {
        float gravity = 0.40f; 
        
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            
            if (fdTimerText != null) {
                fdTimerText.text = $"{timer:F1}s";
                if (timer <= 3.0f) fdTimerText.style.color = Color.red;
                else fdTimerText.style.color = Color.white;
            }

            bool allGreen = true;

            for(int i = 0; i < 3; i++)
            {
                fdValues[i] -= gravity * Time.deltaTime;
                fdValues[i] = Mathf.Clamp01(fdValues[i]);

                if (fdValues[i] < 0.85f) allGreen = false;

                if (fdHandles[i] != null) {
                    float yOffset = (1f - fdValues[i]) * 160f;
                    fdHandles[i].style.translate = new StyleTranslate(new Translate(0, yOffset, 0));
                    
                    if (fdValues[i] >= 0.85f) fdHandles[i].style.backgroundColor = Color.green;
                    else {
                        if (i == 0) fdHandles[i].style.backgroundColor = Color.blue;
                        else if (i == 1) fdHandles[i].style.backgroundColor = Color.yellow;
                        else fdHandles[i].style.backgroundColor = Color.red;
                    }
                }
            }

            float avg = (fdValues[0] + fdValues[1] + fdValues[2]) / 3f;
            for(int i = 0; i < 4; i++) {
                if (fdBars[i] != null) {
                    float randomJitter = (allGreen ? 0f : Random.Range(-0.1f, 0.1f)) * avg;
                    float baseH = 0.3f + (0.2f * i);
                    fdBars[i].style.height = new StyleLength(new Length(Mathf.Clamp01(baseH + randomJitter) * 100f, LengthUnit.Percent));
                    fdBars[i].style.backgroundColor = allGreen ? Color.green : Color.gray;
                }
            }

            if (allGreen) {
                fdHoldTimer += Time.deltaTime;
            } else {
                fdHoldTimer = Mathf.Max(0, fdHoldTimer - Time.deltaTime * 2f);
            }

            float progressPct = Mathf.Clamp01(fdHoldTimer / 2.0f); // 2 seconds to win
            if (fdProgressFill != null) fdProgressFill.style.width = new StyleLength(new Length(progressPct * 100f, LengthUnit.Percent));

            if (progressPct >= 1f) {
                if (fdTimerText != null) {
                    fdTimerText.text = "POPPIN!";
                    fdTimerText.style.color = Color.green;
                }
                yield return new WaitForSeconds(1.0f);
                manager.EndGame(true);
                yield break;
            }

            if (timer <= 0f) {
                if (fdTimerText != null) fdTimerText.text = "FAILED!";
                yield return new WaitForSeconds(1.5f);
                manager.EndGame(false);
                yield break;
            }
            
            yield return null;
        }
    }
}
