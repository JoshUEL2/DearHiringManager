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
    
    private int score = 0;
    private int flippedIndex1 = -1;
    private int flippedIndex2 = -1;
    private bool isCheckingMatch = false;
    private bool isPlaying = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("MemoryMatchContainer");
        memoryGrid = container.Q<VisualElement>("MemoryGrid");
        matchStatusText = container.Q<Label>("MatchStatusText");
    }

    public void StartGame()
    {
        isPlaying = true;
        isCheckingMatch = false;
        flippedIndex1 = -1;
        flippedIndex2 = -1;
        score = 0;
        
        SetupMemoryMatch(config.pairs > 0 ? config.pairs : 4);
        container.style.display = DisplayStyle.Flex;
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private void SetupMemoryMatch(int pairs)
    {
        if (memoryGrid != null) memoryGrid.Clear();
        memoryCards.Clear();
        memoryValues.Clear();
        
        if (matchStatusText != null) matchStatusText.text = $"Pairs Found: 0 / {pairs}";

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
            if (memoryGrid != null) memoryGrid.Add(btn);
            memoryCards.Add(btn);
        }
    }

    private void OnMemoryCardClicked(int index)
    {
        if (!isPlaying || !manager.isPlaying || isCheckingMatch || memoryCards[index].text != "?") return;

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
            int pairs = config.pairs > 0 ? config.pairs : 4;
            if (matchStatusText != null) matchStatusText.text = $"Pairs Found: {score} / {pairs}";
            if (score >= pairs)
            {
                isPlaying = false;
                manager.MarkWinReady();
            }
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
