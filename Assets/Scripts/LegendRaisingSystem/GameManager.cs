using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Permanent Parameters")]
    public PermanentParameterSO permanentParameters;

    [Header("Parameters")]
    public int daysUntilBattle;
    public int stamina;

    public List<GameObject> trainingOptions;

    public GameObject legendChoicePanel;
    public GameObject trainingOptionChoicePanel;
    public GameObject randomBattleButton;

    [Header("UI")]
    public TMP_Text daysPassedText;
    public TMP_Text daysUntilBattleText;
    public TMP_Text entityPietyText;
    public TMP_Text staminaText;
    public TMP_Text legendAgeText;

    public TMP_Text currentLegendText;
    public TMP_Text currentHpText;
    public TMP_Text currentAttackText;
    public TMP_Text currentSpecialText;
    public TMP_Text currentDefenseText;
    public TMP_Text currentSpeedText;

    public static GameManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (permanentParameters.daysPassed < 3)
            daysUntilBattle = 3;
        else
            daysUntilBattle = 4;
        
        AudioManager.instance.PlayOneShotTheme(AudioManager.instance.trainingTheme);
        SetupDay();
    }

    // Update is called once per frame
    void Update()
    {
        // UPDATE UI //
        daysPassedText.text = $"Dia {permanentParameters.daysPassed}";
        daysUntilBattleText.text = $"<color=red>{daysUntilBattle}</color> dias até o torneio!";
        entityPietyText.text = $"{permanentParameters.entityPiety}";
        staminaText.text = $"{stamina}";
        
        if(permanentParameters.currentLegend != null)
        {
            legendAgeText.text = $"{permanentParameters.currentLegend.age}";

            currentLegendText.text = permanentParameters.currentLegend.unitName;
            currentHpText.text = $"{permanentParameters.currentLegend.maxHp}";
            currentAttackText.text = $"{permanentParameters.currentLegend.attack}";
            currentSpecialText.text = $"{permanentParameters.currentLegend.special}";
            currentDefenseText.text = $"{permanentParameters.currentLegend.defense}";
            currentSpeedText.text = $"{permanentParameters.currentLegend.speed}";
        }
        // UPDATE UI //
    }

    public void SetupDay()
    {
        permanentParameters.daysPassed++;
        if (permanentParameters.currentLegend != null)
        {
            var legendController = FindFirstObjectByType<LegendController>();
            legendController.SetLegend(permanentParameters.currentLegend);
            permanentParameters.currentLegend.age++;
        }

        Debug.Log($"ei otaro começou ${daysUntilBattle}");

        daysUntilBattle--;
        if (daysUntilBattle <= 0)
        {
            SceneTransitionManager.Instance.ChangeScene("BattleScene");
            return;
        }

        Debug.Log($"ei otaro ${daysUntilBattle}");

        stamina = 3;

        if(permanentParameters.hasBattled)
            CheckForVictory();
        else
            CheckForLegend();
    }

    public void CheckForVictory()
    {
        if (permanentParameters.hasWon)
        {
            permanentParameters.hasBattled = false;
            permanentParameters.hasWon = false;
            trainingOptionChoicePanel.SetActive(true);
            
            return;
        }
        else
        {
            permanentParameters.hasBattled = false;
            permanentParameters.hasWon = false;

            permanentParameters.currentLegend.age = 10;
            permanentParameters.entityPiety--;
            if(permanentParameters.entityPiety <= 0)
            {
                // Game Over
            }
        }

        CheckForLegend();
    }

    public void CheckForLegend()
    {
        if (permanentParameters.currentLegend == null || permanentParameters.currentLegend.age >= 5)
        {
            legendChoicePanel.SetActive(true);
        }
        else
        {
            var legendController = FindFirstObjectByType<LegendController>();
            legendController.CheckForStamina();
        }

        CheckForRandomBattle();
    }

    public void PassDay()
    {
        SceneTransitionManager.Instance.CallTransitionOnly();
        SetupDay();
    }

    public void CheckForRandomBattle()
    {
        var rng = Random.value;

        if (rng <= 0.5f)
        {
            randomBattleButton.GetComponent<Button>().interactable = true;
            randomBattleButton.GetComponent<Image>().color = Color.green;
        }
        else
        {
            randomBattleButton.GetComponent<Button>().interactable = false;
            randomBattleButton.GetComponent<Image>().color = Color.red;
        }
    }

    public void RandomBattle()
    {
        SceneTransitionManager.Instance.ChangeScene("BattleScene");
    }
}
