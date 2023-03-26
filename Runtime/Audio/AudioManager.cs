using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;

namespace klib
{
    public class AudioManager : Singleton<AudioManager>
    {

        private const string SE_VOL_PARAMNAME = "SEVolume", BGM_VOL_PARAMNAME = "BGMVolume", VOICE_VOL_PARAMNAME = "VOICEVolume";

        [SerializeField] private AudioMixer _mixer = null;
        [SerializeField] private AudioData _2dAudioDataPrefab = null, _3dAudioDataPrefab = null;
        [SerializeField] private AudioClip[] _bgmClips, _seClips, _voiceClips;
        [SerializeField] private int _maxPoolCount = 10;

        public override bool IsDontDestroyOnLoad => true;

        private List<AudioData> _audioData = new List<AudioData>();
        private Coroutine _checkAudioDataPool;

        protected override void AfterAwake()
        {
            base.AfterAwake();
            _seClips = Resources.LoadAll<AudioClip>("SE");
            _bgmClips = Resources.LoadAll<AudioClip>("BGM");
            _voiceClips = Resources.LoadAll<AudioClip>("VOICE");

            if (_maxPoolCount <= 0)
            {
                _maxPoolCount = 1;
            }
        }

        protected override void AfterDestroy()
        {
            base.AfterDestroy();
        }

        private void OnDisable()
        {
            StopCheckAudioDataPool();
            Clear();
        }

        public float SEVolume
        {
            get
            {
                _mixer.GetFloat(SE_VOL_PARAMNAME, out float vol);
                return vol;
            }
            set
            {
                _mixer.SetFloat(SE_VOL_PARAMNAME, value);
            }
        }

        public float BGMVolume
        {
            get
            {
                _mixer.GetFloat(BGM_VOL_PARAMNAME, out float vol);
                return vol;
            }
            set
            {
                _mixer.SetFloat(BGM_VOL_PARAMNAME, value);
            }
        }

        public float VOICEVolume
        {
            get
            {
                _mixer.GetFloat(VOICE_VOL_PARAMNAME, out float vol);
                return vol;
            }
            set
            {
                _mixer.SetFloat(VOICE_VOL_PARAMNAME, value);
            }
        }

        public void PlaySE(string clipName, bool loop = false, float pitch = 1, float duration = 0f, float maxDistance = 500, float vol = 1)
        {
            var clip = _seClips.Where(x => x.name == clipName).FirstOrDefault();

            if (clip == null)
            {
                Debug.LogError("clip is null : clipName = " + clipName);
                return;
            }

            PlayAudioData(clip, loop, pitch, duration, maxDistance, vol, _mixer.FindMatchingGroups("SE")[0]);
        }

        public void PlayBGM(string clipName, bool withFade = false, bool loop = true)
        {
            if (IsPlaying(clipName)) { return; }

            var clip = _bgmClips.Where(x => x.name == clipName).FirstOrDefault();

            if (clip == null)
            {
                Debug.LogError("clip is null : clipName = " + clipName);
                return;
            }

            var audioData = GetAudioData();
            audioData.Setup(_mixer.FindMatchingGroups("BGM")[0], loop);
            audioData.Play(clip, withFade);
        }

        public void PlayVoice(string clipName, bool loop = false, float pitch = 1, float duration = 0f, float maxDistance = 500, float vol = 1)
        {
            var clip = _voiceClips.Where(x => x.name == clipName).FirstOrDefault();

            if (clip == null)
            {
                Debug.LogError("clip is null : clipName = " + clipName);
                return;
            }

            PlayAudioData(clip, loop, pitch, duration, maxDistance, vol, _mixer.FindMatchingGroups("VOICE")[0]);
        }

        private void PlayAudioData(AudioClip clip, bool loop, float pitch, float duration, float maxDistance, float vol, AudioMixerGroup mixer)
        {
            var audioData = GetAudioData();
            audioData.Setup(mixer, loop);
            audioData.Play(clip, pitch, duration, maxDistance, vol);
        }

        public void StopBGM(string clipName, bool withFadeOut = true)
        {
            var audioData = _audioData.Where(x => x.ClipName == clipName && x.IsPlaying()).FirstOrDefault();

            if (audioData == null || !audioData.IsPlaying())
            {
                return;
            }

            if (withFadeOut)
            {
                audioData.FadeOut(0.4f);
            }
            else
            {
                audioData.Stop();
            }
        }

        public void StopSE(string clipName, bool withFadeOut = true, float fadeOutTime = 0.4f)
        {
            var audioData = _audioData.Where(x => x.ClipName == clipName && x.IsPlaying()).FirstOrDefault();

            if (audioData == null || !audioData.IsPlaying())
            {
                return;
            }

            if (withFadeOut)
            {
                audioData.FadeOut(fadeOutTime);
            }
            else
            {
                audioData.Stop();
            }
        }

        public void Clear()
        {
            foreach (var data in _audioData)
            {
                if (data == null)
                {
                    continue;
                }

                Destroy(data.gameObject);
            }

            _audioData.Clear();
        }

        public void StopAll(bool withFadeOut = true)
        {
            foreach (var data in _audioData)
            {
                if (data == null)
                {
                    continue;
                }

                if (withFadeOut)
                {
                    data.FadeOut(0.4f);
                }
                else
                {
                    data.Stop();
                }
            }
        }

        private AudioData GetAudioData()
        {
            if (_audioData.Count == 0)
            {
                var audioData = Instantiate(_2dAudioDataPrefab);
                audioData.transform.SetParent(transform);
                _audioData.Add(audioData);
                return audioData;
            }

            foreach (var audioData in _audioData)
            {
                if (!audioData.IsPlaying() && !audioData.Is3DAudioData)
                {
                    return audioData;
                }
            }

            var data = Instantiate(_2dAudioDataPrefab);
            data.transform.SetParent(transform);
            _audioData.Add(data);
            StartCheckAudioDataPool();
            return data;
        }

        private AudioData Get3DAudioData()
        {
            if (_audioData.Count == 0)
            {
                var audioData = Instantiate(_3dAudioDataPrefab);
                _audioData.Add(audioData);
                return audioData;
            }

            foreach (var audioData in _audioData)
            {
                if (!audioData.IsPlaying() && audioData.Is3DAudioData)
                {
                    return audioData;
                }
            }

            var data = Instantiate(_3dAudioDataPrefab);
            _audioData.Add(data);
            StartCheckAudioDataPool();
            return data;
        }

        private void StartCheckAudioDataPool()
        {
            if (_checkAudioDataPool != null)
            {
                return;
            }

            _checkAudioDataPool = StartCoroutine(CheckAudioDataPool());
        }

        private void StopCheckAudioDataPool()
        {
            if (_checkAudioDataPool == null)
            {
                return;
            }

            StopCoroutine(_checkAudioDataPool);
            _checkAudioDataPool = null;
        }

        private IEnumerator CheckAudioDataPool()
        {
            while (_audioData.Count > _maxPoolCount)
            {
                if (_audioData.All(x => x.IsPlaying()))
                {
                    yield return null;
                    continue;
                }

                var audioData = _audioData.Where(x => !x.IsPlaying()).FirstOrDefault();
                _audioData.Remove(audioData);
                Destroy(audioData.gameObject);
                yield return null;
            }

            _checkAudioDataPool = null;
        }

        public bool IsPlaying(string clipName)
        {
            var audioData = _audioData.Where(x => x.ClipName == clipName).FirstOrDefault();

            if (audioData == null)
            {
                return false;
            }

            return audioData.IsPlaying();
        }

    }
}
