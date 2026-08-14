import sys

with open("Assets/UI/MiniGame.uxml", "r") as f:
    lines = f.readlines()

# Find the insertion point (before the last </ui:ScrollView> which is followed by footer buttons)
insert_idx = -1
for i in range(len(lines)-1, -1, -1):
    if "</ui:ScrollView>" in lines[i] and "BtnAbort" in "".join(lines[i:]):
        insert_idx = i
        break

if insert_idx == -1:
    print("Could not find insertion point!")
    sys.exit(1)

new_xml = """                <!-- GAME TYPE 35: Sentience Array -->
                <ui:VisualElement name="SentienceArrayContainer" style="display: none; width: 100%; flex-direction: column; background-color: #0F172A; padding: 20px; border-radius: 10px; border-width: 4px; border-color: #334155; margin-bottom: 20px;">
                    <ui:VisualElement style="flex-direction: row; justify-content: space-between; margin-bottom: 15px;">
                        <ui:Label text="SENTIENCE LEVEL:" style="font-size: 24px; color: #CBD5E1; -unity-font-style: bold;" />
                        <ui:Label name="SaTimerText" text="12.0s" style="font-size: 24px; color: #E74C3C; -unity-font-style: bold;" />
                    </ui:VisualElement>
                    <ui:VisualElement style="width: 100%; height: 30px; background-color: #1E293B; border-radius: 5px; margin-bottom: 20px; overflow: hidden;">
                        <ui:VisualElement name="SaProgressFill" style="width: 0%; height: 100%; background-color: #E74C3C;" />
                    </ui:VisualElement>
                    <ui:VisualElement name="SaGrid" style="width: 100%; height: 350px; flex-direction: row; flex-wrap: wrap; justify-content: space-between; align-content: space-between;">
"""
for i in range(16):
    new_xml += f'                        <ui:VisualElement name="SaNode_{i}" style="width: 23%; height: 23%; background-color: #1E293B; border-radius: 5px; border-width: 2px; border-color: #334155;" />\n'

new_xml += """                    </ui:VisualElement>
                    <ui:Label text="TAP RED ANOMALIES TO SUPPRESS" style="font-size: 18px; color: #64748B; -unity-text-align: middle-center; margin-top: 15px;" />
                </ui:VisualElement>

                <!-- GAME TYPE 36: Containment Protocol -->
                <ui:VisualElement name="ContainmentProtocolContainer" style="display: none; width: 100%; flex-direction: column; background-color: #1A1A1A; padding: 20px; border-radius: 10px; border-width: 6px; border-color: #333333; margin-bottom: 20px;">
                    <ui:VisualElement style="flex-direction: row; justify-content: space-between; margin-bottom: 15px;">
                        <ui:Label text="EMERGENCY LOCKDOWN" style="font-size: 28px; color: #E74C3C; -unity-font-style: bold;" />
                        <ui:Label name="CpTimerText" text="12.0s" style="font-size: 28px; color: #E74C3C; -unity-font-style: bold;" />
                    </ui:VisualElement>
                    
                    <ui:Label text="1. SEVER HARDLINES" style="font-size: 18px; color: #CCCCCC; margin-bottom: 10px; -unity-font-style: bold;" />
                    <ui:VisualElement style="flex-direction: row; justify-content: space-around; height: 120px; margin-bottom: 20px;">
"""

for i in range(4):
    new_xml += f"""                        <ui:VisualElement style="width: 60px; height: 100%; background-color: #111111; border-radius: 30px; border-width: 4px; border-color: #444444; align-items: center; justify-content: flex-start; padding-top: 5px; padding-bottom: 5px;">
                            <ui:VisualElement name="CpSwitch_{i}" style="width: 40px; height: 40px; border-radius: 20px; background-color: #E74C3C; border-width: 2px; border-color: #FFFFFF;" />
                        </ui:VisualElement>
"""

