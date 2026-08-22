import re

path = 'Assets/Scripts/2 Managers/MiniGames/SizeMatchMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# 1. Change from changingBox.clicked to workArea.RegisterCallback
target1 = r'''        changingBox.clicked \+= OnBoxClicked;'''
replacement1 = r'''        // Register on workArea so ANY tap counts, not just tapping the moving box!
        workArea.RegisterCallback<PointerDownEvent>(OnWorkAreaTapped);'''
content = re.sub(target1, replacement1, content)

# 2. Add OnWorkAreaTapped method
target2 = r'''    private void OnBoxClicked\(\)
    \{
        if \(!manager\.isPlaying \|\| isPaused\) return;'''
replacement2 = r'''    private void OnWorkAreaTapped(PointerDownEvent e)
    {
        if (!manager.isPlaying || isPaused) return;'''
content = re.sub(target2, replacement2, content)

# 3. Clean up the AbortGame deregistration
target3 = r'''    public void AbortGame\(\)
    \{
        container\.style\.display = DisplayStyle\.None;
    \}'''
replacement3 = r'''    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
        workArea.UnregisterCallback<PointerDownEvent>(OnWorkAreaTapped);
    }'''
content = re.sub(target3, replacement3, content)

with open(path, 'w') as f:
    f.write(content)

