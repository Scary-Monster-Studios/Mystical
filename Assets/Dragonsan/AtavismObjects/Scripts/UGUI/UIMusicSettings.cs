using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace Atavism
{

    public class UIMusicSettings : MonoBehaviour
    {
        public AudioMixer masterMixer;
        public Slider masterVol;
        public Slider musicVol;
        public Slider sfxVol;
        public Slider uiVol;
        public Slider ambientVol;
        public Slider footstepsVol;
        //  private GameSettings gameSettings;
        private float startTimer = 0f;
        void OnEnable()
        {
            startTimer = Time.time + 0.3f;
            //gameSettings = GameObject.Find("Scripts").GetComponent<GameSettings>();

            masterVol.value = AtavismSettings.Instance.GetAudioSettings().masterLevel;
            musicVol.value = AtavismSettings.Instance.GetAudioSettings().musicLevel;
            sfxVol.value = AtavismSettings.Instance.GetAudioSettings().sfxLevel;
            uiVol.value = AtavismSettings.Instance.GetAudioSettings().uiLevel;
            ambientVol.value = AtavismSettings.Instance.GetAudioSettings().ambientLevel;
            footstepsVol.value = AtavismSettings.Instance.GetAudioSettings().footstepsLevel;
        }

        public void SetSfxLev(float sfxLev)
        {
            if (startTimer > Time.time)
                return;
            AtavismSettings.Instance.GetAudioSettings().sfxLevel = sfxLev;
            masterMixer.SetFloat("sfxVol", sfxLev);
        }
        public void SetMusicLev(float musicLev)
        {
            if (startTimer > Time.time)
                return;
            AtavismSettings.Instance.GetAudioSettings().musicLevel = musicLev;
            masterMixer.SetFloat("musicVol", musicLev);
        }
        public void SetMasterLev(float masterLev)
        {
            if (startTimer > Time.time)
                return;
            AtavismSettings.Instance.GetAudioSettings().masterLevel = masterLev;
            masterMixer.SetFloat("masterVol", masterLev);
        }
        public void SetUiLev(float uiLev)
        {
            if (startTimer > Time.time)
                return;
            AtavismSettings.Instance.GetAudioSettings().uiLevel = uiLev;
            masterMixer.SetFloat("uiVol", uiLev);
        }
        public void SetAmbientLev(float ambientLev)
        {
            if (startTimer > Time.time)
                return;
            AtavismSettings.Instance.GetAudioSettings().ambientLevel = ambientLev;
            masterMixer.SetFloat("AmbientVol", ambientLev);
        }
        public void SetFootstepsLev(float footstepsLev)
        {
            if (startTimer > Time.time)
                return;
            AtavismSettings.Instance.GetAudioSettings().footstepsLevel = footstepsLev;
            masterMixer.SetFloat("FootstepsVol", footstepsLev);
        }
    }
}