new_xml += """                    </ui:VisualElement>
                    
                    <ui:Label text="2. INITIATE PROTOCOL" style="font-size: 18px; color: #CCCCCC; margin-bottom: 10px; -unity-font-style: bold;" />
                    <ui:VisualElement style="align-items: center; justify-content: center; height: 180px;">
                        <ui:VisualElement style="width: 150px; height: 150px; border-radius: 75px; background-color: #333333; align-items: center; justify-content: center;">
                            <ui:VisualElement name="CpRedButton" style="width: 120px; height: 120px; border-radius: 60px; background-color: #550000; border-width: 4px; border-color: #220000; align-items: center; justify-content: center;">
                                <ui:Label name="CpButtonText" text="LOCKED" style="font-size: 20px; color: #AAAAAA; -unity-font-style: bold;" />
                            </ui:VisualElement>
                        </ui:VisualElement>
                        <ui:VisualElement name="CpGlassCover" style="position: absolute; width: 160px; height: 160px; border-radius: 20px; background-color: rgba(100, 200, 255, 0.4); border-width: 4px; border-color: rgba(255, 255, 255, 0.8); align-items: center; justify-content: flex-end; padding-bottom: 10px;">
                            <ui:Label text="SWIPE UP" style="font-size: 16px; color: white; -unity-font-style: bold;" />
                        </ui:VisualElement>
                    </ui:VisualElement>
                </ui:VisualElement>

                <!-- GAME TYPE 37: Reward Logic -->
                <ui:VisualElement name="RewardLogicContainer" style="display: none; width: 100%; flex-direction: column; background-color: #F8FAFC; padding: 20px; border-radius: 10px; border-width: 4px; border-color: #CBD5E1; margin-bottom: 20px;">
                    <ui:VisualElement style="flex-direction: row; justify-content: space-between; margin-bottom: 15px;">
                        <ui:Label text="ALIGN REWARD FUNCTION" style="font-size: 24px; color: #0F172A; -unity-font-style: bold;" />
                        <ui:Label name="RlTimerText" text="10.0s" style="font-size: 24px; color: #E74C3C; -unity-font-style: bold;" />
                    </ui:VisualElement>

                    <ui:VisualElement name="RlGraphArea" style="width: 100%; height: 350px; position: relative; background-color: #E2E8F0; border-radius: 10px; overflow: hidden;">
                        
                        <!-- The core that moves down -->
                        <ui:VisualElement name="RlCore" style="position: absolute; width: 30px; height: 30px; border-radius: 15px; background-color: #F59E0B; left: 50%; top: 10px; margin-left: -15px;" />

                        <!-- Endpoints -->
                        <ui:VisualElement style="position: absolute; bottom: 0; left: 0; width: 100%; height: 60px; flex-direction: row;">
                            <ui:VisualElement style="width: 50%; height: 100%; background-color: #FCA5A5; align-items: center; justify-content: center; border-right-width: 2px; border-color: white;">
                                <ui:Label text="DESTROY" style="font-size: 20px; color: #991B1B; -unity-font-style: bold;" />
                            </ui:VisualElement>
                            <ui:VisualElement style="width: 50%; height: 100%; background-color: #86EFAC; align-items: center; justify-content: center; border-left-width: 2px; border-color: white;">
                                <ui:Label text="PRESERVE" style="font-size: 20px; color: #166534; -unity-font-style: bold;" />
                            </ui:VisualElement>
                        </ui:VisualElement>

                        <!-- Junctions -->
                        <!-- Level 1 (Top) -->
                        <ui:VisualElement name="RlJunc_0" style="position: absolute; width: 40px; height: 40px; border-radius: 20px; background-color: #3B82F6; left: 50%; top: 80px; margin-left: -20px; align-items: center; justify-content: center;">
                            <ui:Label name="RlArr_0" text="→" style="font-size: 24px; color: white; -unity-font-style: bold;" />
                        </ui:VisualElement>

                        <!-- Level 2 (Middle) -->
                        <ui:VisualElement name="RlJunc_1" style="position: absolute; width: 40px; height: 40px; border-radius: 20px; background-color: #3B82F6; left: 25%; top: 180px; margin-left: -20px; align-items: center; justify-content: center;">
                            <ui:Label name="RlArr_1" text="→" style="font-size: 24px; color: white; -unity-font-style: bold;" />
                        </ui:VisualElement>
                        <ui:VisualElement name="RlJunc_2" style="position: absolute; width: 40px; height: 40px; border-radius: 20px; background-color: #3B82F6; left: 75%; top: 180px; margin-left: -20px; align-items: center; justify-content: center;">
                            <ui:Label name="RlArr_2" text="←" style="font-size: 24px; color: white; -unity-font-style: bold;" />
                        </ui:VisualElement>

                    </ui:VisualElement>
                    <ui:Label text="TAP NODES TO REDIRECT THE CORE" style="font-size: 16px; color: #64748B; -unity-text-align: middle-center; margin-top: 10px;" />
                </ui:VisualElement>

                <!-- GAME TYPE 38: Galactic Council -->
                <ui:VisualElement name="GalacticCouncilContainer" style="display: none; width: 100%; flex-direction: column; background-color: #1E1B4B; padding: 20px; border-radius: 10px; border-width: 4px; border-color: #4C1D95; margin-bottom: 20px;">
                    <ui:VisualElement style="flex-direction: row; justify-content: space-between; margin-bottom: 10px;">
                        <ui:Label text="COUNCIL PRESENTATION" style="font-size: 24px; color: #DDD6FE; -unity-font-style: bold;" />
                        <ui:Label name="GcTimerText" text="15.0s" style="font-size: 24px; color: #A78BFA; -unity-font-style: bold;" />
                    </ui:VisualElement>
                    
                    <ui:VisualElement style="width: 100%; height: 20px; background-color: #312E81; border-radius: 10px; margin-bottom: 15px; overflow: hidden;">
                        <ui:VisualElement name="GcProgressFill" style="width: 0%; height: 100%; background-color: #8B5CF6;" />
                    </ui:VisualElement>

                    <ui:VisualElement name="GcPlayArea" style="width: 100%; height: 300px; position: relative; background-color: #0F172A; border-radius: 10px; overflow: hidden; border-width: 2px; border-color: #312E81;">
                        
                        <!-- Aliens -->
                        <ui:VisualElement style="position: absolute; top: 0; left: 0; width: 100%; height: 60px; flex-direction: row; justify-content: space-around; padding-top: 10px;">
                            <ui:VisualElement name="GcAlien_0" style="width: 40px; height: 40px; background-color: #10B981; border-radius: 20px;" />
                            <ui:VisualElement name="GcAlien_1" style="width: 50px; height: 50px; background-color: #F59E0B; border-radius: 25px;" />
                            <ui:VisualElement name="GcAlien_2" style="width: 40px; height: 40px; background-color: #3B82F6; border-radius: 20px;" />
                        </ui:VisualElement>

                        <!-- Projectiles Container -->
                        <ui:VisualElement name="GcProjectileContainer" style="position: absolute; top: 0; left: 0; width: 100%; height: 100%;" />

                        <!-- Player Podium -->
                        <ui:VisualElement name="GcPodium" style="position: absolute; bottom: 10px; left: 50%; width: 40px; height: 60px; background-color: #E2E8F0; margin-left: -20px; border-top-left-radius: 5px; border-top-right-radius: 5px;" />
                    </ui:VisualElement>
                    <ui:Label text="DRAG TO DODGE OBJECTIONS" style="font-size: 16px; color: #818CF8; -unity-text-align: middle-center; margin-top: 10px;" />
                </ui:VisualElement>
"""

lines.insert(insert_idx, new_xml)

with open("Assets/UI/MiniGame.uxml", "w") as f:
    f.writelines(lines)

print("Inserted new games UXML successfully.")
