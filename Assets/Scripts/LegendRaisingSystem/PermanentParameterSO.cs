using UnityEngine;

public class PermanentParameterSO : ScriptableObject
{
    public int daysPassed;
    public int entityPiety;
    public UnitSO currentLegend;
    public bool hasBattled;
    public bool hasWon;

    public void ResetParameters()
    {
        daysPassed = 0;
        entityPiety = 3;
        currentLegend = null;
        hasBattled = false;
        hasWon = false;
    }
}
