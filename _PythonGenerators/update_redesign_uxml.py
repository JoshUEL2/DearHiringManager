import sys
import re

with open("Assets/UI/MiniGame.uxml", "r") as f:
    uxml = f.read()

# 1. Replace CorporateGolfContainer
cg_pattern = r'<!-- GAME TYPE 47: Corporate Golf Sim \(Miss the Putt\) -->.*?</ui:VisualElement>\n'
cg_new = """<!-- GAME TYPE 47: Corporate Golf Sim (Swing Power) -->
                <ui:VisualElement name="CorporateGolfContainer" style="display: none; width: 100%; min-width: 400px; height: 400px; flex-direction: column; background-color: #14532D; border-radius: 10px; border-width: 6px; border-color: #8B5A2B; margin-bottom: 20px; position: relative; overflow: hidden; align-items: center; justify-content: flex-end; padding-bottom: 40px;">
                    <ui:VisualElement style="flex-grow: 1; width: 100%;" />
                    <ui:Label text="LET THE CEO WIN!" style="font-size: 32px; color: #FCD34D; -unity-font-style: bold; position: absolute; top: 20px;" />
                    
                    <!-- Vertical Power Bar -->
                    <ui:VisualElement style="position: absolute; left: 20px; bottom: 20px; width: 60px; height: 300px; background-color: #064E3B; border-width: 4px; border-color: #FCD34D; border-radius: 10px; justify-content: flex-end; overflow: hidden;">
                        <ui:VisualElement style="position: absolute; bottom: 0; height: 30%; width: 100%; background-color: #DC2626;" />
                        <ui:VisualElement style="position: absolute; bottom: 30%; height: 30%; width: 100%; background-color: #F59E0B;" />
                        <ui:VisualElement style="position: absolute; bottom: 60%; height: 40%; width: 100%; background-color: #10B981;" />
                        <ui:VisualElement name="CgPowerFill" style="width: 100%; height: 0%; background-color: rgba(255,255,255,0.8);" />
                    </ui:VisualElement>
                    
                    <ui:VisualElement style="position: absolute; width: 150px; height: 150px; background-color: #064E3B; border-radius: 75px; top: 80px; justify-content: center; align-items: center;">
                        <ui:VisualElement style="width: 40px; height: 40px; background-color: black; border-radius: 20px;" />
                    </ui:VisualElement>

                    <ui:Button name="BtnCgSwing" text="HOLD TO POWER SWING" style="background-color: #FCD34D; color: black; font-size: 24px; padding: 15px 30px; border-radius: 15px; -unity-font-style: bold; margin-left: 100px;" />
                </ui:VisualElement>
"""
uxml = re.sub(cg_pattern, cg_new, uxml, flags=re.DOTALL)

# 2. Replace MowGreensContainer
mg_pattern = r'<!-- GAME TYPE 48: Mow Executive Golf Greens \(Grid Trace\) -->.*?</ui:VisualElement>\n'
mg_new = """<!-- GAME TYPE 48: Mow Executive Golf Greens (Drag Cut) -->
                <ui:VisualElement name="MowGreensContainer" style="display: none; width: 100%; min-width: 400px; height: 400px; flex-direction: column; background-color: #064E3B; border-radius: 10px; border-width: 6px; border-color: #8B5A2B; margin-bottom: 20px; position: relative; overflow: hidden; align-items: center; justify-content: center;">
                    <ui:VisualElement style="flex-grow: 1; width: 100%;" />
                    <ui:VisualElement name="MgGrid" style="flex-direction: row; flex-wrap: wrap; width: 100%; height: 100%; justify-content: center; align-content: center;"></ui:VisualElement>
                    <ui:Label text="DRAG TO MOW!" style="font-size: 32px; color: white; -unity-font-style: bold; position: absolute; pointer-events: none;" />
                </ui:VisualElement>
"""
uxml = re.sub(mg_pattern, mg_new, uxml, flags=re.DOTALL)

