using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameEntryMenuCCh : GameEntryMenu
{
    public override void Init()
    {
        base.Init();
        Debug.Log("GameEntryMenuChR Init");

        InterfaceManager.Init();

        InterfaceManager.BarMediator.ShowForID("Best", LeaderBoard.GetBestScore());
        InterfaceManager.BarMediator.ShowForID("MaxLevel", PlayerPrefs.GetInt("MaxLevel", 0));

        InterfaceManager.OnClickCommand += ClickCommandMenu;
    }

    private void ClickCommandMenu(InterfaceComand comand)
    {
        Image fullBlack = UnityEngine.GameObject.Find("FullBlack").GetComponent<UnityEngine.UI.Image>();

        switch (comand)
        {
            case InterfaceComand.PlayGame:
                SaveLoadManager.CurrentMap = "";
                fullBlack.DOColor(Color.black, 1.5f).OnComplete(() => GameSceneManager.LoadGame());               
                break;
            case InterfaceComand.LoadGame:
                fullBlack.DOColor(Color.black, 1.5f).OnComplete(() => GameSceneManager.LoadGame());
                break;
            default:
                break;
        }
    }
}

public static class IQSystem
{
    public static bool IsIntcraceAnim;

    public static void IncraceIQ()
    {
        int iq = GetIQ();
        SetIQ(iq + Random.Range(1, 3));
        IsIntcraceAnim = true;
        PlayerPrefs.SetInt("IsSeeIQ", 1);
    }

    public static int GetIQ()
    {
        return PlayerPrefs.GetInt("IQ", 99);
    }

    private static void SetIQ(int iq)
    {
        PlayerPrefs.SetInt("IQ", iq);
    }
}
 