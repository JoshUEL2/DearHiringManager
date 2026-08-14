import os
import glob
import re

extracted = [os.path.basename(f).replace('.uxml', '') for f in glob.glob("Assets/UI/MiniGames/*Container.uxml")]
extracted.sort()

with open("Assets/UI/MiniGame.uxml", "w") as f:
    f.write('<?xml version="1.0" encoding="utf-8"?>\n')
    f.write('<ui:UXML xmlns:ui="UnityEngine.UIElements" xmlns:uie="UnityEditor.UIElements" xsi="http://www.w3.org/2001/XMLSchema-instance" editor-extension-mode="False">\n')
    f.write('    <ui:Style src="styles.uss" />\n\n')
    
    for c in extracted:
        f.write(f'    <ui:Template name="{c}" src="MiniGames/{c}.uxml" />\n')
        
    f.write('\n    <!-- Full screen dark overlay -->\n')
    f.write('    <ui:VisualElement name="Overlay" style="position: absolute; left: 0; top: 0; right: 0; bottom: 0; background-color: rgba(0, 0, 0, 0.85); justify-content: center; align-items: center; display: none;">\n')
    
    # We will add the ABORT button here as well since the user requested it!
    # Wait, the abort button is in the top left corner of the overlay.
    f.write('        <!-- Abort Button (Top Left) -->\n')
    f.write('        <ui:Button name="BtnAbortMinigame" text="X" class="popup-button" style="position: absolute; left: 20px; top: 20px; width: 60px; height: 60px; border-radius: 30px; font-size: 30px; -unity-font-style: bold; background-color: #E74C3C; color: white; justify-content: center; align-items: center;" />\n\n')

    f.write('        <!-- Terminal Window -->\n')
    f.write('        <ui:VisualElement class="popup-card" style="padding: 20px; max-height: 95%; margin-top: -20px;">\n')
    f.write('            <!-- Header -->\n')
    f.write('            <ui:Label name="OperationNameText" text="[Name]" style="font-size: 38px; -unity-font-style: bold; color: #38BDF8; margin-bottom: 10px; -unity-text-align: middle-center;" />\n')
    f.write('            <ui:Label name="InstructionsText" text="[Instructions go here]" style="font-size: 28px; color: #FFFFFF; white-space: normal; margin-bottom: 30px; -unity-text-align: middle-center;" />\n\n')
    f.write('            <ui:ScrollView style="flex-grow: 1; min-height: 500px; max-height: 900px;">\n')
    
    for c in extracted:
        f.write(f'                <ui:Instance template="{c}" />\n')
        
    f.write('            </ui:ScrollView>\n')
    f.write('        </ui:VisualElement>\n')
    f.write('    </ui:VisualElement>\n')
    f.write('</ui:UXML>\n')

print(f"Assembled {len(extracted)} containers into MiniGame.uxml with an Abort Button.")
