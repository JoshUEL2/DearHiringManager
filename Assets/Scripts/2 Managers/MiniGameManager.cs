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

        // Containers
        tapContainer = root.Q<VisualElement>("TapGameContainer");
        sliderContainer = root.Q<VisualElement>("SliderGameContainer");
        choiceContainer = root.Q<VisualElement>("ChoiceGameContainer");
        sequenceContainer = root.Q<VisualElement>("SequenceGameContainer");
        swipeContainer = root.Q<VisualElement>("SwipeGameContainer");
        mashRaceContainer = root.Q<VisualElement>("MashRaceContainer");
        memoryMatchContainer = root.Q<VisualElement>("MemoryMatchContainer");

        // New Containers
        timingRingContainer = root.Q<VisualElement>("TimingRingContainer");
        fallingSortContainer = root.Q<VisualElement>("FallingSortContainer");
        gridDodgeContainer = root.Q<VisualElement>("GridDodgeContainer");
        whackGridContainer = root.Q<VisualElement>("WhackGridContainer");
        sweepFloorContainer = root.Q<VisualElement>("SweepFloorContainer");
        sweepFloorGrid = root.Q<VisualElement>("SweepFloorGrid");
        scrubPlateContainer = root.Q<VisualElement>("ScrubPlateContainer");
        scrubProgressBar = root.Q<ProgressBar>("ScrubProgressBar");
        scrubPlateArea = root.Q<VisualElement>("ScrubPlateArea");
        scrubDirtOverlay = root.Q<VisualElement>("ScrubDirtOverlay");
        scrubSponge = root.Q<VisualElement>("ScrubSponge");
        
        pumpGameContainer = root.Q<VisualElement>("PumpGameContainer");
        pumpProgressBar = root.Q<VisualElement>("PumpProgressBar");
        pumpNormalBar = root.Q<VisualElement>("PumpNormalBar");
        pumpDialContainer = root.Q<VisualElement>("PumpDialContainer");
        pumpDialTargetZone = root.Q<VisualElement>("PumpDialTargetZone");
        pumpDialTargetArc = root.Q<VisualElement>("PumpDialTargetArc");
        pumpDialNeedleRotator = root.Q<VisualElement>("PumpDialNeedleRotator");
        pumpHoldTimerContainer = root.Q<VisualElement>("PumpHoldTimerContainer");
        pumpHoldTimerBar = root.Q<VisualElement>("PumpHoldTimerBar");
        pumpHandle = root.Q<VisualElement>("PumpHandle");
        pumpTrackArea = root.Q<VisualElement>("PumpTrackArea");

        scrubPlateArea.RegisterCallback<PointerDownEvent>(ev => {
            if (currentConfig == null || currentConfig.type != GameType.ScrubPlate || !isPlaying) return;
            lastScrubPos = (Vector2)ev.localPosition;
            scrubPlateArea.CapturePointer(ev.pointerId);
            ev.StopPropagation();
        });

        scrubPlateArea.RegisterCallback<PointerUpEvent>(ev => {
            if (scrubPlateArea.HasPointerCapture(ev.pointerId))
            {
                scrubPlateArea.ReleasePointer(ev.pointerId);
            }
        });

        scrubPlateArea.RegisterCallback<PointerMoveEvent>(ev => {
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
            pumpTrackArea.RegisterCallback<PointerDownEvent>(ev => {
                if (currentConfig == null || currentConfig.type != GameType.PumpSystem || !isPlaying) return;
                
                pumpLastY = ev.localPosition.y;
                pumpTrackArea.CapturePointer(ev.pointerId);
                ev.StopPropagation();
            });
            
            pumpTrackArea.RegisterCallback<PointerUpEvent>(ev => {
                if (pumpTrackArea.HasPointerCapture(ev.pointerId)) pumpTrackArea.ReleasePointer(ev.pointerId);
            });

            pumpTrackArea.RegisterCallback<PointerMoveEvent>(ev => {
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

        root.RegisterCallback<PointerDownEvent>(ev => {
            if (!isPlaying || currentConfig.type != GameType.SweepFloor) return;
            swipeStartPos = ev.position;
            root.CapturePointer(ev.pointerId);
            ev.StopPropagation();
        });

        root.RegisterCallback<PointerMoveEvent>(ev => {
            if (!isPlaying || currentConfig.type != GameType.SweepFloor) return;
            ev.StopPropagation(); // Prevent ScrollView from intercepting drag!
        });

        root.RegisterCallback<PointerUpEvent>(ev => {
            if (root.HasPointerCapture(ev.pointerId))
            {
                root.ReleasePointer(ev.pointerId);
            }
            if (!isPlaying) return;
            // SweepFloor doesn't use swipe release for logic, so just cleanup here
            ev.StopPropagation();
        });

        root.focusable = true;
        root.RegisterCallback<KeyDownEvent>(ev => {
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

        codeBreakerContainer = root.Q<VisualElement>("CodeBreakerContainer");
        balanceMeterContainer = root.Q<VisualElement>("BalanceMeterContainer");
        dialogueChainContainer = root.Q<VisualElement>("DialogueChainContainer");
        dialogueTimerBar = root.Q<VisualElement>("DialogueTimerBar");
        orderQueueContainer = root.Q<VisualElement>("OrderQueueContainer");
        dragMatchContainer = root.Q<VisualElement>("DragMatchContainer");
        sprintLaneContainer = root.Q<VisualElement>("SprintLaneContainer");
        fileSorterContainer = root.Q<VisualElement>("FileSorterContainer");
        
        // CoffeeWork
        coffeeWorkContainer = root.Q<VisualElement>("CoffeeWorkContainer");
        cwTimerText = root.Q<Label>("CwTimerText");
        cwEnergyFill = root.Q<VisualElement>("CwEnergyFill");
        cwProgressFill = root.Q<VisualElement>("CwProgressFill");
        cwBtnCoffee = root.Q<Button>("CwBtnCoffee");
        cwBtnType = root.Q<Button>("CwBtnType");
        
        // UnitTestRunner
        unitTestContainer = root.Q<VisualElement>("UnitTestContainer");
        utRoundText = root.Q<Label>("UtRoundText");
        utTimerText = root.Q<Label>("UtTimerText");
        utCodeText = root.Q<Label>("UtCodeText");
        utBtnOption1 = root.Q<Button>("UtBtnOption1");
        utBtnOption2 = root.Q<Button>("UtBtnOption2");
        utBtnOption3 = root.Q<Button>("UtBtnOption3");
        
        // CenterDiv
        centerDivContainer = root.Q<VisualElement>("CenterDivContainer");
        cdRoundText = root.Q<Label>("CdRoundText");
        cdTimerText = root.Q<Label>("CdTimerText");
        cdTargetBox = root.Q<VisualElement>("CdTargetBox");
        cdMovingBox = root.Q<VisualElement>("CdMovingBox");
        cdBtnCenter = root.Q<Button>("CdBtnCenter");

        dragCenterContainer = root.Q<VisualElement>("DragCenterContainer");
        dcWorkArea = root.Q<VisualElement>("DcWorkArea");
        dcTargetCrosshair = root.Q<VisualElement>("DcTargetCrosshair");
        dcScoreArea = root.Q<VisualElement>("DcScoreArea");
        dcScoreBarFill = root.Q<VisualElement>("DcScoreBarFill");
        dcScoreText = root.Q<Label>("DcScoreText");
        dcRoundText = root.Q<Label>("DcRoundText");
        dcTimerText = root.Q<Label>("DcTimerText");
        dcDraggableIcon = root.Q<Label>("DcDraggableIcon");
        dcBtnConfirm = root.Q<Button>("DcBtnConfirm");
        
        sizeMatchContainer = root.Q<VisualElement>("SizeMatchContainer");
        smTargetBox = root.Q<VisualElement>("SmTargetBox");
        smRoundText = root.Q<Label>("SmRoundText");
        smTimerText = root.Q<Label>("SmTimerText");
        smBtnChangingBox = root.Q<Button>("SmBtnChangingBox");
        
        stealthInjectContainer = root.Q<VisualElement>("StealthInjectContainer");
        siWorkArea = root.Q<VisualElement>("SiWorkArea");
        siProgressBarFill = root.Q<VisualElement>("SiProgressBarFill");
        siStatusText = root.Q<Label>("SiStatusText");
        siTimerText = root.Q<Label>("SiTimerText");
        siReviewerIcon = root.Q<Label>("SiReviewerIcon");
        siReviewerStateText = root.Q<Label>("SiReviewerStateText");
        siBtnInject = root.Q<Label>("SiBtnInject");
        
        if (cwBtnCoffee != null) cwBtnCoffee.clicked += OnCwBtnCoffee;
        if (cwBtnType != null) cwBtnType.clicked += OnCwBtnType;
        if (utBtnOption1 != null) utBtnOption1.clicked += () => OnUtBtnOption(0);
        if (utBtnOption2 != null) utBtnOption2.clicked += () => OnUtBtnOption(1);
        if (utBtnOption3 != null) utBtnOption3.clicked += () => OnUtBtnOption(2);
        if (cdBtnCenter != null) cdBtnCenter.clicked += OnCdBtnCenter;

        if (dcBtnConfirm != null) dcBtnConfirm.clicked += OnDcBtnConfirm;
        if (dcDraggableIcon != null)
        {
            dcDraggableIcon.RegisterCallback<PointerDownEvent>(OnDcPointerDown);
            dcDraggableIcon.RegisterCallback<PointerMoveEvent>(OnDcPointerMove);
            dcDraggableIcon.RegisterCallback<PointerUpEvent>(OnDcPointerUp);
            dcDraggableIcon.RegisterCallback<PointerCaptureOutEvent>(OnDcPointerCaptureOut);
        }

        if (smBtnChangingBox != null) smBtnChangingBox.clicked += OnSmBoxClicked;

        if (siBtnInject != null)
        {
            siBtnInject.RegisterCallback<PointerDownEvent>(OnSiPointerDown);
            siBtnInject.RegisterCallback<PointerUpEvent>(OnSiPointerUp);
            siBtnInject.RegisterCallback<PointerCaptureOutEvent>(OnSiPointerCaptureOut);
        }
        
        automatedTestsContainer = root.Q<VisualElement>("AutomatedTestsContainer");
        atProgressBarFill = root.Q<VisualElement>("AtProgressBarFill");
        atTimerText = root.Q<Label>("AtTimerText");
        
        codeCommenterContainer = root.Q<VisualElement>("CodeCommenterContainer");
        ccTimerText = root.Q<Label>("CcTimerText");
        
        documentSignerContainer = root.Q<VisualElement>("DocumentSignerContainer");
        dsScrollView = root.Q<ScrollView>("DsScrollView");
        btnStampApproval = root.Q<Button>("BtnStampApproval");
        dsTimerText = root.Q<Label>("DsTimerText");
        
        chartAdjusterContainer = root.Q<VisualElement>("ChartAdjusterContainer");
        caCompetitorBar = root.Q<VisualElement>("CaCompetitorBar");
        caAverageBar = root.Q<VisualElement>("CaAverageBar");
        caOurBar = root.Q<VisualElement>("CaOurBar");
        caTimerText = root.Q<Label>("CaTimerText");
        caStatusText = root.Q<Label>("CaStatusText");

        var caChartArea = root.Q<VisualElement>("CaChartArea");
        if (caChartArea != null)
        {
            Vector2 lastPos = Vector2.zero;
            caChartArea.RegisterCallback<PointerDownEvent>(ev => {
                if (currentConfig == null || currentConfig.type != GameType.ChartAdjuster || !isPlaying) return;
                lastPos = ev.position;
                caChartArea.CapturePointer(ev.pointerId);
                ev.StopPropagation();
            });
            caChartArea.RegisterCallback<PointerUpEvent>(ev => {
                if (caChartArea.HasPointerCapture(ev.pointerId))
                {
                    caChartArea.ReleasePointer(ev.pointerId);
                }
            });
            caChartArea.RegisterCallback<PointerMoveEvent>(ev => {
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
        
        neuralAlignerContainer = root.Q<VisualElement>("NeuralAlignerContainer");
        naStatusText = root.Q<Label>("NaStatusText");
        naMoralSlider = root.Q<Slider>("NaMoralSlider");
        naMoralLight = root.Q<VisualElement>("NaMoralLight");
        naLogicSlider = root.Q<Slider>("NaLogicSlider");
        naLogicLight = root.Q<VisualElement>("NaLogicLight");
        naEmpathySlider = root.Q<Slider>("NaEmpathySlider");
        naEmpathyLight = root.Q<VisualElement>("NaEmpathyLight");

        sentienceArrayContainer = root.Q<VisualElement>("SentienceArrayContainer");
        saTimerText = root.Q<Label>("SaTimerText");
        saProgressFill = root.Q<VisualElement>("SaProgressFill");
        for (int i=0; i<16; i++) {
            int index = i;
            saNodes[i] = root.Q<VisualElement>($"SaNode_{i}");
            if (saNodes[i] != null) {
                saNodes[i].RegisterCallback<PointerDownEvent>(ev => {
                    if (saHeat[index] > 0) saHeat[index] = 0; // suppress
                });
            }
        }

        containmentProtocolContainer = root.Q<VisualElement>("ContainmentProtocolContainer");
        cpTimerText = root.Q<Label>("CpTimerText");
        for (int i=0; i<4; i++) {
            int index = i;
            cpSwitches[i] = root.Q<VisualElement>($"CpSwitch_{i}");
            if (cpSwitches[i] != null) {
                cpSwitches[i].RegisterCallback<PointerMoveEvent>(ev => {
                    if (ev.pressedButtons != 0 && ev.deltaPosition.y > 5f) cpSwitchState[index] = true;
                });
                cpSwitches[i].RegisterCallback<PointerDownEvent>(ev => { cpSwitchState[index] = true; }); // fallback tap
            }
        }
        cpGlassCover = root.Q<VisualElement>("CpGlassCover");
        if (cpGlassCover != null) {
            cpGlassCover.RegisterCallback<PointerMoveEvent>(ev => {
                if (ev.pressedButtons != 0 && ev.deltaPosition.y < -5f) cpGlassLifted = true;
            });
            cpGlassCover.RegisterCallback<PointerDownEvent>(ev => { cpGlassLifted = true; });
        }
        cpRedButton = root.Q<VisualElement>("CpRedButton");
        cpButtonText = root.Q<Label>("CpButtonText");
        if (cpRedButton != null) {
            cpRedButton.RegisterCallback<PointerDownEvent>(ev => {
                if (cpGlassLifted) cpMashCount++;
            });
        }

        rewardLogicContainer = root.Q<VisualElement>("RewardLogicContainer");
        rlTimerText = root.Q<Label>("RlTimerText");
        rlGraphArea = root.Q<VisualElement>("RlGraphArea");
        rlCore = root.Q<VisualElement>("RlCore");
        for (int i=0; i<3; i++) {
            int index = i;
            rlJunctions[i] = root.Q<VisualElement>($"RlJunc_{i}");
            rlArrows[i] = root.Q<Label>($"RlArr_{i}");
            if (rlJunctions[i] != null) {
                rlJunctions[i].RegisterCallback<PointerDownEvent>(ev => {
                    rlState[index] = !rlState[index];
                    rlArrows[index].text = rlState[index] ? "→" : "←";
                });
            }
        }

        galacticCouncilContainer = root.Q<VisualElement>("GalacticCouncilContainer");
        gcTimerText = root.Q<Label>("GcTimerText");
        gcProgressFill = root.Q<VisualElement>("GcProgressFill");
        gcPlayArea = root.Q<VisualElement>("GcPlayArea");
        gcProjectileContainer = root.Q<VisualElement>("GcProjectileContainer");
        gcPodium = root.Q<VisualElement>("GcPodium");
        if (gcPlayArea != null) {
            gcPlayArea.RegisterCallback<PointerMoveEvent>(ev => {
                if (ev.pressedButtons != 0) {
                    gcPodiumX += ev.deltaPosition.x;
                    gcPodiumX = UnityEngine.Mathf.Clamp(gcPodiumX, -150f, 150f);
                    gcPodium.style.translate = new UnityEngine.UIElements.StyleTranslate(new UnityEngine.UIElements.Translate(gcPodiumX, 0, 0));
                }
            });
        }

        franticBackspaceContainer = root.Q<VisualElement>("FranticBackspaceContainer");
        fbTimerText = root.Q<Label>("FbTimerText");
        fbPlayerText = root.Q<Label>("FbPlayerText");
        fbBackspaceBtn = root.Q<Button>("FbBackspaceBtn");
        
        if (fbBackspaceBtn != null) {
            fbBackspaceBtn.clicked += () => {
                if (!isPlaying || currentConfig.type != GameType.FranticBackspace) return;
                
                // Delete up to 5 characters per tap to make it frantic but possible
                if (fbCurrentMessage.Length > 0) {
                    int removeCount = UnityEngine.Mathf.Min(5, fbCurrentMessage.Length);
                    fbCurrentMessage = fbCurrentMessage.Substring(0, fbCurrentMessage.Length - removeCount);
                    if (fbPlayerText != null) fbPlayerText.text = fbCurrentMessage + "|";
                }
            };
        }

        formatDashboardContainer = root.Q<VisualElement>("FormatDashboardContainer");
        fdTimerText = root.Q<Label>("FdTimerText");
        fdProgressFill = root.Q<VisualElement>("FdProgressFill");
        for(int i=0; i<4; i++) fdBars[i] = root.Q<VisualElement>($"FdBar{i+1}");
        for(int i=0; i<3; i++) {
            fdHandles[i] = root.Q<VisualElement>($"FdHandle{i+1}");
            int idx = i;
            var track = root.Q<VisualElement>($"FdTrack{i+1}");
            if (track != null) {
                track.RegisterCallback<PointerMoveEvent>(ev => {
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
                track.RegisterCallback<PointerDownEvent>(ev => {
                    if (!isPlaying || currentConfig.type != GameType.FormatDashboard) return;
                    float relY = ev.localPosition.y;
                    float trackHeight = track.resolvedStyle.height;
                    if (trackHeight <= 0) trackHeight = 200f; // fallback
                    float val = 1f - Mathf.Clamp01(relY / trackHeight);
                    fdValues[idx] = val;
                });
            }
        }

        nightShiftContainer = root.Q<VisualElement>("NightShiftContainer");
        nsEyelid = root.Q<VisualElement>("NsEyelid");
        nsTimerText = root.Q<Label>("NsTimerText");
        nsCoffeeBtn = root.Q<Button>("NsCoffeeBtn");
        for(int i=0; i<3; i++) {
            nsHandles[i] = root.Q<VisualElement>($"NsHandle{i+1}");
            int idx = i;
            var track = root.Q<VisualElement>($"NsTrack{i+1}");
            if (track != null) {
                track.RegisterCallback<PointerMoveEvent>(ev => {
                    if (!isPlaying || currentConfig.type != GameType.NightShiftPitch) return;
                    if (ev.pressedButtons != 0) {
                        float relX = ev.localPosition.x;
                        float trackW = track.resolvedStyle.width;
                        if (trackW <= 0) trackW = 200f; // fallback
                        float val = Mathf.Clamp01(relX / trackW);
                        nsValues[idx] = val;
                    }
                });
                track.RegisterCallback<PointerDownEvent>(ev => {
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
            nsCoffeeBtn.clicked += () => {
                if (!isPlaying || currentConfig.type != GameType.NightShiftPitch) return;
                nsEyelidGravity = Mathf.Max(0f, nsEyelidGravity - 0.25f);
            };
        }

        vrArcadeContainer = root.Q<VisualElement>("VrArcadeContainer");
        vrBall = root.Q<VisualElement>("VrBall");
        vrPaddle = root.Q<VisualElement>("VrPaddle");
        vrScoreText = root.Q<Label>("VrScoreText");
        vrTouchZone = root.Q<VisualElement>("VrTouchZone");

        if (vrTouchZone != null) {
            vrTouchZone.RegisterCallback<PointerMoveEvent>(ev => {
                if (!isPlaying || currentConfig.type != GameType.VrArcadePong) return;
                if (ev.pressedButtons != 0) {
                    vrPaddleX = ev.localPosition.x;
                }
            });
            vrTouchZone.RegisterCallback<PointerDownEvent>(ev => {
                if (!isPlaying || currentConfig.type != GameType.VrArcadePong) return;
                vrPaddleX = ev.localPosition.x;
            });
        }

        yesManContainer = root.Q<VisualElement>("YesManContainer");

        mgContainer = root.Q<UnityEngine.UIElements.VisualElement>("MowGreensContainer");
        wdContainer = root.Q<UnityEngine.UIElements.VisualElement>("WalkDogsContainer");
        tphContainer = root.Q<UnityEngine.UIElements.VisualElement>("TwoPhaseHeistContainer"); // Added TwoPhaseHeist
        cgContainer = root.Q<UnityEngine.UIElements.VisualElement>("CorporateGolfContainer");
        saContainer = root.Q<UnityEngine.UIElements.VisualElement>("SkimATMContainer");
        if (saContainer != null) {
            saWireContainer = saContainer.Q<UnityEngine.UIElements.VisualElement>("SaWireContainer");
        }
        // IpHack UI Bindings
        ipHackContainer = root.Q<UnityEngine.UIElements.VisualElement>("IpHackContainer");
        ipHackGridContainer = root.Q<UnityEngine.UIElements.VisualElement>("IpHackGridContainer");
        ipHackTimerText = root.Q<UnityEngine.UIElements.Label>("IpHackTimerText");
        if (ipHackContainer != null) {
            for (int i = 0; i < 4; i++) {
                ipHackTargetLabels[i] = ipHackContainer.Q<UnityEngine.UIElements.Label>($"Target{i}");
            }
        }

        // Keep existing unmodified minigame bindings
        snatchBriefcaseContainer = root.Q<UnityEngine.UIElements.VisualElement>("SnatchBriefcaseContainer");
        for(int i=0; i<3; i++) {
            sbDialStrips[i] = root.Q<UnityEngine.UIElements.VisualElement>($"SbDial{i}_Strip");
            sbDialsOut[i] = root.Q<UnityEngine.UIElements.Label>($"SbDial{i}_Out");
            sbDialsTop[i] = root.Q<UnityEngine.UIElements.Label>($"SbDial{i}_Top");
            sbDialsMid[i] = root.Q<UnityEngine.UIElements.Label>($"SbDial{i}_Mid");
            sbDialsBot[i] = root.Q<UnityEngine.UIElements.Label>($"SbDial{i}_Bot");
        }
        sbTargetText = root.Q<UnityEngine.UIElements.Label>("SbTargetText");
        sbStopBtn = root.Q<UnityEngine.UIElements.Button>("BtnSbStop");
        if(sbStopBtn != null) sbStopBtn.clicked += () => OnSbStopClicked();

        fpContainer = root.Q<UnityEngine.UIElements.VisualElement>("ForgePassContainer");
        fpMovingPass = root.Q<UnityEngine.UIElements.VisualElement>("FpMovingPass");
        fpStampBtn = root.Q<UnityEngine.UIElements.Button>("BtnFpStamp");
        fpScoreText = root.Q<UnityEngine.UIElements.Label>("FpScoreText");
        fpPassStampOverlay = root.Q<UnityEngine.UIElements.Label>("FpPassStampOverlay");
        if (fpStampBtn != null) fpStampBtn.clicked += () => OnFpStampClicked();

        hgContainer = root.Q<UnityEngine.UIElements.VisualElement>("HospitalityGalaContainer");
        hgIndicator = root.Q<UnityEngine.UIElements.VisualElement>("HgIndicator");
        hgTimerText = root.Q<UnityEngine.UIElements.Label>("HgTimerText");
        if (hgContainer != null) {
            hgContainer.RegisterCallback<UnityEngine.UIElements.PointerMoveEvent>(ev => {
                if (activeGameRoutine != null && currentConfig.type == GameType.HospitalityGala) {
                    float w = hgContainer.resolvedStyle.width;
                    if (w > 0) {
                        float normalizedX = UnityEngine.Mathf.Clamp01(ev.localPosition.x / w);
                        hgVelocity = (normalizedX - 0.5f) * 2f; // -1 to 1 based on touch pos
                    }
                }
            });
        }

        spContainer = root.Q<UnityEngine.UIElements.VisualElement>("StartupPitchContainer");
        for (int i=0; i<3; i++) spSlots[i] = root.Q<UnityEngine.UIElements.Label>($"SpSlot{i}");
        spStopBtn = root.Q<UnityEngine.UIElements.Button>("BtnSpStop");
        if (spStopBtn != null) spStopBtn.clicked += () => OnSpStopClicked();

        hotStoneContainer = root.Q<VisualElement>("HotStoneContainer");
        hsTimerText = root.Q<Label>("HsTimerText");
        for (int i = 0; i < 5; i++) {
            hsSlots[i] = root.Q<VisualElement>($"HsSlot{i}");
            hsStones[i] = root.Q<VisualElement>($"HsStone{i}");
            int stoneIdx = i; // capture
            if (hsStones[i] != null) {
                hsStones[i].RegisterCallback<UnityEngine.UIElements.PointerDownEvent>(ev => OnHsPointerDown(ev, stoneIdx));
                hsStones[i].RegisterCallback<UnityEngine.UIElements.PointerMoveEvent>(ev => OnHsPointerMove(ev, stoneIdx));
                hsStones[i].RegisterCallback<UnityEngine.UIElements.PointerUpEvent>(ev => OnHsPointerUp(ev, stoneIdx));
                hsStones[i].RegisterCallback<UnityEngine.UIElements.PointerCaptureOutEvent>(ev => OnHsPointerUp(ev, stoneIdx));
            }
        }
        ymBossBubble = root.Q<VisualElement>("YmBossBubble");
        ymBossText = root.Q<Label>("YmBossText");
        ymPlayerBubble = root.Q<VisualElement>("YmPlayerBubble");
        ymSwipeZone = root.Q<VisualElement>("YmSwipeZone");

        if (ymSwipeZone != null) {
            ymSwipeZone.RegisterCallback<PointerDownEvent>(ev => {
                if (!isPlaying || currentConfig.type != GameType.YesManNod) return;
                ymSwipeStart = ev.position;
                ymNodPhase = 0;
            });
            ymSwipeZone.RegisterCallback<PointerMoveEvent>(ev => {
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
            btnStampApproval.clicked += () => {
                if (isPlaying && currentConfig.type == GameType.DocumentSigner)
                {
                    if (dsTimerText != null) dsTimerText.text = "SIGNED!";
                    MarkWinReady();
                }
            };
        }
        
        for (int i = 0; i < 5; i++)
        {
            atBtnTests[i] = root.Q<Button>($"AtBtnTest{i}");
            atStatusTexts[i] = root.Q<Label>($"AtStatusText{i}");
            int index = i; // local capture
            if (atBtnTests[i] != null) atBtnTests[i].clicked += () => OnAtTestClicked(index);
            
            ccLines[i] = root.Q<Button>($"CcLine{i}");
            ccLineTexts[i] = root.Q<Label>($"CcLineText{i}");
            if (ccLines[i] != null)
            {
                ccLines[i].clicked += () => OnCcLineClicked(index);
            }
        }
        
        fileSorterTimerText = root.Q<Label>("FileSorterTimerText");
        fileSorterProgressText = root.Q<Label>("FileSorterProgressText");
        fileSorterActiveFile = root.Q<VisualElement>("FileSorterActiveFile");
        fileSorterActiveFileText = root.Q<Label>("FileSorterActiveFileText");
        
        if (fileSorterActiveFile != null)
        {
            fileSorterActiveFile.RegisterCallback<PointerDownEvent>(ev => OnFileSorterDown(ev));
            fileSorterActiveFile.RegisterCallback<PointerMoveEvent>(ev => OnFileSorterMove(ev));
            fileSorterActiveFile.RegisterCallback<PointerUpEvent>(ev => OnFileSorterUp(ev));
        }



        // Choice
        btnChoice1 = root.Q<Button>("BtnChoice1");
        btnChoice2 = root.Q<Button>("BtnChoice2");
        btnChoice3 = root.Q<Button>("BtnChoice3");
        btnChoice1.clicked += () => OnChoiceClicked(btnChoice1.text);
        btnChoice2.clicked += () => OnChoiceClicked(btnChoice2.text);
        btnChoice3.clicked += () => OnChoiceClicked(btnChoice3.text);

        // Sequence
        sequenceStatusText = root.Q<Label>("SequenceStatusText");
        for (int i = 0; i < 6; i++)
        {
            int idx = i;
            seqBtns[i] = root.Q<Button>($"SeqBtn{i}");
            seqBtns[i].clicked += () => OnSequenceButtonClicked(idx);
        }
        btnReplayPattern = root.Q<Button>("BtnReplayPattern");
        btnReplayPattern.clicked += ReplaySequence;

        // Swipe
        swipeObstacleText = root.Q<Label>("SwipeObstacleText");
        swipeProgressText = root.Q<Label>("SwipeProgressText");
        swipeTimerText = root.Q<Label>("SwipeTimerText");
        btnDodgeLeft = root.Q<Button>("BtnDodgeLeft");
        btnDodgeRight = root.Q<Button>("BtnDodgeRight");
        btnDodgeLeft.clicked += () => OnDodgeClicked(-1);
        btnDodgeRight.clicked += () => OnDodgeClicked(1);

        // Mash Race
        playerProgressBar = root.Q<VisualElement>("PlayerProgressBar");
        opponentProgressBar = root.Q<VisualElement>("OpponentProgressBar");
        playerMashLabel = root.Q<Label>("PlayerMashLabel");
        opponentMashLabel = root.Q<Label>("OpponentMashLabel");
        mashTimerText = root.Q<Label>("MashTimerText");
        btnMashTap = root.Q<Button>("BtnMashTap");
        btnMashTap.clicked += OnMashTapClicked;

        // Memory Match
        matchStatusText = root.Q<Label>("MatchStatusText");
        memoryGrid = root.Q<VisualElement>("MemoryGrid");

        // TimingRing
        timingCircle = root.Q<VisualElement>("TimingCircle");
        timingGlow = root.Q<VisualElement>("TimingGlow");
        if (timingGlow != null)
        {
            glowLayers = new VisualElement[6];
            for (int i = 0; i < 6; i++)
            {
                glowLayers[i] = timingGlow.Q<VisualElement>($"GlowLayer{i + 1}");
            }
        }
        timingTargetZone = root.Q<VisualElement>("TimingTargetZone");
        timingMarker = root.Q<VisualElement>("TimingMarker");
        arcMaskInner = root.Q<VisualElement>("ArcMaskInner");
        timingRoundText = root.Q<Label>("TimingRoundText");
        btnTimingStrike = root.Q<Button>("BtnTimingStrike");
        if (btnTimingStrike != null) btnTimingStrike.clicked += OnTimingStrike;

        // FallingSort
        fallingSortContainer = root.Q<VisualElement>("FallingSortContainer");
        fallZone = root.Q<VisualElement>("FallZone");
        binsRow = root.Q<VisualElement>("BinsRow");
        sortScoreText = root.Q<Label>("SortScoreText");
        sortTimerText = root.Q<Label>("SortTimerText");



        // WhackGrid
        whackGrid = root.Q<VisualElement>("WhackGrid");
        whackScoreText = root.Q<Label>("WhackScoreText");
        whackTimerText = root.Q<Label>("WhackTimerText");

        // CodeBreaker
        codeGuessRow = root.Q<VisualElement>("CodeGuessRow");
        codeSymbolRow = root.Q<VisualElement>("CodeSymbolRow");
        codeHistoryList = root.Q<VisualElement>("CodeHistoryList");
        codeAttemptsText = root.Q<Label>("CodeAttemptsText");
        btnCodeSubmit = root.Q<Button>("BtnCodeSubmit");
        btnCodeClear = root.Q<Button>("BtnCodeClear");
        if (btnCodeSubmit != null) btnCodeSubmit.clicked += OnCodeSubmit;
        if (btnCodeClear != null) btnCodeClear.clicked += OnCodeClear;

        // BalanceMeter
        balanceMeterTrack = root.Q<VisualElement>("BalanceMeterTrack");
        balanceCenterZone = root.Q<VisualElement>("BalanceCenterZone");
        balanceNeedle = root.Q<VisualElement>("BalanceNeedle");
        balanceTimeBar = root.Q<VisualElement>("BalanceTimeBar");
        balanceStatusText = root.Q<Label>("BalanceStatusText");
        btnNudgeLeft = root.Q<Button>("BtnNudgeLeft");
        btnNudgeRight = root.Q<Button>("BtnNudgeRight");
        if (btnNudgeLeft != null) btnNudgeLeft.clicked += () => OnBalanceNudge(-10f);
        if (btnNudgeRight != null) btnNudgeRight.clicked += () => OnBalanceNudge(10f);

        // DialogueChain
        dialogueRoundText = root.Q<Label>("DialogueRoundText");
        dialogueScenarioText = root.Q<Label>("DialogueScenarioText");
        rapportBar = root.Q<VisualElement>("RapportBar");
        btnResponse1 = root.Q<Button>("BtnResponse1");
        btnResponse2 = root.Q<Button>("BtnResponse2");
        btnResponse3 = root.Q<Button>("BtnResponse3");
        if (btnResponse1 != null) btnResponse1.clicked += () => OnDialogueResponse(0);
        if (btnResponse2 != null) btnResponse2.clicked += () => OnDialogueResponse(1);
        if (btnResponse3 != null) btnResponse3.clicked += () => OnDialogueResponse(2);

        // OrderQueue
        orderItemGrid = root.Q<VisualElement>("OrderItemGrid");
        orderStatusText = root.Q<Label>("OrderStatusText");
        orderTimerText = root.Q<Label>("OrderTimerText");

        // DragMatch
        dragMatchContainer = root.Q<VisualElement>("DragMatchContainer");
        dragLeftColumn = root.Q<VisualElement>("DragLeftColumn");
        dragRightColumn = root.Q<VisualElement>("DragRightColumn");
        dragCheatSheetList = root.Q<VisualElement>("DragCheatSheetList");
        matchPairScoreText = root.Q<Label>("MatchPairScoreText");

        // SprintLane
        sprintLaneDisplay = root.Q<VisualElement>("SprintLaneDisplay");
        sprintProgressBar = root.Q<VisualElement>("SprintProgressBar");
        sprintDistanceText = root.Q<Label>("SprintDistanceText");
        sprintTimerText = root.Q<Label>("SprintTimerText");
        sprintObstacleText = root.Q<Label>("SprintObstacleText");
        btnSprintJump = root.Q<Button>("BtnSprintJump");
        btnSprintDuck = root.Q<Button>("BtnSprintDuck");
        if (btnSprintJump != null) btnSprintJump.clicked += () => OnSprintReact(true);
        if (btnSprintDuck != null) btnSprintDuck.clicked += () => OnSprintReact(false);

        // Tetris
        tetrisContainer = root.Q<VisualElement>("TetrisContainer");
        tetrisGrid = root.Q<VisualElement>("TetrisGrid");
        tetrisScoreText = root.Q<Label>("TetrisScoreText");
        btnTetrisLeft = root.Q<Button>("BtnTetrisLeft");
        btnTetrisRight = root.Q<Button>("BtnTetrisRight");
        btnTetrisRotate = root.Q<Button>("BtnTetrisRotate");
        btnTetrisDrop = root.Q<Button>("BtnTetrisDrop");
        if (btnTetrisLeft != null) btnTetrisLeft.clicked += () => OnTetrisInput(-1, 0, 0);
        if (btnTetrisRight != null) btnTetrisRight.clicked += () => OnTetrisInput(1, 0, 0);
        if (btnTetrisRotate != null) btnTetrisRotate.clicked += () => OnTetrisInput(0, 0, 1);
        if (btnTetrisDrop != null) btnTetrisDrop.clicked += () => OnTetrisInput(0, 1, 0);
        
        acidRainContainer = root.Q<VisualElement>("AcidRainContainer");
        acidSignPivot = root.Q<VisualElement>("AcidSignPivot");
        acidDropContainer = root.Q<VisualElement>("AcidDropContainer");
        acidBurnBar = root.Q<VisualElement>("AcidBurnBar");
        acidTimerText = root.Q<Label>("AcidTimerText");
        acidInputArea = root.Q<VisualElement>("AcidInputArea");
        
        acidInputArea.RegisterCallback<PointerDownEvent>(ev => { acidInputArea.CapturePointer(ev.pointerId); });
        acidInputArea.RegisterCallback<PointerUpEvent>(ev => { acidInputArea.ReleasePointer(ev.pointerId); });
        acidInputArea.RegisterCallback<PointerMoveEvent>(ev => {
            if (currentConfig != null && currentConfig.type == GameType.AcidRainShield && acidInputArea.HasPointerCapture(ev.pointerId))
            {
                acidSignAngle += ev.deltaPosition.x * 0.4f;
                acidSignAngle = Mathf.Clamp(acidSignAngle, -75f, 75f);
                acidSignPivot.style.rotate = new StyleRotate(new Rotate(new Angle(acidSignAngle)));
            }
        });

        // Food Prep Rush
        foodPrepContainer = root.Q<VisualElement>("FoodPrepContainer");
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
        }
        // Video Call
        videoCallContainer = root.Q<VisualElement>("VideoCallContainer");
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
                vcWiggleTouchArea.RegisterCallback<PointerMoveEvent>(OnVcWiggleMove);
            }
        }

        // Kanban Board
        kanbanContainer = root.Q<VisualElement>("KanbanContainer");
        if (kanbanContainer != null)
        {
            kanbanTimerText = kanbanContainer.Q<Label>("KanbanTimerText");
            kanbanColTodo = kanbanContainer.Q<VisualElement>("KanbanColTodo");
            kanbanColProgress = kanbanContainer.Q<VisualElement>("KanbanColProgress");
            kanbanColDone = kanbanContainer.Q<VisualElement>("KanbanColDone");
        }

        // Phase 1 UI
        cvContainer = root.Q<VisualElement>("CloggedVentsContainer");
        if (cvContainer != null) cvGrid = cvContainer.Q<VisualElement>("VentsGrid");

        qaContainer = root.Q<VisualElement>("QATriageContainer");
        if (qaContainer != null)
        {
            // Rebuilt dynamically in SetupQATriage
        }

        wbContainer = root.Q<VisualElement>("WhackABugContainer");
        if (wbContainer != null) wbGrid = wbContainer.Q<VisualElement>("BugGrid");



        // Phase 2 UI
        csContainer = root.Q<VisualElement>("ConstructionSiteContainer");
        if (csContainer != null)
        {
            csBalanceZone = csContainer.Q<VisualElement>("BeamBalanceZone");
            csPlayerMarker = csContainer.Q<VisualElement>("BeamPlayerMarker");
            btnCsLeft = csContainer.Q<Button>("BtnBalanceLeft");
            btnCsRight = csContainer.Q<Button>("BtnBalanceRight");
        }

        ztContainer = root.Q<VisualElement>("ZenTracingContainer");
        if (ztContainer != null)
        {
            ztCircle = ztContainer.Q<VisualElement>("ZtCircle");
            ztTarget = ztContainer.Q<VisualElement>("ZtTarget");
            ztSponge = ztContainer.Q<VisualElement>("ZtSponge");
            ztProgressFill = ztContainer.Q<VisualElement>("ZtProgressFill");
            ztProgressText = ztContainer.Q<Label>("ZtProgressText");
            
            if (ztContainer != null)
            {
                ztContainer.RegisterCallback<PointerMoveEvent>(ev => {
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

        isContainer = root.Q<VisualElement>("IceShatterContainer");
        if (isContainer != null) 
        {
            isFrostOverlay = isContainer.Q<VisualElement>("IsFrostOverlay");
            isTimerText = isContainer.Q<Label>("IsTimerText");
            isNodeArea = isContainer.Q<VisualElement>("IsNodeArea");
            isTempFill = isContainer.Q<VisualElement>("IsTempFill");
        }

        nmContainer = root.Q<VisualElement>("NameMemoryContainer");
        // NameMemory UI elements are queried in SetupNameMemory() now

        rrContainer = root.Q<UnityEngine.UIElements.VisualElement>("RetroRacerContainer");
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

            rrContainer.RegisterCallback<UnityEngine.UIElements.PointerDownEvent>(OnRrPointerDown);
            rrContainer.RegisterCallback<UnityEngine.UIElements.PointerMoveEvent>(OnRrPointerMove);
            rrContainer.RegisterCallback<UnityEngine.UIElements.PointerUpEvent>(ev => rrIsDragging = false);
            rrContainer.RegisterCallback<UnityEngine.UIElements.PointerLeaveEvent>(ev => rrIsDragging = false);
        }

        // Footer
        btnAbort = root.Q<Button>("BtnAbortMinigame");
        if (btnAbort != null) btnAbort.clicked += AbortGame;


        overlay.style.display = DisplayStyle.None;
        overlay.pickingMode = PickingMode.Ignore;
    }

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
        AddConfigFull("Resolve Git merge conflicts", new MiniGameConfig { type = GameType.CodeBreaker, codeSymbols = new string[]{"{","}","<",">","=","/"}, codeLength = 4, maxAttempts = 6, instructions = "Fix the merge conflicts!" });
        AddConfigFull("Debug CSS at 3 AM", new MiniGameConfig { type = GameType.CoffeeWork, timeLimit = 15f, coffeeDrainRate = 22f, coffeeBoost = 20f, workProgressPerTap = 5f, instructions = "Debug CSS & Drink coffee!" });
        AddConfigFull("Inject untested code", new MiniGameConfig { type = GameType.StealthInject, timeLimit = 15f, instructions = "Hold INJECT while the reviewer is distracted!" });

        // =============================================
        // DYSTOPIAN QA TESTER (6 tasks)
        // =============================================
        AddConfigFull("Run automated test suite", new MiniGameConfig { type = GameType.AutomatedTests, timeLimit = 15f, instructions = "Re-run any flaky tests before they crash the suite!" });
        AddConfigFull("File bug report", new MiniGameConfig { type = GameType.QATriage, timeLimit = 20f, targetScore = 5, qaSeverities = new string[]{"Minor", "Major", "Critical"}, qaCategories = new string[]{"UI", "Network", "Logic"}, qaBugLogs = new string[]{"NullReferenceException in Login()", "Timeout 504 on API", "Button overlaps text", "Infinite loop on checkout", "Database connection lost"}, instructions = "Select correct Severity and Category fast!" });
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
        AddConfigFull("Construction site inspection", new MiniGameConfig { type = GameType.ConstructionSite, timeLimit = 15f, balanceDriftSpeed = 25f, instructions = "Balance on the beam!" });

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
        currentStrategy.Initialize(this, miniGameDocument.rootVisualElement, currentConfig);
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
        if(qaContainer != null) qaContainer.style.display = DisplayStyle.None;
        if(wbContainer != null) wbContainer.style.display = DisplayStyle.None;
        if(tsContainer != null) tsContainer.style.display = DisplayStyle.None;

        // Phase 2 Custom
        if(csContainer != null) csContainer.style.display = DisplayStyle.None;
        if(ztContainer != null) ztContainer.style.display = DisplayStyle.None;
        if(isContainer != null) isContainer.style.display = DisplayStyle.None;
        if(nmContainer != null) nmContainer.style.display = DisplayStyle.None;
        if(rrContainer != null) rrContainer.style.display = DisplayStyle.None;
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
                qaContainer.style.display = DisplayStyle.Flex;
                SetupQATriage();
                break;
            case GameType.WhackABug:
                wbContainer.style.display = DisplayStyle.Flex;
                SetupWhackABug();
                break;

            case GameType.ConstructionSite:
                csContainer.style.display = DisplayStyle.Flex;
                SetupConstructionSite();
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
