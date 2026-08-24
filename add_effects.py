import re

path1 = 'Assets/Scripts/2 Managers/MiniGameManager.cs'
with open(path1, 'r') as f:
    content1 = f.read()

content1 = re.sub(
    r'AddConfigFull\("Scrape the burger grill", new MiniGameConfig \{ type = GameType\.ScrubPlate, targetScore = 25, instructions = "Scrape the carbonized grill!"\}\);',
    r'AddConfigFull("Scrape the burger grill", new MiniGameConfig { type = GameType.ScrubPlate, targetScore = 25, timeLimit = 10f, instructions = "Scrape the carbonized grill!"});',
    content1
)
with open(path1, 'w') as f:
    f.write(content1)


path2 = 'Assets/Scripts/2 Managers/MiniGames/ScrubPlateMiniGame.cs'
with open(path2, 'r') as f:
    content2 = f.read()

# Add spark struct and list
struct_target = r'''    private int initialDirtCount = 1;
    
    private bool isScrubbing = false;'''
struct_replacement = r'''    private int initialDirtCount = 1;
    
    private bool isScrubbing = false;
    
    private struct Spark {
        public Vector2 pos;
        public Vector2 vel;
        public float life;
    }
    private List<Spark> sparks = new List<Spark>();'''
content2 = re.sub(struct_target, struct_replacement, content2)

# StartGame coroutine
start_target = r'''    public void StartGame\(\)
    \{
        if \(container != null\) container\.style\.display = DisplayStyle\.Flex;
        SetupScrubPlate\(\);
    \}'''
start_replacement = r'''    public void StartGame()
    {
        if (container != null) container.style.display = DisplayStyle.Flex;
        SetupScrubPlate();
        manager.StartCoroutine(GameRoutine());
    }

    private System.Collections.IEnumerator GameRoutine()
    {
        while (manager != null && manager.isPlaying)
        {
            bool needsRepaint = false;
            for (int i = sparks.Count - 1; i >= 0; i--) {
                var s = sparks[i];
                s.pos += s.vel * Time.deltaTime;
                s.vel.y += 1200f * Time.deltaTime; // gravity
                s.life -= Time.deltaTime * 2.5f;
                sparks[i] = s;
                if (s.life <= 0) sparks.RemoveAt(i);
                needsRepaint = true;
            }
            if (needsRepaint && drawArea != null) drawArea.MarkDirtyRepaint();
            yield return null;
        }
    }'''
content2 = re.sub(start_target, start_replacement, content2)

# SetupScrubPlate clear sparks
setup_target = r'''        dirtParticles\.Clear\(\);
        dirtSizes\.Clear\(\);
        isScrubbing = false;'''
setup_replacement = r'''        dirtParticles.Clear();
        dirtSizes.Clear();
        sparks.Clear();
        isScrubbing = false;'''
content2 = re.sub(setup_target, setup_replacement, content2)

# OnPointerMove spawn sparks
move_target = r'''        if \(removedAny\)
        \{
            drawArea\.MarkDirtyRepaint\(\);'''
move_replacement = r'''        if (removedAny)
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
            drawArea.MarkDirtyRepaint();'''
content2 = re.sub(move_target, move_replacement, content2)

# OnDrawGame draw sparks
draw_target = r'''        p\.fillColor = dirtColor;
        for \(int i = 0; i < dirtParticles\.Count; i\+\+\)
        \{
            p\.BeginPath\(\);
            p\.Arc\(dirtParticles\[i\], dirtSizes\[i\], 0, 360\);
            p\.Fill\(\);
        \}
    \}
\}'''
draw_replacement = r'''        p.fillColor = dirtColor;
        for (int i = 0; i < dirtParticles.Count; i++)
        {
            p.BeginPath();
            p.Arc(dirtParticles[i], dirtSizes[i], 0, 360);
            p.Fill();
        }

        // Draw Sparks
        if (currentTheme == ScrubTheme.Grill)
        {
            foreach (var s in sparks)
            {
                p.fillColor = new Color(1f, 0.6f, 0f, s.life);
                p.BeginPath(); p.Arc(s.pos, 4f * s.life, 0, 360); p.Fill();
            }
        }
    }
}'''
content2 = re.sub(draw_target, draw_replacement, content2)

with open(path2, 'w') as f:
    f.write(content2)

