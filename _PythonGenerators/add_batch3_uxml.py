import sys

with open("Assets/UI/MiniGame.uxml", "r") as f:
    uxml = f.read()

batch3_uxml = """                <!-- GAME TYPE 51: Snatch Luxury Briefcase (Dial Spin) -->
                <ui:VisualElement name="SnatchBriefcaseContainer" style="display: none; width: 100%; min-width: 400px; height: 400px; flex-direction: column; background-color: #1F2937; border-radius: 10px; border-width: 6px; border-color: #FCD34D; margin-bottom: 20px; position: relative; overflow: hidden; align-items: center; justify-content: center; padding: 20px;">
                    <ui:VisualElement style="flex-grow: 1; width: 100%;" />
                    <!-- Leather Texture Background -->
                    <ui:VisualElement style="position: absolute; width: 100%; height: 100%; opacity: 0.8; background-color: #451A03;" />
                    
                    <ui:Label text="CRACK THE LOCK!" style="font-size: 28px; color: #FCD34D; -unity-font-style: bold; margin-bottom: 30px;" />
                    
                    <!-- The 3 Dials -->
                    <ui:VisualElement style="flex-direction: row; justify-content: center; width: 100%; margin-bottom: 30px;">
                        <ui:VisualElement style="width: 80px; height: 100px; background-color: #D4AF37; border-radius: 10px; margin: 0 10px; justify-content: center; align-items: center; border-width: 2px; border-color: #9CA3AF;">
                            <ui:Label name="SbDial0" text="0" style="font-size: 60px; color: black; -unity-font-style: bold;" />
                        </ui:VisualElement>
                        <ui:VisualElement style="width: 80px; height: 100px; background-color: #D4AF37; border-radius: 10px; margin: 0 10px; justify-content: center; align-items: center; border-width: 2px; border-color: #9CA3AF;">
                            <ui:Label name="SbDial1" text="0" style="font-size: 60px; color: black; -unity-font-style: bold;" />
                        </ui:VisualElement>
                        <ui:VisualElement style="width: 80px; height: 100px; background-color: #D4AF37; border-radius: 10px; margin: 0 10px; justify-content: center; align-items: center; border-width: 2px; border-color: #9CA3AF;">
                            <ui:Label name="SbDial2" text="0" style="font-size: 60px; color: black; -unity-font-style: bold;" />
                        </ui:VisualElement>
                    </ui:VisualElement>
                    
                    <ui:Label name="SbTargetText" text="TARGET: 7 - 3 - 9" style="font-size: 24px; color: #10B981; -unity-font-style: bold; margin-bottom: 20px;" />
                    
                    <ui:Button name="BtnSbStop" text="STOP DIAL 1" class="popup-button" style="background-color: #EF4444; color: white; font-size: 28px; padding: 15px 40px; border-radius: 15px; -unity-font-style: bold;" />
                </ui:VisualElement>

                <!-- GAME TYPE 52: Skim ATM Card Data (Wire Connect) -->
                <ui:VisualElement name="SkimATMContainer" style="display: none; width: 100%; min-width: 400px; height: 400px; flex-direction: row; background-color: #111827; border-radius: 10px; border-width: 6px; border-color: #10B981; margin-bottom: 20px; position: relative; overflow: hidden; justify-content: space-between; padding: 20px;">
                    <ui:VisualElement style="flex-grow: 1; width: 100%; position: absolute;" />
                    
                    <ui:Label text="INSTALL SKIMMER" style="position: absolute; top: 10px; left: 50%; translate: -50% 0; font-size: 24px; color: #10B981; -unity-font-style: bold;" />
                    
                    <!-- Left Wires -->
                    <ui:VisualElement style="flex-direction: column; justify-content: space-around; height: 100%; margin-top: 30px;">
                        <ui:Button name="BtnSaL0" style="width: 50px; height: 50px; border-radius: 25px; border-width: 4px; border-color: white;" />
                        <ui:Button name="BtnSaL1" style="width: 50px; height: 50px; border-radius: 25px; border-width: 4px; border-color: white;" />
                        <ui:Button name="BtnSaL2" style="width: 50px; height: 50px; border-radius: 25px; border-width: 4px; border-color: white;" />
                        <ui:Button name="BtnSaL3" style="width: 50px; height: 50px; border-radius: 25px; border-width: 4px; border-color: white;" />
                    </ui:VisualElement>
                    
                    <!-- Right Wires -->
                    <ui:VisualElement style="flex-direction: column; justify-content: space-around; height: 100%; margin-top: 30px;">
                        <ui:Button name="BtnSaR0" style="width: 50px; height: 50px; border-radius: 25px; border-width: 4px; border-color: white;" />
                        <ui:Button name="BtnSaR1" style="width: 50px; height: 50px; border-radius: 25px; border-width: 4px; border-color: white;" />
                        <ui:Button name="BtnSaR2" style="width: 50px; height: 50px; border-radius: 25px; border-width: 4px; border-color: white;" />
                        <ui:Button name="BtnSaR3" style="width: 50px; height: 50px; border-radius: 25px; border-width: 4px; border-color: white;" />
                    </ui:VisualElement>
                </ui:VisualElement>
"""

target = "                <!-- GAME TYPE 44: Volcanic Hot Stone Spa -->"
uxml = uxml.replace(target, batch3_uxml + "\n" + target)

with open("Assets/UI/MiniGame.uxml", "w") as f:
    f.write(uxml)

print("Injected Batch 3 UXML")
