using UnityEngine;

public class GeneratorManager : MonoBehaviour
{
    [SerializeField] private string targetTag = "generator";

    private int repairedGenerators = 0;
    private int totalGenerators;
    public Generator LastRepairedGenerator {  get; private set; }
    // add transform here ?

    private void Start()
    {
        totalGenerators = GameObject.FindGameObjectsWithTag(targetTag).Length;  // leaves openness for more or less gens in scene
        repairedGenerators = 0;

        Debug.Log($"Generators in level: {totalGenerators}");
    }

    public void GeneratorCompleted()
    {
        repairedGenerators++;

        Debug.Log($"Generators repaired: {repairedGenerators}/{totalGenerators}");

        if (repairedGenerators >= totalGenerators)
        {
            Debug.Log("All generators repaired ! Open the exit.");

            // GameManager.Instance.OpenExit(); // for when i make GM 
        }
    }

    public void GeneratorRepaired(Generator generator)
    {
        
        LastRepairedGenerator = generator;
        Debug.Log("Generator Repaired at: " + LastRepairedGenerator.transform);
    }
}