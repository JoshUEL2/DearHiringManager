using System.Collections.Generic;
public class Test {
    public List<int> availableJobOpenings { get; set; }
    public void CycleJobs() {
        if (availableJobOpenings != null) availableJobOpenings.Clear();
    }
}
