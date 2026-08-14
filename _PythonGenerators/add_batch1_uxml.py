import sys

with open("Assets/UI/MiniGame.uxml", "r") as f:
    uxml = f.read()

batch1_uxml = """                <!-- GAME TYPE 45: Hospitality Gala (Champagne Balance) -->
                <ui:VisualElement name="HospitalityGalaContainer" style="display: none; width: 100%; min-width: 400px; height: 400px; flex-direction: column; background-color: #4A0E17; border-radius: 10px; border-width: 6px; border-color: #D4AF37; margin-bottom: 20px; position: relative; overflow: hidden; align-items: center; justify-content: space-between; padding: 20px;">
                    <ui:VisualElement style="flex-grow: 1; width: 100%;" />
                    <!-- Champagne Glass Graphic -->
                    <ui:Label text="🥂" style="font-size: 120px; -unity-text-align: middle-center; margin-bottom: 20px;" />
                    
                    <!-- Balance Meter -->
                    <ui:VisualElement style="width: 80%; height: 40px; background-color: #2C080E; border-radius: 20px; border-width: 2px; border-color: #D4AF37; position: relative; justify-content: center; overflow: hidden;">
                        <!-- Safe Zone -->
                        <ui:VisualElement style="position: absolute; width: 40%; height: 100%; left: 30%; background-color: rgba(46, 204, 113, 0.4);" />
                        <!-- Indicator -->
                        <ui:VisualElement name="HgIndicator" style="position: absolute; width: 20px; height: 36px; top: 0px; left: 50%; background-color: #F1C40F; border-radius: 10px;" />
                    </ui:VisualElement>
                    <ui:Label name="HgTimerText" text="10.0s" style="font-size: 24px; color: #D4AF37; -unity-font-style: bold; margin-top: 10px;" />
                </ui:VisualElement>

                <!-- GAME TYPE 46: Startup Pitch Meetup (Buzzword Slot Machine) -->
                <ui:VisualElement name="StartupPitchContainer" style="display: none; width: 100%; min-width: 400px; height: 400px; flex-direction: column; background-color: #1F2937; border-radius: 10px; border-width: 6px; border-color: #F59E0B; margin-bottom: 20px; position: relative; overflow: hidden; align-items: center; justify-content: center;">
                    <ui:VisualElement style="flex-grow: 1; width: 100%;" />
                    <!-- Brick Wall Pattern Background -->
                    <ui:VisualElement style="position: absolute; width: 100%; height: 100%; opacity: 0.1; background-color: #374151;" />
                    
                    <ui:Label text="PITCH YOUR IDEA!" style="font-size: 32px; color: #10B981; -unity-font-style: bold; margin-bottom: 20px;" />
                    
                    <ui:VisualElement style="flex-direction: row; justify-content: center; width: 100%; margin-bottom: 30px;">
                        <!-- Slot 1 -->
                        <ui:VisualElement style="width: 200px; height: 80px; background-color: #111827; border-width: 4px; border-color: #F59E0B; border-radius: 10px; margin: 0 10px; justify-content: center; overflow: hidden;">
                            <ui:Label name="SpSlot0" text="SYNERGY" style="font-size: 28px; color: #FCD34D; -unity-font-style: bold; -unity-text-align: middle-center;" />
                        </ui:VisualElement>
                        <!-- Slot 2 -->
                        <ui:VisualElement style="width: 200px; height: 80px; background-color: #111827; border-width: 4px; border-color: #F59E0B; border-radius: 10px; margin: 0 10px; justify-content: center; overflow: hidden;">
                            <ui:Label name="SpSlot1" text="CLOUD" style="font-size: 28px; color: #FCD34D; -unity-font-style: bold; -unity-text-align: middle-center;" />
                        </ui:VisualElement>
                        <!-- Slot 3 -->
                        <ui:VisualElement style="width: 200px; height: 80px; background-color: #111827; border-width: 4px; border-color: #F59E0B; border-radius: 10px; margin: 0 10px; justify-content: center; overflow: hidden;">
                            <ui:Label name="SpSlot2" text="AI" style="font-size: 28px; color: #FCD34D; -unity-font-style: bold; -unity-text-align: middle-center;" />
                        </ui:VisualElement>
                    </ui:VisualElement>
                    
                    <ui:Button name="BtnSpStop" text="STOP REEL" class="popup-button" style="background-color: #EF4444; color: white; font-size: 32px; padding: 15px 40px; border-radius: 15px; -unity-font-style: bold;" />
                </ui:VisualElement>

                <!-- GAME TYPE 47: Corporate Golf Sim (Miss the Putt) -->
                <ui:VisualElement name="CorporateGolfContainer" style="display: none; width: 100%; min-width: 400px; height: 400px; flex-direction: column; background-color: #14532D; border-radius: 10px; border-width: 6px; border-color: #8B5A2B; margin-bottom: 20px; position: relative; overflow: hidden; align-items: center; justify-content: flex-end; padding-bottom: 40px;">
                    <ui:VisualElement style="flex-grow: 1; width: 100%;" />
                    <ui:Label text="LET THE CEO WIN!" style="font-size: 32px; color: #FCD34D; -unity-font-style: bold; position: absolute; top: 20px;" />
                    <ui:Label text="DO NOT HIT THE HOLE!" style="font-size: 24px; color: #EF4444; -unity-font-style: bold; position: absolute; top: 60px;" />
                    
                    <!-- Swing Meter -->
                    <ui:VisualElement style="width: 80%; height: 60px; background-color: #064E3B; border-radius: 30px; border-width: 4px; border-color: #FCD34D; position: relative; justify-content: center; overflow: hidden; margin-bottom: 30px;">
                        <!-- Red Zone (Fail - Too Far) -->
                        <ui:VisualElement style="position: absolute; width: 100%; height: 100%; background-color: #DC2626;" />
                        <!-- Yellow Zone (Win - Close Miss) -->
                        <ui:VisualElement style="position: absolute; width: 30%; height: 100%; left: 35%; background-color: #F59E0B;" />
                        <!-- Green Zone (Fail - Hole in one) -->
                        <ui:VisualElement style="position: absolute; width: 10%; height: 100%; left: 45%; background-color: #10B981;" />
                        
                        <!-- The Marker -->
                        <ui:VisualElement name="CgMarker" style="position: absolute; width: 12px; height: 60px; top: -4px; left: 0%; background-color: #FFFFFF; border-radius: 6px;" />
                    </ui:VisualElement>
                    
                    <ui:Button name="BtnCgSwing" text="SWING!" class="popup-button" style="background-color: #3B82F6; color: white; font-size: 36px; padding: 15px 60px; border-radius: 20px; -unity-font-style: bold;" />
                </ui:VisualElement>
"""

target = "                <!-- GAME TYPE 44: Volcanic Hot Stone Spa -->"
uxml = uxml.replace(target, batch1_uxml + "\n" + target)

with open("Assets/UI/MiniGame.uxml", "w") as f:
    f.write(uxml)

print("Injected Batch 1 UXML")
