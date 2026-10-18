using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class HotStoneSpaMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;
    private VisualElement container;

    private Label timerText;
    private VisualElement[] slots = new VisualElement[5];
    private VisualElement[] stones = new VisualElement[5];
    private bool[] stonePlaced = new bool[5];
    private int activeStoneIndex = -1;
    private Vector2 pointerStartPos;
    private Vector2 stoneStartPos;
    private float timer;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;
        
        container = root.Q<VisualElement>("HotStoneContainer");
        timerText = root.Q<Label>("HsTimerText");

        for (int i = 0; i < 5; i++)
        {
            slots[i] = root.Q<VisualElement>($"HsSlot{i}");
            stones[i] = root.Q<VisualElement>($"HsStone{i}");
            
            int stoneIdx = i;
            if (stones[i] != null)
            {
                stones[i].RegisterCallback<PointerDownEvent>(ev => OnPointerDown(ev, stoneIdx));
                stones[i].RegisterCallback<PointerMoveEvent>(ev => OnPointerMove(ev, stoneIdx));
                stones[i].RegisterCallback<PointerUpEvent>(ev => OnPointerUp(ev, stoneIdx));
                stones[i].RegisterCallback<PointerCaptureOutEvent>(ev => OnPointerUp(ev, stoneIdx));
            }
        }
    }

    public void StartGame()
    {
        if (container != null) container.style.display = DisplayStyle.Flex;
        timer = config.timeLimit;
        activeStoneIndex = -1;

        for (int i = 0; i < 5; i++)
        {
            stonePlaced[i] = false;
            if (stones[i] != null)
            {
                stones[i].style.backgroundColor = new StyleColor(new Color(0.18f, 0.31f, 0.31f)); // #2F4F4F
                stones[i].style.left = StyleKeyword.Auto;
                stones[i].style.top = StyleKeyword.Auto;
                stones[i].style.right = StyleKeyword.Auto;
                
                stones[i].style.bottom = 20f + Random.Range(-10f, 10f);
                float lx = 40f + (i * 60f) + Random.Range(-10f, 10f);
                stones[i].style.left = lx;
            }
        }

        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        if (container != null) container.style.display = DisplayStyle.None;
    }

    private IEnumerator GameRoutine()
    {
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            if (timerText != null) timerText.text = $"{Mathf.Max(0, timer):F1}s";
            
            if (timer <= 0f)
            {
                manager.AbortGame();
                yield break;
            }
            yield return null;
        }
    }

    private void OnPointerDown(PointerDownEvent ev, int stoneIdx)
    {
        if (!manager.isPlaying || stonePlaced[stoneIdx]) return;
        activeStoneIndex = stoneIdx;
        pointerStartPos = ev.position;
        float currentLeft = stones[stoneIdx].resolvedStyle.left;
        float currentTop = stones[stoneIdx].resolvedStyle.top;
        if (float.IsNaN(currentLeft)) currentLeft = 0;
        if (float.IsNaN(currentTop)) currentTop = 0;
        
        stoneStartPos = new Vector2(currentLeft, currentTop);
        stones[stoneIdx].CapturePointer(ev.pointerId);
    }

    private void OnPointerMove(PointerMoveEvent ev, int stoneIdx)
    {
        if (!manager.isPlaying || activeStoneIndex != stoneIdx || stonePlaced[stoneIdx]) return;
        
        Vector2 delta = (Vector2)ev.position - pointerStartPos;
        float newLeft = stoneStartPos.x + delta.x;
        float newTop = stoneStartPos.y + delta.y;
        
        stones[stoneIdx].style.left = newLeft;
        stones[stoneIdx].style.top = newTop;
        stones[stoneIdx].style.bottom = StyleKeyword.Auto;
        stones[stoneIdx].style.right = StyleKeyword.Auto;
    }

    private void OnPointerUp(EventBase ev, int stoneIdx)
    {
        if (activeStoneIndex != stoneIdx) return;
        activeStoneIndex = -1;
        
        if (ev is IPointerEvent pointerEvent) {
            stones[stoneIdx].ReleasePointer(pointerEvent.pointerId);
        }
        
        if (stonePlaced[stoneIdx]) return;

        Vector2 sCenter = stones[stoneIdx].worldBound.center;
        
        for (int i = 0; i < 5; i++) {
            if (slots[i] == null) continue;
            
            Vector2 targetCenter = slots[i].worldBound.center;
            
            if (Vector2.Distance(sCenter, targetCenter) < 50f) {
                stonePlaced[stoneIdx] = true;
                stones[stoneIdx].style.backgroundColor = new StyleColor(new Color(0.82f, 0.33f, 0f)); // #D35400
                AudioManager.Instance?.PlayActivityWellness("stone");
                MiniGameFeedback.PlaySuccess(stones[stoneIdx], "SOOTHING! ♨️", container);
                // Center the stone roughly in the slot
                stones[stoneIdx].style.left = stones[stoneIdx].resolvedStyle.left + (targetCenter.x - sCenter.x);
                stones[stoneIdx].style.top = stones[stoneIdx].resolvedStyle.top + (targetCenter.y - sCenter.y);
                
                CheckWin();
                return;
            }
        }
    }

    private void CheckWin()
    {
        bool allPlaced = true;
        for (int i = 0; i < 5; i++) {
            if (!stonePlaced[i]) {
                allPlaced = false;
                break;
            }
        }
        if (allPlaced) {
            manager.MarkWinReady();
        }
    }
}
