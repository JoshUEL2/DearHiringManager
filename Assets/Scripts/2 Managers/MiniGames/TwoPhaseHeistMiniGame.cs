using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class TwoPhaseHeistMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;
    private VisualElement tphContainer;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;
        
        tphContainer = root.Q<VisualElement>("TwoPhaseHeistContainer");
    }

    public void StartGame()
    {
        if (tphContainer != null)
        {
            tphContainer.style.display = DisplayStyle.Flex;
            tphContainer.Clear();
            Label lbl = new Label("HEIST INITIATED\nPHASE 1: BREAK IN");
            lbl.style.color = new StyleColor(Color.white);
            lbl.style.fontSize = 24;
            lbl.style.unityTextAlign = TextAnchor.MiddleCenter;
            tphContainer.Add(lbl);
        }
        
        manager.StartCoroutine(TwoPhaseHeistFlashRoutine());
    }

    public void AbortGame()
    {
        if (tphContainer != null)
        {
            tphContainer.style.display = DisplayStyle.None;
        }
    }

    private IEnumerator TwoPhaseHeistFlashRoutine()
    {
        manager.isPlaying = true;
        float flashTimer = 1.5f;
        while (manager.isPlaying)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0f)
            {
                manager.isPlaying = false;
                manager.HideAllContainers();
                manager.SetupGameType(config.phase1Type);
                yield break;
            }
            yield return null;
        }
    }
}
