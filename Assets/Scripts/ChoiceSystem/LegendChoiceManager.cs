using UnityEngine;
using UnityEngine.UI;

public class LegendChoiceManager : MonoBehaviour
{
    public Image image;
    public UnitSO legend;

    public void SetLegend(UnitSO legendSO)
    {
        legend = legendSO;
        image.sprite = legend.eggSprite;
    }

    public void GetLegend()
    {
        var newLegend = Instantiate(legend);
        newLegend.age++;
        GameManager.instance.permanentParameters.currentLegend = newLegend;
        var legendController = FindFirstObjectByType<LegendController>();
        legendController.SetLegend(newLegend);
    }
}
