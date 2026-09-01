with open('Assets/UI/EducationUI.uxml', 'r') as f:
    uxml = f.read()

# I will move the ActiveEnrollmentContainer INSIDE the CourseScrollView, so it scrolls with the courses and isn't "stuck" at the top of the DynamicMiddleWrapper blocking everything!
# Wait, actually CourseScrollView is a ScrollView. If I move it inside, I can just prepend it in C# or place it inside a container inside the ScrollView in UXML.
# Since C# clears the CourseScrollView (courseScrollView.Clear()), it would delete the ActiveEnrollmentContainer if I put it in the ScrollView in UXML.
# So I should leave it outside in UXML, but in C#, I will Add() it to the courseScrollView BEFORE adding the trackContainers!

