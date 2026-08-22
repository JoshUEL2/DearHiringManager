using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class CloggedVentsMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;
    private VisualElement cvContainer;
    private VisualElement cvGrid;

    private List<VisualElement> cvVents = new List<VisualElement>();
    private float[] cvDustLevels;
    private bool[] cvIsClogged;
    private float timer;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;
        cvContainer = root.Q<VisualElement>("CloggedVentsContainer");
        if (cvContainer != null) cvGrid = cvContainer.Q<VisualElement>("VentsGrid");
    }

    public void StartGame()
    {
        if (cvContainer == null) return;
        cvContainer.style.display = DisplayStyle.Flex;
        
        cvContainer.Clear();
        cvVents.Clear();
        
        int count = config.ventCount > 0 ? config.ventCount : 6;
        cvDustLevels = new float[count];
        cvIsClogged = new bool[count];

        Label timerLabel = new Label("15.0");
        timerLabel.name = "TimerLabel";
        timerLabel.style.fontSize = 24;
        timerLabel.style.color = Color.white;
        timerLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        timerLabel.style.marginBottom = 20;
        cvContainer.Add(timerLabel);

        cvGrid = new VisualElement();
        cvGrid.name = "VentsGrid";
        cvGrid.style.flexDirection = FlexDirection.Row;
        cvGrid.style.flexWrap = Wrap.Wrap;
        cvGrid.style.justifyContent = Justify.Center;
        cvContainer.Add(cvGrid);

        for (int i = 0; i < count; i++)
        {
            VisualElement vent = new VisualElement();
            vent.style.width = 100;
            vent.style.height = 120;
            vent.style.marginTop = 10; vent.style.marginBottom = 10; vent.style.marginLeft = 10; vent.style.marginRight = 10;
            vent.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f);
            vent.style.borderTopWidth = 4; vent.style.borderBottomWidth = 4; vent.style.borderLeftWidth = 4; vent.style.borderRightWidth = 4;
            vent.style.borderTopColor = new Color(0.3f, 0.3f, 0.3f); vent.style.borderBottomColor = new Color(0.3f, 0.3f, 0.3f); vent.style.borderLeftColor = new Color(0.3f, 0.3f, 0.3f); vent.style.borderRightColor = new Color(0.3f, 0.3f, 0.3f);
            vent.style.justifyContent = Justify.FlexEnd;

            VisualElement dust = new VisualElement();
            dust.name = "DustFill";
            dust.style.width = new Length(100, LengthUnit.Percent);
            dust.style.height = new Length(0, LengthUnit.Percent);
            dust.style.backgroundColor = Color.green;
            
            vent.Add(dust);
            
            int index = i;
            vent.RegisterCallback<PointerDownEvent>(ev => {
                if (!manager.isPlaying || cvIsClogged[index]) return;
                cvDustLevels[index] = 0f;
            });

            cvVents.Add(vent);
            cvGrid.Add(vent);
        }
        timer = config.timeLimit > 0 ? config.timeLimit : 15f;
        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        if (cvContainer != null) cvContainer.style.display = DisplayStyle.None;
    }

    private IEnumerator GameRoutine()
    {
        Label timerLabel = cvContainer.Q<Label>("TimerLabel");
        
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            if (timerLabel != null)
                timerLabel.text = timer.ToString("0.0");

            if (timer <= 0f)
            {
                manager.EndGame(true);
                yield break;
            }
            
            int cloggedCount = 0;
            for (int i = 0; i < cvDustLevels.Length; i++)
            {
                if (cvIsClogged[i])
                {
                    cloggedCount++;
                    continue;
                }

                // Random dust build rate between 5% and 15% per second
                cvDustLevels[i] += UnityEngine.Random.Range(5f, 15f) * Time.deltaTime;
                
                if (cvDustLevels[i] >= 100f)
                {
                    cvDustLevels[i] = 100f;
                    cvIsClogged[i] = true;
                    cvVents[i].style.backgroundColor = Color.gray;
                    VisualElement d = cvVents[i].Q<VisualElement>("DustFill");
                    if (d != null) d.style.backgroundColor = Color.gray;
                    cvVents[i].style.borderTopColor = Color.gray; cvVents[i].style.borderBottomColor = Color.gray; cvVents[i].style.borderLeftColor = Color.gray; cvVents[i].style.borderRightColor = Color.gray;
                    cloggedCount++;
                    continue;
                }

                VisualElement dust = cvVents[i].Q<VisualElement>("DustFill");
                if (dust != null)
                {
                    dust.style.height = new Length(cvDustLevels[i], LengthUnit.Percent);
                    if (cvDustLevels[i] < 33f) dust.style.backgroundColor = Color.green;
                    else if (cvDustLevels[i] < 66f) dust.style.backgroundColor = Color.yellow;
                    else if (cvDustLevels[i] < 85f) dust.style.backgroundColor = new Color(1f, 0.5f, 0f); // Orange
                    else dust.style.backgroundColor = Color.red;
                }

                if (cvDustLevels[i] > 70f)
                {
                    float pulse = (Mathf.Sin(Time.time * 10f) + 1f) * 0.5f;
                    Color pulseColor = Color.Lerp(new Color(0.3f, 0.3f, 0.3f), Color.red, pulse);
                    cvVents[i].style.borderTopColor = pulseColor; cvVents[i].style.borderBottomColor = pulseColor; cvVents[i].style.borderLeftColor = pulseColor; cvVents[i].style.borderRightColor = pulseColor;
                }
                else
                {
                    cvVents[i].style.borderTopColor = new Color(0.3f, 0.3f, 0.3f); cvVents[i].style.borderBottomColor = new Color(0.3f, 0.3f, 0.3f); cvVents[i].style.borderLeftColor = new Color(0.3f, 0.3f, 0.3f); cvVents[i].style.borderRightColor = new Color(0.3f, 0.3f, 0.3f);
                }
            }

            if (cloggedCount >= 2)
            {
                manager.EndGame(false);
                yield break;
            }
            yield return null;
        }
    }
}
