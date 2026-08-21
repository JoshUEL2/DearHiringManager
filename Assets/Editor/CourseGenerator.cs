using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public static class CourseGenerator
{
    [MenuItem("Tools/Generate Courses")]
    public static void GenerateCourses()
    {
        string dir = "Assets/Resources/GameData/Courses";
        if (!AssetDatabase.IsValidFolder(dir))
        {
            string[] folders = dir.Split('/');
            string currentPath = folders[0];
            for (int i = 1; i < folders.Length; i++)
            {
                if (!AssetDatabase.IsValidFolder(currentPath + "/" + folders[i]))
                {
                    AssetDatabase.CreateFolder(currentPath, folders[i]);
                }
                currentPath += "/" + folders[i];
            }
        }

        List<CourseSO> generatedCourses = new List<CourseSO>();

        var rawData = new[]
        {
            new { track="SOFTWARE ENGINEERING", t="React Bootcamp", d="48hr div-centering speedruns. Fast-tracks React Trainee & QA eligibility.", c=100.0, w=2, s=0.15f, p="", sec=JobSector.Tech, l=0.08f },
            new { track="SOFTWARE ENGINEERING", t="MS in Reactive Component Lifecycle", d="Advanced performance profiling and re-renders.", c=220.0, w=3, s=0.18f, p="React Bootcamp", sec=JobSector.Tech, l=0.12f },
            new { track="SOFTWARE ENGINEERING", t="PhD in Virtual DOM & Fiber Splicing", d="Deep research into reconciliation algorithms.", c=380.0, w=4, s=0.22f, p="MS in Reactive Component Lifecycle", sec=JobSector.Tech, l=0.18f },

            new { track="CORPORATE MANAGEMENT", t="BA (Trash Sorting)", d="Prerequisite for fast-food shift managerial and general clerical droning.", c=80.0, w=3, s=0.10f, p="", sec=JobSector.Manual, l=0.05f },
            new { track="CORPORATE MANAGEMENT", t="MA (Corp Aggression)", d="Master of Arts in corporate warfare. Unlocks high-society jobs.", c=180.0, w=4, s=0.12f, p="BA (Trash Sorting)", sec=JobSector.Corp, l=0.10f },
            new { track="CORPORATE MANAGEMENT", t="PhD (Greasology)", d="Deep academic doctorate research. Unlocks Interstellar CEO candidacy.", c=350.0, w=5, s=0.18f, p="MA (Corp Aggression)", sec=JobSector.Corp, l=0.15f },

            new { track="SOLAR ENGINEERING", t="Solar Energy Certification", d="Basic sun-panel scrubbing techniques.", c=70.0, w=2, s=0.08f, p="", sec=JobSector.Manual, l=0.05f },
            new { track="SOLAR ENGINEERING", t="MS in Photovoltaic Science", d="Intermediate sunlight capturing.", c=130.0, w=3, s=0.12f, p="Solar Energy Certification", sec=JobSector.Manual, l=0.08f },
            new { track="SOLAR ENGINEERING", t="PhD (Solar Quantum Fusion)", d="Advanced star energy conversion.", c=300.0, w=4, s=0.16f, p="MS in Photovoltaic Science", sec=JobSector.Manual, l=0.12f },

            new { track="ARTISAN BARISTA", t="Certified Barista Basics", d="Learn the alchemy of bean extraction under massive pressure.", c=60.0, w=2, s=0.06f, p="", sec=JobSector.Hospitality, l=0.05f },
            new { track="ARTISAN BARISTA", t="MS in Artisan Latte Artistry", d="Visual arts on milk foam using specialized organic inks.", c=120.0, w=3, s=0.09f, p="Certified Barista Basics", sec=JobSector.Hospitality, l=0.08f },
            new { track="ARTISAN BARISTA", t="PhD (Sensory Siphon Extraction)", d="Doctoral chemistry of volatile coffee-bean aromatic profiles.", c=280.0, w=4, s=0.13f, p="MS in Artisan Latte Artistry", sec=JobSector.Hospitality, l=0.12f },

            new { track="LIQUID LOGIC ARCHITECTURE", t="Plumbing Systems 101", d="Under-slum pipe threading and pressurized safety basics.", c=75.0, w=2, s=0.09f, p="", sec=JobSector.Manual, l=0.05f },
            new { track="LIQUID LOGIC ARCHITECTURE", t="MS in Liquid Logic Architecture", d="Designing automatic water routing grids and filtration networks.", c=140.0, w=3, s=0.12f, p="Plumbing Systems 101", sec=JobSector.Manual, l=0.08f },
            new { track="LIQUID LOGIC ARCHITECTURE", t="PhD (Hydrodynamic Resonance)", d="Academic exploration of sound-frequency water pressure stabilization.", c=310.0, w=4, s=0.17f, p="MS in Liquid Logic Architecture", sec=JobSector.Manual, l=0.12f },
            
            new { track="HOLOGRAPHIC ARTS", t="Basics of Hologram Projection", d="Intro to light bending.", c=80.0, w=2, s=0.08f, p="", sec=JobSector.Tech, l=0.05f },
            new { track="HOLOGRAPHIC ARTS", t="MS in High-Fidelity Persona Design", d="Advanced 3D modeling.", c=150.0, w=3, s=0.12f, p="Basics of Hologram Projection", sec=JobSector.Tech, l=0.08f },
            new { track="HOLOGRAPHIC ARTS", t="PhD (Synthetic Neural Theater)", d="AI driven holograms.", c=320.0, w=4, s=0.18f, p="MS in High-Fidelity Persona Design", sec=JobSector.Tech, l=0.12f },

            new { track="BIOTECH", t="Introductory Algae Biology", d="Basic algae farming.", c=70.0, w=2, s=0.07f, p="", sec=JobSector.Manual, l=0.05f },
            new { track="BIOTECH", t="MS in Bioluminescent Agriculture", d="Glowing algae genetics.", c=140.0, w=3, s=0.11f, p="Introductory Algae Biology", sec=JobSector.Tech, l=0.08f },
            new { track="BIOTECH", t="PhD (Chloroplast Optimization)", d="Maximum algae yield.", c=290.0, w=4, s=0.15f, p="MS in Bioluminescent Agriculture", sec=JobSector.Tech, l=0.12f },

            new { track="LOGISTICS", t="Basic Courier Route Navigation", d="Intro to package delivery.", c=50.0, w=2, s=0.05f, p="", sec=JobSector.Manual, l=0.05f },
            new { track="LOGISTICS", t="MS in Advanced Logistical Trajectories", d="Optimizing delivery paths.", c=110.0, w=3, s=0.08f, p="Basic Courier Route Navigation", sec=JobSector.Manual, l=0.08f },
            new { track="LOGISTICS", t="PhD (Sub-orbital Route Design)", d="Global delivery networks.", c=260.0, w=4, s=0.14f, p="MS in Advanced Logistical Trajectories", sec=JobSector.Manual, l=0.12f },

            new { track="DRONE ENGINEERING", t="Drone Core Mechanics", d="Basic drone repair.", c=85.0, w=2, s=0.10f, p="", sec=JobSector.Tech, l=0.05f },
            new { track="DRONE ENGINEERING", t="MS in Autonomous Aviatronic Engineering", d="Drone AI programming.", c=160.0, w=3, s=0.14f, p="Drone Core Mechanics", sec=JobSector.Tech, l=0.08f },
            new { track="DRONE ENGINEERING", t="PhD (Robotic Aerodynamics)", d="Advanced drone design.", c=330.0, w=4, s=0.19f, p="MS in Autonomous Aviatronic Engineering", sec=JobSector.Tech, l=0.12f },

            new { track="CYBERSECURITY", t="Introduction to Cybersecurity", d="Basic firewall rules.", c=90.0, w=2, s=0.12f, p="", sec=JobSector.Tech, l=0.05f },
            new { track="CYBERSECURITY", t="MS in Network Cryptographic Defense", d="Advanced encryption.", c=170.0, w=3, s=0.16f, p="Introduction to Cybersecurity", sec=JobSector.Tech, l=0.08f },
            new { track="CYBERSECURITY", t="PhD (Quantum Memory Cryptology)", d="Unhackable quantum security.", c=350.0, w=4, s=0.21f, p="MS in Network Cryptographic Defense", sec=JobSector.Tech, l=0.12f },

            new { track="MIXOLOGY", t="Artisanal Beverage Blending", d="Basic drink mixing.", c=65.0, w=2, s=0.06f, p="", sec=JobSector.Hospitality, l=0.05f },
            new { track="MIXOLOGY", t="MS in Gaseous Flavor Infusion", d="Vapor drinks.", c=125.0, w=3, s=0.09f, p="Artisanal Beverage Blending", sec=JobSector.Hospitality, l=0.08f },
            new { track="MIXOLOGY", t="PhD (Carbonated Chemistry Dynamics)", d="Molecular mixology.", c=270.0, w=4, s=0.14f, p="MS in Gaseous Flavor Infusion", sec=JobSector.Hospitality, l=0.12f },

            new { track="ARCHAEOLOGY", t="Retro Typewriter Restorations", d="Fixing old tech.", c=75.0, w=2, s=0.07f, p="", sec=JobSector.Corp, l=0.05f },
            new { track="ARCHAEOLOGY", t="MS in Precision Chronograph Preservation", d="Watch repair.", c=145.0, w=3, s=0.11f, p="Retro Typewriter Restorations", sec=JobSector.Corp, l=0.08f },
            new { track="ARCHAEOLOGY", t="PhD (Chronological Mechanical Archeology)", d="Master of ancient tech.", c=300.0, w=4, s=0.16f, p="MS in Precision Chronograph Preservation", sec=JobSector.Corp, l=0.12f }
        };

        foreach (var data in rawData)
        {
            CourseSO so = ScriptableObject.CreateInstance<CourseSO>();
            so.trackName = data.track;
            so.title = data.t;
            so.description = data.d;
            so.cost = data.c;
            so.weeks = data.w;
            so.stress = data.s;
            so.prerequisite = data.p;
            so.luckCategory = data.sec;
            so.luckBoostAmount = data.l;
            
            if (so.title == "React Bootcamp") so.unlocksBootcamp = true;
            if (so.title == "BA (Trash Sorting)") so.unlocksBA = true;
            if (so.title == "MA (Corp Aggression)") so.unlocksMA = true;
            if (so.title == "PhD (Greasology)") so.unlocksPhD = true;

            string safeName = so.title.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("&", "And");
            string path = $"{dir}/Course_{safeName}.asset";
            AssetDatabase.CreateAsset(so, path);
            generatedCourses.Add(so);
        }

        var dbPath = "Assets/Resources/GameData/GameDatabase.asset";
        var db = AssetDatabase.LoadAssetAtPath<GameDatabaseSO>(dbPath);
        if (db != null)
        {
            db.courses = generatedCourses;
            EditorUtility.SetDirty(db);
        }
        else
        {
            Debug.LogError("Could not find GameDatabaseSO at " + dbPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Course generation complete!");
    }
}
