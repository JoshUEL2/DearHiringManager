using UnityEngine;

[CreateAssetMenu(fileName = "NewsEvent", menuName = "DearHiringManager/NewsEvent")]
public class NewsEventSO : ScriptableObject
{
    [TextArea(2, 4)]
    public string headline;
    public JobSector luckType;
    public float luckModifier;
    public int durationWeeks;
}