# 3. Replace WalkDogsContainer
wd_pattern = r'<!-- GAME TYPE 49: Walk Purebred Pedigree Dogs \(Sort puzzle\) -->.*?</ui:VisualElement>\n'
wd_new = """<!-- GAME TYPE 49: Walk Purebred Pedigree Dogs (Tension Management) -->
                <ui:VisualElement name="WalkDogsContainer" style="display: none; width: 100%; min-width: 400px; height: 400px; flex-direction: column; background-color: #9CA3AF; border-radius: 10px; border-width: 6px; border-color: #4B5563; margin-bottom: 20px; position: relative; overflow: hidden; justify-content: flex-end; padding: 20px;">
                    <ui:VisualElement style="flex-grow: 1; width: 100%;" />
                    <ui:Label text="KEEP TENSION DOWN!" style="font-size: 24px; color: white; position: absolute; top: 10px; left: 50%; translate: -50% 0;" />
                    <ui:VisualElement style="flex-direction: row; justify-content: space-around; width: 100%; height: 300px;">
                        <ui:VisualElement style="width: 80px; height: 100%; flex-direction: column; justify-content: flex-end;">
                            <ui:Label text="🐕" style="font-size: 50px; -unity-text-align: middle-center;" />
                            <ui:VisualElement style="width: 100%; height: 200px; background-color: #4B5563; border-radius: 5px; overflow: hidden; margin-bottom: 10px; justify-content: flex-end;">
                                <ui:VisualElement name="WdLeash0" style="width: 100%; height: 0%; background-color: #EF4444;" />
                            </ui:VisualElement>
                            <ui:Button name="BtnWdYank0" text="YANK" style="background-color: #3B82F6; color: white; font-size: 16px; padding: 10px;" />
                        </ui:VisualElement>
                        <ui:VisualElement style="width: 80px; height: 100%; flex-direction: column; justify-content: flex-end;">
                            <ui:Label text="🐩" style="font-size: 50px; -unity-text-align: middle-center;" />
                            <ui:VisualElement style="width: 100%; height: 200px; background-color: #4B5563; border-radius: 5px; overflow: hidden; margin-bottom: 10px; justify-content: flex-end;">
                                <ui:VisualElement name="WdLeash1" style="width: 100%; height: 0%; background-color: #EF4444;" />
                            </ui:VisualElement>
                            <ui:Button name="BtnWdYank1" text="YANK" style="background-color: #3B82F6; color: white; font-size: 16px; padding: 10px;" />
                        </ui:VisualElement>
                        <ui:VisualElement style="width: 80px; height: 100%; flex-direction: column; justify-content: flex-end;">
                            <ui:Label text="🐕‍🦺" style="font-size: 50px; -unity-text-align: middle-center;" />
                            <ui:VisualElement style="width: 100%; height: 200px; background-color: #4B5563; border-radius: 5px; overflow: hidden; margin-bottom: 10px; justify-content: flex-end;">
                                <ui:VisualElement name="WdLeash2" style="width: 100%; height: 0%; background-color: #EF4444;" />
                            </ui:VisualElement>
                            <ui:Button name="BtnWdYank2" text="YANK" style="background-color: #3B82F6; color: white; font-size: 16px; padding: 10px;" />
                        </ui:VisualElement>
                    </ui:VisualElement>
                </ui:VisualElement>
"""
uxml = re.sub(wd_pattern, wd_new, uxml, flags=re.DOTALL)

# 4. Inject RecycleBottlesContainer
rb_uxml = """                <!-- GAME TYPE 53: Collect Recycled Bottles (Bag Tear) -->
                <ui:VisualElement name="RecycleBottlesContainer" style="display: none; width: 100%; min-width: 400px; height: 400px; flex-direction: column; background-color: #1F2937; border-radius: 10px; border-width: 6px; border-color: #374151; margin-bottom: 20px; position: relative; overflow: hidden; align-items: center; justify-content: center;">
                    <ui:VisualElement style="flex-grow: 1; width: 100%;" />
                    <ui:Label text="RIP OPEN THE BAG! (SWIPE 3 TIMES)" name="RbInstructionText" style="font-size: 20px; color: white; position: absolute; top: 10px;" />
                    
                    <ui:VisualElement name="RbBag" style="width: 300px; height: 300px; background-color: #111827; border-radius: 150px; border-width: 2px; border-color: #374151; justify-content: center; align-items: center;">
                        <ui:Label text="🗑️" style="font-size: 100px;" />
                    </ui:VisualElement>
                    
                    <ui:VisualElement name="RbItemsContainer" style="position: absolute; width: 100%; height: 100%; display: none;">
                        <ui:Button name="BtnRbItem0" text="🍾" style="position: absolute; top: 50px; left: 50px; font-size: 50px; background-color: transparent; border-width: 0;" />
                        <ui:Button name="BtnRbItem1" text="🍾" style="position: absolute; top: 150px; left: 200px; font-size: 50px; background-color: transparent; border-width: 0;" />
                        <ui:Button name="BtnRbItem2" text="🍾" style="position: absolute; top: 250px; left: 80px; font-size: 50px; background-color: transparent; border-width: 0;" />
                        <ui:Button name="BtnRbTrash0" text="🐀" style="position: absolute; top: 80px; left: 250px; font-size: 50px; background-color: transparent; border-width: 0;" />
                        <ui:Button name="BtnRbTrash1" text="🐟" style="position: absolute; top: 200px; left: 30px; font-size: 50px; background-color: transparent; border-width: 0;" />
                        <ui:Button name="BtnRbTrash2" text="🦴" style="position: absolute; top: 280px; left: 220px; font-size: 50px; background-color: transparent; border-width: 0;" />
                    </ui:VisualElement>
                </ui:VisualElement>
"""
target = "                <!-- GAME TYPE 44: Volcanic Hot Stone Spa -->"
uxml = uxml.replace(target, rb_uxml + "\n" + target)

with open("Assets/UI/MiniGame.uxml", "w") as f:
    f.write(uxml)

print("Redesign UXML successfully updated!")
