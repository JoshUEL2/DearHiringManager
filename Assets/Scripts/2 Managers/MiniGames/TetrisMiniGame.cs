using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class TetrisMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement rootElement;
    
    private VisualElement tetrisContainer;
    private VisualElement tetrisGrid;
    private Label tetrisScoreText;
    private Button btnTetrisLeft;
    private Button btnTetrisRight;
    private Button btnTetrisRotate;
    private Button btnTetrisDrop;

    private int[,] tetrisBoard = new int[10, 15]; // 0=empty, 1=filled
    private int currentTetrisX, currentTetrisY, currentTetrisType, currentTetrisRot;
    private int tetrisScore = 0;
    private float tetrisTickTimer = 0f;
    private float tetrisTickRate = 0.2f;

    private readonly int[,,,] tetrominoes = {
        // I
        { { {0,0}, {1,0}, {2,0}, {3,0} }, { {1,-1}, {1,0}, {1,1}, {1,2} }, { {0,0}, {1,0}, {2,0}, {3,0} }, { {1,-1}, {1,0}, {1,1}, {1,2} } },
        // J
        { { {0,0}, {0,1}, {1,1}, {2,1} }, { {1,0}, {2,0}, {1,1}, {1,2} }, { {0,0}, {1,0}, {2,0}, {2,1} }, { {1,0}, {1,1}, {0,2}, {1,2} } },
        // L
        { { {2,0}, {0,1}, {1,1}, {2,1} }, { {1,0}, {1,1}, {1,2}, {2,2} }, { {0,0}, {1,0}, {2,0}, {0,1} }, { {0,0}, {1,0}, {1,1}, {1,2} } },
        // O
        { { {0,0}, {1,0}, {0,1}, {1,1} }, { {0,0}, {1,0}, {0,1}, {1,1} }, { {0,0}, {1,0}, {0,1}, {1,1} }, { {0,0}, {1,0}, {0,1}, {1,1} } },
        // S
        { { {1,0}, {2,0}, {0,1}, {1,1} }, { {1,0}, {1,1}, {2,1}, {2,2} }, { {1,0}, {2,0}, {0,1}, {1,1} }, { {1,0}, {1,1}, {2,1}, {2,2} } },
        // T
        { { {1,0}, {0,1}, {1,1}, {2,1} }, { {1,0}, {1,1}, {2,1}, {1,2} }, { {0,0}, {1,0}, {2,0}, {1,1} }, { {1,0}, {0,1}, {1,1}, {1,2} } },
        // Z
        { { {0,0}, {1,0}, {1,1}, {2,1} }, { {2,0}, {1,1}, {2,1}, {1,2} }, { {0,0}, {1,0}, {1,1}, {2,1} }, { {2,0}, {1,1}, {2,1}, {1,2} } }
    };

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;
        this.rootElement = root;

        tetrisContainer = root.Q<VisualElement>("TetrisContainer");
        tetrisGrid = root.Q<VisualElement>("TetrisGrid");
        tetrisScoreText = root.Q<Label>("TetrisScoreText");
        btnTetrisLeft = root.Q<Button>("BtnTetrisLeft");
        btnTetrisRight = root.Q<Button>("BtnTetrisRight");
        btnTetrisRotate = root.Q<Button>("BtnTetrisRotate");
        btnTetrisDrop = root.Q<Button>("BtnTetrisDrop");

        if (btnTetrisLeft != null) btnTetrisLeft.clicked += OnTetrisLeft;
        if (btnTetrisRight != null) btnTetrisRight.clicked += OnTetrisRight;
        if (btnTetrisRotate != null) btnTetrisRotate.clicked += OnTetrisRotate;
        if (btnTetrisDrop != null) btnTetrisDrop.clicked += OnTetrisDrop;

        rootElement.RegisterCallback<KeyDownEvent>(OnKeyDown);
    }

    private void OnTetrisLeft() => OnTetrisInput(-1, 0, 0);
    private void OnTetrisRight() => OnTetrisInput(1, 0, 0);
    private void OnTetrisRotate() => OnTetrisInput(0, 0, 1);
    private void OnTetrisDrop() => OnTetrisInput(0, 1, 0);

    public void StartGame()
    {
        tetrisContainer.style.display = DisplayStyle.Flex;

        tetrisGrid.Clear();
        for(int x=0; x<10; x++)
            for(int y=0; y<15; y++)
                tetrisBoard[x,y] = 0;
        
        tetrisScore = 0;
        if (tetrisScoreText != null)
            tetrisScoreText.text = $"Stacked: 0 / 10";
        SpawnTetromino();

        manager.StartCoroutine(GameRoutine());
    }

    public void AbortGame()
    {
        tetrisContainer.style.display = DisplayStyle.None;
        rootElement.UnregisterCallback<KeyDownEvent>(OnKeyDown);

        if (btnTetrisLeft != null) btnTetrisLeft.clicked -= OnTetrisLeft;
        if (btnTetrisRight != null) btnTetrisRight.clicked -= OnTetrisRight;
        if (btnTetrisRotate != null) btnTetrisRotate.clicked -= OnTetrisRotate;
        if (btnTetrisDrop != null) btnTetrisDrop.clicked -= OnTetrisDrop;
    }

    private void OnKeyDown(KeyDownEvent ev)
    {
        if (!manager.isPlaying) return;
        
        int dx = 0, dy = 0;
        if (ev.keyCode == KeyCode.UpArrow || ev.keyCode == KeyCode.W) dy = -1;
        else if (ev.keyCode == KeyCode.DownArrow || ev.keyCode == KeyCode.S) dy = 1;
        else if (ev.keyCode == KeyCode.LeftArrow || ev.keyCode == KeyCode.A) dx = -1;
        else if (ev.keyCode == KeyCode.RightArrow || ev.keyCode == KeyCode.D) dx = 1;
        
        if (dx == 0 && dy == 0) return;
        
        ev.StopPropagation();

        if (dx == -1) OnTetrisInput(-1, 0, 0);
        else if (dx == 1) OnTetrisInput(1, 0, 0);
        else if (dy == -1) OnTetrisInput(0, 0, 1);
        else if (dy == 1) OnTetrisInput(0, 1, 0);
    }

    private void SpawnTetromino()
    {
        currentTetrisType = Random.Range(0, 7);
        currentTetrisRot = 0;
        currentTetrisX = 3;
        currentTetrisY = 0;
        
        if (CheckTetrisCollision(currentTetrisX, currentTetrisY, currentTetrisRot))
        {
            manager.AbortGame(); // Topped out
        }
        else
        {
            DrawTetris();
        }
    }

    private bool CheckTetrisCollision(int x, int y, int rot)
    {
        for (int i=0; i<4; i++)
        {
            int bx = x + tetrominoes[currentTetrisType, rot, i, 0];
            int by = y + tetrominoes[currentTetrisType, rot, i, 1];
            
            if (bx < 0 || bx >= 10 || by >= 15) return true;
            if (by >= 0 && tetrisBoard[bx, by] != 0) return true;
        }
        return false;
    }

    private void LockTetromino()
    {
        for (int i=0; i<4; i++)
        {
            int bx = currentTetrisX + tetrominoes[currentTetrisType, currentTetrisRot, i, 0];
            int by = currentTetrisY + tetrominoes[currentTetrisType, currentTetrisRot, i, 1];
            if (bx >= 0 && bx < 10 && by >= 0 && by < 15)
                tetrisBoard[bx, by] = 1;
        }
        
        tetrisScore++;
        if (tetrisScoreText != null)
            tetrisScoreText.text = $"Stacked: {tetrisScore} / 10";
            
        if (tetrisScore >= 10)
        {
            manager.MarkWinReady();
            DrawTetris();
        }
        else
        {
            SpawnTetromino();
        }
    }

    private void DrawTetris()
    {
        tetrisGrid.Clear();
        
        for(int x=0; x<10; x++)
        {
            for(int y=0; y<15; y++)
            {
                if (tetrisBoard[x,y] != 0)
                    tetrisGrid.Add(CreateTetrisBlock(x, y));
            }
        }
        
        for (int i=0; i<4; i++)
        {
            int bx = currentTetrisX + tetrominoes[currentTetrisType, currentTetrisRot, i, 0];
            int by = currentTetrisY + tetrominoes[currentTetrisType, currentTetrisRot, i, 1];
            if (by >= 0 && by < 15)
                tetrisGrid.Add(CreateTetrisBlock(bx, by));
        }
    }

    private VisualElement CreateTetrisBlock(int x, int y)
    {
        var block = new VisualElement();
        block.style.width = 30;
        block.style.height = 30;
        block.style.position = Position.Absolute;
        block.style.left = x * 30;
        block.style.top = y * 30;
        block.style.backgroundColor = new StyleColor(new Color(0.15f, 0.15f, 0.15f));
        block.style.borderLeftWidth = 2; block.style.borderRightWidth = 2; block.style.borderTopWidth = 2; block.style.borderBottomWidth = 2;
        block.style.borderLeftColor = new StyleColor(new Color(0.8f, 0.1f, 0.1f));
        block.style.borderRightColor = new StyleColor(new Color(0.8f, 0.1f, 0.1f));
        block.style.borderTopColor = new StyleColor(new Color(0.8f, 0.1f, 0.1f));
        block.style.borderBottomColor = new StyleColor(new Color(0.8f, 0.1f, 0.1f));
        block.style.alignItems = Align.Center;
        block.style.justifyContent = Justify.Center;
        
        var xLabel = new Label("X");
        xLabel.style.color = new StyleColor(new Color(0.8f, 0.1f, 0.1f));
        xLabel.style.fontSize = 28;
        xLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        xLabel.style.position = Position.Absolute;
        xLabel.style.left = 0; xLabel.style.right = 0; xLabel.style.top = 0; xLabel.style.bottom = 0;
        xLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        block.Add(xLabel);
        
        return block;
    }

    private void OnTetrisInput(int dx, int dy, int drot)
    {
        if (!manager.isPlaying) return;
        
        int nextX = currentTetrisX + dx;
        int nextY = currentTetrisY + dy;
        int nextRot = (currentTetrisRot + drot) % 4;
        
        if (!CheckTetrisCollision(nextX, nextY, nextRot))
        {
            currentTetrisX = nextX;
            currentTetrisY = nextY;
            currentTetrisRot = nextRot;
            DrawTetris();
        }
        else if (dy == 1) // Collision while moving down = lock in place
        {
            LockTetromino();
        }
    }

    private IEnumerator GameRoutine()
    {
        tetrisTickTimer = 0f;
        while (manager.isPlaying)
        {
            tetrisTickTimer += Time.deltaTime;
            
            if (tetrisTickTimer >= tetrisTickRate)
            {
                tetrisTickTimer = 0f;
                OnTetrisInput(0, 1, 0); // Move down
            }
            yield return null;
        }
    }
}
