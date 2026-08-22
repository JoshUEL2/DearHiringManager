import re

with open("ConsoleErrors.txt", "w") as f:
    f.write("") # Clear file just in case, but actually I don't have it

vars_to_remove = [
    "isCheckingMatch", "ipHackShuffleTimer", "flippedIndex1", "sprintObstacleIsHigh",
    "timingMarkerAngle", "sortScore", "balanceValue", "sprintDistance", "vcWiggleReady",
    "balanceDrift", "timingMarkerMoving", "vcCurrentPhase", "sequenceInputIndex",
    "acidWindTimer", "dialogueRound", "timingTargetAngle", "vcWiggleProgress",
    "kanbanTicketsDone", "dialogueTimer", "timingSpinDir", "draggedTicket",
    "expectedDodgeDir", "fileSorterTargetDirection", "ipHackCurrentProgress",
    "fileSorterSortedCount", "isDecoy", "acidSignAngle", "dialogueTimerMax",
    "acidDropTimer", "vcWiggleWaitTimer", "sprintWaitingForInput", "isShowingSequence",
    "acidWindAngle", "orderIsPreview", "orderInputIndex", "flippedIndex2", "whackScore",
    "dragSelectedLeft", "acidWindTarget", "isDraggingFileSorter", "opponentProgress",
    "dragMatchCount", "playerMashProgress", "acidBurn", "timingRingRotSpeed",
    "balanceTimeInZone", "timer", "activeWhackCell", "rapportValue", "hasWon", "currentSequence", "activeTetrominoBlocks", "activeAcidDrops", "memoryCards", "memoryValues"
]

with open('Assets/Scripts/2 Managers/MiniGameManager_Variables.cs', 'r') as f:
    lines = f.readlines()

new_lines = []
for line in lines:
    keep = True
    for v in vars_to_remove:
        # Match word boundaries so we don't accidentally match part of another var
        if re.search(r'\b' + v + r'\b', line) and ("=" in line or ";" in line):
            keep = False
            break
    if keep:
        new_lines.append(line)

with open('Assets/Scripts/2 Managers/MiniGameManager_Variables.cs', 'w') as f:
    f.writelines(new_lines)
