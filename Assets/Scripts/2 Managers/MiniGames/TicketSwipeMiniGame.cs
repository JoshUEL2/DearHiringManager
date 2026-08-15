using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class TicketSwipeMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;

    private VisualElement container;
    private Label scoreText;
    private Label timeText;
    private VisualElement cardStack;
    
    private Button btnSpam;
    private Button btnResolve;
    private Button btnEscalate;

    private VisualElement currentCard;
    private Label currentCardText;

    private int score;
    private float timeRemaining;

    private enum TicketCategory { Spam, Resolve, Escalate }

    private class Ticket
    {
        public string text;
        public TicketCategory category;
        public Ticket(string t, TicketCategory c) { text = t; category = c; }
    }

    private List<Ticket> tickets = new List<Ticket>
    {
        // SPAM
        new Ticket("URGENT: Your mailbox is full! Click here to upgrade.", TicketCategory.Spam),
        new Ticket("Claim your free $1000 gift card now!", TicketCategory.Spam),
        new Ticket("Enlarge your RAM by 500% with this one weird trick.", TicketCategory.Spam),
        new Ticket("Prince Nigerian needs your help transferring funds.", TicketCategory.Spam),
        new Ticket("Invoice #49281 attached. Please open the EXE file.", TicketCategory.Spam),
        new Ticket("You have 1 new voice message from [Unknown].", TicketCategory.Spam),
        new Ticket("SEO Optimization for your personal blog!", TicketCategory.Spam),
        new Ticket("CONGRATULATIONS! You are the 1,000,000th visitor!", TicketCategory.Spam),
        new Ticket("Meet hot singles in your server rack.", TicketCategory.Spam),
        new Ticket("Weight loss pills that actually work!", TicketCategory.Spam),
        new Ticket("Bitcoin multiplier bot - guaranteed returns!", TicketCategory.Spam),
        new Ticket("Your account has been suspended. Please login to verify.", TicketCategory.Spam),
        new Ticket("Hot stock tip: Buy $XYZ before it moons!", TicketCategory.Spam),
        new Ticket("Cheap prescription drugs shipped discreetly.", TicketCategory.Spam),
        new Ticket("Please review this document I shared with you.", TicketCategory.Spam),

        // RESOLVE (Basic user errors)
        new Ticket("I forgot my password again.", TicketCategory.Resolve),
        new Ticket("My monitor is completely black. (It was unplugged).", TicketCategory.Resolve),
        new Ticket("How do I change my email signature?", TicketCategory.Resolve),
        new Ticket("The printer says 'Load Paper'. What do I do?", TicketCategory.Resolve),
        new Ticket("I accidentally deleted my shortcut to Excel.", TicketCategory.Resolve),
        new Ticket("Mouse cursor is moving backwards (held mouse upside down).", TicketCategory.Resolve),
        new Ticket("Keyboard isn't typing (wireless keyboard out of battery).", TicketCategory.Resolve),
        new Ticket("Can you install Google Chrome for me?", TicketCategory.Resolve),
        new Ticket("My computer is running slow (has 400 tabs open).", TicketCategory.Resolve),
        new Ticket("I can't find the 'Any' key.", TicketCategory.Resolve),
        new Ticket("Need a guest Wi-Fi password for a visitor.", TicketCategory.Resolve),
        new Ticket("The coffee machine is jammed.", TicketCategory.Resolve),
        new Ticket("My chair is squeaking.", TicketCategory.Resolve),
        new Ticket("How do I create a new folder?", TicketCategory.Resolve),
        new Ticket("I need a new mousepad.", TicketCategory.Resolve),

        // ESCALATE (Critical issues)
        new Ticket("The server room is flooding with water!", TicketCategory.Escalate),
        new Ticket("All databases just got encrypted. Ransomware note on screen.", TicketCategory.Escalate),
        new Ticket("The core router is literally on fire.", TicketCategory.Escalate),
        new Ticket("Production database was accidentally dropped.", TicketCategory.Escalate),
        new Ticket("Client financial data has been leaked online.", TicketCategory.Escalate),
        new Ticket("Datacenter cooling failed. Temps at 95C.", TicketCategory.Escalate),
        new Ticket("CEO's laptop was stolen at the airport.", TicketCategory.Escalate),
        new Ticket("Main website is down and throwing 502 Bad Gateway.", TicketCategory.Escalate),
        new Ticket("We are under a massive DDoS attack.", TicketCategory.Escalate),
        new Ticket("Someone tripped on the main power cable.", TicketCategory.Escalate),
        new Ticket("The backup drives are all corrupted.", TicketCategory.Escalate),
        new Ticket("A racoon is in the wiring closet.", TicketCategory.Escalate)
    };

    private Ticket currentTicketData;
    private bool isAnimating = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;

        container = root.Q<VisualElement>("TicketSwipeContainer");
        scoreText = container.Q<Label>("TsScoreText");
        timeText = container.Q<Label>("TsTimeText");
        cardStack = container.Q<VisualElement>("TsCardStack");

        btnSpam = container.Q<Button>("BtnTsSpam");
        btnResolve = container.Q<Button>("BtnTsResolve");
        btnEscalate = container.Q<Button>("BtnTsEscalate");
    }

    public void StartGame()
    {
        score = 0;
        timeRemaining = config.timeLimit > 0 ? config.timeLimit : 15f;
        isAnimating = false;

        btnSpam.clicked += OnSpam;
        btnResolve.clicked += OnResolve;
        btnEscalate.clicked += OnEscalate;

        container.style.display = DisplayStyle.Flex;
        
        UpdateHUD();
        SpawnNewCard();
        
        manager.StartCoroutine(TimerRoutine());
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
        
        if (btnSpam != null) btnSpam.clicked -= OnSpam;
        if (btnResolve != null) btnResolve.clicked -= OnResolve;
        if (btnEscalate != null) btnEscalate.clicked -= OnEscalate;
        
        cardStack.Clear();
    }

    private void UpdateHUD()
    {
        int target = config.targetScore > 0 ? config.targetScore : 10;
        scoreText.text = $"SCORE: {score} / {target}";
        timeText.text = $"TIME: {timeRemaining:F1}";
    }

    private void SpawnNewCard()
    {
        currentTicketData = tickets[Random.Range(0, tickets.Count)];

        currentCard = new VisualElement();
        currentCard.AddToClassList("ts-card");
        
        currentCardText = new Label(currentTicketData.text);
        currentCardText.AddToClassList("ts-card-text");
        
        currentCard.Add(currentCardText);
        cardStack.Add(currentCard);
    }

    private void OnSpam() { HandleSwipe(TicketCategory.Spam); }
    private void OnResolve() { HandleSwipe(TicketCategory.Resolve); }
    private void OnEscalate() { HandleSwipe(TicketCategory.Escalate); }

    private void HandleSwipe(TicketCategory selectedCategory)
    {
        if (!manager.isPlaying || isAnimating) return;
        
        bool isCorrect = (selectedCategory == currentTicketData.category);

        if (isCorrect)
        {
            score++;
            manager.StartCoroutine(AnimateCardSwipe(currentCard, selectedCategory, true));
        }
        else
        {
            timeRemaining -= 2f; // Penalty!
            if (timeRemaining < 0) timeRemaining = 0;
            manager.StartCoroutine(AnimateCardSwipe(currentCard, selectedCategory, false));
        }

        UpdateHUD();

        int target = config.targetScore > 0 ? config.targetScore : 10;
        if (score >= target)
        {
            manager.MarkWinReady();
        }
        else if (timeRemaining > 0)
        {
            SpawnNewCard();
        }
    }

    private IEnumerator AnimateCardSwipe(VisualElement cardToAnimate, TicketCategory direction, bool isCorrect)
    {
        isAnimating = true;
        
        // Change color based on correct/wrong
        if (isCorrect)
        {
            cardToAnimate.style.backgroundColor = new StyleColor(new Color(0.86f, 0.99f, 0.86f)); // Light green
            cardToAnimate.style.borderColor = new StyleColor(new Color(0.13f, 0.77f, 0.36f)); // Green border
        }
        else
        {
            cardToAnimate.style.backgroundColor = new StyleColor(new Color(0.99f, 0.86f, 0.86f)); // Light red
            cardToAnimate.style.borderColor = new StyleColor(new Color(0.88f, 0.15f, 0.15f)); // Red border
            
            // Shake effect on text
            var label = cardToAnimate.Q<Label>();
            if (label != null) label.text = "❌ WRONG CATEGORY!\n-2 SECONDS";
        }

        // Calculate translation based on category choice
        float endX = 0f;
        float endY = 0f;
        
        if (direction == TicketCategory.Spam) endX = -500f; // Swipe left
        else if (direction == TicketCategory.Resolve) endX = 500f; // Swipe right
        else if (direction == TicketCategory.Escalate) endY = -500f; // Swipe up

        // Apply translation
        cardToAnimate.style.translate = new StyleTranslate(new Translate(new Length(endX, LengthUnit.Pixel), new Length(endY, LengthUnit.Pixel)));
        cardToAnimate.style.opacity = 0f; // Fade out

        yield return new WaitForSeconds(0.25f);
        
        if (cardToAnimate.parent != null)
        {
            cardToAnimate.parent.Remove(cardToAnimate);
        }
        
        isAnimating = false;
    }

    private IEnumerator TimerRoutine()
    {
        while (manager.isPlaying)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                UpdateHUD();
                manager.isPlaying = false; // Fail
                yield break;
            }
            UpdateHUD();
            yield return null;
        }
    }
}
