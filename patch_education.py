import re

with open('Assets/Scripts/3 UI/EducationUI.cs', 'r') as f:
    content = f.read()

# First, modify CourseData to add 'unlocks'
content = content.replace(
    'public string prerequisite;',
    'public string prerequisite;\n        public string unlocks;'
)

# Replace LoadCatalog
old_load_catalog = '''    private void LoadCatalog()
    {
        catalog.Clear();

        // 💻 SOFTWARE ENGINEERING
        catalog.Add(new CourseData { trackName = "SOFTWARE ENGINEERING", title = "React Bootcamp", description = "48hr div-centering speedruns. Fast-tracks React Trainee & QA eligibility.", cost = 100.0, weeks = 2, stress = 0.15f, prerequisite = null });
        catalog.Add(new CourseData { trackName = "SOFTWARE ENGINEERING", title = "MS in Reactive Component Lifecycle", description = "Advanced performance profiling and re-renders.", cost = 220.0, weeks = 3, stress = 0.18f, prerequisite = "React Bootcamp" });
        catalog.Add(new CourseData { trackName = "SOFTWARE ENGINEERING", title = "PhD in Virtual DOM & Fiber Splicing", description = "Deep research into reconciliation algorithms.", cost = 380.0, weeks = 4, stress = 0.22f, prerequisite = "MS in Reactive Component Lifecycle" });

        // 🗑️ CORPORATE MANAGEMENT
        catalog.Add(new CourseData { trackName = "CORPORATE MANAGEMENT", title = "BA (Trash Sorting)", description = "Prerequisite for fast-food shift managerial and general clerical droning.", cost = 80.0, weeks = 3, stress = 0.10f, prerequisite = null });
        catalog.Add(new CourseData { trackName = "CORPORATE MANAGEMENT", title = "MA (Corp Aggression)", description = "Master of Arts in corporate warfare. Unlocks high-society jobs.", cost = 180.0, weeks = 4, stress = 0.12f, prerequisite = "BA (Trash Sorting)" });
        catalog.Add(new CourseData { trackName = "CORPORATE MANAGEMENT", title = "PhD (Greasology)", description = "Deep academic doctorate research. Unlocks Interstellar CEO candidacy.", cost = 350.0, weeks = 5, stress = 0.18f, prerequisite = "MA (Corp Aggression)" });
    }'''

new_load_catalog = '''    private void LoadCatalog()
    {
        catalog.Clear();

        // 💻 SOFTWARE ENGINEERING
        catalog.Add(new CourseData { trackName = "SOFTWARE ENGINEERING", title = "React Bootcamp", description = "48hr div-centering speedruns.", cost = 100.0, weeks = 2, stress = 0.15f, prerequisite = null, unlocks = "React Trainee & QA" });
        catalog.Add(new CourseData { trackName = "SOFTWARE ENGINEERING", title = "MS in Reactive Component Lifecycle", description = "Advanced performance profiling.", cost = 220.0, weeks = 3, stress = 0.18f, prerequisite = "React Bootcamp", unlocks = "Advanced Dev Roles" });
        catalog.Add(new CourseData { trackName = "SOFTWARE ENGINEERING", title = "PhD in Virtual DOM & Fiber Splicing", description = "Deep research into reconciliation.", cost = 380.0, weeks = 4, stress = 0.22f, prerequisite = "MS in Reactive Component Lifecycle", unlocks = "Lead Engineer & Architect" });

        // 🗑️ CORPORATE MANAGEMENT
        catalog.Add(new CourseData { trackName = "CORPORATE MANAGEMENT", title = "BA (Trash Sorting)", description = "General clerical droning fundamentals.", cost = 80.0, weeks = 3, stress = 0.10f, prerequisite = null, unlocks = "Entry Management" });
        catalog.Add(new CourseData { trackName = "CORPORATE MANAGEMENT", title = "MA (Corp Aggression)", description = "Master of Arts in corporate warfare.", cost = 180.0, weeks = 4, stress = 0.12f, prerequisite = "BA (Trash Sorting)", unlocks = "High-Society & Director" });
        catalog.Add(new CourseData { trackName = "CORPORATE MANAGEMENT", title = "PhD (Greasology)", description = "Deep academic doctorate research.", cost = 350.0, weeks = 5, stress = 0.18f, prerequisite = "MA (Corp Aggression)", unlocks = "Interstellar CEO Candidacy" });
    }'''
content = content.replace(old_load_catalog, new_load_catalog)

with open('Assets/Scripts/3 UI/EducationUI.cs', 'w') as f:
    f.write(content)
