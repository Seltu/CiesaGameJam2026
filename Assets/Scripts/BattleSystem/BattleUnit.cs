using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private Image unitImage;
    [SerializeField] private Image shieldImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Animator unitAnimator;

    [Header("Move Sprites")]
    [SerializeField] private Sprite counterSprite;
    [SerializeField] private Sprite defendSprite;

    private UnitSO _unitSO;
    private int _currentHp;
    private BattleMove _currentMove;
    private bool _charged;

    public BattleMove CurrentMove { get => _currentMove; set => _currentMove = value; }
    public bool Charged { get => _charged; set => _charged = value; }

    public void SetHUD(UnitSO unit)
    {
        unitImage.sprite = unit.unitSprite;
        nameText.text = unit.unitName;
        _unitSO = unit;
        _currentHp = _unitSO.maxHp;
        SetHP();
    }
    public void SetHP()
    {
        hpText.text = _currentHp + "/" + _unitSO.maxHp;
        hpSlider.normalizedValue = (float)_currentHp / _unitSO.maxHp;
    }

    public void PlayMoveAnimation()
    {
        switch (_currentMove)
        {
            case BattleMove.ATTACK:
                unitAnimator.SetTrigger("Attack");
                break;

            case BattleMove.SPECIALATTACK:
                unitAnimator.SetTrigger("Special");
                break;

            case BattleMove.DEFEND:
                unitAnimator.SetTrigger("Defense");
                break;

            case BattleMove.CHARGEUP:
                unitAnimator.SetTrigger("Charge");
                break;

            case BattleMove.COUNTER:
                unitAnimator.SetTrigger("Counter");
                break;
        }
    }

    public void SetShieldSprite(bool counter)
    {
        if (counter)
            shieldImage.sprite = counterSprite;
        else
            shieldImage.sprite = defendSprite;
    }

    public bool TakeDamage(int dmg)
    {
        _currentHp -= dmg;
        SetHP();
        if (_currentHp <= 0)
            return true;
        else
            return false;
    }

    public UnitSO GetSO()
    {
        return _unitSO;
    }
}
