using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class JobDataMigrator : MonoBehaviour
{
    private class TaskDef { public string name; public float stress; public float readiness; }

    [MenuItem("Tools/Dear Hiring Manager/Migrate Job Tasks")]
    public static void MigrateJobTasks()
    {
        string folderPath = "Assets/Resources/GameData";
        string[] guids = AssetDatabase.FindAssets("t:JobListingSO", new[] { folderPath });

        if (guids.Length == 0)
        {
            Debug.LogError("No JobListingSO found in " + folderPath);
            return;
        }

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            JobListingSO job = AssetDatabase.LoadAssetAtPath<JobListingSO>(path);

            if (job != null)
            {
                job.regularTasks.Clear();
                foreach (var t in GetRegularTasks(job.title))
                {
                    job.regularTasks.Add(new JobTask { taskName = t.name, stressCost = t.stress, readinessBoost = t.readiness });
                }

                job.extraTasks.Clear();
                foreach (var t in GetExtraTasks(job.title))
                {
                    job.extraTasks.Add(new JobTask { taskName = t.name, stressCost = t.stress, readinessBoost = t.readiness });
                }

                EditorUtility.SetDirty(job);
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Job task migration complete! Migrated " + guids.Length + " jobs.");
    }

    private static List<TaskDef> GetRegularTasks(string title)
    {
        if (title == "Dishwasher") return new List<TaskDef> {
            new TaskDef { name = "Scrub dinner plates", stress = 0.05f },
            new TaskDef { name = "Sweep kitchen floors", stress = 0.04f },
            new TaskDef { name = "Mop grease spills", stress = 0.05f }
        };
        if (title == "Scrap Metal Sorter") return new List<TaskDef> {
            new TaskDef { name = "Sort iron from copper", stress = 0.05f },
            new TaskDef { name = "Stack rusty steel pipes", stress = 0.06f },
            new TaskDef { name = "Clear conveyor dust filters", stress = 0.05f }
        };
        if (title == "Street Corner Sign Spinner") return new List<TaskDef> {
            new TaskDef { name = "Spin arrow sign in traffic", stress = 0.06f },
            new TaskDef { name = "Dance with sign for tourists", stress = 0.05f },
            new TaskDef { name = "Wipe smog off the neon arrow", stress = 0.04f }
        };
        if (title == "Fast Food Shift Manager") return new List<TaskDef> {
            new TaskDef { name = "Check fryer oil temperature", stress = 0.06f },
            new TaskDef { name = "Restock condiment stations", stress = 0.04f },
            new TaskDef { name = "Handle customer complaint", stress = 0.08f }
        };
        if (title == "Corporate Cubicle Drone") return new List<TaskDef> {
            new TaskDef { name = "Send weekly TPS reports", stress = 0.05f },
            new TaskDef { name = "Attend 3-hour video call", stress = 0.07f },
            new TaskDef { name = "Reorganize shared drive", stress = 0.06f }
        };
        if (title == "Junior React Trainee") return new List<TaskDef> {
            new TaskDef { name = "Test program accuracy", stress = 0.04f },
            new TaskDef { name = "Fix CSS layout locations", stress = 0.05f },
            new TaskDef { name = "Center content icons", stress = 0.04f }
        };
        if (title == "Dystopian Quality Assurance Tester") return new List<TaskDef> {
            new TaskDef { name = "Run automated test suite", stress = 0.04f },
            new TaskDef { name = "File bug report", stress = 0.06f },
            new TaskDef { name = "Verify hotfix deployment", stress = 0.05f }
        };
        if (title == "Executive Interstellar CEO") return new List<TaskDef> {
            new TaskDef { name = "Approve quarterly budget", stress = 0.06f },
            new TaskDef { name = "Board meeting presentation", stress = 0.08f },
            new TaskDef { name = "Sign regulatory compliance docs", stress = 0.04f }
        };
        if (title == "AI Prophecy Alignment Director") return new List<TaskDef> {
            new TaskDef { name = "Calibrate neural alignment matrix", stress = 0.08f },
            new TaskDef { name = "Review AI ethics proposals", stress = 0.06f },
            new TaskDef { name = "Monitor sentience detection array", stress = 0.07f }
        };

        // Fallback
        return new List<TaskDef> {
            new TaskDef { name = "Respond to supervisor", stress = 0.04f },
            new TaskDef { name = "Categorize ticket logs", stress = 0.05f },
            new TaskDef { name = "Format summary dashboards", stress = 0.05f }
        };
    }

    private static List<TaskDef> GetExtraTasks(string title)
    {
        if (title == "Dishwasher") return new List<TaskDef> {
            new TaskDef { name = "Scour microwave grease", stress = 0.12f, readiness = 0.20f },
            new TaskDef { name = "Deep scrub waste containers", stress = 0.16f, readiness = 0.32f },
            new TaskDef { name = "Clean fryer grease vats", stress = 0.10f, readiness = 0.15f }
        };
        if (title == "Scrap Metal Sorter") return new List<TaskDef> {
            new TaskDef { name = "Inspect lithium reactor tubes", stress = 0.15f, readiness = 0.25f },
            new TaskDef { name = "Carry tungsten girders", stress = 0.18f, readiness = 0.35f },
            new TaskDef { name = "Audit sewage drainage lines", stress = 0.11f, readiness = 0.18f }
        };
        if (title == "Street Corner Sign Spinner") return new List<TaskDef> {
            new TaskDef { name = "Perform a triple backflip spin", stress = 0.15f, readiness = 0.25f },
            new TaskDef { name = "Hold sign in acid rain storm", stress = 0.18f, readiness = 0.30f },
            new TaskDef { name = "Fight rival sign spinner", stress = 0.16f, readiness = 0.28f }
        };
        if (title == "Fast Food Shift Manager") return new List<TaskDef> {
            new TaskDef { name = "Emergency food prep rush", stress = 0.18f, readiness = 0.30f },
            new TaskDef { name = "Deep clean grease traps", stress = 0.14f, readiness = 0.22f },
            new TaskDef { name = "Train new hire on POS system", stress = 0.12f, readiness = 0.20f }
        };
        if (title == "Corporate Cubicle Drone") return new List<TaskDef> {
            new TaskDef { name = "Corporate sprint tasks", stress = 0.12f, readiness = 0.20f },
            new TaskDef { name = "Overnight marketing pitch", stress = 0.16f, readiness = 0.28f },
            new TaskDef { name = "Agree with executive complaints", stress = 0.10f, readiness = 0.18f }
        };
        if (title == "Junior React Trainee") return new List<TaskDef> {
            new TaskDef { name = "Resolve Git merge conflicts", stress = 0.12f, readiness = 0.22f },
            new TaskDef { name = "Debug CSS at 3 AM", stress = 0.14f, readiness = 0.28f },
            new TaskDef { name = "Inject untested code", stress = 0.16f, readiness = 0.35f }
        };
        if (title == "Dystopian Quality Assurance Tester") return new List<TaskDef> {
            new TaskDef { name = "Regression test full app", stress = 0.15f, readiness = 0.25f },
            new TaskDef { name = "Stress test server load", stress = 0.14f, readiness = 0.22f },
            new TaskDef { name = "Write flaky test workaround", stress = 0.12f, readiness = 0.20f }
        };
        if (title == "Executive Interstellar CEO") return new List<TaskDef> {
            new TaskDef { name = "Hostile takeover negotiations", stress = 0.18f, readiness = 0.35f },
            new TaskDef { name = "Fire underperforming VP", stress = 0.12f, readiness = 0.22f },
            new TaskDef { name = "Construction site inspection", stress = 0.15f, readiness = 0.28f }
        };
        if (title == "AI Prophecy Alignment Director") return new List<TaskDef> {
            new TaskDef { name = "Emergency AI containment protocol", stress = 0.22f, readiness = 0.40f },
            new TaskDef { name = "Align superintelligence reward function", stress = 0.18f, readiness = 0.32f },
            new TaskDef { name = "Present findings to Galactic Council", stress = 0.15f, readiness = 0.25f }
        };

        // Fallback
        return new List<TaskDef> {
            new TaskDef { name = "Do corporate sprint tasks aggressively", stress = 0.12f, readiness = 0.20f },
            new TaskDef { name = "Stay overnight to align marketing pitch variables", stress = 0.14f, readiness = 0.25f },
            new TaskDef { name = "Agree verbally with automated executive complaints", stress = 0.09f, readiness = 0.16f }
        };
    }
}
