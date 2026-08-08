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

        if(GameManager.instance.permanentParameters.currentLegend != null)
        {
            var levels = GameManager.instance.permanentParameters.currentLegend.level;

            for(int i = 0; i < levels; i++)
            {
                var rng = Random.Range(0, levels);

                switch(rng)
                {
                    case 0:
                        newLegend.maxHp++;
                        break;
                    case 1:
                        newLegend.attack++;
                        break;
                    case 2:
                        newLegend.special++;
                        break;
                    case 3:
                        newLegend.defense++;
                        break;
                    case 4:
                        newLegend.speed++;
                        break;
                }
            }

            newLegend.level = levels;
        }

        GameManager.instance.permanentParameters.currentLegend = newLegend;
        var legendController = FindFirstObjectByType<LegendController>();
        legendController.SetLegend(newLegend);
    }
}
