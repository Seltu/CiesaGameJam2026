using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LegendController : MonoBehaviour
{
    public UnitSO unitSO;
    public Image image;
    public GameObject reactionPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLegend(UnitSO legendSO)
    {
        unitSO = legendSO;
        image.sprite = unitSO.unitSprite;
    }

    public void DisplayTrainingResults(bool sucess)
    {
        StartCoroutine(DisplayTrainingResultsCoroutine(sucess));
    }

    IEnumerator DisplayTrainingResultsCoroutine(bool sucess)
    {
        if(unitSO.maxHp <= 0)
        {
            GameManager.instance.permanentParameters.currentLegend = null;
            GameManager.instance.PassDay();
            yield break;
        }

        if(sucess)
        {
            reactionPanel.GetComponent<Image>().color = Color.green;
            reactionPanel.GetComponentInChildren<TMP_Text>().text = "deu bom o treinao";
        }
        else
        {
            reactionPanel.GetComponent<Image>().color = Color.red;
            reactionPanel.GetComponentInChildren<TMP_Text>().text = "puta q parolas";
        }

        GameManager.instance.stamina--;

        reactionPanel.SetActive(true);

        yield return new WaitForSeconds(1f);

        CheckForStamina();
    }

    public void CheckForStamina()
    {
        if (GameManager.instance.stamina <= 0)
        {
            reactionPanel.GetComponent<Image>().color = Color.blue;
            reactionPanel.GetComponentInChildren<TMP_Text>().text = "cansei por hoje";

            image.raycastTarget = false;
        }
        else
        {
            reactionPanel.SetActive(false);

            image.raycastTarget = true;
        }
    }
}
