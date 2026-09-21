using Unity.VisualScripting;
using UnityEngine;

public class GeneratorManager : MonoBehaviour
{
    [SerializeField] private string targetTag = "generator";
    public UIManager uimanager;

    private int repairedGenerators = 0;
    public int amount = 1;
    private int totalGenerators;
    public Generator LastRepairedGenerator {  get; private set; }
    public bool AnyGeneratorRepaired  { get; private set; }
    public GameObject exit;
    public FinalDoor finalDoor;
    // add transform here ?

    private void Start()
    {
        exit = GameObject.FindGameObjectWithTag("Exit");
        finalDoor = exit.GetComponent<FinalDoor>();
        totalGenerators = GameObject.FindGameObjectsWithTag(targetTag).Length;  // leaves openness for more or less gens in scene
        repairedGenerators = 0;
        AnyGeneratorRepaired = false;
    }

    public void GeneratorCompleted()
    {
        repairedGenerators++; 
        uimanager.lightProgressDoor();


        if (repairedGenerators >= totalGenerators)
        {
            finalDoor.allCollected = true;
        }
    }

    public void GeneratorRepaired(Generator generator)
    {
        AnyGeneratorRepaired = true;
        LastRepairedGenerator = generator;  
    }

    public bool JustRepaired()
    {
        if (AnyGeneratorRepaired) return true;
        else return false;
    }

    public void InvestigationComplete()
    {
        AnyGeneratorRepaired = false;
    }
}