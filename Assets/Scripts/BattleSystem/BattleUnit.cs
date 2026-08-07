using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    private int _maxHp;
    private int _currentHp;
    
    public void setHUD(UnitSO unit)
    {
        nameText.text = unit.unitName;
        hpText.text = unit.maxHp.ToString();
        setHP();
    }
    public void setHP()
    {
        hpText.text = _currentHp + "/" + _maxHp;
    }
    public bool TakeDamage(int dmg)
    {
        _currentHp -= dmg;
        if (_currentHp <= 0)
            return true;
        else
            return false;
    }
}
