using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class WhackGridMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private VisualElement whackGrid;
    private Label whackScoreText;
    private Label whackTimerText;

    private Button[] whackCells;
    private float timer = 15f;
    private int whackScore = 0;
    private int activeWhackCell = -1;
    private bool isDecoy = false;
    private bool isPlaying = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("WhackGridContainer");
        whackGrid = container.Q<VisualElement>("WhackGrid");
        whackScoreText = container.Q<Label>("WhackScoreText");
        whackTimerText = container.Q<Label>("WhackTimerText");
    }

    public void StartGame()
    {
        isPlaying = true;
        whackScore = 0;
        activeWhackCell = -1;
        timer = config.timeLimit > 0 ? config.timeLimit : 15f;

        if (whackScoreText != null) whackScoreText.text = $"Score: 0 / {config.whackTarget}";
        
        SetupWhackGrid();
        container.style.display = DisplayStyle.Flex;
        manager.StartCoroutine(WhackGridRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private void SetupWhackGrid()
    {
        if (whackGrid == null) return;
        whackGrid.Clear();
        whackCells = new Button[9];
        for (int i = 0; i < 9; i++)
        {
            var btn = new Button();
            btn.style.width = new Length(30, LengthUnit.Percent);
            btn.style.height = new Length(30, LengthUnit.Percent);
            btn.style.marginTop = 2; btn.style.marginBottom = 2; btn.style.marginLeft = 2; btn.style.marginRight = 2;
            btn.style.fontSize = 32;
            int idx = i;
            btn.clicked += () => OnWhackCellClicked(idx);
            whackGrid.Add(btn);
            whackCells[i] = btn;
            ResetWhackBtn(btn);
        }
    }

    private void ResetWhackBtn(Button btn)
    {
        if (btn == null) return;
        btn.style.backgroundColor = new StyleColor(Color.gray);
        btn.text = "";
    }

    private void OnWhackCellClicked(int idx)
    {
        if (!isPlaying || !manager.isPlaying) return;
        if (idx == activeWhackCell)
        {
            if (isDecoy) whackScore = Mathf.Max(0, whackScore - 1);
            else whackScore++;
            
            if (whackScoreText != null) whackScoreText.text = $"Score: {whackScore} / {config.whackTarget}";
            ResetWhackBtn(whackCells[activeWhackCell]);
            activeWhackCell = -1;
            
            if (whackScore >= config.whackTarget)
            {
                isPlaying = false;
                manager.MarkWinReady();
            }
        }
    }

    private IEnumerator WhackGridRoutine()
    {
        float popTimer = 0f;
        while(isPlaying && manager.isPlaying && timer > 0 && whackScore < config.whackTarget)
        {
            timer -= Time.deltaTime;
            if (whackTimerText != null) whackTimerText.text = $"Time: {Mathf.Ceil(timer)}s";
            
            if (activeWhackCell == -1)
            {
                activeWhackCell = Random.Range(0, 9);
                isDecoy = Random.value > 0.7f;
                var btn = whackCells[activeWhackCell];
                if (btn != null)
                {
                    btn.style.backgroundColor = new StyleColor(isDecoy ? Color.red : Color.green);
                    btn.text = "!";
                }
                popTimer = Random.Range(0.8f, 1.2f);
            }
            else
            {
                popTimer -= Time.deltaTime;
                if (popTimer <= 0f)
                {
                    ResetWhackBtn(whackCells[activeWhackCell]);
                    activeWhackCell = -1;
                }
            }
            yield return null;
        }
        if (whackScore >= config.whackTarget || (manager.currentGame != null && !manager.currentGame.canFail && timer <= 0))
        {
            manager.MarkWinReady();
        }
        else if (timer <= 0 && manager.currentGame != null && manager.currentGame.canFail)
        {
            manager.AbortGame();
        }
    }
}
