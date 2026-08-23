import re

path = 'Assets/Scripts/2 Managers/MiniGames/CodeBreakerMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target = r'''    private void StyleTerminalButton\(Button btn, bool isGuessSlot\)
    \{
        btn\.style\.width = isGuessSlot \? 70 : 60;
        btn\.style\.height = isGuessSlot \? 70 : 60;
        btn\.style\.fontSize = isGuessSlot \? 40 : 30;
        btn\.style\.marginTop = 5; btn\.style\.marginBottom = 5; btn\.style\.marginLeft = 5; btn\.style\.marginRight = 5;'''

replacement = r'''    private void StyleTerminalButton(Button btn, bool isGuessSlot)
    {
        btn.style.width = isGuessSlot ? 70 : 35;
        btn.style.height = isGuessSlot ? 70 : 50;
        btn.style.fontSize = isGuessSlot ? 40 : 22;
        
        if (isGuessSlot) {
            btn.style.marginTop = 5; btn.style.marginBottom = 5; btn.style.marginLeft = 5; btn.style.marginRight = 5;
        } else {
            btn.style.marginTop = 3; btn.style.marginBottom = 3; btn.style.marginLeft = 3; btn.style.marginRight = 3;
            btn.style.paddingLeft = 0; btn.style.paddingRight = 0; btn.style.paddingTop = 0; btn.style.paddingBottom = 0;
        }'''

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)
