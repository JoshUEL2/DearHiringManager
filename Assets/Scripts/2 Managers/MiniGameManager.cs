using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class MiniGameManager : MonoBehaviour
{
    private void Start()
    {
        var root = miniGameDocument.rootVisualElement;
        root.pickingMode = PickingMode.Ignore;


        overlay = root.Q<VisualElement>("Overlay");
        btnAbortMinigame = root.Q<UnityEngine.UIElements.Button>("BtnAbortMinigame");
        if (btnAbortMinigame != null) btnAbortMinigame.clicked += AbortGame;
        operationNameText = root.Q<Label>("OperationNameText");
        instructionsText = root.Q<Label>("InstructionsText");

        // Footer
        btnAbort = root.Q<Button>("BtnAbortMinigame");
        if (btnAbort != null) btnAbort.clicked += AbortGame;


        overlay.style.display = DisplayStyle.None;
        overlay.pickingMode = PickingMode.Ignore;
    }
    private void BindLegacyUI(VisualElement activeRoot)
    {

        

        // Containers
        tapContainer = activeRoot.Q<VisualElement>("TapGameContainer");
        sliderContainer = activeRoot.Q<VisualElement>("SliderGameContainer");
        choiceContainer = activeRoot.Q<VisualElement>("ChoiceGameContainer");
        sequenceContainer = activeRoot.Q<VisualElement>("SequenceGameContainer");
        swipeContainer = activeRoot.Q<VisualElement>("SwipeGameContainer");
        mashRaceContainer = activeRoot.Q<VisualElement>("MashRaceContainer");
        memoryMatchContainer = activeRoot.Q<VisualElement>("MemoryMatchContainer");

        // New Containers
        timingRingContainer = activeRoot.Q<VisualElement>("TimingRingContainer");
        fallingSortContainer = activeRoot.Q<VisualElement>("FallingSortContainer");
        gridDodgeContainer = activeRoot.Q<VisualElement>("GridDodgeContainer");
        whackGridContainer = activeRoot.Q<VisualElement>("WhackGridContainer");
        sweepFloorContainer = activeRoot.Q<VisualElement>("SweepFloorContainer");
        sweepFloorGrid = activeRoot.Q<VisualElement>("SweepFloorGrid");
        scrubPlateContainer = activeRoot.Q<VisualElement>("ScrubPlateContainer");
        scrubProgressBar = activeRoot.Q<ProgressBar>("ScrubProgressBar");
        scrubPlateArea = activeRoot.Q<VisualElement>("ScrubPlateArea");
        scrubDirtOverlay = activeRoot.Q<VisualElement>("ScrubDirtOverlay");
        scrubSponge = activeRoot.Q<VisualElement>("ScrubSponge");
        
        pumpGameContainer = activeRoot.Q<VisualElement>("PumpGameContainer");
        pumpProgressBar = activeRoot.Q<VisualElement>("PumpProgressBar");
        pumpNormalBar = activeRoot.Q<VisualElement>("PumpNormalBar");
        pumpDialContainer = activeRoot.Q<VisualElement>("PumpDialContainer");
        pumpDialTargetZone = activeRoot.Q<VisualElement>("PumpDialTargetZone");
        pumpDialTargetArc = activeRoot.Q<VisualElement>("PumpDialTargetArc");
        pumpDialNeedleRotator = activeRoot.Q<VisualElement>("PumpDialNeedleRotator");
        pumpHoldTimerContainer = activeRoot.Q<VisualElement>("PumpHoldTimerContainer");
        pumpHoldTimerBar = activeRoot.Q<VisualElement>("PumpHoldTimerBar");
        pumpHandle = activeRoot.Q<VisualElement>("PumpHandle");
        pumpTrackArea = activeRoot.Q<VisualElement>("PumpTrackArea");

        scrubPlateArea?.RegisterCallback<PointerDownEvent>(ev => {
            if (currentConfig == null || currentConfig.type != GameType.ScrubPlate || !isPlaying) return;
            lastScrubPos = (Vector2)ev.localPosition;
            scrubPlateArea.CapturePointer(ev.pointerId);
            ev.StopPropagation();
        });

        scrubPlateArea?.RegisterCallback<PointerUpEvent>(ev => {
            if (scrubPlateArea.HasPointerCapture(ev.pointerId))
            {
                scrubPlateArea.ReleasePointer(ev.pointerId);
            }
        });

        scrubPlateArea?.RegisterCallback<PointerMoveEvent>(ev => {
            if (currentConfig == null || currentConfig.type != GameType.ScrubPlate || !isPlaying) return;
            
            // Constrain sponge visually
            Vector2 localPos = (Vector2)ev.localPosition;
            float spongeW = 80f, spongeH = 80f;
            localPos.x = Mathf.Clamp(localPos.x, 0, scrubPlateArea.layout.width - spongeW);
            localPos.y = Mathf.Clamp(localPos.y, 0, scrubPlateArea.layout.height - spongeH);
            
            scrubSponge.style.left = localPos.x;
            scrubSponge.style.top = localPos.y;

            if (scrubPlateArea.HasPointerCapture(ev.pointerId)) // Only scrub if dragging
            {
                float delta = ((Vector2)ev.localPosition - lastScrubPos).magnitude;
                if (delta > 2f)
                {
                    scrubProgress += delta * 0.015f; 
                    scrubProgressBar.value = scrubProgress;
                    
                    float opacity = 1f - (scrubProgress / currentConfig.targetScore);
                    scrubDirtOverlay.style.opacity = Mathf.Clamp01(opacity);
                    
                    if (scrubProgress >= currentConfig.targetScore)
                    {
                        scrubProgressBar.value = currentConfig.targetScore;
                        scrubDirtOverlay.style.opacity = 0f;
                        scrubPlateArea.ReleasePointer(ev.pointerId);
                        MarkWinReady();
                    }
                }
                lastScrubPos = (Vector2)ev.localPosition;
            }
        });

        if (pumpTrackArea != null)
        {
            pumpTrackArea?.RegisterCallback<PointerDownEvent>(ev => {
                if (currentConfig == null || currentConfig.type != GameType.PumpSystem || !isPlaying) return;
                
                pumpLastY = ev.localPosition.y;
                pumpTrackArea.CapturePointer(ev.pointerId);
                ev.StopPropagation();
            });
            
            pumpTrackArea?.RegisterCallback<PointerUpEvent>(ev => {
                if (pumpTrackArea.HasPointerCapture(ev.pointerId)) pumpTrackArea.ReleasePointer(ev.pointerId);
            });

            pumpTrackArea?.RegisterCallback<PointerMoveEvent>(ev => {
                if (currentConfig == null || currentConfig.type != GameType.PumpSystem || !isPlaying) return;
                if (pumpTrackArea.HasPointerCapture(ev.pointerId))
                {
                    float dy = ev.localPosition.y - pumpLastY;
                    pumpLastY = ev.localPosition.y;
                    
                    if (dy > 0) // moving down
                    {
                        pumpProgress += dy * 0.3f; // Reduced sensitivity for better balancing
                    }
                    
                    float hY = Mathf.Clamp(ev.localPosition.y, 0, pumpTrackArea.resolvedStyle.height - 40f);
                    pumpHandle.style.top = hY;
                }
            });
        }

        activeRoot.RegisterCallback<PointerDownEvent>(ev => {
            if (!isPlaying || currentConfig.type != GameType.SweepFloor) return;
            swipeStartPos = ev.position;
            activeRoot.CapturePointer(ev.pointerId);
            ev.StopPropagation();
        });

        activeRoot.RegisterCallback<PointerMoveEvent>(ev => {
            if (!isPlaying || currentConfig.type != GameType.SweepFloor) return;
            ev.StopPropagation(); // Prevent ScrollView from intercepting drag!
        });

        activeRoot.RegisterCallback<PointerUpEvent>(ev => {
            if (activeRoot.HasPointerCapture(ev.pointerId))
            {
                activeRoot.ReleasePointer(ev.pointerId);
            }
            if (!isPlaying) return;
            // SweepFloor doesn't use swipe release for logic, so just cleanup here
            ev.StopPropagation();
        });

        activeRoot.focusable = true;
        activeRoot.RegisterCallback<KeyDownEvent>(ev => {
            if (!isPlaying || currentConfig == null) return;
            int dx = 0, dy = 0;
            if (ev.keyCode == KeyCode.UpArrow || ev.keyCode == KeyCode.W) dy = -1;
            else if (ev.keyCode == KeyCode.DownArrow || ev.keyCode == KeyCode.S) dy = 1;
            else if (ev.keyCode == KeyCode.LeftArrow || ev.keyCode == KeyCode.A) dx = -1;
            else if (ev.keyCode == KeyCode.RightArrow || ev.keyCode == KeyCode.D) dx = 1;
            
            if (dx == 0 && dy == 0) return;
            
            ev.StopPropagation();

            if (currentConfig.type == GameType.SweepFloor)
            {
                int nx = sweepPlayerX + dx;
                int ny = sweepPlayerY + dy;
                if (nx >= 0 && nx < 5 && ny >= 0 && ny < 5)
                {
                    sweepPlayerX = nx;
                    sweepPlayerY = ny;
                    sweepTileTimers[ny * 5 + nx] = 15f; // Instantly clean
                    RenderSweepFloor();
                }
            }
            else if (currentConfig.type == GameType.Tetris)
            {
                if (dx == -1) OnTetrisInput(-1, 0, 0);
                else if (dx == 1) OnTetrisInput(1, 0, 0);
                else if (dy == -1) OnTetrisInput(0, 0, 1);
                else if (dy == 1) OnTetrisInput(0, 1, 0);
            }
        }, TrickleDown.TrickleDown);

        codeBreakerContainer = activeRoot.Q<VisualElement>("CodeBreakerContainer");
        balanceMeterContainer = activeRoot.Q<VisualElement>("BalanceMeterContainer");
        dialogueChainContainer = activeRoot.Q<VisualElement>("DialogueChainContainer");
        dialogueTimerBar = activeRoot.Q<VisualElement>("DialogueTimerBar");
        orderQueueContainer = activeRoot.Q<VisualElement>("OrderQueueContainer");
        dragMatchContainer = activeRoot.Q<VisualElement>("DragMatchContainer");
        sprintLaneContainer = activeRoot.Q<VisualElement>("SprintLaneContainer");
        fileSorterContainer = activeRoot.Q<VisualElement>("FileSorterContainer");
        
        // CoffeeWork
        coffeeWorkContainer = activeRoot.Q<VisualElement>("CoffeeWorkContainer");
        cwTimerText = activeRoot.Q<Label>("CwTimerText");
        cwEnergyFill = activeRoot.Q<VisualElement>("CwEnergyFill");
        cwProgressFill = activeRoot.Q<VisualElement>("CwProgressFill");
        cwBtnCoffee = activeRoot.Q<Button>("CwBtnCoffee");
        cwBtnType = activeRoot.Q<Button>("CwBtnType");
        
        // UnitTestRunner
        unitTestContainer = activeRoot.Q<VisualElement>("UnitTestContainer");
        utRoundText = activeRoot.Q<Label>("UtRoundText");
        utTimerText = activeRoot.Q<Label>("UtTimerText");
        utCodeText = activeRoot.Q<Label>("UtCodeText");
        utBtnOption1 = activeRoot.Q<Button>("UtBtnOption1");
        utBtnOption2 = activeRoot.Q<Button>("UtBtnOption2");
        utBtnOption3 = activeRoot.Q<Button>("UtBtnOption3");
        
        // CenterDiv
        centerDivContainer = activeRoot.Q<VisualElement>("CenterDivContainer");
        cdRoundText = activeRoot.Q<Label>("CdRoundText");
        cdTimerText = activeRoot.Q<Label>("CdTimerText");
        cdTargetBox = activeRoot.Q<VisualElement>("CdTargetBox");
        cdMovingBox = activeRoot.Q<VisualElement>("CdMovingBox");
        cdBtnCenter = activeRoot.Q<Button>("CdBtnCenter");

        dragCenterContainer = activeRoot.Q<VisualElement>("DragCenterContainer");
        dcWorkArea = activeRoot.Q<VisualElement>("DcWorkArea");
        dcTargetCrosshair = activeRoot.Q<VisualElement>("DcTargetCrosshair");
        dcScoreArea = activeRoot.Q<VisualElement>("DcScoreArea");
        dcScoreBarFill = activeRoot.Q<VisualElement>("DcScoreBarFill");
        dcScoreText = activeRoot.Q<Label>("DcScoreText");
        dcRoundText = activeRoot.Q<Label>("DcRoundText");
        dcTimerText = activeRoot.Q<Label>("DcTimerText");
        dcDraggableIcon = activeRoot.Q<Label>("DcDraggableIcon");
        dcBtnConfirm = activeRoot.Q<Button>("DcBtnConfirm");
        
        sizeMatchContainer = activeRoot.Q<VisualElement>("SizeMatchContainer");
        smTargetBox = activeRoot.Q<VisualElement>("SmTargetBox");
        smRoundText = activeRoot.Q<Label>("SmRoundText");
        smTimerText = activeRoot.Q<Label>("SmTimerText");
        smBtnChangingBox = activeRoot.Q<Button>("SmBtnChangingBox");
        
        stealthInjectContainer = activeRoot.Q<VisualElement>("StealthInjectContainer");
        siWorkArea = activeRoot.Q<VisualElement>("SiWorkArea");
        siProgressBarFill = activeRoot.Q<VisualElement>("SiProgressBarFill");
        siStatusText = activeRoot.Q<Label>("SiStatusText");
        siTimerText = activeRoot.Q<Label>("SiTimerText");
        siReviewerIcon = activeRoot.Q<Label>("SiReviewerIcon");
        siReviewerStateText = activeRoot.Q<Label>("SiReviewerStateText");
        siBtnInject = activeRoot.Q<Label>("SiBtnInject");
        
        if (cwBtnCoffee != null) cwBtnCoffee.clicked += OnCwBtnCoffee;
        if (cwBtnType != null) cwBtnType.clicked += OnCwBtnType;
        if (utBtnOption1 != null) utBtnOption1.clicked += () => OnUtBtnOption(0);
        if (utBtnOption2 != null) utBtnOption2.clicked += () => OnUtBtnOption(1);
        if (utBtnOption3 != null) utBtnOption3.clicked += () => OnUtBtnOption(2);
        if (cdBtnCenter != null) cdBtnCenter.clicked += OnCdBtnCenter;

        if (dcBtnConfirm != null) dcBtnConfirm.clicked += OnDcBtnConfirm;
        if (dcDraggableIcon != null)
        {
            dcDraggableIcon?.RegisterCallback<PointerDownEvent>(OnDcPointerDown);
            dcDraggableIcon?.RegisterCallback<PointerMoveEvent>(OnDcPointerMove);
            dcDraggableIcon?.RegisterCallback<PointerUpEvent>(OnDcPointerUp);
            dcDraggableIcon?.RegisterCallback<PointerCaptureOutEvent>(OnDcPointerCaptureOut);
        }

        if (smBtnChangingBox != null) smBtnChangingBox.clicked += OnSmBoxClicked;

        if (siBtnInject != null)
        {
            siBtnInject?.RegisterCallback<PointerDownEvent>(OnSiPointerDown);
            siBtnInject?.RegisterCallback<PointerUpEvent>(OnSiPointerUp);
            siBtnInject?.RegisterCallback<PointerCaptureOutEvent>(OnSiPointerCaptureOut);
        }
        
        automatedTestsContainer = activeRoot.Q<VisualElement>("AutomatedTestsContainer");
        atProgressBarFill = activeRoot.Q<VisualElement>("AtProgressBarFill");
        atTimerText = activeRoot.Q<Label>("AtTimerText");
        
        codeCommenterContainer = activeRoot.Q<VisualElement>("CodeCommenterContainer");
        ccTimerText = activeRoot.Q<Label>("CcTimerText");
        
        documentSignerContainer = activeRoot.Q<VisualElement>("DocumentSignerContainer");
        dsScrollView = activeRoot.Q<ScrollView>("DsScrollView");
        btnStampApproval = activeRoot.Q<Button>("BtnStampApproval");
        dsTimerText = activeRoot.Q<Label>("DsTimerText");
        
        chartAdjusterContainer = activeRoot.Q<VisualElement>("ChartAdjusterContainer");
        caCompetitorBar = activeRoot.Q<VisualElement>("CaCompetitorBar");
        caAverageBar = activeRoot.Q<VisualElement>("CaAverageBar");
        caOurBar = activeRoot.Q<VisualElement>("CaOurBar");
        caTimerText = activeRoot.Q<Label>("CaTimerText");
        caStatusText = activeRoot.Q<Label>("CaStatusText");

        var caChartArea = activeRoot.Q<VisualElement>("CaChartArea");
        if (caChartArea != null)
        {
            Vector2 lastPos = Vector2.zero;
            caChartArea?.RegisterCallback<PointerDownEvent>(ev => {
                if (currentConfig == null || currentConfig.type != GameType.ChartAdjuster || !isPlaying) return;
                lastPos = ev.position;
                caChartArea.CapturePointer(ev.pointerId);
                ev.StopPropagation();
            });
            caChartArea?.RegisterCallback<PointerUpEvent>(ev => {
                if (caChartArea.HasPointerCapture(ev.pointerId))
                {
                    caChartArea.ReleasePointer(ev.pointerId);
                }
            });
            caChartArea?.RegisterCallback<PointerMoveEvent>(ev => {
                if (currentConfig == null || currentConfig.type != GameType.ChartAdjuster || !isPlaying) return;
                if (caChartArea.HasPointerCapture(ev.pointerId))
                {
                    float deltaY = ev.position.y - lastPos.y;
                    
                    // Did they drag over the competitor bar or our bar?
                    // We can check the x coordinate roughly. 
                    // Competitor is on the left, Ours is on the right.
                    float width = caChartArea.layout.width;
                    float x = ev.localPosition.x;
                    
                    if (x < width * 0.33f)
                    {
                        // Competitor Bar: We want to shrink it (swipe down -> positive deltaY in UI Toolkit space)
                        caCompVal -= deltaY * 0.2f;
                        caCompVal = Mathf.Clamp(caCompVal, 5f, 95f);
                        if (caCompetitorBar != null) caCompetitorBar.style.height = new Length(caCompVal, LengthUnit.Percent);
                    }
                    else if (x > width * 0.66f)
                    {
                        // Our Bar: We want to grow it (swipe up -> negative deltaY)
                        caOurVal -= deltaY * 0.2f;
                        caOurVal = Mathf.Clamp(caOurVal, 5f, 95f);
                        if (caOurBar != null) caOurBar.style.height = new Length(caOurVal, LengthUnit.Percent);
                    }
                    
                    lastPos = ev.position;
                }
            });
        }
        
        neuralAlignerContainer = activeRoot.Q<VisualElement>("NeuralAlignerContainer");
        naStatusText = activeRoot.Q<Label>("NaStatusText");
        naMoralSlider = activeRoot.Q<Slider>("NaMoralSlider");
        naMoralLight = activeRoot.Q<VisualElement>("NaMoralLight");
        naLogicSlider = activeRoot.Q<Slider>("NaLogicSlider");
        naLogicLight = activeRoot.Q<VisualElement>("NaLogicLight");
        naEmpathySlider = activeRoot.Q<Slider>("NaEmpathySlider");
        naEmpathyLight = activeRoot.Q<VisualElement>("NaEmpathyLight");

        sentienceArrayContainer = activeRoot.Q<VisualElement>("SentienceArrayContainer");
        saTimerText = activeRoot.Q<Label>("SaTimerText");
        saProgressFill = activeRoot.Q<VisualElement>("SaProgressFill");
        for (int i=0; i<16; i++) {
            int index = i;
            saNodes[i] = activeRoot.Q<VisualElement>($"SaNode_{i}");
            if (saNodes[i] != null) {
                saNodes[i]?.RegisterCallback<PointerDownEvent>(ev => {
                    if (saHeat[index] > 0) saHeat[index] = 0; // suppress
                });
            }
        }

        containmentProtocolContainer = activeRoot.Q<VisualElement>("ContainmentProtocolContainer");
        cpTimerText = activeRoot.Q<Label>("CpTimerText");
        for (int i=0; i<4; i++) {
            int index = i;
            cpSwitches[i] = activeRoot.Q<VisualElement>($"CpSwitch_{i}");
            if (cpSwitches[i] != null) {
                cpSwitches[i]?.RegisterCallback<PointerMoveEvent>(ev => {
                    if (ev.pressedButtons != 0 && ev.deltaPosition.y > 5f) cpSwitchState[index] = true;
                });
                cpSwitches[i]?.RegisterCallback<PointerDownEvent>(ev => { cpSwitchState[index] = true; }); // fallback tap
            }
        }
        cpGlassCover = activeRoot.Q<VisualElement>("CpGlassCover");
        if (cpGlassCover != null) {
            cpGlassCover?.RegisterCallback<PointerMoveEvent>(ev => {
                if (ev.pressedButtons != 0 && ev.deltaPosition.y < -5f) cpGlassLifted = true;
            });
            cpGlassCover?.RegisterCallback<PointerDownEvent>(ev => { cpGlassLifted = true; });
        }
        cpRedButton = activeRoot.Q<VisualElement>("CpRedButton");
        cpButtonText = activeRoot.Q<Label>("CpButtonText");
        if (cpRedButton != null) {
            cpRedButton?.RegisterCallback<PointerDownEvent>(ev => {
                if (cpGlassLifted) cpMashCount++;
            });
        }

        rewardLogicContainer = activeRoot.Q<VisualElement>("RewardLogicContainer");
        rlTimerText = activeRoot.Q<Label>("RlTimerText");
        rlGraphArea = activeRoot.Q<VisualElement>("RlGraphArea");
        rlCore = activeRoot.Q<VisualElement>("RlCore");
        for (int i=0; i<3; i++) {
            int index = i;
            rlJunctions[i] = activeRoot.Q<VisualElement>($"RlJunc_{i}");
            rlArrows[i] = activeRoot.Q<Label>($"RlArr_{i}");
            if (rlJunctions[i] != null) {
                rlJunctions[i]?.RegisterCallback<PointerDownEvent>(ev => {
                    rlState[index] = !rlState[index];
                    rlArrows[index].text = rlState[index] ? "→" : "←";
                });
            }
        }

        galacticCouncilContainer = activeRoot.Q<VisualElement>("GalacticCouncilContainer");
        gcTimerText = activeRoot.Q<Label>("GcTimerText");
        gcProgressFill = activeRoot.Q<VisualElement>("GcProgressFill");
        gcPlayArea = activeRoot.Q<VisualElement>("GcPlayArea");
        gcProjectileContainer = activeRoot.Q<VisualElement>("GcProjectileContainer");
        gcPodium = activeRoot.Q<VisualElement>("GcPodium");
        if (gcPlayArea != null) {
            gcPlayArea?.RegisterCallback<PointerMoveEvent>(ev => {
                if (ev.pressedButtons != 0) {
                    gcPodiumX += ev.deltaPosition.x;
                    gcPodiumX = UnityEngine.Mathf.Clamp(gcPodiumX, -150f, 150f);
                    gcPodium.style.translate = new UnityEngine.UIElements.StyleTranslate(new UnityEngine.UIElements.Translate(gcPodiumX, 0, 0));
                }
            });
        }

        franticBackspaceContainer = activeRoot.Q<VisualElement>("FranticBackspaceContainer");
        fbTimerText = activeRoot.Q<Label>("FbTimerText");
        fbPlayerText = activeRoot.Q<Label>("FbPlayerText");
        fbBackspaceBtn = activeRoot.Q<Button>("FbBackspaceBtn");
        
        if (fbBackspaceBtn != null) {
            if (fbBackspaceBtn != null) fbBackspaceBtn.clicked += () => {
                if (!isPlaying || currentConfig.type != GameType.FranticBackspace) return;
                
                // Delete up to 5 characters per tap to make it frantic but possible
                if (fbCurrentMessage.Length > 0) {
                    int removeCount = UnityEngine.Mathf.Min(5, fbCurrentMessage.Length);
                    fbCurrentMessage = fbCurrentMessage.Substring(0, fbCurrentMessage.Length - removeCount);
                    if (fbPlayerText != null) fbPlayerText.text = fbCurrentMessage + "|";
                }
            };
        }

        formatDashboardContainer = activeRoot.Q<VisualElement>("FormatDashboardContainer");
        fdTimerText = activeRoot.Q<Label>("FdTimerText");
        fdProgressFill = activeRoot.Q<VisualElement>("FdProgressFill");
        for(int i=0; i<4; i++) fdBars[i] = activeRoot.Q<VisualElement>($"FdBar{i+1}");
        for(int i=0; i<3; i++) {
            fdHandles[i] = activeRoot.Q<VisualElement>($"FdHandle{i+1}");
            int idx = i;
            var track = activeRoot.Q<VisualElement>($"FdTrack{i+1}");
            if (track != null) {
                track?.RegisterCallback<PointerMoveEvent>(ev => {
                    if (!isPlaying || currentConfig.type != GameType.FormatDashboard) return;
                    if (ev.pressedButtons != 0) {
                        float relY = ev.localPosition.y;
                        float trackHeight = track.resolvedStyle.height;
                        if (trackHeight <= 0) trackHeight = 200f; // fallback
                        // y=0 is top (100%), y=trackHeight is bottom (0%)
                        float val = 1f - Mathf.Clamp01(relY / trackHeight);
                        // Make sure we only allow dragging UP, or just set it
                        fdValues[idx] = val;
                    }
                });
                track?.RegisterCallback<PointerDownEvent>(ev => {
                    if (!isPlaying || currentConfig.type != GameType.FormatDashboard) return;
                    float relY = ev.localPosition.y;
                    float trackHeight = track.resolvedStyle.height;
                    if (trackHeight <= 0) trackHeight = 200f; // fallback
                    float val = 1f - Mathf.Clamp01(relY / trackHeight);
                    fdValues[idx] = val;
                });
            }
        }

        nightShiftContainer = activeRoot.Q<VisualElement>("NightShiftContainer");
        nsEyelid = activeRoot.Q<VisualElement>("NsEyelid");
        nsTimerText = activeRoot.Q<Label>("NsTimerText");
        nsCoffeeBtn = activeRoot.Q<Button>("NsCoffeeBtn");
        for(int i=0; i<3; i++) {
            nsHandles[i] = activeRoot.Q<VisualElement>($"NsHandle{i+1}");
            int idx = i;
            var track = activeRoot.Q<VisualElement>($"NsTrack{i+1}");
            if (track != null) {
                track?.RegisterCallback<PointerMoveEvent>(ev => {
                    if (!isPlaying || currentConfig.type != GameType.NightShiftPitch) return;
                    if (ev.pressedButtons != 0) {
                        float relX = ev.localPosition.x;
                        float trackW = track.resolvedStyle.width;
                        if (trackW <= 0) trackW = 200f; // fallback
                        float val = Mathf.Clamp01(relX / trackW);
                        nsValues[idx] = val;
                    }
                });
                track?.RegisterCallback<PointerDownEvent>(ev => {
                    if (!isPlaying || currentConfig.type != GameType.NightShiftPitch) return;
                    float relX = ev.localPosition.x;
                    float trackW = track.resolvedStyle.width;
                    if (trackW <= 0) trackW = 200f; // fallback
                    float val = Mathf.Clamp01(relX / trackW);
                    nsValues[idx] = val;
                });
            }
        }
        if (nsCoffeeBtn != null) {
            if (nsCoffeeBtn != null) nsCoffeeBtn.clicked += () => {
                if (!isPlaying || currentConfig.type != GameType.NightShiftPitch) return;
                nsEyelidGravity = Mathf.Max(0f, nsEyelidGravity - 0.25f);
            };
        }

        vrArcadeContainer = activeRoot.Q<VisualElement>("VrArcadeContainer");
        vrBall = activeRoot.Q<VisualElement>("VrBall");
        vrPaddle = activeRoot.Q<VisualElement>("VrPaddle");
        vrScoreText = activeRoot.Q<Label>("VrScoreText");
        vrTouchZone = activeRoot.Q<VisualElement>("VrTouchZone");

        if (vrTouchZone != null) {
            vrTouchZone?.RegisterCallback<PointerMoveEvent>(ev => {
                if (!isPlaying || currentConfig.type != GameType.VrArcadePong) return;
                if (ev.pressedButtons != 0) {
                    vrPaddleX = ev.localPosition.x;
                }
            });
            vrTouchZone?.RegisterCallback<PointerDownEvent>(ev => {
                if (!isPlaying || currentConfig.type != GameType.VrArcadePong) return;
                vrPaddleX = ev.localPosition.x;
            });
        }

        yesManContainer = activeRoot.Q<VisualElement>("YesManContainer");

        mgContainer = activeRoot.Q<UnityEngine.UIElements.VisualElement>("MowGreensContainer");
        wdContainer = activeRoot.Q<UnityEngine.UIElements.VisualElement>("WalkDogsContainer");
        tphContainer = activeRoot.Q<UnityEngine.UIElements.VisualElement>("TwoPhaseHeistContainer"); // Added TwoPhaseHeist
        cgContainer = activeRoot.Q<UnityEngine.UIElements.VisualElement>("CorporateGolfContainer");
        saContainer = activeRoot.Q<UnityEngine.UIElements.VisualElement>("SkimATMContainer");
        if (saContainer != null) {
            saWireContainer = saContainer.Q<UnityEngine.UIElements.VisualElement>("SaWireContainer");
        }
        // IpHack UI Bindings
        ipHackContainer = activeRoot.Q<UnityEngine.UIElements.VisualElement>("IpHackContainer");
        ipHackGridContainer = activeRoot.Q<UnityEngine.UIElements.VisualElement>("IpHackGridContainer");
        ipHackTimerText = activeRoot.Q<UnityEngine.UIElements.Label>("IpHackTimerText");
        if (ipHackContainer != null) {
            for (int i = 0; i < 4; i++) {
                ipHackTargetLabels[i] = ipHackContainer.Q<UnityEngine.UIElements.Label>($"Target{i}");
            }
        }

        // Keep existing unmodified minigame bindings
        snatchBriefcaseContainer = activeRoot.Q<UnityEngine.UIElements.VisualElement>("SnatchBriefcaseContainer");
        for(int i=0; i<3; i++) {
            sbDialStrips[i] = activeRoot.Q<UnityEngine.UIElements.VisualElement>($"SbDial{i}_Strip");
            sbDialsOut[i] = activeRoot.Q<UnityEngine.UIElements.Label>($"SbDial{i}_Out");
            sbDialsTop[i] = activeRoot.Q<UnityEngine.UIElements.Label>($"SbDial{i}_Top");
            sbDialsMid[i] = activeRoot.Q<UnityEngine.UIElements.Label>($"SbDial{i}_Mid");
            sbDialsBot[i] = activeRoot.Q<UnityEngine.UIElements.Label>($"SbDial{i}_Bot");
        }
        sbTargetText = activeRoot.Q<UnityEngine.UIElements.Label>("SbTargetText");
        sbStopBtn = activeRoot.Q<UnityEngine.UIElements.Button>("BtnSbStop");
        if (sbStopBtn != null) if(sbStopBtn != null) sbStopBtn.clicked += () => OnSbStopClicked();

        fpContainer = activeRoot.Q<UnityEngine.UIElements.VisualElement>("ForgePassContainer");
        fpMovingPass = activeRoot.Q<UnityEngine.UIElements.VisualElement>("FpMovingPass");
        fpStampBtn = activeRoot.Q<UnityEngine.UIElements.Button>("BtnFpStamp");
        fpScoreText = activeRoot.Q<UnityEngine.UIElements.Label>("FpScoreText");
        fpPassStampOverlay = activeRoot.Q<UnityEngine.UIElements.Label>("FpPassStampOverlay");
        if (fpStampBtn != null) fpStampBtn.clicked += () => OnFpStampClicked();

        hgContainer = activeRoot.Q<UnityEngine.UIElements.VisualElement>("HospitalityGalaContainer");
        hgIndicator = activeRoot.Q<UnityEngine.UIElements.VisualElement>("HgIndicator");
        hgTimerText = activeRoot.Q<UnityEngine.UIElements.Label>("HgTimerText");
        if (hgContainer != null) {
            hgContainer?.RegisterCallback<UnityEngine.UIElements.PointerMoveEvent>(ev => {
                if (activeGameRoutine != null && currentConfig.type == GameType.HospitalityGala) {
                    float w = hgContainer.resolvedStyle.width;
                    if (w > 0) {
                        float normalizedX = UnityEngine.Mathf.Clamp01(ev.localPosition.x / w);
                        hgVelocity = (normalizedX - 0.5f) * 2f; // -1 to 1 based on touch pos
                    }
                }
            });
        }

        spContainer = activeRoot.Q<UnityEngine.UIElements.VisualElement>("StartupPitchContainer");
        for (int i=0; i<3; i++) spSlots[i] = activeRoot.Q<UnityEngine.UIElements.Label>($"SpSlot{i}");
        spStopBtn = activeRoot.Q<UnityEngine.UIElements.Button>("BtnSpStop");
        if (spStopBtn != null) spStopBtn.clicked += () => OnSpStopClicked();

        hotStoneContainer = activeRoot.Q<VisualElement>("HotStoneContainer");
        hsTimerText = activeRoot.Q<Label>("HsTimerText");
        for (int i = 0; i < 5; i++) {
            hsSlots[i] = activeRoot.Q<VisualElement>($"HsSlot{i}");
            hsStones[i] = activeRoot.Q<VisualElement>($"HsStone{i}");
            int stoneIdx = i; // capture
            if (hsStones[i] != null) {
                hsStones[i]?.RegisterCallback<UnityEngine.UIElements.PointerDownEvent>(ev => OnHsPointerDown(ev, stoneIdx));
                hsStones[i]?.RegisterCallback<UnityEngine.UIElements.PointerMoveEvent>(ev => OnHsPointerMove(ev, stoneIdx));
                hsStones[i]?.RegisterCallback<UnityEngine.UIElements.PointerUpEvent>(ev => OnHsPointerUp(ev, stoneIdx));
                hsStones[i]?.RegisterCallback<UnityEngine.UIElements.PointerCaptureOutEvent>(ev => OnHsPointerUp(ev, stoneIdx));
            }
        }
        ymBossBubble = activeRoot.Q<VisualElement>("YmBossBubble");
        ymBossText = activeRoot.Q<Label>("YmBossText");
        ymPlayerBubble = activeRoot.Q<VisualElement>("YmPlayerBubble");
        ymSwipeZone = activeRoot.Q<VisualElement>("YmSwipeZone");

        if (ymSwipeZone != null) {
            ymSwipeZone?.RegisterCallback<PointerDownEvent>(ev => {
                if (!isPlaying || currentConfig.type != GameType.YesManNod) return;
                ymSwipeStart = ev.position;
                ymNodPhase = 0;
            });
            ymSwipeZone?.RegisterCallback<PointerMoveEvent>(ev => {
                if (!isPlaying || currentConfig.type != GameType.YesManNod) return;
                if (ev.pressedButtons != 0) {
                    float deltaY = ev.position.y - ymSwipeStart.y;
                    if (ymNodPhase == 0 && deltaY < -40f) {
                        ymNodPhase = 1; // Swiped up
                        ymSwipeStart = ev.position; // reset to look for downward motion
                    } else if (ymNodPhase == 1 && deltaY > 40f) {
                        ymNodPhase = 2; // Swiped down
                        ymSwipedUp = true; // Complete nod!
                    }
                }
            });
        }
        if (btnStampApproval != null)
        {
            if (btnStampApproval != null) btnStampApproval.clicked += () => {
                if (isPlaying && currentConfig.type == GameType.DocumentSigner)
                {
                    if (dsTimerText != null) dsTimerText.text = "SIGNED!";
                    MarkWinReady();
                }
            };
        }
        
        for (int i = 0; i < 5; i++)
        {
            atBtnTests[i] = activeRoot.Q<Button>($"AtBtnTest{i}");
            atStatusTexts[i] = activeRoot.Q<Label>($"AtStatusText{i}");
            int index = i; // local capture
            if (atBtnTests[i] != null) atBtnTests[i].clicked += () => OnAtTestClicked(index);
            
            ccLines[i] = activeRoot.Q<Button>($"CcLine{i}");
            ccLineTexts[i] = activeRoot.Q<Label>($"CcLineText{i}");
            if (ccLines[i] != null)
            {
                if (ccLines[i] != null) ccLines[i].clicked += () => OnCcLineClicked(index);
            }
        }
        
        fileSorterTimerText = activeRoot.Q<Label>("FileSorterTimerText");
        fileSorterProgressText = activeRoot.Q<Label>("FileSorterProgressText");
        fileSorterActiveFile = activeRoot.Q<VisualElement>("FileSorterActiveFile");
        fileSorterActiveFileText = activeRoot.Q<Label>("FileSorterActiveFileText");
        
        if (fileSorterActiveFile != null)
        {
            fileSorterActiveFile?.RegisterCallback<PointerDownEvent>(ev => OnFileSorterDown(ev));
            fileSorterActiveFile?.RegisterCallback<PointerMoveEvent>(ev => OnFileSorterMove(ev));
            fileSorterActiveFile?.RegisterCallback<PointerUpEvent>(ev => OnFileSorterUp(ev));
        }



        // Choice
        btnChoice1 = activeRoot.Q<Button>("BtnChoice1");
        btnChoice2 = activeRoot.Q<Button>("BtnChoice2");
        btnChoice3 = activeRoot.Q<Button>("BtnChoice3");
        if (btnChoice1 != null) btnChoice1.clicked += () => OnChoiceClicked(btnChoice1.text);
        if (btnChoice2 != null) btnChoice2.clicked += () => OnChoiceClicked(btnChoice2.text);
        if (btnChoice3 != null) btnChoice3.clicked += () => OnChoiceClicked(btnChoice3.text);

        // Sequence
        sequenceStatusText = activeRoot.Q<Label>("SequenceStatusText");
        for (int i = 0; i < 6; i++)
        {
            int idx = i;
            seqBtns[i] = activeRoot.Q<Button>($"SeqBtn{i}");
            if (seqBtns[i] != null) seqBtns[i].clicked += () => OnSequenceButtonClicked(idx);
        }
        btnReplayPattern = activeRoot.Q<Button>("BtnReplayPattern");
        if (btnReplayPattern != null) btnReplayPattern.clicked += ReplaySequence;

        // Swipe
        swipeObstacleText = activeRoot.Q<Label>("SwipeObstacleText");
        swipeProgressText = activeRoot.Q<Label>("SwipeProgressText");
        swipeTimerText = activeRoot.Q<Label>("SwipeTimerText");
        btnDodgeLeft = activeRoot.Q<Button>("BtnDodgeLeft");
        btnDodgeRight = activeRoot.Q<Button>("BtnDodgeRight");
        if (btnDodgeLeft != null) btnDodgeLeft.clicked += () => OnDodgeClicked(-1);
        if (btnDodgeRight != null) btnDodgeRight.clicked += () => OnDodgeClicked(1);

        // Mash Race
        playerProgressBar = activeRoot.Q<VisualElement>("PlayerProgressBar");
        opponentProgressBar = activeRoot.Q<VisualElement>("OpponentProgressBar");
        playerMashLabel = activeRoot.Q<Label>("PlayerMashLabel");
        opponentMashLabel = activeRoot.Q<Label>("OpponentMashLabel");
        mashTimerText = activeRoot.Q<Label>("MashTimerText");
        btnMashTap = activeRoot.Q<Button>("BtnMashTap");
        if (btnMashTap != null) btnMashTap.clicked += OnMashTapClicked;

        // Memory Match
        matchStatusText = activeRoot.Q<Label>("MatchStatusText");
        memoryGrid = activeRoot.Q<VisualElement>("MemoryGrid");

        // TimingRing
        timingCircle = activeRoot.Q<VisualElement>("TimingCircle");
        timingGlow = activeRoot.Q<VisualElement>("TimingGlow");
        if (timingGlow != null)
        {
            glowLayers = new VisualElement[6];
            for (int i = 0; i < 6; i++)
            {
                glowLayers[i] = timingGlow.Q<VisualElement>($"GlowLayer{i + 1}");
            }
        }
        timingTargetZone = activeRoot.Q<VisualElement>("TimingTargetZone");
        timingMarker = activeRoot.Q<VisualElement>("TimingMarker");
        arcMaskInner = activeRoot.Q<VisualElement>("ArcMaskInner");
        timingRoundText = activeRoot.Q<Label>("TimingRoundText");
        btnTimingStrike = activeRoot.Q<Button>("BtnTimingStrike");
        if (btnTimingStrike != null) btnTimingStrike.clicked += OnTimingStrike;

        // FallingSort
        fallingSortContainer = activeRoot.Q<VisualElement>("FallingSortContainer");
        fallZone = activeRoot.Q<VisualElement>("FallZone");
        binsRow = activeRoot.Q<VisualElement>("BinsRow");
        sortScoreText = activeRoot.Q<Label>("SortScoreText");
        sortTimerText = activeRoot.Q<Label>("SortTimerText");



        // WhackGrid
        whackGrid = activeRoot.Q<VisualElement>("WhackGrid");
        whackScoreText = activeRoot.Q<Label>("WhackScoreText");
        whackTimerText = activeRoot.Q<Label>("WhackTimerText");

        // CodeBreaker
        codeGuessRow = activeRoot.Q<VisualElement>("CodeGuessRow");
        codeSymbolRow = activeRoot.Q<VisualElement>("CodeSymbolRow");
        codeHistoryList = activeRoot.Q<VisualElement>("CodeHistoryList");
        codeAttemptsText = activeRoot.Q<Label>("CodeAttemptsText");
        btnCodeSubmit = activeRoot.Q<Button>("BtnCodeSubmit");
        btnCodeClear = activeRoot.Q<Button>("BtnCodeClear");
        if (btnCodeSubmit != null) btnCodeSubmit.clicked += OnCodeSubmit;
        if (btnCodeClear != null) btnCodeClear.clicked += OnCodeClear;

        // BalanceMeter
        balanceMeterTrack = activeRoot.Q<VisualElement>("BalanceMeterTrack");
        balanceCenterZone = activeRoot.Q<VisualElement>("BalanceCenterZone");
        balanceNeedle = activeRoot.Q<VisualElement>("BalanceNeedle");
        balanceTimeBar = activeRoot.Q<VisualElement>("BalanceTimeBar");
        balanceStatusText = activeRoot.Q<Label>("BalanceStatusText");
        btnNudgeLeft = activeRoot.Q<Button>("BtnNudgeLeft");
        btnNudgeRight = activeRoot.Q<Button>("BtnNudgeRight");
        if (btnNudgeLeft != null) btnNudgeLeft.clicked += () => OnBalanceNudge(-10f);
        if (btnNudgeRight != null) btnNudgeRight.clicked += () => OnBalanceNudge(10f);

        // DialogueChain
        dialogueRoundText = activeRoot.Q<Label>("DialogueRoundText");
        dialogueScenarioText = activeRoot.Q<Label>("DialogueScenarioText");
        rapportBar = activeRoot.Q<VisualElement>("RapportBar");
        btnResponse1 = activeRoot.Q<Button>("BtnResponse1");
        btnResponse2 = activeRoot.Q<Button>("BtnResponse2");
        btnResponse3 = activeRoot.Q<Button>("BtnResponse3");
        if (btnResponse1 != null) btnResponse1.clicked += () => OnDialogueResponse(0);
        if (btnResponse2 != null) btnResponse2.clicked += () => OnDialogueResponse(1);
        if (btnResponse3 != null) btnResponse3.clicked += () => OnDialogueResponse(2);

        // OrderQueue
        orderItemGrid = activeRoot.Q<VisualElement>("OrderItemGrid");
        orderStatusText = activeRoot.Q<Label>("OrderStatusText");
        orderTimerText = activeRoot.Q<Label>("OrderTimerText");

        // DragMatch
        dragMatchContainer = activeRoot.Q<VisualElement>("DragMatchContainer");
        dragLeftColumn = activeRoot.Q<VisualElement>("DragLeftColumn");
        dragRightColumn = activeRoot.Q<VisualElement>("DragRightColumn");
        dragCheatSheetList = activeRoot.Q<VisualElement>("DragCheatSheetList");
        matchPairScoreText = activeRoot.Q<Label>("MatchPairScoreText");

        // SprintLane
        sprintLaneDisplay = activeRoot.Q<VisualElement>("SprintLaneDisplay");
        sprintProgressBar = activeRoot.Q<VisualElement>("SprintProgressBar");
        sprintDistanceText = activeRoot.Q<Label>("SprintDistanceText");
        sprintTimerText = activeRoot.Q<Label>("SprintTimerText");
        sprintObstacleText = activeRoot.Q<Label>("SprintObstacleText");
        btnSprintJump = activeRoot.Q<Button>("BtnSprintJump");
        btnSprintDuck = activeRoot.Q<Button>("BtnSprintDuck");
        if (btnSprintJump != null) btnSprintJump.clicked += () => OnSprintReact(true);
        if (btnSprintDuck != null) btnSprintDuck.clicked += () => OnSprintReact(false);

        // Tetris
        tetrisContainer = activeRoot.Q<VisualElement>("TetrisContainer");
        tetrisGrid = activeRoot.Q<VisualElement>("TetrisGrid");
        tetrisScoreText = activeRoot.Q<Label>("TetrisScoreText");
        btnTetrisLeft = activeRoot.Q<Button>("BtnTetrisLeft");
        btnTetrisRight = activeRoot.Q<Button>("BtnTetrisRight");
        btnTetrisRotate = activeRoot.Q<Button>("BtnTetrisRotate");
        btnTetrisDrop = activeRoot.Q<Button>("BtnTetrisDrop");
        if (btnTetrisLeft != null) btnTetrisLeft.clicked += () => OnTetrisInput(-1, 0, 0);
        if (btnTetrisRight != null) btnTetrisRight.clicked += () => OnTetrisInput(1, 0, 0);
        if (btnTetrisRotate != null) btnTetrisRotate.clicked += () => OnTetrisInput(0, 0, 1);
        if (btnTetrisDrop != null) btnTetrisDrop.clicked += () => OnTetrisInput(0, 1, 0);
        
        acidRainContainer = activeRoot.Q<VisualElement>("AcidRainContainer");
        acidSignPivot = activeRoot.Q<VisualElement>("AcidSignPivot");
        acidDropContainer = activeRoot.Q<VisualElement>("AcidDropContainer");
        acidBurnBar = activeRoot.Q<VisualElement>("AcidBurnBar");
        acidTimerText = activeRoot.Q<Label>("AcidTimerText");
        acidInputArea = activeRoot.Q<VisualElement>("AcidInputArea");
        
        acidInputArea?.RegisterCallback<PointerDownEvent>(ev => { acidInputArea.CapturePointer(ev.pointerId); });
        acidInputArea?.RegisterCallback<PointerUpEvent>(ev => { acidInputArea.ReleasePointer(ev.pointerId); });
        acidInputArea?.RegisterCallback<PointerMoveEvent>(ev => {
            if (currentConfig != null && currentConfig.type == GameType.AcidRainShield && acidInputArea.HasPointerCapture(ev.pointerId))
            {
                acidSignAngle += ev.deltaPosition.x * 0.4f;
                acidSignAngle = Mathf.Clamp(acidSignAngle, -75f, 75f);
                acidSignPivot.style.rotate = new StyleRotate(new Rotate(new Angle(acidSignAngle)));
            }
        });

        // Food Prep Rush
        foodPrepContainer = activeRoot.Q<VisualElement>("FoodPrepContainer");
        if (foodPrepContainer != null)
        {
            prepTimerText = foodPrepContainer.Q<Label>("PrepTimerText");
            prepScoreText = foodPrepContainer.Q<Label>("PrepScoreText");
            for (int i = 0; i < 4; i++)
            {
                int index = i;
                prepBars[i] = foodPrepContainer.Q<VisualElement>($"PrepBar{i}");
                prepStatus[i] = foodPrepContainer.Q<Label>($"StationStatus{i}");
                var btn = foodPrepContainer.Q<Button>($"BtnStation{i}");
                if (btn != null) btn.clicked += () => OnFoodPrepStationTapped(index);
            }
        }// Video Call
        videoCallContainer = activeRoot.Q<VisualElement>("VideoCallContainer");
        if (videoCallContainer != null)
        {
            vcPhase1 = videoCallContainer.Q<VisualElement>("VcPhase1");
            vcPhase2 = videoCallContainer.Q<VisualElement>("VcPhase2");
            vcPhase3 = videoCallContainer.Q<VisualElement>("VcPhase3");
            vcTimerText = videoCallContainer.Q<Label>("VideoCallTimerText");
            vcWigglePromptText = videoCallContainer.Q<Label>("VcWigglePromptText");
            vcWiggleProgressBar = videoCallContainer.Q<VisualElement>("VcWiggleProgressBar");
            vcScrollView = videoCallContainer.Q<ScrollView>("VcScrollView");
            vcWiggleTouchArea = videoCallContainer.Q<VisualElement>("VcWiggleTouchArea");
            vcMouseGraphic = videoCallContainer.Q<VisualElement>("VcMouseGraphic");
            btnVcRaiseHand = videoCallContainer.Q<Button>("BtnVcRaiseHand");
            btnVcAcknowledge = videoCallContainer.Q<Button>("BtnVcAcknowledge");

            if (btnVcRaiseHand != null) btnVcRaiseHand.clicked += OnVcRaiseHandTapped;
            if (btnVcAcknowledge != null) btnVcAcknowledge.clicked += OnVcAcknowledgeTapped;
            
            // Pointer events for swipe/wiggle
            if (vcWiggleTouchArea != null) {
                vcWiggleTouchArea?.RegisterCallback<PointerMoveEvent>(OnVcWiggleMove);
            }
        }

        // Kanban Board
        kanbanContainer = activeRoot.Q<VisualElement>("KanbanContainer");
        if (kanbanContainer != null)
        {
            kanbanTimerText = kanbanContainer.Q<Label>("KanbanTimerText");
            kanbanColTodo = kanbanContainer.Q<VisualElement>("KanbanColTodo");
            kanbanColProgress = kanbanContainer.Q<VisualElement>("KanbanColProgress");
            kanbanColDone = kanbanContainer.Q<VisualElement>("KanbanColDone");
        }

        // Phase 1 UI
        cvContainer = activeRoot.Q<VisualElement>("CloggedVentsContainer");
        if (cvContainer != null) cvGrid = cvContainer.Q<VisualElement>("VentsGrid");

        

        wbContainer = activeRoot.Q<VisualElement>("WhackABugContainer");
        if (wbContainer != null) wbGrid = wbContainer.Q<VisualElement>("BugGrid");





        ztContainer = activeRoot.Q<VisualElement>("ZenTracingContainer");
        if (ztContainer != null)
        {
            ztCircle = ztContainer.Q<VisualElement>("ZtCircle");
            ztTarget = ztContainer.Q<VisualElement>("ZtTarget");
            ztSponge = ztContainer.Q<VisualElement>("ZtSponge");
            ztProgressFill = ztContainer.Q<VisualElement>("ZtProgressFill");
            ztProgressText = ztContainer.Q<Label>("ZtProgressText");
            
            if (ztContainer != null)
            {
                ztContainer?.RegisterCallback<PointerMoveEvent>(ev => {
                    if (currentConfig == null || currentConfig.type != GameType.ZenTracing || !isPlaying) return;
                    Vector2 localPos = ztCircle.WorldToLocal(ev.position);
                    Vector2 center = new Vector2(ztCircle.layout.width / 2f, ztCircle.layout.height / 2f);
                    Vector2 dir = localPos - center;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    if (angle < 0) angle += 360f;
                    ztAngle = angle;
                });
            }
        }

        isContainer = activeRoot.Q<VisualElement>("IceShatterContainer");
        if (isContainer != null) 
        {
            isFrostOverlay = isContainer.Q<VisualElement>("IsFrostOverlay");
            isTimerText = isContainer.Q<Label>("IsTimerText");
            isNodeArea = isContainer.Q<VisualElement>("IsNodeArea");
            isTempFill = isContainer.Q<VisualElement>("IsTempFill");
        }

        nmContainer = activeRoot.Q<VisualElement>("NameMemoryContainer");
        // NameMemory UI elements are queried in SetupNameMemory() now

        gitMergeContainer = activeRoot.Q<VisualElement>("GitMergeContainer");

        rrContainer = activeRoot.Q<UnityEngine.UIElements.VisualElement>("RetroRacerContainer");
        if (rrContainer != null)
        {
            rrPlayer = rrContainer.Q<UnityEngine.UIElements.Label>("RrPlayer");
            rrDistanceLabel = rrContainer.Q<UnityEngine.UIElements.Label>("RrDistanceLabel");
            rrTimeLabel = rrContainer.Q<UnityEngine.UIElements.Label>("RrTimeLabel");
            btnRrLeft = rrContainer.Q<UnityEngine.UIElements.Button>("BtnRrLeft");
            btnRrRight = rrContainer.Q<UnityEngine.UIElements.Button>("BtnRrRight");
            
            for (int i = 0; i < 8; i++) {
                rrRoadLines[i] = rrContainer.Q<UnityEngine.UIElements.VisualElement>($"RrLine{i}");
                rrObstacleElements[i] = rrContainer.Q<UnityEngine.UIElements.Label>($"RrObs{i}");
            }

            rrContainer?.RegisterCallback<UnityEngine.UIElements.PointerDownEvent>(OnRrPointerDown);
            rrContainer?.RegisterCallback<UnityEngine.UIElements.PointerMoveEvent>(OnRrPointerMove);
            rrContainer?.RegisterCallback<UnityEngine.UIElements.PointerUpEvent>(ev => rrIsDragging = false);
            rrContainer?.RegisterCallback<UnityEngine.UIElements.PointerLeaveEvent>(ev => rrIsDragging = false);
        }    }


    private void InitializeConfigs()
    {
        gameConfigs = new Dictionary<string, MiniGameConfig>();

        // =============================================
        // DISHWASHER (6 tasks)
        // =============================================
        AddConfigFull("Scrub dinner plates", new MiniGameConfig { type = GameType.ScrubPlate, targetScore = 15, instructions = "Scrub the plate clean by moving the sponge!" });
        AddConfigFull("Sweep kitchen floors", new MiniGameConfig { type = GameType.SweepFloor, instructions = "Sweep everywhere! Tiles get dirty again after 15s!" });
        AddConfigFull("Mop grease spills", new MiniGameConfig { type = GameType.SweepFloor, instructions = "Mop everywhere! Tiles get dirty again after 15s!" });
        AddConfigFull("Scour microwave grease", new MiniGameConfig { type = GameType.ScrubPlate, targetScore = 15, instructions = "Fast! Scour the grease bubbles!" });
        AddConfigFull("Deep scrub waste containers", new MiniGameConfig { type = GameType.PumpSystem, instructions = "Pump to scrub the containers!" });
        AddConfigFull("Clean fryer grease vats", new MiniGameConfig { type = GameType.PumpSystem, pumpIsBalanceMode = true, pumpBalanceTargetMin = 0.6f, pumpBalanceTargetMax = 0.8f, pumpBalanceHoldTime = 3f, instructions = "Pump to maintain 170°C - 220°C for 3 seconds!" });
        AddConfigFull("Empty waste bin", new MiniGameConfig { type = GameType.GridDodge, gridSize = 5, gridHealth = 3, instructions = "Carry the leaky bags!" });
        AddConfigFull("Unclog kitchen sinks", new MiniGameConfig { type = GameType.PumpSystem, instructions = "Pump to plunge the sink!" });
        AddConfigFull("Emergency deep freeze sorting", new MiniGameConfig { type = GameType.FallingSort, fallingItems = new string[]{"🥩 Beef","🍗 Chicken","🐟 Fish"}, binLabels = new string[]{"Beef","Chicken","Fish"}, sortTarget = 10, sortTimeLimit = 30f, instructions = "Sort the freezer before it melts!" });
        AddConfigFull("Deep clean grease traps", new MiniGameConfig { type = GameType.PumpSystem, instructions = "Pump to empty the grease traps!" });

        // =============================================
        // SCRAP METAL SORTER (6 tasks)
        // =============================================
        AddConfigFull("Sort iron from copper", new MiniGameConfig { type = GameType.FallingSort, fallingItems = new string[]{"🔩 Iron","🟤 Copper","⬜ Alu"}, binLabels = new string[]{"Iron","Copper","Alu"}, sortTarget = 10, sortTimeLimit = 25f, instructions = "Sort falling metal scraps!" });
        AddConfigFull("Stack rusty steel pipes", new MiniGameConfig { type = GameType.Tetris, instructions = "Stack 10 beams perfectly!" });
        AddConfigFull("Clear conveyor dust filters", new MiniGameConfig { type = GameType.CloggedVents, timeLimit = 20f, ventCount = 6, dustBuildRate = 12f, instructions = "Rapidly tap clogged vents before they overheat!" });
        AddConfigFull("Inspect lithium reactor tubes", new MiniGameConfig { type = GameType.DragMatch, matchLeft = new string[]{"Tube A","Tube B","Tube C","Tube D","Tube E"}, matchRight = new string[]{"Critical","Warning","Stable","Optimal","Offline"}, instructions = "Match tubes to statuses!" });
        AddConfigFull("Carry tungsten girders", new MiniGameConfig { type = GameType.GridDodge, gridSize = 5, gridHealth = 3, instructions = "Carry heavy girders safely!" });
        AddConfigFull("Audit sewage drainage lines", new MiniGameConfig { type = GameType.DragMatch, matchLeft = new string[]{"Valve A","Valve B","Valve C","Valve D"}, matchRight = new string[]{"Clear","Blocked","Leaking","Corroded"}, instructions = "Match valves to their status!" });

        // =============================================
        // STREET CORNER SIGN SPINNER (6 tasks)
        // =============================================
        AddConfigFull("Spin arrow sign in traffic", new MiniGameConfig { type = GameType.TimingRing, timingRounds = 5, timingZoneWidth = 16f, instructions = "Hit the timing zones to perform spins!" });
        AddConfigFull("Dance with sign for tourists", new MiniGameConfig { type = GameType.OrderQueue, orderItems = new string[]{"Left","Right","Spin","Pose","Wave","Dip"}, orderPreviewTime = 3f, instructions = "Perform the full dance routine!" });
        AddConfigFull("Wipe smog off the neon arrow", new MiniGameConfig { type = GameType.ScrubPlate, targetScore = 12, instructions = "Apply even pressure to wipe!" });
        AddConfig("Perform a triple backflip spin", GameType.Sequence, "Execute the acrobatic sequence perfectly!", 0, 0, 0, 0, null, null, null, null, 6);
        AddConfigFull("Hold sign in acid rain storm", new MiniGameConfig { type = GameType.AcidRainShield, instructions = "Block the acid rain!" });
        AddConfig("Fight rival sign spinner", GameType.MashRace, "Out-spin your rival! Mash to assert dominance!", 35, 12f);

        // =============================================
        // FAST FOOD SHIFT MANAGER (6 tasks)
        // =============================================
        AddConfigFull("Check fryer oil temperature", new MiniGameConfig { type = GameType.PumpSystem, pumpIsBalanceMode = true, pumpBalanceTargetMin = 0.6f, pumpBalanceTargetMax = 0.8f, pumpBalanceHoldTime = 3f, instructions = "Pump to maintain 170°C - 220°C for 3 seconds!" });
        AddConfigFull("Restock condiment stations", new MiniGameConfig { type = GameType.FallingSort, fallingItems = new string[]{"🍯 Mustard","🥫 Ketchup","🧂 Relish"}, binLabels = new string[]{"Mustard","Ketchup","Relish"}, sortTarget = 10, sortTimeLimit = 30f, instructions = "Restock the falling condiments!" });
        AddConfigFull("Handle customer complaint", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.75f, dialogueScenarios = new string[]{"Customer: My chips are COLD!", "Customer: I WAITED 20 MINUTES!", "Customer: There's a HAIR in my burger!", "Customer: I want to speak to YOUR MANAGER!"}, dialogueOptions = new string[][]{new string[]{"I sincerely apologise","That's not my problem","Have you tried heating them up?"},new string[]{"I'm so sorry for the wait","We're understaffed","You should've ordered online"},new string[]{"That's horrifying, let me replace it","Are you sure it's not yours?","We use synthetic ingredients"},new string[]{"I AM the manager, let me help","They're not available","Go ahead, they'll say the same"}}, dialogueCorrectIndices = new int[]{0,0,0,0}, instructions = "De-escalate the complaint!" });
        AddConfigFull("Emergency food prep rush", new MiniGameConfig { type = GameType.FoodPrepRush, timeLimit = 20f, targetScore = 12, instructions = "Prep food fast! Tap when in the GREEN zone!" });
        AddConfigFull("Train new hire on POS system", new MiniGameConfig { type = GameType.OrderQueue, orderItems = new string[]{"Login","Select Menu","Add Items","Apply Discount","Process Payment","Print Receipt"}, orderPreviewTime = 3f, instructions = "Show them the POS steps!" });

        // =============================================
        // CORPORATE CUBICLE DRONE (6 tasks)
        // =============================================
        AddConfigFull("Send weekly TPS reports", new MiniGameConfig { type = GameType.CodeBreaker, codeSymbols = new string[]{"A","B","C","D","E","F"}, codeLength = 4, maxAttempts = 6, instructions = "Crack the system password to send reports!" });
        AddConfigFull("Attend 3-hour video call", new MiniGameConfig { type = GameType.VideoCall, timeLimit = 15f, targetScore = 1, instructions = "Pay attention!" });
        AddConfigFull("Reorganize shared drive", new MiniGameConfig { type = GameType.FileSorter, timeLimit = 18f, fileSorterTarget = 15, fileSorterItems = new string[]{"Q3_Budget.xlsx", "Q2_Payroll_Final.pdf", "Logo_v4.png", "MeetingNotes.docx", "DO_NOT_DELETE.sys", "ClientList.csv", "Q1_Forecast.xlsx", "Jan_Invoice.pdf", "Deck_v9_Final.pptx"}, instructions = "Swipe files to correct folders!" });
        AddConfigFull("Corporate sprint tasks", new MiniGameConfig { type = GameType.KanbanBoard, timeLimit = 20f, kanbanTicketCount = 8, instructions = "Drag all tickets to IN PROGRESS then DONE!" });
        AddConfigFull("Overnight marketing pitch", new MiniGameConfig { type = GameType.CoffeeWork, timeLimit = 15f, coffeeDrainRate = 22f, coffeeBoost = 20f, workProgressPerTap = 5f, instructions = "Type slides & Drink coffee to stay awake!" });
        AddConfigFull("Agree with executive complaints", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.8f, dialogueScenarios = new string[]{"VP: Q3 numbers are DISASTROUS!", "VP: Marketing spent HOW MUCH?!", "VP: Why can't we be more like Apple?!"}, dialogueOptions = new string[][]{new string[]{"You're absolutely right, sir","Actually the data shows...","That's above my pay grade"},new string[]{"Outrageous, I agree completely","It was within budget actually","I'll look into it"},new string[]{"Brilliant observation, sir","We have different strengths","I'll draft a proposal"}}, dialogueCorrectIndices = new int[]{0,0,0}, instructions = "Agree enthusiastically!" });

        // =============================================
        // JUNIOR REACT TRAINEE (6 tasks)
        // =============================================
        AddConfigFull("Test program accuracy", new MiniGameConfig { 
            type = GameType.UnitTestRunner, 
            timeLimit = 15f,
            unitTestRounds = 8,
            unitTestQuestions = new string[] {
                "Assert.AreEqual(?, 2 + 2)",
                "Assert.IsTrue(? > 10)",
                "Assert.IsNull(?)",
                "Assert.IsFalse(? == true)",
                "Assert.AreEqual(\"test\", ?)",
                "Assert.Throws<?>(() => Crash())",
                "Assert.IsNotNull(?)",
                "Assert.AreEqual(?, Math.Abs(-5))"
            },
            unitTestOptions = new string[][] {
                new string[] { "3", "4", "5" },
                new string[] { "5", "10", "15" },
                new string[] { "\"data\"", "0", "null" },
                new string[] { "true", "false", "null" },
                new string[] { "\"unit\"", "\"test\"", "null" },
                new string[] { "Exception", "String", "Int" },
                new string[] { "null", "new Object()", "0" },
                new string[] { "-5", "0", "5" }
            },
            unitTestCorrectIndices = new int[] { 1, 2, 2, 1, 1, 0, 1, 2 },
            instructions = "Rapid fire unit tests!" 
        });
        AddConfigFull("Fix CSS layout locations", new MiniGameConfig { type = GameType.CenterDiv, timeLimit = 15f, centerDivTargets = 3, centerDivSpeedMin = 300f, centerDivSpeedMax = 700f, instructions = "Center the div exactly!" });
        AddConfigFull("Fix CSS layout sizes", new MiniGameConfig { type = GameType.SizeMatch, timeLimit = 15f, instructions = "Match the exact size!" });
        AddConfigFull("Center content icons", new MiniGameConfig { type = GameType.DragCenter, timeLimit = 15f, instructions = "Drag the icon perfectly to the center!" });
        AddConfigFull("Resolve Git merge conflicts", new MiniGameConfig { type = GameType.GitMerge, timeLimit = 15f, targetScore = 5, instructions = "Tap the correct block to resolve conflicts!" });
        AddConfigFull("Debug CSS at 3 AM", new MiniGameConfig { type = GameType.CoffeeWork, timeLimit = 15f, coffeeDrainRate = 22f, coffeeBoost = 20f, workProgressPerTap = 5f, instructions = "Debug CSS & Drink coffee!" });
        AddConfigFull("Inject untested code", new MiniGameConfig { type = GameType.StealthInject, timeLimit = 15f, instructions = "Hold INJECT while the reviewer is distracted!" });

        // =============================================
        // DYSTOPIAN QA TESTER (6 tasks)
        // =============================================
        AddConfigFull("Run automated test suite", new MiniGameConfig { type = GameType.AutomatedTests, timeLimit = 15f, instructions = "Re-run any flaky tests before they crash the suite!" });
        AddConfigFull("File bug report", new MiniGameConfig { type = GameType.QATriage, timeLimit = 20f, targetScore = 5, qaSeverities = new string[]{"Minor", "Major", "Critical"}, qaCategories = new string[]{"UI", "Network", "Logic"}, qaBugLogs = new string[]{"NullReferenceException in Login()", "Timeout 504 on API", "Button overlaps text", "Infinite loop on checkout", "Database connection lost"}, instructions = "Drag the bug report to the correct department folder!" });
        AddConfigFull("Verify hotfix deployment", new MiniGameConfig { type = GameType.OrderQueue, orderItems = new string[]{"Build","Unit Test","Integration","Stage Deploy","Smoke Test","Prod Deploy"}, orderPreviewTime = 3f, instructions = "Run through deployment stages!" });
        AddConfigFull("Regression test full app", new MiniGameConfig { type = GameType.WhackABug, timeLimit = 20f, targetScore = 15, instructions = "Tap failing (red) modules to pass them!" });
        AddConfig("Stress test server load", GameType.MashRace, "Hammer the server! Mash faster!", 40, 15f);
        AddConfigFull("Write flaky test workaround", new MiniGameConfig { type = GameType.CodeCommenter, timeLimit = 10f, instructions = "Comment out the flaky tests before the build fails!" });

        // =============================================
        // EXECUTIVE INTERSTELLAR CEO (6 tasks)
        // =============================================
        AddConfigFull("Approve quarterly budget", new MiniGameConfig { type = GameType.DocumentSigner, timeLimit = 4f, instructions = "Scroll to the bottom and STAMP APPROVAL!" });
        AddConfigFull("Board meeting presentation", new MiniGameConfig { type = GameType.ChartAdjuster, timeLimit = 10f, instructions = "Fudge the presentation! Shrink them, grow us!" });
        AddConfigFull("Sign regulatory compliance docs", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.75f, dialogueScenarios = new string[]{"Legal: Clause 4.7 requires data residency","Legal: Section 12 has liability gaps","Legal: GDPR compliance needs updating","Legal: Sign here to accept all terms"}, dialogueOptions = new string[][]{new string[]{"Flag for legal review","Sign it anyway","Delegate to VP"},new string[]{"Request amendment","Ignore the gaps","That's fine"},new string[]{"Engage compliance team","We'll deal with it later","What's GDPR?"},new string[]{"Review one more time","Sign immediately","Have my assistant sign"}}, dialogueCorrectIndices = new int[]{0,0,0,0}, instructions = "Handle compliance carefully!" });
        AddConfigFull("Hostile takeover negotiations", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.8f, dialogueScenarios = new string[]{"Their CEO: Our company isn't for sale","Their CEO: Your offer is insulting","Board: The shareholders are nervous","Media: Is this a hostile takeover?","Their CEO: Final counteroffer - $2B"}, dialogueOptions = new string[][]{new string[]{"Everyone has a price","Let's discuss the benefits","We'll go directly to shareholders"},new string[]{"We'll raise to fair market value","Take it or leave it","Let's find common ground"},new string[]{"This acquisition creates value","Shareholders should trust us","We'll sweeten the deal"},new string[]{"It's a strategic partnership","No comment","We're creating jobs"},new string[]{"Deal. Let's close this","We can go higher","Let me consult my board"}}, dialogueCorrectIndices = new int[]{1,0,0,0,0}, instructions = "Negotiate the takeover!" });
        AddConfigFull("Fire underperforming VP", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.7f, dialogueScenarios = new string[]{"VP: I've been with the company 15 years!","VP: My team loves me!","VP: What about my stock options?"}, dialogueOptions = new string[][]{new string[]{"And we value that. Here's a generous package","That's exactly the problem","Security will escort you out"},new string[]{"They'll be taken care of","They'll manage without you","We're restructuring the team"},new string[]{"They'll vest on schedule","Forfeited as per contract","We'll negotiate"}}, dialogueCorrectIndices = new int[]{0,0,0}, instructions = "Fire them smoothly!" });
        AddConfigFull("Construction site inspection", new MiniGameConfig { type = GameType.ConstructionSite, timeLimit = 15f, balanceDriftSpeed = 0f, instructions = "Scan the wall for heat leaks!" });

        // =============================================
        // AI PROPHECY ALIGNMENT DIRECTOR (6 tasks)
        // =============================================
        AddConfigFull("Calibrate neural alignment matrix", new MiniGameConfig { type = GameType.NeuralAligner, timeLimit = 12f, instructions = "Tune the sliders until all lights are GREEN!" });
        AddConfigFull("Review AI ethics proposals", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.8f, dialogueScenarios = new string[]{"Proposal: Give AI emotional awareness","Proposal: AI self-modification capabilities","Proposal: Autonomous weapons systems","Proposal: AI voting rights","Proposal: Merge human consciousness with AI"}, dialogueOptions = new string[][]{new string[]{"Approve with strict monitoring","Reject — too dangerous","Classify top-secret"},new string[]{"Reject — existential risk","Approve if sandboxed","Needs more research"},new string[]{"Absolutely not","Only for defense","Approve with oversight"},new string[]{"Premature but worth studying","Approve immediately","Reject — they're not people"},new string[]{"Fund research cautiously","Too sci-fi, reject","Approve clinical trials"}}, dialogueCorrectIndices = new int[]{0,0,0,0,0}, instructions = "Judge AI proposals!" });
        AddConfigFull("Monitor sentience detection array", new MiniGameConfig { type = GameType.SentienceArray, timeLimit = 12f, instructions = "Whack the red sentience spikes!" });
        AddConfigFull("Emergency AI containment protocol", new MiniGameConfig { type = GameType.ContainmentProtocol, timeLimit = 10f, instructions = "Sever hardlines! Lift cover! Mash lockdown!" });
        AddConfigFull("Align superintelligence reward function", new MiniGameConfig { type = GameType.RewardLogic, timeLimit = 12f, instructions = "Route logic to PRESERVE humanity!" });
        AddConfigFull("Present findings to Galactic Council", new MiniGameConfig { type = GameType.GalacticCouncil, timeLimit = 15f, instructions = "Dodge objections and finish speech!" });

        // =============================================
        // FALLBACK TASKS (6 tasks)
        // =============================================
        AddConfigFull("Respond to supervisor", new MiniGameConfig { type = GameType.FranticBackspace, timeLimit = 6f, instructions = "FRANTIC BACKSPACE! DELETE YOUR HONEST THOUGHTS!" });
        AddConfigFull("Categorize ticket logs", new MiniGameConfig { type = GameType.TicketSwipe, timeLimit = 20f, targetScore = 10, instructions = "Correctly categorize 10 tickets before time runs out!" });
        AddConfigFull("Format summary dashboards", new MiniGameConfig { type = GameType.FormatDashboard, timeLimit = 15f, instructions = "Make it POP! Swipe all 3 sliders up and HOLD them in the green!" });
        AddConfigFull("Do corporate sprint tasks aggressively", new MiniGameConfig { type = GameType.KanbanBoard, timeLimit = 15f, kanbanTicketCount = 12, instructions = "Drag ALL tickets to IN PROGRESS then DONE! FAST!" });
        AddConfigFull("Stay overnight to align marketing pitch variables", new MiniGameConfig { type = GameType.NightShiftPitch, timeLimit = 15f, instructions = "Drink coffee to stay awake and align the variables!" });
        AddConfigFull("Agree verbally with automated executive complaints", new MiniGameConfig { type = GameType.YesManNod, timeLimit = 15f, instructions = "Nod (SWIPE UP) aggressively to every complaint!" });

        // =============================================
        // WELLNESS ACTIVITIES (5 tasks)
        // =============================================
        AddConfigFull("Walk in the Park", new MiniGameConfig { type = GameType.GridDodge, gridSize = 5, gridHealth = 5, instructions = "A peaceful walk." });
        AddConfigFull("Capsule Arcade VR", new MiniGameConfig { type = GameType.VrArcadePong, timeLimit = 15f, instructions = "Keep the ball alive! (Bounce 5 times)" });
        AddConfigFull("Volcanic Hot Stone Spa", new MiniGameConfig { type = GameType.HotStoneSpa, timeLimit = 15f, instructions = "Place the hot stones along the spine to relax." });
        AddConfigFull("Suburban Sensory Mud Spa", new MiniGameConfig { type = GameType.ZenTracing, timeLimit = 15f, instructions = "Keep the sponge in the target circle!" });
        AddConfigFull("Luxury Cryo Therapy", new MiniGameConfig { type = GameType.IceShatter, timeLimit = 15f, instructions = "Tap ice nodes before you freeze!" });

        // =============================================
        // MIXERS (4 tasks)
        // =============================================
        AddConfigFull("Industrial Dive Bar", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.6f, dialogueScenarios = new string[]{"Foreman: What brings you here?","Worker: You look too clean for this place","Bartender: Another round?"}, dialogueOptions = new string[][]{new string[]{"Looking for work connections","Just passing through","Buy everyone a round"},new string[]{"I work hard too","I just got off shift","You should see my hands"},new string[]{"Absolutely, my treat","I'll have water","One more then I'm out"}}, dialogueCorrectIndices = new int[]{2,1,0}, instructions = "Network at the dive bar!" });
        AddConfigFull("Hospitality Gala", new MiniGameConfig { type = GameType.NameMemory, timeLimit = 15f, vipNames = new string[]{"Sarah from Tech", "John from Sales", "Mike from HR", "Emma from Legal"}, instructions = "Memorize the VIPs!" });
        AddConfigFull("Startup Pitch Meetup", new MiniGameConfig { type = GameType.StartupPitch, timeLimit = 15f, instructions = "Pitch the perfect buzzwords!" });
        AddConfigFull("Corporate Golf Sim", new MiniGameConfig { type = GameType.CorporateGolf, timeLimit = 15f, instructions = "Let the CEO win! Aim for the sand trap!" });

        // =============================================
        // LEGAL SIDE HUSTLES (4 tasks)
        // =============================================
        AddConfigFull("Collect Recycled Bottles", new MiniGameConfig { type = GameType.FallingSort, fallingItems = new string[]{"🟢 Glass","🔵 Plastic","🟤 Aluminium"}, binLabels = new string[]{"Glass","Plastic","Metal"}, sortTarget = 10, sortTimeLimit = 20f, instructions = "Sort the recyclables!" });
        AddConfigFull("Executive Lawn Care", new MiniGameConfig { type = GameType.MowGreens, timeLimit = 15f, instructions = "Mow the grass, avoid obstacles!" });
        AddConfigFull("Walk Purebred Pedigree Dogs", new MiniGameConfig { type = GameType.WalkDogs, timeLimit = 15f, instructions = "Keep all 3 leashes in the green zone!" });
        AddConfigFull("Deliver Fast Food Order", new MiniGameConfig { type = GameType.RetroRacer, timeLimit = 20f, instructions = "Dodge traffic and deliver the food!" });

        // =============================================
        // ILLEGAL SIDE HUSTLES (4 tasks)
        // =============================================
        AddConfigFull("Forge Transit Passes", new MiniGameConfig { type = GameType.ForgePass, timeLimit = 10f, instructions = "Stamp exactly in the center!" });
        AddConfigFull("Crack Luxury Briefcase", new MiniGameConfig { type = GameType.SnatchBriefcase, timeLimit = 15f, instructions = "Crack the lock!" });
        AddConfigFull("Skim ATM Card Data", new MiniGameConfig { type = GameType.SkimATM, timeLimit = 10f, instructions = "Connect the skimmer wires!" });
        AddConfigFull("Corporate Espionage Heist", new MiniGameConfig { type = GameType.IpHack, timeLimit = 30f, instructions = "Hack the IP Address!" });
    }

    private void AddConfig(string name, GameType type, string instructions, int targetScore = 0, float timeLimit = 0, float sMin = 0, float sMax = 0, string c1 = null, string c2 = null, string c3 = null, string cCorrect = null, int seqLen = 4, int pairs = 4)
    {
        gameConfigs[name] = new MiniGameConfig
        {
            type = type,
            instructions = instructions,
            targetScore = targetScore,
            timeLimit = timeLimit,
            sliderMin = sMin,
            sliderMax = sMax,
            choices = (c1 != null) ? new string[] { c1, c2, c3 } : null,
            correctChoice = cCorrect,
            sequenceLength = seqLen,
            pairs = pairs
        };
    }

    private void AddConfigFull(string name, MiniGameConfig config)
    {
        gameConfigs[name] = config;
    }

    public Dictionary<string, MiniGameConfig> GameConfigs => gameConfigs;

    private MiniGameConfig GetConfigForGame(string gameName)
    {
        if (gameConfigs.TryGetValue(gameName, out var config)) return config;

        int hash = Mathf.Abs(gameName.GetHashCode());
        GameType randomType = GameType.Tap;

        return new MiniGameConfig
        {
            type = randomType,
            instructions = "Complete the designated task.",
            targetScore = 15,
            timeLimit = 15f,
            sliderMin = 40f,
            sliderMax = 60f,
            choices = new string[] { "Option A", "Option B", "Option C" },
            correctChoice = "Option A",
            sequenceLength = 4,
            pairs = 4
        };
    }

    private TemplateContainer activeGameUI;

    private string GetUxmlName(GameType type)
    {
        switch(type)
        {
            case GameType.CloggedVents: return "CloggedVents";
            case GameType.WhackABug: return "WhackABug";
            case GameType.ZenTracing: return "ZenTracing";
            case GameType.IceShatter: return "IceShatter";
            case GameType.NameMemory: return "NameMemory";
            case GameType.RetroRacer: return "RetroRacer";
            case GameType.Choice: return "ChoiceGame";
            case GameType.Sequence: return "SequenceGame";
            case GameType.Swipe: return "SwipeGame";
            case GameType.MashRace: return "MashRace";
            case GameType.MemoryMatch: return "MemoryMatch";
            case GameType.TimingRing: return "TimingRing";
            case GameType.FallingSort: return "FallingSort";
            case GameType.WhackGrid: return "WhackGrid";
            case GameType.SweepFloor: return "SweepFloor";
            case GameType.PumpSystem: return "PumpGame";
            case GameType.ScrubPlate: return "ScrubPlate";
            case GameType.CodeBreaker: return "CodeBreaker";
            case GameType.BalanceMeter: return "BalanceMeter";
            case GameType.DialogueChain: return "DialogueChain";
            case GameType.OrderQueue: return "OrderQueue";
            case GameType.DragMatch: return "DragMatch";
            case GameType.SprintLane: return "SprintLane";
            case GameType.Tetris: return "Tetris";
            case GameType.AcidRainShield: return "AcidRain";
            case GameType.VideoCall: return "VideoCall";
            case GameType.KanbanBoard: return "Kanban";
            case GameType.FileSorter: return "FileSorter";
            case GameType.CoffeeWork: return "CoffeeWork";
            case GameType.UnitTestRunner: return "UnitTest";
            case GameType.CenterDiv: return "CenterDiv";
            case GameType.DragCenter: return "DragCenter";
            case GameType.SizeMatch: return "SizeMatch";
            case GameType.StealthInject: return "StealthInject";
            case GameType.AutomatedTests: return "AutomatedTests";
            case GameType.CodeCommenter: return "CodeCommenter";
            case GameType.DocumentSigner: return "DocumentSigner";
            case GameType.ChartAdjuster: return "ChartAdjuster";
            case GameType.VrArcadePong: return "VrArcade";
            case GameType.HotStoneSpa: return "HotStone";
            case GameType.ForgePass: return "ForgePass";
            case GameType.SnatchBriefcase: return "SnatchBriefcase";
            case GameType.SkimATM: return "SkimATM";
            case GameType.IpHack: return "IpHack";
            case GameType.NightShiftPitch: return "NightShift";
            case GameType.YesManNod: return "YesMan";
            case GameType.FormatDashboard: return "FormatDashboard";
            case GameType.FranticBackspace: return "FranticBackspace";
            case GameType.SentienceArray: return "SentienceArray";
            case GameType.ContainmentProtocol: return "ContainmentProtocol";
            case GameType.RewardLogic: return "RewardLogic";
            case GameType.GalacticCouncil: return "GalacticCouncil";
            case GameType.NeuralAligner: return "NeuralAligner";
            case GameType.FoodPrepRush: return "FoodPrep";
            case GameType.Tap: return "TapGame";
            case GameType.Slider: return "SliderGame";
            case GameType.TwoPhaseHeist: return "TwoPhaseHeist";
            case GameType.QATriage: return "QATriage";
            case GameType.TicketSwipe: return "TicketSwipe";
            case GameType.ConstructionSite: return "ConstructionSite";
            case GameType.GitMerge: return "GitMerge";
            case GameType.HospitalityGala: return "HospitalityGala";
            case GameType.CorporateGolf: return "CorporateGolf";
            case GameType.StartupPitch: return "StartupPitch";
            case GameType.MowGreens: return "MowGreens";
            case GameType.WalkDogs: return "WalkDogs";
            case GameType.RecycleBottles: return "RecycleBottles";
            default: return type.ToString();
        }
    }

    public void LaunchMiniGame(ActiveMiniGame game)
    {
        currentGame = game;
        currentConfig = GetConfigForGame(game.name);
        
        score = 0;
        hasWon = false;
        isPlaying = true;
        currentPhase = 1;

        if (activeGameRoutine != null) StopCoroutine(activeGameRoutine);

        operationNameText.text = game.name;
        instructionsText.text = currentConfig.instructions;
        overlay.style.display = DisplayStyle.Flex;
        overlay.pickingMode = PickingMode.Position;

        HideAllContainers();
        GameType typeToSetup = currentConfig.type;
        
        // Delegate to new Strategy Pattern
        currentStrategy = MiniGameFactory.Create(typeToSetup);

        // Phase 4 Automation: Automatically load UI templates if they exist
        string uxmlName = GetUxmlName(typeToSetup);
        var template = Resources.Load<VisualTreeAsset>("MiniGames/" + uxmlName);
        
        if (template != null)
        {
            activeGameUI = template.Instantiate();
            activeGameUI.style.flexGrow = 1; // Ensure it expands
            
            var targetContainer = overlay.Q<ScrollView>() as VisualElement ?? overlay;
            targetContainer.Add(activeGameUI);
            
            BindLegacyUI(activeGameUI);
            activeGameUI.Focus();
            
            currentStrategy.Initialize(this, activeGameUI, currentConfig);
        }
        else
        {
            // Fallback to legacy massive UI document
            currentStrategy.Initialize(this, miniGameDocument.rootVisualElement, currentConfig);
        }
        
        currentStrategy.StartGame();
    }

    private void HideAllContainers()
    {
        if(ipHackContainer != null) ipHackContainer.style.display = DisplayStyle.None;
        if(tapContainer != null) tapContainer.style.display = DisplayStyle.None;
        if(sliderContainer != null) sliderContainer.style.display = DisplayStyle.None;
        if(choiceContainer != null) choiceContainer.style.display = DisplayStyle.None;
        if(sequenceContainer != null) sequenceContainer.style.display = DisplayStyle.None;
        if(swipeContainer != null) swipeContainer.style.display = DisplayStyle.None;
        if(mashRaceContainer != null) mashRaceContainer.style.display = DisplayStyle.None;
        if(memoryMatchContainer != null) memoryMatchContainer.style.display = DisplayStyle.None;
        
        // New containers
        if(timingRingContainer != null) timingRingContainer.style.display = DisplayStyle.None;
        if(fallingSortContainer != null) fallingSortContainer.style.display = DisplayStyle.None;
        if(gridDodgeContainer != null) gridDodgeContainer.style.display = DisplayStyle.None;
        if(whackGridContainer != null) whackGridContainer.style.display = DisplayStyle.None;
        if(sweepFloorContainer != null) sweepFloorContainer.style.display = DisplayStyle.None;
        if(scrubPlateContainer != null) scrubPlateContainer.style.display = DisplayStyle.None;
        if(codeBreakerContainer != null) codeBreakerContainer.style.display = DisplayStyle.None;
        if(balanceMeterContainer != null) balanceMeterContainer.style.display = DisplayStyle.None;
        if(dialogueChainContainer != null) dialogueChainContainer.style.display = DisplayStyle.None;
        if(orderQueueContainer != null) orderQueueContainer.style.display = DisplayStyle.None;
        if(dragMatchContainer != null) dragMatchContainer.style.display = DisplayStyle.None;
        if(sprintLaneContainer != null) sprintLaneContainer.style.display = DisplayStyle.None;
        if(pumpGameContainer != null) pumpGameContainer.style.display = DisplayStyle.None;
        if(tetrisContainer != null) tetrisContainer.style.display = DisplayStyle.None;
        if(acidRainContainer != null) acidRainContainer.style.display = DisplayStyle.None;
        if(foodPrepContainer != null) foodPrepContainer.style.display = DisplayStyle.None;
        if(videoCallContainer != null) videoCallContainer.style.display = DisplayStyle.None;
        if(kanbanContainer != null) kanbanContainer.style.display = DisplayStyle.None;
        if(fileSorterContainer != null) fileSorterContainer.style.display = DisplayStyle.None;
        if(coffeeWorkContainer != null) coffeeWorkContainer.style.display = DisplayStyle.None;
        if(unitTestContainer != null) unitTestContainer.style.display = DisplayStyle.None;
        if(centerDivContainer != null) centerDivContainer.style.display = DisplayStyle.None;
        if(dragCenterContainer != null) dragCenterContainer.style.display = DisplayStyle.None;
        if(sizeMatchContainer != null) sizeMatchContainer.style.display = DisplayStyle.None;
        if(stealthInjectContainer != null) stealthInjectContainer.style.display = DisplayStyle.None;
        if(automatedTestsContainer != null) automatedTestsContainer.style.display = DisplayStyle.None;
        if(codeCommenterContainer != null) codeCommenterContainer.style.display = DisplayStyle.None;
        if(documentSignerContainer != null) documentSignerContainer.style.display = DisplayStyle.None;
        if(chartAdjusterContainer != null) chartAdjusterContainer.style.display = DisplayStyle.None;
        if(neuralAlignerContainer != null) neuralAlignerContainer.style.display = DisplayStyle.None;
        if(sentienceArrayContainer != null) sentienceArrayContainer.style.display = DisplayStyle.None;
        if(containmentProtocolContainer != null) containmentProtocolContainer.style.display = DisplayStyle.None;
        if(rewardLogicContainer != null) rewardLogicContainer.style.display = DisplayStyle.None;
        if(galacticCouncilContainer != null) galacticCouncilContainer.style.display = DisplayStyle.None;
        if(franticBackspaceContainer != null) franticBackspaceContainer.style.display = DisplayStyle.None;
        if(formatDashboardContainer != null) formatDashboardContainer.style.display = DisplayStyle.None;
        if(nightShiftContainer != null) nightShiftContainer.style.display = DisplayStyle.None;
        if(yesManContainer != null) yesManContainer.style.display = DisplayStyle.None;
        if(vrArcadeContainer != null) vrArcadeContainer.style.display = DisplayStyle.None;
        if(hotStoneContainer != null) hotStoneContainer.style.display = DisplayStyle.None;
        if(hgContainer != null) hgContainer.style.display = DisplayStyle.None;
        if(spContainer != null) spContainer.style.display = DisplayStyle.None;
        if(cgContainer != null) cgContainer.style.display = DisplayStyle.None;
        if(mgContainer != null) mgContainer.style.display = DisplayStyle.None;
        if(wdContainer != null) wdContainer.style.display = DisplayStyle.None;
        if(fpContainer != null) fpContainer.style.display = DisplayStyle.None;
        if(snatchBriefcaseContainer != null) snatchBriefcaseContainer.style.display = DisplayStyle.None;
        if(saContainer != null) saContainer.style.display = DisplayStyle.None;
        if(tphContainer != null) tphContainer.style.display = DisplayStyle.None;

        // Phase 1 Custom
        if(cvContainer != null) cvContainer.style.display = DisplayStyle.None;
        
        if(wbContainer != null) wbContainer.style.display = DisplayStyle.None;

        // Phase 2 Custom
        if(ztContainer != null) ztContainer.style.display = DisplayStyle.None;
        if(isContainer != null) isContainer.style.display = DisplayStyle.None;
        if(nmContainer != null) nmContainer.style.display = DisplayStyle.None;
        if(rrContainer != null) rrContainer.style.display = DisplayStyle.None;
        if(gitMergeContainer != null) gitMergeContainer.style.display = DisplayStyle.None;
    }
    public void SetupGameType(GameType type)
    {
        switch (type)
        {
            case GameType.CloggedVents:
                cvContainer.style.display = DisplayStyle.Flex;
                SetupCloggedVents();
                break;
            case GameType.QATriage:
                // Handled by Strategy
                break;
            case GameType.WhackABug:
                wbContainer.style.display = DisplayStyle.Flex;
                SetupWhackABug();
                break;


            case GameType.ZenTracing:
                ztContainer.style.display = DisplayStyle.Flex;
                SetupZenTracing();
                break;
            case GameType.IceShatter:
                isContainer.style.display = DisplayStyle.Flex;
                SetupIceShatter();
                break;
            case GameType.NameMemory:
                nmContainer.style.display = DisplayStyle.Flex;
                SetupNameMemory();
                break;
            case GameType.RetroRacer:
                rrContainer.style.display = DisplayStyle.Flex;
                SetupRetroRacer();
                break;


            case GameType.Choice:
                choiceContainer.style.display = DisplayStyle.Flex;
                btnChoice1.text = currentConfig.choices[0];
                btnChoice2.text = currentConfig.choices[1];
                btnChoice3.text = currentConfig.choices[2];
                break;

            case GameType.Sequence:
                sequenceContainer.style.display = DisplayStyle.Flex;
                if (btnReplayPattern != null)
                    btnReplayPattern.style.display = (currentGame.name == "Perform a triple backflip spin") ? DisplayStyle.None : DisplayStyle.Flex;
                GenerateSequence(currentConfig.sequenceLength);
                activeGameRoutine = StartCoroutine(PlaySequenceRoutine());
                break;

            case GameType.Swipe:
                swipeContainer.style.display = DisplayStyle.Flex;
                score = 0;
                timer = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 15f;
                swipeProgressText.text = $"Dodged: {score} / {currentConfig.targetScore}";
                activeGameRoutine = StartCoroutine(SwipeGameRoutine());
                break;

            case GameType.MashRace:
                mashRaceContainer.style.display = DisplayStyle.Flex;
                score = 0;
                playerMashProgress = 0f;
                opponentProgress = 0f;
                timer = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 10f;
                playerProgressBar.style.width = new Length(0, LengthUnit.Percent);
                opponentProgressBar.style.width = new Length(0, LengthUnit.Percent);
                
                if (currentGame.name == "Stress test server load")
                {
                    if (playerMashLabel != null) { playerMashLabel.text = "INCOMING TRAFFIC"; playerMashLabel.style.color = new StyleColor(ColorUtility.TryParseHtmlString("#e67e22", out Color c1) ? c1 : Color.yellow); }
                    if (opponentMashLabel != null) { opponentMashLabel.text = "SERVER LOAD (HEAT)"; opponentMashLabel.style.color = new StyleColor(ColorUtility.TryParseHtmlString("#e74c3c", out Color c2) ? c2 : Color.red); }
                    playerProgressBar.style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#e67e22", out Color c3) ? c3 : Color.yellow);
                    opponentProgressBar.style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#e74c3c", out Color c4) ? c4 : Color.red);
                    mashRaceContainer.style.backgroundColor = new StyleColor(new Color(0.1f, 0.05f, 0.05f));
                }
                else
                {
                    if (playerMashLabel != null) { playerMashLabel.text = "YOUR PROGRESS"; playerMashLabel.style.color = new StyleColor(ColorUtility.TryParseHtmlString("#2ECC71", out Color c5) ? c5 : Color.green); }
                    if (opponentMashLabel != null) { opponentMashLabel.text = "OPPONENT"; opponentMashLabel.style.color = new StyleColor(ColorUtility.TryParseHtmlString("#E74C3C", out Color c6) ? c6 : Color.red); }
                    playerProgressBar.style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#2ECC71", out Color c7) ? c7 : Color.green);
                    opponentProgressBar.style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#E74C3C", out Color c8) ? c8 : Color.red);
                    mashRaceContainer.style.backgroundColor = new StyleColor(Color.clear);
                }

                activeGameRoutine = StartCoroutine(MashRaceRoutine());
                break;

            case GameType.MemoryMatch:
                memoryMatchContainer.style.display = DisplayStyle.Flex;
                SetupMemoryMatch(currentConfig.pairs);
                break;

            case GameType.TimingRing:
                timingRingContainer.style.display = DisplayStyle.Flex;
                timingTargetAngle = Random.Range(0f, 360f);
                timingMarkerAngle = 0f;
                timingSpinDir = 1;
                timingCurrentRound = 0;
                activeGameRoutine = StartCoroutine(TimingRingRoutine());
                break;
                
            case GameType.FallingSort:
                fallingSortContainer.style.display = DisplayStyle.Flex;
                SetupFallingSort();
                sortScore = 0;
                timer = currentConfig.sortTimeLimit;
                activeGameRoutine = StartCoroutine(FallingSortRoutine());
                break;
                

            case GameType.WhackGrid:
                whackGridContainer.style.display = DisplayStyle.Flex;
                whackScore = 0;
                timer = currentConfig.whackTime;
                SetupWhackGrid();
                activeGameRoutine = StartCoroutine(WhackGridRoutine());
                break;
                
            case GameType.SweepFloor:
                sweepFloorContainer.style.display = DisplayStyle.Flex;
                SetupSweepFloor();
                miniGameDocument.rootVisualElement.Focus();
                activeGameRoutine = StartCoroutine(SweepFloorRoutine());
                break;
                
            case GameType.PumpSystem:
                pumpGameContainer.style.display = DisplayStyle.Flex;
                pumpProgress = 0f;
                pumpHoldTimer = 0f;
                pumpProgressBar.style.height = Length.Percent(0f);
                pumpHandle.style.top = Length.Percent(0f);
                
                if (currentConfig.pumpIsBalanceMode)
                {
                    pumpNormalBar.style.display = DisplayStyle.None;
                    pumpDialContainer.style.display = DisplayStyle.Flex;
                    
                    pumpHoldTimerContainer.style.display = DisplayStyle.Flex;
                    pumpHoldTimerBar.style.width = Length.Percent(0f);
                    
                    float minAngle = -120f + (currentConfig.pumpBalanceTargetMin * 240f);
                    float maxAngle = -120f + (currentConfig.pumpBalanceTargetMax * 240f);
                    float greenSweep = maxAngle - minAngle;
                    
                    float maskRot = 90f - greenSweep;
                    float zoneRot = minAngle - maskRot;
                    
                    pumpDialTargetZone.style.rotate = new StyleRotate(new Rotate(new Angle(zoneRot)));
                    pumpDialTargetArc.style.rotate = new StyleRotate(new Rotate(new Angle(maskRot)));
                    
                    pumpDialNeedleRotator.style.rotate = new StyleRotate(new Rotate(new Angle(-120f)));
                }
                else
                {
                    pumpNormalBar.style.display = DisplayStyle.Flex;
                    pumpDialContainer.style.display = DisplayStyle.None;
                    pumpHoldTimerContainer.style.display = DisplayStyle.None;
                }
                
                activeGameRoutine = StartCoroutine(PumpSystemRoutine());
                break;
                
            case GameType.ScrubPlate:
                scrubPlateContainer.style.display = DisplayStyle.Flex;
                SetupScrubPlate();
                activeGameRoutine = StartCoroutine(ScrubPlateRoutine());
                break;
                
            case GameType.CodeBreaker:
                codeBreakerContainer.style.display = DisplayStyle.Flex;
                SetupCodeBreaker();
                break;
                
            case GameType.BalanceMeter:
                balanceMeterContainer.style.display = DisplayStyle.Flex;
                balanceValue = 50f;
                balanceTimeInZone = 0f;
                activeGameRoutine = StartCoroutine(BalanceMeterRoutine());
                break;
                
            case GameType.DialogueChain:
                dialogueChainContainer.style.display = DisplayStyle.Flex;
                dialogueRound = 0;
                rapportValue = 0.5f;
                UpdateDialogueUI();
                activeGameRoutine = StartCoroutine(DialogueChainRoutine());
                break;
                
            case GameType.OrderQueue:
                orderQueueContainer.style.display = DisplayStyle.Flex;
                if (currentGame.name == "Train new hire on POS system")
                {
                    orderQueueContainer.style.backgroundColor = new StyleColor(new Color(0.05f, 0.12f, 0.05f));
                    orderQueueContainer.style.borderTopWidth = 15;
                    orderQueueContainer.style.borderBottomWidth = 15;
                    orderQueueContainer.style.borderLeftWidth = 15;
                    orderQueueContainer.style.borderRightWidth = 15;
                    orderQueueContainer.style.borderTopColor = new StyleColor(new Color(0.85f, 0.85f, 0.8f));
                    orderQueueContainer.style.borderBottomColor = new StyleColor(new Color(0.85f, 0.85f, 0.8f));
                    orderQueueContainer.style.borderLeftColor = new StyleColor(new Color(0.85f, 0.85f, 0.8f));
                    orderQueueContainer.style.borderRightColor = new StyleColor(new Color(0.85f, 0.85f, 0.8f));
                    orderQueueContainer.style.borderTopLeftRadius = 20;
                    orderQueueContainer.style.borderTopRightRadius = 20;
                    orderQueueContainer.style.borderBottomLeftRadius = 20;
                    orderQueueContainer.style.borderBottomRightRadius = 20;
                    orderQueueContainer.style.paddingTop = 20;
                    orderQueueContainer.style.paddingBottom = 20;
                    
                    orderStatusText.style.color = new StyleColor(new Color(0.2f, 1f, 0.2f));
                    orderStatusText.text = "POS READY\nMEMORIZE INPUT:";
                }
                else if (currentGame.name == "Verify hotfix deployment")
                {
                    Color ghBg = ColorUtility.TryParseHtmlString("#0d1117", out Color c1) ? c1 : new Color(0.05f, 0.06f, 0.09f);
                    Color ghBorder = ColorUtility.TryParseHtmlString("#30363d", out Color c2) ? c2 : new Color(0.18f, 0.21f, 0.23f);
                    
                    orderQueueContainer.style.backgroundColor = new StyleColor(ghBg);
                    orderQueueContainer.style.borderTopWidth = 2;
                    orderQueueContainer.style.borderBottomWidth = 2;
                    orderQueueContainer.style.borderLeftWidth = 2;
                    orderQueueContainer.style.borderRightWidth = 2;
                    orderQueueContainer.style.borderTopColor = new StyleColor(ghBorder);
                    orderQueueContainer.style.borderBottomColor = new StyleColor(ghBorder);
                    orderQueueContainer.style.borderLeftColor = new StyleColor(ghBorder);
                    orderQueueContainer.style.borderRightColor = new StyleColor(ghBorder);
                    orderQueueContainer.style.borderTopLeftRadius = 10;
                    orderQueueContainer.style.borderTopRightRadius = 10;
                    orderQueueContainer.style.borderBottomLeftRadius = 10;
                    orderQueueContainer.style.borderBottomRightRadius = 10;
                    orderQueueContainer.style.paddingTop = 20;
                    orderQueueContainer.style.paddingBottom = 20;
                    
                    orderStatusText.style.color = new StyleColor(ColorUtility.TryParseHtmlString("#c9d1d9", out Color c3) ? c3 : Color.white);
                    orderStatusText.text = "VERIFY WORKFLOW RUN";
                }
                else
                {
                    orderQueueContainer.style.backgroundColor = new StyleColor(Color.clear);
                    orderQueueContainer.style.borderTopWidth = 0;
                    orderQueueContainer.style.borderBottomWidth = 0;
                    orderQueueContainer.style.borderLeftWidth = 0;
                    orderQueueContainer.style.borderRightWidth = 0;
                    orderQueueContainer.style.paddingTop = 0;
                    orderQueueContainer.style.paddingBottom = 0;
                    
                    orderStatusText.style.color = new StyleColor(new Color(0.94f, 0.76f, 0.05f));
                    orderStatusText.text = "MEMORIZE THE ORDER!";
                }
                activeGameRoutine = StartCoroutine(OrderQueueRoutine());
                break;
                
            case GameType.DragMatch:
                dragMatchContainer.style.display = DisplayStyle.Flex;
                SetupDragMatch();
                break;
                
            case GameType.SprintLane:
                sprintLaneContainer.style.display = DisplayStyle.Flex;
                sprintDistance = 0;
                sprintProgressBar.style.width = new Length(0, LengthUnit.Percent);
                activeGameRoutine = StartCoroutine(SprintLaneRoutine());
                break;
                
            case GameType.Tetris:
                tetrisContainer.style.display = DisplayStyle.Flex;
                SetupTetris();
                activeGameRoutine = StartCoroutine(TetrisRoutine());
                break;
                
            case GameType.AcidRainShield:
                acidRainContainer.style.display = DisplayStyle.Flex;
                acidBurn = 0f;
                acidSignAngle = -90f;
                acidSignPivot.style.rotate = new StyleRotate(new Rotate(new Angle(0f, AngleUnit.Degree)));
                acidBurnBar.style.width = new Length(0, LengthUnit.Percent);
                acidDropContainer.Clear();
                activeGameRoutine = StartCoroutine(AcidRainRoutine());
                break;

            case GameType.VideoCall:
                videoCallContainer.style.display = DisplayStyle.Flex;
                SetupVideoCall();
                break;
                
            case GameType.KanbanBoard:
                kanbanContainer.style.display = DisplayStyle.Flex;
                SetupKanbanBoard();
                break;
                
            case GameType.FileSorter:
                if (fileSorterContainer != null) fileSorterContainer.style.display = DisplayStyle.Flex;
                SetupFileSorter();
                break;

            case GameType.CoffeeWork:
                if (coffeeWorkContainer != null) coffeeWorkContainer.style.display = DisplayStyle.Flex;
                SetupCoffeeWork();
                break;
                
            case GameType.UnitTestRunner:
                unitTestContainer.style.display = DisplayStyle.Flex;
                SetupUnitTest();
                break;
                
            case GameType.CenterDiv:
                centerDivContainer.style.display = DisplayStyle.Flex;
                SetupCenterDiv();
                break;
                
            case GameType.DragCenter:
                if (dragCenterContainer != null) dragCenterContainer.style.display = DisplayStyle.Flex;
                dcPoints = 0;
                SetupDragCenter();
                break;
                
            case GameType.SizeMatch:
                if (sizeMatchContainer != null) sizeMatchContainer.style.display = DisplayStyle.Flex;
                smPoints = 0;
                SetupSizeMatch();
                break;

            case GameType.StealthInject:
                if (stealthInjectContainer != null) stealthInjectContainer.style.display = DisplayStyle.Flex;
                SetupStealthInject();
                break;

            case GameType.AutomatedTests:
                if (automatedTestsContainer != null) automatedTestsContainer.style.display = DisplayStyle.Flex;
                SetupAutomatedTests();
                break;
                
            case GameType.CodeCommenter:
                if (codeCommenterContainer != null) codeCommenterContainer.style.display = DisplayStyle.Flex;
                SetupCodeCommenter();
                break;

            case GameType.DocumentSigner:
                if (documentSignerContainer != null) documentSignerContainer.style.display = DisplayStyle.Flex;
                SetupDocumentSigner();
                break;
                
            case GameType.ChartAdjuster:
                if (chartAdjusterContainer != null) chartAdjusterContainer.style.display = DisplayStyle.Flex;
                SetupChartAdjuster();
                break;
                




            case GameType.VrArcadePong:
                if (vrArcadeContainer != null) vrArcadeContainer.style.display = DisplayStyle.Flex;
                SetupVrArcadePong();
                break;
            case GameType.HotStoneSpa:
                if (hotStoneContainer != null) hotStoneContainer.style.display = DisplayStyle.Flex;
                SetupHotStoneSpa();
                break;
            case GameType.MowGreens:
                if (mgContainer != null) mgContainer.style.display = DisplayStyle.Flex;
                SetupMowGreens();
                break;
            case GameType.WalkDogs:
                if (wdContainer != null) wdContainer.style.display = DisplayStyle.Flex;
                SetupWalkDogs();
                break;
            case GameType.TwoPhaseHeist:
                if (tphContainer != null) tphContainer.style.display = DisplayStyle.Flex;
                SetupTwoPhaseHeist();
                break;
            case GameType.ForgePass:
                if (fpContainer != null) fpContainer.style.display = DisplayStyle.Flex;
                SetupForgePass();
                break;
            case GameType.SnatchBriefcase:
                if (snatchBriefcaseContainer != null) snatchBriefcaseContainer.style.display = DisplayStyle.Flex;
                SetupSnatchBriefcase();
                break;
            case GameType.SkimATM:
                if (saContainer != null) saContainer.style.display = DisplayStyle.Flex;
                SetupSkimATM();
                break;
            case GameType.IpHack:
                if (ipHackContainer != null) ipHackContainer.style.display = DisplayStyle.Flex;
                SetupIpHack();
                break;
            case GameType.HospitalityGala:
                if (hgContainer != null) hgContainer.style.display = DisplayStyle.Flex;
                SetupHospitalityGala();
                break;
            case GameType.StartupPitch:
                if (spContainer != null) spContainer.style.display = DisplayStyle.Flex;
                SetupStartupPitch();
                break;
            case GameType.CorporateGolf:
                if (cgContainer != null) cgContainer.style.display = DisplayStyle.Flex;
                SetupCorporateGolf();
                break;
            case GameType.NightShiftPitch:
                if (nightShiftContainer != null) nightShiftContainer.style.display = DisplayStyle.Flex;
                SetupNightShiftPitch();
                break;
            case GameType.YesManNod:
                if (yesManContainer != null) yesManContainer.style.display = DisplayStyle.Flex;
                SetupYesManNod();
                break;
            case GameType.FormatDashboard:
                if (formatDashboardContainer != null) formatDashboardContainer.style.display = DisplayStyle.Flex;
                SetupFormatDashboard();
                break;
            case GameType.FranticBackspace:
                if (franticBackspaceContainer != null) franticBackspaceContainer.style.display = DisplayStyle.Flex;
                SetupFranticBackspace();
                break;
            case GameType.SentienceArray:
                if (sentienceArrayContainer != null) sentienceArrayContainer.style.display = DisplayStyle.Flex;
                SetupSentienceArray();
                break;
            case GameType.ContainmentProtocol:
                if (containmentProtocolContainer != null) containmentProtocolContainer.style.display = DisplayStyle.Flex;
                SetupContainmentProtocol();
                break;
            case GameType.RewardLogic:
                if (rewardLogicContainer != null) rewardLogicContainer.style.display = DisplayStyle.Flex;
                SetupRewardLogic();
                break;
            case GameType.GalacticCouncil:
                if (galacticCouncilContainer != null) galacticCouncilContainer.style.display = DisplayStyle.Flex;
                SetupGalacticCouncil();
                break;
            case GameType.NeuralAligner:
                if (neuralAlignerContainer != null) neuralAlignerContainer.style.display = DisplayStyle.Flex;
                SetupNeuralAligner();
                break;

            case GameType.FoodPrepRush:
                foodPrepContainer.style.display = DisplayStyle.Flex;
                score = 0;
                timer = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 15f;
                prepTimerText.text = $"{timer:0.0}s";
                prepScoreText.text = $"Prepped: 0 / {currentConfig.targetScore}";
                for(int i=0; i<4; i++)
                {
                    prepProgress[i] = 0f;
                    prepFreezes[i] = 0f;
                    prepBars[i].style.height = new Length(0, LengthUnit.Percent);
                    prepStatus[i].text = "";
                }
                activeGameRoutine = StartCoroutine(FoodPrepRoutine());
                break;
        }
    }

    private void SetupFallingSort()
    {
        fallZone.Clear();
        binsRow.Clear();
        
        Color[] binColors = new Color[] {
            new Color(0.9f, 0.3f, 0.3f), // Red
            new Color(0.3f, 0.8f, 0.3f), // Green
            new Color(0.9f, 0.8f, 0.2f), // Yellow
        };
        
        for (int i = 0; i < currentConfig.binLabels.Length; i++)
        {
            var bin = new VisualElement();
            bin.style.flexGrow = 1;
            bin.style.backgroundColor = new StyleColor(binColors[i % binColors.Length]);
            bin.style.borderLeftWidth = 4;
            bin.style.borderRightWidth = 4;
            bin.style.borderTopWidth = 0;
            bin.style.borderBottomWidth = 4;
            bin.style.borderLeftColor = new StyleColor(Color.black);
            bin.style.borderRightColor = new StyleColor(Color.black);
            bin.style.borderBottomColor = new StyleColor(Color.black);
            
            // Bin Shape
            bin.style.borderBottomLeftRadius = 20;
            bin.style.borderBottomRightRadius = 20;
            bin.style.borderTopLeftRadius = 5;
            bin.style.borderTopRightRadius = 5;
            
            bin.style.marginRight = 10;
            bin.style.marginLeft = 10;
            bin.style.justifyContent = Justify.Center;
            bin.style.alignItems = Align.Center;
            
            // Extract just the emoji if present, else use full text
            string fullText = currentConfig.fallingItems[i];
            string emojiText = fullText;
            if (fullText.Contains(" ")) emojiText = fullText.Split(' ')[0];
            
            var label = new Label(emojiText); 
            label.style.fontSize = 50;
            label.style.color = Color.white;
            bin.Add(label);
            
            binsRow.Add(bin);
        }
    }

}
