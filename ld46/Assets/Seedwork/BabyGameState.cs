using Cinemachine;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "BabyGameStateState.asset", menuName = "peanutbutters/BabyGameState", order = 20)]
public class BabyGameState : ScriptableObject, IState
{
    public IState NextState { get; private set; }

    public void OnEnter() {
        Juicer.ShakeCamera(0.5f);
    }

    public IEnumerator OnUpdate()
    {
        while (true) yield return null;
        //do
        //{
        //    DataDump.Set("HP", DataDump.Get<int>("HP") - 1);
        //    yield return new WaitForSeconds(1);
        //} while (DataDump.Get<int>("HP") > 0);
        //Juicer.CreateFx(0, player.transform.position);
        //GameObject.Destroy(player);
        //Juicer.ShakeCamera(1.5f);
        //bool readyToMoveOn = false;
        //MessageController.AddMessage("butterboi is dead now.", postAction: () => readyToMoveOn = true);
        //while (!readyToMoveOn)
        //{
        //    yield return null;
        //}
    }

    public void OnExit()
    {
        GameConductor.SetShowHud(false);
        ScreenFader.FadeOut();
        NextState = new CreditsState();
    }
}
