import re

path = 'Assets/Scripts/2 Managers/MiniGames/ScrubPlateMiniGame.cs'

new_code = '''using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class ScrubPlateMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;
    private VisualElement drawArea;
    private Label scraperLabel;
    
    private List<Vector2> dirtParticles = new List<Vector2>();
    private List<float> dirtSizes = new List<float>();
    private int initialDirtCount = 1;
    
    private bool isScrubbing = false;

    private enum ScrubTheme { Plate, Microwave, Arrow, Grill }
    private ScrubTheme currentTheme;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("ScrubPlateContainer");
        if (container != null)
        {
            container.Clear();
            container.style.flexGrow = 1;
            container.style.minHeight = 450;
            container.style.alignItems = Align.Center;
            container.style.justifyContent = Justify.Center;
            
            drawArea = new VisualElement();
            drawArea.style.width = 400;
            drawArea.style.height = 400;
            drawArea.style.overflow = Overflow.Hidden;
            drawArea.style.borderTopLeftRadius = 20;
            drawArea.style.borderTopRightRadius = 20;
            drawArea.style.borderBottomLeftRadius = 20;
            drawArea.style.borderBottomRightRadius = 20;
            drawArea.generateVisualContent += OnDrawGame;
            container.Add(drawArea);

            scraperLabel = new Label();
            scraperLabel.style.position = Position.Absolute;
            scraperLabel.style.fontSize = 60;
            scraperLabel.style.color = Color.white;
            scraperLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            drawArea.Add(scraperLabel);
            
            drawArea.RegisterCallback<PointerDownEvent>(OnPointerDown);
            drawArea.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            drawArea.RegisterCallback<PointerUpEvent>(OnPointerUp);
            drawArea.RegisterCallback<PointerLeaveEvent>(OnPointerUp);
        }
    }

    public void StartGame()
    {
        if (container != null) container.style.display = DisplayStyle.Flex;
        SetupScrubPlate();
    }

    public void AbortGame()
    {
        if (container != null) container.style.display = DisplayStyle.None;
    }

    private void SetupScrubPlate()
    {
        dirtParticles.Clear();
        dirtSizes.Clear();
        isScrubbing = false;
        
        string gameName = manager.currentGame?.name ?? "";
        if (gameName == "Scour microwave grease") { currentTheme = ScrubTheme.Microwave; scraperLabel.text = "🧽"; }
        else if (gameName == "Wipe smog off the neon arrow") { currentTheme = ScrubTheme.Arrow; scraperLabel.text = "🧻"; }
        else if (gameName.Contains("grill")) { currentTheme = ScrubTheme.Grill; scraperLabel.text = "🔪"; } 
        else { currentTheme = ScrubTheme.Plate; scraperLabel.text = "🧽"; }

        Random.InitState(42); 
        
        if (currentTheme == ScrubTheme.Grill)
        {
            // Dense rectangular grid for massive solid carbonized grease layer
            for (int x = 10; x < 390; x += 12) {
                for (int y = 10; y < 360; y += 12) {
                    dirtParticles.Add(new Vector2(x + Random.Range(-8f, 8f), y + Random.Range(-8f, 8f)));
                    dirtSizes.Add(Random.Range(15f, 32f));
                }
            }
        }
        else if (currentTheme == ScrubTheme.Plate)
        {
            for (int i = 0; i < 400; i++) {
                Vector2 pos = new Vector2(200, 200) + Random.insideUnitCircle * 150f;
                dirtParticles.Add(pos);
                dirtSizes.Add(Random.Range(10f, 25f));
            }
        }
        else if (currentTheme == ScrubTheme.Arrow)
        {
            for (int i = 0; i < 350; i++) {
                Vector2 pos = new Vector2(Random.Range(100, 300), Random.Range(100, 300));
                dirtParticles.Add(pos);
                dirtSizes.Add(Random.Range(15f, 30f));
            }
        }
        else // Microwave
        {
            for (int i = 0; i < 400; i++) {
                Vector2 pos = new Vector2(Random.Range(50, 350), Random.Range(50, 350));
                dirtParticles.Add(pos);
                dirtSizes.Add(Random.Range(12f, 25f));
            }
        }
        
        initialDirtCount = dirtParticles.Count;
        scraperLabel.style.left = 170f;
        scraperLabel.style.top = 170f;
        if (drawArea != null) drawArea.MarkDirtyRepaint();
    }

    private void OnPointerDown(PointerDownEvent ev)
    {
        if (!manager.isPlaying) return;
        isScrubbing = true;
    }

    private void OnPointerMove(PointerMoveEvent ev)
    {
        if (!isScrubbing || !manager.isPlaying) return;
        
        Vector2 pos = ev.localPosition;
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
    }

    private void OnDrawGame(MeshGenerationContext ctx)
    {
        var p = ctx.painter2D;
        float w = ctx.visualElement.layout.width;
        float h = ctx.visualElement.layout.height;
        if (w == 0) return;

        // Draw Base Object
        if (currentTheme == ScrubTheme.Grill)
        {
            p.fillColor = new Color(0.1f, 0.1f, 0.15f);
            p.BeginPath(); p.MoveTo(new Vector2(0,0)); p.LineTo(new Vector2(w,0)); p.LineTo(new Vector2(w,h)); p.LineTo(new Vector2(0,h)); p.Fill();
            
            p.fillColor = new Color(0.7f, 0.7f, 0.75f); // Shiny Steel
            p.BeginPath(); p.MoveTo(new Vector2(10, 10)); p.LineTo(new Vector2(w-10, 10)); p.LineTo(new Vector2(w-10, h-40)); p.LineTo(new Vector2(10, h-40)); p.Fill();
            
            p.fillColor = new Color(0.05f, 0.05f, 0.05f); // Grease Gutter
            p.BeginPath(); p.MoveTo(new Vector2(10, h-35)); p.LineTo(new Vector2(w-10, h-35)); p.LineTo(new Vector2(w-10, h-10)); p.LineTo(new Vector2(10, h-10)); p.Fill();
            
            p.fillColor = new Color(0,0,0);
            p.BeginPath(); p.Arc(new Vector2(w-30, h-22), 8, 0, 360); p.Fill();
        }
        else if (currentTheme == ScrubTheme.Plate)
        {
            p.fillColor = new Color(0.2f, 0.2f, 0.2f);
            p.BeginPath(); p.MoveTo(new Vector2(0,0)); p.LineTo(new Vector2(w,0)); p.LineTo(new Vector2(w,h)); p.LineTo(new Vector2(0,h)); p.Fill();
            
            p.fillColor = new Color(0.9f, 0.9f, 0.9f);
            p.BeginPath(); p.Arc(new Vector2(200, 200), 180, 0, 360); p.Fill();
            p.fillColor = new Color(0.95f, 0.95f, 0.95f);
            p.BeginPath(); p.Arc(new Vector2(200, 200), 140, 0, 360); p.Fill();
        }
        else if (currentTheme == ScrubTheme.Arrow)
        {
            p.fillColor = new Color(0.05f, 0.05f, 0.05f);
            p.BeginPath(); p.MoveTo(new Vector2(0,0)); p.LineTo(new Vector2(w,0)); p.LineTo(new Vector2(w,h)); p.LineTo(new Vector2(0,h)); p.Fill();
            
            p.fillColor = new Color(1f, 0.2f, 0.6f);
            p.BeginPath(); 
            p.MoveTo(new Vector2(100, 150)); p.LineTo(new Vector2(250, 150)); 
            p.LineTo(new Vector2(250, 100)); p.LineTo(new Vector2(350, 200)); 
            p.LineTo(new Vector2(250, 300)); p.LineTo(new Vector2(250, 250));
            p.LineTo(new Vector2(100, 250)); p.Fill();
        }
        else // Microwave
        {
            p.fillColor = new Color(0.8f, 0.8f, 0.8f);
            p.BeginPath(); p.MoveTo(new Vector2(0,0)); p.LineTo(new Vector2(w,0)); p.LineTo(new Vector2(w,h)); p.LineTo(new Vector2(0,h)); p.Fill();
            p.fillColor = new Color(0.2f, 0.2f, 0.2f);
            p.BeginPath(); p.MoveTo(new Vector2(20, 20)); p.LineTo(new Vector2(w-20, 20)); p.LineTo(new Vector2(w-20, h-20)); p.LineTo(new Vector2(20, h-20)); p.Fill();
        }

        // Draw Dirt Particles
        Color dirtColor = Color.black;
        if (currentTheme == ScrubTheme.Plate) dirtColor = new Color(0.4f, 0.2f, 0.1f);
        else if (currentTheme == ScrubTheme.Arrow) dirtColor = new Color(0.4f, 0.4f, 0.4f, 0.8f);
        else if (currentTheme == ScrubTheme.Microwave) dirtColor = new Color(0.8f, 0.7f, 0.2f, 0.8f);
        else dirtColor = new Color(0.08f, 0.08f, 0.08f); // Pitch black carbon

        p.fillColor = dirtColor;
        for (int i = 0; i < dirtParticles.Count; i++)
        {
            p.BeginPath();
            p.Arc(dirtParticles[i], dirtSizes[i], 0, 360);
            p.Fill();
        }
    }
}
'''

with open(path, 'w') as f:
    f.write(new_code)
