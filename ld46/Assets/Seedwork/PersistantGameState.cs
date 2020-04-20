using UnityEngine;

public class PersistantGameState : ScriptableObject
{
    public string Time { get; set; }
    public bool BerriesDiscovered { get; set; }
    public int Berries { get; set; }
    public bool CookiesDiscovered { get; set; }
    public int Cookies { get; set; }
    public bool FishDiscovered { get; set; }
    public int Fish { get; set; }
    public int LocalPetGrowthStage { get; set; }
    public int LocalPetPokes { get; set; }
    public int LocalPetFullness { get; set; }
    public int LocalPetHappiness { get; set; }
    public int LocalPetAnnoyance { get; set; }
    public int LocalPetBerriesEaten { get; set; }
    public int LocalPetCookiesEaten { get; set; }
    public int LocalPetFishEaten { get; set; }
    public int LocalPetNextGrowthTime { get; set; }
}
