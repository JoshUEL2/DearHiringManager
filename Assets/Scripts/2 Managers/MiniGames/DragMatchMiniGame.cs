using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class DragMatchMiniGame : IMiniGame
{
    private MiniGameManager manager;
    private MiniGameConfig config;
    private VisualElement container;

    private VisualElement dragLeftColumn;
    private VisualElement dragRightColumn;
    private VisualElement dragCheatSheetList;
    private Label matchPairScoreText;

    private int dragSelectedLeft = -1;
    private List<Button> dragLeftButtons = new List<Button>();
    private List<Button> dragRightButtons = new List<Button>();
    private int dragMatchCount = 0;
    private Dictionary<int,int> dragCorrectMap = new Dictionary<int,int>();

    public void Initialize(MiniGameManager manager, VisualElement root, MiniGameConfig config)
    {
        this.manager = manager;
        this.config = config;

        container = root.Q<VisualElement>("DragMatchContainer");
        dragLeftColumn = root.Q<VisualElement>("DragLeftColumn");
        dragRightColumn = root.Q<VisualElement>("DragRightColumn");
        dragCheatSheetList = root.Q<VisualElement>("DragCheatSheetList");
        matchPairScoreText = root.Q<Label>("MatchPairScoreText");
    }

    public void StartGame()
    {
        container.style.display = DisplayStyle.Flex;
        SetupDragMatch();
    }

    public void AbortGame()
    {
        container.style.display = DisplayStyle.None;
    }

    private void SetupDragMatch()
    {
        dragLeftColumn.Clear();
        dragRightColumn.Clear();
        dragCheatSheetList.Clear();
        dragLeftButtons.Clear();
        dragRightButtons.Clear();
        dragCorrectMap.Clear();
        dragMatchCount = 0;
        dragSelectedLeft = -1;
        
        var lefts = config.matchLeft.ToList();
        var rights = config.matchRight.ToList();
        var rightIndices = Enumerable.Range(0, rights.Count).OrderBy(x => Random.value).ToList();
        
        for (int i=0; i<lefts.Count; i++)
        {
            var lbl = new Label($"{lefts[i]} = {rights[rightIndices[i]]}");
            lbl.style.fontSize = 22;
            lbl.style.color = new StyleColor(new Color(0.1f, 0.1f, 0.1f)); // Dark grey/black ink
            lbl.style.whiteSpace = WhiteSpace.NoWrap;
            lbl.style.marginBottom = 10;
            lbl.style.paddingBottom = 8;
            lbl.style.paddingTop = 8;
            lbl.style.borderBottomWidth = 1;
            lbl.style.borderBottomColor = new StyleColor(new Color(0.3f, 0.6f, 0.9f, 0.5f)); // Light blue ruler line
            lbl.style.unityTextAlign = TextAnchor.MiddleLeft;
            dragCheatSheetList.Add(lbl);
            
            int idx = i;
            var lBtn = new Button { text = lefts[i] };
            lBtn.AddToClassList("popup-button");
            lBtn.style.paddingTop = 15; lBtn.style.paddingBottom = 15; lBtn.style.width = new Length(100, LengthUnit.Percent); lBtn.style.fontSize = 20;
            lBtn.clicked += () => { 
                dragSelectedLeft = idx; 
                foreach(var b in dragLeftButtons) b.style.backgroundColor = new StyleColor(Color.gray); 
                lBtn.style.backgroundColor = new StyleColor(Color.yellow); 
            };
            dragLeftButtons.Add(lBtn);
            dragLeftColumn.Add(lBtn);
            
            int rIdx = rightIndices[i];
            dragCorrectMap[i] = rIdx;
        }
        
        // Empty bottom line for notepad aesthetic
        var emptyLbl = new Label(" ");
        emptyLbl.style.paddingBottom = 8;
        emptyLbl.style.paddingTop = 8;
        emptyLbl.style.borderBottomWidth = 1;
        emptyLbl.style.borderBottomColor = new StyleColor(new Color(0.3f, 0.6f, 0.9f, 0.5f));
        dragCheatSheetList.Add(emptyLbl);
        
        for (int i=0; i<rights.Count; i++)
        {
            int idx = i;
            var rBtn = new Button { text = rights[idx] };
            rBtn.AddToClassList("popup-button");
            rBtn.style.paddingTop = 15; rBtn.style.paddingBottom = 15; rBtn.style.width = new Length(100, LengthUnit.Percent); rBtn.style.fontSize = 20;
            rBtn.clicked += () => OnDragRightClicked(idx, rBtn);
            dragRightButtons.Add(rBtn);
            dragRightColumn.Add(rBtn);
        }
    }

    private void OnDragRightClicked(int rIdx, Button rBtn)
    {
        if (!manager.isPlaying || dragSelectedLeft == -1) return;
        
        if (dragCorrectMap[dragSelectedLeft] == rIdx)
        {
            dragLeftButtons[dragSelectedLeft].style.backgroundColor = new StyleColor(Color.green);
            dragLeftButtons[dragSelectedLeft].SetEnabled(false);
            rBtn.style.backgroundColor = new StyleColor(Color.green);
            rBtn.SetEnabled(false);
            dragMatchCount++;
            if (dragMatchCount >= config.matchLeft.Length) manager.MarkWinReady();
        }
        else
        {
            dragLeftButtons[dragSelectedLeft].style.backgroundColor = new StyleColor(Color.gray);
        }
        dragSelectedLeft = -1;
    }
}
