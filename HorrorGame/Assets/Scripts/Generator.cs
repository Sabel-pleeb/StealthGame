using UnityEngine;

public class Generator : MonoBehaviour
{
    public bool repaired = false;
    private GeneratorManager generatorManager;

    private void Start()
    {
        generatorManager = FindFirstObjectByType<GeneratorManager>();
    }

    public void Repair()  // call this in player controller after interacting with gen
    {
        if (!repaired)
        {
            repaired = true;
            generatorManager.GeneratorCompleted();
            generatorManager.GeneratorRepaired(this);
        }
        else return;

     //   if (repaired)
     //       return;

      //  repaired = true;
      //  Debug.Log("Gen Reapired !");

        // Play animation or action stuff

    }
}