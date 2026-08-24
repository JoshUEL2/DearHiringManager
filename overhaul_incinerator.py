import re

path = 'Assets/Scripts/2 Managers/MiniGames/IncineratorTossMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target = r'''    private float flashTimer = 0f;
    private Vector2 explosionPos;
    private float explosionTimer = 0f;'''

replacement = r'''    private float flashTimer = 0f;
    private Vector2 explosionPos;
    private float explosionTimer = 0f;
    private bool lastTossSuccess = false;'''
content = re.sub(target, replacement, content)


target_trigger = r'''    private void TriggerExplosion\(bool success\)
    \{
        explosionPos = new Vector2\(200, 80\);'''
replacement_trigger = r'''    private void TriggerExplosion(bool success)
    {
        lastTossSuccess = success;
        explosionPos = new Vector2(200, 80);'''
content = re.sub(target_trigger, replacement_trigger, content)

target_draw = r'''    private void OnDrawGame\(MeshGenerationContext ctx\)
    \{
        var p = ctx\.painter2D;
.*?
            p\.BeginPath\(\); p\.MoveTo\(new Vector2\(0,0\)\); p\.LineTo\(new Vector2\(w,0\)\); p\.LineTo\(new Vector2\(w,h\)\); p\.LineTo\(new Vector2\(0,h\)\); p\.Fill\(\);
        \}
    \}'''

replacement_draw = r'''    private void OnDrawGame(MeshGenerationContext ctx)
    {
        var p = ctx.painter2D;
        float w = ctx.visualElement.layout.width;
        float h = ctx.visualElement.layout.height;
        if (w == 0) return;

        // Factory Ambient Background
        p.fillColor = new Color(0.08f, 0.08f, 0.1f);
        p.BeginPath(); p.MoveTo(new Vector2(0,0)); p.LineTo(new Vector2(w,0)); p.LineTo(new Vector2(w,h)); p.LineTo(new Vector2(0,h)); p.Fill();

        // 1. FURNACE INTERIOR (The roaring fire)
        float furnaceLeft = 50f;
        float furnaceRight = 350f;
        float furnaceTop = 15f;
        float furnaceBottom = 110f;

        p.fillColor = new Color(0.9f, 0.3f, 0f);
        p.BeginPath(); p.MoveTo(new Vector2(furnaceLeft, furnaceTop)); p.LineTo(new Vector2(furnaceRight, furnaceTop)); 
        p.LineTo(new Vector2(furnaceRight, furnaceBottom)); p.LineTo(new Vector2(furnaceLeft, furnaceBottom)); p.Fill();
        
        p.fillColor = new Color(1f, 0.6f, 0f);
        for(int i=0; i<8; i++) {
            float fireX = 70f + i * 35f;
            float pulse = Mathf.Sin(Time.time * 15f + i) * 10f;
            p.BeginPath(); p.Arc(new Vector2(fireX, furnaceBottom + 10f), 30f + pulse, 0, 360); p.Fill();
        }
        p.fillColor = new Color(1f, 0.9f, 0.2f);
        for(int i=0; i<6; i++) {
            float fireX = 90f + i * 40f;
            float pulse = Mathf.Sin(Time.time * 20f + i * 2f) * 15f;
            p.BeginPath(); p.Arc(new Vector2(fireX, furnaceBottom + 20f), 20f + pulse, 0, 360); p.Fill();
        }

        // Floating Embers
        p.fillColor = new Color(1f, 0.8f, 0.2f, 0.8f);
        Random.State oldState = Random.state;
        Random.InitState(Mathf.FloorToInt(Time.time * 5f));
        for(int i=0; i<15; i++) {
            float ex = Random.Range(furnaceLeft + 10f, furnaceRight - 10f);
            float ey = Random.Range(furnaceTop + 10f, furnaceBottom - 10f);
            p.BeginPath(); p.Arc(new Vector2(ex, ey), Random.Range(2f, 5f), 0, 360); p.Fill();
        }
        Random.state = oldState;

        // 2. FURNACE EXTERIOR SHELL (Dark iron)
        p.fillColor = new Color(0.15f, 0.15f, 0.18f);
        p.BeginPath(); p.MoveTo(new Vector2(0, 0)); p.LineTo(new Vector2(furnaceLeft, 0)); p.LineTo(new Vector2(furnaceLeft, 130)); p.LineTo(new Vector2(0, 130)); p.Fill();
        p.BeginPath(); p.MoveTo(new Vector2(furnaceRight, 0)); p.LineTo(new Vector2(w, 0)); p.LineTo(new Vector2(w, 130)); p.LineTo(new Vector2(furnaceRight, 130)); p.Fill();
        p.BeginPath(); p.MoveTo(new Vector2(0, 0)); p.LineTo(new Vector2(w, 0)); p.LineTo(new Vector2(w, furnaceTop)); p.LineTo(new Vector2(0, furnaceTop)); p.Fill();
        p.BeginPath(); p.MoveTo(new Vector2(0, furnaceBottom)); p.LineTo(new Vector2(w, furnaceBottom)); p.LineTo(new Vector2(w, 130)); p.LineTo(new Vector2(0, 130)); p.Fill();
        
        p.fillColor = new Color(0.08f, 0.08f, 0.1f);
        for(float x = 20; x <= w; x += 40) {
            p.BeginPath(); p.Arc(new Vector2(x, 8), 4, 0, 360); p.Fill();
            p.BeginPath(); p.Arc(new Vector2(x, 122), 4, 0, 360); p.Fill();
        }

        // 3. BLAST DOORS (Heavy steel with hazard stripes)
        float doorWidth = 150f;
        float leftDoorX = 50 - (doorState * doorWidth);
        float rightDoorX = 200 + (doorState * doorWidth);

        p.fillColor = new Color(0.25f, 0.25f, 0.3f);
        p.BeginPath(); p.MoveTo(new Vector2(leftDoorX, furnaceTop)); p.LineTo(new Vector2(leftDoorX + doorWidth, furnaceTop)); 
        p.LineTo(new Vector2(leftDoorX + doorWidth, furnaceBottom)); p.LineTo(new Vector2(leftDoorX, furnaceBottom)); p.Fill();
        
        p.fillColor = new Color(0.8f, 0.7f, 0.1f);
        p.BeginPath(); p.MoveTo(new Vector2(leftDoorX + doorWidth - 20, furnaceTop)); p.LineTo(new Vector2(leftDoorX + doorWidth, furnaceTop)); 
        p.LineTo(new Vector2(leftDoorX + doorWidth, furnaceBottom)); p.LineTo(new Vector2(leftDoorX + doorWidth - 20, furnaceBottom)); p.Fill();
        
        p.strokeColor = new Color(0.1f, 0.1f, 0.1f);
        p.lineWidth = 8f;
        for (float y = furnaceTop + 5; y < furnaceBottom; y += 20) {
            p.BeginPath(); p.MoveTo(new Vector2(leftDoorX + doorWidth - 20, y)); p.LineTo(new Vector2(leftDoorX + doorWidth, y + 15)); p.Stroke();
        }

        p.fillColor = new Color(0.25f, 0.25f, 0.3f);
        p.BeginPath(); p.MoveTo(new Vector2(rightDoorX, furnaceTop)); p.LineTo(new Vector2(rightDoorX + doorWidth, furnaceTop)); 
        p.LineTo(new Vector2(rightDoorX + doorWidth, furnaceBottom)); p.LineTo(new Vector2(rightDoorX, furnaceBottom)); p.Fill();
        
        p.fillColor = new Color(0.8f, 0.7f, 0.1f);
        p.BeginPath(); p.MoveTo(new Vector2(rightDoorX, furnaceTop)); p.LineTo(new Vector2(rightDoorX + 20, furnaceTop)); 
        p.LineTo(new Vector2(rightDoorX + 20, furnaceBottom)); p.LineTo(new Vector2(rightDoorX, furnaceBottom)); p.Fill();
        
        p.strokeColor = new Color(0.1f, 0.1f, 0.1f);
        for (float y = furnaceTop + 5; y < furnaceBottom; y += 20) {
            p.BeginPath(); p.MoveTo(new Vector2(rightDoorX, y)); p.LineTo(new Vector2(rightDoorX + 20, y - 15)); p.Stroke();
        }
        
        p.strokeColor = new Color(0.05f, 0.05f, 0.05f);
        p.lineWidth = 4f;
        p.BeginPath(); p.MoveTo(new Vector2(leftDoorX + doorWidth, furnaceTop)); p.LineTo(new Vector2(leftDoorX + doorWidth, furnaceBottom)); p.Stroke();

        // 4. EXPLOSION / SPLATTER FEEDBACK
        if (explosionTimer > 0)
        {
            if (lastTossSuccess) {
                p.fillColor = new Color(1f, 0.6f, 0f, explosionTimer * 2f);
                p.BeginPath(); p.Arc(explosionPos, (0.5f - explosionTimer) * 300f, 0, 360); p.Fill();
                p.fillColor = new Color(1f, 0.9f, 0.2f, explosionTimer * 2f);
                p.BeginPath(); p.Arc(explosionPos, (0.5f - explosionTimer) * 150f, 0, 360); p.Fill();
            } else {
                p.fillColor = new Color(0.3f, 0.8f, 0.1f, explosionTimer * 2f);
                Random.InitState(42);
                for(int i=0; i<12; i++) {
                    p.BeginPath(); p.Arc(explosionPos + new Vector2(Random.Range(-50f, 50f), Random.Range(-30f, 30f)), Random.Range(10f, 25f), 0, 360); p.Fill();
                }
            }
        }

        // 5. THE TRASH BAG
        if (!isBagFlying || explosionTimer <= 0)
        {
            float bagScale = isBagFlying ? 0.7f : 1.0f;
            float bagRot = isBagFlying ? Mathf.Sin(Time.time * 20f) * 10f : 0f;

            p.fillColor = new Color(0.12f, 0.15f, 0.12f);
            p.BeginPath(); p.Arc(bagPos, 35f * bagScale, 0, 360); p.Fill();
            
            p.fillColor = new Color(0.2f, 0.25f, 0.2f);
            p.BeginPath(); p.Arc(bagPos + new Vector2(-10f * bagScale, -5f * bagScale), 20f * bagScale, 0, 360); p.Fill();
            
            p.fillColor = new Color(0.12f, 0.15f, 0.12f);
            p.BeginPath(); 
            p.MoveTo(bagPos + new Vector2(-15f * bagScale, -28f * bagScale)); 
            p.LineTo(bagPos + new Vector2(15f * bagScale, -28f * bagScale)); 
            p.LineTo(bagPos + new Vector2(0, -45f * bagScale)); 
            p.Fill();
            
            if (!isBagFlying) {
                p.fillColor = new Color(0.3f, 0.8f, 0.1f);
                float dripY = bagPos.y + 30f + (Time.time * 60f % 40f);
                if (dripY < h) {
                    p.BeginPath(); p.Arc(new Vector2(bagPos.x + 5f, dripY), 4f, 0, 360); p.Fill();
                }
            }
        }
        
        if (flashTimer > 0)
        {
            p.fillColor = new Color(1f, 0f, 0f, 0.4f);
            p.BeginPath(); p.MoveTo(new Vector2(0,0)); p.LineTo(new Vector2(w,0)); p.LineTo(new Vector2(w,h)); p.LineTo(new Vector2(0,h)); p.Fill();
        }
    }'''

content = re.sub(target_draw, replacement_draw, content, flags=re.DOTALL)

with open(path, 'w') as f:
    f.write(content)

