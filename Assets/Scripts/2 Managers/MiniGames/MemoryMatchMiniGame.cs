using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class MemoryMatchMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private VisualElement memoryGrid;
    private Label matchStatusText;

    private List<Button> memoryCards = new List<Button>();
    private List<string> memoryValues = new List<string>();
    private int flippedIndex1 = -1;
    private int flippedIndex2 = -1;
    private bool isCheckingMatch = false;
    private int score = 0;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("MemoryMatchContainer");
        memoryGrid = root.Q<VisualElement>("MemoryGrid");
        matchStatusText = root.Q<Label>("MatchStatusText");
    }

    public void StartGame()
    {
        container.style.display = DisplayStyle.Flex;
        SetupMemoryMatch(config.pairs);
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
    }

    private void SetupMemoryMatch(int pairs)
    {
        memoryGrid.Clear();
        memoryCards.Clear();
        memoryValues.Clear();
        score = 0;
        
        if (matchStatusText != null)
            matchStatusText.text = $"Pairs Found: 0 / {pairs}";

        List<string> icons = new List<string> { "📈", "💼", "🏢", "📊", "💰", "☕", "📱", "📝" };
        for (int i = 0; i < pairs; i++)
        {
            memoryValues.Add(icons[i % icons.Count]);
            memoryValues.Add(icons[i % icons.Count]);
        }
        
        for (int i = 0; i < memoryValues.Count; i++)
        {
            string temp = memoryValues[i];
            int randomIndex = Random.Range(i, memoryValues.Count);
            memoryValues[i] = memoryValues[randomIndex];
            memoryValues[randomIndex] = temp;
        }

        for (int i = 0; i < memoryValues.Count; i++)
        {
            int index = i;
            var btn = new Button { text = "?" };
            btn.AddToClassList("popup-button");
            btn.AddToClassList("memory-card");
            btn.clicked += () => OnMemoryCardClicked(index);
            memoryGrid.Add(btn);
            memoryCards.Add(btn);
        }
    }

    private void OnMemoryCardClicked(int index)
    {
        if (!manager.isPlaying || isCheckingMatch || memoryCards[index].text != "?") return;

        memoryCards[index].text = memoryValues[index];
        memoryCards[index].RemoveFromClassList("memory-card");
        memoryCards[index].AddToClassList("memory-card-matched");

        if (flippedIndex1 == -1)
        {
            flippedIndex1 = index;
        }
        else
        {
            flippedIndex2 = index;
            manager.StartCoroutine(CheckMemoryMatch());
        }
    }

    private IEnumerator CheckMemoryMatch()
    {
        isCheckingMatch = true;
        yield return new WaitForSeconds(0.6f);

        if (memoryValues[flippedIndex1] == memoryValues[flippedIndex2])
        {
            score++;
            if (matchStatusText != null)
                matchStatusText.text = $"Pairs Found: {score} / {config.pairs}";
            if (score >= config.pairs) manager.MarkWinReady();
        }
        else
        {
            memoryCards[flippedIndex1].text = "?";
            memoryCards[flippedIndex1].RemoveFromClassList("memory-card-matched");
            memoryCards[flippedIndex1].AddToClassList("memory-card");
            memoryCards[flippedIndex2].text = "?";
            memoryCards[flippedIndex2].RemoveFromClassList("memory-card-matched");
            memoryCards[flippedIndex2].AddToClassList("memory-card");
        }

        flippedIndex1 = -1;
        flippedIndex2 = -1;
        isCheckingMatch = false;
    }
}
