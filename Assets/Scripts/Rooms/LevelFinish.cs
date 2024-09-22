using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelFinish : MonoBehaviour
{
    [SerializeField] string levelName;

    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")){ 
            Debug.Log("levelName: " + levelName);
            LoadingManager.instance.LoadScene(levelName);
        }
    }
}
