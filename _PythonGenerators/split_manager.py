import os
import shutil

def split_manager():
    file_path = "Assets/Scripts/2 Managers/MiniGameManager.cs"
    with open(file_path, "r", encoding="utf-8") as f:
        lines = f.readlines()

    types_lines = []
    vars_lines = []
    core_lines = []
    routines_lines = []

    state = "types"
    
    for i, line in enumerate(lines):
        if state == "types":
            if line.strip().startswith("public class MiniGameManager : MonoBehaviour"):
                state = "vars"
                vars_lines.append("using UnityEngine;\nusing UnityEngine.UIElements;\nusing System.Collections;\nusing System.Collections.Generic;\nusing System.Linq;\n\n")
                vars_lines.append("public partial class MiniGameManager : MonoBehaviour\n{\n")
            else:
                types_lines.append(line)
        elif state == "vars":
            if line.strip().startswith("private void Start()"):
                state = "core"
                core_lines.append("using UnityEngine;\nusing UnityEngine.UIElements;\nusing System.Collections;\nusing System.Collections.Generic;\nusing System.Linq;\n\n")
                core_lines.append("public partial class MiniGameManager : MonoBehaviour\n{\n")
                core_lines.append(line)
            else:
                vars_lines.append(line)
        elif state == "core":
            if line.strip().startswith("private IEnumerator FallingSortRoutine()"):
                state = "routines"
                routines_lines.append(line)
            else:
                core_lines.append(line)
        elif state == "routines":
            routines_lines.append(line)
            
    vars_lines.append("}\n")
    core_lines.append("}\n")
    
    # Write types
    with open("Assets/Scripts/2 Managers/MiniGameTypes.cs", "w", encoding="utf-8") as f:
        f.writelines(types_lines)

    # Write vars
    with open("Assets/Scripts/2 Managers/MiniGameManager_Variables.cs", "w", encoding="utf-8") as f:
        f.writelines(vars_lines)

    # Write core
    with open("Assets/Scripts/2 Managers/MiniGameManager_Core.cs", "w", encoding="utf-8") as f:
        f.writelines(core_lines)

    # Split routines into 4 files
    chunk_size = len(routines_lines) // 4
    
    current_chunk = []
    chunk_index = 1
    
    header = "using UnityEngine;\nusing UnityEngine.UIElements;\nusing System.Collections;\nusing System.Collections.Generic;\nusing System.Linq;\n\npublic partial class MiniGameManager : MonoBehaviour\n{\n"
    
    def write_chunk(lines_to_write, index, is_last=False):
        if not lines_to_write: return
        with open(f"Assets/Scripts/2 Managers/MiniGameManager_Routines{index}.cs", "w", encoding="utf-8") as f:
            f.write(header)
            f.writelines(lines_to_write)
            if not is_last:
                f.write("}\n")

    lines_accum = []
    for line in routines_lines:
        lines_accum.append(line)
        if len(lines_accum) > chunk_size and line.startswith("    private ") and ("Routine(" in line or "IEnumerator" in line):
            write_chunk(lines_accum[:-1], chunk_index)
            chunk_index += 1
            lines_accum = [line]

    write_chunk(lines_accum, chunk_index, is_last=True)
    
    print(f"Split completed. Types: {len(types_lines)}, Vars: {len(vars_lines)}, Core: {len(core_lines)}, Routines: {len(routines_lines)}")

split_manager()
