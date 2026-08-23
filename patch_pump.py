import re

path = 'Assets/Scripts/2 Managers/MiniGames/PumpSystemMiniGame.cs'
with open(path, 'r') as f:
    content = f.read()

target = r'''    // Thermometer specific UI
    private VisualElement thermometerContainer;
    private VisualElement thermometerLiquid;
    private VisualElement thermometerTargetBracket;'''

replacement = r'''    // Speedometer specific UI
    private VisualElement speedometerTargetArc;
    
    private class GaugeElement : VisualElement
    {
        public float minAngle = 150f;
        public float maxAngle = 390f;
        public Color arcColor = Color.green;
        public float arcThickness = 16f;

        public GaugeElement()
        {
            generateVisualContent += OnGenerateVisualContent;
        }

        private void OnGenerateVisualContent(MeshGenerationContext ctx)
        {
            var painter = ctx.painter2D;
            if (painter == null) return;
            
            float radius = (Mathf.Min(layout.width, layout.height) / 2f) - (arcThickness / 2f);
            Vector2 center = new Vector2(layout.width / 2f, layout.height / 2f);
            
            painter.strokeColor = arcColor;
            painter.lineWidth = arcThickness;
            painter.lineCap = LineCap.Round;
            
            painter.BeginPath();
            painter.Arc(center, radius, minAngle, maxAngle, ArcDirection.Clockwise);
            painter.Stroke();
        }
    }
    private GaugeElement gaugeElement;'''
content = re.sub(target, replacement, content)

with open(path, 'w') as f:
    f.write(content)
