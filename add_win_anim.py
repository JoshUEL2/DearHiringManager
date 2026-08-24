import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target_win = r'''                if \(pumpProgress >= 100f\)
                \{
                    if \(pumpProgressBar != null\) pumpProgressBar\.style\.height = Length\.Percent\(100f\);
                    isPlaying = false;
                    manager\.StartCoroutine\(ShowWinFeedbackAndClose\(\)\);
                    yield break;
                \}'''
replacement_win = r'''                if (pumpProgress >= 100f)
                {
                    if (pumpProgressBar != null) pumpProgressBar.style.height = Length.Percent(100f);
                    isPlaying = false;
                    manager.StartCoroutine(WinAnimationRoutine());
                    manager.StartCoroutine(ShowWinFeedbackAndClose());
                    yield break;
                }'''
content = re.sub(target_win, replacement_win, content)

target_add_method = r'''    private IEnumerator ShowWinFeedbackAndClose\(\)'''
replacement_add_method = r'''    private IEnumerator WinAnimationRoutine()
    {
        while (!isPlaying && manager.isPlaying)
        {
            bubbleTime += Time.deltaTime;
            if (pumpNormalBar != null) pumpNormalBar.MarkDirtyRepaint();
            yield return null;
        }
    }

    private IEnumerator ShowWinFeedbackAndClose()'''
content = re.sub(target_add_method, replacement_add_method, content)

with open(path, 'w') as f:
    f.write(content)

