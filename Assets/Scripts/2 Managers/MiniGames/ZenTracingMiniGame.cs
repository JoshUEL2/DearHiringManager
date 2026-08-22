using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class ZenTracingMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private VisualElement ztCircle;
    private VisualElement ztTarget;
    private VisualElement ztSponge;
    private VisualElement ztProgressFill;
    private Label ztProgressText;

    private float timer = 15f;
    private float ztAngle = 0f;
    private float ztTargetAngle = 0f;
    private float ztScore = 0f;
    private bool isPlaying = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("ZenTracingContainer");
        ztCircle = container.Q<VisualElement>("ZtCircle");
        ztTarget = container.Q<VisualElement>("ZtTarget");
        ztSponge = container.Q<VisualElement>("ZtSponge");
        ztProgressFill = container.Q<VisualElement>("ZtProgressFill");
        ztProgressText = container.Q<Label>("ZtProgressText");

        if (container != null)
        {
            container.RegisterCallback<PointerMoveEvent>(evt => {
                if (!isPlaying || !manager.isPlaying) return;
                if (evt.pressedButtons != 0)
                {
                    Vector2 localPos = evt.localPosition;
                    Vector2 center = new Vector2(container.layout.width / 2f, container.layout.height / 2f);
                    Vector2 dir = localPos - center;
                    ztAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                }
            });
            container.RegisterCallback<PointerDownEvent>(evt => {
                if (!isPlaying || !manager.isPlaying) return;
                Vector2 localPos = evt.localPosition;
                Vector2 center = new Vector2(container.layout.width / 2f, container.layout.height / 2f);
                Vector2 dir = localPos - center;
                ztAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            });
        }
    }

    public void StartGame()
    {
        isPlaying = true;
        ztAngle = 0f;
        ztTargetAngle = 0f;
        ztScore = 0f;
        timer = config.timeLimit > 0 ? config.timeLimit : 15f;
        
        if (ztProgressFill != null) ztProgressFill.style.width = new Length(0, LengthUnit.Percent);
        if (ztProgressText != null) ztProgressText.text = "RELAXATION: 0%";

        container.style.display = DisplayStyle.Flex;
        manager.StartCoroutine(ZenTracingRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private IEnumerator ZenTracingRoutine()
    {
        float totalTime = timer;
        while (isPlaying && manager.isPlaying)
        {
            timer -= Time.deltaTime;
            
            if (timer <= 0f)
            {
                if (ztScore >= totalTime * 0.7f)
                {
                    isPlaying = false;
                    manager.MarkWinReady();
                }
                else if (manager.currentGame != null && manager.currentGame.canFail)
                {
                    isPlaying = false;
                    manager.AbortGame();
                }
                else 
                {
                    isPlaying = false;
                    manager.MarkWinReady(); 
                }
                yield break;
            }

            ztTargetAngle += 90f * Time.deltaTime; 
            if (ztTargetAngle >= 360f) ztTargetAngle -= 360f;

            if (ztTarget != null)
            {
                float rad = ztTargetAngle * Mathf.Deg2Rad;
                ztTarget.style.left = new Length(50f + Mathf.Cos(rad) * 45f, LengthUnit.Percent);
                ztTarget.style.top = new Length(50f + Mathf.Sin(rad) * 45f, LengthUnit.Percent);
                ztTarget.style.translate = new StyleTranslate(new Translate(-22, -22, 0));
            }
            if (ztSponge != null)
            {
                float rad = ztAngle * Mathf.Deg2Rad;
                ztSponge.style.left = new Length(50f + Mathf.Cos(rad) * 45f, LengthUnit.Percent);
                ztSponge.style.top = new Length(50f + Mathf.Sin(rad) * 45f, LengthUnit.Percent);
                ztSponge.style.translate = new StyleTranslate(new Translate(-28, -28, 0));
            }

            float diff = Mathf.Abs(Mathf.DeltaAngle(ztAngle, ztTargetAngle));
            if (diff < 30f)
            {
                ztScore += Time.deltaTime;
                if (ztSponge != null) ztSponge.style.backgroundColor = new Color(0.2f, 0.8f, 0.2f, 0.8f); 
            }
            else
            {
                if (ztSponge != null) ztSponge.style.backgroundColor = new Color(0.9f, 0.2f, 0.2f, 0.8f); 
            }

            float relaxPct = Mathf.Clamp01(ztScore / (totalTime * 0.7f));
            if (ztProgressFill != null) ztProgressFill.style.width = new StyleLength(new Length(relaxPct * 100f, LengthUnit.Percent));
            if (ztProgressText != null) ztProgressText.text = $"Relaxation: {Mathf.FloorToInt(relaxPct * 100f)}%";

            yield return null;
        }
    }
}
