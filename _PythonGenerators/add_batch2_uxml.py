import sys

with open("Assets/UI/MiniGame.uxml", "r") as f:
    uxml = f.read()

batch2_uxml = """                <!-- GAME TYPE 48: Mow Executive Golf Greens (Grid Trace) -->
                <ui:VisualElement name="MowGreensContainer" style="display: none; width: 100%; min-width: 400px; height: 400px; flex-direction: column; background-color: #064E3B; border-radius: 10px; border-width: 6px; border-color: #8B5A2B; margin-bottom: 20px; position: relative; overflow: hidden; align-items: center; justify-content: center; padding: 20px;">
                    <ui:VisualElement style="flex-grow: 1; width: 100%;" />
                    <ui:Label text="MOW THE GREENS!" style="font-size: 28px; color: white; -unity-font-style: bold; margin-bottom: 20px;" />
                    
                    <ui:VisualElement name="MgGrid" style="flex-direction: row; flex-wrap: wrap; width: 320px; height: 320px; background-color: #022C22; justify-content: space-around; align-content: space-around; padding: 5px; border-radius: 10px;">
                        <!-- 16 Squares (4x4) -->
                        <ui:VisualElement name="MgSq0" style="width: 70px; height: 70px; background-color: #22C55E; border-radius: 8px;" />
                        <ui:VisualElement name="MgSq1" style="width: 70px; height: 70px; background-color: #22C55E; border-radius: 8px;" />
                        <ui:VisualElement name="MgSq2" style="width: 70px; height: 70px; background-color: #22C55E; border-radius: 8px;" />
                        <ui:VisualElement name="MgSq3" style="width: 70px; height: 70px; background-color: #22C55E; border-radius: 8px;" />
                        
                        <ui:VisualElement name="MgSq4" style="width: 70px; height: 70px; background-color: #22C55E; border-radius: 8px;" />
                        <ui:VisualElement name="MgSq5" style="width: 70px; height: 70px; background-color: #22C55E; border-radius: 8px;" />
                        <ui:VisualElement name="MgSq6" style="width: 70px; height: 70px; background-color: #22C55E; border-radius: 8px;" />
                        <ui:VisualElement name="MgSq7" style="width: 70px; height: 70px; background-color: #22C55E; border-radius: 8px;" />
                        
                        <ui:VisualElement name="MgSq8" style="width: 70px; height: 70px; background-color: #22C55E; border-radius: 8px;" />
                        <ui:VisualElement name="MgSq9" style="width: 70px; height: 70px; background-color: #22C55E; border-radius: 8px;" />
                        <ui:VisualElement name="MgSq10" style="width: 70px; height: 70px; background-color: #22C55E; border-radius: 8px;" />
                        <ui:VisualElement name="MgSq11" style="width: 70px; height: 70px; background-color: #22C55E; border-radius: 8px;" />
                        
                        <ui:VisualElement name="MgSq12" style="width: 70px; height: 70px; background-color: #22C55E; border-radius: 8px;" />
                        <ui:VisualElement name="MgSq13" style="width: 70px; height: 70px; background-color: #22C55E; border-radius: 8px;" />
                        <ui:VisualElement name="MgSq14" style="width: 70px; height: 70px; background-color: #22C55E; border-radius: 8px;" />
                        <ui:VisualElement name="MgSq15" style="width: 70px; height: 70px; background-color: #22C55E; border-radius: 8px;" />
                    </ui:VisualElement>
                </ui:VisualElement>

                <!-- GAME TYPE 49: Walk Purebred Pedigree Dogs (Sort puzzle) -->
                <ui:VisualElement name="WalkDogsContainer" style="display: none; width: 100%; min-width: 400px; height: 400px; flex-direction: column; background-color: #6B7280; border-radius: 10px; border-width: 6px; border-color: #374151; margin-bottom: 20px; position: relative; overflow: hidden; align-items: center; justify-content: space-between; padding: 20px;">
                    <ui:VisualElement style="flex-grow: 1; width: 100%;" />
                    <!-- Owners at top -->
                    <ui:VisualElement style="flex-direction: row; justify-content: space-around; width: 100%;">
                        <ui:Label text="🧍‍♂️" name="WdOwner0" style="font-size: 60px; -unity-text-align: middle-center; background-color: #EF4444; border-radius: 10px; width: 80px;" />
                        <ui:Label text="🧍‍♀️" name="WdOwner1" style="font-size: 60px; -unity-text-align: middle-center; background-color: #3B82F6; border-radius: 10px; width: 80px;" />
                        <ui:Label text="🧍" name="WdOwner2" style="font-size: 60px; -unity-text-align: middle-center; background-color: #10B981; border-radius: 10px; width: 80px;" />
                    </ui:VisualElement>
                    
                    <ui:Label text="MATCH DOG TO OWNER!" style="font-size: 24px; color: white; -unity-font-style: bold; margin-top: 20px; margin-bottom: 20px;" />
                    
                    <!-- Dogs at bottom -->
                    <ui:VisualElement style="flex-direction: row; justify-content: space-around; width: 100%;">
                        <ui:Button name="BtnWdDog0" text="🐕" style="font-size: 60px; -unity-text-align: middle-center; background-color: #3B82F6; border-radius: 10px; width: 80px; padding: 0;" />
                        <ui:Button name="BtnWdDog1" text="🐩" style="font-size: 60px; -unity-text-align: middle-center; background-color: #10B981; border-radius: 10px; width: 80px; padding: 0;" />
                        <ui:Button name="BtnWdDog2" text="🐕‍🦺" style="font-size: 60px; -unity-text-align: middle-center; background-color: #EF4444; border-radius: 10px; width: 80px; padding: 0;" />
                    </ui:VisualElement>
                </ui:VisualElement>

                <!-- GAME TYPE 50: Forge Transit Passes (Timing Stamp) -->
                <ui:VisualElement name="ForgePassContainer" style="display: none; width: 100%; min-width: 400px; height: 400px; flex-direction: column; background-color: #111827; border-radius: 10px; border-width: 6px; border-color: #4B5563; margin-bottom: 20px; position: relative; overflow: hidden; align-items: center; justify-content: flex-end; padding-bottom: 20px;">
                    <ui:VisualElement style="flex-grow: 1; width: 100%;" />
                    <!-- Conveyor Belt Background -->
                    <ui:VisualElement style="position: absolute; top: 120px; width: 100%; height: 120px; background-color: #374151; border-width: 2px; border-color: #1F2937;" />
                    
                    <!-- The Moving Pass -->
                    <ui:VisualElement name="FpMovingPass" style="position: absolute; top: 130px; left: -200px; width: 160px; height: 100px; background-color: #F3F4F6; border-radius: 8px; justify-content: center; align-items: center;">
                        <ui:Label text="TRANSIT" style="font-size: 20px; color: #374151; -unity-font-style: bold;" />
                    </ui:VisualElement>
                    
                    <!-- The Holographic Stamp Zone -->
                    <ui:VisualElement name="FpStampZone" style="position: absolute; top: 125px; left: 50%; translate: -50% 0; width: 170px; height: 110px; border-width: 4px; border-color: #10B981; border-radius: 10px; background-color: rgba(16, 185, 129, 0.2);" />
                    
                    <ui:Button name="BtnFpStamp" text="STAMP NOW!" class="popup-button" style="background-color: #10B981; color: white; font-size: 32px; padding: 15px 40px; border-radius: 15px; -unity-font-style: bold; margin-bottom: 10px;" />
                </ui:VisualElement>
"""

target = "                <!-- GAME TYPE 44: Volcanic Hot Stone Spa -->"
uxml = uxml.replace(target, batch2_uxml + "\n" + target)

with open("Assets/UI/MiniGame.uxml", "w") as f:
    f.write(uxml)

print("Injected Batch 2 UXML")
