using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class EducationUI
{
    public static EducationUI Instance;

    public UIDocument educationDocument;

    private VisualElement activeEnrollmentContainer;
    private Label activeCourseNameText, activeProgressText, activeCostText;
    private VisualElement activeCourseProgressFill;
    private Button btnDropout;
    private ScrollView courseScrollView;

    public class CourseData
    {
        public string trackName;
        public string title;
        public string description;
        public double cost;
        public int weeks;
        public float stress;
        public string prerequisite;
        public string unlocks;
    }

    private List<CourseData> catalog = new List<CourseData>();

    public void Initialize(UIDocument doc)
    {
        educationDocument = doc;
        if (educationDocument == null) return;
        LoadCatalog();

        var root = educationDocument.rootVisualElement;

        activeEnrollmentContainer = root.Q<VisualElement>("ActiveEnrollmentContainer");
        activeCourseNameText = root.Q<Label>("ActiveCourseNameText");
        activeProgressText = root.Q<Label>("ActiveProgressText");
        activeCostText = root.Q<Label>("ActiveCostText");
        activeCourseProgressFill = root.Q<VisualElement>("ActiveCourseProgressFill");
        
        btnDropout = root.Q<Button>("BtnDropout");
        btnDropout.clicked += DropoutOfCourse;

        courseScrollView = root.Q<ScrollView>("CourseScrollView");

        RefreshView();
    }

    private void LoadCatalog()
    {
        catalog.Clear();

        // 💻 SOFTWARE ENGINEERING
        catalog.Add(new CourseData { trackName = "SOFTWARE ENGINEERING", title = "React Bootcamp", description = "48hr div-centering speedruns. Fast-tracks React Trainee & QA eligibility.", cost = 100.0, weeks = 2, stress = 0.15f, prerequisite = null });
        catalog.Add(new CourseData { trackName = "SOFTWARE ENGINEERING", title = "MS in Reactive Component Lifecycle", description = "Advanced performance profiling and re-renders.", cost = 220.0, weeks = 3, stress = 0.18f, prerequisite = "React Bootcamp" });
        catalog.Add(new CourseData { trackName = "SOFTWARE ENGINEERING", title = "PhD in Virtual DOM & Fiber Splicing", description = "Deep research into reconciliation algorithms.", cost = 380.0, weeks = 4, stress = 0.22f, prerequisite = "MS in Reactive Component Lifecycle" });

        // 🗑️ CORPORATE MANAGEMENT
        catalog.Add(new CourseData { trackName = "CORPORATE MANAGEMENT", title = "BA (Trash Sorting)", description = "Prerequisite for fast-food shift managerial and general clerical droning.", cost = 80.0, weeks = 3, stress = 0.10f, prerequisite = null });
        catalog.Add(new CourseData { trackName = "CORPORATE MANAGEMENT", title = "MA (Corp Aggression)", description = "Master of Arts in corporate synergy. Unlocks high-society jobs.", cost = 180.0, weeks = 4, stress = 0.12f, prerequisite = "BA (Trash Sorting)" });
        catalog.Add(new CourseData { trackName = "CORPORATE MANAGEMENT", title = "PhD (Greasology)", description = "Deep academic doctorate research. Unlocks Fortune 500 CEO candidacy.", cost = 350.0, weeks = 5, stress = 0.18f, prerequisite = "MA (Corp Aggression)" });

        // ☕ ARTISAN BARISTA
        catalog.Add(new CourseData { trackName = "ARTISAN BARISTA", title = "Certified Barista Basics", description = "Learn the alchemy of bean extraction under massive pressure.", cost = 60.0, weeks = 2, stress = 0.06f, prerequisite = null });
        catalog.Add(new CourseData { trackName = "ARTISAN BARISTA", title = "MS in Artisan Latte Artistry", description = "Visual arts on milk foam using specialized organic inks.", cost = 120.0, weeks = 3, stress = 0.09f, prerequisite = "Certified Barista Basics" });
        catalog.Add(new CourseData { trackName = "ARTISAN BARISTA", title = "PhD (Sensory Siphon Extraction)", description = "Doctoral chemistry of modern artisanal coffee bean roasting.", cost = 280.0, weeks = 4, stress = 0.13f, prerequisite = "MS in Artisan Latte Artistry" });

        // 🚰 LIQUID LOGIC ARCHITECTURE
        catalog.Add(new CourseData { trackName = "LIQUID LOGIC ARCHITECTURE", title = "Plumbing Systems 101", description = "Underground pipe threading and pressurized safety basics.", cost = 75.0, weeks = 2, stress = 0.09f, prerequisite = null });
        catalog.Add(new CourseData { trackName = "LIQUID LOGIC ARCHITECTURE", title = "MS in Liquid Logic Architecture", description = "Designing automatic water routing grids and filtration networks.", cost = 140.0, weeks = 3, stress = 0.12f, prerequisite = "Plumbing Systems 101" });
        catalog.Add(new CourseData { trackName = "LIQUID LOGIC ARCHITECTURE", title = "PhD (Hydrodynamic Resonance)", description = "Academic exploration of sound-frequency water pressure stabilization.", cost = 310.0, weeks = 4, stress = 0.17f, prerequisite = "MS in Liquid Logic Architecture" });
    }

    public void RefreshView()
    {
        var data = GameManager.Instance.currentData;

        // 1. Toggle Active Enrollment Header
        if (!string.IsNullOrEmpty(GameManager.Instance.currentData.activeEducationTitle))
        {
            activeEnrollmentContainer.style.display = DisplayStyle.Flex;
            activeCourseNameText.text = data.activeEducationTitle;
            activeProgressText.text = $"{data.currentEducationProgressWeeks} / {data.totalEducationWeeksRequested} Weeks";
            activeCostText.text = $"-${data.weeklyEducationCost}/wk";
            
            if (activeCourseProgressFill != null && data.totalEducationWeeksRequested > 0)
            {
                float pct = (float)data.currentEducationProgressWeeks / data.totalEducationWeeksRequested * 100f;
                activeCourseProgressFill.style.width = Length.Percent(pct);
            }
        }
        else
        {
            activeEnrollmentContainer.style.display = DisplayStyle.None;
        }

        // 2. Populate Catalog
        courseScrollView.Clear();
        if (activeEnrollmentContainer.parent != null) {
            activeEnrollmentContainer.parent.Remove(activeEnrollmentContainer);
        }
        courseScrollView.Add(activeEnrollmentContainer);

        string currentTrack = "";
        VisualElement trackContainer = null;
        VisualElement coursesList = null;

        foreach (var course in catalog)
        {
            if (course.trackName != currentTrack)
            {
                currentTrack = course.trackName;

                trackContainer = new VisualElement();
                trackContainer.AddToClassList("stunning-card");
                trackContainer.style.marginBottom = 40;
                trackContainer.style.paddingLeft = 0; trackContainer.style.paddingRight = 0;
                trackContainer.style.paddingTop = 0; trackContainer.style.paddingBottom = 0;
                trackContainer.style.overflow = Overflow.Hidden;

                // The Header
                VisualElement pediment = new VisualElement();
                pediment.style.alignItems = Align.Center;
                pediment.style.justifyContent = Justify.Center;
                pediment.style.paddingTop = 30; pediment.style.paddingBottom = 30;
                pediment.style.backgroundColor = new StyleColor(new Color(0.03f, 0.16f, 0.11f, 0.95f)); // Dark Emerald
                pediment.style.borderBottomWidth = 2;
                pediment.style.borderBottomColor = new StyleColor(new Color(0.06f, 0.73f, 0.51f, 0.6f)); // #10B981
                
                Label trackHeader = new Label(currentTrack);
                trackHeader.style.color = new StyleColor(new Color(0.9f, 0.95f, 1f));
                trackHeader.style.fontSize = 32;
                trackHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
                trackHeader.style.letterSpacing = 4;
                pediment.Add(trackHeader);

                trackContainer.Add(pediment);

                VisualElement contentArea = new VisualElement();
                contentArea.style.flexDirection = FlexDirection.Column;
                contentArea.style.paddingTop = 25;
                contentArea.style.paddingBottom = 15;
                
                coursesList = new VisualElement();
                coursesList.style.flexGrow = 1;
                coursesList.style.paddingLeft = 25;
                coursesList.style.paddingRight = 25;
                contentArea.Add(coursesList);

                trackContainer.Add(contentArea);
                courseScrollView.Add(trackContainer);
            }

            bool isCompleted = GameManager.Instance.completedCourses.Contains(course.title);
            if (course.title == "React Bootcamp" && GameManager.Instance.currentData.hasBootcamp) isCompleted = true;
            if (course.title == "BA (Trash Sorting)" && GameManager.Instance.currentData.hasBa) isCompleted = true;
            if (course.title == "MA (Corp Aggression)" && GameManager.Instance.currentData.hasMa) isCompleted = true;
            if (course.title == "PhD (Greasology)" && GameManager.Instance.currentData.hasPhd) isCompleted = true;

            bool isPrereqMet = string.IsNullOrEmpty(course.prerequisite) || 
                               GameManager.Instance.completedCourses.Contains(course.prerequisite) ||
                               (course.prerequisite == "React Bootcamp" && GameManager.Instance.currentData.hasBootcamp) ||
                               (course.prerequisite == "BA (Trash Sorting)" && GameManager.Instance.currentData.hasBa) ||
                               (course.prerequisite == "MA (Corp Aggression)" && GameManager.Instance.currentData.hasMa);

            bool isEnrolled = data.activeEducationTitle == course.title;

            VisualElement card = new VisualElement();
            card.AddToClassList("task-card-stunning");
            
            if (isCompleted) {
                card.AddToClassList("extra"); // Green border
                card.style.backgroundColor = new StyleColor(new Color(0.1f, 0.3f, 0.15f, 0.6f));
            } else if (isEnrolled) {
                card.style.backgroundColor = new StyleColor(new Color(0.04f, 0.20f, 0.14f, 0.8f));
                card.style.borderLeftColor = new StyleColor(new Color(0.06f, 0.73f, 0.51f));
            } else if (!isPrereqMet) {
                card.style.opacity = 0.5f;
                card.style.backgroundColor = new StyleColor(new Color(0.1f, 0.1f, 0.1f, 0.6f));
                card.style.borderLeftColor = new StyleColor(new Color(0.4f, 0.4f, 0.4f));
            }

            VisualElement leftCol = new VisualElement();
            leftCol.style.flexGrow = 1;

            Label titleLabel = new Label(course.title);
            titleLabel.style.fontSize = 28;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.color = new StyleColor(new Color(1f, 1f, 1f));
            if (!isPrereqMet) titleLabel.style.color = new StyleColor(new Color(0.6f, 0.6f, 0.6f));
            
            Label descLabel = new Label(course.description);
            descLabel.style.fontSize = 20;
            descLabel.style.color = new StyleColor(new Color(0.7f, 0.75f, 0.8f));
            descLabel.style.whiteSpace = WhiteSpace.Normal;
            descLabel.style.marginTop = 8;

            leftCol.Add(titleLabel);
            leftCol.Add(descLabel);

            VisualElement statsRow = new VisualElement();
            statsRow.style.flexDirection = FlexDirection.Row;
            statsRow.style.marginTop = 15;

            Label durationLabel = new Label($"⏳ {course.weeks} Weeks");
            durationLabel.style.fontSize = 22;
            durationLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            durationLabel.style.color = new StyleColor(new Color(0.94f, 0.77f, 0.06f));
            durationLabel.style.marginRight = 25;

            Label costLabel = new Label($"💰 -${course.cost}/wk");
            costLabel.style.fontSize = 22;
            costLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            costLabel.style.color = new StyleColor(new Color(0.9f, 0.3f, 0.23f));

            if (!isPrereqMet) {
                durationLabel.style.color = new StyleColor(new Color(0.6f, 0.6f, 0.6f));
                costLabel.style.color = new StyleColor(new Color(0.6f, 0.6f, 0.6f));
            }

            statsRow.Add(durationLabel);
            statsRow.Add(costLabel);
            leftCol.Add(statsRow);

            card.Add(leftCol);

            Button actionBtn = new Button();
            actionBtn.AddToClassList("btn-task-stunning");
            actionBtn.style.width = 240;
            actionBtn.style.paddingTop = 15; actionBtn.style.paddingBottom = 15;

            if (isCompleted)
            {
                actionBtn.text = "🎓 GRADUATED";
                actionBtn.SetEnabled(false);
                actionBtn.AddToClassList("extra"); 
            }
            else if (isEnrolled)
            {
                actionBtn.text = "📚 STUDYING";
                actionBtn.SetEnabled(false);
            }
            else if (!isPrereqMet)
            {
                actionBtn.text = "🔒 LOCKED";
                actionBtn.SetEnabled(false);
                actionBtn.style.backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f)); 
                actionBtn.style.color = new StyleColor(new Color(0.6f, 0.6f, 0.6f));
            }
            else
            {
                actionBtn.text = "ENROLL";
                actionBtn.clicked += () => EnrollInCourse(course);
            }

            card.Add(actionBtn);
            if (coursesList != null) coursesList.Add(card);
        }
    }

    private void EnrollInCourse(CourseData course)
    {
        var data = GameManager.Instance.currentData;

        if (!string.IsNullOrEmpty(data.activeEducationTitle))
        {
            AudioManager.Instance?.PlayBuzzer();
            global::GameUIManager.Instance.ShowPopup(new RandomEvent { 
                title = "ENROLLMENT ERROR", 
                description = "You are already studying for a degree. Drop out first!" 
            });
            return;
        }

        if (data.balance < course.cost)
        {
            AudioManager.Instance?.PlayBuzzer();
            global::GameUIManager.Instance.ShowPopup(new RandomEvent { 
                title = "TUITION REJECT", 
                description = "Insufficient funds to cover the first week of tuition." 
            });
            return;
        }

        AudioManager.Instance?.PlayStampPass(true);
        GameManager.Instance.currentData.activeEducationTitle = course.title;
        GameManager.Instance.currentData.currentEducationProgressWeeks = 0;
        GameManager.Instance.currentData.totalEducationWeeksRequested = course.weeks;
        GameManager.Instance.currentData.weeklyEducationCost = course.cost;
        GameManager.Instance.currentData.weeklyEducationStress = course.stress;

        global::GameUIManager.Instance.ShowPopup(new RandomEvent { 
            title = "ENROLLED!", 
            description = $"You are now studying for: {course.title}.\n\n<color=#E74C3C>Tuition of ${course.cost} was deducted and will recur weekly.</color>" 
        }, () => {
            GameManager.Instance.CheckHeartAttack(() => {
                global::GameUIManager.Instance.SwitchToDashboard();
            });
        });
        GameManager.Instance.SaveGame();
        RefreshView();
    }

    private void DropoutOfCourse()
    {
        AudioManager.Instance?.PlayPromotionRefused();
        GameManager.Instance.currentData.activeEducationTitle = null;
        GameManager.Instance.currentData.currentEducationProgressWeeks = 0;
        GameManager.Instance.currentData.totalEducationWeeksRequested = 0;
        GameManager.Instance.currentData.weeklyEducationCost = 0.0;
        GameManager.Instance.currentData.weeklyEducationStress = 0.0f;

        global::GameUIManager.Instance.ShowPopup(new RandomEvent { 
            title = "DROPPED OUT", 
            description = "You abandoned your studies. Prior investments lost." 
        }, () => {
            GameManager.Instance.CheckHeartAttack(() => {
                global::GameUIManager.Instance.SwitchToDashboard();
            });
        });
        GameManager.Instance.SaveGame();
        RefreshView();
    }
}
