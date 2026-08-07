using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum BattleState { START, PLAYERTURN, CLASH, WON, LOST }
public enum BattleMove { CHARGEUP, ATTACK, SPECIALATTACK, DEFEND, COUNTER }
public class BattleSystem : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds1 = new WaitForSeconds(1f);
    private static WaitForSeconds _waitForSeconds2 = new WaitForSeconds(2f);
    [SerializeField] private UnitSO playerFile;
    [SerializeField] private UnitSO enemyFile;

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject playerTurnHUD;

    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;

    [SerializeField] private List<Transform> moveSlots;
    private MoveIconUI[] _selectedMoves;
    private BattleMove[] _enemyMoves;

    private BattleState _state;

    private void Awake()
    {
        EventManager.AddListener<MoveIconUI, Transform>("MoveDrop", OnMoveDrop);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<MoveIconUI, Transform>("MoveDrop", OnMoveDrop);
    }

    private void Start()
    {
        _state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    private IEnumerator SetupBattle()
    {
        dialogueText.gameObject.SetActive(true);
        playerTurnHUD.gameObject.SetActive(false);

        dialogueText.text = enemyFile.unitName + " te desafia!";

        playerUnit.SetHUD(playerFile);
        enemyUnit.SetHUD(enemyFile);

        yield return _waitForSeconds2;

        _state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    private IEnumerator ClashTurn()
    {
        for (int i = 0; i < _selectedMoves.Length; i++)
        {
            MoveIconUI move = _selectedMoves[i];
            playerUnit.CurrentMove = move.GetMove();
            enemyUnit.CurrentMove = _enemyMoves[i];
            if(playerFile.speed > enemyFile.speed)
            {
                yield return StartCoroutine(ProcessMove(playerUnit, enemyUnit));
                if (_state != BattleState.CLASH) yield break;
                yield return StartCoroutine(ProcessMove(enemyUnit, playerUnit));
            }
            else
            {
                yield return StartCoroutine(ProcessMove(enemyUnit, playerUnit));
                if (_state != BattleState.CLASH) yield break;
                yield return StartCoroutine(ProcessMove(playerUnit, enemyUnit));
            }
            if (_state != BattleState.CLASH) yield break;
        }
        _state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    private IEnumerator ProcessMove(BattleUnit unit, BattleUnit target)
    {
        switch (unit.CurrentMove)
        {
            case BattleMove.CHARGEUP:
                yield return StartCoroutine(ChargeUp(unit));
                break;
            case BattleMove.ATTACK:
                yield return StartCoroutine(Attack(unit, target));
                break;
            case BattleMove.SPECIALATTACK:
                yield return StartCoroutine(SpecialAttack(unit, target));
                break;
            case BattleMove.DEFEND:
            case BattleMove.COUNTER:
                if (target.CurrentMove == BattleMove.DEFEND|| target.CurrentMove == BattleMove.COUNTER)
                {
                    dialogueText.text = "Ninguém atacou!";
                    yield return _waitForSeconds1;
                }
                else if (target.CurrentMove == BattleMove.CHARGEUP)
                {
                    if(unit.CurrentMove == BattleMove.DEFEND)
                        if (unit == playerUnit)
                            dialogueText.text = "Seu " + target.GetSO().unitName + " tentou se defender e falhou!";
                        else
                            dialogueText.text = target.GetSO().unitName + " inimigo tentou se defender e falhou!";
                    else
                        if (unit == playerUnit)
                            dialogueText.text = "Seu " + target.GetSO().unitName + " tentou contra-atacar e falhou!";
                        else
                            dialogueText.text = target.GetSO().unitName + " inimigo tentou contra-atacar e falhou!";
                    yield return _waitForSeconds1;
                }
                break;
        }
    }
    private IEnumerator ChargeUp(BattleUnit unit)
    {
        var unitFile = unit.GetSO();
        if (unit == playerUnit)
            dialogueText.text = "Seu " + unitFile.unitName + " está carregando seu ataque!";
        else
            dialogueText.text = unitFile.unitName + " inimigo está carregando seu ataque!";

        unit.Charged = true;

        yield return _waitForSeconds2;
    }

    private IEnumerator Attack(BattleUnit unit, BattleUnit target)
    {
        var unitFile = unit.GetSO();
        if(unit.CurrentMove == BattleMove.ATTACK)
            if (unit == playerUnit)
                dialogueText.text = "Seu " + unitFile.unitName + " atacou!";
            else
                dialogueText.text = unitFile.unitName + " inimigo atacou!";

        yield return _waitForSeconds1;

        int attackDamage = unit.Charged ? unitFile.attack * 2 : unitFile.attack;
        unit.Charged = false;

        bool isDead = false;
        if (target.CurrentMove == BattleMove.DEFEND)
        {
            if(unit == playerUnit)
                dialogueText.text = "Porém, " + target.GetSO().unitName + " inimigo se defendeu!";
            else
                dialogueText.text = "Porém, seu " + target.GetSO().unitName + " se defendeu!";
        }
        else
            isDead = target.TakeDamage(attackDamage);

        yield return _waitForSeconds1;

        if (isDead)
        {
            if (unit == playerUnit)
                _state = BattleState.WON;
            else
                _state = BattleState.LOST;
            EndBattle();
            yield break;
        }

        if (target.CurrentMove == BattleMove.COUNTER)
        {
            if (unit == playerUnit)
                dialogueText.text = "Em resposta, " + target.GetSO().unitName + " inimigo contra-atacou!";
            else
                dialogueText.text = "Em resposta, seu " + target.GetSO().unitName + " contra-atacou!";
            yield return StartCoroutine(Attack(target, unit));
            yield break;
        }
    }

    private IEnumerator SpecialAttack(BattleUnit unit, BattleUnit target)
    {
        var unitFile = unit.GetSO();
        if (unit == playerUnit)
            dialogueText.text = "Seu " + unitFile.unitName + " usou um ataque especial!";
        else
            dialogueText.text = unitFile.unitName + " inimigo usou um ataque especial!";

        yield return _waitForSeconds1;

        int attackDamage = unit.Charged ? unitFile.attack * 2 : unitFile.attack;
        unit.Charged = false;

        bool isDead;
        if (target.CurrentMove == BattleMove.COUNTER)
        {
            if (unit == playerUnit)
                dialogueText.text = "Porém, " + target.GetSO().unitName + " inimigo contra-atacou!";
            else
                dialogueText.text = "Porém, seu " + target.GetSO().unitName + " contra-atacou!";
            yield return StartCoroutine(Attack(target, unit));
            yield break;
        }
        else if (target.CurrentMove == BattleMove.DEFEND)
        {
            if (unit == playerUnit)
                dialogueText.text = target.GetSO().unitName + " inimigo se defendeu!";
            else
                dialogueText.text = "Seu " + target.GetSO().unitName + " se defendeu!";
            yield return _waitForSeconds1;
            dialogueText.text = "Porém, metade do dano atravessa a sua defesa!";
            isDead = target.TakeDamage(Mathf.CeilToInt(attackDamage / 2f));
        }
        else
            isDead = target.TakeDamage(attackDamage);

        yield return _waitForSeconds1;

        if (isDead)
        {
            if (unit == playerUnit)
                _state = BattleState.WON;
            else
                _state = BattleState.LOST;
            EndBattle();
        }
    }

    private void EndBattle()
    {
        if (_state == BattleState.WON)
        {
            dialogueText.text = "Você venceu!";
        }
        else if (_state == BattleState.LOST)
        {
            dialogueText.text = "Seu " + playerFile.unitName + " foi derrotado.";
        }
    }

    private void PlayerTurn()
    {
        _selectedMoves = new MoveIconUI[moveSlots.Count];
        _enemyMoves = new BattleMove[3];
        for (int i = 0; i < moveSlots.Count; i++)
        {
            BattleMove[] moves = (BattleMove[])Enum.GetValues(typeof(BattleMove));
            BattleMove enemyMove = moves[Random.Range(0, moves.Length)];
            _enemyMoves[i] = enemyMove;
        }
        dialogueText.gameObject.SetActive(false);
        playerTurnHUD.gameObject.SetActive(true);
    }

    private void OnMoveDrop(MoveIconUI moveIcon, Transform slotTransform)
    {
        if (moveSlots.Contains(slotTransform))
        {
            _selectedMoves[moveSlots.IndexOf(slotTransform)] = moveIcon;
        }
    }

    public void OnOkButton()
    {
        if (_state != BattleState.PLAYERTURN)
            return;

        foreach (var moveIcon in _selectedMoves)
        {
            if (moveIcon == null) return;
        }

        dialogueText.gameObject.SetActive(true);
        playerTurnHUD.gameObject.SetActive(false);

        _state = BattleState.CLASH;
        StartCoroutine(ClashTurn());
    }
}
