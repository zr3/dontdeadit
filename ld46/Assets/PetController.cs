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

    public int FullnessTolerance = 30;
    public int FullnessDeath => FullnessTolerance * 3;

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
        if (DataDump.Get<int>("LocalPetGrowthStage") == 0) {
            if (pokes > 5)
            {
                DataDump.Set("LocalPetGrowthStage", 1);
            }
            return;
        }
        if (FoodMenu.activeSelf)
        {
            CloseFoodMenu();
        } else
        {
            SetFoodMenu();
            OpenFoodMenu();
        }
    }

    public void Feed(int type)
    {
        CloseFoodMenu();
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
            DataDump.Set("LocalPetGrowthStage", (int)GrowthStage.Dead);
            return;
        }
        if (fullness > FullnessTolerance)
        {
            // play sick animation
        }

    }

    public void TimeStep()
    {
        int fullness = DataDump.Get<int>("LocalPetFullness") - 1;
        if (fullness <= 0)
        {
            // go to the die
            DataDump.Set("LocalPetGrowthStage", (int)GrowthStage.Dead);
            return;
        }
        if (fullness < FullnessTolerance / 2)
        {
            // go to the complain
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
        Juicer.ShakeCamera(3);
    }

    /// <summary>
    /// If all stats within 10 of each other, random type
    /// If some within 10, random type of the top 2
    /// else just the top type
    /// </summary>
    /// <returns></returns>
    public PetType CalculatePetType()
    {
        int berries = DataDump.Get<int>("LocalPetBerriesEaten");
        int cookies = DataDump.Get<int>("LocalPetCookiesEaten");
        int fish = DataDump.Get<int>("LocalPetFishEaten");

        if (Mathf.Abs(berries - cookies) < 10 && Mathf.Abs(berries - fish) < 10 && Mathf.Abs(cookies - fish) < 10)
        {
            return (PetType)Random.Range(0, 4);
        }
        var sortedStats = new[] {
                (PetType.Cute, berries), (PetType.Chubby, cookies), (PetType.Tough, fish)
            }
            .OrderBy(stat => stat.Item2);
        if (Mathf.Abs(berries - cookies) < 10 || Mathf.Abs(berries - fish) < 10 || Mathf.Abs(cookies - fish) < 10)
        {
            return sortedStats
                .Skip(1)
                .ToArray()[Random.Range(0, 2)]
                .Item1;
        }
        return sortedStats.Last().Item1;
    }

    public enum GrowthStage { Egg, Hatching, Baby, Juvi, Dead, Pheonix }
    public enum PetType { Cute, Chubby, Tough }
    public enum FoodType { Berries, Cookies, Fish }
}
