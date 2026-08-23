import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target1 = r'''                if \(pumpHoldTimer >= config\.pumpBalanceHoldTime\)
                \{
                    if \(pumpHoldTimerBar != null\) pumpHoldTimerBar\.style\.width = Length\.Percent\(100f\);
                    if \(gaugeElement != null\) \{ gaugeElement\.arcColor = new Color\(0\.18f, 0\.8f, 0\.44f, 1f\); gaugeElement\.MarkDirtyRepaint\(\); \}
                    isPlaying = false;
                    manager\.MarkWinReady\(\);
                    yield break;
                \}'''

replacement1 = r'''                if (pumpHoldTimer >= config.pumpBalanceHoldTime)
                {
                    if (pumpHoldTimerBar != null) pumpHoldTimerBar.style.width = Length.Percent(100f);
                    if (gaugeElement != null) { gaugeElement.arcColor = new Color(0.18f, 0.8f, 0.44f, 1f); gaugeElement.MarkDirtyRepaint(); }
                    isPlaying = false;
                    manager.StartCoroutine(ShowWinFeedbackAndClose());
                    yield break;
                }'''
content = re.sub(target1, replacement1, content)

target2 = r'''                if \(pumpProgress >= 100f\)
                \{
                    if \(pumpProgressBar != null\) pumpProgressBar\.style\.height = Length\.Percent\(100f\);
                    isPlaying = false;
                    manager\.MarkWinReady\(\);
                    yield break;
                \}'''

replacement2 = r'''                if (pumpProgress >= 100f)
                {
                    if (pumpProgressBar != null) pumpProgressBar.style.height = Length.Percent(100f);
                    isPlaying = false;
                    manager.StartCoroutine(ShowWinFeedbackAndClose());
                    yield break;
                }'''
content = re.sub(target2, replacement2, content)


target3 = r'''    private IEnumerator PumpSystemRoutine\(\)'''
replacement3 = r'''    private IEnumerator ShowWinFeedbackAndClose()
    {
        // Flash entire container green and wait
        StyleColor originalColor = container.style.backgroundColor;
        container.style.backgroundColor = new StyleColor(new Color(0.18f, 0.8f, 0.44f, 0.5f));
        
        var successLabel = new Label("PERFECT TEMP!");
        successLabel.style.position = Position.Absolute;
        successLabel.style.top = Length.Percent(50);
        successLabel.style.width = Length.Percent(100);
        successLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        successLabel.style.fontSize = 48;
        successLabel.style.color = Color.white;
        successLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        container.Add(successLabel);

        yield return new WaitForSeconds(0.6f);
        
        container.style.backgroundColor = originalColor;
        container.Remove(successLabel);
        manager.MarkWinReady();
    }

    private IEnumerator PumpSystemRoutine()'''
content = re.sub(target3, replacement3, content)

with open(path, 'w') as f:
    f.write(content)
