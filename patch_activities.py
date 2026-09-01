import re

with open('Assets/Scripts/3 UI/ActivitiesUI.cs', 'r') as f:
    content = f.read()

# Add animation to Walk in the Park (move trees left)
content = content.replace(
    '''VisualElement tree = new VisualElement();''',
    '''VisualElement tree = new VisualElement(); tree.userData = (float)Random.Range(70, 100);'''
)
content = content.replace(
    '''tree.style.left = Length.Percent(70 + (i*5));''',
    '''tree.style.left = Length.Percent((float)tree.userData);'''
)

# Replace the whole switch case to add scheduling
new_switch = '''switch (name)
        {
            case "Walk in the Park":
                bg.style.backgroundColor = new StyleColor(new Color(0.1f, 0.3f, 0.1f));
                var trees = new List<VisualElement>();
                for(int i=0; i<5; i++) {
                    VisualElement tree = new VisualElement();
                    tree.userData = (float)(70 + (i*6));
                    tree.style.position = Position.Absolute;
                    tree.style.bottom = -10; tree.style.left = Length.Percent((float)tree.userData);
                    tree.style.width = 15; tree.style.height = Random.Range(60, 100);
                    tree.style.backgroundColor = new StyleColor(new Color(0.15f, 0.4f, 0.15f));
                    tree.style.borderTopLeftRadius = 8; tree.style.borderTopRightRadius = 8;
                    bg.Add(tree);
                    trees.Add(tree);
                }
                bg.schedule.Execute(() => {
                    foreach(var t in trees) {
                        float v = (float)t.userData;
                        v -= 0.1f;
                        if(v < 40) v = 100;
                        t.userData = v;
                        t.style.left = Length.Percent(v);
                    }
                }).Every(16);
                break;
            case "Capsule Arcade VR":
                bg.style.backgroundColor = new StyleColor(new Color(0.05f, 0.05f, 0.15f));
                var lines = new List<VisualElement>();
                for(int i=0; i<8; i++) {
                    VisualElement line = new VisualElement();
                    line.userData = (float)(60 + (i*5));
                    line.style.position = Position.Absolute;
                    line.style.top = 0; line.style.bottom = 0;
                    line.style.left = Length.Percent((float)line.userData);
                    line.style.width = 2;
                    line.style.backgroundColor = new StyleColor(new Color(0f, 1f, 1f, 0.3f));
                    bg.Add(line);
                    lines.Add(line);
                }
                VisualElement horizon = new VisualElement();
                horizon.style.position = Position.Absolute; horizon.style.top = Length.Percent(50); horizon.style.width = Length.Percent(100); horizon.style.height = 2; horizon.style.backgroundColor = new StyleColor(new Color(1f, 0f, 1f, 0.5f));
                bg.Add(horizon);
                bg.schedule.Execute(() => {
                    foreach(var l in lines) {
                        float v = (float)l.userData;
                        v -= 0.5f;
                        if(v < 50) v = 100;
                        l.userData = v;
                        l.style.left = Length.Percent(v);
                    }
                }).Every(16);
                break;
            case "Volcanic Hot Stone Spa":
                bg.style.backgroundColor = new StyleColor(new Color(0.3f, 0.1f, 0.05f));
                var rocks = new List<VisualElement>();
                for(int i=0; i<12; i++) {
                    VisualElement rock = new VisualElement();
                    rock.userData = (float)Random.Range(-10, 40);
                    rock.style.position = Position.Absolute;
                    rock.style.bottom = (float)rock.userData; rock.style.left = Length.Percent(65 + (i*3));
                    float s = Random.Range(20, 50);
                    rock.style.width = s; rock.style.height = s;
                    rock.style.backgroundColor = new StyleColor(new Color(0.15f, 0.05f, 0.02f));
                    rock.style.borderTopLeftRadius = s/2; rock.style.borderTopRightRadius = s/2; rock.style.borderBottomLeftRadius = s/2; rock.style.borderBottomRightRadius = s/2;
                    bg.Add(rock);
                    rocks.Add(rock);
                }
                bg.schedule.Execute(() => {
                    foreach(var r in rocks) {
                        float v = (float)r.userData;
                        v += 0.2f;
                        if(v > 100) v = -20;
                        r.userData = v;
                        r.style.bottom = v;
                    }
                }).Every(16);
                break;
            case "Suburban Sensory Mud Spa":
                bg.style.backgroundColor = new StyleColor(new Color(0.2f, 0.15f, 0.1f));
                VisualElement mud = new VisualElement();
                mud.style.position = Position.Absolute; mud.style.bottom = 0; mud.style.width = Length.Percent(100); mud.style.height = Length.Percent(40);
                mud.style.backgroundColor = new StyleColor(new Color(0.15f, 0.1f, 0.05f));
                bg.Add(mud);
                break;
            case "Luxury Cryo Therapy":
                bg.style.backgroundColor = new StyleColor(new Color(0.85f, 0.95f, 1f));
                var flakes = new List<VisualElement>();
                for(int i=0; i<20; i++) {
                    VisualElement flake = new VisualElement();
                    flake.userData = (float)Random.Range(0, 200);
                    flake.style.position = Position.Absolute;
                    flake.style.top = (float)flake.userData; flake.style.left = Length.Percent(Random.Range(60, 100));
                    flake.style.width = 4; flake.style.height = 4;
                    flake.style.backgroundColor = new StyleColor(Color.white);
                    bg.Add(flake);
                    flakes.Add(flake);
                }
                bg.schedule.Execute(() => {
                    foreach(var f in flakes) {
                        float v = (float)f.userData;
                        v += 1f;
                        if(v > 300) v = -10;
                        f.userData = v;
                        f.style.top = v;
                    }
                }).Every(16);
                break;
            case "Industrial Dive Bar":
                bg.style.backgroundColor = new StyleColor(new Color(0.1f, 0.1f, 0.15f));
                VisualElement sign = new VisualElement();
                sign.style.position = Position.Absolute; sign.style.top = 20; sign.style.left = Length.Percent(75);
                sign.style.width = 60; sign.style.height = 30; sign.style.backgroundColor = new StyleColor(new Color(0.8f, 0.8f, 0.2f, 0.5f));
                bg.Add(sign);
                float alpha = 0.5f;
                bg.schedule.Execute(() => {
                    alpha = alpha > 0.3f ? 0.2f : 0.8f;
                    sign.style.backgroundColor = new StyleColor(new Color(0.8f, 0.8f, 0.2f, alpha));
                }).Every(500);
                break;
            case "Hospitality Gala":
                bg.style.backgroundColor = new StyleColor(new Color(0.2f, 0.05f, 0.05f));
                VisualElement carpet = new VisualElement();
                carpet.style.position = Position.Absolute; carpet.style.bottom = 0; carpet.style.width = Length.Percent(100); carpet.style.height = 40;
                carpet.style.backgroundColor = new StyleColor(new Color(0.6f, 0.1f, 0.1f));
                bg.Add(carpet);
                break;
            case "Startup Pitch Meetup":
                bg.style.backgroundColor = new StyleColor(new Color(0.05f, 0.15f, 0.25f));
                var bars = new List<VisualElement>();
                for(int i=0; i<4; i++) {
                    VisualElement bar = new VisualElement();
                    bar.style.position = Position.Absolute; bar.style.bottom = 20; bar.style.left = Length.Percent(70 + (i*6));
                    bar.style.width = 20; bar.style.height = 30 + (i*30);
                    bar.style.backgroundColor = new StyleColor(new Color(0.2f, 0.8f, 0.4f));
                    bg.Add(bar);
                    bars.Add(bar);
                }
                bg.schedule.Execute(() => {
                    foreach(var b in bars) {
                        b.style.height = Random.Range(20, 100);
                    }
                }).Every(200);
                break;
            case "Corporate Golf Sim":
                bg.style.backgroundColor = new StyleColor(new Color(0.05f, 0.25f, 0.1f));
                VisualElement hole = new VisualElement();
                hole.style.position = Position.Absolute; hole.style.bottom = 40; hole.style.left = Length.Percent(85);
                hole.style.width = 20; hole.style.height = 10; hole.style.backgroundColor = new StyleColor(Color.black);
                hole.style.borderTopLeftRadius = 10; hole.style.borderTopRightRadius = 10; hole.style.borderBottomLeftRadius = 10; hole.style.borderBottomRightRadius = 10;
                bg.Add(hole);
                VisualElement flag = new VisualElement();
                flag.style.position = Position.Absolute; flag.style.bottom = 45; flag.style.left = Length.Percent(85); flag.style.width = 2; flag.style.height = 50; flag.style.backgroundColor = new StyleColor(Color.white);
                bg.Add(flag);
                VisualElement ball = new VisualElement();
                ball.style.position = Position.Absolute; ball.style.bottom = 40; ball.style.left = Length.Percent(60); ball.style.width = 8; ball.style.height = 8; ball.style.backgroundColor = new StyleColor(Color.white);
                ball.style.borderTopLeftRadius = 4; ball.style.borderTopRightRadius = 4; ball.style.borderBottomLeftRadius = 4; ball.style.borderBottomRightRadius = 4;
                ball.userData = 60f;
                bg.Add(ball);
                bg.schedule.Execute(() => {
                    float v = (float)ball.userData;
                    v += 0.5f;
                    if(v > 86) v = 60;
                    ball.userData = v;
                    ball.style.left = Length.Percent(v);
                }).Every(16);
                break;
        }'''

# Replace the whole switch statement
content = re.sub(r'switch \(name\)\s*\{.*\}', new_switch, content, flags=re.DOTALL)

with open('Assets/Scripts/3 UI/ActivitiesUI.cs', 'w') as f:
    f.write(content)

