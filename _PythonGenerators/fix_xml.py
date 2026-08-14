import sys

with open("Assets/UI/MiniGame.uxml", "r") as f:
    content = f.read()

# Find where DocumentSignerContainer starts
idx = content.find('<!-- GAME TYPE 32: Document Signer -->')
if idx == -1:
    print("Could not find start of DocumentSigner")
    sys.exit(1)

# Keep everything before it
new_content = content[:idx]

# Add our replacement
with open("replacement.txt", "r") as f:
    replacement = f.read()

new_content += replacement

# Add the footer and closing tags
footer = """
            </ui:ScrollView>

            <!-- Footer Buttons -->
            <ui:VisualElement style="margin-top: 20px; align-items: center; justify-content: center;">
                <ui:Button name="BtnAbort" text="Abort Task" style="background-color: #E74C3C; font-size: 26px; color: white; border-radius: 20px; padding: 15px 40px; border-width: 0; -unity-font-style: bold;" />
            </ui:VisualElement>

        </ui:VisualElement>
    </ui:VisualElement>

</ui:UXML>
"""

new_content += footer

with open("Assets/UI/MiniGame.uxml", "w") as f:
    f.write(new_content)

print("Done")
