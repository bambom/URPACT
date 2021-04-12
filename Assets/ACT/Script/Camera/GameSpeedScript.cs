using UnityEngine;
using System.Collections;

public class GameSpeedScript : MonoBehaviour 
{
    static int GameSpeedCount = 0;

    int mEffectCount = 0;
	void GameSpeed(string parameter)
	{
		string[] strs = parameter.Split(',');
		float timescale = float.Parse(strs[0]);
		float duration = float.Parse(strs[1]);
		Time.timeScale = timescale;

        mEffectCount++;
        GameSpeedCount++;

		Invoke("GameSpeedReset",duration * timescale);
	}

    void OnDestroy()
    {
        if (mEffectCount > 0)
        {
            GameSpeedCount -= mEffectCount;
            mEffectCount = 0;
        }
    }
	
	void GameSpeedReset()
	{
        mEffectCount--;
        GameSpeedCount--;

        // reset to normal if there is no effects.
        if (GameSpeedCount == 0)
		    Time.timeScale = 1;
	}
}