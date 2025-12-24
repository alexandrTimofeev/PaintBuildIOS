using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(-99)]
public class GameEntryGameplayCCh : GameEntryGameplay
{
    private GlobalMover globalMover;
    private Spawner spawner;
    private GameStateManager stateManager;
    private SpawnManager spawnManager;
    private ResourceSystem resourceSystem;

    public static GameSessionDataContainer DataContainer { get; private set; }

    private PlayerChCF player;
    private TimerStarter timerStarter;
    private GameTimer gameTimer;

    public static LevelData DataLevel;
    public static ScoreSystem GameScoreSystem;

    private GrappableObjectMediator ObjectMediator;
    public static IInput input;
    public static AchiviementMediator AchivMediator;
    public static GameSessionManagerCCh gameManager;

    public static MovebleObjectInput movebleObjectInput;
    public static ButtonMediator buttonMediator;
    public static InstrumentManager instrumentManager;
    public static SaveLoadManager saveLoadManager;
    public static WeatherManager weatherManager;
    public static UIManagerMono uIManagerMono;

    public override void Init()
    {
        //base.Init();
        Debug.Log("GameEntryGameplay Init");

        InitializeReferences();
        InitializeLevel();
        InitializeInterface();
        InitializeAudio();
        SetupGameManager();
        SetupScoreSystem();
        SetupSpawnerAndProgressBar();
        SetupInterfaceEvents();
        SetupHealthContainer();
        SetupSpawnManager();
        SetupGameStateEvents();
        SetupPlayer();
        SetupWinLoseCondition();
        SetupBonuses();
        SetupAchivmients();
        SetupSkins();
        SetupInteractionMediator();
        SetupButtonMediator();
        SetupInstrumentManager();
        LoadLevel();

        //gameManager.ApplyNextLevelData();        

        GamePause.SetPause(false);
        stateManager.SetState(GameState.Game);
        instrumentManager.SetInstrument(InstrumentType.None);
    }

    private void Lose()
    {
        stateManager.SetState(GameState.Lose);
    }

    private void InitializeReferences()
    {
        input = InputFabric.GetOrCreateInpit();
        globalMover = Object.FindFirstObjectByType<GlobalMover>();
        spawner = Object.FindFirstObjectByType<Spawner>();
        weatherManager = Object.FindFirstObjectByType<WeatherManager>();
        uIManagerMono = Object.FindFirstObjectByType<UIManagerMono>();

        AchivMediator = new AchiviementMediator();
        ObjectMediator = new GrappableObjectMediator();
        GameScoreSystem = new ScoreSystem();

        GrapCollider.Mediator = ObjectMediator;
        stateManager = new GameStateManager();
        spawnManager = new SpawnManager();
        resourceSystem = new ResourceSystem();

        gameManager = (new GameObject("GameManager")).AddComponent<GameSessionManagerCCh>();

        DataContainer = Resources.Load<GameSessionDataContainer>("GameSessionDataContainer").Clone();
        if (DataLevel == null)
            DataLevel = Resources.Load<LevelData>($"Levels/{DataContainer.StandartLevelData}");

        movebleObjectInput = new MovebleObjectInput(input, DataContainer.SelectLayer, DataContainer.MoveLayer, DataContainer.FirstSelectLayer,
            DataContainer.WallLayer);
        movebleObjectInput.SetEnable(true);
        MoveObjectSystem.Init();
        buttonMediator = new ButtonMediator();
        instrumentManager = new InstrumentManager();
        saveLoadManager = new SaveLoadManager();

        /*timerStarter = (new GameObject()).AddComponent<TimerStarter>();
        if(timerStarter != null && DataLevel != null)
            timerStarter.Play(DataLevel.Timer);
        else
            timerStarter.Stop();
        gameTimer = timerStarter.Timer;*/

        //timerStarter.IsUnTimeScale = true;
    }

    private void LoadLevel()
    {
        saveLoadManager.Init();
    }

    private void InitializeLevel()
    {
        if (DataLevel == null)
            return;

        Transform levelTr = GameObject.Find("Level").transform;
        if (DataLevel.LevelPrefab)
            GameObject.Instantiate(DataLevel.LevelPrefab, levelTr);


        GameObject background = GameObject.Find("Background");
        if (background.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            if (DataLevel != null && DataLevel.Background)
                spriteRenderer.sprite = DataLevel.Background;
        if (GameSessionManagerCCh.Level > 0)
            background.GetComponent<ColorChanger>().RandomColorGradient();
        }
    }

