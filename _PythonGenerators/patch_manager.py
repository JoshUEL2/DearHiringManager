import sys

def patch_file(target, patch_src, start_idx, end_idx):
    with open(target, "r") as f:
        lines = f.readlines()
    with open(patch_src, "r") as f:
        patch_lines = f.readlines()
    
    # Optional: strip the first line if it's a comment we don't need
    if patch_lines[0].startswith("// 1. CloggedVents"):
        patch_lines = patch_lines[1:]

    new_lines = lines[:start_idx] + patch_lines + lines[end_idx:]
    with open(target, "w") as f:
        f.writelines(new_lines)

patch_file(
    "/Users/josh/Dear Hiring Manager/Assets/Scripts/2 Managers/MiniGameManager.cs",
    "/Users/josh/.gemini/antigravity/brain/b9616fa6-bed2-43e8-bab6-daf002e207ad/scratch/BatchB1.cs",
    7521, 
    7903
)
