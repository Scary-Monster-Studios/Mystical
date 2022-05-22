using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if AT_MASTERAUDIO_PRESET
using DarkTonic.MasterAudio;
#endif
namespace Atavism
{
    [System.Serializable]
    public class AtavismNpcAudioEntry
    {
        public string name = "";
        public AudioClip[] audioClips;
#if AT_MASTERAUDIO_PRESET
        public string[] audioClipsName;
#endif
        public float volume = 1.0f;
       // public float minPitch = 1.0f;
      //  public float maxPitch = 1.0f;
    }

    public class AtavismNpcAudioManager : MonoBehaviour
    {
        static AtavismNpcAudioManager instance;
        [SerializeField]
        string mixerAudioGroup = "SFX";
        [SerializeField]
        List<AtavismNpcAudioEntry> audio = new List<AtavismNpcAudioEntry>();
       

        // Start is called before the first frame update
        void Start()
        {
            if (instance != null)
                return;
            instance = this;
        }

        public static AtavismNpcAudioManager Instance
        {
            get
            {
                return instance;
            }
        }

        Dictionary<long, AudioSource> playingAudio = new Dictionary<long, AudioSource>();
        public void PlayAudio(string name, GameObject go, long oid)
        {
          //  Debug.LogError("AtavismNpcAudioManager: PlayAudio " + name + " " + go + " " + oid);
            if (playingAudio.ContainsKey(oid))
            {
                if (playingAudio[oid] != null)
                {
                    playingAudio[oid].Stop();
                    Destroy(playingAudio[oid].gameObject);
                }
                playingAudio.Remove(oid);
            }

            int index = AudioClipIndex(name);

            if (index > -1)
            {
#if AT_MASTERAUDIO_PRESET
               MasterAudio.StopAllSoundsOfTransform(go.transform);
               if (audio[index].audioClipsName.Length > 0)
               {
                   string audioClip = audio[index].audioClipsName[Random.Range(0, audio[index].audioClipsName.Length)];
                   if (!string.IsNullOrEmpty(audioClip))
                       MasterAudio.PlaySound3DAtTransform(audioClip, go.transform, audio[index].volume);
               }

#endif
                if (audio[index].audioClips.Length > 0)
                {
                    //float volume = audio[index].volume;
                    AudioClip audioClip = audio[index].audioClips[Random.Range(0, audio[index].audioClips.Length)];
                    GameObject goTemp = new GameObject("Audio");
                    goTemp.transform.position = go.transform.position;
                    AudioSource audios = goTemp.AddComponent<AudioSource>();
                    audios.outputAudioMixerGroup = AtavismSettings.Instance.masterMixer.FindMatchingGroups(mixerAudioGroup)[0];
                    audios.clip = audioClip;
                    audios.volume = audio[index].volume;
                    audios.spatialBlend = 1f;
                    audios.maxDistance = 30f;
                    audios.rolloffMode = AudioRolloffMode.Linear;
                    audios.Play();
                    Destroy(goTemp, audios.clip.length);
                    playingAudio.Add(oid, audios);
                }

            }
            else
            {
                Debug.LogWarning("Audi Clip Not Found "+name);
            }
        }


        private int AudioClipIndex(string name)
        {
            for (int i = 0; i < audio.Count; i++)
            {   
                if (audio[i].name == name)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}