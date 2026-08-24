import re

path = 'Assets/Scripts/2 Managers/MiniGames/ScrubPlateMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

# 1. Move timerLabel to container
init_target = r'''            timerLabel = new Label\(\);
            timerLabel\.style\.position = Position\.Absolute;
            timerLabel\.style\.right = 15;
            timerLabel\.style\.top = 15;
            timerLabel\.style\.fontSize = 24;
            timerLabel\.style\.color = Color\.white;
            timerLabel\.style\.unityFontStyleAndWeight = FontStyle\.Bold;
            drawArea\.Add\(timerLabel\);'''

init_replacement = r'''            timerLabel = new Label();
            timerLabel.style.position = Position.Relative;
            timerLabel.style.fontSize = 28;
            timerLabel.style.color = Color.white;
            timerLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            timerLabel.style.marginBottom = 10;
            container.Insert(0, timerLabel);'''

content = re.sub(init_target, init_replacement, content)


# 2. Extract scrubbing logic to ScrubAt and update Pointer handlers
pointer_target = r'''    private void OnPointerDown\(PointerDownEvent ev\)
    \{
        if \(!manager\.isPlaying\) return;
        isScrubbing = true;
    \}

    private void OnPointerMove\(PointerMoveEvent ev\)
    \{
        if \(!isScrubbing \|\| !manager\.isPlaying\) return;
        
        Vector2 pos = ev\.localPosition;
        scraperLabel\.style\.left = pos\.x - 30f;
        scraperLabel\.style\.top = pos\.y - 30f;

        float scrubRadius = currentTheme == ScrubTheme\.Grill \? 40f : 35f; 
        
        bool removedAny = false;
        for \(int i = dirtParticles\.Count - 1; i >= 0; i--\)
        \{
            if \(Vector2\.Distance\(pos, dirtParticles\[i\]\) < scrubRadius\)
            \{
                dirtParticles\.RemoveAt\(i\);
                dirtSizes\.RemoveAt\(i\);
                removedAny = true;
            \}
        \}
        
        if \(removedAny\)
        \{
            if \(currentTheme == ScrubTheme\.Grill\)
            \{
                for \(int j = 0; j < 3; j\+\+\) \{
                    sparks\.Add\(new Spark \{
                        pos = pos \+ new Vector2\(Random\.Range\(-15f, 15f\), Random\.Range\(-15f, 15f\)\),
                        vel = new Vector2\(Random\.Range\(-300f, 300f\), Random\.Range\(-400f, -100f\)\),
                        life = 1f
                    \}\);
                \}
            \}
            drawArea\.MarkDirtyRepaint\(\);
            if \(dirtParticles\.Count < initialDirtCount \* 0\.04f\) // 96% cleaned
            \{
                dirtParticles\.Clear\(\);
                dirtSizes\.Clear\(\);
                isScrubbing = false;
                manager\.MarkWinReady\(\);
                drawArea\.MarkDirtyRepaint\(\);
            \}
        \}
    \}

    private void OnPointerUp\(EventBase ev\)
    \{
        isScrubbing = false;
    \}'''

pointer_replacement = r'''    private void OnPointerDown(PointerDownEvent ev)
    {
        if (!manager.isPlaying) return;
        isScrubbing = true;
        drawArea.CapturePointer(ev.pointerId);
        ScrubAt(ev.localPosition);
    }

    private void OnPointerMove(PointerMoveEvent ev)
    {
        if (!isScrubbing || !manager.isPlaying) return;
        ScrubAt(ev.localPosition);
    }

    private void ScrubAt(Vector2 pos)
    {
        scraperLabel.style.left = pos.x - 30f;
        scraperLabel.style.top = pos.y - 30f;

        float scrubRadius = currentTheme == ScrubTheme.Grill ? 40f : 35f; 
        
        bool removedAny = false;
        for (int i = dirtParticles.Count - 1; i >= 0; i--)
        {
            if (Vector2.Distance(pos, dirtParticles[i]) < scrubRadius)
            {
                dirtParticles.RemoveAt(i);
                dirtSizes.RemoveAt(i);
                removedAny = true;
            }
        }
        
        if (removedAny)
        {
            if (currentTheme == ScrubTheme.Grill)
            {
                for (int j = 0; j < 3; j++) {
                    sparks.Add(new Spark {
                        pos = pos + new Vector2(Random.Range(-15f, 15f), Random.Range(-15f, 15f)),
                        vel = new Vector2(Random.Range(-300f, 300f), Random.Range(-400f, -100f)),
                        life = 1f
                    });
                }
            }
            drawArea.MarkDirtyRepaint();
            if (dirtParticles.Count < initialDirtCount * 0.04f) // 96% cleaned
            {
                dirtParticles.Clear();
                dirtSizes.Clear();
                isScrubbing = false;
                manager.MarkWinReady();
                drawArea.MarkDirtyRepaint();
            }
        }
    }

    private void OnPointerUp(EventBase ev)
    {
        isScrubbing = false;
        if (ev is PointerUpEvent pu && drawArea.HasPointerCapture(pu.pointerId)) {
            drawArea.ReleasePointer(pu.pointerId);
        }
    }'''

content = re.sub(pointer_target, pointer_replacement, content)

with open(path, 'w') as f:
    f.write(content)
