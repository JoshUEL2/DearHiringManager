import re

path = 'Assets/Scripts/2 Managers/MiniGames/IncineratorTossMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

# Add physics variables
vars_target = r'''    private Vector2 bagPos = new Vector2\(200, 320\);
    private bool isBagFlying = false;'''
vars_replacement = r'''    private Vector2 bagPos = new Vector2(200, 320);
    private Vector2 bagVel = Vector2.zero;
    private float bagRot = 0f;
    private float bagRotVel = 0f;
    private bool isBagFlying = false;
    private bool isDead = false;'''
content = re.sub(vars_target, vars_replacement, content)

# StartGame reset
start_target = r'''    public void StartGame\(\)
    \{
        bagsTossed = 0;
        health = 1;
        bagPos = new Vector2\(200, 320\);
        isBagFlying = false;'''
start_replacement = r'''    public void StartGame()
    {
        bagsTossed = 0;
        health = 1;
        bagPos = new Vector2(200, 320);
        bagVel = Vector2.zero;
        bagRot = 0f;
        bagRotVel = 0f;
        isBagFlying = false;
        isDead = false;'''
content = re.sub(start_target, start_replacement, content)

# Swipe physics trigger
swipe_target = r'''    private void OnPointerMove\(PointerMoveEvent ev\)
    \{
        if \(!isSwiping \|\| isBagFlying\) return;
        Vector2 delta = \(Vector2\)ev\.position - swipeStart;
        if \(delta\.y < -40f\) // Swiped up
        \{
            isBagFlying = true;
            isSwiping = false;
        \}
    \}'''
swipe_replacement = r'''    private void OnPointerMove(PointerMoveEvent ev)
    {
        if (!isSwiping || isBagFlying || isDead) return;
        Vector2 delta = (Vector2)ev.position - swipeStart;
        if (delta.y < -40f) // Swiped up
        {
            isBagFlying = true;
            bagVel = new Vector2(0, -900f); // Throw velocity!
            bagRotVel = Random.Range(-100f, 100f);
            isSwiping = false;
        }
    }'''
content = re.sub(swipe_target, swipe_replacement, content)

# Physics update in routine
routine_target = r'''            if \(isBagFlying\)
            \{
                bagPos\.y -= Time\.deltaTime \* 800f; // Fast throw

                if \(bagPos\.y < 80f\) // Hit furnace line
                \{
                    if \(doorState > 0\.7f\) // Door is open!
                    \{
                        bagsTossed\+\+;
                        TriggerExplosion\(true\);
                        if \(bagsTossed >= targetBags\)
                        \{
                            manager\.MarkWinReady\(\);
                        \}
                    \}
                    else
                    \{
                        health--;
                        TriggerExplosion\(false\);
                        flashTimer = 0\.2f;
                        if \(health <= 0\) manager\.AbortGame\(\);
                    \}
                    
                    isBagFlying = false;
                    bagPos = new Vector2\(200, 320\);
                \}
            \}
            
            if \(flashTimer > 0\) flashTimer -= Time\.deltaTime;
            if \(explosionTimer > 0\) explosionTimer -= Time\.deltaTime;

            if \(!isBagFlying \|\| explosionTimer <= 0\) \{
                bagLabel\.style\.display = DisplayStyle\.Flex;
                bagLabel\.style\.left = bagPos\.x - 25f;
                bagLabel\.style\.top = bagPos\.y - 25f;
                if \(isBagFlying\) \{
                    bagLabel\.style\.scale = new StyleScale\(new Scale\(new Vector2\(0\.6f, 0\.6f\)\)\);
                \} else \{
                    bagLabel\.style\.scale = new StyleScale\(new Scale\(new Vector2\(1f, 1f\)\)\);
                \}
            \} else \{
                bagLabel\.style\.display = DisplayStyle\.None;
            \}'''

