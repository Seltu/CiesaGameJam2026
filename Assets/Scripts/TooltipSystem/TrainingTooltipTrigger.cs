using UnityEngine;
using UnityEngine.EventSystems;

public class TrainingTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        var trainingOptionSO = gameObject.GetComponent<TrainingSlot>().trainingOption;
        var statsChangeText = "";

        if (trainingOptionSO.hpModifier != 0)
            statsChangeText += $"HP {(trainingOptionSO.hpModifier >= 0 ? "+" : "")}{trainingOptionSO.hpModifier}";
        if (trainingOptionSO.attackModifier != 0)
            statsChangeText += $"\nATQ {(trainingOptionSO.attackModifier >= 0 ? "+" : "")}{trainingOptionSO.attackModifier}";
        if (trainingOptionSO.specialModifier != 0)
            statsChangeText += $"\nSPC {(trainingOptionSO.specialModifier >= 0 ? "+" : "")}{trainingOptionSO.specialModifier}";
        if (trainingOptionSO.defenseModifier != 0)
            statsChangeText += $"\nDEF {(trainingOptionSO.defenseModifier >= 0 ? "+" : "")}{trainingOptionSO.defenseModifier}";
        if (trainingOptionSO.speedModifier != 0)
            statsChangeText += $"\nVEL {(trainingOptionSO.speedModifier >= 0 ? "+" : "")}{trainingOptionSO.speedModifier}";

        statsChangeText += $"\nChance de sucesso: {trainingOptionSO.successRate * 100}%";

        TooltipSystem.Show(statsChangeText, trainingOptionSO.optionName);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Hide();
    }
}

