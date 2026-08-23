import re

path = 'Assets/Scripts/2 Managers/MiniGames/MashRaceMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target = r'''            if \(opponentProgress >= config\.targetScore\)
            \{
                if \(manager\.currentGame != null && manager\.currentGame\.canFail\)
                \{
                    if \(mashTimerText != null\) mashTimerText\.text = "OPPONENT WINS!";
                    isPlaying = false;
                    manager\.AbortGame\(\);
                    yield break;
                \}
                else
                \{
                    opponentProgress = config\.targetScore;
                \}
            \}'''

replacement = r'''            if (opponentProgress >= config.targetScore)
            {
                if (mashTimerText != null) mashTimerText.text = "OPPONENT WINS!";
                isPlaying = false;
                manager.AbortGame();
                yield break;
            }'''
content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)
