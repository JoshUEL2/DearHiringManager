public interface IWeeklySystem
{
    /// <summary>
    /// Processes weekly logic. Returns true if the week should continue processing, 
    /// or false if processing should halt early (e.g. Game Over, or Jail intercept).
    /// </summary>
    bool ProcessWeek(SavedGame data, GameManager manager);
}