    private void InitializeInterface()
    {
        InterfaceManager.Init();
        InterfaceManager.BarMediator.ShowForID("Score", 0);
        InterfaceManager.BarMediator.ShowForID("Level", GameSessionManagerCCh.Level + 1);
    }

    private void InitializeAudio()
    {
        AudioManager.Init();
        AudioManager.PlayMusic();
        SoundManager.Init();

        DataContainer.OnChangeSpeedGame += (speed) => AudioManager.SetSpeedMusic(((speed - 1f) * 0.5f) + 1f);
    }

    private void SetupScoreSystem()
    {
        GameScoreSystem.OnScoreChange += (score, point) =>
        {
            InterfaceManager.BarMediator.ShowForID("Score", score);
        };

        GameScoreSystem.OnAddScore += (score, point) => DataContainer.MoneyContainer.AddValue(score / 80);
        DataContainer.MoneyContainer.OnChangeValue += (value, delta) => InterfaceManager.BarMediator.ShowForID("Money", value);
        DataContainer.MoneyContainer.OnChangeValue += (value, delta) => 
        InterfaceManager.CreateFlyingText($"<size=20>{(delta < 0 ? "" : "+")}{delta}<color=yellow>$", delta < 0 ? Color.red : Color.white, 
        Camera.main.ScreenToWorldPoint(input.GetOverPosition()) + Vector3.forward, null);
        DataContainer.MoneyContainer.UpdateValue();

        //scoreSystem.OnAddScore += InterfaceManager.CreateScoreFlyingText;
        //scoreSystem.OnRemoveScore += InterfaceManager.CreateScoreFlyingText;
    }

    private void SetupSpawnerAndProgressBar()
    {
        if (spawner == null)
            return;

        spawner.OnInstructionProgress += (id, progress) =>
        {
            InterfaceManager.BarMediator.ShowForID("Progress", progress);
        };
    }

    private void SetupInterfaceEvents()
    {
        InterfaceManager.OnClose += (window) =>
        {
            if (window is PauseWindowUI)
                stateManager.BackState();
        };

        InterfaceManager.OnOpen += (window) =>
        {
            if (window is PauseWindowUI)
                stateManager.SetState(GameState.Pause);
        };     

        if (gameTimer != null)
            gameTimer.OnTick += (s) => InterfaceManager.BarMediator.ShowForID("Timer", s);
    }

    private void SetupHealthContainer()
    {
        DataContainer.HealthContainer.OnChangeValue += (life, delta) => InterfaceManager.BarMediator.ShowForID("Life", life);
        DataContainer.HealthContainer.OnChangeValue += (life, delta) => 
        InterfaceManager.BarMediator.SetMaxForID("Life", DataContainer.HealthContainer.ClampRange.y);
        DataContainer.HealthContainer.UpdateValue();
    }

    private void SetupSpawnManager()
    {
        if (DataContainer.UseSpawnManager == false)
            return;

        var settings = Resources.Load<SpawnerSettings>("Spawn/SpawnerSettings");
        spawnManager.Init(spawner, settings);

        if(spawner)
            spawnManager.OnChangeSpeed += spawner.SetSpeed;
        spawnManager.OnChangeSpeed += (speed) => DataContainer.SpeedGame = speed;
        if(globalMover)
            spawnManager.OnChangeSpeed += globalMover.SetSpeedCoef;
    }

