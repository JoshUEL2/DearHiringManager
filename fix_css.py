import glob

for f in glob.glob("Assets/UI/*UI.uxml"):
    with open(f, 'r') as file:
        content = file.read()
    
    # Remove text-shadow
    content = content.replace('text-shadow: 0 0 10px #39FF14;', '')
    content = content.replace('text-shadow: 0 0 5px #FF073A;', '')
    
    # Remove transition-property: scale
    content = content.replace('transition-property: scale; transition-duration: 0.1s;', '')
    
    with open(f, 'w') as file:
        file.write(content)
        
