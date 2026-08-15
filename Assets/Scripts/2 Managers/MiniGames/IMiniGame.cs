using UnityEngine.UIElements;

public interface IMiniGame
{
    void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config);
    void StartGame();
    void AbortGame();
}
