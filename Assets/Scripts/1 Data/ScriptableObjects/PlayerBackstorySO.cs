using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBackstory", menuName = "DearHiringManager/PlayerBackstory")]
public class PlayerBackstorySO : ScriptableObject
{
    public int id;
    public string title;
    [TextArea(3, 5)]
    public string description;
    public double startingCash;
    public float startingStress;
    public float startingManualLuck;
    public float startingHospitalityLuck;
    public float startingTechLuck;
    public float startingCorpLuck;
}
