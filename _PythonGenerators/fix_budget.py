import sys

with open("Assets/UI/MiniGame.uxml", "r") as f:
    content = f.read()

start_marker = '<!-- GAME TYPE 32: Document Signer -->'
end_marker = '<!-- GAME TYPE 33: Chart Adjuster -->'

idx_start = content.find(start_marker)
idx_end = content.find(end_marker)

if idx_start == -1 or idx_end == -1:
    print("Markers not found")
    sys.exit(1)

pre_content = content[:idx_start]
post_content = content[idx_end:]

# Generate the massive text block
lorem = "The party of the first part shall henceforth be known as the party of the first part, in perpetuity, throughout the known universe. By agreeing to these terms, you concede all rights to your genetic material, digital footprint, and weekend free time. Furthermore, any attempts to understand this document will be deemed a violation of corporate policy. " * 5

labels = ""
for i in range(150):
    labels += f'                            <ui:Label text="{lorem}" style="font-size: 20px; color: #64748B; white-space: normal; margin-bottom: 20px;" />\n'

new_xml = f"""                <!-- GAME TYPE 32: Document Signer -->
                <ui:VisualElement name="DocumentSignerContainer" style="display: none; width: 100%; flex-direction: column; background-color: #F8F9FA; padding: 20px; border-radius: 10px; border-width: 4px; border-color: #CBD5E1; margin-bottom: 20px;">
                    <ui:VisualElement style="flex-direction: row; justify-content: space-between; align-items: center; margin-bottom: 10px;">
                        <ui:Label text="Q3 BUDGET PROPOSAL" style="font-size: 28px; color: #1E293B; -unity-font-style: bold;" />
                        <ui:Label name="DsTimerText" text="12.0s" style="font-size: 28px; color: #E74C3C; -unity-font-style: bold;" />
                    </ui:VisualElement>
                    
                    <ui:Label text="⬇ FAST SCROLL TO BOTTOM ⬇" style="font-size: 22px; color: #DC2626; -unity-font-style: bold; -unity-text-align: middle-center; margin-bottom: 5px;" />
                    
                    <ui:ScrollView name="DsScrollView" mode="Vertical" style="flex-grow: 1; height: 500px; background-color: white; border-width: 2px; border-color: #94A3B8; padding: 15px;">
                        <!-- Container to hold all the text -->
                        <ui:VisualElement style="height: 40000px; justify-content: flex-start;">
                            
                            <ui:Label text="CONFIDENTIAL BUDGETARY PROPOSAL V4.2.19\\n\\nSECTION 1: OVERVIEW\\nThe purpose of this document is to outline the excessive and largely unjustified expenditure planned for the upcoming fiscal quarter.\\n\\nSECTION 2: ALLOCATIONS\\n- Research &amp; Development: $4.2B\\n- Marketing: $1.1B\\n- Executive Bonuses: $12.5B\\n- Employee Healthcare: $500\\n- Office Snacks: $12M (Caviar only)\\n\\n" style="font-size: 20px; color: #64748B; white-space: normal; margin-bottom: 30px;" />
                            
{labels}
                            <ui:VisualElement style="flex-grow: 1;"></ui:VisualElement>

                            <!-- Bottom Signature Area -->
                            <ui:VisualElement style="align-items: center; margin-bottom: 50px; margin-top: 50px;">
                                <ui:Label text="x_________________________________" style="font-size: 30px; color: #000000; margin-bottom: 20px;" />
                                <ui:Button name="BtnStampApproval" text="STAMP APPROVAL" class="popup-button" style="background-color: #DC2626; color: white; font-size: 36px; -unity-font-style: bold; padding: 20px 40px; border-radius: 10px;" />
                            </ui:VisualElement>
                        </ui:VisualElement>
                    </ui:ScrollView>
                </ui:VisualElement>

"""

new_content = pre_content + new_xml + post_content

with open("Assets/UI/MiniGame.uxml", "w") as f:
    f.write(new_content)

print("Replaced!")
