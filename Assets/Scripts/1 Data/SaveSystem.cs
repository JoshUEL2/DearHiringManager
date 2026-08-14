using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    // Application.persistentDataPath ensures saves aren't wiped when updating the game
    private static string ActiveGamePath => Path.Combine(Application.persistentDataPath, "active_game_v1.json");
    private static string HistoryPath => Path.Combine(Application.persistentDataPath, "game_history_v1.json");

    // --- Active Game Saving / Loading ---

    public static void SaveActiveGame(SavedGame game)
    {
        string json = JsonUtility.ToJson(game, true);
        File.WriteAllText(ActiveGamePath, json);
    }

    public static SavedGame LoadActiveGame()
    {
        if (File.Exists(ActiveGamePath))
        {
            string json = File.ReadAllText(ActiveGamePath);
            return JsonUtility.FromJson<SavedGame>(json);
        }
        return null; // Returns null if there is no ongoing game
    }

    public static void DeleteActiveGame()
    {
        if (File.Exists(ActiveGamePath))
        {
            File.Delete(ActiveGamePath);
        }
    }

    // --- Game History / Graveyard Saving / Loading ---

    // Unity's JsonUtility cannot serialize top-level lists directly, so we use a wrapper.
    [System.Serializable]
    private class HistoryWrapper
    {
        public List<GameRecord> records = new List<GameRecord>();

        public HistoryWrapper(List<GameRecord> records)
        {
            this.records = records;
        }
    }

    public static List<GameRecord> LoadHistory()
    {
        // Check file storage first
        if (File.Exists(HistoryPath))
        {
            string json = File.ReadAllText(HistoryPath);
            HistoryWrapper wrapper = JsonUtility.FromJson<HistoryWrapper>(json);
            if (wrapper != null && wrapper.records != null)
            {
                return wrapper.records;
            }
        }
        
        // Fallback check to old PlayerPrefs ledger if it exists, then clear it over to files
        if (PlayerPrefs.HasKey("HistoryLedger"))
        {
            string legacyJson = PlayerPrefs.GetString("HistoryLedger");
            var wrapper = JsonUtility.FromJson<HistoryWrapper>(legacyJson);
            if (wrapper != null && wrapper.records != null)
            {
                SaveHistoryToFile(wrapper.records);
                PlayerPrefs.DeleteKey("HistoryLedger");
                PlayerPrefs.Save();
                return wrapper.records;
            }
        }

        return new List<GameRecord>();
    }

    public static void SaveHistoryRecord(string playerName, double finalBalance, int weeksSurvived, string highestJob, string endingAchieved)
    {
        List<GameRecord> history = LoadHistory();

        GameRecord newRecord = new GameRecord
        {
            id = System.Guid.NewGuid().ToString().Substring(0, 5).ToUpper(),
            playerName = playerName,
            finalBalance = finalBalance,
            weeksSurvived = weeksSurvived,
            highestJob = highestJob,
            endingAchieved = endingAchieved
        };

        history.Add(newRecord);
        SaveHistoryToFile(history);
    }

    private static void SaveHistoryToFile(List<GameRecord> records)
    {
        HistoryWrapper wrapper = new HistoryWrapper(records);
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(HistoryPath, json);
    }
}
