import os
import glob

replacements = {
    "Executive Interstellar CEO": "Fortune 500 CEO",
    "Interstellar Overlord Victory": "Global Overlord Victory",
    "Interstellar colony site inspection": "Offshore tax-haven site inspection",
    "planetary boundaries": "international borders",
    "Galactic Chairman": "Global Chairman",
    "three star systems": "three continents",
    "AI Prophecy Alignment Director": "Chief AI Prompt Engineer",
    "ethical containment and reward-function alignment of hyper-intelligent neural networks, preventing multiple extinction-level events": "ethical alignment of large language models, preventing the company from being canceled on social media",
    "Prime Arbiter of Machine Will, successfully surviving three consecutive rogue-AI assassination attempts": "VP of Generative Synergy, successfully surviving three consecutive AI-driven mass layoff rounds",
    "Capsule Arcade VR": "Vintage Retro Arcade",
    "virtual retro classics": "vintage 8-bit classics",
    "Luxury Cryo Therapy": "Luxury Ice Bath Therapy",
    "zero-K chambers": "freezing cold tubs",
    "Under-slum pipe threading": "Underground pipe threading",
    "Interstellar CEO candidacy": "Fortune 500 CEO candidacy",
    "Corporate Cubicle Drone": "Data Entry Drone",
    "Master of Arts in corporate warfare": "Master of Arts in corporate synergy",
    "Doctoral chemistry of volatile coffee-bean aromatic profiles": "Doctoral chemistry of modern artisanal coffee bean roasting",
    "Corporate Golf Sim": "Corporate Golf Outing",
    "Suburban Sensory Mud Spa": "Suburban Detox Mud Spa",
    "Interstellar Cryo Resort": "Luxury Ice Bath Resort",
    "Capsule Arcade": "Retro Arcade",
}

for root, _, files in os.walk('Assets/Scripts/'):
    for file in files:
        if file.endswith('.cs'):
            filepath = os.path.join(root, file)
            with open(filepath, 'r') as f:
                content = f.read()
            
            original_content = content
            for k, v in replacements.items():
                content = content.replace(k, v)
                
            if content != original_content:
                with open(filepath, 'w') as f:
                    f.write(content)
                print(f"Patched {filepath}")
