import os

careers_uxml = """<ui:UXML xmlns:ui="UnityEngine.UIElements" xmlns:uie="UnityEditor.UIElements" xsi="http://www.w3.org/2001/XMLSchema-instance" editor-extension-mode="False">
    <ui:Style src="styles.uss" />
    <ui:VisualElement class="screen-container modal-container modal-hidden" style="padding-bottom: 160px;">
        <ui:Button name="BtnCloseModal" text="X" class="modal-close-btn" />
        <ui:Label text="CAREERS PORTAL" class="header-text" style="color: #38BDF8; margin-bottom: 10px;" />
        
        <ui:VisualElement name="JobBoardContainer" style="flex-grow: 1; display: flex;">
            <ui:Label text="Available Vacancies in your District:" class="sub-text" style="margin-bottom: 10px;" />
            <ui:ScrollView name="JobScrollView" style="flex-grow: 1;" />
        </ui:VisualElement>

        <ui:VisualElement name="ActiveJobContainer" style="flex-grow: 1; display: none;">
            <ui:VisualElement class="card">
                <ui:Label name="JobTitleText" text="Level 1 Job" class="job-title" />
                <ui:Label name="BaseSalaryText" text="$0" />
                <ui:Label name="PromoBonusText" text="+0%" />
                <ui:Label name="HoursAlignText" text="40 hrs/wk" />
                <ui:Label name="SalaryText" text="Weekly Income: $0" class="job-salary" />
            </ui:VisualElement>

            <ui:VisualElement class="card" style="margin-top: 10px;">
                <ui:Label name="HoursText" text="40 LABORED HOURS / WEEK" style="-unity-font-style: bold; margin-bottom: 5px; color: #FFFFFF;" />
                <ui:VisualElement class="row">
                    <ui:Button name="BtnLessHours" text="Less Hours (-5)" class="apply-button" style="flex-grow: 1; margin-right: 5px;" />
                    <ui:Button name="BtnMoreHours" text="More Hours (+5)" class="apply-button" style="flex-grow: 1; margin-left: 5px;" />
                </ui:VisualElement>
            </ui:VisualElement>

            <ui:Button name="BtnPromotion" text="Ask Manager for Promotion" class="apply-button" style="margin-top: 10px; background-color: #F1C40F; color: #000000;" />

            <ui:VisualElement name="regularTasksContainer" class="card" style="margin-top: 10px;">
                <ui:Label name="regularTasksStatus" text="REQUIRED WEEKLY DUTIES" class="job-title" style="font-size: 12px; color: #E74C3C;" />
                <ui:Button name="BtnRegularTask" text="Perform Core Duty" class="danger-button" style="margin-top: 5px;" />
            </ui:VisualElement>

            <ui:VisualElement name="extraTasksContainer" class="card" style="margin-top: 10px;">
                <ui:Label name="extraTasksStatus" text="EXTRA TASKS (SURGES PERFORMANCE)" class="job-title" style="font-size: 12px; color: #2ECC71;" />
                <ui:Button name="BtnExtraTask" text="Work Overtime Task" class="apply-button" style="margin-top: 5px; background-color: #2ECC71;" />
            </ui:VisualElement>

            <ui:Button name="BtnQuitJob" text="Resign &amp; Walk Out" class="danger-button" style="margin-top: 20px;" />
        </ui:VisualElement>
    </ui:VisualElement>
</ui:UXML>"""

hustles_uxml = """<ui:UXML xmlns:ui="UnityEngine.UIElements" xmlns:uie="UnityEditor.UIElements" xsi="http://www.w3.org/2001/XMLSchema-instance" editor-extension-mode="False">
    <ui:Style src="styles.uss" />
    <ui:VisualElement class="screen-container modal-container modal-hidden" style="padding-bottom: 160px;">
        <ui:Button name="BtnCloseModal" text="X" class="modal-close-btn" />
        <ui:Label text="SIDE HUSTLES PORTAL" class="header-text" style="color: #38BDF8; margin-bottom: 10px;" />
        <ui:Label text="Legal blue hustles have 0% arrest risks. Red illegal actions carry severe jail time risks." class="sub-text" />
        <ui:ScrollView style="flex-grow: 1; width: 100%;" />
    </ui:VisualElement>
</ui:UXML>"""

