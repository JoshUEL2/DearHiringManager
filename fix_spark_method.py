path = 'Assets/Scripts/2 Managers/MiniGames/DragMatchMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

target = """    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
    }"""

replacement = """    public void AbortGame()
    {
        if (sparkCoroutine != null) { manager.StopCoroutine(sparkCoroutine); sparkCoroutine = null; }
        container.style.display = DisplayStyle.None;
    }
    
    private System.Collections.IEnumerator SparkRoutine(Label bolt)
    {
        while (manager.isPlaying && container.style.display != DisplayStyle.None)
        {
            yield return new UnityEngine.WaitForSeconds(UnityEngine.Random.Range(1.5f, 4f));
            if (bolt == null) break;
            
            // Multi-spark flash
            bolt.style.color = new StyleColor(UnityEngine.Color.white);
            bolt.style.scale = new StyleScale(new Scale(new UnityEngine.Vector3(1.3f, 1.3f, 1f)));
            yield return new UnityEngine.WaitForSeconds(0.05f);
            bolt.style.color = new StyleColor(new UnityEngine.Color(1f, 0.8f, 0.1f));
            bolt.style.scale = new StyleScale(new Scale(UnityEngine.Vector3.one));
            yield return new UnityEngine.WaitForSeconds(0.1f);
            
            bolt.style.color = new StyleColor(UnityEngine.Color.white);
            bolt.style.scale = new StyleScale(new Scale(new UnityEngine.Vector3(1.5f, 1.5f, 1f)));
            yield return new UnityEngine.WaitForSeconds(0.05f);
            bolt.style.color = new StyleColor(new UnityEngine.Color(1f, 0.8f, 0.1f));
            bolt.style.scale = new StyleScale(new Scale(UnityEngine.Vector3.one));
        }
    }"""

# Normalize line endings to avoid failure
content = content.replace('\r\n', '\n')

if target in content:
    content = content.replace(target, replacement)
    with open(path, 'w') as f:
        f.write(content)
else:
    print("TARGET NOT FOUND")

