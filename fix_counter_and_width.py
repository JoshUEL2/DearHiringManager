import re

path = 'Assets/Scripts/2 Managers/MiniGames/FallingSortMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# Fix 1: Score Update
target_score = r'''                    if \(binIdx == item\.typeId\)
                    \{
                        sortScore\+\+;
                        manager\.StartCoroutine\(BinFlash\(binIdx, true\)\);
                    \}
                    else
                    \{
                        if \(manager\.currentGame != null && manager\.currentGame\.canFail\) sortScore = Mathf\.Max\(0, sortScore - 1\);
                        manager\.StartCoroutine\(BinFlash\(binIdx, false\)\);
                    \}'''

replacement_score = r'''                    if (binIdx == item.typeId)
                    {
                        sortScore++;
                        if (sortScoreText != null) sortScoreText.text = $"Sorted: {sortScore} / {config.sortTarget}";
                        manager.StartCoroutine(BinFlash(binIdx, true));
                    }
                    else
                    {
                        if (manager.currentGame != null && manager.currentGame.canFail) sortScore = Mathf.Max(0, sortScore - 1);
                        if (sortScoreText != null) sortScoreText.text = $"Sorted: {sortScore} / {config.sortTarget}";
                        manager.StartCoroutine(BinFlash(binIdx, false));
                    }'''

content = re.sub(target_score, replacement_score, content)

# Fix 2: Width Expansion
target_width = r'''        if \(isButtonSort\)
        \{'''

replacement_width = r'''        container.style.minWidth = 600; // Force popup to expand for comfortable sorting!

        if (isButtonSort)
        {'''

content = re.sub(target_width, replacement_width, content)

with open(path, 'w') as f:
    f.write(content)