routine_replacement = r'''            if (isBagFlying || isDead)
            {
                bagPos += bagVel * Time.deltaTime;
                bagVel.y += 1800f * Time.deltaTime; // Gravity pulling it down!
                bagRot += bagRotVel * Time.deltaTime; // Spin!

                // Hit furnace line check
                if (bagPos.y < 110f && bagVel.y < 0 && !isDead)
                {
                    if (doorState > 0.7f) // Success!
                    {
                        bagsTossed++;
                        TriggerExplosion(true);
                        if (bagsTossed >= targetBags) manager.MarkWinReady();
                        
                        isBagFlying = false;
                        bagPos = new Vector2(200, 320);
                        bagVel = Vector2.zero;
                        bagRot = 0f;
                    }
                    else // Hit closed door - DEATH!
                    {
                        health--;
                        TriggerExplosion(false);
                        flashTimer = 0.2f;
                        isDead = true; // Trigger death animation
                        bagVel = new Vector2(Random.Range(-300f, 300f), 300f); // Bounce off the doors!
                        bagRotVel = Random.Range(-720f, 720f); // Violent spin
                    }
                }
                
                // If it falls off screen while dead
                if (isDead && bagPos.y > 500f) {
                    manager.AbortGame();
                }
                
                // If missed throw entirely (didn't reach) and fell off
                if (!isDead && isBagFlying && bagPos.y > 500f) {
                    bagPos = new Vector2(200, 320);
                    bagVel = Vector2.zero;
                    bagRot = 0f;
                    isBagFlying = false;
                }
            }
            
            if (flashTimer > 0) flashTimer -= Time.deltaTime;
            if (explosionTimer > 0) explosionTimer -= Time.deltaTime;

            if (!isBagFlying || isDead) {
                bagLabel.style.display = DisplayStyle.Flex;
                bagLabel.style.left = bagPos.x - 25f;
                bagLabel.style.top = bagPos.y - 25f;
                bagLabel.style.rotate = new StyleRotate(new Rotate(new Angle(bagRot, AngleUnit.Degree)));
                bagLabel.style.scale = new StyleScale(new Scale(new Vector2(1f, 1f)));
            } else if (isBagFlying && explosionTimer <= 0) {
                bagLabel.style.display = DisplayStyle.Flex;
                bagLabel.style.left = bagPos.x - 25f;
                bagLabel.style.top = bagPos.y - 25f;
                bagLabel.style.rotate = new StyleRotate(new Rotate(new Angle(bagRot, AngleUnit.Degree)));
                float distScale = Mathf.Clamp(bagPos.y / 320f, 0.4f, 1f); // Shrink as it goes up!
                bagLabel.style.scale = new StyleScale(new Scale(new Vector2(distScale, distScale)));
            } else {
                bagLabel.style.display = DisplayStyle.None;
            }'''
content = re.sub(routine_target, routine_replacement, content, flags=re.DOTALL)

# Shell bottom lip extension
lip_target = r'''        p\.BeginPath\(\); p\.MoveTo\(new Vector2\(0, furnaceBottom\)\); p\.LineTo\(new Vector2\(w, furnaceBottom\)\); p\.LineTo\(new Vector2\(w, 160\)\); p\.LineTo\(new Vector2\(0, 160\)\); p\.Fill\(\);
        
        p\.fillColor = new Color\(0\.08f, 0\.08f, 0\.1f\);
        for\(float x = 20; x <= w; x \+= 40\) \{
            p\.BeginPath\(\); p\.Arc\(new Vector2\(x, 8\), 4, 0, 360\); p\.Fill\(\);
            p\.BeginPath\(\); p\.Arc\(new Vector2\(x, 122\), 4, 0, 360\); p\.Fill\(\);
            p\.BeginPath\(\); p\.Arc\(new Vector2\(x, 152\), 4, 0, 360\); p\.Fill\(\);
        \}'''

lip_replacement = r'''        p.BeginPath(); p.MoveTo(new Vector2(0, furnaceBottom)); p.LineTo(new Vector2(w, furnaceBottom)); p.LineTo(new Vector2(w, 200)); p.LineTo(new Vector2(0, 200)); p.Fill();
        
        p.fillColor = new Color(0.08f, 0.08f, 0.1f);
        for(float x = 20; x <= w; x += 40) {
            p.BeginPath(); p.Arc(new Vector2(x, 8), 4, 0, 360); p.Fill();
            p.BeginPath(); p.Arc(new Vector2(x, 122), 4, 0, 360); p.Fill();
            p.BeginPath(); p.Arc(new Vector2(x, 182), 4, 0, 360); p.Fill();
        }'''
content = re.sub(lip_target, lip_replacement, content)

with open(path, 'w') as f:
    f.write(content)

