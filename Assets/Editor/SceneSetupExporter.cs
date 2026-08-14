using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;
using System.Text;

public class SceneSetupExporter : EditorWindow
{
    // This makes it appear when you right-click anything in your Scene Hierarchy
    [MenuItem("GameObject/Export UI & Manager Setup", false, 0)]
    // This makes it appear when you right-click files/folders in your Project tab
    [MenuItem("Assets/Export UI & Manager Setup", false, 0)]
    public static void ExportSetup()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== UNITY SCENE SETUP EXPORT ===");
        sb.AppendLine("Reviewing UIDocuments and Manager Scripts...\n");

        // Find all UIDocuments in the scene using the updated Unity API
        UIDocument[] uiDocs = FindObjectsByType<UIDocument>(FindObjectsSortMode.None);
        
        foreach (var doc in uiDocs)
        {
            sb.AppendLine($"📦 GameObject: {doc.gameObject.name}");
            sb.AppendLine($"   ├─ Sort Order: {doc.sortingOrder}");
            sb.AppendLine($"   ├─ Panel Settings: {(doc.panelSettings != null ? doc.panelSettings.name : "MISSING!")}");
            sb.AppendLine($"   ├─ Source UXML: {(doc.visualTreeAsset != null ? doc.visualTreeAsset.name : "MISSING!")}");

            // List other scripts attached to the same GameObject
            var components = doc.gameObject.GetComponents<MonoBehaviour>();
            foreach (var comp in components)
            {
                if (comp != null && comp.GetType() != typeof(UIDocument))
                {
                    sb.AppendLine($"   └─ Attached Script: {comp.GetType().Name}");
                }
            }
            sb.AppendLine("");
        }

        // Catch the GameManager using the updated Unity API
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            sb.AppendLine($"⚙️ GameManager Found on GameObject: {gm.gameObject.name}");
        }
        else
        {
            sb.AppendLine("⚠️ WARNING: GameManager not found in the active scene!");
        }

        // Save the file
        string filePath = Path.Combine(Application.dataPath, "SceneSetupExport.txt");
        File.WriteAllText(filePath, sb.ToString());
        
        // Refresh the asset database so the text file appears in Unity immediately
        AssetDatabase.Refresh();
        
        Debug.Log($"[Export Complete] Scene setup exported successfully to: {filePath}");
    }
}
