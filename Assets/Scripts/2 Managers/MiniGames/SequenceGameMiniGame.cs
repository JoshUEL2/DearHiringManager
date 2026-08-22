using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class SequenceGameMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private Label sequenceStatusText;
    private Button[] seqBtns = new Button[6];
    private Button btnReplayPattern;

    private List<int> currentSequence = new List<int>();
    private int sequenceInputIndex = 0;
    private bool isShowingSequence = false;
    private bool isPlaying = false;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("SequenceGameContainer");
        sequenceStatusText = container.Q<Label>("SequenceStatusText");
        btnReplayPattern = container.Q<Button>("BtnReplayPattern");

        for (int i = 0; i < 6; i++)
        {
            int idx = i;
            seqBtns[i] = container.Q<Button>($"SeqBtn{i + 1}");
            if (seqBtns[i] != null) seqBtns[i].clicked += () => OnSequenceButtonClicked(idx);
        }

        if (btnReplayPattern != null) btnReplayPattern.clicked += ReplaySequence;
    }

    public void StartGame()
    {
        isPlaying = true;
        container.style.display = DisplayStyle.Flex;
        
        int length = config.sequenceLength > 0 ? config.sequenceLength : 4;
        GenerateSequence(length);
        manager.StartCoroutine(PlaySequenceRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        container.style.display = DisplayStyle.None;
    }

    private void GenerateSequence(int length)
    {
        currentSequence.Clear();
        int activeButtons = Mathf.Min(6, length + 2);
        for (int i = 0; i < 6; i++)
        {
            if (seqBtns[i] == null) continue;
            
            if (i < activeButtons)
            {
                seqBtns[i].style.display = DisplayStyle.Flex;
                seqBtns[i].text = $"[{i + 1}]";
            }
            else
            {
                seqBtns[i].style.display = DisplayStyle.None;
            }
        }

        for (int i = 0; i < length; i++)
        {
            currentSequence.Add(Random.Range(0, activeButtons));
        }
    }

    private WaitForSeconds _wait1f = new WaitForSeconds(1f);
    private WaitForSeconds _wait04f = new WaitForSeconds(0.4f);
    private WaitForSeconds _wait02f = new WaitForSeconds(0.2f);

    private IEnumerator PlaySequenceRoutine()
    {
        isShowingSequence = true;
        sequenceInputIndex = 0;
        if (sequenceStatusText != null) sequenceStatusText.text = "Watch the pattern...";
        
        foreach (var btn in seqBtns)
        {
            if (btn != null) btn.style.backgroundColor = new StyleColor(new Color(0.12f, 0.16f, 0.23f));
        }

        yield return _wait1f;

        foreach (int index in currentSequence)
        {
            if (!isPlaying || !manager.isPlaying) yield break;
            
            if (seqBtns[index] != null) seqBtns[index].style.backgroundColor = new StyleColor(new Color(0.22f, 0.74f, 0.97f));
            yield return _wait04f;
            if (seqBtns[index] != null) seqBtns[index].style.backgroundColor = new StyleColor(new Color(0.12f, 0.16f, 0.23f));
            yield return _wait02f;
        }

        if (sequenceStatusText != null) sequenceStatusText.text = "Your Turn!";
        isShowingSequence = false;
    }

    private void ReplaySequence()
    {
        if (isPlaying && manager.isPlaying && !isShowingSequence)
        {
            manager.StartCoroutine(PlaySequenceRoutine());
        }
    }

    private void OnSequenceButtonClicked(int index)
    {
        if (!isPlaying || !manager.isPlaying || isShowingSequence) return;

        if (currentSequence[sequenceInputIndex] == index)
        {
            sequenceInputIndex++;
            if (seqBtns[index] != null) seqBtns[index].style.backgroundColor = new StyleColor(new Color(0.18f, 0.8f, 0.44f));
            manager.StartCoroutine(ResetButtonColor(seqBtns[index], new Color(0.12f, 0.16f, 0.23f)));

            if (sequenceInputIndex >= currentSequence.Count)
            {
                if (sequenceStatusText != null) sequenceStatusText.text = "Pattern Complete!";
                isPlaying = false;
                manager.MarkWinReady();
            }
        }
        else
        {
            if (manager.currentGame != null && manager.currentGame.canFail)
            {
                if (sequenceStatusText != null) sequenceStatusText.text = "Pattern Failed!";
                if (seqBtns[index] != null) seqBtns[index].style.backgroundColor = new StyleColor(new Color(0.9f, 0.3f, 0.23f));
                manager.StartCoroutine(ResetButtonColor(seqBtns[index], new Color(0.12f, 0.16f, 0.23f)));
                ReplaySequence();
            }
            else
            {
                if (seqBtns[index] != null) seqBtns[index].style.backgroundColor = new StyleColor(new Color(0.9f, 0.3f, 0.23f));
                manager.StartCoroutine(ResetButtonColor(seqBtns[index], new Color(0.12f, 0.16f, 0.23f)));
            }
        }
    }

    private IEnumerator ResetButtonColor(Button btn, Color color)
    {
        yield return new WaitForSeconds(0.2f);
        if (btn != null) btn.style.backgroundColor = new StyleColor(color);
    }
}
