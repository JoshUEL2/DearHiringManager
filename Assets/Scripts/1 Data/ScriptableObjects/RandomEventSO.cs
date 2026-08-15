using UnityEngine;

[CreateAssetMenu(fileName = "RandomEvent", menuName = "DearHiringManager/RandomEvent")]
public class RandomEventSO : ScriptableObject
{
    public string title;
    [TextArea(3, 5)]
    public string description;
    public double cashOffset;
    public float stressOffset;
    public JobSector luckType; 
    public float luckOffset;
}
