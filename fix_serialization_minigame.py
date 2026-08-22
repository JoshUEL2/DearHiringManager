import re

path = 'Assets/Scripts/2 Managers/MiniGameManager_Variables.cs'
with open(path, 'r') as f:
    content = f.read()

# Add using UnityEngine.Serialization; if not there
if "using UnityEngine.Serialization;" not in content:
    content = content.replace("using UnityEngine;", "using UnityEngine;\nusing UnityEngine.Serialization;")

# Replace
replacements = {
    r'\[SerializeField\] private UIDocument _miniGameDocument;': r'[FormerlySerializedAs("miniGameDocument")] [SerializeField] private UIDocument _miniGameDocument;',
    r'\[SerializeField\] private ActiveMiniGame _currentGame;': r'[FormerlySerializedAs("currentGame")] [SerializeField] private ActiveMiniGame _currentGame;',
    r'\[SerializeField\] private bool _isPlaying = false;': r'[FormerlySerializedAs("isPlaying")] [SerializeField] private bool _isPlaying = false;',
    r'\[SerializeField\] private int _score = 0;': r'[FormerlySerializedAs("score")] [SerializeField] private int _score = 0;'
}

for k, v in replacements.items():
    content = re.sub(k, v, content)

with open(path, 'w') as f:
    f.write(content)

