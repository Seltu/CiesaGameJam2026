using UnityEngine;

public class TrainingOptionSO : ScriptableObject
{
    public Sprite icon;
    public float successRate;

    public string optionName;
    public int slot;
    public int hpModifier;
    public int attackModifier;
    public int specialModifier;
    public int defenseModifier;
    public int speedModifier;
    
    public void Train(LegendController legendController, UnitSO unitSO)
    {
        var successRNG = Random.value;

        if (successRNG <= successRate)
        {
            // train
            unitSO.maxHp += hpModifier;
            unitSO.attack += attackModifier;
            unitSO.special += specialModifier;
            unitSO.defense += defenseModifier;
            unitSO.speed += speedModifier;

            unitSO.level++;

            legendController.DisplayTrainingResults(true);
        }
        else
        {
            legendController.DisplayTrainingResults(false);
        }
    }
}
