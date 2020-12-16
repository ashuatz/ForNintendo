using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Util;

#if UNITY_EDITOR
using UnityEditor.Events;
using UnityEditor;
#endif

namespace Billiards
{

    public class SoundManager : MonoSingleton<SoundManager>
    {

        public enum AudioClipType
        {
            None,
            SFX_A,
            SFX_B,
            SFX_C,
            SFX_D,
            SFX_E,

            BGM_A,
            BGM_B,

            Intro,
        }

        [Serializable]
        private class AudioDatum
        {
            public AudioClipType type;
            public AudioClip clip;

            [Range(0, 1)]
            public float volume = 0.5f;
        }

        [SerializeField]
        private AudioDatum BGM_A_Datum;
        private AudioSource BGM_A_Source;

        [SerializeField]
        private AudioDatum BGM_B_Datum;
        private AudioSource BGM_B_Source;

        [Space(15)]
        [SerializeField]
        private AudioDatum[] AudioData;

        [Space(15)]
        [SerializeField]
        private AnimationCurve FadeInCurve;
        [SerializeField]
        private AnimationCurve FadeOutCurve;

        private Dictionary<AudioClipType, AudioDatum> AudioDictionary = new Dictionary<AudioClipType, AudioDatum>();

        private List<AudioSource> AudioSources = new List<AudioSource>();

        private List<AudioSource> CachedAudioSource = new List<AudioSource>();

        private float defaultBgmVolume;
        public bool isInitialized { get; private set; }

        private float CachedFXVolume = float.MinValue;
        private float SavedFXVolume
        {
            get => PlayerPrefs.GetFloat("FXVolume", 1f);
            set => PlayerPrefs.SetFloat("FXVolume", value);
        }

        private float CachedBGMVolume = float.MinValue;
        private float SavedBGMVolume
        {
            get => PlayerPrefs.GetFloat("BGMVolume", 1f);
            set => PlayerPrefs.SetFloat("BGMVolume", value);
        }

        public float FXVolume
        {
            get
            {
                if (CachedFXVolume == float.MinValue)
                {
                    CachedFXVolume = SavedFXVolume;
                }

                return CachedFXVolume;
            }
            set
            {
                if (SavedFXVolume != value)
                {
                    SavedFXVolume = value;
                    CachedFXVolume = value;

                    UpdateVolume();
                }
            }
        }


        public float BGMVolume
        {
            get
            {
                if (CachedBGMVolume == float.MinValue)
                {
                    CachedBGMVolume = SavedBGMVolume;
                }

                return CachedBGMVolume;
            }
            set
            {
                if (SavedBGMVolume != value)
                {
                    SavedBGMVolume = value;
                    CachedBGMVolume = value;

                    UpdateVolume();
                }
            }
        }

        private int SceneChangedCount;

