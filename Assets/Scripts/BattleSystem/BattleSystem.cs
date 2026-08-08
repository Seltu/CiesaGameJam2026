using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum BattleState { START, PLAYERTURN, CLASH, WON, LOST }
public enum BattleMove { CHARGEUP, ATTACK, SPECIALATTACK, DEFEND, COUNTER }
public class BattleSystem : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds1 = new WaitForSeconds(1f);
    private static WaitForSeconds _waitForSeconds2 = new WaitForSeconds(2f);
    [SerializeField] private BattleSetupSO battleSetupSO;

    private UnitSO _playerFile;
    private UnitSO _enemyFile;

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject playerTurnHUD;
    [SerializeField] private GameObject clashTurnHUD;

    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;

    [SerializeField] private Image playerBubbleIcon;
    [SerializeField] private Image enemyBubbleIcon;

    [SerializeField] private List<Transform> moveSlots;
    private MoveIconUI[] _selectedMoves;
    private BattleMove[] _enemyMoves;

    [SerializeField] private List<Sprite> moveIconSprites;

    private BattleState _state;
    private bool _bothDefended;

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
        var musicID = Random.Range(0, AudioManager.instance.stageThemeLoops.Count);
        AudioManager.instance.PlayAndLoop(AudioManager.instance.stageThemeIntros[musicID], AudioManager.instance.stageThemeLoops[musicID]);

        _state = BattleState.START;
        _playerFile = PermanentParameterManager.instance.permanentParameters.currentLegend;
        _enemyFile = battleSetupSO.RandomUnit();

        // Scale enemy
        var levels = PermanentParameterManager.instance.permanentParameters.currentLegend.level;

        for (int i = 0; i < levels; i++)
        {
            var rng = Random.Range(0, levels);

            switch (rng)
            {
                case 0:
                    _enemyFile.maxHp++;
                    break;
                case 1:
                    _enemyFile.attack++;
                    break;
                case 2:
                    _enemyFile.special++;
                    break;
                case 3:
                    _enemyFile.defense++;
                    break;
                case 4:
                    _enemyFile.speed++;
                    break;
            }
        }

        _enemyFile.level = levels;

        StartCoroutine(SetupBattle());
    }

    private IEnumerator SetupBattle()
    {
        dialogueText.gameObject.SetActive(true);
        playerTurnHUD.gameObject.SetActive(false);
        clashTurnHUD.gameObject.SetActive(false);

        dialogueText.text = _enemyFile.unitName + " te desafia!";

        playerUnit.SetHUD(_playerFile);
        enemyUnit.SetHUD(_enemyFile);

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
            playerBubbleIcon.sprite = moveIconSprites[(int)playerUnit.CurrentMove];
            enemyBubbleIcon.sprite = moveIconSprites[(int)enemyUnit.CurrentMove];
            _bothDefended = false;
            if (_playerFile.speed > _enemyFile.speed)
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
                if (_bothDefended) break;
                if (target.CurrentMove == BattleMove.DEFEND|| target.CurrentMove == BattleMove.COUNTER)
                {
                    dialogueText.text = "Ninguém atacou!";
                    _bothDefended = true;
                    unit.SetShieldSprite(unit.CurrentMove == BattleMove.COUNTER);
                    target.SetShieldSprite(target.CurrentMove == BattleMove.COUNTER);
                    unit.CurrentMove = BattleMove.DEFEND;
                    target.CurrentMove = BattleMove.DEFEND;
                    unit.PlayMoveAnimation();
                    target.PlayMoveAnimation();
                    yield return _waitForSeconds1;
                }
                else if (target.CurrentMove == BattleMove.CHARGEUP)
                {
                    unit.SetShieldSprite(unit.CurrentMove == BattleMove.COUNTER);
                    unit.PlayMoveAnimation();
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

        unit.PlayMoveAnimation();
        unit.Charged = true;

        yield return _waitForSeconds2;
    }

    private IEnumerator Attack(BattleUnit unit, BattleUnit target)
    {
        var unitFile = unit.GetSO();
        if(unit.CurrentMove == BattleMove.ATTACK)
        {
            if (unit == playerUnit)
                dialogueText.text = "Seu " + unitFile.unitName + " atacou!";
            else
                dialogueText.text = unitFile.unitName + " inimigo atacou!";
        }
        unit.SetShieldSprite(true);
        if (unit.CurrentMove== BattleMove.COUNTER && target.CurrentMove == BattleMove.ATTACK)
        { // Troca animação pra ataque se estiver counterando um ataque
            unit.CurrentMove = BattleMove.ATTACK;
            unit.PlayMoveAnimation();
            unit.CurrentMove = BattleMove.COUNTER;
        }
        else
            unit.PlayMoveAnimation();

        yield return _waitForSeconds1;

        int attackDamage = unit.Charged ? unitFile.attack * 2 : unitFile.attack;
        unit.Charged = false;

        bool isDead = false;
        if (target.CurrentMove == BattleMove.DEFEND)
        {
            target.SetShieldSprite(false);
            target.PlayMoveAnimation();
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

        unit.PlayMoveAnimation();

        yield return _waitForSeconds1;

        int attackDamage = unit.Charged ? unitFile.special * 2 : unitFile.special;
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
            target.SetShieldSprite(false);
            target.PlayMoveAnimation();
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
            PermanentParameterManager.instance.permanentParameters.hasBattled = true;
            PermanentParameterManager.instance.permanentParameters.hasWon = true;
        }
        else if (_state == BattleState.LOST)
        {
            dialogueText.text = "Seu " + _playerFile.unitName + " foi derrotado.";
            PermanentParameterManager.instance.permanentParameters.hasBattled = true;
            PermanentParameterManager.instance.permanentParameters.hasWon = false;
        }

        StartCoroutine(EndBattleCoroutine());
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
        clashTurnHUD.gameObject.SetActive(false);
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
        clashTurnHUD.gameObject.SetActive(true);

        _state = BattleState.CLASH;
        StartCoroutine(ClashTurn());
    }

    IEnumerator EndBattleCoroutine()
    {
        if (_state == BattleState.WON)
        {
            AudioManager.instance.PlayOneShotTheme(AudioManager.instance.stageClearTheme);
        }
        else if (_state == BattleState.LOST)
        {
            AudioManager.instance.PlayOneShotTheme(AudioManager.instance.gameOverTheme);
        }

        yield return new WaitForSeconds(7f);

        SceneTransitionManager.Instance.ChangeScene("TrainingScene");
    }
}
