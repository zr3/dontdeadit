using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PetController : MonoBehaviour
{
    public GameObject EggStage;

    public GameObject BabyStage;

    public GameObject JuviStage;
    public GameObject JuviCuteStage;
    public GameObject JuviChubbyStage;
    public GameObject JuviToughStage;

    public GameObject DeadStage;

    public GameObject PhoenixStage;
    
    public GameObject FoodMenu;
    public GameObject[] FoodButtons;

    public AudioClip CloseMenuClip;

    public int InitialFullness = 10;
    public int FullnessTolerance = 30;
    public int FullnessDeath => FullnessTolerance * 3;
    public int MinutesSpentAsChild = 5;
    public int MinutesSpentDead = 5;
    public int TimeStepInSeconds = 15;

    public void Start()
    {
        StartCoroutine(TimeLoop());
    }

    private IEnumerator TimeLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(TimeStepInSeconds);
            TimeStep();
        }
    }

    public void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void OnGrowthStageChanged(int newStage)
    {
        Juicer.CreateFx(0, transform.position);
        switch ((GrowthStage)newStage)
        {
            case GrowthStage.Egg:
                PhoenixStage.SetActive(false);
                EggStage.SetActive(true);
                break;
            case GrowthStage.Hatching:
                StartCoroutine(Hatch());
                break;
            case GrowthStage.Baby:
                EggStage.SetActive(false);
                BabyStage.SetActive(true);
                break;
            case GrowthStage.Juvi:
                BabyStage.SetActive(false);
                JuviStage.SetActive(true);
                var petType = CalculatePetType();
                switch(petType)
                {
                    case PetType.Cute:
                        JuviCuteStage.SetActive(true);
                        break;
                    case PetType.Chubby:
                        JuviChubbyStage.SetActive(true);
                        break;
                    case PetType.Tough:
                        JuviToughStage.SetActive(true);
                        break;
                }
                break;
            case GrowthStage.Dead:
                EggStage.SetActive(false);
                BabyStage.SetActive(false);
                JuviStage.SetActive(false);
                JuviCuteStage.SetActive(false);
                JuviChubbyStage.SetActive(false);
                JuviToughStage.SetActive(false);
                DeadStage.SetActive(true);
                break;
            case GrowthStage.Pheonix:
                DeadStage.SetActive(false);
                PhoenixStage.SetActive(true);
                break;
        }
    }

    public void Poke()
    {
        int pokes = DataDump.Get<int>("LocalPetPokes") + 1;
        DataDump.Set("LocalPetPokes", pokes);
        var stage = (GrowthStage)DataDump.Get<int>("LocalPetGrowthStage");
        switch (stage)
        {
            case GrowthStage.Egg:
                Juicer.Instance.PlayPickSFX();
                if (pokes > 5)
                {
                    DataDump.Set("LocalPetGrowthStage", 1);
                }
                break;
            case GrowthStage.Baby:
            case GrowthStage.Juvi:
                if (FoodMenu.activeSelf)
                {
                    CloseFoodMenu();
                    Juicer.Instance.PlayNaturalClip(CloseMenuClip);
                }
                else
                {
                    SetFoodMenu();
                    OpenFoodMenu();
                    Juicer.Instance.PlayPickSFX();
                }
                break;
            case GrowthStage.Pheonix:
                Juicer.Instance.PlayPickSFX();
                StartCoroutine(Rebirth());
                break;
        }
    }

    public void Feed(int type)
    {
        CloseFoodMenu();
        Juicer.Instance.PlayEatSFX();
        int foodLeft = DataDump.Get<int>(((FoodType)type).ToString()) - 1;
        DataDump.Set<int>(((FoodType)type).ToString(), foodLeft);
        string dataName = $"LocalPet{((FoodType)type).ToString()}Eaten";
        int petEaten = DataDump.Get<int>(dataName) + 1;
        DataDump.Set(dataName, petEaten);
        int fullness = DataDump.Get<int>("LocalPetFullness") + 1;
        DataDump.Set("LocalPetFullness", fullness);
        if (fullness > FullnessDeath)
        {
            // go to the die
            StartCoroutine(Die());
            return;
        }
        if (fullness > FullnessTolerance)
        {
            // play sick animation
        }

    }

    private System.DateTime gameEpoch = new System.DateTime(2020, 3, 18);
    private int CurrentTime => Mathf.FloorToInt((float)(System.DateTime.UtcNow - gameEpoch).TotalSeconds);

    public void TimeStep()
    {
        GrowthStage stage = (GrowthStage)DataDump.Get<int>("LocalPetGrowthStage");
        switch (stage)
        {
            case GrowthStage.Baby:
            case GrowthStage.Juvi:
                int fullness = DataDump.Get<int>("LocalPetFullness") - 1;
                if (fullness <= 0)
                {
                    // go to the die
                    StartCoroutine(Die());
                    return;
                }
                if (fullness < FullnessTolerance / 2)
                {
                    // go to the complain
                }
                DataDump.Set("LocalPetFullness", fullness);
                break;
        }
        
        if (stage == GrowthStage.Baby && DataDump.Get<int>("LocalPetNextGrowthTime") < CurrentTime)
        {
            // go to the grow
            StartCoroutine(Grow());
        }

        if (stage == GrowthStage.Dead && DataDump.Get<int>("LocalPetNextGrowthTime") < CurrentTime)
        {
            StartCoroutine(Sprout());
        }
    }

    public void SetFoodMenu()
    {
        for (int i = 0; i <= 2; ++i)
        {
            var foodType = (FoodType)i;
            int amount = DataDump.Get<int>(foodType.ToString());
            FoodButtons[i].SetActive(amount > 0);
        }
    }

    public void OpenFoodMenu()
    {
        FoodMenu.SetActive(true);
    }

    public void CloseFoodMenu()
    {
        FoodMenu.SetActive(false);
    }

    private IEnumerator Hatch()
    {
        Juicer.ShakeCamera(1);
        yield return new WaitForSeconds(2);
        DataDump.Set("LocalPetGrowthStage", 2);
        DataDump.Set("LocalPetFullness", InitialFullness);
        DataDump.Set("LocalPetBerriesEaten", 0);
        DataDump.Set("LocalPetCookiesEaten", 0);
        DataDump.Set("LocalPetFishEaten", 0);
        DataDump.Set("LocalPetHappiness", 0);
        DataDump.Set("LocalPetAnnoyance", 0);
        int nextGrowthTime = CurrentTime + (MinutesSpentAsChild * 60);
        DataDump.Set("LocalPetNextGrowthTime", nextGrowthTime);
        Juicer.ShakeCamera(3);
    }

    private IEnumerator Grow()
    {
        Juicer.ShakeCamera(1);
        yield return new WaitForSeconds(2);
        DataDump.Set("LocalPetGrowthStage", 3);
        Juicer.ShakeCamera(3);
    }

    private IEnumerator Die()
    {
        CloseFoodMenu();
        Juicer.ShakeCamera(1);
        yield return new WaitForSeconds(2);
        int nextGrowthTime = CurrentTime + (MinutesSpentDead * 60);
        DataDump.Set("LocalPetNextGrowthTime", nextGrowthTime);
        DataDump.Set("LocalPetGrowthStage", (int)GrowthStage.Dead);
        Juicer.ShakeCamera(3);
    }

    private IEnumerator Sprout()
    {
        Juicer.ShakeCamera(1);
        yield return new WaitForSeconds(2);
        DataDump.Set("LocalPetGrowthStage", 5);
        Juicer.ShakeCamera(3);
    }

    private IEnumerator Rebirth()
    {
        Juicer.ShakeCamera(1);
        yield return new WaitForSeconds(2);
        DataDump.Set("LocalPetGrowthStage", 0);
        DataDump.Set("LocalPetPokes", 0);
        Juicer.ShakeCamera(3);
    }

    /// <summary>
    /// just the top type
    /// </summary>
    /// <returns></returns>
    public PetType CalculatePetType()
    {
        int berries = DataDump.Get<int>("LocalPetBerriesEaten");
        int cookies = DataDump.Get<int>("LocalPetCookiesEaten");
        int fish = DataDump.Get<int>("LocalPetFishEaten");

        return new[] {
                (PetType.Cute, berries), (PetType.Chubby, cookies), (PetType.Tough, fish)
            }
            .OrderBy(stat => stat.Item2)
            .Last()
            .Item1;
    }

    public enum GrowthStage { Egg, Hatching, Baby, Juvi, Dead, Pheonix }
    public enum PetType { Cute, Chubby, Tough }
    public enum FoodType { Berries, Cookies, Fish }
}
