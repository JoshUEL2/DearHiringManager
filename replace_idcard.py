import re
import glob

replacement = """            <!-- STYLE 0: ID CARD -->
            <ui:VisualElement name="HUD-IDCard" style="display: none; background-color: #E2E8F0; border-radius: 12px; border-width: 4px; border-color: #94A3B8; padding: 10px 20px; flex-direction: row; align-items: center; justify-content: space-between;">
                <!-- FAR LEFT: Settings -->
                <ui:Button name="BtnOpenSettings" text="≡" style="font-size: 45px; color: #475569; background-color: transparent; border-width: 0; padding: 0; margin: 0; margin-right: 15px;"/>
                
                <!-- NAME (Clickable -> View CV) -->
                <ui:Button name="BtnViewCV" class="id-card-name-btn" text="PlayerName" style="font-size: 32px; color: #0F172A; -unity-font-style: bold; background-color: transparent; border-width: 0; padding: 0; margin: 0; flex-shrink: 1; transition-property: scale; transition-duration: 0.1s;"/>
                
                <!-- MIDDLE: JOB -->
                <ui:VisualElement style="flex-grow: 1; align-items: center; justify-content: center;">
                    <ui:VisualElement style="background-color: #CBD5E1; border-radius: 6px; padding: 6px 16px;">
                        <ui:Label name="JobText" text="Unemployed" style="font-size: 22px; color: #334155; -unity-font-style: bold; margin: 0;"/>
                    </ui:VisualElement>
                </ui:VisualElement>
                
                <!-- RIGHT: FINANCE (Master Balance, Income, Expenses) -->
                <ui:VisualElement style="flex-direction: column; align-items: flex-end;">
                    <ui:VisualElement style="flex-direction: row; align-items: center;">
                        <ui:Label text="INCOME" style="font-size: 14px; color: #64748B; -unity-font-style: bold; margin-right: 5px;"/>
                        <ui:Label name="IncomeText" text="+$0.00" style="font-size: 16px; color: #16A34A; -unity-font-style: bold; margin: 0;"/>
                    </ui:VisualElement>
                    <ui:Label name="CashText" text="$0.00" style="font-size: 36px; color: #0F172A; -unity-font-style: bold; margin: 2px 0;"/>
                    <ui:VisualElement style="flex-direction: row; align-items: center;">
                        <ui:Label text="EXPENSES" style="font-size: 14px; color: #64748B; -unity-font-style: bold; margin-right: 5px;"/>
                        <ui:Label name="ExpenseText" text="-$0.00" style="font-size: 16px; color: #DC2626; -unity-font-style: bold; margin: 0;"/>
                    </ui:VisualElement>
                </ui:VisualElement>
            </ui:VisualElement>"""

for f in glob.glob("Assets/UI/*UI.uxml"):
    with open(f, 'r') as file:
        content = file.read()
    
    # Regex to match the existing STYLE 0 block
    pattern = re.compile(r'            <!-- STYLE 0: ID CARD -->.*?            </ui:VisualElement>', re.DOTALL)
    
    new_content = pattern.sub(replacement, content, count=1)
    
    with open(f, 'w') as file:
        file.write(new_content)

