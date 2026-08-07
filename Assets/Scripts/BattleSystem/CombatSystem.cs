using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum CombatState { START, PLAYERTURN, CLASH, WON, LOST }
public class CombatSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    UnitSO playerFile;
    UnitSO enemyFile;

    public Text dialogueText;

    public BattleUnit playerUnit;
    public BattleUnit enemyUnit;

    public CombatState state;

    // Start is called before the first frame update
    void Start()
    {
        state = CombatState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerFile = playerGO.GetComponent<UnitSO>();

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyFile = enemyGO.GetComponent<UnitSO>();

        dialogueText.text = "A wild " + enemyFile.unitName + " approaches...";

        playerUnit.setHUD(playerFile);
        enemyUnit.setHUD(enemyFile);

        yield return new WaitForSeconds(2f);

        state = CombatState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        bool isDead = enemyUnit.TakeDamage(playerFile.attack);

        dialogueText.text = "The attack is successful!";

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = CombatState.WON;
            EndBattle();
        }
        else
        {
            state = CombatState.CLASH;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        dialogueText.text = enemyFile.unitName + " attacks!";

        yield return new WaitForSeconds(1f);

        bool isDead = playerUnit.TakeDamage(enemyFile.attack);

        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            state = CombatState.LOST;
            EndBattle();
        }
        else
        {
            state = CombatState.PLAYERTURN;
            PlayerTurn();
        }

    }

    void EndBattle()
    {
        if (state == CombatState.WON)
        {
            dialogueText.text = "You won the battle!";
        }
        else if (state == CombatState.LOST)
        {
            dialogueText.text = "You were defeated.";
        }
    }

    void PlayerTurn()
    {
        dialogueText.text = "Choose an action:";
    }

    public void OnAttackButton()
    {
        if (state != CombatState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }
}
