using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class FinancialGraphUI : MonoBehaviour
{
    [Header("UI Document")]
    public UIDocument graphDocument;

    private VisualElement graphOverlay;
    private VisualElement graphArea;
    private Button btnCloseGraph;

    private void OnEnable()
    {
        var root = graphDocument.rootVisualElement;

        // CRITICAL: Let events pass through this panel when graph is hidden
        root.pickingMode = PickingMode.Ignore;

        graphOverlay = root.Q<VisualElement>("GraphOverlay");
        graphArea = root.Q<VisualElement>("GraphArea");
        
        btnCloseGraph = root.Q<Button>("BtnCloseGraph");
        btnCloseGraph.clicked += CloseGraph;

        var card = graphOverlay.Q<VisualElement>("GraphCard");
        if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
        graphOverlay.style.opacity = 0f;
        graphOverlay.schedule.Execute(() => {
            graphOverlay.style.display = DisplayStyle.None;
        }).StartingIn(300);
        graphOverlay.pickingMode = PickingMode.Ignore;
    }

    public void OpenGraph()
    {
        AudioManager.Instance?.PlayLedgerChaChing();
        HapticManager.LightImpact();
        DrawGraph();
        graphOverlay.style.display = DisplayStyle.Flex;
        graphOverlay.style.opacity = 0f;
        var card = graphOverlay.Q<VisualElement>("GraphCard");
        if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
        graphOverlay.schedule.Execute(() => {
            graphOverlay.style.opacity = 1f;
            if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 1, 1)));
        }).StartingIn(50);
        graphOverlay.pickingMode = PickingMode.Position;
    }

    private void CloseGraph()
    {
        AudioManager.Instance?.PlayClick();
        var card = graphOverlay.Q<VisualElement>("GraphCard");
        if (card != null) card.style.scale = new UnityEngine.UIElements.StyleScale(new UnityEngine.UIElements.Scale(new UnityEngine.Vector3(1, 0, 1)));
        graphOverlay.style.opacity = 0f;
        graphOverlay.schedule.Execute(() => {
            graphOverlay.style.display = DisplayStyle.None;
        }).StartingIn(300);
        graphOverlay.pickingMode = PickingMode.Ignore;
    }

    private void DrawGraph()
    {
        graphArea.Clear(); // Clear previous drawing

        List<FinancialSnapshot> history = GameManager.Instance.financialHistory;

        if (history == null || history.Count == 0)
        {
            Label noData = new Label("Insufficient ledger data. Run more weeks to assemble reports.");
            noData.style.color = Color.gray;
            noData.style.alignSelf = Align.Center;
            noData.style.marginTop = 80;
            graphArea.Add(noData);
            return;
        }

        // Find the maximum value to scale the Y axis
        double maxVal = 200.0;
        foreach (var snap in history)
        {
            if (snap.income > maxVal) maxVal = snap.income;
            if (snap.expenses > maxVal) maxVal = snap.expenses;
        }

        // Delay drawing slightly so layout engine has time to calculate width/height of GraphArea
        graphArea.RegisterCallback<GeometryChangedEvent>(evt => 
        {
            RenderLinesAndNodes(history, (float)maxVal, graphArea.resolvedStyle.width, graphArea.resolvedStyle.height);
        });
    }

    private void RenderLinesAndNodes(List<FinancialSnapshot> history, float maxVal, float width, float height)
    {
        graphArea.Clear();

        int count = history.Count;
        float padding = 15f; 
        float availableWidth = width - (padding * 2);
        float availableHeight = height - (padding * 2);
        
        float stepX = count > 1 ? availableWidth / (count - 1) : availableWidth / 2f;

        List<Vector2> incomePoints = new List<Vector2>();
        List<Vector2> expensePoints = new List<Vector2>();

        Color incomeColor; ColorUtility.TryParseHtmlString("#2ECC71", out incomeColor);
        Color expenseColor; ColorUtility.TryParseHtmlString("#E74C3C", out expenseColor);

        // 1. Calculate all coordinates
        for (int i = 0; i < count; i++)
        {
            float x = padding + (count > 1 ? i * stepX : 0);
            
            float incomeY = height - padding - ((float)history[i].income / maxVal) * availableHeight;
            float expenseY = height - padding - ((float)history[i].expenses / maxVal) * availableHeight;

            incomePoints.Add(new Vector2(x, incomeY));
            expensePoints.Add(new Vector2(x, expenseY));
        }

        // 2. Draw Connecting Lines
        if (count > 1)
        {
            for (int i = 0; i < count - 1; i++)
            {
                DrawLine(incomePoints[i], incomePoints[i + 1], incomeColor);
                DrawLine(expensePoints[i], expensePoints[i + 1], expenseColor);
            }
        }

        // 3. Draw Nodes (Rendered last so they sit on top of the lines)
        for (int i = 0; i < count; i++)
        {
            DrawNode(incomePoints[i], incomeColor);
            DrawNode(expensePoints[i], expenseColor);
        }
    }

    private void DrawLine(Vector2 p1, Vector2 p2, Color color)
    {
        VisualElement line = new VisualElement();
        line.style.position = Position.Absolute;
        line.style.backgroundColor = new StyleColor(color);
        
        float distance = Vector2.Distance(p1, p2);
        float angle = Mathf.Atan2(p2.y - p1.y, p2.x - p1.x) * Mathf.Rad2Deg;

        line.style.width = distance;
        line.style.height = 3f; // Line thickness

        // Set pivot to left-center for correct rotation positioning
        line.style.transformOrigin = new TransformOrigin(new Length(0f, LengthUnit.Percent), new Length(50f, LengthUnit.Percent), 0f);
        line.style.left = p1.x;
        line.style.top = p1.y;
        
        line.style.rotate = new Rotate(new Angle(angle, AngleUnit.Degree));

        graphArea.Add(line);
    }

    private void DrawNode(Vector2 pos, Color color)
    {
        VisualElement node = new VisualElement();
        node.style.position = Position.Absolute;
        node.style.backgroundColor = new StyleColor(color);
        
        float radius = 6f; // Node size
        node.style.width = radius * 2;
        node.style.height = radius * 2;
        node.style.borderTopLeftRadius = radius;
        node.style.borderTopRightRadius = radius;
        node.style.borderBottomLeftRadius = radius;
        node.style.borderBottomRightRadius = radius;

        // Center the dot on the coordinate
        node.style.left = pos.x - radius;
        node.style.top = pos.y - radius;

        graphArea.Add(node);
    }
}
