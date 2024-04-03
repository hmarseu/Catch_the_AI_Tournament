using YokaiNoMori.Interface;

public class CompetitorData
{
    public ICompetitor CurrentCompetitor;
    public int CurrentVictory;
    public int CurrentDraw;
    public int CurrentLose;
    public int CurrentErrorOccured;

    /// <summary>
    /// Get score. Victory : 3 points. Draw : 1 point
    /// </summary>
    /// <returns></returns>
    public int GetScore()
    {
        return (3 * CurrentVictory) + CurrentDraw;
    }
}