education_uxml = """<ui:UXML xmlns:ui="UnityEngine.UIElements" xmlns:uie="UnityEditor.UIElements" xsi="http://www.w3.org/2001/XMLSchema-instance" editor-extension-mode="False">
    <ui:Style src="styles.uss" />
    <ui:VisualElement class="screen-container modal-container modal-hidden" style="padding-bottom: 160px;">
        <ui:Button name="BtnCloseModal" text="X" class="modal-close-btn" />
        <ui:Label text="EDUCATION PORTAL" class="header-text" style="color: #38BDF8; margin-bottom: 10px;" />
        
        <ui:VisualElement name="ActiveEnrollmentContainer" class="card" style="display: none; margin-bottom: 20px;">
            <ui:Label name="ActiveCourseNameText" text="Course Name" class="job-title" />
            <ui:Label name="ActiveCostText" text="$0" />
            <ui:Label name="ActiveProgressText" text="0%" />
            <ui:VisualElement class="progress-track">
                <ui:VisualElement name="ActiveCourseProgressFill" class="progress-fill" style="background-color: #F1C40F;" />
            </ui:VisualElement>
            <ui:Button name="BtnDropout" text="Drop Out" class="danger-button" style="margin-top: 10px;" />
        </ui:VisualElement>

        <ui:ScrollView name="CourseScrollView" style="flex-grow: 1; width: 100%;" />
    </ui:VisualElement>
</ui:UXML>"""

activities_uxml = """<ui:UXML xmlns:ui="UnityEngine.UIElements" xmlns:uie="UnityEditor.UIElements" xsi="http://www.w3.org/2001/XMLSchema-instance" editor-extension-mode="False">
    <ui:Style src="styles.uss" />
    <ui:VisualElement class="screen-container modal-container modal-hidden" style="padding-bottom: 160px;">
        <ui:Button name="BtnCloseModal" text="X" class="modal-close-btn" />
        <ui:Label text="ACTIVITIES PORTAL" class="header-text" style="color: #38BDF8; margin-bottom: 10px;" />
        
        <ui:VisualElement class="row" style="margin-bottom: 20px;">
            <ui:Button name="BtnTabStress" text="Wellness" class="apply-button" style="flex-grow: 1;" />
            <ui:Button name="BtnTabMixers" text="Networking" class="apply-button" style="flex-grow: 1;" />
        </ui:VisualElement>

        <ui:VisualElement name="WellnessContainer" style="flex-grow: 1; display: flex;">
            <ui:ScrollView style="flex-grow: 1; width: 100%;">
                <ui:VisualElement class="card">
                    <ui:Label text="Walk in the Park" class="job-title" />
                    <ui:Button name="BtnPark" text="Perform" class="apply-button" />
                </ui:VisualElement>
                <ui:VisualElement class="card">
                    <ui:Label text="Capsule Arcade VR" class="job-title" />
                    <ui:Button name="BtnVR" text="Perform" class="apply-button" />
                </ui:VisualElement>
                <ui:VisualElement class="card">
                    <ui:Label text="Volcanic Hot Stone Spa" class="job-title" />
                    <ui:Button name="BtnOxygen" text="Perform" class="apply-button" />
                </ui:VisualElement>
                <ui:VisualElement class="card">
                    <ui:Label text="Suburban Sensory Mud Spa" class="job-title" />
                    <ui:Button name="BtnSpa" text="Perform" class="apply-button" />
                </ui:VisualElement>
                <ui:VisualElement class="card">
                    <ui:Label text="Interstellar Cryo Resort" class="job-title" />
                    <ui:Button name="BtnCryo" text="Perform" class="apply-button" />
                </ui:VisualElement>
            </ui:ScrollView>
        </ui:VisualElement>

        <ui:VisualElement name="MixersContainer" style="flex-grow: 1; display: none;">
            <ui:ScrollView style="flex-grow: 1; width: 100%;">
                <ui:VisualElement class="card">
                    <ui:Label text="Manual / Service Mixer" class="job-title" />
                    <ui:Button name="BtnMixerManual" text="Perform" class="apply-button" />
                </ui:VisualElement>
                <ui:VisualElement class="card">
                    <ui:Label text="Hospitality Mixer" class="job-title" />
                    <ui:Button name="BtnMixerHospitality" text="Perform" class="apply-button" />
                </ui:VisualElement>
                <ui:VisualElement class="card">
                    <ui:Label text="Tech Mixer" class="job-title" />
                    <ui:Button name="BtnMixerTech" text="Perform" class="apply-button" />
                </ui:VisualElement>
                <ui:VisualElement class="card">
                    <ui:Label text="Corporate Mixer" class="job-title" />
                    <ui:Button name="BtnMixerCorp" text="Perform" class="apply-button" />
                </ui:VisualElement>
            </ui:ScrollView>
        </ui:VisualElement>

    </ui:VisualElement>
</ui:UXML>"""

import os
os.makedirs('Assets/UI', exist_ok=True)
with open('Assets/UI/CareersUI.uxml', 'w') as f:
    f.write(careers_uxml)
with open('Assets/UI/HustlesUI.uxml', 'w') as f:
    f.write(hustles_uxml)
with open('Assets/UI/EducationUI.uxml', 'w') as f:
    f.write(education_uxml)
with open('Assets/UI/ActivitiesUI.uxml', 'w') as f:
    f.write(activities_uxml)
print("Generated basic UXMLs")
