import re

path = 'Assets/Scripts/2 Managers/MiniGames/OrderQueueMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

target = r'yield return new WaitForSeconds\(config\.orderPreviewTime\);'
replacement = r'''if (manager.currentGame.name == "Dance with sign for tourists")
        {
            float timePerMove = config.orderPreviewTime / orderCorrectSequence.Count;
            foreach (var item in orderCorrectSequence)
            {
                if (!isPlaying) break;
                manager.StartCoroutine(DanceMoveAnimation(item));
                yield return new WaitForSeconds(timePerMove);
            }
        }
        else
        {
            yield return new WaitForSeconds(config.orderPreviewTime);
        }'''

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)

