using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleSetupSO", menuName = "Legends/BattleSetupSO", order = 1)]
public class BattleSetupSO : ScriptableObject
{
    public UnitSO playerUnit;
    public List<UnitSO> randomUnitList;

    public UnitSO RandomUnit()
    {
        return Instantiate(randomUnitList[Random.Range(0, randomUnitList.Count)]);
    }
}
