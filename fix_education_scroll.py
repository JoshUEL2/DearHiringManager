with open('Assets/Scripts/3 UI/EducationUI.cs', 'r') as f:
    content = f.read()

# Remove unlocksLabel
content = content.replace('Label unlocksLabel = new Label($"► Unlocks: {course.unlocks}");', '')
content = content.replace('unlocksLabel.style.fontSize = 20;', '')
content = content.replace('unlocksLabel.style.unityFontStyleAndWeight = FontStyle.Bold;', '')
content = content.replace('unlocksLabel.style.color = new StyleColor(new Color(0.9f, 0.6f, 0.2f));', '')
content = content.replace('unlocksLabel.style.marginTop = 10;', '')
content = content.replace('if (!isPrereqMet) unlocksLabel.style.color = new StyleColor(new Color(0.4f, 0.4f, 0.4f));', '')
content = content.replace('leftCol.Add(unlocksLabel);', '')

# Move activeEnrollmentContainer into courseScrollView
old_populate = '''        // 2. Populate Catalog
        courseScrollView.Clear();
        string currentTrack = "";'''

new_populate = '''        // 2. Populate Catalog
        courseScrollView.Clear();
        if (activeEnrollmentContainer.parent != null) {
            activeEnrollmentContainer.parent.Remove(activeEnrollmentContainer);
        }
        courseScrollView.Add(activeEnrollmentContainer);

        string currentTrack = "";'''

content = content.replace(old_populate, new_populate)

with open('Assets/Scripts/3 UI/EducationUI.cs', 'w') as f:
    f.write(content)

