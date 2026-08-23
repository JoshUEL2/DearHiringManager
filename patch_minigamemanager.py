import re

path = 'Assets/Scripts/2 Managers/MiniGameManager.cs'
with open(path, 'r') as f:
    content = f.read()

target = r'AddConfigFull\("Send weekly TPS reports", new MiniGameConfig \{ type = GameType.CodeBreaker, codeSymbols = new string\[\]\{"A","B","C","D","E","F"\}, codeLength = 4, maxAttempts = 6, instructions = "Crack the system password to send reports!"\}\);'
replacement = r'AddConfigFull("Access payroll database", new MiniGameConfig { type = GameType.CodeBreaker, codeLength = 4, maxAttempts = 6, instructions = "Crack the system password to access database!"});'

content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)
