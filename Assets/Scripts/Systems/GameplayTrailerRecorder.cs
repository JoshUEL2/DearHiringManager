using UnityEngine;
using System.Collections;
using System.IO;

public class GameplayTrailerRecorder : MonoBehaviour
{
    private static GameplayTrailerRecorder _instance;
    public static GameplayTrailerRecorder Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<GameplayTrailerRecorder>();
                if (_instance == null)
                {
                    var go = new GameObject("GameplayTrailerRecorder");
                    _instance = go.AddComponent<GameplayTrailerRecorder>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    public bool isRecording = false;
    public int framesCaptured = 0;
    public int targetFrames = 0;

    public void StartRecording(string clipFolder, int totalFrames, int fps = 30)
    {
        StartCoroutine(RecordFramesRoutine(clipFolder, totalFrames, fps));
    }

    private IEnumerator RecordFramesRoutine(string clipFolder, int totalFrames, int fps)
    {
        if (Directory.Exists(clipFolder))
        {
            try { Directory.Delete(clipFolder, true); } catch {}
        }
        Directory.CreateDirectory(clipFolder);

        isRecording = true;
        framesCaptured = 0;
        targetFrames = totalFrames;
        Time.captureFramerate = fps;

        for (int i = 0; i < totalFrames; i++)
        {
            yield return new WaitForEndOfFrame();
            string framePath = Path.Combine(clipFolder, $"frame_{i:D4}.png");
            ScreenCapture.CaptureScreenshot(framePath);
            framesCaptured = i + 1;
        }

        yield return new WaitForSeconds(0.5f);
        Time.captureFramerate = 0;
        isRecording = false;
        Debug.Log($"[GameplayTrailerRecorder] Finished capturing {totalFrames} frames to {clipFolder}");
    }
}
