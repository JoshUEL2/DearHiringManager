import sys

lorem = "The party of the first part shall henceforth be known as the party of the first part, in perpetuity, throughout the known universe. By agreeing to these terms, you concede all rights to your genetic material, digital footprint, and weekend free time. " * 50

xml = f"""                <!-- GAME TYPE 32: Document Signer -->
                <ui:VisualElement name="DocumentSignerContainer" style="display: none; width: 100%; flex-direction: column; background-color: #F8F9FA; padding: 20px; border-radius: 10px; border-width: 4px; border-color: #CBD5E1; margin-bottom: 20px;">
                    <ui:VisualElement style="flex-direction: row; justify-content: space-between; align-items: center; margin-bottom: 10px;">
                        <ui:Label text="Q3 BUDGET PROPOSAL" style="font-size: 28px; color: #1E293B; -unity-font-style: bold;" />
                        <ui:Label name="DsTimerText" text="12.0s" style="font-size: 28px; color: #E74C3C; -unity-font-style: bold;" />
                    </ui:VisualElement>
                    
                    <ui:Label text="⬇ FAST SCROLL TO BOTTOM ⬇" style="font-size: 22px; color: #DC2626; -unity-font-style: bold; -unity-text-align: middle-center; margin-bottom: 5px;" />
                    
                    <ui:ScrollView name="DsScrollView" mode="Vertical" style="flex-grow: 1; height: 500px; background-color: white; border-width: 2px; border-color: #94A3B8; padding: 15px;">
                        <!-- Very tall dummy element to force scrolling -->
                        <ui:VisualElement style="height: 80000px; justify-content: space-between;">
                            <ui:Label text="CONFIDENTIAL BUDGETARY PROPOSAL V4.2.19\\n\\nSECTION 1: OVERVIEW\\n{lorem}\\n\\nSECTION 2: ALLOCATIONS\\n- Research &amp; Development: $4.2B\\n- Marketing: $1.1B\\n- Executive Bonuses: $12.5B\\n- Employee Healthcare: $500\\n- Office Snacks: $12M (Caviar only)\\n\\n[SCROLL TO BOTTOM TO APPROVE]" style="font-size: 20px; color: #64748B; white-space: normal;" />
                            
                            <ui:Label text="{lorem}\\n\\n{lorem}" style="font-size: 20px; color: #64748B; white-space: normal; position: absolute; top: 15000px;" />
                            <ui:Label text="{lorem}\\n\\n{lorem}" style="font-size: 20px; color: #64748B; white-space: normal; position: absolute; top: 30000px;" />
                            <ui:Label text="{lorem}\\n\\n{lorem}" style="font-size: 20px; color: #64748B; white-space: normal; position: absolute; top: 45000px;" />
                            <ui:Label text="{lorem}\\n\\n{lorem}" style="font-size: 20px; color: #64748B; white-space: normal; position: absolute; top: 60000px;" />

                            <!-- Bottom Signature Area -->
                            <ui:VisualElement style="align-items: center; margin-bottom: 50px;">
                                <ui:Label text="x_________________________________" style="font-size: 30px; color: #000000; margin-bottom: 20px;" />
                                <ui:Button name="BtnStampApproval" text="STAMP APPROVAL" class="popup-button" style="background-color: #DC2626; color: white; font-size: 36px; -unity-font-style: bold; padding: 20px 40px; border-radius: 10px;" />
                            </ui:VisualElement>
                        </ui:VisualElement>
                    </ui:ScrollView>
                </ui:VisualElement>

                <!-- GAME TYPE 33: Chart Adjuster -->
                <ui:VisualElement name="ChartAdjusterContainer" style="display: none; width: 100%; height: 400px; flex-direction: column; background-color: #F1F5F9; padding: 20px; border-radius: 10px; border-width: 4px; border-color: #94A3B8; margin-bottom: 20px;">
                    <ui:VisualElement style="flex-direction: row; justify-content: space-between; align-items: center; margin-bottom: 20px;">
                        <ui:Label name="CaStatusText" text="Q3 REVENUE PROJECTIONS" style="font-size: 24px; color: #1E293B; -unity-font-style: bold;" />
                        <ui:Label name="CaTimerText" text="10.0s" style="font-size: 28px; color: #E74C3C; -unity-font-style: bold;" />
                    </ui:VisualElement>
                    
                    <ui:VisualElement name="CaChartArea" style="flex-grow: 1; flex-direction: row; justify-content: space-around; align-items: flex-end; padding-bottom: 10px; border-bottom-width: 2px; border-color: #64748B;">
                        
                        <!-- Competitor A -->
                        <ui:VisualElement style="width: 20%; height: 100%; flex-direction: column; justify-content: flex-end; align-items: center;">
                            <ui:VisualElement name="CaCompetitorBar" style="width: 100%; height: 80%; background-color: #EF4444; border-top-left-radius: 5px; border-top-right-radius: 5px;" />
                            <ui:Label text="COMP A" style="font-size: 18px; color: #334155; margin-top: 5px; -unity-font-style: bold;" />
                        </ui:VisualElement>

                        <!-- Market Average -->
                        <ui:VisualElement style="width: 20%; height: 100%; flex-direction: column; justify-content: flex-end; align-items: center;">
                            <ui:VisualElement name="CaAverageBar" style="width: 100%; height: 40%; background-color: #94A3B8; border-top-left-radius: 5px; border-top-right-radius: 5px;" />
                            <ui:Label text="AVERAGE" style="font-size: 18px; color: #334155; margin-top: 5px; -unity-font-style: bold;" />
                        </ui:VisualElement>

                        <!-- Our Revenue -->
                        <ui:VisualElement style="width: 20%; height: 100%; flex-direction: column; justify-content: flex-end; align-items: center;">
                            <ui:VisualElement name="CaOurBar" style="width: 100%; height: 20%; background-color: #22C55E; border-top-left-radius: 5px; border-top-right-radius: 5px;" />
                            <ui:Label text="US" style="font-size: 18px; color: #334155; margin-top: 5px; -unity-font-style: bold;" />
                        </ui:VisualElement>

                    </ui:VisualElement>
                    <ui:Label text="Swipe DOWN on Competitor. Swipe UP on Us." style="font-size: 16px; color: #64748B; -unity-text-align: middle-center; margin-top: 10px;" />
                </ui:VisualElement>
"""

with open("replacement.txt", "w") as f:
    f.write(xml)
