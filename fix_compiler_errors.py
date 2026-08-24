import re

path1 = 'Assets/Scripts/2 Managers/MiniGames/TearingBagMiniGame.cs'
with open(path1, 'r') as f:
    content1 = f.read()

target1 = r'''        p\.BeginPath\(\);
        p\.Ellipse\(new Vector2\(200, 240\), baseRadius \+ widthStretch \+ ripPulse, baseRadius \+ 40f \+ ripPulse, 0\);
        p\.Fill\(\);'''
replacement1 = r'''        p.BeginPath();
        p.Arc(new Vector2(200, 240), baseRadius + (widthStretch / 2f) + ripPulse, 0, 360);
        p.Fill();'''
content1 = re.sub(target1, replacement1, content1)

with open(path1, 'w') as f:
    f.write(content1)

path2 = 'Assets/Scripts/2 Managers/MiniGames/IncineratorTossMiniGame.cs'
with open(path2, 'r') as f:
    content2 = f.read()

target2 = r'''    private float flashTimer = 0f;
    private bool isExplosion = false;
    private Vector2 explosionPos;'''
replacement2 = r'''    private float flashTimer = 0f;
    private Vector2 explosionPos;'''
content2 = re.sub(target2, replacement2, content2)

target2b = r'''    private void TriggerExplosion\(bool success\)
    \{
        isExplosion = true;
        explosionPos = new Vector2\(200, 80\);'''
replacement2b = r'''    private void TriggerExplosion(bool success)
    {
        explosionPos = new Vector2(200, 80);'''
content2 = re.sub(target2b, replacement2b, content2)

with open(path2, 'w') as f:
    f.write(content2)

