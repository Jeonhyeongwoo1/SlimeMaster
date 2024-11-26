using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SlimeMaster.Enum;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SlimeMaster.Manager
{
    public class AudioManager
    {
        private List<AudioSource> _audioSourceList = new((int)Sound.Max);
        private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
        private GameObject _audio;
        
        public void Initialize()
        {
            if (_audio != null)
            {
                return;
            }
            
            _audio = new GameObject("@AudioRoot");
            Object.DontDestroyOnLoad(_audio);

            int length = System.Enum.GetNames(typeof(Sound)).Length;
            for (int i = 0; i < length; i++)
            {
                Sound sound = (Sound)i;
                GameObject newAudio = new GameObject(sound.ToString());
                AudioSource audioSource = newAudio.AddComponent<AudioSource>();
                _audioSourceList.Add(audioSource);
                newAudio.transform.SetParent(_audio.transform);

                if (sound == Sound.Bgm)
                {
                    audioSource.loop = true;
                }
            }
        }

        public async void Play(Sound sound, string key, float pitch = 1.0f)
        {
            switch (sound)
            {
                case Sound.Bgm:
                    if (!GameManager.I.IsOnBGM)
                    {
                        return;
                    }
                    
                    AudioSource audioSource = _audioSourceList[(int)Sound.Bgm];
                    AudioClip audioClip = LoadAudioClip(key);
                    await ChangeBGMSound(audioSource, audioClip, 0.5f);
                    break;
                case Sound.Effect:
                    if (!GameManager.I.IsOnSfx)
                    {
                        return;
                    }
                    
                    audioSource = _audioSourceList[(int)Sound.Effect];
                    audioSource.Stop();
                    audioClip = LoadAudioClip(key);
                    audioSource.clip = audioClip;
                    audioSource.PlayOneShot(audioClip);
                    break;
                case Sound.Max:
                    break;
            }
        }

        public void Stop(Sound sound)
        {
            switch (sound)
            {
                case Sound.Bgm:
                    AudioSource audioSource = _audioSourceList[(int)Sound.Bgm];
                    audioSource.Stop();
                    break;
                case Sound.Effect:
                    audioSource = _audioSourceList[(int)Sound.Effect];
                    audioSource.Stop();
                    break;
                case Sound.Max:
                    break;
            }
        }
        
        private async UniTask ChangeBGMSound(AudioSource audioSource, AudioClip audioClip, float fadeDuration)
        {
            audioSource.DOFade(0, fadeDuration);
            await UniTask.WaitForSeconds(fadeDuration);
            
            audioSource.Stop();

            audioSource.clip = audioClip;
            audioSource.Play();
            audioSource.DOFade(1, fadeDuration);
            await UniTask.WaitForSeconds(fadeDuration);
        }
        
        private AudioClip LoadAudioClip(string key)
        {
            AudioClip audioClip = null;
            if (_audioClips.TryGetValue(key, out audioClip))
            {
                return audioClip;
            }

            audioClip = GameManager.I.Resource.Load<AudioClip>(key);

            if (!_audioClips.ContainsKey(key))
                _audioClips.Add(key, audioClip);

            return audioClip;
        }
    }
}