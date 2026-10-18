using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class WalkDogsMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;

    private VisualElement wdContainer;
    private VisualElement[] wdDogElements = new VisualElement[5];
    private VisualElement[] wdLeashElements = new VisualElement[5];
    private float[] wdDogDistances = new float[5];
    private float[] wdDogAngles = new float[5];
    private float[] wdDogSpeeds = new float[5];
    private float[] wdDogYankTimers = new float[5];
    private VisualElement wdPlayArea;
    private Label wdTimeLabel;
    
    private float timer = 0f;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;
        
        wdContainer = root.Q<VisualElement>("WalkDogsContainer");
        if (wdContainer != null)
        {
            wdTimeLabel = wdContainer.Q<Label>("WdTimeText");
            wdPlayArea = wdContainer.Q<VisualElement>("WdPlayArea");
        }
    }

    public void StartGame()
    {
        timer = config.timeLimit > 0 ? config.timeLimit : 15f;
        if (wdContainer != null) wdContainer.style.display = DisplayStyle.Flex;
        
        if (wdContainer != null)
        {
            if (wdTimeLabel != null) wdTimeLabel.text = $"TIME: {timer:F1}s";
            
            if (wdPlayArea != null) {
                // Clear old dogs/leashes, preserving WdHuman (first child usually, but let us be careful)
                var children = new List<VisualElement>(wdPlayArea.Children());
                foreach(var c in children) {
                    if (c.name != "WdHuman") wdPlayArea.Remove(c);
                }
                
                string[] dogEmojis = { "🐕", "🐩", "🦮", "🐕‍🦺", "🐕" };
                
                for (int i = 0; i < 5; i++)
                {
                    // Create Leash
                    VisualElement leash = new VisualElement();
                    leash.style.position = Position.Absolute;
                    leash.style.height = 4;
                    leash.style.backgroundColor = new StyleColor(new Color(0.6f, 0.3f, 0.1f)); // Brown leash
                    leash.style.transformOrigin = new TransformOrigin(new Length(0, LengthUnit.Percent), new Length(50, LengthUnit.Percent));
                    leash.style.left = new Length(50, LengthUnit.Percent);
                    leash.style.top = new Length(50, LengthUnit.Percent);
                    leash.pickingMode = PickingMode.Ignore;
                    wdPlayArea.Add(leash);
                    wdLeashElements[i] = leash;
                    
                    // Create Dog
                    Label dog = new Label(dogEmojis[i]);
                    dog.style.position = Position.Absolute;
                    dog.style.fontSize = 40;
                    dog.style.width = 50;
                    dog.style.height = 50;
                    dog.style.unityTextAlign = TextAnchor.MiddleCenter;
                    
                    // Random starting stats
                    wdDogAngles[i] = Random.Range(0f, 360f);
                    wdDogDistances[i] = Random.Range(40f, 80f); // start further away
                    wdDogSpeeds[i] = Random.Range(30f, 50f); // double speed
                    wdDogYankTimers[i] = 0f;
                    
                    int idx = i;
                    dog.RegisterCallback<PointerDownEvent>(ev => {
                        if (manager.isPlaying) {
                            wdDogYankTimers[idx] = 1.0f; // Set yank timer to 1 second
                            AudioManager.Instance?.PlayDogBark();
                            MiniGameFeedback.PlayTap(dog);
                            MiniGameFeedback.SpawnFloatingText(dog, "WOOF! 🐕", wdPlayArea ?? wdContainer, new Color(1f, 0.85f, 0.2f));
                            MiniGameFeedback.SpawnSparks(wdPlayArea ?? wdContainer, dog.layout.center, Color.yellow, 8);
                        }
                    });
                    
                    wdPlayArea.Add(dog);
                    wdDogElements[i] = dog;
                }
            }
        }
        manager.StartCoroutine(WalkDogsRoutine());
    }

    public void AbortGame()
    {
        if (wdContainer != null)
        {
            wdContainer.style.display = DisplayStyle.None;
        }
    }

    private IEnumerator WalkDogsRoutine()
    {
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            if (wdTimeLabel != null) wdTimeLabel.text = $"TIME: {Mathf.Max(0, timer):F1}s";
            
            if (timer <= 0f)
            {
                manager.MarkWinReady(); // Survived
                yield break;
            }
            
            if (wdPlayArea != null) {
                float containerWidth = wdPlayArea.worldBound.width;
                float containerHeight = wdPlayArea.worldBound.height;
                
                // If layout hasn't occurred yet, skip processing this frame
                if (containerWidth <= 0 || containerHeight <= 0) {
                    yield return null;
                    continue;
                }
                
                for (int i = 0; i < 5; i++)
                {
                    if (wdDogYankTimers[i] > 0f) {
                        // Smoothly yank back over 1 second
                        wdDogYankTimers[i] -= Time.deltaTime;
                        wdDogDistances[i] -= 80f * Time.deltaTime; // Yankee pull strength
                        wdDogDistances[i] = Mathf.Max(10f, wdDogDistances[i]); // Min distance floor
                    } else {
                        // Move dog outward
                        wdDogDistances[i] += wdDogSpeeds[i] * Time.deltaTime;
                    }
                    
                    // Add some erratic wandering to angle
                    wdDogAngles[i] += Random.Range(-15f, 15f) * Time.deltaTime;
                    
                    float rad = wdDogAngles[i] * Mathf.Deg2Rad;
                    float x = Mathf.Cos(rad) * wdDogDistances[i];
                    float y = Mathf.Sin(rad) * wdDogDistances[i];
                    
                    // Center offset (container coords)
                    float centerX = containerWidth / 2f;
                    float centerY = containerHeight / 2f;
                    
                    if (wdDogElements[i] != null) {
                        wdDogElements[i].style.left = centerX + x - 25f; // offset half dog width
                        wdDogElements[i].style.top = centerY + y - 25f;
                        
                        // Flip sprite horizontally if running left
                        if (x < 0) {
                            wdDogElements[i].style.scale = new StyleScale(new Scale(new Vector3(-1f, 1f, 1f)));
                        } else {
                            wdDogElements[i].style.scale = new StyleScale(new Scale(new Vector3(1f, 1f, 1f)));
                        }
                    }
                    
                    // Box boundary checks (instead of pure radius, so they can reach the corners)
                    float edgeDistX = (containerWidth / 2f) - 30f;
                    float edgeDistY = (containerHeight / 2f) - 30f;
                    bool isNearEdge = Mathf.Abs(x) > edgeDistX * 0.8f || Mathf.Abs(y) > edgeDistY * 0.8f;
                    bool isOut = Mathf.Abs(x) > edgeDistX || Mathf.Abs(y) > edgeDistY;
                    
                    if (wdLeashElements[i] != null) {
                        wdLeashElements[i].style.width = wdDogDistances[i];
                        wdLeashElements[i].style.rotate = new Rotate(new Angle(wdDogAngles[i], AngleUnit.Degree));
                        
                        // Change leash color if getting tense
                        if (isNearEdge) {
                            wdLeashElements[i].style.backgroundColor = new StyleColor(Color.red);
                        } else {
                            wdLeashElements[i].style.backgroundColor = new StyleColor(new Color(0.6f, 0.3f, 0.1f));
                        }
                    }
                    
                    // Check snap condition
                    if (isOut)
                    {
                        if (wdLeashElements[i] != null) wdLeashElements[i].style.display = DisplayStyle.None;
                        manager.EndGame(false); // Leash snaps
                        yield break;
                    }
                }
            }
            
            yield return null;
        }
    }
}
