using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class YesManNodMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;

    private float timer;

    private VisualElement ymBossBubble;
    private Label ymBossText;
    private VisualElement ymPlayerBubble;
    private VisualElement ymSwipeZone;
    private Vector2 ymSwipeStart;
    private bool ymSwipedUp = false;
    private int ymNodPhase = 0;
    
    private string[] ymComplaints = new string[] {
        "SYNERGY IS DOWN 4%!",
        "WHO AUTHORIZED THIS BREAK!?",
        "WE NEED MORE ACTIONABLE METRICS!",
        "THE MARGINS ARE TOO THIN!",
        "WHERE IS THE PARADIGM SHIFT!?",
        "THIS IS UNACCEPTABLE!",
        "PIVOT! WE NEED TO PIVOT!"
    };

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;

        ymBossBubble = root.Q<VisualElement>("YmBossBubble");
        ymBossText = root.Q<Label>("YmBossText");
        ymPlayerBubble = root.Q<VisualElement>("YmPlayerBubble");
        ymSwipeZone = root.Q<VisualElement>("YmSwipeZone");

        if (ymSwipeZone != null) {
            ymSwipeZone.RegisterCallback<PointerDownEvent>(OnPointerDown);
            ymSwipeZone.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        }
    }

    private void OnPointerDown(PointerDownEvent ev)
    {
        if (!manager.isPlaying || config.type != GameType.YesManNod) return;
        ymSwipeStart = ev.position;
        ymNodPhase = 0;
    }

    private void OnPointerMove(PointerMoveEvent ev)
    {
        if (!manager.isPlaying || config.type != GameType.YesManNod) return;
        if (ev.pressedButtons != 0) {
            float deltaY = ev.position.y - ymSwipeStart.y;
            if (ymNodPhase == 0 && deltaY < -40f) {
                ymNodPhase = 1; // Swiped up
                ymSwipeStart = ev.position; // reset to look for downward motion
            } else if (ymNodPhase == 1 && deltaY > 40f) {
                ymNodPhase = 2; // Swiped down
                ymSwipedUp = true; // Complete nod!
            }
        }
    }

    public void StartGame()
    {
        timer = config.timeLimit;
        if (ymBossBubble != null) ymBossBubble.style.display = DisplayStyle.None;
        if (ymPlayerBubble != null) ymPlayerBubble.style.display = DisplayStyle.None;
        manager.StartCoroutine(YesManRoutine());
    }

    public void AbortGame()
    {
        if (ymBossBubble != null) ymBossBubble.style.display = DisplayStyle.None;
        if (ymPlayerBubble != null) ymPlayerBubble.style.display = DisplayStyle.None;
        
        if (ymSwipeZone != null) {
            ymSwipeZone.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            ymSwipeZone.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        }
    }

    private System.Collections.IEnumerator YesManRoutine()
    {
        int complaintsHandled = 0;
        
        while (manager.isPlaying && complaintsHandled < 5)
        {
            // Brief pause before next complaint
            float pauseTimer = Random.Range(0.2f, 0.5f);
            while (pauseTimer > 0f)
            {
                pauseTimer -= Time.deltaTime;
                timer -= Time.deltaTime;
                if (timer <= 0f) { manager.AbortGame(); yield break; }
                yield return null;
            }
            
            ymSwipedUp = false;
            if (ymBossText != null) ymBossText.text = ymComplaints[Random.Range(0, ymComplaints.Length)];
            if (ymBossBubble != null) ymBossBubble.style.display = DisplayStyle.Flex;
            if (ymPlayerBubble != null) ymPlayerBubble.style.display = DisplayStyle.None;
            
            float reactTimer = 1.8f; // increased because nodding takes longer
            bool reacted = false;
            
            while (reactTimer > 0f) {
                reactTimer -= Time.deltaTime;
                timer -= Time.deltaTime;
                
                if (ymSwipedUp) {
                    reacted = true;
                    break;
                }
                if (timer <= 0f) { manager.AbortGame(); yield break; }
                yield return null;
            }
            
            if (!reacted) {
                AudioManager.Instance?.PlayMinigameFail();
                manager.AbortGame(); // Didn't nod in time, fired.
                yield break;
            }
            
            // Successfully nodded
            AudioManager.Instance?.PlayScorePop();
            if (ymPlayerBubble != null) ymPlayerBubble.style.display = DisplayStyle.Flex;
            complaintsHandled++;
            
            float showTimer = 0.6f;
            while (showTimer > 0f)
            {
                showTimer -= Time.deltaTime;
                timer -= Time.deltaTime;
                if (timer <= 0f) { manager.AbortGame(); yield break; }
                yield return null;
            }
            if (ymBossBubble != null) ymBossBubble.style.display = DisplayStyle.None;
        }

        AudioManager.Instance?.PlayVictoryFanfare();
        manager.MarkWinReady();
    }
}
