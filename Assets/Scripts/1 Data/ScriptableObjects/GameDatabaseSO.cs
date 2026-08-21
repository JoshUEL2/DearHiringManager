using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameDatabase", menuName = "DearHiringManager/GameDatabase")]
public class GameDatabaseSO : ScriptableObject
{
    [Header("Backstories")]
    public List<PlayerBackstorySO> backstories;

    [Header("Jobs")]
    public List<JobListingSO> jobs;

    [Header("Events")]
    public List<RandomEventSO> goodEvents;
    public List<RandomEventSO> badEvents;
    public List<NewsEventSO> newsEvents;

    [Header("Education")]
    public List<CourseSO> courses;
}
