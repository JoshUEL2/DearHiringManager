import re

path = 'Assets/Scripts/2 Managers/MiniGames/DragMatchMiniGame.cs'

with open(path, 'r') as f:
    content = f.read()

# Add sparkCoroutine reference
target_fields = r'''    private Dictionary<int,int> dragCorrectMap = new Dictionary<int,int>\(\);

    public void Initialize'''

replacement_fields = r'''    private Dictionary<int,int> dragCorrectMap = new Dictionary<int,int>();
    private Coroutine sparkCoroutine;

    public void Initialize'''

if 'sparkCoroutine' not in content:
    content = re.sub(target_fields, replacement_fields, content)

# Adjust ScrollView and Start Spark Coroutine
target_setup_start = r'''    private void SetupDragMatch\(\)
    \{
        dragLeftColumn\.Clear\(\);'''

replacement_setup_start = r'''    private void SetupDragMatch()
    {
        var sv = manager.miniGameDocument.rootVisualElement.Q<ScrollView>();
        if (sv != null)
        {
            sv.style.minHeight = 850;
            sv.style.maxHeight = 2500;
        }
        
        dragLeftColumn.Clear();'''

if 'sv.style.minHeight = 850;' not in content:
    content = re.sub(target_setup_start, replacement_setup_start, content)

target_sparks = r'''            var bolt = generatorPanel\.Q<Label>\(\);
            if \(bolt != null\) \{ bolt\.text = "⚡"; bolt\.style\.color = new StyleColor\(new Color\(1f, 0\.8f, 0\.1f\)\); bolt\.style\.fontSize = 60; \}
        \}'''

replacement_sparks = r'''            var bolt = generatorPanel.Q<Label>();
            if (bolt != null) { bolt.text = "⚡"; bolt.style.color = new StyleColor(new Color(1f, 0.8f, 0.1f)); bolt.style.fontSize = 60; }
            
            if (sparkCoroutine != null) manager.StopCoroutine(sparkCoroutine);
            sparkCoroutine = manager.StartCoroutine(SparkRoutine(bolt));
        }'''

if 'SparkRoutine(bolt)' not in content:
    content = re.sub(target_sparks, replacement_sparks, content)


# Add SparkRoutine and Abort logic
target_abort = r'''    public void AbortGame\(\)
    \{
        container\.style\.display = DisplayStyle\.None;
    \}'''

replacement_abort = r'''    public void AbortGame()
    {
        if (sparkCoroutine != null) { manager.StopCoroutine(sparkCoroutine); sparkCoroutine = null; }
        container.style.display = DisplayStyle.None;
    }
    
    private System.Collections.IEnumerator SparkRoutine(Label bolt)
    {
        while (manager.isPlaying && container.style.display != DisplayStyle.None)
        {
            yield return new WaitForSeconds(Random.Range(1.5f, 4f));
            if (bolt == null) break;
            
            // Multi-spark flash
            bolt.style.color = new StyleColor(Color.white);
            bolt.style.scale = new StyleScale(new Scale(new Vector3(1.3f, 1.3f, 1f)));
            yield return new WaitForSeconds(0.05f);
            bolt.style.color = new StyleColor(new Color(1f, 0.8f, 0.1f));
            bolt.style.scale = new StyleScale(new Scale(Vector3.one));
            yield return new WaitForSeconds(0.1f);
            
            bolt.style.color = new StyleColor(Color.white);
            bolt.style.scale = new StyleScale(new Scale(new Vector3(1.5f, 1.5f, 1f)));
            yield return new WaitForSeconds(0.05f);
            bolt.style.color = new StyleColor(new Color(1f, 0.8f, 0.1f));
            bolt.style.scale = new StyleScale(new Scale(Vector3.one));
        }
    }'''

if 'SparkRoutine(' not in content:
    content = re.sub(target_abort, replacement_abort, content)

with open(path, 'w') as f:
    f.write(content)

