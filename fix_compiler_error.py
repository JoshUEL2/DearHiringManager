import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target_init = r'''    public void Initialize\(MiniGameManager manager, VisualElement root, MiniGameConfig config\)
    \{
        this\.manager = manager;
        this\.config = config;'''
replacement_init = r'''    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        bool isToilet = config != null && config.instructions.ToLower().Contains("toilet");'''
content = re.sub(target_init, replacement_init, content)

target_remove = r'''            bool isToilet = config != null && config\.instructions\.ToLower\(\)\.Contains\("toilet"\);
            var shaft = pumpTrackArea\.Q<VisualElement>\("PumpShaft"\);'''
replacement_remove = r'''            var shaft = pumpTrackArea.Q<VisualElement>("PumpShaft");'''
content = re.sub(target_remove, replacement_remove, content)

# Fix double border drawing for pumpNormalBar if isToilet
# At line 187 it overwrites border widths.
target_border = r'''            if \(isToilet\) \{
                pumpNormalBar\.style\.borderTopWidth = 0; pumpNormalBar\.style\.borderBottomWidth = 0;
                pumpNormalBar\.style\.borderLeftWidth = 0; pumpNormalBar\.style\.borderRightWidth = 0;
            \}
            pumpNormalBar\.style\.borderTopWidth = 6; pumpNormalBar\.style\.borderBottomWidth = 6;
            pumpNormalBar\.style\.borderLeftWidth = 6; pumpNormalBar\.style\.borderRightWidth = 6;'''
replacement_border = r'''            if (isToilet) {
                pumpNormalBar.style.borderTopWidth = 0; pumpNormalBar.style.borderBottomWidth = 0;
                pumpNormalBar.style.borderLeftWidth = 0; pumpNormalBar.style.borderRightWidth = 0;
            } else {
                pumpNormalBar.style.borderTopWidth = 6; pumpNormalBar.style.borderBottomWidth = 6;
                pumpNormalBar.style.borderLeftWidth = 6; pumpNormalBar.style.borderRightWidth = 6;
            }'''
content = re.sub(target_border, replacement_border, content)

with open(path, 'w') as f:
    f.write(content)
