import re

with open('Assets/UI/EducationUI.uxml', 'r') as f:
    uxml = f.read()

old_enroll = '''            <ui:VisualElement name="ActiveEnrollmentContainer" class="card" style="display: none; margin-bottom: 20px;">
                <ui:Label name="ActiveCourseNameText" text="Course Name" class="job-title" />
                <ui:Label name="ActiveCostText" text="$0" />
                <ui:Label name="ActiveProgressText" text="0%" />
                <ui:VisualElement class="progress-track">
                    <ui:VisualElement name="ActiveCourseProgressFill" class="progress-fill" style="background-color: #F1C40F;" />
                </ui:VisualElement>
                <ui:Button name="BtnDropout" text="Drop Out" class="danger-button" style="margin-top: 10px;" />
            </ui:VisualElement>'''

new_enroll = '''            <ui:VisualElement name="ActiveEnrollmentContainer" class="squircle-card" style="display: none; margin-bottom: 30px; background-color: rgba(30, 70, 120, 0.4); border-color: #38BDF8; border-width: 2px;">
                <ui:VisualElement style="flex-direction: row; justify-content: space-between; align-items: center; margin-bottom: 10px;">
                    <ui:Label text="ACTIVE STUDIES" style="color: #38BDF8; font-size: 22px; -unity-font-style: bold; letter-spacing: 2px;" />
                    <ui:Label name="ActiveCostText" text="$0" style="color: #E74C3C; font-size: 24px; -unity-font-style: bold;" />
                </ui:VisualElement>
                
                <ui:Label name="ActiveCourseNameText" text="Course Name" class="job-title" style="color: #FFFFFF; font-size: 34px; margin-bottom: 15px;" />
                
                <ui:VisualElement style="flex-direction: row; justify-content: space-between; margin-bottom: 5px;">
                    <ui:Label text="Semester Progress" style="color: #CBD5E1; font-size: 20px;" />
                    <ui:Label name="ActiveProgressText" text="0%" style="color: #F1C40F; font-size: 20px; -unity-font-style: bold;" />
                </ui:VisualElement>
                
                <ui:VisualElement class="progress-track" style="margin-bottom: 20px;">
                    <ui:VisualElement name="ActiveCourseProgressFill" class="progress-fill" style="background-color: #F1C40F;" />
                </ui:VisualElement>
                
                <ui:Button name="BtnDropout" text="DROP OUT" style="background-color: #E74C3C; color: white; font-size: 24px; -unity-font-style: bold; border-radius: 8px; padding: 12px;" />
            </ui:VisualElement>'''

uxml = uxml.replace(old_enroll, new_enroll)

with open('Assets/UI/EducationUI.uxml', 'w') as f:
    f.write(uxml)
