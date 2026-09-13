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

     //   Debug.Log($"Generators in level: {totalGenerators}");
    }

    public void GeneratorCompleted()
    {
        repairedGenerators++; // add something to ui manager here too 
                              // AnyGeneratorRepaired = true;
        uimanager.lightProgressDoor();

        uimanager.SliderFill(amount);
     //   Debug.Log($"Generators repaired: {repairedGenerators}/{totalGenerators}");

        if (repairedGenerators >= totalGenerators)
        {
         //   Debug.Log("All generators repaired ! Open the exit.");
            finalDoor.allCollected = true;
            // GameManager.Instance.OpenExit(); // for when i make GM 
        }
    }

    public void GeneratorRepaired(Generator generator)
    {
        AnyGeneratorRepaired = true;
        LastRepairedGenerator = generator;  // feed last repaired generator position to enemy bhtree if needed ? 
     //   Debug.Log("Generator Repaired at: " + LastRepairedGenerator.transform);
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