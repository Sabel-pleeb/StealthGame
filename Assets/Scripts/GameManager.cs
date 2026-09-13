using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] public GameObject Player;
    public PlayerController _PC;

    private void Awake()
    {
        _PC = Player.GetComponent<PlayerController>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Bleh()
    {
        StartCoroutine(GameOver());
    }

    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1.8f);
    }
}
