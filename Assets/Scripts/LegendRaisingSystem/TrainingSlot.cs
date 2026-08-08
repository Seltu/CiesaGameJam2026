using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrainingSlot : MonoBehaviour, IDropHandler
{
    public TrainingOptionSO trainingOption;
    public Image image;
    public TMP_Text sucessRate;

    private void Start()
    {
        if(trainingOption != null)
        {
            image.sprite = trainingOption.icon;
            sucessRate.text = $"{trainingOption.successRate * 100}%";
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        draggableItem.parentAfterDrag = transform;

        if(trainingOption != null)
        {
            trainingOption.Train(dropped.GetComponent<LegendController>(), GameManager.instance.permanentParameters.currentLegend);
            Debug.Log("treino é aqui hein");
        }
        else
        {
            Debug.Log("musculação é lá do outro lado");
        }
    }
}
