using UnityEngine;

[CreateAssetMenu(fileName = "Course", menuName = "DearHiringManager/Course")]
public class CourseSO : ScriptableObject
{
    public string trackName;
    public string title;
    [TextArea(3, 5)]
    public string description;
    public double cost;
    public int weeks;
    public float stress;
    public string prerequisite; // null or empty if none

    [Header("Completion Rewards")]
    public JobSector luckCategory;
    public float luckBoostAmount;
    
    public bool unlocksBootcamp;
    public bool unlocksBA;
    public bool unlocksMA;
    public bool unlocksPhD;
}
