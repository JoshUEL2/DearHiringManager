using UnityEngine;
using UnityEngine.UIElements;

public class TestPainter : VisualElement
{
    public TestPainter()
    {
        generateVisualContent += OnGenerateVisualContent;
    }

    private void OnGenerateVisualContent(MeshGenerationContext ctx)
    {
        var painter = ctx.painter2D;
        painter.strokeColor = Color.green;
        painter.lineWidth = 10f;
        painter.BeginPath();
        painter.Arc(new Vector2(100, 100), 50, 0, 90);
        painter.Stroke();
    }
}
