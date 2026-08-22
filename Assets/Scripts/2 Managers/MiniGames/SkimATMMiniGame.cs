using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class SkimATMMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;
    
    // UI Elements
    private VisualElement saContainer;
    private VisualElement saWireContainer;
    private VisualElement saActiveWire;
    private VisualElement saStartNode;
    private Label saTimerLabel;
    private Label saConnectionLabel;
    private VisualElement leftCol;
    private VisualElement rightCol;
    
    // State
    private bool saIsDraggingWire = false;
    private List<VisualElement> saLeftNodes = new List<VisualElement>();
    private List<VisualElement> saRightNodes = new List<VisualElement>();
    private int saConnections = 0;
    private float timer;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;
        
        saContainer = root.Q<VisualElement>("SkimATMContainer");
        if (saContainer != null) {
            saWireContainer = saContainer.Q<VisualElement>("SaWireContainer");
            saTimerLabel = saContainer.Q<Label>("SaTimerText");
            saConnectionLabel = saContainer.Q<Label>("SaConnectionText");
            leftCol = saContainer.Q<VisualElement>("SaLeftPanel");
            rightCol = saContainer.Q<VisualElement>("SaRightPanel");
        }
    }

    public void StartGame()
    {
        timer = config.timeLimit > 0 ? config.timeLimit : 10f;
        saConnections = 0;
        saIsDraggingWire = false;
        saActiveWire = null;
        saStartNode = null;
        
        if (saContainer != null)
        {
            saContainer.style.display = DisplayStyle.Flex;
            
            if (saTimerLabel != null) saTimerLabel.text = $"TIME: {timer:F1}s";
            if (saConnectionLabel != null) saConnectionLabel.text = "LINKS: 0/4";

            if (leftCol != null) leftCol.Clear();
            if (rightCol != null) rightCol.Clear();
            
            if (saWireContainer != null) {
                saWireContainer.Clear();
                saWireContainer.RegisterCallback<PointerMoveEvent>(OnSaPointerMove);
                saWireContainer.RegisterCallback<PointerUpEvent>(OnSaPointerUp);
                saWireContainer.RegisterCallback<PointerLeaveEvent>(OnSaPointerUp);
            }
            
            saLeftNodes.Clear();
            saRightNodes.Clear();
            
            Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
            Color[] rightColors = { Color.red, Color.blue, Color.green, Color.yellow };
            
            for(int i = 0; i < rightColors.Length; i++) {
                int r = Random.Range(i, rightColors.Length);
                Color temp = rightColors[i];
                rightColors[i] = rightColors[r];
                rightColors[r] = temp;
            }
            
            for (int i = 0; i < 4; i++)
            {
                VisualElement lBtn = new VisualElement();
                lBtn.style.width = 40; lBtn.style.height = 40; 
                lBtn.style.marginTop = 15; lBtn.style.marginBottom = 15;
                lBtn.style.backgroundColor = new StyleColor(colors[i]);
                lBtn.style.borderTopLeftRadius = 20; lBtn.style.borderTopRightRadius = 20; lBtn.style.borderBottomLeftRadius = 20; lBtn.style.borderBottomRightRadius = 20;
                
                lBtn.RegisterCallback<PointerDownEvent>(ev => OnSaNodePointerDown(ev, lBtn));
                saLeftNodes.Add(lBtn);
                if (leftCol != null) leftCol.Add(lBtn);
                
                VisualElement rBtn = new VisualElement();
                rBtn.style.width = 40; rBtn.style.height = 40; 
                rBtn.style.marginTop = 15; rBtn.style.marginBottom = 15;
                rBtn.style.backgroundColor = new StyleColor(rightColors[i]);
                rBtn.style.borderTopLeftRadius = 20; rBtn.style.borderTopRightRadius = 20; rBtn.style.borderBottomLeftRadius = 20; rBtn.style.borderBottomRightRadius = 20;
                
                saRightNodes.Add(rBtn);
                if (rightCol != null) rightCol.Add(rBtn);
            }
        }

        manager.StartCoroutine(SkimATMRoutine());
    }

    public void AbortGame()
    {
        if (saContainer != null) saContainer.style.display = DisplayStyle.None;
        
        if (saWireContainer != null) {
            saWireContainer.UnregisterCallback<PointerMoveEvent>(OnSaPointerMove);
            saWireContainer.UnregisterCallback<PointerUpEvent>(OnSaPointerUp);
            saWireContainer.UnregisterCallback<PointerLeaveEvent>(OnSaPointerUp);
        }
    }

    private void OnSaNodePointerDown(PointerDownEvent ev, VisualElement node) {
        if (!manager.isPlaying || !node.enabledSelf || saWireContainer == null) return;
        saIsDraggingWire = true;
        saStartNode = node;
        
        saActiveWire = new VisualElement();
        saActiveWire.style.position = Position.Absolute;
        saActiveWire.style.height = 8; // Wire thickness
        saActiveWire.style.backgroundColor = node.style.backgroundColor;
        saActiveWire.style.borderTopLeftRadius = 4;
        saActiveWire.style.borderBottomLeftRadius = 4;
        saActiveWire.style.transformOrigin = new TransformOrigin(new Length(0, LengthUnit.Percent), new Length(50, LengthUnit.Percent));
        saActiveWire.pickingMode = PickingMode.Ignore; // Crucial so it doesn't block PointerUp
        
        // Calculate center of start node relative to saWireContainer
        Vector2 nodeCenter = node.worldBound.center;
        Vector2 localCenter = saWireContainer.WorldToLocal(nodeCenter);
        
        saActiveWire.style.left = localCenter.x;
        saActiveWire.style.top = localCenter.y - 4; // offset by half height
        
        saWireContainer.Add(saActiveWire);
        
        // Ensure the container captures the pointer so we can drag outside the initial element
        saWireContainer.CapturePointer(ev.pointerId);
    }
    
    private void OnSaPointerMove(PointerMoveEvent ev) {
        if (!saIsDraggingWire || saActiveWire == null || saStartNode == null || saWireContainer == null) return;
        
        Vector2 startPos = saWireContainer.WorldToLocal(saStartNode.worldBound.center);
        Vector2 mousePos = saWireContainer.WorldToLocal(ev.position);
        
        Vector2 dir = mousePos - startPos;
        float distance = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        
        saActiveWire.style.width = distance;
        saActiveWire.style.rotate = new Rotate(new Angle(angle, AngleUnit.Degree));
    }
    
    private void OnSaPointerUp(EventBase ev) {
        if (!saIsDraggingWire || saWireContainer == null) return;
        saIsDraggingWire = false;
        
        IPointerEvent pointerEv = ev as IPointerEvent;
        if (pointerEv != null && saWireContainer.HasPointerCapture(pointerEv.pointerId)) {
            saWireContainer.ReleasePointer(pointerEv.pointerId);
        }
        
        if (saActiveWire == null || saStartNode == null) return;
        
        if (pointerEv == null) return;
        
        Vector2 mousePos = pointerEv.position;
        VisualElement targetNode = null;
        
        foreach (var rBtn in saRightNodes) {
            if (rBtn.enabledSelf && rBtn.worldBound.Contains(mousePos)) {
                targetNode = rBtn;
                break;
            }
        }
        
        if (targetNode != null && targetNode.style.backgroundColor.value == saStartNode.style.backgroundColor.value) {
            // Success snap
            Vector2 startPos = saWireContainer.WorldToLocal(saStartNode.worldBound.center);
            Vector2 targetPos = saWireContainer.WorldToLocal(targetNode.worldBound.center);
            Vector2 dir = targetPos - startPos;
            
            saActiveWire.style.width = dir.magnitude;
            saActiveWire.style.rotate = new Rotate(new Angle(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg, AngleUnit.Degree));
            
            saStartNode.enabledSelf = false;
            targetNode.enabledSelf = false;
            
            saConnections++;
            if (saConnectionLabel != null) saConnectionLabel.text = $"LINKS: {saConnections}/4";
            
            if (saConnections >= 4) manager.EndGame(true);
        } else {
            // Snap back / Destroy
            if (saWireContainer.Contains(saActiveWire)) {
                saWireContainer.Remove(saActiveWire);
            }
            if (targetNode != null && targetNode.style.backgroundColor.value != saStartNode.style.backgroundColor.value) {
                // Penalty for connecting wrong colors
                timer -= 2f;
            }
        }
        
        saActiveWire = null;
        saStartNode = null;
    }

    private IEnumerator SkimATMRoutine()
    {
        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            if (saTimerLabel != null) saTimerLabel.text = $"TIME: {Mathf.Max(0, timer):F1}s";
            
            if (timer <= 0f)
            {
                manager.EndGame(false);
                yield break;
            }
            yield return null;
        }
    }
}
