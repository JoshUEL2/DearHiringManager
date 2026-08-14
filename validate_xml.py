import os
import glob
import xml.etree.ElementTree as ET

for f in glob.glob("Assets/UI/MiniGames/*.uxml"):
    try:
        ET.parse(f)
    except Exception as e:
        print(f"Error in {f}: {e}")

try:
    ET.parse("Assets/UI/MiniGame.uxml")
except Exception as e:
    print(f"Error in MiniGame.uxml: {e}")

