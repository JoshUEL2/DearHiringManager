import os
import subprocess

ARTIFACTS_DIR = r"C:\Users\Weari\.gemini\antigravity\brain\8b169a0f-95a7-4ea1-8d8c-badddfd104ef"
AUDIO_DIR = r"C:\Users\Weari\Documents\Dear Hiring Manager PC\Assets\Resources\Audio"
OUTPUT_DIR = r"C:\Users\Weari\Documents\Dear Hiring Manager PC\Marketing\Trailers"

os.makedirs(OUTPUT_DIR, exist_ok=True)

def run_ffmpeg(cmd, desc):
    print(f"--> Building: {desc}...")
    result = subprocess.run(cmd, shell=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
    if result.returncode != 0:
        print(f"Error building {desc}:\n{result.stderr[-400:]}")
    else:
        print(f"[OK] Successfully generated {desc}")

# -------------------------------------------------------------
# CLIP 1: Dashboard & Week Advance Payout (3.5s)
# -------------------------------------------------------------
img1 = os.path.join(ARTIFACTS_DIR, "screen_revealed_dashboard.png")
sfx_advance = os.path.join(AUDIO_DIR, "SFX", "sfx_turn_advance.wav")
sfx_cash = os.path.join(AUDIO_DIR, "SFX", "sfx_cash.wav")
bgm_neon = os.path.join(AUDIO_DIR, "Music", "bgm_lofi_neon.mp3")
out1 = os.path.join(OUTPUT_DIR, "01_Dashboard_TurnAdvance.mp4")

cmd1 = (
    f'ffmpeg -y -loop 1 -t 3.5 -i "{img1}" '
    f'-i "{bgm_neon}" -i "{sfx_advance}" -i "{sfx_cash}" '
    f'-filter_complex "'
    f'[0:v]scale=1080:1920,zoompan=z=\'min(zoom+0.0006,1.06)\':x=\'iw/2-(iw/zoom/2)\':y=\'ih/2-(ih/zoom/2)+150\':d=105:s=1080x1920:fps=30[v]; '
    f'[1:a]atrim=0:3.5,afade=t=in:st=0:d=0.3,afade=t=out:st=3.2:d=0.3,volume=0.35[bgm]; '
    f'[2:a]adelay=600|600,volume=0.85[adv]; '
    f'[3:a]adelay=1400|1400,volume=0.90[cash]; '
    f'[bgm][adv][cash]amix=inputs=3:dropout_transition=0:normalize=0[a]" '
    f'-map "[v]" -map "[a]" -c:v libx264 -preset fast -pix_fmt yuv420p -r 30 -crf 20 -c:a aac -b:a 192k -t 3.5 "{out1}"'
)
run_ffmpeg(cmd1, "01_Dashboard_TurnAdvance.mp4")

# -------------------------------------------------------------
# CLIP 2: Street Sign Spinner Timing Ring Minigame (3.0s)
# -------------------------------------------------------------
img2 = os.path.join(ARTIFACTS_DIR, "screen_minigame_timingring.png")
sfx_click = os.path.join(AUDIO_DIR, "SFX", "sfx_click.wav")
sfx_win = os.path.join(AUDIO_DIR, "SFX", "sfx_minigame_success.wav")
bgm_term = os.path.join(AUDIO_DIR, "Music", "bgm_lofi_terminal.mp3")
out2 = os.path.join(OUTPUT_DIR, "02_Minigame_TimingRing_Spin.mp4")

cmd2 = (
    f'ffmpeg -y -loop 1 -i "{img2}" '
    f'-i "{bgm_term}" -i "{sfx_click}" -i "{sfx_win}" '
    f'-filter_complex "'
    f'[0:v]scale=1080:1920,zoompan=z=\'min(zoom+0.0008,1.08)\':x=\'iw/2-(iw/zoom/2)\':y=\'ih/2-(ih/zoom/2)-80\':d=90:s=1080x1920:fps=30[v]; '
    f'[1:a]atrim=0:3.0,afade=t=in:st=0:d=0.3,afade=t=out:st=2.7:d=0.3,volume=0.35[bgm]; '
    f'[2:a]adelay=400|400,volume=0.75[tap]; '
    f'[3:a]adelay=1200|1200,volume=0.85[win]; '
    f'[bgm][tap][win]amix=inputs=3:dropout_transition=0:normalize=0[a]" '
    f'-map "[v]" -map "[a]" -c:v libx264 -preset fast -pix_fmt yuv420p -r 30 -crf 20 -c:a aac -b:a 192k -t 3.0 "{out2}"'
)
run_ffmpeg(cmd2, "02_Minigame_TimingRing_Spin.mp4")

# -------------------------------------------------------------
# CLIP 3: Falling Sort Freezer/Metal Scrap Minigame (3.0s)
# -------------------------------------------------------------
img3 = os.path.join(ARTIFACTS_DIR, "screen_minigame_fallingsort.png")
out3 = os.path.join(OUTPUT_DIR, "03_Minigame_FallingSort_Freezer.mp4")

cmd3 = (
    f'ffmpeg -y -loop 1 -i "{img3}" '
    f'-i "{bgm_neon}" -i "{sfx_click}" -i "{sfx_win}" '
    f'-filter_complex "'
    f'[0:v]scale=1080:1920,zoompan=z=\'min(zoom+0.0007,1.07)\':x=\'iw/2-(iw/zoom/2)\':y=\'ih/2-(ih/zoom/2)+50\':d=90:s=1080x1920:fps=30[v]; '
    f'[1:a]atrim=2:5.0,afade=t=in:st=0:d=0.3,afade=t=out:st=2.7:d=0.3,volume=0.35[bgm]; '
    f'[2:a]adelay=500|500,volume=0.75[tap]; '
    f'[3:a]adelay=1500|1500,volume=0.85[win]; '
    f'[bgm][tap][win]amix=inputs=3:dropout_transition=0:normalize=0[a]" '
    f'-map "[v]" -map "[a]" -c:v libx264 -preset fast -pix_fmt yuv420p -r 30 -crf 20 -c:a aac -b:a 192k -t 3.0 "{out3}"'
)
run_ffmpeg(cmd3, "03_Minigame_FallingSort_Freezer.mp4")

# -------------------------------------------------------------
# CLIP 4: Side Hustles & Illegal High Stakes (3.0s)
# -------------------------------------------------------------
img4 = os.path.join(ARTIFACTS_DIR, "screen_hustles_revealed.png")
sfx_tab = os.path.join(AUDIO_DIR, "SFX", "sfx_tab_switch.wav")
out4 = os.path.join(OUTPUT_DIR, "04_SideHustles_RiskReward.mp4")

cmd4 = (
    f'ffmpeg -y -loop 1 -i "{img4}" '
    f'-i "{bgm_term}" -i "{sfx_tab}" -i "{sfx_click}" '
    f'-filter_complex "'
    f'[0:v]scale=1080:1920,zoompan=z=\'min(zoom+0.0006,1.06)\':x=\'iw/2-(iw/zoom/2)\':y=\'ih/2-(ih/zoom/2)+100\':d=90:s=1080x1920:fps=30[v]; '
    f'[1:a]atrim=5:8.0,afade=t=in:st=0:d=0.3,afade=t=out:st=2.7:d=0.3,volume=0.35[bgm]; '
    f'[2:a]adelay=200|200,volume=0.80[tab]; '
    f'[3:a]adelay=1200|1200,volume=0.75[click]; '
    f'[bgm][tab][click]amix=inputs=3:dropout_transition=0:normalize=0[a]" '
    f'-map "[v]" -map "[a]" -c:v libx264 -preset fast -pix_fmt yuv420p -r 30 -crf 20 -c:a aac -b:a 192k -t 3.0 "{out4}"'
)
run_ffmpeg(cmd4, "04_SideHustles_RiskReward.mp4")

# -------------------------------------------------------------
# CLIP 5: Emergency Shift Warning Crisis (3.0s)
# -------------------------------------------------------------
img5 = os.path.join(ARTIFACTS_DIR, "screen_shift_warning_live.png")
sfx_warn = os.path.join(AUDIO_DIR, "SFX", "sfx_warning_shift.wav")
sfx_heart = os.path.join(AUDIO_DIR, "SFX", "sfx_heartbeat.wav")
sfx_deduct = os.path.join(AUDIO_DIR, "SFX", "sfx_bill_deduction.wav")
out5 = os.path.join(OUTPUT_DIR, "05_HighStakes_ShiftWarning_Crisis.mp4")

cmd5 = (
    f'ffmpeg -y -loop 1 -i "{img5}" '
    f'-i "{bgm_neon}" -i "{sfx_warn}" -i "{sfx_heart}" -i "{sfx_deduct}" '
    f'-filter_complex "'
    f'[0:v]scale=1080:1920,zoompan=z=\'min(zoom+0.0009,1.09)\':x=\'iw/2-(iw/zoom/2)\':y=\'ih/2-(ih/zoom/2)\':d=90:s=1080x1920:fps=30[v]; '
    f'[1:a]atrim=8:11.0,afade=t=in:st=0:d=0.3,afade=t=out:st=2.7:d=0.3,volume=0.30[bgm]; '
    f'[2:a]adelay=100|100,volume=0.90[warn]; '
    f'[3:a]adelay=700|700,volume=0.85[heart]; '
    f'[4:a]adelay=1600|1600,volume=0.80[deduct]; '
    f'[bgm][warn][heart][deduct]amix=inputs=4:dropout_transition=0:normalize=0[a]" '
    f'-map "[v]" -map "[a]" -c:v libx264 -preset fast -pix_fmt yuv420p -r 30 -crf 20 -c:a aac -b:a 192k -t 3.0 "{out5}"'
)
run_ffmpeg(cmd5, "05_HighStakes_ShiftWarning_Crisis.mp4")

# -------------------------------------------------------------
# FULL TEASER: 15.5-Second Mobile Vertical Gameplay Trailer (1080x1920)
# -------------------------------------------------------------
out_full = os.path.join(OUTPUT_DIR, "Trailer_Teaser_Vertical_1080x1920.mp4")

concat_file = os.path.join(OUTPUT_DIR, "concat_list.txt")
with open(concat_file, "w") as f:
    f.write(f"file '01_Dashboard_TurnAdvance.mp4'\n")
    f.write(f"file '02_Minigame_TimingRing_Spin.mp4'\n")
    f.write(f"file '03_Minigame_FallingSort_Freezer.mp4'\n")
    f.write(f"file '04_SideHustles_RiskReward.mp4'\n")
    f.write(f"file '05_HighStakes_ShiftWarning_Crisis.mp4'\n")

cmd_concat = (
    f'ffmpeg -y -f concat -safe 0 -i "{concat_file}" -c copy "{out_full}"'
)
run_ffmpeg(cmd_concat, "Trailer_Teaser_Vertical_1080x1920.mp4")

print("\n=== ALL GAMEPLAY MARKETING ASSETS COMPILED ===")
