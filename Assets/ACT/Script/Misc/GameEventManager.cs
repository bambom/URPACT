using UnityEngine;
using System.Collections.Generic;

public class GameEventManager
{
    static GameEventManager sInstance = null;
    public static GameEventManager Instance { get { return sInstance ?? (sInstance = new GameEventManager()); } }

    List<GameEvent> mGameEvents = new List<GameEvent>();
    int mCursor = 0;

    public void Update()
    {
        if (mGameEvents.Count > 0)
        {
            if (mCursor < mGameEvents.Count)
                mGameEvents[mCursor++].Execute();

            if (mCursor >= mGameEvents.Count)
            {
                mGameEvents.Clear();
                mCursor = 0;
            }
        }
    }
	
	public void Reset()
	{
		mGameEvents.Clear();
        mCursor = 0;
	}
	
    public void EnQueue(GameEvent gameEvent, bool insert)
    {
		if (MainScript.Instance == null)
		{
			gameEvent.Execute();
			return;
		}
		
		if (gameEvent.CanIgnore && mGameEvents.Count > 10)
			return;
		
		if (insert)
		{
			if (mCursor > 0)
				mGameEvents[--mCursor] = gameEvent;
			else
				mGameEvents.Insert(0, gameEvent);
		}
		else
        	mGameEvents.Add(gameEvent);
	}
	
    public void EnQueue(GameEvent gameEvent)
    {
		EnQueue(gameEvent, false);
    }
}
