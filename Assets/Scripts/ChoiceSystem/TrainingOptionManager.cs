using System.Collections.Generic;
using UnityEngine;

public class TrainingOptionManager : MonoBehaviour
{
    public List<TrainingOptionSO> listOfTrainingOptions;
    public List<TrainingOptionSO> chosenTrainingOptions;
    public List<GameObject> trainingOptionsChoiceHolder;

    private void OnEnable()
    {
        while (chosenTrainingOptions.Count < 3)
        {
            var randomTrainingOption = listOfTrainingOptions[Random.Range(0, listOfTrainingOptions.Count)];

            if (!chosenTrainingOptions.Contains(randomTrainingOption)) chosenTrainingOptions.Add(randomTrainingOption);
        }

        for (int i = 0; i < trainingOptionsChoiceHolder.Count; i++)
        {
            trainingOptionsChoiceHolder[i].GetComponent<TrainingOptionChoiceManager>().SetTrainingOption(chosenTrainingOptions[i]);
        }
    }
}
