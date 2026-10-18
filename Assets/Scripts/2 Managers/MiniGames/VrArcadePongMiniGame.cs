using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class VrArcadePongMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;

    private VisualElement vrArcadeContainer;
    private VisualElement vrBall;
    private VisualElement vrPaddle;
    private VisualElement vrTouchZone;
    private Label vrScoreText;

    private float timer;
    private Vector2 vrBallPos;
    private Vector2 vrBallVel;
    private float vrPaddleX;
    private int vrBounces;

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;

        vrArcadeContainer = root.Q<VisualElement>("VrArcadeContainer");
        vrBall = root.Q<VisualElement>("VrBall");
        vrPaddle = root.Q<VisualElement>("VrPaddle");
        vrTouchZone = root.Q<VisualElement>("VrTouchZone");
        vrScoreText = root.Q<Label>("VrScoreText");
    }

    public void StartGame()
    {
        timer = config.timeLimit;
        vrBounces = 0;
        vrBallPos = new Vector2(200f, 50f);
        vrBallVel = new Vector2(Random.value > 0.5f ? 150f : -150f, 200f);
        vrPaddleX = 200f;
        
        if (vrScoreText != null) vrScoreText.text = $"BOUNCES: 0 / 5";
        if (vrBall != null) {
            vrBall.style.left = new StyleLength(new Length(vrBallPos.x, LengthUnit.Pixel));
            vrBall.style.top = new StyleLength(new Length(vrBallPos.y, LengthUnit.Pixel));
        }

        if (vrArcadeContainer != null) vrArcadeContainer.style.display = DisplayStyle.Flex;

        if (vrTouchZone != null) {
            vrTouchZone.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            vrTouchZone.RegisterCallback<PointerDownEvent>(OnPointerDown);
        }

        manager.StartCoroutine(VrArcadeRoutine());
    }

    public void AbortGame()
    {
        if (vrArcadeContainer != null) vrArcadeContainer.style.display = DisplayStyle.None;

        if (vrTouchZone != null) {
            vrTouchZone.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            vrTouchZone.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        }
    }

    private void OnPointerMove(PointerMoveEvent ev)
    {
        if (!manager.isPlaying) return;
        if (ev.pressedButtons != 0)
        {
            vrPaddleX = ev.localPosition.x;
        }
    }

    private void OnPointerDown(PointerDownEvent ev)
    {
        if (!manager.isPlaying) return;
        vrPaddleX = ev.localPosition.x;
    }

    private IEnumerator VrArcadeRoutine()
    {
        float containerW = 400f;
        float containerH = 400f;

        float paddleWidth = 100f;
        float paddleHeight = 20f;
        float paddleBottomOffset = 10f;
        float ballSize = 20f;

        while (manager.isPlaying)
        {
            timer -= Time.deltaTime;
            
            if (vrArcadeContainer != null) {
                float w = vrArcadeContainer.resolvedStyle.width;
                float h = vrArcadeContainer.resolvedStyle.height;
                if (!float.IsNaN(w) && w > 0) containerW = w;
                if (!float.IsNaN(h) && h > 0) containerH = h;
            }

            // Move Ball
            vrBallPos += vrBallVel * Time.deltaTime;

            // Constrain Paddle (centered on vrPaddleX)
            float paddleLeft = Mathf.Clamp(vrPaddleX - (paddleWidth / 2f), 0, containerW - paddleWidth);
            if (vrPaddle != null) vrPaddle.style.left = new StyleLength(new Length(paddleLeft, LengthUnit.Pixel));

            // Wall Collisions
            if (vrBallPos.x <= 0f) {
                vrBallPos.x = 0f;
                vrBallVel.x *= -1f;
            } else if (vrBallPos.x >= containerW - ballSize) {
                vrBallPos.x = containerW - ballSize;
                vrBallVel.x *= -1f;
            }

            if (vrBallPos.y <= 0f) {
                vrBallPos.y = 0f;
                vrBallVel.y *= -1f;
            }

            // Paddle Collision (paddle is at containerH - paddleBottomOffset - paddleHeight)
            float paddleTopY = containerH - paddleBottomOffset - paddleHeight;
            
            if (vrBallVel.y > 0 && vrBallPos.y + ballSize >= paddleTopY && vrBallPos.y < paddleTopY + paddleHeight) {
                // Check X bounds
                if (vrBallPos.x + ballSize >= paddleLeft && vrBallPos.x <= paddleLeft + paddleWidth) {
                    // Bounce!
                    vrBallPos.y = paddleTopY - ballSize;
                    vrBallVel.y *= -1f; // go up
                    
                    // Add some english based on where it hit the paddle
                    float hitFactor = (vrBallPos.x + (ballSize/2f)) - (paddleLeft + (paddleWidth/2f));
                    vrBallVel.x = hitFactor * 5f; // apply horizontal speed based on hit offset
                    
                    // Speed up slightly
                    vrBallVel *= 1.1f;
                    
                    vrBounces++;
                    AudioManager.Instance?.PlayActivityWellness("arcade");
                    if (vrScoreText != null) vrScoreText.text = $"BOUNCES: {vrBounces} / 5";

                    if (vrBounces >= 5) {
                        AudioManager.Instance?.PlayVictoryFanfare();
                        manager.MarkWinReady();
                        yield break;
                    }
                }
            }

            // Floor Collision (Miss)
            if (vrBallPos.y > containerH) {
                AudioManager.Instance?.PlayMinigameFail();
                if (vrScoreText != null) vrScoreText.text = "FAILED!";
                yield return new WaitForSeconds(1.0f);
                manager.AbortGame();
                yield break;
            }

            // Update visuals
            if (vrBall != null) {
                vrBall.style.left = new StyleLength(new Length(vrBallPos.x, LengthUnit.Pixel));
                vrBall.style.top = new StyleLength(new Length(vrBallPos.y, LengthUnit.Pixel));
            }

            if (timer <= 0f) {
                manager.AbortGame();
                yield break;
            }
            
            yield return null;
        }
    }
}
