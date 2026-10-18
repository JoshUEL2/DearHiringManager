using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class IceShatterMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;
    private VisualElement container;

    private VisualElement isFrostOverlay;
    private VisualElement isTempFill;
    private VisualElement isNodeArea;
    private Label isTimerText;
    
    private List<Button> isNodes = new List<Button>();
    private float isBodyTemp = 100f;
    private float timer;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;

        container = root.Q<VisualElement>("IceShatterContainer");
        
        isFrostOverlay = root.Q<VisualElement>("IsFrostOverlay") ?? root.Q<VisualElement>("isFrostOverlay");
        isTempFill = root.Q<VisualElement>("IsTempFill") ?? root.Q<VisualElement>("isTempFill");
        isNodeArea = root.Q<VisualElement>("IsNodeArea") ?? root.Q<VisualElement>("isNodeArea");
        isTimerText = root.Q<Label>("IsTimerText") ?? root.Q<Label>("isTimerText");
    }

    public void StartGame()
    {
        container.style.display = DisplayStyle.Flex;

        isBodyTemp = 100f;
        timer = config.timeLimit > 0 ? config.timeLimit : 15f;
        
        isNodes.Clear();
        isNodeArea.Clear();

        if (isFrostOverlay != null)
            isFrostOverlay.style.backgroundColor = new Color(1f, 1f, 1f, 0f);

        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
    }

    private IEnumerator GameRoutine()
    {
        float spawnTimer = 0f;
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            
            if (isTimerText != null)
                isTimerText.text = $"TIME: {Mathf.Max(0, timer):F1}s";
                
            if (timer <= 0f)
            {
                manager.MarkWinReady(); 
                yield break;
            }

            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                int spawnCount = Random.Range(2, 4);
                for (int i = 0; i < spawnCount; i++)
                {
                    Button node = new Button();
                    node.style.width = 60;
                    node.style.height = 60;
                    node.style.position = Position.Absolute;
                    node.style.left = new Length(Random.Range(10f, 85f), LengthUnit.Percent);
                    node.style.top = new Length(Random.Range(15f, 75f), LengthUnit.Percent);
                    
                    node.style.backgroundColor = new Color(0.2f, 0.8f, 1f, 0.9f);
                    node.style.borderTopLeftRadius = 30; node.style.borderTopRightRadius = 30; node.style.borderBottomLeftRadius = 30; node.style.borderBottomRightRadius = 30;
                    node.style.borderTopWidth = 4; node.style.borderBottomWidth = 4; node.style.borderLeftWidth = 4; node.style.borderRightWidth = 4;
                    node.style.borderTopColor = new Color(0.8f, 0.9f, 1f); 
                    node.style.borderBottomColor = new Color(0.8f, 0.9f, 1f);
                    node.style.borderLeftColor = new Color(0.8f, 0.9f, 1f);
                    node.style.borderRightColor = new Color(0.8f, 0.9f, 1f);
                    
                    node.text = "❄️";
                    node.style.fontSize = 24;
                    node.style.color = Color.white;
                    node.style.unityTextAlign = TextAnchor.MiddleCenter;
                    
                    node.clicked += () => {
                        if (!manager.isPlaying) return;
                        isBodyTemp = Mathf.Clamp(isBodyTemp + 12f, 0f, 100f);
                        AudioManager.Instance?.PlayIceShatter();
                        MiniGameFeedback.PlaySuccess(node, "SHATTER! ❄️", container);
                        node.style.backgroundColor = Color.white;
                        node.text = ""; 
                        node.style.scale = new StyleScale(new Scale(new Vector3(0.3f, 0.3f, 1f)));
                        isNodes.Remove(node);
                        
                        isNodeArea.schedule.Execute(() => {
                            if (isNodeArea.Contains(node)) isNodeArea.Remove(node);
                        }).StartingIn(100);
                    };

                    isNodes.Add(node);
                    isNodeArea.Add(node);
                }
                
                float minTime = Mathf.Lerp(1.5f, 0.3f, 1f - (timer / 15f));
                float maxTime = Mathf.Lerp(2.0f, 0.6f, 1f - (timer / 15f));
                spawnTimer = Random.Range(minTime, maxTime);
            }

            isBodyTemp -= (3f + isNodes.Count * 3f) * Time.deltaTime;
            
            if (isFrostOverlay != null)
            {
                float freezePercent = 1f - (isBodyTemp / 100f);
                isFrostOverlay.style.backgroundColor = new Color(1f, 1f, 1f, freezePercent * 0.8f);
            }

            if (isTempFill != null)
            {
                isTempFill.style.width = new Length(isBodyTemp, LengthUnit.Percent);
                if (isBodyTemp > 60f) isTempFill.style.backgroundColor = Color.green;
                else if (isBodyTemp > 30f) isTempFill.style.backgroundColor = Color.yellow;
                else isTempFill.style.backgroundColor = Color.red;
            }

            if (isBodyTemp <= 0f)
            {
                manager.EndGame(false);
                yield break;
            }

            yield return null;
        }
    }
}
