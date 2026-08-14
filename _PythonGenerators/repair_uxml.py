import sys
import re

with open("Assets/UI/MiniGame.uxml", "r") as f:
    uxml = f.read()

# Fix Corporate Golf
bad_cg = r'<!-- GAME TYPE 47: Corporate Golf Sim \(Swing Power\).*?<!-- GAME TYPE 48:'
good_cg = """<!-- GAME TYPE 47: Corporate Golf Sim (Swing Power) -->
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

                <!-- GAME TYPE 48:"""
uxml = re.sub(bad_cg, good_cg, uxml, flags=re.DOTALL)

# Fix Mow Greens
bad_mg = r'<!-- GAME TYPE 48: Mow Executive Golf Greens \(Drag Cut\).*?<!-- GAME TYPE 49:'
good_mg = """<!-- GAME TYPE 48: Mow Executive Golf Greens (Drag Cut) -->
                <ui:VisualElement name="MowGreensContainer" style="display: none; width: 100%; min-width: 400px; height: 400px; flex-direction: column; background-color: #064E3B; border-radius: 10px; border-width: 6px; border-color: #8B5A2B; margin-bottom: 20px; position: relative; overflow: hidden; align-items: center; justify-content: center;">
                    <ui:VisualElement style="flex-grow: 1; width: 100%;" />
                    <ui:VisualElement name="MgGrid" style="flex-direction: row; flex-wrap: wrap; width: 100%; height: 100%; justify-content: center; align-content: center;"></ui:VisualElement>
                    <ui:Label text="DRAG TO MOW!" style="font-size: 32px; color: white; -unity-font-style: bold; position: absolute; pointer-events: none;" />
                </ui:VisualElement>

                <!-- GAME TYPE 49:"""
uxml = re.sub(bad_mg, good_mg, uxml, flags=re.DOTALL)

# Fix Walk Dogs
bad_wd = r'<!-- GAME TYPE 49: Walk Purebred Pedigree Dogs \(Tension Management\).*?<!-- GAME TYPE 50:'
good_wd = """<!-- GAME TYPE 49: Walk Purebred Pedigree Dogs (Tension Management) -->
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

                <!-- GAME TYPE 50:"""
uxml = re.sub(bad_wd, good_wd, uxml, flags=re.DOTALL)

with open("Assets/UI/MiniGame.uxml", "w") as f:
    f.write(uxml)
