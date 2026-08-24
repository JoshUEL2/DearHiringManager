import re

path = 'Assets/Scripts/2 Managers/MiniGames/DragMatchMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

# 1. Move High Voltage warning up
target_text = r'''                warningText\.style\.position = Position\.Absolute;
                warningText\.style\.bottom = 20;
                warningText\.style\.right = -10;'''

replacement_text = r'''                warningText.style.position = Position.Absolute;
                warningText.style.bottom = 35;
                warningText.style.right = 0;'''

content = re.sub(target_text, replacement_text, content)

# 2. Add AnimateParticle call to SparkRoutine
target_spark_loop = r'''            yield return new UnityEngine\.WaitForSeconds\(UnityEngine\.Random\.Range\(1\.5f, 4f\)\);
            if \(bolt == null\) break;
            
            // Multi-spark flash'''

replacement_spark_loop = r'''            yield return new UnityEngine.WaitForSeconds(UnityEngine.Random.Range(1.5f, 4f));
            if (bolt == null) break;
            
            // Spawn physical sparks
            for(int i=0; i<8; i++) {
                manager.StartCoroutine(AnimateParticle());
            }
            
            // Multi-spark flash'''

content = re.sub(target_spark_loop, replacement_spark_loop, content)

# 3. Add AnimateParticle method
target_end = r'''            bolt\.style\.color = new StyleColor\(new UnityEngine\.Color\(1f, 0\.8f, 0\.1f\)\);
            bolt\.style\.scale = new StyleScale\(new Scale\(UnityEngine\.Vector3\.one\)\);
        \}
    \}
\}'''

replacement_end = r'''            bolt.style.color = new StyleColor(new UnityEngine.Color(1f, 0.8f, 0.1f));
            bolt.style.scale = new StyleScale(new Scale(UnityEngine.Vector3.one));
        }
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
            velocity.y += 1200f * dt; // Gravity pulling sparks down
            
            particle.style.translate = new StyleTranslate(new Translate(new Length(x, LengthUnit.Pixel), new Length(y, LengthUnit.Pixel), 0));
            particle.style.opacity = 1f - (elapsed / lifeTime);
            
            yield return null;
        }
        
        if (particle != null && particle.parent != null) particle.RemoveFromHierarchy();
    }
}'''

content = re.sub(target_end, replacement_end, content)

with open(path, 'w') as f:
    f.write(content)
