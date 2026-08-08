using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Legend Unit", menuName = "Legends/UnitSO", order = 1)]
public class UnitSO : ScriptableObject
{
    public Sprite unitSprite;
    public string unitName;
    public int maxHp;
    public int attack;
    public int special;
    public int defense;
    public int speed;
}