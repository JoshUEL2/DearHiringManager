import os
import glob

# Get all containers we found
extracted = [os.path.basename(f).replace('.uxml', '') for f in glob.glob("Assets/UI/MiniGames/*.uxml")]

missing = ['GcProjectileContainer', 'VrArcadeContainer', 'NightShiftContainer', 'CloggedVentsContainer', 'NameMemoryContainer', 'ZenTracingContainer', 'RetroRacerContainer', 'IceShatterContainer', 'WhackABugContainer', 'TicketSwipeContainer', 'NeuralAlignerContainer', 'HotStoneContainer', 'QATriageContainer', 'YesManContainer', 'FormatDashboardContainer', 'FranticBackspaceContainer', 'ConstructionSiteContainer']

# Create empty placeholders for the missing ones
for m in missing:
    with open(f"Assets/UI/MiniGames/{m}.uxml", "w") as f:
        f.write('<?xml version="1.0" encoding="utf-8"?>\n')
        f.write('<ui:UXML xmlns:ui="UnityEngine.UIElements" xmlns:uie="UnityEditor.UIElements">\n')
        f.write(f'    <ui:VisualElement name="{m}" style="display: none;">\n')
        f.write(f'        <ui:Label text="{m} (Under Construction)" style="font-size: 24px; color: white;" />\n')
        f.write('    </ui:VisualElement>\n')
        f.write('</ui:UXML>\n')
    extracted.append(m)

# Now assemble MiniGame.uxml
with open("Assets/UI/MiniGame.uxml", "w") as f:
    f.write('<?xml version="1.0" encoding="utf-8"?>\n')
    f.write('<ui:UXML xmlns:ui="UnityEngine.UIElements" xmlns:uie="UnityEditor.UIElements" xsi="http://www.w3.org/2001/XMLSchema-instance" editor-extension-mode="False">\n')
    f.write('    <ui:Style src="styles.uss" />\n\n')
    
    # Write templates
    for c in extracted:
        f.write(f'    <ui:Template name="{c}" src="MiniGames/{c}.uxml" />\n')
        
    f.write('\n    <!-- Full screen dark overlay -->\n')
    f.write('    <ui:VisualElement name="Overlay" style="position: absolute; left: 0; top: 0; right: 0; bottom: 0; background-color: rgba(0, 0, 0, 0.85); justify-content: center; align-items: center; display: none;">\n')
    f.write('        <!-- Terminal Window -->\n')
    f.write('        <ui:VisualElement class="popup-card" style="padding: 20px; max-height: 95%; margin-top: -20px;">\n')
    f.write('            <!-- Header -->\n')
    f.write('            <ui:Label name="OperationNameText" text="[Name]" style="font-size: 38px; -unity-font-style: bold; color: #38BDF8; margin-bottom: 10px; -unity-text-align: middle-center;" />\n')
    f.write('            <ui:Label name="InstructionsText" text="[Instructions go here]" style="font-size: 28px; color: #FFFFFF; white-space: normal; margin-bottom: 30px; -unity-text-align: middle-center;" />\n\n')
    f.write('            <ui:ScrollView style="flex-grow: 1; min-height: 500px; max-height: 900px;">\n')
    
    # Write instances
    for c in extracted:
        f.write(f'                <ui:Instance template="{c}" />\n')
        
    f.write('            </ui:ScrollView>\n')
    f.write('        </ui:VisualElement>\n')
    f.write('    </ui:VisualElement>\n')
    f.write('</ui:UXML>\n')

print(f"Successfully assembled MiniGame.uxml with {len(extracted)} containers!")
