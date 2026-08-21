using UnityEngine;
using UnityEngine.UIElements;
using System;

public abstract class BaseMiniGame : MonoBehaviour
{
    protected MiniGameConfig config;
    protected ActiveMiniGame activeGame;
    protected VisualElement root;
    protected bool isPlaying;
    protected float timer;
    protected bool isWinReady;
    
    public Action<bool> OnGameEnded; // true = win, false = fail

    public virtual void Initialize(VisualElement uiRoot, ActiveMiniGame gameData, MiniGameConfig gameConfig)
    {
        root = uiRoot;
        activeGame = gameData;
        config = gameConfig;
        timer = config.timeLimit;
        isPlaying = true;
        isWinReady = false;
        
        BindUI();
    }

    protected abstract void BindUI();
    protected abstract void UnbindUI();

    protected virtual void Update()
    {
        if (!isPlaying) return;

        if (timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateTimerUI();
            
            if (timer <= 0)
            {
                timer = 0;
                OnTimeUp();
            }
        }
    }

    protected virtual void UpdateTimerUI()
    {
        // Optional override for games with specific timer UI text
    }

    protected virtual void OnTimeUp()
    {
        EndGame(isWinReady); 
    }

    protected void MarkWinReady()
    {
        isWinReady = true;
        EndGame(true); 
    }

    protected void EndGame(bool success)
    {
        if (!isPlaying) return;
        isPlaying = false;
        UnbindUI();
        OnGameEnded?.Invoke(success);
        
        if (root != null && root.parent != null)
        {
            root.parent.Remove(root);
        }
        
        Destroy(this.gameObject);
    }
}