        protected override void Awake()
        {
            if ((object)_instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            Initialze();
            base.Awake();
        }

        private void Initialze()
        {
            for (int i = 0; i < 10; ++i)
            {
                var component = gameObject.AddComponent<AudioSource>();

                InitializeAudioSource(component);
                AudioSources.Add(component);
            }

            BGM_A_Source = gameObject.AddComponent<AudioSource>();
            BGM_A_Source.clip = BGM_A_Datum.clip;
            BGM_A_Source.volume = BGM_A_Datum.volume * BGMVolume;
            BGM_A_Source.loop = true;
            BGM_A_Source.playOnAwake = false;
            BGM_A_Source.Stop();

            BGM_B_Source = gameObject.AddComponent<AudioSource>();
            BGM_B_Source.clip = BGM_B_Datum.clip;
            BGM_B_Source.volume = BGM_B_Datum.volume * BGMVolume;
            BGM_B_Source.loop = true;
            BGM_B_Source.playOnAwake = false;
            BGM_B_Source.Stop();

            for (int i = 0; i < AudioData.Length; ++i)
            {
                AudioDictionary[AudioData[i].type] = AudioData[i];
            }

            isInitialized = true;

            defaultBgmVolume = BGM_A_Datum.volume;
        }

        private void SceneManager_activeSceneChanged(Scene prev, Scene next)
        {
            if (SceneChangedCount++ == 0)
                return;

            StopAllCoroutines();
            if (next.buildIndex == 2)
            {
                //game
                StartCoroutine(FadeBGM(BGM_A_Source, BGM_B_Source, 2f));
            }
            else
            {
                StartCoroutine(FadeBGM(BGM_B_Source, BGM_A_Source, 2f));
            }
        }

        private void FadeBGMInternal(AudioClipType type)
        {

            if (type == AudioClipType.BGM_A)
            {
                StartCoroutine(FadeBGM(BGM_B_Source, BGM_A_Source, 2f));
            }

            if (type == AudioClipType.BGM_B)
            {
                StartCoroutine(FadeBGM(BGM_A_Source, BGM_B_Source, 2f));
            }
        }

        private IEnumerator FadeBGM(AudioSource prev, AudioSource next, float runtime)
        {
            float t = 0;

            next.volume = 0;

            next.Play();
            while (t < runtime)
            {
                t += Time.deltaTime;

                prev.volume = defaultBgmVolume * FadeOutCurve.Evaluate(t / runtime) * BGMVolume;
                next.volume = defaultBgmVolume * FadeInCurve.Evaluate(t / runtime) * BGMVolume;

                yield return null;
            }
            prev.Stop();

            prev.volume = defaultBgmVolume * FadeOutCurve.Evaluate(1) * BGMVolume;
            next.volume = defaultBgmVolume * FadeInCurve.Evaluate(1) * BGMVolume;

        }

        private void InitializeAudioSource(AudioSource source)
        {
            source.playOnAwake = false;
        }

        private void UpdateVolume()
        {
            BGM_A_Source.volume = BGM_A_Datum.volume * BGMVolume;
            BGM_B_Source.volume = BGM_B_Datum.volume * BGMVolume;

            for (int i = 0; i < AudioSources.Count; ++i)
            {
                if (AudioSources[i].isPlaying)
                {
                    var current = AudioDictionary.Values.First((datum) => datum.clip == AudioSources[i].clip);
                    AudioSources[i].volume = current.volume * FXVolume;
                }
            }
        }

        private void PlaySoundInternal(AudioClipType type)
        {
            if (type == AudioClipType.BGM_A)
            {
                BGM_A_Source.Play();
                return;
            }

            if (type == AudioClipType.BGM_B)
            {
                BGM_B_Source.Play();
                return;
            }

            for (int i = 0; i < AudioSources.Count; ++i)
            {
                if (!AudioSources[i].isPlaying)
                {
                    AudioSources[i].clip = AudioDictionary[type].clip;
                    AudioSources[i].volume = AudioDictionary[type].volume * FXVolume;
                    AudioSources[i].loop = false;
                    AudioSources[i].Play();
                    break;
                }
            }
        }

        private void StopSoundInternal(AudioClipType type)
        {
            if (type == AudioClipType.BGM_A)
            {
                BGM_A_Source.Stop();
                return;
            }

            if (type == AudioClipType.BGM_B)
            {
                BGM_B_Source.Stop();
                return;
            }

            for (int i = 0; i < AudioSources.Count; ++i)
            {
                if (AudioSources[i].isPlaying && AudioSources[i].clip == AudioDictionary[type].clip)
                {
                    AudioSources[i].Stop();
                }
            }
        }

        private void PlayingSoundModifyInternal(AudioClipType type, Action<AudioSource> target)
        {
            if (type == AudioClipType.BGM_A)
            {
                target?.Invoke(BGM_A_Source);
                return;
            }

            if (type == AudioClipType.BGM_B)
            {
                target?.Invoke(BGM_B_Source);
                return;
            }

            for (int i = 0; i < AudioSources.Count; ++i)
            {
                if (AudioSources[i].isPlaying && AudioSources[i].clip == AudioDictionary[type].clip)
                {
                    target?.Invoke(AudioSources[i]);
                }
            }
        }

        private bool isPlayingInternal(AudioClipType type)
        {
            if (type == AudioClipType.BGM_A)
            {
                return BGM_A_Source.isPlaying;
            }

            if (type == AudioClipType.BGM_B)
            {
                return BGM_B_Source.isPlaying;
            }

            for (int i = 0; i < AudioSources.Count; ++i)
            {
                if (AudioSources[i].isPlaying && AudioSources[i].clip == AudioDictionary[type].clip)
                {
                    return AudioSources[i].isPlaying;
                }
            }

            return false;
        }

        private void PauseSoundInternal(bool isPause)
        {
            if (isPause)
            {
                for (int i = 0; i < AudioSources.Count; ++i)
                {
                    if (AudioSources[i].isPlaying)
                    {
                        CachedAudioSource.Add(AudioSources[i]);
                        AudioSources[i].Pause();
                    }
                }
            }
            else
            {
                var e = CachedAudioSource.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Play();
                }
                CachedAudioSource.Clear();
            }
        }


        public static bool isPlaying(AudioClipType type)
        {
            if (!Instance.isInitialized)
                return false;

            return Instance.isPlayingInternal(type);
        }

        public static void FadeBGM(AudioClipType type)
        {
            if (!Instance.isInitialized)
                return;

            Instance.FadeBGMInternal(type);
        }

        public static void PlaySound(AudioClipType type)
        {
            if (!Instance.isInitialized)
                return;

            Instance.PlaySoundInternal(type);
        }

        public static void StopSound(AudioClipType type)
        {
            if (!Instance.isInitialized)
                return;

            Instance.StopSoundInternal(type);
        }

        public static void PlayingSoundModify(AudioClipType type, Action<AudioSource> target)
        {
            Instance.PlayingSoundModifyInternal(type, target);
        }

        public static float GetDefaultVolume(AudioClipType type)
        {
            return Instance.AudioDictionary[type].volume;
        }

        public static void Pause(bool isPause)
        {
            if (!Instance.isInitialized)
                return;

            Instance.PauseSoundInternal(isPause);
        }
    }

}