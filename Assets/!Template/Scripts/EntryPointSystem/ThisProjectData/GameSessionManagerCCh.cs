using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameSessionManagerCCh : MonoBehaviour
{
    public event Action<int> OnPhaseChange;
    private int phase;

    public static int Level;
   //public static BattleModePlayerData PlayerData;
    public static EndLevelData NextLevelData;

    public CanonProjectilePlayer ProjectilePlayer { get; private set; }

    public void Init()
    {
        StartCoroutine(GameRoutine());
    }

    private IEnumerator GameRoutine()
    {
        yield return new WaitForSeconds(0.5f);
    }
}

public class EndLevelData
{
    public int maxSteps;
    public int avilibleBoxes;
    public int unlcokBoxes;
    public int money;

    public EndLevelData(int maxSteps, int avilibleBoxes, int unlcokBoxes, int money)
    {
        this.maxSteps = maxSteps;
        this.avilibleBoxes = avilibleBoxes;
        this.unlcokBoxes = unlcokBoxes;
        this.money = money;
    }
}