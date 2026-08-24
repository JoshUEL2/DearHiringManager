import re

path = 'Assets/Scripts/2 Managers/MiniGames/SweepFloorMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# 1. Remove inputCooldown field
content = re.sub(r'    private float inputCooldown = 0f;\n', '', content)

# 2. Revert input callbacks in Initialize
init_target = r'''            drawArea\.RegisterCallback<PointerMoveEvent>\(ev => \{
                if \(!isPlaying \|\| !isSwiping\) return;
                
                Vector2 swipeDelta = \(Vector2\)ev\.position - swipeStart;
                if \(swipeDelta\.magnitude > 30f\)
                \{
                    if \(Mathf\.Abs\(swipeDelta\.x\) > Mathf\.Abs\(swipeDelta\.y\)\)
                    \{
                        if \(swipeDelta\.x > 0\) MovePlayer\(1, 0\);
                        else MovePlayer\(-1, 0\);
                    \}
                    else
                    \{
                        if \(swipeDelta\.y > 0\) MovePlayer\(0, 1\);
                        else MovePlayer\(0, -1\);
                    \}
                    swipeStart = ev\.position; // Continuous swipe support!
                \}
            \}\);
            drawArea\.RegisterCallback<PointerUpEvent>\(ev => \{
                if \(!isPlaying \|\| !isSwiping\) return;
                isSwiping = false;
                if \(drawArea\.HasPointerCapture\(ev\.pointerId\)\) drawArea\.ReleasePointer\(ev\.pointerId\);
            \}\);
            drawArea\.RegisterCallback<KeyDownEvent>\(ev => \{
                // Immediate response for distinct taps
                if \(!isPlaying\) return;
                if \(ev\.keyCode == KeyCode\.UpArrow \|\| ev\.keyCode == KeyCode\.W\) \{ MovePlayer\(0, -1\); inputCooldown = 0\.2f; \}
                if \(ev\.keyCode == KeyCode\.DownArrow \|\| ev\.keyCode == KeyCode\.S\) \{ MovePlayer\(0, 1\); inputCooldown = 0\.2f; \}
                if \(ev\.keyCode == KeyCode\.LeftArrow \|\| ev\.keyCode == KeyCode\.A\) \{ MovePlayer\(-1, 0\); inputCooldown = 0\.2f; \}
                if \(ev\.keyCode == KeyCode\.RightArrow \|\| ev\.keyCode == KeyCode\.D\) \{ MovePlayer\(1, 0\); inputCooldown = 0\.2f; \}
            \}\);'''

init_replacement = r'''            drawArea.RegisterCallback<PointerMoveEvent>(ev => {
                // Ignore continuous dragging
            });
            drawArea.RegisterCallback<PointerUpEvent>(ev => {
                if (!isPlaying || !isSwiping) return;
                isSwiping = false;
                if (drawArea.HasPointerCapture(ev.pointerId)) drawArea.ReleasePointer(ev.pointerId);
                
                // Only process once per release
                Vector2 swipeDelta = (Vector2)ev.position - swipeStart;
                if (swipeDelta.magnitude > 20f)
                {
                    if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                    {
                        if (swipeDelta.x > 0) MovePlayer(1, 0);
                        else MovePlayer(-1, 0);
                    }
                    else
                    {
                        if (swipeDelta.y > 0) MovePlayer(0, 1);
                        else MovePlayer(0, -1);
                    }
                }
            });
            drawArea.RegisterCallback<KeyDownEvent>(ev => {
                if (!isPlaying) return;
                if (ev.keyCode == KeyCode.UpArrow || ev.keyCode == KeyCode.W) MovePlayer(0, -1);
                if (ev.keyCode == KeyCode.DownArrow || ev.keyCode == KeyCode.S) MovePlayer(0, 1);
                if (ev.keyCode == KeyCode.LeftArrow || ev.keyCode == KeyCode.A) MovePlayer(-1, 0);
                if (ev.keyCode == KeyCode.RightArrow || ev.keyCode == KeyCode.D) MovePlayer(1, 0);
            });'''
content = re.sub(init_target, init_replacement, content)

# 3. Remove Input.GetKey logic from SweepFloorRoutine
routine_target = r'''    private IEnumerator SweepFloorRoutine\(\)
    \{
        while \(isPlaying && manager\.isPlaying\)
        \{
            // Continuous keyboard hold support \(bypasses OS repeat delay\)
            if \(inputCooldown > 0\) inputCooldown -= Time\.deltaTime;
            if \(inputCooldown <= 0\)
            \{
                int dx = 0; int dy = 0;
                if \(Input\.GetKey\(KeyCode\.UpArrow\) \|\| Input\.GetKey\(KeyCode\.W\)\) dy = -1;
                else if \(Input\.GetKey\(KeyCode\.DownArrow\) \|\| Input\.GetKey\(KeyCode\.S\)\) dy = 1;
                else if \(Input\.GetKey\(KeyCode\.LeftArrow\) \|\| Input\.GetKey\(KeyCode\.A\)\) dx = -1;
                else if \(Input\.GetKey\(KeyCode\.RightArrow\) \|\| Input\.GetKey\(KeyCode\.D\)\) dx = 1;
                
                if \(dx != 0 \|\| dy != 0\)
                \{
                    MovePlayer\(dx, dy\);
                    inputCooldown = 0\.15f; // Fast continuous movement rate
                \}
            \}

            Vector2 targetPos = new Vector2\(playerGridX \* 80 \+ 40, playerGridY \* 80 \+ 40\);'''

routine_replacement = r'''    private IEnumerator SweepFloorRoutine()
    {
        while (isPlaying && manager.isPlaying)
        {
            Vector2 targetPos = new Vector2(playerGridX * 80 + 40, playerGridY * 80 + 40);'''
content = re.sub(routine_target, routine_replacement, content)

with open(path, 'w') as f:
    f.write(content)

