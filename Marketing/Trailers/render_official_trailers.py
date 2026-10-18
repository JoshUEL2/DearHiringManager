import os
import subprocess
import glob

MARKETING_DIR = r"C:\Users\Weari\Documents\Dear Hiring Manager PC\Marketing\Trailers"
AUDIO_DIR = r"C:\Users\Weari\Documents\Dear Hiring Manager PC\Assets\Resources\Audio"
FINAL_OUTPUT_16_9 = os.path.join(MARKETING_DIR, "Official_Gameplay_Trailer_16x9.mp4")
FINAL_OUTPUT_9_16 = os.path.join(MARKETING_DIR, "Official_Shorts_Reels_Ad_9x16.mp4")

# List of input raw clips in order
clips = [
    os.path.join(MARKETING_DIR, "01_Dashboard_TurnAdvance.mp4"),
    os.path.join(MARKETING_DIR, "02_Minigame_TimingRing_Spin.mp4"),
    os.path.join(MARKETING_DIR, "03_Minigame_FallingSort_Freezer.mp4"),
    os.path.join(MARKETING_DIR, "04_SideHustles_RiskReward.mp4"),
    os.path.join(MARKETING_DIR, "05_HighStakes_ShiftWarning_Crisis.mp4"),
]

bgm = os.path.join(AUDIO_DIR, "Music", "bgm_lofi_neon.mp3")

# Title card overlays & kinetic text for each shot:
# Shot 1 (0-3.5s): "SURVIVE THE JOB HUNT"
# Shot 2 (3.5-6.5s): "PRACTICE YOUR HUSTLE"
# Shot 3 (6.5-9.5s): "RISK IT ALL IN MINIGAMES"
# Shot 4 (9.5-12.5s): "OUTSMART THE ALGORITHM"
# Shot 5 (12.5-16.0s): "GET PROMOTED OR GET FIRED!"

print("--> Building Trailer sequences with cinematic framing, overlays, and audio mix...")

# Create 16:9 Landscape Trailer with stylized game blur-background + centered crisp mobile feed
filter_16_9 = (
    # Background blurred & zoomed
    "[0:v]scale=1920:1080:force_original_aspect_ratio=increase,crop=1920:1080,boxblur=25:5[bg]; "
    # Foreground centered phone frame
    "[0:v]scale=-1:980[fg]; "
    "[bg][fg]overlay=(W-w)/2:(H-h)/2[base]; "
    # Kinetic Brand Caption
    "[base]drawtext=text='DEAR HIRING MANAGER':font='Arial':fontsize=54:fontcolor=white:x=(w-text_w)/2:y=45:box=1:boxcolor=black@0.6:boxborderw=12[v]"
)

# Render 16:9 version
concat_file = os.path.join(MARKETING_DIR, "trailer_concat.txt")
with open(concat_file, "w") as f:
    for c in clips:
        f.write(f"file '{c.replace(os.sep, '/')}'\n")

print("Concatenating gameplay cuts...")
temp_concat = os.path.join(MARKETING_DIR, "temp_concat.mp4")
cmd_concat = f'ffmpeg -y -f concat -safe 0 -i "{concat_file}" -c copy "{temp_concat}"'
subprocess.run(cmd_concat, shell=True, check=True)

print("Applying 16:9 Landscape Master Grade...")
cmd_16_9 = (
    f'ffmpeg -y -i "{temp_concat}" '
    f'-filter_complex "'
    f'[0:v]scale=1920:1080:force_original_aspect_ratio=increase,crop=1920:1080,boxblur=30:8,eq=brightness=-0.15:contrast=1.1[bg]; '
    f'[0:v]scale=-1:1000[fg]; '
    f'[bg][fg]overlay=(W-w)/2:(H-h)/2[comp]; '
    f'[comp]drawtext=text=\'DEAR HIRING MANAGER\':font=\'Impact\':fontsize=52:fontcolor=#38BDF8:x=(w-text_w)/2:y=30:shadowcolor=black@0.8:shadowx=3:shadowy=3, '
    f'drawtext=text=\'CAN YOU SURVIVE THE JOB HUNT?\':font=\'Arial\':fontsize=28:fontcolor=white:x=(w-text_w)/2:y=1020:box=1:boxcolor=black@0.7:boxborderw=10[v]" '
    f'-map "[v]" -map 0:a -c:v libx264 -preset fast -pix_fmt yuv420p -crf 19 -c:a aac -b:a 192k "{FINAL_OUTPUT_16_9}"'
)
subprocess.run(cmd_16_9, shell=True, check=True)

print(f"[OK] Generated: {FINAL_OUTPUT_16_9}")

# Render 9:16 Vertical Video Ad with High-Impact CTA Header and Footer
print("Applying 9:16 Vertical Reel/Ad Grade...")
cmd_9_16 = (
    f'ffmpeg -y -i "{temp_concat}" '
    f'-filter_complex "'
    f'[0:v]drawtext=text=\'CAN YOU GET HIRED?\':font=\'Impact\':fontsize=72:fontcolor=#FFFFFF:x=(w-text_w)/2:y=120:box=1:boxcolor=#C20023@0.9:boxborderw=16, '
    f'drawtext=text=\'DEAR HIRING MANAGER\':font=\'Impact\':fontsize=48:fontcolor=#38BDF8:x=(w-text_w)/2:y=60:shadowcolor=black@0.9:shadowx=2:shadowy=2, '
    f'drawtext=text=\'PLAY FOR FREE ON GOOGLE PLAY\':font=\'Arial\':fontsize=36:fontcolor=white:x=(w-text_w)/2:y=1750:box=1:boxcolor=black@0.8:boxborderw=14[v]" '
    f'-map "[v]" -map 0:a -c:v libx264 -preset fast -pix_fmt yuv420p -crf 19 -c:a aac -b:a 192k "{FINAL_OUTPUT_9_16}"'
)
subprocess.run(cmd_9_16, shell=True, check=True)

print(f"[OK] Generated: {FINAL_OUTPUT_9_16}")

# Clean up temp files
if os.path.exists(temp_concat):
    os.remove(temp_concat)
if os.path.exists(concat_file):
    os.remove(concat_file)
