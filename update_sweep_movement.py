import re

path = 'Assets/Scripts/2 Managers/MiniGames/SweepFloorMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# Update fields
fields_target = r'''    private bool isPlaying = false;
    private bool isMopping = false;
    private Vector2 mopPos = new Vector2\(200, 200\);'''
fields_replacement = r'''    private bool isPlaying = false;
    private int playerGridX = 2;
    private int playerGridY = 2;
    private Vector2 mopVisualPos = new Vector2(200, 200);
    private Vector2 swipeStart;
    private bool isSwiping = false;'''
content = re.sub(fields_target, fields_replacement, content)


# Update input callbacks in Initialize
init_target = r'''            drawArea\.RegisterCallback<PointerDownEvent>\(ev => \{
                if \(!isPlaying\) return;
                isMopping = true;
                drawArea\.CapturePointer\(ev\.pointerId\);
                mopPos = ev\.localPosition;
                CleanAt\(mopPos\);
            \}\);
            drawArea\.RegisterCallback<PointerMoveEvent>\(ev => \{
                if \(!isPlaying \|\| !isMopping\) return;
                mopPos = ev\.localPosition;
                CleanAt\(mopPos\);
            \}\);
            drawArea\.RegisterCallback<PointerUpEvent>\(ev => \{
                isMopping = false;
                if \(drawArea\.HasPointerCapture\(ev\.pointerId\)\) drawArea\.ReleasePointer\(ev\.pointerId\);
            \}\);'''

init_replacement = r'''            drawArea.focusable = true;
            drawArea.RegisterCallback<PointerDownEvent>(ev => {
                if (!isPlaying) return;
                isSwiping = true;
                drawArea.CapturePointer(ev.pointerId);
                swipeStart = ev.position;
            });
            drawArea.RegisterCallback<PointerMoveEvent>(ev => {
                // Not needed for discrete swipe
            });
            drawArea.RegisterCallback<PointerUpEvent>(ev => {
                if (!isPlaying || !isSwiping) return;
                isSwiping = false;
                if (drawArea.HasPointerCapture(ev.pointerId)) drawArea.ReleasePointer(ev.pointerId);
                
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

# StartGame focus fix
start_target = r'''        isPlaying = true;
        container\.style\.display = DisplayStyle\.Flex;
        SetupSweepFloor\(\);
        manager\.StartCoroutine\(SweepFloorRoutine\(\)\);'''
start_replacement = r'''        isPlaying = true;
        container.style.display = DisplayStyle.Flex;
        SetupSweepFloor();
        if (drawArea != null) drawArea.Focus(); // Ensure keyboard focus
        manager.StartCoroutine(SweepFloorRoutine());'''
content = re.sub(start_target, start_replacement, content)

# Setup sweep updates
setup_target = r'''        mopPos = new Vector2\(200, 200\);
        isMopping = false;'''
setup_replacement = r'''        playerGridX = 2;
        playerGridY = 2;
        mopVisualPos = new Vector2(200, 200);
        isSwiping = false;'''
content = re.sub(setup_target, setup_replacement, content)

# Remove CleanAt
clean_target = r'''    private void CleanAt\(Vector2 pos\)
    \{
        int cx = Mathf\.Clamp\(\(int\)\(pos\.x / 80f\), 0, 4\);
        int cy = Mathf\.Clamp\(\(int\)\(pos\.y / 80f\), 0, 4\);
        sweepTileTimers\[cy \* 5 \+ cx\] = 15f;
    \}'''
clean_replacement = r'''    private void MovePlayer(int dx, int dy)
    {
        playerGridX = Mathf.Clamp(playerGridX + dx, 0, 4);
        playerGridY = Mathf.Clamp(playerGridY + dy, 0, 4);
        sweepTileTimers[playerGridY * 5 + playerGridX] = 15f;
    }'''
content = re.sub(clean_target, clean_replacement, content)


# Routine Lerping
routine_target = r'''        while \(isPlaying && manager\.isPlaying\)
        \{
            toolLabel\.style\.left = mopPos\.x - 30f;
            toolLabel\.style\.top = mopPos\.y - 30f;'''
routine_replacement = r'''        while (isPlaying && manager.isPlaying)
        {
            Vector2 targetPos = new Vector2(playerGridX * 80 + 40, playerGridY * 80 + 40);
            mopVisualPos = Vector2.Lerp(mopVisualPos, targetPos, Time.deltaTime * 20f);
            
            toolLabel.style.left = mopVisualPos.x - 30f;
            toolLabel.style.top = mopVisualPos.y - 30f;'''
content = re.sub(routine_target, routine_replacement, content)

with open(path, 'w') as f:
    f.write(content)
