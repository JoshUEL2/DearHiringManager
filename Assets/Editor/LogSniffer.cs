using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class LogSniffer
{
    static LogSniffer()
    {
        Application.logMessageReceived += HandleLog;
    }

    private static void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            try {
                System.IO.File.AppendAllText("ConsoleErrors.txt", $"[{type}] {logString}\n{stackTrace}\n---\n");
            } catch {}
        }
    }
}
