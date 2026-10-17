using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;
using System.Linq;

public class LoadingScreenUI : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private RenderTexture renderTexture;
    private UIDocument uiDocument;
    private VisualElement rootContainer;
    private VisualElement videoElement;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Initialize()
    {
        GameObject obj = new GameObject("LoadingScreen");
        DontDestroyOnLoad(obj);
        obj.AddComponent<LoadingScreenUI>();
    }

    private void Start()
    {
        // Load the video clip
        VideoClip clip = Resources.Load<VideoClip>("StudioLogo");
        if (clip == null)
        {
            Debug.LogError("LoadingScreenUI: Could not find StudioLogo.mp4 in Resources!");
            Destroy(gameObject);
            return;
        }

        // Create RenderTexture
        renderTexture = new RenderTexture(1920, 1080, 0, RenderTextureFormat.ARGB32);
        renderTexture.Create();

        // Setup VideoPlayer
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.clip = clip;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.isLooping = false;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;

        // Setup UIDocument
        uiDocument = gameObject.AddComponent<UIDocument>();
        uiDocument.sortingOrder = 999; // Ensure it's on top of all other UI
        
        // Use an empty visual tree so UI Toolkit initializes the root
        uiDocument.visualTreeAsset = ScriptableObject.CreateInstance<VisualTreeAsset>();
        
        // Find an existing PanelSettings in the scene
        UIDocument existingDoc = FindObjectsByType<UIDocument>(FindObjectsSortMode.None).FirstOrDefault(d => d.panelSettings != null);
        if (existingDoc != null)
        {
            uiDocument.panelSettings = existingDoc.panelSettings;
        }

        // We can force the panel to initialize if it hasn't already by enabling/disabling
        uiDocument.enabled = false;
        uiDocument.enabled = true;

        if (uiDocument.rootVisualElement != null)
        {
            rootContainer = new VisualElement();
            rootContainer.style.flexGrow = 1;
            rootContainer.style.backgroundColor = new Color(0.082f, 0f, 0f, 1f); // Red background as requested
            rootContainer.style.position = Position.Absolute;
            rootContainer.style.left = 0;
            rootContainer.style.right = 0;
            rootContainer.style.top = 0;
            rootContainer.style.bottom = 0;
            
            videoElement = new VisualElement();
            videoElement.style.flexGrow = 1;
            videoElement.style.backgroundImage = Background.FromRenderTexture(renderTexture);
            videoElement.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            
            rootContainer.RegisterCallback<PointerDownEvent>(evt => {
                Destroy(gameObject);
            });
            rootContainer.focusable = true;
            rootContainer.Focus();
            rootContainer.RegisterCallback<KeyDownEvent>(evt => {
                Destroy(gameObject);
            });

            rootContainer.Add(videoElement);
            uiDocument.rootVisualElement.Add(rootContainer);
        }
        else
        {
            Debug.LogError("LoadingScreenUI: rootVisualElement is null!");
        }

        // Listen for end of video
        videoPlayer.loopPointReached += OnVideoFinished;
        
        // Prepare and play
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += (vp) => { vp.Play(); };

        // Fallback auto-dismiss after video duration + 1s
        Destroy(gameObject, (float)clip.length + 1f);
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
            videoPlayer.targetTexture = null;
            if (videoPlayer.isPlaying) videoPlayer.Stop();
        }
        if (videoElement != null)
        {
            videoElement.style.backgroundImage = null;
            videoElement.RemoveFromHierarchy();
            videoElement = null;
        }
        if (rootContainer != null)
        {
            rootContainer.RemoveFromHierarchy();
            rootContainer = null;
        }
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
            renderTexture = null;
        }
    }
}
