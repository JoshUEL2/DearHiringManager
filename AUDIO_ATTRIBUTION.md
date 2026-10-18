# Dear Hiring Manager - Audio Library & Attribution

All audio assets integrated into *Dear Hiring Manager* are dedicated to the Public Domain under the **Creative Commons CC0 1.0 Universal License** or are certified Public Domain. No copyright restrictions apply.

---

## 1. Background Music (Lo-Fi Cyberpunk / Corporate Vibe)

Source: [btahir/open-lofi](https://github.com/btahir/open-lofi)  
License: **Creative Commons Zero (CC0 1.0 Universal)**

| File Name | Track Title | Style & Vibe | In-Game Context |
| :--- | :--- | :--- | :--- |
| `bgm_lofi_neon.mp3` | *Rain off the Neon Signs* | Warm Rhodes piano, gentle vinyl crackle, distant rainfall | Default gameplay BGM (Dashboard, Hustles, Careers) |
| `bgm_lofi_terminal.mp3` | *Terminal Rain* | Deep sub bass, tape flutter, contemplative cyberpunk synth | Playlist rotation / Late-night focus sessions |
| `bgm_lofi_debug.mp3` | *2 AM Debug Loop* | Minimalist electronic pulse, downtempo cyber groove | Playlist rotation / Long planning cycles |

---

## 2. Situational & Contextual Ambience

| File Name | Asset Name | Source & Author | License | In-Game Context |
| :--- | :--- | :--- | :--- | :--- |
| `amb_park_birds.ogg` | *Birds chirping in a garden* | Wikimedia Commons (`File:Birds_chirping_in_a_garden.ogg`) | **CC0 1.0 Universal** | Plays during the **Activities** tab and the *"Walk in the Park"* wellness activity. Ducks background music to 20% so natural atmosphere takes prominence. |
| `amb_education_murmurs.ogg` | *Ambient classroom mono* | Wikimedia Commons (`File:Ambient_classroom_mono.ogg`) | **Public Domain** | Plays during the **Education** tab and while undertaking courses/degrees. Creates a cozy academic library/hall murmur. |

---

## 3. Physical & Tactical Sound Effects (SFX)

Custom synthesized and engineered 44.1 kHz, 16-bit PCM WAV assets dedicated to the Public Domain (**CC0 1.0 Universal**).

| SFX Asset | Description | Trigger Location & Game Loop Role |
| :--- | :--- | :--- |
| `sfx_scrub_plate_01.wav` | Low sponge friction sweep on ceramic | Fired dynamically on pointer movement in `ScrubPlateMiniGame` & `PhysicalScrubMiniGame`. |
| `sfx_scrub_plate_02.wav` | High sponge friction sweep on ceramic | Alternates with plate scrub 01 with dynamic pitch variation to create natural tactile texture. |
| `sfx_water_splash.wav` | Fluid pressure burst / water rinse | Plays during water-based cleaning (e.g. Wheelie Bin pressure spray) and dish washing. |
| `sfx_dish_clatter.wav` | Crisp ceramic dish stacking clatter | Plays immediately upon 100% cleaning completion of a dish or tray. |
| `sfx_tab_switch.wav` | Cybernetic sliding interface chime | Plays on switching between bottom navigation tabs (`Careers`, `Hustles`, `Education`, `Activities`). |
| `sfx_click.wav` | Snappy high-tech UI blip | Default tactile button click across all dialogs and cards. |
| `sfx_cash.wav` | Multi-harmonic E-major register chime | Rewarded upon shift payout, hustle completion, and salary deposits. |
| `sfx_bill_deduction.wav` | Low pneumatic mechanical stamp | Deducted upon weekly rent, food, and tuition payments. |
| `sfx_turn_advance.wav` | Mechanical digital shutter cycle | Triggered upon clicking `+ Week` to progress time. |
| `sfx_warning_shift.wav` | Amber caution alert tone | Pops up when advancing the week with unworked required job shifts. |
| `sfx_minigame_success.wav`| Ascending fanfare arpeggio | Triggered on triumphant minigame victory and promotions. |
| `sfx_minigame_fail.wav` | Dissonant descending buzzer | Triggered on minigame time-out or contract termination. |
| `sfx_heartbeat.wav` | Dual-pulse visceral lub-dub | Triggered rhythmically when stress exceeds critical threshold (>80%). |
| `sfx_typewriter.wav` | Mechanical keystroke click | Feeds terminal log lines onto the player's dashboard. |

---

## 4. Audio Routing Architecture

- **Channel Separation**:
  - `musicSource`: Plays looping lo-fi background tracks.
  - `ambienceSource`: Plays contextual spatial environments with smooth crossfading.
  - `sfxSource`: One-shot tactile UI and minigame sound effects with rate limiting and pitch randomization.
  - `heartbeatSource`: Dedicated unmuted channel triggered dynamically by stress state.
- **Dynamic Ducking**:
  - Entering Education or Park activities automatically lowers BGM volume to 20–25% over 0.8s.
  - Returning to the Dashboard, Careers, or Hustles tab seamlessly fades out ambience and restores BGM to 100%.
