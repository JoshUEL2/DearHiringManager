import re

path = 'Assets/Scripts/2 Managers/MiniGames/SweepFloorMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# Add moveDir and autoMoveTimer
fields_target = r'''    private int playerGridY = 2;
    private Vector2 mopVisualPos = new Vector2\(200, 200\);'''
fields_replacement = r'''    private int playerGridY = 2;
    private Vector2 mopVisualPos = new Vector2(200, 200);
    private Vector2 currentMoveDir = Vector2.zero;
    private float autoMoveTimer = 0f;'''
content = re.sub(fields_target, fields_replacement, content)

# Update Input to set currentMoveDir
input_target = r'''            drawArea\.RegisterCallback<PointerUpEvent>\(ev => \{
                if \(!isPlaying \|\| !isSwiping\) return;
                isSwiping = false;
                if \(drawArea\.HasPointerCapture\(ev\.pointerId\)\) drawArea\.ReleasePointer\(ev\.pointerId\);
                
                // Only process once per release
                Vector2 swipeDelta = \(Vector2\)ev\.position - swipeStart;
                if \(swipeDelta\.magnitude > 20f\)
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
                \}
            \}\);
            drawArea\.RegisterCallback<KeyDownEvent>\(ev => \{
                if \(!isPlaying\) return;
                if \(ev\.keyCode == KeyCode\.UpArrow \|\| ev\.keyCode == KeyCode\.W\) MovePlayer\(0, -1\);
                if \(ev\.keyCode == KeyCode\.DownArrow \|\| ev\.keyCode == KeyCode\.S\) MovePlayer\(0, 1\);
                if \(ev\.keyCode == KeyCode\.LeftArrow \|\| ev\.keyCode == KeyCode\.A\) MovePlayer\(-1, 0\);
                if \(ev\.keyCode == KeyCode\.RightArrow \|\| ev\.keyCode == KeyCode\.D\) MovePlayer\(1, 0\);
            \}\);'''

input_replacement = r'''            drawArea.RegisterCallback<PointerUpEvent>(ev => {
                if (!isPlaying || !isSwiping) return;
                isSwiping = false;
                if (drawArea.HasPointerCapture(ev.pointerId)) drawArea.ReleasePointer(ev.pointerId);
                
                Vector2 swipeDelta = (Vector2)ev.position - swipeStart;
                if (swipeDelta.magnitude > 20f)
                {
                    if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                    {
                        currentMoveDir = swipeDelta.x > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
                    }
                    else
                    {
                        currentMoveDir = swipeDelta.y > 0 ? new Vector2(0, 1) : new Vector2(0, -1);
                    }
                    autoMoveTimer = 0f; // move immediately
                }
            });
            drawArea.RegisterCallback<KeyDownEvent>(ev => {
                if (!isPlaying) return;
                bool handled = false;
                if (ev.keyCode == KeyCode.UpArrow || ev.keyCode == KeyCode.W) { currentMoveDir = new Vector2(0, -1); handled = true; }
                if (ev.keyCode == KeyCode.DownArrow || ev.keyCode == KeyCode.S) { currentMoveDir = new Vector2(0, 1); handled = true; }
                if (ev.keyCode == KeyCode.LeftArrow || ev.keyCode == KeyCode.A) { currentMoveDir = new Vector2(-1, 0); handled = true; }
                if (ev.keyCode == KeyCode.RightArrow || ev.keyCode == KeyCode.D) { currentMoveDir = new Vector2(1, 0); handled = true; }
                
                if (handled) {
                    autoMoveTimer = 0f; // Force immediate move on key press
                    ev.StopPropagation(); // Prevent background elements from receiving this key
                }
            });'''
content = re.sub(input_target, input_replacement, content)

# Setup to reset move dir
setup_target = r'''        playerGridY = 2;
        mopVisualPos = new Vector2\(200, 200\);'''
setup_replacement = r'''        playerGridY = 2;
        mopVisualPos = new Vector2(200, 200);
        currentMoveDir = Vector2.zero;
        autoMoveTimer = 0f;'''
content = re.sub(setup_target, setup_replacement, content)

# Update routine to auto-move
routine_target = r'''    private IEnumerator SweepFloorRoutine\(\)
    \{
        while \(isPlaying && manager\.isPlaying\)
        \{
            Vector2 targetPos = new Vector2\(playerGridX \* 80 \+ 40, playerGridY \* 80 \+ 40\);'''

routine_replacement = r'''    private IEnumerator SweepFloorRoutine()
    {
        while (isPlaying && manager.isPlaying)
        {
            // Auto-move in current direction
            if (currentMoveDir != Vector2.zero)
            {
                autoMoveTimer -= Time.deltaTime;
                if (autoMoveTimer <= 0)
                {
                    MovePlayer((int)currentMoveDir.x, (int)currentMoveDir.y);
                    autoMoveTimer = 0.15f; // Auto-slide speed (lower is faster)
                }
            }

            Vector2 targetPos = new Vector2(playerGridX * 80 + 40, playerGridY * 80 + 40);'''
content = re.sub(routine_target, routine_replacement, content)

with open(path, 'w') as f:
    f.write(content)

