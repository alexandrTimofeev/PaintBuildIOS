using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameSessionDataContainer", menuName = "SGames/GameSessionDataContainer")]
public class GameSessionDataContainer : ScriptableObject
{
    public IntContainer HealthContainer;
    [SerializeField] private float speedGame = 1f;
    public float TimerPhase = 5f;

    [Space]
    public int AnGroundBonus = 100;

    [Space]
    public string StandartLevelData = "Level_2";

    [Space]
    public bool UseSpawnManager = true;
    public IntContainer MoneyContainer;

    [Space]
    public LayerMask FirstSelectLayer;
    public LayerMask SelectLayer;
    public LayerMask MoveLayer;
    public LayerMask WallLayer;

    [Space(10)]
    public GameObject CreateVFX;
    public GameObject DeliteVFX;
    public GameObject VisualVFX;

    [Space]
    public SFXPlayPreset LightAngleSFX;
    public SFXPlayPreset LightPowerSFX;
    public SFXPlayPreset ScreenshootSFX;
    public SFXPlayPreset ChangeObSizeSFX;
    public SFXPlayPreset ChangeObPositionSFX;

    [Space]
    public List<MovebleObject> movebleObjectsPrefs = new List<MovebleObject>();

    public float SpeedGame { get => speedGame; set { speedGame = value; OnChangeSpeedGame?.Invoke(speedGame); } }

    public Action<float> OnChangeSpeedGame;

    public GameSessionDataContainer(IntContainer healthContainer, float speedGame)
    {
        HealthContainer = healthContainer;
        SpeedGame = speedGame;
    }

    public GameSessionDataContainer Clone()
    {
        return Instantiate(this);
    }
}