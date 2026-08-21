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

    public void Initialize(UIDocument doc)
    {
        educationDocument = doc;
        if (educationDocument == null) return;

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
        string currentTrack = "";

        // Safely declare the colors we need for backgrounds
        Color completedCol = Color.green;
        UnityEngine.ColorUtility.TryParseHtmlString("#E8F8F5", out completedCol);

        Color enrolledCol = Color.blue;
        UnityEngine.ColorUtility.TryParseHtmlString("#EBF5FB", out enrolledCol);

        foreach (var course in GameManager.Instance.gameDatabase.courses)
        {
            // Print Track Headers
            if (course.trackName != currentTrack)
            {
                currentTrack = course.trackName;
                Label trackHeader = new Label($"💼 TRACK: {currentTrack}");
                
                // Safe Color assignment for track headers
                Color headerCol = Color.cyan;
                UnityEngine.ColorUtility.TryParseHtmlString("#38BDF8", out headerCol);
                trackHeader.style.color = new StyleColor(headerCol);
                
                trackHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
                trackHeader.style.marginTop = 15;
                trackHeader.style.marginBottom = 5;
                courseScrollView.Add(trackHeader);
            }

            bool isCompleted = GameManager.Instance.completedCourses.Contains(course.title);
            
            // Handle Hardcoded Flags for backward compatibility with old saves
            if (course.unlocksBootcamp && GameManager.Instance.currentData.hasBootcamp) isCompleted = true;
            if (course.unlocksBA && GameManager.Instance.currentData.hasBa) isCompleted = true;
            if (course.unlocksMA && GameManager.Instance.currentData.hasMa) isCompleted = true;
            if (course.unlocksPhD && GameManager.Instance.currentData.hasPhd) isCompleted = true;

            bool isPrereqMet = string.IsNullOrEmpty(course.prerequisite) || 
                               GameManager.Instance.completedCourses.Contains(course.prerequisite) ||
                               (course.prerequisite == "React Bootcamp" && GameManager.Instance.currentData.hasBootcamp) ||
                               (course.prerequisite == "BA (Trash Sorting)" && GameManager.Instance.currentData.hasBa) ||
                               (course.prerequisite == "MA (Corp Aggression)" && GameManager.Instance.currentData.hasMa);

            bool isEnrolled = data.activeEducationTitle == course.title;

            // Build Card
            VisualElement card = new VisualElement();
            card.AddToClassList("job-card");
            card.style.flexDirection = FlexDirection.Row;
            card.style.justifyContent = Justify.SpaceBetween;
            card.style.alignItems = Align.Center;
            
            if (isCompleted) card.style.backgroundColor = new StyleColor(completedCol);
            else if (isEnrolled) card.style.backgroundColor = new StyleColor(enrolledCol);

            // Left Column (Text & Stats)
            VisualElement leftCol = new VisualElement();
            leftCol.style.flexGrow = 1;

            Label titleLabel = new Label(course.title);
            titleLabel.AddToClassList("job-title");
            titleLabel.style.color = new StyleColor(new Color(0.22f, 0.74f, 0.97f)); // #38BDF8
            
            Label descLabel = new Label(course.description);
            descLabel.AddToClassList("job-desc");

            leftCol.Add(titleLabel);
            leftCol.Add(descLabel);

            // Stats Row
            VisualElement statsRow = new VisualElement();
            statsRow.AddToClassList("row");
            statsRow.style.marginTop = 5;
            statsRow.style.justifyContent = Justify.FlexStart;

            Label durationLabel = new Label($"Duration: {course.weeks} Weeks");
            durationLabel.style.fontSize = 28;
            durationLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            durationLabel.style.color = new StyleColor(new Color(0.94f, 0.77f, 0.06f)); // #F1C40F
            durationLabel.style.marginRight = 15;

            Label costLabel = new Label($"Cost: -${course.cost}/wk");
            costLabel.style.fontSize = 28;
            costLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            costLabel.style.color = new StyleColor(new Color(0.9f, 0.3f, 0.23f)); // #E74C3C

            statsRow.Add(durationLabel);
            statsRow.Add(costLabel);
            leftCol.Add(statsRow);

            card.Add(leftCol);

            // Right Column (Action Button)
            Button actionBtn = new Button();
            actionBtn.AddToClassList("card-btn");

            if (isCompleted)
            {
                actionBtn.text = "Completed";
                actionBtn.SetEnabled(false);
                actionBtn.style.backgroundColor = new StyleColor(new Color(0.18f, 0.8f, 0.44f)); // Green
            }
            else if (isEnrolled)
            {
                actionBtn.text = "Studying";
                actionBtn.SetEnabled(false);
                actionBtn.style.backgroundColor = new StyleColor(new Color(0.2f, 0.6f, 0.86f)); // Blue
            }
            else if (!isPrereqMet)
            {
                actionBtn.text = "Locked";
                actionBtn.SetEnabled(false);
                actionBtn.style.backgroundColor = new StyleColor(new Color(0.9f, 0.3f, 0.23f)); // Red
            }
            else
            {
                actionBtn.text = "Enroll";
                actionBtn.clicked += () => EnrollInCourse(course);
            }

            card.Add(actionBtn);

            courseScrollView.Add(card);
        }
    }

    private void EnrollInCourse(CourseSO course)
    {
        var data = GameManager.Instance.currentData;

        if (!string.IsNullOrEmpty(data.activeEducationTitle))
        {
            GameEvents.TriggerShowPopup(new RandomEvent { 
                title = "ENROLLMENT ERROR", 
                description = "You are already studying for a degree. Drop out first!" 
            });
            return;
        }

        if (data.balance < course.cost)
        {
            GameEvents.TriggerShowPopup(new RandomEvent { 
                title = "TUITION REJECT", 
                description = "Insufficient funds to cover the first week of tuition." 
            });
            return;
        }

        GameManager.Instance.currentData.activeEducationTitle = course.title;
        GameManager.Instance.currentData.currentEducationProgressWeeks = 0;
        GameManager.Instance.currentData.totalEducationWeeksRequested = course.weeks;
        GameManager.Instance.currentData.weeklyEducationCost = course.cost;
        GameManager.Instance.currentData.weeklyEducationStress = course.stress;

        GameEvents.TriggerShowPopup(new RandomEvent { 
            title = "ENROLLED!", 
            description = $"You are now studying for: {course.title}.\n\n<color=#E74C3C>Tuition of ${course.cost} was deducted and will recur weekly.</color>" 
        }, () => {
            GameManager.Instance.CheckHeartAttack(() => {
                GameEvents.TriggerSwitchToDashboard();
            });
        });
        GameManager.Instance.SaveGame();
        RefreshView();
    }

    private void DropoutOfCourse()
    {
        GameManager.Instance.currentData.activeEducationTitle = null;
        GameManager.Instance.currentData.currentEducationProgressWeeks = 0;
        GameManager.Instance.currentData.totalEducationWeeksRequested = 0;
        GameManager.Instance.currentData.weeklyEducationCost = 0.0;
        GameManager.Instance.currentData.weeklyEducationStress = 0.0f;

        GameEvents.TriggerShowPopup(new RandomEvent { 
            title = "DROPPED OUT", 
            description = "You abandoned your studies. Prior investments lost." 
        }, () => {
            GameManager.Instance.CheckHeartAttack(() => {
                GameEvents.TriggerSwitchToDashboard();
            });
        });
        GameManager.Instance.SaveGame();
        RefreshView();
    }
}