    private void SetupGameStateEvents()
    {
        /*
         TutorialWindowUI tutorialWindow = InterfaceManager.CreateAndShowWindow<TutorialWindowUI>();
        tutorialWindow.OnClose += (win) => stateManager.SetState(GameState.Game);
        tutorialWindow.Init(input);
        */

        stateManager.OnWin += () =>
        {
            RecordData recordData = LeaderBoard.GetScore($"score_{LevelSelectWindow.CurrentLvl}");
            bool newRecord = recordData == null || recordData.score < GameScoreSystem.Score;
            InterfaceManager.ShowWinWindow(GameScoreSystem.Score, (recordData != null ? recordData.score : 0));
            LeaderBoard.SaveScore($"score_{LevelSelectWindow.CurrentLvl}", GameScoreSystem.Score);
            LevelSelectWindow.CompliteLvl();

            if (GameSessionManagerCCh.Level + 1 > PlayerPrefs.GetInt("MaxLevel", 0))
                PlayerPrefs.SetInt("MaxLevel", GameSessionManagerCCh.Level);

            if (newRecord)
                IQSystem.IncraceIQ();
        };

        stateManager.OnLose += () =>
        {
            //InterfaceManager.ShowLoseWindow(scoreSystem.Score, LeaderBoard.GetBestScore());
            //LeaderBoard.SaveScore($"default", scoreSystem.Score);
            //LevelSelectWindow.CompliteLvl();
            RecordData recordData = LeaderBoard.GetScore($"score_{LevelSelectWindow.CurrentLvl}");
            bool newRecord = recordData == null || recordData.score < GameScoreSystem.Score;
            InterfaceManager.ShowLoseWindow(GameScoreSystem.Score, recordData != null ? recordData.score : 0);
            LeaderBoard.SaveScore($"score_{LevelSelectWindow.CurrentLvl}", GameScoreSystem.Score);

            if (GameSessionManagerCCh.Level > PlayerPrefs.GetInt("MaxLevel", 0))
                PlayerPrefs.SetInt("MaxLevel", GameSessionManagerCCh.Level);

            if (newRecord)            
                IQSystem.IncraceIQ();            
        };

        stateManager.OnStateChange += (state) =>
        {
            GamePause.SetPause(state != GameState.Game);
            AudioManager.PassFilterMusic(state != GameState.Game);

            if (state == GameState.Win || state == GameState.Lose)
            {
                if (player != null)
                    player.gameObject.SetActive(false);
                AudioManager.StopMusic();
                if (gameTimer != null)
                    gameTimer.Stop();
            }
        };
    }

    private void SetupPlayer()
    {
        player = Object.FindFirstObjectByType<PlayerChCF>();

        if (player != null)
        {
            player.Init(input);
            player.OnDamage += (dmgCont) =>
            {
                DataContainer.HealthContainer.RemoveValue(1);
            };

            player.OnGroundUpdate += (plane) =>
            {
                GameScoreSystem.AddScore(DataContainer.AnGroundBonus, player.transform.position + (Vector3.down * 1f));
                InterfaceManager.CreateScoreFlyingText(DataContainer.AnGroundBonus, player.transform.position + (Vector3.down * 1f), 0.05f);
            };
            //DataContainer.OnChangeSpeedGame += (speed) => player.SetSpeedKof(speed);
        }
    }

    private void SetupWinLoseCondition()
    {
        if(gameTimer != null)
            gameTimer.OnComplete += () => stateManager.SetState(GameState.Win);

        DataContainer.HealthContainer.OnDownfullValue += (_) =>
        {
            stateManager.SetState(GameState.Lose);
        };
    }

    private void SetupBonuses()
    {
        ObjectMediator.Subscribe<AddScoreGrapAction>((beh, grapOb) =>
        {
            GameScoreSystem.AddScore(beh.AddScore);
            InterfaceManager.CreateScoreFlyingText(beh.AddScore, grapOb.transform.position, 0.005f);
        });

        ObjectMediator.Subscribe<AddLifeGrapAction>((beh, grapOb) =>
        {
            DataContainer.HealthContainer.AddValue(beh.AddLife);
        });

        ObjectMediator.Subscribe<SlowMotionGrapAction>((beh, grapOb) =>
        {
            player.SlowMotion(beh.Duration);
        });

        ObjectMediator.Subscribe<InvictibleGrapAction>((beh, grapOb) =>
        {
            player.Invictible(beh.Duration);
        });
    }

    private void SetupGameManager()
    {
        gameManager.Init();
        gameManager.OnPhaseChange += (phase) =>
        {
            spawnManager.NextPhase();
        };
    }

    private void SetupAchivmients()
    {
        /*AchivMediator.AddAchiviementForEndLevel<GameObject>("Grounds", (grounds) =>
        {
            if (grounds == null || grounds.Count > 0)
            {
                if (grounds.Count >= 6)
                    AchieviementSystem.ForceUnlock("UseAllPlatform");
            }
            else
            {
                AchieviementSystem.ForceUnlock("NotUsePlatform");
            }
        });
        spawner.OnObjectTimerDestroy += (spOb) =>
        {
            if (spOb.GetComponent<GrapObject>().IsActive)
                AchivMediator.ChangeStateAchiviementForEndLevel("GrapAllEggs", false);
        };

        stateManager.OnWin += () =>
        {
            AchivMediator.InvokeEndLevel();
        };

        AchivMediator.AddAchiviementForEndLevel("NotGrapEggs", true, (isNotGrap) =>
        {
            if (isNotGrap)
                AchieviementSystem.ForceUnlock("NotGrapEggs");
        });
        AchivMediator.AddAchiviementForEndLevel("GrapAllEggs", true, (isGrapAll) =>
        {
            if (isGrapAll)
                AchieviementSystem.ForceUnlock("GrapAllEggs");
        });*/

        if (player == null)
            return;

        player.OnGroundUpdate += (go) =>
        {
            AchivMediator.AddInList("Grounds", go, false);
        };
        player.OnGrap += (egg) =>
        {
            AchivMediator.ChangeStateAchiviementForEndLevel("NotGrapEggs", false);
        };
    }

