using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class LegendManager : MonoBehaviour
{
    public List<UnitSO> listOfLegends;
    public List<UnitSO> chosenLegends;
    public List<GameObject> legendChoiceHolder;

    private void OnEnable()
    {
        while(chosenLegends.Count < 3)
        {
            var randomLegend = listOfLegends[Random.Range(0, listOfLegends.Count)];

            if(!chosenLegends.Contains(randomLegend)) chosenLegends.Add(randomLegend);
        }

        for(int i = 0; i < legendChoiceHolder.Count; i++)
        {
            legendChoiceHolder[i].GetComponent<LegendChoiceManager>().SetLegend(chosenLegends[i]);
        }
    }
}
