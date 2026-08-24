path = 'Assets/Scripts/2 Managers/MiniGames/SequenceGameMiniGame.cs'

code = '''using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class SequenceGameMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private VisualElement altitudeBar;
    private VisualElement playerAvatar;
    private Label flipText;
    private VisualElement btnTuck;

    private bool isPlaying = false;
    private bool isTucking = false;
    private float altitude = 100f;
    private float rotationAngle = 0f;
    private float rotationSpeed = 0f;
    private int flipCount = 0;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        
        container = root.Q<VisualElement>("SequenceGameContainer");
        if (container == null) return;
        
        container.Clear(); // Nuke the old Simon Says UI
        container.style.flexDirection = FlexDirection.Row;
        container.style.alignItems = Align.Stretch;
        container.style.height = 400; // Fixed height for minigame area
        container.style.backgroundColor = new StyleColor(new Color(0.05f, 0.08f, 0.12f));
        container.style.borderRadius = 15;
        container.style.borderTopWidth = 4; container.style.borderBottomWidth = 4; container.style.borderLeftWidth = 4; container.style.borderRightWidth = 4;
        container.style.borderColor = new StyleColor(new Color(0.2f, 0.25f, 0.35f));
        container.style.overflow = Overflow.Hidden;
        container.style.marginBottom = 30;

        // Left: Altitude Meter
        var altitudeContainer = new VisualElement();
        altitudeContainer.style.width = 40;
        altitudeContainer.style.height = Length.Percent(100);
        altitudeContainer.style.backgroundColor = new StyleColor(new Color(0.1f, 0.15f, 0.2f));
        altitudeContainer.style.borderRightWidth = 4;
        altitudeContainer.style.borderRightColor = new StyleColor(new Color(0.2f, 0.25f, 0.35f));
        altitudeContainer.style.justifyContent = Justify.FlexEnd;
        
        altitudeBar = new VisualElement();
        altitudeBar.style.width = Length.Percent(100);
        altitudeBar.style.height = Length.Percent(100);
        altitudeBar.style.backgroundColor = new StyleColor(new Color(0.9f, 0.2f, 0.2f)); // Red for danger ground
        altitudeContainer.Add(altitudeBar);
        container.Add(altitudeContainer);

        // Right: Action Area
        var actionArea = new VisualElement();
        actionArea.style.flexGrow = 1;
        actionArea.style.position = Position.Relative;
        actionArea.style.alignItems = Align.Center;
        actionArea.style.justifyContent = Justify.Center;
        
        // Speed lines background (stylized blue)
        var skyBg = new VisualElement();
        skyBg.style.position = Position.Absolute;
        skyBg.style.width = Length.Percent(100);
        skyBg.style.height = Length.Percent(100);
        skyBg.style.backgroundColor = new StyleColor(new Color(0.12f, 0.53f, 0.9f));
        actionArea.Add(skyBg);

        playerAvatar = new VisualElement();
        playerAvatar.style.width = 80;
        playerAvatar.style.height = 140;
        playerAvatar.style.backgroundColor = new StyleColor(new Color(0.95f, 0.77f, 0.06f)); // Yellow sign spinner
        playerAvatar.style.borderRadius = 10;
        playerAvatar.style.borderTopWidth = 4; playerAvatar.style.borderBottomWidth = 4; playerAvatar.style.borderLeftWidth = 4; playerAvatar.style.borderRightWidth = 4;
        playerAvatar.style.borderColor = new StyleColor(Color.white);
        
        // Head / top indicator so player knows which way is UP
        var head = new VisualElement();
        head.style.position = Position.Absolute;
        head.style.top = -30;
        head.style.left = 20;
        head.style.width = 40;
        head.style.height = 40;
        head.style.backgroundColor = new StyleColor(Color.white);
        head.style.borderRadius = 20;
        playerAvatar.Add(head);
        
        actionArea.Add(playerAvatar);

        flipText = new Label("");
        flipText.style.position = Position.Absolute;
        flipText.style.top = Length.Percent(20);
        flipText.style.fontSize = 64;
        flipText.style.color = new StyleColor(Color.white);
        flipText.style.unityFontStyleAndWeight = FontStyle.Bold;
        flipText.style.opacity = 0;
        flipText.style.textShadow = new StyleTextShadow(new TextShadow { color = new Color(0,0,0,0.8f), offset = new Vector2(4f, 4f), blurRadius = 5f });
        actionArea.Add(flipText);

        btnTuck = new VisualElement();
        btnTuck.style.position = Position.Absolute;
        btnTuck.style.bottom = 20;
        btnTuck.style.width = Length.Percent(80);
        btnTuck.style.height = 80;
        btnTuck.style.backgroundColor = new StyleColor(new Color(0.9f, 0.2f, 0.2f));
        btnTuck.style.borderRadius = 40;
        btnTuck.style.borderTopWidth = 4; btnTuck.style.borderBottomWidth = 4; btnTuck.style.borderLeftWidth = 4; btnTuck.style.borderRightWidth = 4;
        btnTuck.style.borderColor = new StyleColor(new Color(1f, 0.4f, 0.4f));
        btnTuck.style.alignItems = Align.Center;
        btnTuck.style.justifyContent = Justify.Center;
        
        var btnLabel = new Label("HOLD TO TUCK");
        btnLabel.style.fontSize = 28;
        btnLabel.style.color = Color.white;
        btnLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        btnTuck.Add(btnLabel);
        
        actionArea.Add(btnTuck);
        container.Add(actionArea);

        // Event bindings for tuck button
        btnTuck.RegisterCallback<PointerDownEvent>(ev => {
            if (!isPlaying) return;
            isTucking = true;
            btnTuck.style.scale = new StyleScale(new Vector2(0.9f, 0.9f));
            btnTuck.style.backgroundColor = new StyleColor(new Color(0.7f, 0.1f, 0.1f));
            btnTuck.CapturePointer(ev.pointerId);
        });
        btnTuck.RegisterCallback<PointerUpEvent>(ev => {
            isTucking = false;
            btnTuck.style.scale = new StyleScale(new Vector2(1f, 1f));
            btnTuck.style.backgroundColor = new StyleColor(new Color(0.9f, 0.2f, 0.2f));
            if (btnTuck.HasPointerCapture(ev.pointerId)) btnTuck.ReleasePointer(ev.pointerId);
        });
    }

    public void StartGame()
    {
        if (container != null) container.style.display = DisplayStyle.Flex;
        isPlaying = true;
        isTucking = false;
        manager.StartCoroutine(AcrobatRoutine());
    }

    public void AbortGame()
    {
        isPlaying = false;
        if (container != null) container.style.display = DisplayStyle.None;
    }

    private IEnumerator AcrobatRoutine()
    {
        float fallSpeed = 22f; // ~4.5 seconds fall time
        altitude = 100f;
        rotationAngle = 0f;
        rotationSpeed = 0f;
        flipCount = 0;
        
        if (playerAvatar != null) playerAvatar.style.rotate = new StyleRotate(new Rotate(new Angle(0f)));
        if (flipText != null) flipText.style.opacity = 0;

        while (altitude > 0 && isPlaying && manager.isPlaying)
        {
            altitude -= fallSpeed * Time.deltaTime;
            if (altitude < 0) altitude = 0;
            
            if (altitudeBar != null) altitudeBar.style.height = Length.Percent(altitude);
            
            if (isTucking)
            {
                rotationSpeed = Mathf.Lerp(rotationSpeed, 850f, Time.deltaTime * 3.5f); // Fast spin!
                if (playerAvatar != null) 
                {
                    // Visual squish when tucking
                    playerAvatar.style.scale = new StyleScale(new Vector2(0.8f, 0.6f));
                }
            }
            else
            {
                rotationSpeed = Mathf.Lerp(rotationSpeed, 40f, Time.deltaTime * 4f); // Untucked slow spin
                if (playerAvatar != null) 
                {
                    playerAvatar.style.scale = new StyleScale(new Vector2(1f, 1f));
                }
            }
            
            rotationAngle += rotationSpeed * Time.deltaTime;
            if (playerAvatar != null) playerAvatar.style.rotate = new StyleRotate(new Rotate(new Angle(rotationAngle, AngleUnit.Degree)));
            
            // Flip detection
            int currentFlips = Mathf.FloorToInt(rotationAngle / 360f);
            if (currentFlips > flipCount)
            {
                flipCount = currentFlips;
                manager.StartCoroutine(FlashFlipText(flipCount));
            }
            
            yield return null;
        }
        
        if (!isPlaying || !manager.isPlaying) yield break;
        
        isPlaying = false;
        
        float finalModulo = rotationAngle % 360f;
        bool isUpright = (finalModulo <= 40f || finalModulo >= 320f);
        
        if (isTucking)
        {
            manager.StartCoroutine(StampEffectRoutine("CRUSHED!", new Color(0.9f, 0.2f, 0.2f), false));
        }
        else if (flipCount < 3)
        {
            manager.StartCoroutine(StampEffectRoutine("WEAK!", new Color(0.9f, 0.2f, 0.2f), false));
        }
        else if (!isUpright)
        {
            manager.StartCoroutine(StampEffectRoutine("FACEPLANT!", new Color(0.9f, 0.2f, 0.2f), false));
        }
        else
        {
            manager.StartCoroutine(StampEffectRoutine("NAILED IT!", new Color(0.18f, 0.8f, 0.44f), true));
        }
    }

    private IEnumerator FlashFlipText(int count)
    {
        if (flipText == null) yield break;
        
        flipText.text = count + " FLIP" + (count > 1 ? "S!" : "!");
        
        if (count >= 3) flipText.style.color = new StyleColor(new Color(1f, 0.84f, 0f)); // Gold for 3+
        else flipText.style.color = new StyleColor(Color.white);
        
        float life = 0.6f;
        float elapsed = 0f;
        while (elapsed < life && isPlaying)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / life;
            
            float scale = Mathf.Lerp(2.5f, 1f, p * 5f);
            if (scale < 1f) scale = 1f;
            flipText.style.scale = new StyleScale(new Vector2(scale, scale));
            
            if (p < 0.2f) flipText.style.opacity = Mathf.Lerp(0f, 1f, p * 5f);
            else if (p > 0.7f) flipText.style.opacity = Mathf.Lerp(1f, 0f, (p - 0.7f) * 3.33f);
            else flipText.style.opacity = 1f;
            
            yield return null;
        }
        flipText.style.opacity = 0;
    }

    private IEnumerator StampEffectRoutine(string text, Color color, bool isWin)
    {
        Label stampText = new Label(text);
        stampText.style.position = Position.Absolute;
        stampText.style.color = new StyleColor(color);
        stampText.style.fontSize = 110;
        stampText.style.letterSpacing = 10;
        stampText.style.rotate = new StyleRotate(new Rotate(new Angle(-12f, AngleUnit.Degree)));
        stampText.style.textShadow = new StyleTextShadow(new TextShadow { color = new Color(0,0,0,0.8f), offset = new Vector2(12f, 15f), blurRadius = 15f });
        stampText.style.unityFontStyleAndWeight = FontStyle.Bold;
        stampText.style.unityTextAlign = TextAnchor.MiddleCenter;
        
        VisualElement backgroundFade = new VisualElement();
        backgroundFade.style.position = Position.Absolute;
        backgroundFade.style.width = new Length(100, LengthUnit.Percent);
        backgroundFade.style.height = new Length(100, LengthUnit.Percent);
        backgroundFade.style.backgroundColor = new StyleColor(new Color(0f, 0f, 0f, 0.7f));
        backgroundFade.style.opacity = 0f;
        
        stampText.style.top = new Length(50, LengthUnit.Percent);
        stampText.style.left = new Length(50, LengthUnit.Percent);
        stampText.style.translate = new Translate(new Length(-50, LengthUnit.Percent), new Length(-50, LengthUnit.Percent));
        
        manager.miniGameDocument.rootVisualElement.Add(backgroundFade);
        manager.miniGameDocument.rootVisualElement.Add(stampText);
        
        float life = 1.0f;
        float elapsed = 0f;
        while(elapsed < life)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / life;
            
            float scale = Mathf.Lerp(3.5f, 1f, p * 12f);
            if (scale < 1f) scale = 1f;
            stampText.style.scale = new StyleScale(new Vector2(scale, scale));
            
            backgroundFade.style.opacity = Mathf.Lerp(0f, 1f, p * 8f);
            
            if (p > 0.8f) {
                float fadeOut = 1f - ((p - 0.8f) * 5f);
                stampText.style.opacity = fadeOut;
                backgroundFade.style.opacity = fadeOut;
            }
            
            yield return null;
        }
        
        if (manager.miniGameDocument.rootVisualElement.Contains(stampText)) manager.miniGameDocument.rootVisualElement.Remove(stampText);
        if (manager.miniGameDocument.rootVisualElement.Contains(backgroundFade)) manager.miniGameDocument.rootVisualElement.Remove(backgroundFade);
        
        if (isWin) manager.MarkWinReady();
        else {
            if (config.canFail) manager.AbortGame();
            else manager.MarkWinReady(); 
        }
    }
}
'''

with open(path, 'w') as f:
    f.write(code)