    private void SetupSkins()
    {

        AchivMediator.AddAchiviementForEndLevel("NoDamage", true, (isNoDamage) =>
        {
            if (isNoDamage)
            {
                SkinsSystem.UnlockSkin("NoDamage");
                if (DataLevel.ID == "Last")
                    SkinsSystem.UnlockSkin("NoDamageLast");
            }            
        });

        if (player == null)
            return;

        player.SetSkin(SkinsSystem.GetCurrentSkin());
        player.OnDamage += (egg) =>
        {
            AchivMediator.ChangeStateAchiviementForEndLevel("NoDamage", false);
        };
    }

    private void SetupInteractionMediator()
    {
    }

    public static Vector3 GetMousePoint (float offsetRandom = 0f)
    {
        return Camera.main.ScreenToWorldPoint(input.GetOverPosition()) + Vector3.forward + (Vector3)(UnityEngine.Random.insideUnitCircle * offsetRandom);
    }

    private void SetupButtonMediator()
    {
        /*Ray ray = new Ray(Camera.main.transform.position + (Camera.main.transform.right * 5f), Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfoCob, Mathf.Infinity, DataContainer.MoveLayer))
        {

        }*/
        Vector3 pointCreate = Vector3.zero;

        buttonMediator.OnButtonClick += (clickInfo) =>
        {
            switch (clickInfo.action)
            {
                case ButtonMSAction.None:
                    MoveObjectBank.ClearCreateObject();
                    break;

                case ButtonMSAction.Delite:
                    MoveObjectSystem.DeliteCurrentObject();
                    break;
                case ButtonMSAction.CreateObjectNum:

                    MoveObjectBank.ChoiceCreateObject(clickInfo.number);
                    //pointCreate = GameObject.Find("SpawnCreate").transform.position + (Vector3.ProjectOnPlane(Random.insideUnitSphere, Vector3.up) + Vector3.up);
                    //MoveObjectBank.CreateObject(clickInfo.number, pointCreate);
                    break;
                case ButtonMSAction.CreateObjectStr:

                    MoveObjectBank.ChoiceCreateObject(clickInfo.text);
                    //pointCreate = GameObject.Find("SpawnCreate").transform.position + (Vector3.ProjectOnPlane(Random.insideUnitSphere, Vector3.up) + Vector3.up);
                    //MoveObjectBank.CreateObject(clickInfo.text, pointCreate);
                    break;

                case ButtonMSAction.SwitchLayerMode:
                    LayerObject.SeeLayerUpdateAllSwitch();                     
                    break;

                case ButtonMSAction.SwitchInstrument:
                    instrumentManager.SetInstrument((InstrumentType)Enum.Parse(typeof(InstrumentType), clickInfo.text));
                    break;

                case ButtonMSAction.NextVisual:
                    if(MoveObjectSystem.CurrentObject)
                        MoveObjectSystem.CurrentObject.NextVisual();
                    break;

                case ButtonMSAction.Save:
                    SaveLoadManager.Save();
                    break;

                case ButtonMSAction.Load:
                    SaveLoadManager.LoadCurrentMap();
                    break;

                case ButtonMSAction.NextWeather:
                    weatherManager.NextWeather();
                    break;

                case ButtonMSAction.HideUI:
                    uIManagerMono.SwichUIState();
                    break;

                case ButtonMSAction.Screeshot:
                    uIManagerMono.DoScreen();
                    break;

                default:
                    break;
            }
        };
    }

    private void SetupInstrumentManager()
    {
        instrumentManager.OnChangeInstrument += (instrument) =>
        {
            switch (instrument)
            {
                case InstrumentType.None:
                    break;
                case InstrumentType.Create:
                    break;
                case InstrumentType.HideWall:
                    break;
                default:
                    break;
            }

            Tap3DButton.ViewButtonID("HideWall", instrument == InstrumentType.HideWall);
            uIManagerMono.ChoiceStateUI(instrument.ToString());
            uIManagerMono.CreateState(instrument == InstrumentType.Create);
        };
    }
}