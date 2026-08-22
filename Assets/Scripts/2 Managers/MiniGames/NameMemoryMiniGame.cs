using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class NameMemoryMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;

    private VisualElement nmContainer;
    private Label nmTimeLabel;
    private Label nmTableText;
    private Label nmTableTitle;
    private VisualElement[] nmSeats = new VisualElement[4];
    private VisualElement[] nmPlates = new VisualElement[4];
    private Label[] nmCards = new Label[4];
    private Label[] nmEmojis = new Label[4];

    private int nmCorrectIndex = -1;
    private bool nmIsAnsweringPhase = false;
    private int nmRoundsPlayed = 0;
    private int nmCorrectAnswers = 0;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;

        nmContainer = root.Q<VisualElement>("NameMemoryContainer");
        nmTimeLabel = nmContainer.Q<Label>("NmTimeText");
        nmTableText = nmContainer.Q<Label>("NmTableText");
        nmTableTitle = nmContainer.Q<Label>("NmTableTitle");

        for (int i = 0; i < 4; i++)
        {
            nmSeats[i] = nmContainer.Q<VisualElement>($"NmSeat{i}");
            nmPlates[i] = nmContainer.Q<VisualElement>($"NmPlate{i}");
            nmCards[i] = nmContainer.Q<Label>($"NmCard{i}");
            nmEmojis[i] = nmContainer.Q<Label>($"NmEmoji{i}");
            
            nmSeats[i].UnregisterCallback<PointerDownEvent>(OnSeatClicked);
            nmSeats[i].RegisterCallback<PointerDownEvent>(OnSeatClicked);
        }
    }

    public void StartGame()
    {
        nmContainer.style.display = DisplayStyle.Flex;
        nmRoundsPlayed = 0;
        nmCorrectAnswers = 0;
        
        manager.StartCoroutine(NameMemoryRoutineSequence());
    }

    public void AbortGame()
    {
        nmContainer.style.display = DisplayStyle.None;
        for (int i = 0; i < 4; i++)
        {
            if (nmSeats[i] != null)
            {
                nmSeats[i].UnregisterCallback<PointerDownEvent>(OnSeatClicked);
            }
        }
    }

    private void OnSeatClicked(PointerDownEvent ev)
    {
        if (!manager.isPlaying || !nmIsAnsweringPhase) return;
        VisualElement seat = ev.currentTarget as VisualElement;
        int seatIndex = -1;
        if (seat.name == "NmSeat0") seatIndex = 0;
        else if (seat.name == "NmSeat1") seatIndex = 1;
        else if (seat.name == "NmSeat2") seatIndex = 2;
        else if (seat.name == "NmSeat3") seatIndex = 3;
        
        if (seatIndex == -1) return;
        
        nmIsAnsweringPhase = false; // block further clicks

        if (seatIndex == nmCorrectIndex)
        {
            nmPlates[seatIndex].style.backgroundColor = new StyleColor(new Color(0.18f, 0.8f, 0.44f)); // Green
            nmCorrectAnswers++;
        }
        else
        {
            nmPlates[seatIndex].style.backgroundColor = new StyleColor(new Color(0.9f, 0.29f, 0.23f)); // Red
            if (nmCorrectIndex >= 0)
            {
                nmPlates[nmCorrectIndex].style.backgroundColor = new StyleColor(new Color(0.18f, 0.8f, 0.44f));
            }
            manager.EndGame(false); // Strict fail condition!
        }
    }

    private IEnumerator NameMemoryRoutineSequence()
    {
        List<int> availableTableNums = new List<int>();
        for (int i = 1; i <= 10; i++) availableTableNums.Add(i);
        
        // Shuffle table numbers
        for (int i = 0; i < availableTableNums.Count; i++)
        {
            int temp = availableTableNums[i];
            int rnd = Random.Range(i, availableTableNums.Count);
            availableTableNums[i] = availableTableNums[rnd];
            availableTableNums[rnd] = temp;
        }
        
        while (manager.isPlaying && nmRoundsPlayed < 3)
        {
            if (nmTableTitle != null) nmTableTitle.text = $"TABLE {availableTableNums[nmRoundsPlayed]}";
            
            yield return manager.StartCoroutine(PlayNameMemoryRound());
            if (!manager.isPlaying) yield break; // game ended inside round
            nmRoundsPlayed++;
        }

        if (manager.isPlaying)
        {
            manager.EndGame(nmCorrectAnswers == 3);
        }
    }

    private IEnumerator PlayNameMemoryRound()
    {
        nmIsAnsweringPhase = false;
        
        // Reset plate colors
        for (int i = 0; i < 4; i++)
        {
            nmPlates[i].style.backgroundColor = new StyleColor(new Color(0.95f, 0.96f, 0.96f)); // #F3F4F6
        }

        List<string> selectedVips = new List<string>();
        List<string> pool = new List<string>(config.vipNames);
        
        for (int i = 0; i < 4; i++) // Show 4 seats
        {
            if (pool.Count == 0) break;
            int idx = Random.Range(0, pool.Count);
            selectedVips.Add(pool[idx]);
            pool.RemoveAt(idx);
            
            // Set UI with proper formatting "Sarah from Tech" -> "Sarah (Tech)"
            string formattedName = selectedVips[i];
            if (formattedName.Contains(" from ")) {
                formattedName = formattedName.Replace(" from ", " (") + ")";
            }
            
            nmCards[i].text = formattedName;
            nmCards[i].style.display = DisplayStyle.Flex; // Show namecards
        }

        nmCorrectIndex = Random.Range(0, selectedVips.Count);
        string questionTarget = selectedVips[nmCorrectIndex]; 
        if (questionTarget.Contains(" from ")) {
            questionTarget = questionTarget.Replace(" from ", " (") + ")";
        }
        
        nmTableText.text = "MEMORIZE SEATS!";
        
        float memTimer = 4f;
        while (memTimer > 0 && manager.isPlaying)
        {
            memTimer -= Time.deltaTime;
            nmTimeLabel.text = $"TIME: {memTimer:F1}s";
            yield return null;
        }
        if (!manager.isPlaying) yield break;

        // Hide names and ask question
        for (int i = 0; i < 4; i++)
        {
            nmCards[i].style.display = DisplayStyle.None;
        }
        
        nmTableText.text = $"Where is\n{questionTarget}?";
        nmIsAnsweringPhase = true;

        // Wait for player to answer
        float answerTimer = 5f;
        while (answerTimer > 0 && nmIsAnsweringPhase && manager.isPlaying)
        {
            answerTimer -= Time.deltaTime;
            nmTimeLabel.text = $"TIME: {answerTimer:F1}s";
            yield return null;
        }
        
        if (!manager.isPlaying) yield break;

        // If they ran out of time
        if (nmIsAnsweringPhase)
        {
            nmIsAnsweringPhase = false;
            nmPlates[nmCorrectIndex].style.backgroundColor = new StyleColor(new Color(0.18f, 0.8f, 0.44f));
        }

        // Brief pause to show result
        yield return new WaitForSeconds(1.5f);
    }
}
