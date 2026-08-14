import sys

# 1. Update UXML
with open("Assets/UI/MiniGame.uxml", "r") as f:
    uxml = f.read()

# Make eyelid fully black
uxml = uxml.replace("background-color: rgba(0, 0, 0, 0.95);", "background-color: #000000;")
# Add timer to NightShift
ns_target = '<!-- The Background UI (Variables to align) -->'
ns_replacement = """<!-- Timer Header -->
                    <ui:VisualElement style="position: absolute; top: 10px; right: 10px; background-color: #1F2937; padding: 5px 10px; border-radius: 5px; z-index: 10;">
                        <ui:Label name="NsTimerText" text="15.0s" style="color: white; font-size: 16px; -unity-font-style: bold;" />
                    </ui:VisualElement>
                    """ + ns_target
uxml = uxml.replace(ns_target, ns_replacement)

# Update YesMan prompt
uxml = uxml.replace("⬆ SWIPE UP TO NOD", "↕ SWIPE UP & DOWN TO NOD")

with open("Assets/UI/MiniGame.uxml", "w") as f:
    f.write(uxml)


# 2. Update C#
with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "r") as f:
    cs = f.read()

# Add new fields
fields_target = "    private bool ymSwipedUp = false;"
fields_new = """    private bool ymSwipedUp = false;
    private int ymNodPhase = 0;
    private Label nsTimerText;"""
cs = cs.replace(fields_target, fields_new)

# Bind new field
bind_target = "        nsEyelid = root.Q<VisualElement>(\"NsEyelid\");"
bind_new = bind_target + '\n        nsTimerText = root.Q<Label>("NsTimerText");'
cs = cs.replace(bind_target, bind_new)

# Update Swipe Logic
swipe_old = """        if (ymSwipeZone != null) {
            ymSwipeZone.RegisterCallback<PointerDownEvent>(ev => {
                if (!isPlaying || currentConfig.type != GameType.YesManNod) return;
                ymSwipeStart = ev.position;
            });
            ymSwipeZone.RegisterCallback<PointerUpEvent>(ev => {
                if (!isPlaying || currentConfig.type != GameType.YesManNod) return;
                float deltaY = ev.position.y - ymSwipeStart.y;
                // Swipe UP means negative delta Y in UI Toolkit
                if (deltaY < -40f) {
                    ymSwipedUp = true;
                } else if (deltaY > 40f) {
                    // Swiped down - shook head (Fail immediately)
                    AbortGame();
                }
            });
        }"""
swipe_new = """        if (ymSwipeZone != null) {
            ymSwipeZone.RegisterCallback<PointerDownEvent>(ev => {
                if (!isPlaying || currentConfig.type != GameType.YesManNod) return;
                ymSwipeStart = ev.position;
                ymNodPhase = 0;
            });
            ymSwipeZone.RegisterCallback<PointerMoveEvent>(ev => {
                if (!isPlaying || currentConfig.type != GameType.YesManNod) return;
                if (ev.pressedButtons != 0) {
                    float deltaY = ev.position.y - ymSwipeStart.y;
                    if (ymNodPhase == 0 && deltaY < -40f) {
                        ymNodPhase = 1; // Swiped up
                        ymSwipeStart = ev.position; // reset to look for downward motion
                    } else if (ymNodPhase == 1 && deltaY > 40f) {
                        ymNodPhase = 2; // Swiped down
                        ymSwipedUp = true; // Complete nod!
                    }
                }
            });
        }"""
cs = cs.replace(swipe_old, swipe_new)


# Update NightShift timer display
ns_timer_old = "            nsEyelidGravity += 0.20f * Time.deltaTime;"
ns_timer_new = """            if (nsTimerText != null) {
                nsTimerText.text = $"{timer:F1}s";
                nsTimerText.style.color = timer <= 3f ? Color.red : Color.white;
            }
            nsEyelidGravity += 0.20f * Time.deltaTime;"""
cs = cs.replace(ns_timer_old, ns_timer_new)


# Also increase the time given to react to YesMan, since a full up/down swipe takes longer than just up
# In YesManRoutine:
react_old = "            float reactTimer = 1.2f;"
react_new = "            float reactTimer = 1.8f; // increased because nodding takes longer"
cs = cs.replace(react_old, react_new)


with open("Assets/Scripts/2 Managers/MiniGameManager.cs", "w") as f:
    f.write(cs)

print("Fixed tweaks")
