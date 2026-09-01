import os

replacement = """        <!-- TOP CARD ROOT -->
        <ui:VisualElement class="card top-card-root" style="margin-bottom: 20px;">
            
            <!-- STYLE 0: ID CARD -->
            <ui:VisualElement name="HUD-IDCard" style="display: none; background-color: #E2E8F0; border-radius: 12px; border-width: 4px; border-color: #94A3B8; padding: 15px; flex-direction: row;">
                <ui:VisualElement style="width: 120px; height: 120px; background-color: #CBD5E1; border-width: 2px; border-color: #64748B; border-radius: 8px; justify-content: center; align-items: center; margin-right: 20px;">
                    <ui:Label name="PlayerInitials" text="J" style="font-size: 60px; color: #475569; -unity-font-style: bold; margin: 0;"/>
                </ui:VisualElement>
                <ui:VisualElement style="flex-grow: 1; flex-direction: column; justify-content: space-around;">
                    <ui:VisualElement style="flex-direction: row; justify-content: space-between;">
                        <ui:Label text="NAME" style="font-size: 16px; color: #64748B; -unity-font-style: bold;"/>
                        <ui:Label name="IDCardWeek" text="VALID: WK 1" style="font-size: 16px; color: #E74C3C; -unity-font-style: bold;"/>
                    </ui:VisualElement>
                    <ui:Label name="PlayerNameText" text="PlayerName" style="font-size: 32px; color: #0F172A; -unity-font-style: bold; margin: -5px 0 5px 0;"/>
                    <ui:VisualElement style="height: 2px; background-color: #94A3B8; margin-bottom: 5px;"/>
                    <ui:VisualElement style="flex-direction: row; justify-content: space-between;">
                        <ui:VisualElement>
                            <ui:Label text="OCCUPATION" style="font-size: 14px; color: #64748B; -unity-font-style: bold;"/>
                            <ui:Label name="JobText" text="Unemployed" style="font-size: 20px; color: #0F172A; -unity-font-style: bold;"/>
                        </ui:VisualElement>
                        <ui:VisualElement>
                            <ui:Label text="BALANCE" style="font-size: 14px; color: #64748B; -unity-font-style: bold;"/>
                            <ui:Label name="CashText" text="$0.00" style="font-size: 20px; color: #16A34A; -unity-font-style: bold;"/>
                        </ui:VisualElement>
                    </ui:VisualElement>
                </ui:VisualElement>
                <ui:VisualElement style="justify-content: space-between; align-items: flex-end; margin-left: 10px;">
                    <ui:Button name="BtnOpenSettings" text="≡" style="font-size: 40px; color: #64748B; background-color: transparent; border-width: 0; padding: 0; margin: 0;"/>
                    <ui:Button name="BtnViewCV" text="CV" style="font-size: 20px; color: white; background-color: #38BDF8; border-radius: 6px; border-width: 0; padding: 5px 10px; margin: 0; -unity-font-style: bold;"/>
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
                    <ui:Label name="CashText" text="$0.00" style="font-size: 48px; color: #39FF14; -unity-font-style: bold; text-shadow: 0 0 10px #39FF14; margin: 0;"/>
                    <ui:VisualElement style="align-items: flex-end;">
                        <ui:Label text="EXP/WK" style="font-size: 14px; color: #FF073A; -unity-font-style: bold; margin: 0;"/>
                        <ui:Label name="ExpenseText" text="-$0.00" style="font-size: 24px; color: #FF073A; -unity-font-style: bold; margin: 0; text-shadow: 0 0 5px #FF073A;"/>
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

import sys

files = [
    "Assets/UI/DashboardUI.uxml",
    "Assets/UI/CareersUI.uxml",
    "Assets/UI/HustlesUI.uxml",
    "Assets/UI/EducationUI.uxml",
    "Assets/UI/ActivitiesUI.uxml"
]

for f in files:
    with open(f, 'r') as file:
        content = file.read()
    
    # We will find <!-- TOP CARD --> and the following </ui:VisualElement> that matches its level.
    # Since we know the previous replacement perfectly, we can just search for:
    # "        <!-- TOP CARD -->" up to "                </ui:VisualElement>\n            </ui:VisualElement>\n        </ui:VisualElement>"
    
    start_tag = "        <!-- TOP CARD -->"
    # Actually, we can split by "        <!-- TOP CARD -->" and "        </ui:VisualElement>\n" + some condition
    # Even simpler: use regex
    import re
    # Match from "        <!-- TOP CARD -->" until the 3rd </ui:VisualElement> which ends the top-card if we use non-greedy.
    # But it's easier to just match from "<!-- TOP CARD -->" up to the end of the top-card block which is right before <ui:Label name="DashboardTitle" or <ui:VisualElement name="DynamicMiddleWrapper"
    
    pattern = re.compile(r'        <!-- TOP CARD -->.*?        </ui:VisualElement>', re.DOTALL)
    
    # Wait, the top-card has multiple nested </ui:VisualElement>. Regex with dotall will stop at the first one if non-greedy, or last if greedy.
    # Let's use the explicit string of the last replacement!
    
    old_block = """        <!-- TOP CARD -->
        <ui:VisualElement class="card top-card" style="padding: 20px; border-radius: 16px; margin-bottom: 20px;">
            <!-- Header Row -->
            <ui:VisualElement style="flex-direction: row; justify-content: space-between; align-items: center; margin-bottom: 15px;">
                <ui:VisualElement style="flex-direction: row; align-items: center; flex-shrink: 1;">
                    <ui:Label name="PlayerNameText" text="PlayerName" style="color: #F8FAFC; font-size: 40px; -unity-font-style: bold; margin: 0; margin-right: 15px; white-space: nowrap;" />
                    <ui:VisualElement style="background-color: rgba(56, 189, 248, 0.15); border-color: #38BDF8; border-width: 2px; border-radius: 10px; padding: 4px 12px;">
                        <ui:Label name="JobText" text="Unemployed" style="color: #38BDF8; font-size: 24px; -unity-font-style: bold; margin: 0; white-space: nowrap;" />
                    </ui:VisualElement>
                </ui:VisualElement>
                
                <ui:VisualElement style="flex-direction: row; align-items: center;">
                    <ui:Button name="BtnViewCV" text="VIEW CV" style="background-color: #38BDF8; color: #0F172A; -unity-font-style: bold; padding: 10px 20px; font-size: 24px; border-radius: 12px; border-width: 0; margin: 0; margin-right: 15px; transition-property: scale, background-color; transition-duration: 0.15s;" />
                    <ui:Button name="BtnOpenSettings" text="≡" style="background-color: transparent; border-width: 0; color: #94A3B8; font-size: 60px; padding: 0; margin: 0; transition-property: color; transition-duration: 0.2s;" />
                </ui:VisualElement>
            </ui:VisualElement>
            
            <!-- Stats Row -->
            <ui:VisualElement style="flex-direction: row; justify-content: space-between; align-items: center; background-color: rgba(15, 23, 42, 0.4); border-radius: 12px; padding: 15px 20px;">
                <ui:VisualElement name="ColCash" style="flex-direction: column; align-items: flex-start; width: 33%;">
                    <ui:Label text="CASH RESERVE" style="font-size: 18px; color: #94A3B8; -unity-font-style: bold; margin-bottom: 2px;"/>
                    <ui:Label name="CashText" text="$0.00" style="font-size: 28px; -unity-font-style: bold; color: #4ADE80; margin: 0;"/>
                </ui:VisualElement>
                
                <ui:VisualElement name="ColExpenses" style="flex-direction: column; align-items: flex-start; width: 33%;">
                    <ui:Label text="EXPENSES/WK" style="font-size: 18px; color: #94A3B8; -unity-font-style: bold; margin-bottom: 2px;"/>
                    <ui:Label name="ExpenseText" text="-$0.00" style="font-size: 28px; -unity-font-style: bold; color: #F87171; margin: 0;"/>
                </ui:VisualElement>
                
                <ui:VisualElement name="ColDistrict" style="flex-direction: column; align-items: flex-start; width: 33%;">
                    <ui:Label text="HOME DISTRICT" style="font-size: 18px; color: #94A3B8; -unity-font-style: bold; margin-bottom: 2px;"/>
                    <ui:VisualElement style="flex-direction: row; justify-content: space-between; width: 100%; align-items: baseline;">
                        <ui:Label name="DistrictText" text="Slums" style="font-size: 28px; -unity-font-style: bold; color: #FBBF24; margin: 0;"/>
                        <ui:Label name="SubTitleText" text="WEEK 1" style="font-size: 20px; color: #64748B; -unity-font-style: bold; margin: 0;" />
                    </ui:VisualElement>
                </ui:VisualElement>
            </ui:VisualElement>
        </ui:VisualElement>"""

    new_content = content.replace(old_block, replacement)
    
    with open(f, 'w') as file:
        file.write(new_content)
    
    if content != new_content:
        print(f"Successfully replaced in {f}")
    else:
        print(f"FAILED to replace in {f}")

