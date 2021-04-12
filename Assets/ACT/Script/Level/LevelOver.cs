using UnityEngine;
using System.Collections;

public class LevelOver : MonoBehaviour 
{
    public GameLevel Level;

    void LevelFinish()
    {
        Level.LevelFinish();
	}
	
	void OnTriggerEnter(Collider collider)
	{
        Invoke("LevelFinish", 1.0f);
	}
}
