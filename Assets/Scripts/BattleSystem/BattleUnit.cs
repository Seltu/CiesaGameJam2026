using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;
    private UnitSO _unitSO;
    private int _currentHp;
    private BattleMove _currentMove;
    private bool _charged;

    public BattleMove CurrentMove { get => _currentMove; set => _currentMove = value; }
    public bool Charged { get => _charged; set => _charged = value; }

    public void SetHUD(UnitSO unit)
    {
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
