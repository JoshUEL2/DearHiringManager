import re

with open("Assets/UI/DashboardUI.uxml", "r") as file:
    content = file.read()

# I will find everything from <!-- TOP CARD ROOT --> up to <ui:Label name="DashboardTitle"
# and replace it with full_top_card.

full_top_card = """        <!-- TOP CARD ROOT -->
        <ui:VisualElement class="card top-card-root" style="margin-bottom: 20px;">
            
            <!-- STYLE 0: ID CARD -->
            <ui:VisualElement name="HUD-IDCard" style="display: none; background-color: #E2E8F0; border-radius: 12px; border-width: 4px; border-color: #94A3B8; padding: 10px 20px; flex-direction: row; align-items: center; justify-content: space-between;">
                <!-- FAR LEFT: Settings -->
                <ui:Button name="BtnOpenSettings" text="≡" style="font-size: 45px; color: #475569; background-color: transparent; border-width: 0; padding: 0; margin: 0; margin-right: 15px;"/>
                
                <!-- NAME (Clickable -> View CV) -->
                <ui:Button name="BtnViewCV" class="id-card-name-btn" text="PlayerName" style="font-size: 32px; color: #0F172A; -unity-font-style: bold; background-color: transparent; border-width: 0; padding: 0; margin: 0; flex-shrink: 1;"/>
                
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
            </ui:VisualElement>

            <!-- STYLE 1: SMARTPHONE -->
            <ui:VisualElement name="HUD-Phone" style="display: none; background-color: rgba(15, 23, 42, 0.95); border-radius: 30px; border-width: 2px; border-color: #334155; padding: 20px 25px;">
                <ui:VisualElement style="flex-direction: row; justify-content: space-between; margin-bottom: 15px;">
                    <ui:Label name="DistrictSignal" text="📶 SLUMS - 3G" style="font-size: 20px; color: #94A3B8; -unity-font-style: bold; margin: 0;"/>
                    <ui:Label name="CashText" text="$0.00 🔋" style="font-size: 20px; color: #4ADE80; -unity-font-style: bold; margin: 0;"/>
                </ui:VisualElement>
                <ui:Label name="PhoneTime" text="WEEK 1" style="font-size: 48px; color: #F8FAFC; -unity-font-style: bold; -unity-text-align: middle-center; margin-bottom: 5px;"/>
                <ui:Label name="PlayerNameText" text="PlayerName" style="font-size: 22px; color: #94A3B8; -unity-text-align: middle-center; margin-bottom: 20px; -unity-font-style: bold;"/>
                
                <ui:VisualElement style="flex-direction: row; justify-content: space-around; background-color: rgba(255, 255, 255, 0.05); border-radius: 15px; padding: 15px;">
                    <ui:VisualElement style="align-items: center;">
                        <ui:Label text="💼" style="font-size: 32px; margin: 0;"/>
                        <ui:Label name="JobText" text="Unemployed" style="font-size: 16px; color: #38BDF8; margin-top: 5px; -unity-font-style: bold;"/>
                    </ui:VisualElement>
                    <ui:VisualElement style="align-items: center;">
                        <ui:Label text="💸" style="font-size: 32px; margin: 0;"/>
                        <ui:Label name="ExpenseText" text="-$0.00" style="font-size: 16px; color: #F87171; margin-top: 5px; -unity-font-style: bold;"/>
                    </ui:VisualElement>
                    <ui:VisualElement style="align-items: center;">
                        <ui:Button name="BtnViewCV" text="📄" style="font-size: 32px; background-color: transparent; border-width: 0; padding: 0; margin: 0;"/>
                        <ui:Label text="View CV" style="font-size: 16px; color: #FBBF24; margin-top: 5px; -unity-font-style: bold;"/>
                    </ui:VisualElement>
                    <ui:VisualElement style="align-items: center;">
                        <ui:Button name="BtnOpenSettings" text="⚙️" style="font-size: 32px; background-color: transparent; border-width: 0; padding: 0; margin: 0;"/>
                        <ui:Label text="Config" style="font-size: 16px; color: #94A3B8; margin-top: 5px; -unity-font-style: bold;"/>
                    </ui:VisualElement>
                </ui:VisualElement>
            </ui:VisualElement>

            <!-- STYLE 2: LEDGER -->
            <ui:VisualElement name="HUD-Ledger" style="display: none; background-color: #FDF6E3; border-radius: 4px; border-width: 1px; border-color: #D4D4D8; padding: 15px; flex-direction: column;">
                <ui:VisualElement style="flex-direction: row; justify-content: space-between; border-bottom-width: 2px; border-color: #EF4444; margin-bottom: 10px; padding-bottom: 5px;">
                    <ui:Label name="PlayerNameText" text="ACCOUNT: PlayerName" style="font-size: 24px; color: #7F1D1D; -unity-font-style: bold; margin: 0;"/>
                    <ui:Label name="LedgerDate" text="DATE: WEEK 1" style="font-size: 24px; color: #1E3A8A; -unity-font-style: bold; margin: 0;"/>
                </ui:VisualElement>
                
                <ui:VisualElement style="flex-direction: row; justify-content: space-between;">
                    <ui:VisualElement style="flex-grow: 1; border-right-width: 2px; border-color: #9CA3AF; padding-right: 15px; margin-right: 15px;">
                        <ui:VisualElement style="flex-direction: row; justify-content: space-between; border-bottom-width: 1px; border-color: #D1D5DB; margin-bottom: 5px; padding-bottom: 2px;">
                            <ui:Label text="DESC" style="font-size: 16px; color: #4B5563; -unity-font-style: bold; margin: 0;"/>
                            <ui:Label text="AMOUNT" style="font-size: 16px; color: #4B5563; -unity-font-style: bold; margin: 0;"/>
                        </ui:VisualElement>
                        <ui:VisualElement style="flex-direction: row; justify-content: space-between; margin-bottom: 5px;">
                            <ui:Label text="Current Balance" style="font-size: 20px; color: #111827; margin: 0;"/>
                            <ui:Label name="CashText" text="$0.00" style="font-size: 20px; color: #16A34A; -unity-font-style: bold; margin: 0;"/>
                        </ui:VisualElement>
                        <ui:VisualElement style="flex-direction: row; justify-content: space-between;">
                            <ui:Label text="Est. Expenses" style="font-size: 20px; color: #111827; margin: 0;"/>
                            <ui:Label name="ExpenseText" text="-$0.00" style="font-size: 20px; color: #DC2626; -unity-font-style: bold; margin: 0;"/>
                        </ui:VisualElement>
                    </ui:VisualElement>
                    
                    <ui:VisualElement style="flex-grow: 1;">
                        <ui:VisualElement style="flex-direction: row; justify-content: space-between; border-bottom-width: 1px; border-color: #D1D5DB; margin-bottom: 5px; padding-bottom: 2px;">
                            <ui:Label text="NOTES" style="font-size: 16px; color: #4B5563; -unity-font-style: bold; margin: 0;"/>
                        </ui:VisualElement>
                        <ui:Label name="JobText" text="Unemployed" style="font-size: 20px; color: #111827; margin: 0; margin-bottom: 5px; -unity-font-style: bold;"/>
                        <ui:Label name="DistrictText" text="Slums" style="font-size: 20px; color: #111827; margin: 0; margin-bottom: 5px; -unity-font-style: bold;"/>
                    </ui:VisualElement>
                    
                    <ui:VisualElement style="justify-content: space-between; padding-left: 15px; border-left-width: 2px; border-color: #9CA3AF;">
                        <ui:Button name="BtnOpenSettings" text="≡" style="font-size: 32px; color: #111827; background-color: transparent; border-width: 0; padding: 0; margin: 0;"/>
                        <ui:Button name="BtnViewCV" text="CV" style="font-size: 20px; color: #1E3A8A; background-color: transparent; border-width: 2px; border-color: #1E3A8A; border-radius: 4px; padding: 2px 8px; margin: 0; -unity-font-style: bold;"/>
                    </ui:VisualElement>
                </ui:VisualElement>
            </ui:VisualElement>

            <!-- STYLE 3: DIGITAL TICKER -->
            <ui:VisualElement name="HUD-Ticker" style="display: none; background-color: #000000; border-radius: 8px; border-width: 6px; border-color: #333333; padding: 15px; flex-direction: column;">
                <ui:VisualElement style="flex-direction: row; justify-content: space-between; margin-bottom: 10px;">
                    <ui:Label text="SYS.ON // LIFESIM_OS" style="font-size: 14px; color: #39FF14; -unity-font-style: bold; letter-spacing: 2px; margin: 0;"/>
                    <ui:Label name="TickerWeek" text="WK:01" style="font-size: 14px; color: #39FF14; -unity-font-style: bold; letter-spacing: 2px; margin: 0;"/>
                </ui:VisualElement>
                
                <ui:VisualElement style="flex-direction: row; justify-content: space-between; align-items: center; margin-bottom: 15px; background-color: #0A0A0A; padding: 10px; border-width: 1px; border-color: #39FF14;">
                    <ui:Label name="CashText" text="$0.00" style="font-size: 48px; color: #39FF14; -unity-font-style: bold; margin: 0;"/>
                    <ui:VisualElement style="align-items: flex-end;">
                        <ui:Label text="EXP/WK" style="font-size: 14px; color: #FF073A; -unity-font-style: bold; margin: 0;"/>
                        <ui:Label name="ExpenseText" text="-$0.00" style="font-size: 24px; color: #FF073A; -unity-font-style: bold; margin: 0;"/>
                    </ui:VisualElement>
                </ui:VisualElement>
                
                <ui:VisualElement style="flex-direction: row; justify-content: space-between; align-items: center;">
                    <ui:VisualElement style="flex-direction: row;">
                        <ui:Label name="PlayerNameText" text="P.NAME" style="font-size: 18px; color: #FFB000; -unity-font-style: bold; margin: 0; margin-right: 15px;"/>
                        <ui:Label name="JobText" text="UNEMPLOYED" style="font-size: 18px; color: #0FF0FC; -unity-font-style: bold; margin: 0; margin-right: 15px;"/>
                        <ui:Label name="DistrictText" text="SLUMS" style="font-size: 18px; color: #FF00FF; -unity-font-style: bold; margin: 0;"/>
                    </ui:VisualElement>
                    <ui:VisualElement style="flex-direction: row;">
                        <ui:Button name="BtnViewCV" text="[CV]" style="font-size: 18px; color: #0FF0FC; background-color: transparent; border-width: 0; padding: 0; margin: 0; margin-right: 15px; -unity-font-style: bold;"/>
                        <ui:Button name="BtnOpenSettings" text="[OPT]" style="font-size: 18px; color: #39FF14; background-color: transparent; border-width: 0; padding: 0; margin: 0; -unity-font-style: bold;"/>
                    </ui:VisualElement>
                </ui:VisualElement>
            </ui:VisualElement>

        </ui:VisualElement>"""

pattern = re.compile(r'        <!-- TOP CARD ROOT -->.*?        <ui:Label name="DashboardTitle"', re.DOTALL)

if pattern.search(content):
    new_content = pattern.sub(full_top_card + '\n\n        <ui:Label name="DashboardTitle"', content, count=1)
    with open("Assets/UI/DashboardUI.uxml", "w") as file:
        file.write(new_content)
    print("Fixed DashboardUI.uxml")
else:
    print("Could not find pattern in DashboardUI.uxml")
