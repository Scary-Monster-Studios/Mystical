using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if AT_MASTERAUDIO_PRESET
using DarkTonic;
using DarkTonic.MasterAudio;
#endif

namespace Atavism
{
    public class ArenaObject : MonoBehaviour
    {
       public float moveDistanceY = 4f;
       public float movetime = 3f;
       public float timeStep = 0.1f;
       public List<AudioClip> soundClip = new List<AudioClip>();
        public bool soundClip3D = true;
        public float soundVolume = 1.0f;
#if AT_MASTERAUDIO_PRESET
        public List<string> soundNames = new List<string>();
#endif
        float destinationPosition = 0f;
        float moveStep = 0f;
        // Use this for initialization
        void Start()
        {
            NetworkAPI.RegisterExtensionMessageHandler("arena_started", HandleArenaStart);
        }

        void OnDestroy()
        {
            NetworkAPI.RemoveExtensionMessageHandler("arena_started", HandleArenaStart);
        }

        private void HandleArenaStart(Dictionary<string, object> props)
        {
            destinationPosition = transform.localPosition.y + moveDistanceY;
            moveStep = moveDistanceY / (movetime / timeStep);
            StartCoroutine(MoveObject());
        }

        IEnumerator MoveObject()
        {
            PlaySound();
            WaitForSeconds delay = new WaitForSeconds(timeStep);
            while (transform.localPosition.y < destinationPosition)
            {
                transform.localPosition = transform.localPosition + Vector3.up * moveStep;
                yield return delay;
            }
        }

        void PlaySound()
        {
                if ((soundClip != null && soundClip.Count > 0))
                {
                    AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                    if (soundClip != null && soundClip.Count > 0)
                    {
                        audioSource.clip = soundClip[new System.Random().Next(soundClip.Count - 1)];
                    }
                    if (soundClip3D)
                        audioSource.spatialBlend = 1.0f;
                audioSource.outputAudioMixerGroup = AtavismSettings.Instance.masterMixer.FindMatchingGroups("SFX")[0];
                audioSource.volume = SoundSystem.SoundEffectVolume * soundVolume;
             
                audioSource.Play();
                }
#if AT_MASTERAUDIO_PRESET
                string soundName = "";
               if (soundNames != null && soundNames.Count > 0)
                {
                    soundName = soundNames[new System.Random().Next(soundNames.Count - 1)];
                }
                if (!string.IsNullOrEmpty(soundName))
                    MasterAudio.PlaySound3DAtTransform(soundName, transform, soundVolume);
            
#endif
            

        }


    }
}