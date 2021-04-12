using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public class SoundCue
    {
        public string Name = "";
        public AudioClip[] Sounds = new AudioClip[1];
    };

    class AudioClipInfo
    {
        public string Name;
        AudioClip mClip;
        public AudioClipInfo(string name, AudioClip clip)
        {
            Name = name;
            mClip = clip;
        }

        public AudioClip Clip
        { 
            get { return mClip ?? (mClip = Resources.Load(Name) as AudioClip); } 
            set { mClip = value; } 
        }
    }

    SoundCue[] SoundList = new SoundCue[0];
    float FadeDistance = 20.0f;
    bool FadeLinear = false;
    float FadeVolume = 0.5f;

    const int RUNTIME_BASE = 0x10000;
    List<AudioClipInfo> mRuntimeClips = new List<AudioClipInfo>();
    Dictionary<string, int> mSoundIndexMap = new Dictionary<string, int>();
    AudioSource mAudioSource;
    float FadeVolumeScale;
    public AudioSource CurrentAudioSource
    {
        get
        {
			if (mAudioSource == null)
 				mAudioSource = GameObject.FindObjectOfType(typeof(AudioSource)) as AudioSource;
			/*
			if( mAudioSource == null )
			{
				AudioSource[] ASources = GameObject.FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
				for(int i = 0;i<ASources.Length;i++)
				{
					if( 0 == string.Compare(ASources[i].name,"Camera"))
					{
						mAudioSource = ASources[i];
						break;
					}
				}				
			}*/
            return mAudioSource;
        }
        set { mAudioSource = value; }
    }

    public void Init() { }

    // Use this for initialization
    public SoundManager()
    {
        FadeVolumeScale = FadeVolume / FadeDistance;

        // build the sound cue map.
        for (int i = 0; i < SoundList.Length; i++)
        {
            SoundCue soundCue = SoundList[i];
            if (soundCue != null)
                mSoundIndexMap[soundCue.Name] = i;
        }
        //enabled = false;
    }

    public int GetSoundIndex(string soundName)
    {
		if (string.IsNullOrEmpty(soundName)) return -1;
		
        int ret;
        if (mSoundIndexMap.TryGetValue(soundName, out ret))
            return ret;

        AudioClip audioClip = Resources.Load(soundName) as AudioClip;
        if (audioClip != null)
        {
            ret = RUNTIME_BASE + mRuntimeClips.Count;
            mSoundIndexMap[soundName] = ret;
            mRuntimeClips.Add(new AudioClipInfo(soundName, audioClip));
            return ret;
        }

        Debug.LogError("Fail to found sound: " + soundName);

        return -1;
    }

    public void UnloadRuntimeClips()
    {
        foreach (AudioClipInfo info in mRuntimeClips)
            info.Clip = null;
		mAudioSource = null;
    }

    AudioClip GetAudioClip(int soundIndex)
    {
        if (soundIndex < 0)
            return null;

        if (soundIndex < SoundList.Length)
        {
            SoundCue soundCue = SoundList[soundIndex];
            return soundCue.Sounds[UnityEngine.Random.Range(0, soundCue.Sounds.Length)];
        }
        else
        {
            soundIndex -= RUNTIME_BASE;
            if (soundIndex < 0 || soundIndex >= mRuntimeClips.Count)
                return null;

            return mRuntimeClips[soundIndex].Clip;
        }
    }

    public void Play3DSound(int soundIndex, Vector3 position)
    {
        Play3DSound(soundIndex, position, 1.0f);
    }

    public void Play3DSound(int soundIndex, Vector3 position, float volume)
    {
        Vector3 listenerPos = position;
        Unit localPlayer = UnitManager.Instance.LocalPlayer;
        if (localPlayer != null)
            listenerPos = localPlayer.Position;

        // clip sound too far.
        float distance = MathUtility.DistanceMax(listenerPos, position);
        if (distance > FadeDistance)
            return;

        AudioClip clip = GetAudioClip(soundIndex);
        if (clip == null)
            return;

        float volumeScale = (FadeDistance - distance) * FadeVolumeScale;
        if (!FadeLinear)
            volumeScale = volumeScale * volumeScale;

        CurrentAudioSource.PlayOneShot(clip, volumeScale * volume);
    }

    public void Play3DSound(string clipname, Vector3 position)
    {
        Play3DSound(GetSoundIndex(clipname), position);
    }

    public void PlaySound(string clipname)
    {
        AudioClip clip = GetAudioClip(GetSoundIndex(clipname));
        CurrentAudioSource.PlayOneShot(clip);
    }
	
	public void SetSound(float volume)
	{
		AudioSource ASource = null;
		AudioSource[] ASources = GameObject.FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
		for(int i = 0;i<ASources.Length;i++)
		{
			if( 0 == string.Compare(ASources[i].name,"Camera"))
			{
				ASource = ASources[i];
				break;
			}
		}	
		
		if( ASource != null )
		{
			ASource.volume = volume;
		}
	}
	
	public void SetMusic(float volume)
	{
		AudioSource ASource = null;
		AudioSource[] ASources = GameObject.FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
		for(int i = 0;i<ASources.Length;i++)
		{
			if( 0 != string.Compare(ASources[i].name,"Camera"))
			{
				ASource = ASources[i];
				ASource.volume = volume;
			}
		}
	}
}
