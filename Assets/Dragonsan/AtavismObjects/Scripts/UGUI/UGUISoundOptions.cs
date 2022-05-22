using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Atavism
{
    public class UGUISoundOptions : MonoBehaviour
    {

        public UGUIPanelTitleBar titleBar;
        public Slider mobSoundSlider;
        public Slider musicSlider;
        public Text mobSoundLevelText;
        public Text musicLevelText;

        // Use this for initialization
        void Start()
        {
            Hide();
            if (titleBar != null)
                titleBar.SetOnPanelClose(Hide);

            SoundSystem.LoadSoundSettings();
            mobSoundSlider.value = SoundSystem.SoundEffectVolume;
            musicSlider.value = SoundSystem.MusicVolume;
            mobSoundLevelText.text = (SoundSystem.SoundEffectVolume * 100) + "%";
            musicLevelText.text = (SoundSystem.MusicVolume * 100) + "%";
        }

        public void Show()
        {
            GetComponent<CanvasGroup>().alpha = 1.0f;
            GetComponent<CanvasGroup>().interactable = true;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        public void Hide()
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().interactable = false;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        public void SetMobSoundVolume(float volume)
        {
            SoundSystem.SoundEffectVolume = volume;
            mobSoundLevelText.text = (SoundSystem.SoundEffectVolume * 100) + "%";
            SoundSystem.SaveSoundSettings();
        }

        public void SetMusicVolume(float volume)
        {
            SoundSystem.MusicVolume = volume;
            musicLevelText.text = (SoundSystem.MusicVolume * 100) + "%";
            SoundSystem.SaveSoundSettings();
        }

        public void Toggle()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}