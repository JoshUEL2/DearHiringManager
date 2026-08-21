using UnityEngine;

[CreateAssetMenu(fileName = "JobListing", menuName = "DearHiringManager/JobListing")]
public class JobListingSO : ScriptableObject
{
    public string title;
    [TextArea(3, 5)]
    public string description;
    public double weeklySalary;
    public District district;
    public bool isPartTime;

    [Header("Requirements")]
    public int dishwasherWeeksNeeded;
    public int juniorDevWeeksNeeded;
    public bool phdNeeded;
    public bool mbaNeeded;
    public bool bootcampNeeded;
    public bool isEntryLevelShiftSubstitute;
    public bool nepotismNeeded;
    public bool requiresFinancialDistrict;

    [Header("Tasks")]
    public System.Collections.Generic.List<JobTask> regularTasks = new System.Collections.Generic.List<JobTask>();
    public System.Collections.Generic.List<JobTask> extraTasks = new System.Collections.Generic.List<JobTask>();
}

[System.Serializable]
public class JobTask
{
    public string taskName;
    public float stressCost;
    public float readinessBoost;
}
