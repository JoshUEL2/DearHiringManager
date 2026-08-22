using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class RecycleBottlesMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    
    private VisualElement container;
    private VisualElement rbBag;
    private VisualElement rbItemsContainer;
    private Label rbInstructionText;
    private Button[] rbItems = new Button[6];
    private System.Action[] rbItemCallbacks = new System.Action[6];
    
    private int rbSwipes = 0;
    private int rbCollected = 0;
    private bool rbBagOpen = false;
    private float timer = 0f;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;

        container = root.Q<VisualElement>("RecycleBottlesContainer");
        if (container == null) return;
        
        rbBag = container.Q<VisualElement>("RbBag");
        rbItemsContainer = container.Q<VisualElement>("RbItemsContainer");
        rbInstructionText = container.Q<Label>("RbInstructionText");
        
        rbItems[0] = container.Q<Button>("BtnRbItem0");
        rbItems[1] = container.Q<Button>("BtnRbItem1");
        rbItems[2] = container.Q<Button>("BtnRbItem2");
        rbItems[3] = container.Q<Button>("BtnRbTrash0");
        rbItems[4] = container.Q<Button>("BtnRbTrash1");
        rbItems[5] = container.Q<Button>("BtnRbTrash2");

        for (int i = 0; i < 6; i++)
        {
            int idx = i;
            if (rbItems[i] != null)
            {
                if (rbItemCallbacks[i] != null)
                {
                    rbItems[i].clicked -= rbItemCallbacks[i];
                }
                rbItemCallbacks[i] = () => OnRbItemClicked(idx);
                rbItems[i].clicked += rbItemCallbacks[i];
            }
        }

        if (rbBag != null)
        {
            rbBag.UnregisterCallback<PointerDownEvent>(OnRbSwipe);
            rbBag.RegisterCallback<PointerDownEvent>(OnRbSwipe);
        }
    }

    public void StartGame()
    {
        if (container == null) return;
        
        container.style.display = DisplayStyle.Flex;
        timer = config.timeLimit;
        rbSwipes = 0;
        rbCollected = 0;
        rbBagOpen = false;

        if (rbBag != null)
        {
            rbBag.style.display = DisplayStyle.Flex;
            rbBag.style.backgroundColor = new StyleColor(new Color(0.06f, 0.09f, 0.15f));
        }
        if (rbItemsContainer != null) rbItemsContainer.style.display = DisplayStyle.None;
        if (rbInstructionText != null) rbInstructionText.text = "RIP OPEN THE BAG! (SWIPE 3 TIMES)";

        for (int i = 0; i < 6; i++)
        {
            if (rbItems[i] != null) rbItems[i].style.display = DisplayStyle.Flex;
        }

        manager.StartCoroutine(GameRoutine());
    }

    private void OnRbSwipe(PointerDownEvent ev)
    {
        if (!manager.isPlaying || rbBagOpen) return;
        
        rbSwipes++;
        // Visual feedback for tear
        if (rbBag != null)
        {
            if (rbSwipes == 1) rbBag.style.backgroundColor = new StyleColor(new Color(0.15f, 0.2f, 0.3f));
            if (rbSwipes == 2) rbBag.style.backgroundColor = new StyleColor(new Color(0.2f, 0.3f, 0.4f));
        }
        
        if (rbSwipes >= 3)
        {
            rbBagOpen = true;
            if (rbBag != null) rbBag.style.display = DisplayStyle.None;
            if (rbItemsContainer != null) rbItemsContainer.style.display = DisplayStyle.Flex;
            if (rbInstructionText != null) rbInstructionText.text = "COLLECT ALL 3 BOTTLES!";
        }
    }

    private void OnRbItemClicked(int idx)
    {
        if (!manager.isPlaying || !rbBagOpen) return;
        
        if (rbItems[idx].style.display == DisplayStyle.None) return;
        
        if (idx < 3)
        {
            // 0, 1, 2 are bottles
            rbItems[idx].style.display = DisplayStyle.None;
            rbCollected++;
            if (rbCollected >= 3)
            {
                manager.MarkWinReady();
            }
        }
        else
        {
            // Tapped trash!
            manager.EndGame(false);
        }
    }

    private IEnumerator GameRoutine()
    {
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                manager.EndGame(false);
                yield break;
            }
            yield return null;
        }
    }

    public void AbortGame()
    {
        if (container != null)
        {
            container.style.display = DisplayStyle.None;
        }
        
        if (rbBag != null)
        {
            rbBag.UnregisterCallback<PointerDownEvent>(OnRbSwipe);
        }
        
        for (int i = 0; i < 6; i++)
        {
            if (rbItems[i] != null && rbItemCallbacks[i] != null)
            {
                rbItems[i].clicked -= rbItemCallbacks[i];
            }
        }
    }
}
