path = 'Assets/Scripts/2 Managers/MiniGames/DragMatchMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

target = r'''        p.BeginPath(); p.Arc(new Vector2(w-20, h-20), 6, 0, 360); p.Fill();
    }
}'''

replacement = r'''        p.BeginPath(); p.Arc(new Vector2(w-20, h-20), 6, 0, 360); p.Fill();
    }

    private System.Collections.IEnumerator AnimateParticle()
    {
        var panel = dragLeftColumn.parent;
        if (panel == null) yield break;
        
        var particle = new VisualElement();
        particle.style.position = Position.Absolute;
        particle.style.width = UnityEngine.Random.Range(6, 14);
        particle.style.height = particle.style.width;
        particle.style.backgroundColor = new StyleColor(UnityEngine.Random.value > 0.5f ? UnityEngine.Color.white : UnityEngine.Color.yellow);
        particle.style.borderTopLeftRadius = 10; particle.style.borderTopRightRadius = 10; particle.style.borderBottomLeftRadius = 10; particle.style.borderBottomRightRadius = 10;
        
        particle.style.left = new Length(50, LengthUnit.Percent);
        particle.style.top = new Length(50, LengthUnit.Percent);
        
        panel.Add(particle);
        
        float angle = UnityEngine.Random.Range(0f, 360f) * UnityEngine.Mathf.Deg2Rad;
        float speed = UnityEngine.Random.Range(300f, 700f);
        UnityEngine.Vector2 velocity = new UnityEngine.Vector2(UnityEngine.Mathf.Cos(angle), UnityEngine.Mathf.Sin(angle)) * speed;
        
        float lifeTime = UnityEngine.Random.Range(0.2f, 0.5f);
        float elapsed = 0f;
        
        float x = 0;
        float y = 0;
        
        while(elapsed < lifeTime)
        {
            if (particle == null || panel == null) break;
            float dt = UnityEngine.Time.deltaTime;
            elapsed += dt;
            
            x += velocity.x * dt;
            y += velocity.y * dt;
            velocity.y += 1200f * dt; // Gravity
            
            particle.style.translate = new StyleTranslate(new Translate(new Length(x, LengthUnit.Pixel), new Length(y, LengthUnit.Pixel), 0));
            particle.style.opacity = 1f - (elapsed / lifeTime);
            
            yield return null;
        }
        
        if (particle != null && particle.parent != null) particle.RemoveFromHierarchy();
    }
}'''

content = content.replace('\r\n', '\n')
if target in content:
    content = content.replace(target, replacement)
    with open(path, 'w') as f:
        f.write(content)
else:
    print("TARGET NOT FOUND")

