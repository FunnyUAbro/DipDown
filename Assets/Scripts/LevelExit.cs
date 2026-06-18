using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] string levelName;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void ChangeLevel()
    {
        SceneManager.LoadScene(levelName);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ChangeLevel();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
    }
}
