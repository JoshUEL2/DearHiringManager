import re
import glob

single_top_card = """        <!-- TOP CARD ROOT -->
        <ui:VisualElement name="TopCardRoot" class="card top-card-root" style="margin-bottom: 20px; border-radius: 16px; border-width: 2px; border-color: rgba(255,255,255,0.1); padding: 15px 25px; flex-direction: row; justify-content: space-between; align-items: center;">
            
            <!-- LEFT: Settings & Name -->
            <ui:VisualElement style="flex-direction: row; align-items: center; width: 25%;">
                <ui:Button name="BtnOpenSettings" text="≡" style="font-size: 50px; color: #F8FAFC; background-color: transparent; border-width: 0; padding: 0; margin: 0; margin-right: 15px; -unity-font-style: bold;" />
                <ui:Button name="BtnViewCV" class="id-card-name-btn" text="PlayerName" style="font-size: 32px; color: #F8FAFC; -unity-font-style: bold; background-color: transparent; border-width: 0; padding: 0; margin: 0;" />
            </ui:VisualElement>

            <!-- MIDDLE: Job -->
            <ui:VisualElement style="width: 50%; align-items: center; justify-content: center;">
                <ui:VisualElement style="background-color: rgba(0, 0, 0, 0.4); border-radius: 20px; border-width: 1px; border-color: rgba(255, 255, 255, 0.2); padding: 8px 30px;">
                    <ui:Label name="JobText" text="Unemployed" style="font-size: 24px; color: #E2E8F0; -unity-font-style: bold; margin: 0;" />
                </ui:VisualElement>
            </ui:VisualElement>

            <!-- RIGHT: Home Icon & Finances -->
            <ui:VisualElement style="flex-direction: row; align-items: center; justify-content: flex-end; width: 25%;">
                <ui:Button name="BtnOpenMigration" text="🏠" style="font-size: 40px; background-color: transparent; border-width: 0; padding: 0; margin: 0; margin-right: 25px;" />
                
                <ui:VisualElement style="flex-direction: column; align-items: flex-end; justify-content: center;">
                    <ui:VisualElement style="flex-direction: row; align-items: center;">
                        <ui:Label text="INCOME" style="font-size: 14px; color: #94A3B8; -unity-font-style: bold; margin-right: 5px; padding: 0;" />
                        <ui:Label name="IncomeText" text="+$0.00" style="font-size: 16px; color: #4ADE80; -unity-font-style: bold; margin: 0; padding: 0;" />
                    </ui:VisualElement>
                    
                    <ui:Label name="CashText" text="$0.00" style="font-size: 42px; color: #F8FAFC; -unity-font-style: bold; margin: 2px 0; padding: 0;" />
                    
                    <ui:VisualElement style="flex-direction: row; align-items: center;">
                        <ui:Label text="EXPENSE" style="font-size: 14px; color: #94A3B8; -unity-font-style: bold; margin-right: 5px; padding: 0;" />
                        <ui:Label name="ExpenseText" text="-$0.00" style="font-size: 16px; color: #F87171; -unity-font-style: bold; margin: 0; padding: 0;" />
                    </ui:VisualElement>
                </ui:VisualElement>
            </ui:VisualElement>

        </ui:VisualElement>"""

for f in glob.glob("Assets/UI/*UI.uxml"):
    with open(f, 'r') as file:
        content = file.read()
    
    # In earlier scripts, we ended top-card-root before DynamicMiddleWrapper or DashboardTitle.
    pattern_middle = re.compile(r'        <!-- TOP CARD ROOT -->.*?        <ui:VisualElement name="DynamicMiddleWrapper"', re.DOTALL)
    pattern_dash = re.compile(r'        <!-- TOP CARD ROOT -->.*?        <ui:Label name="DashboardTitle"', re.DOTALL)
    
    if pattern_middle.search(content):
        new_content = pattern_middle.sub(single_top_card + '\n\n        <ui:VisualElement name="DynamicMiddleWrapper"', content, count=1)
        with open(f, 'w') as file: file.write(new_content)
    elif pattern_dash.search(content):
        new_content = pattern_dash.sub(single_top_card + '\n\n        <ui:Label name="DashboardTitle"', content, count=1)
        with open(f, 'w') as file: file.write(new_content)

