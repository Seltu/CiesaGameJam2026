using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrainingOptionChoiceManager : MonoBehaviour
{
    public Image image;
    public TrainingOptionSO trainingOption;

    public void SetTrainingOption(TrainingOptionSO trainingOptionSO)
    {
        trainingOption = trainingOptionSO;
        image.sprite = trainingOption.icon;
    }

    public void GetTrainingOption()
    {
        var newOption = Instantiate(trainingOption);
        GameManager.instance.trainingOptions[newOption.slot].GetComponentInChildren<TrainingSlot>().SetTrainingOption(newOption);
        GameManager.instance.CheckForLegend();
    }
}
