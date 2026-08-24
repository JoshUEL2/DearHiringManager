using UnityEngine;
using UnityEngine.UIElements;
class Program { static void Main() { Label l = new Label(); l.style.textShadow = new StyleTextShadow(new TextShadow { color = Color.black, offset = new Vector2(5, 5), blurRadius = 5f }); } }
