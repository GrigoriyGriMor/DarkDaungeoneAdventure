using Game.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoundSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManagerAllControll : AbstractManager
    {
        [SerializeField] private int _sourceCurrent = 5;
        [SerializeField] private List<AudioSource> _defaultSource = new List<AudioSource>();
        [SerializeField] private List<AudioSource> _additionalSource = new List<AudioSource>();


        [SerializeField] private AudioSource _backgroundSource;

        ///[SerializeField] private List<AudioSource> _backgroundSource = new List<AudioSource>();
        [SerializeField, Range(0, 1)] private float _backgroundVolume = 0.7f;

        [SerializeField] private ButtonUISpriteChanger _muteSoundButton;

        [Header("Sound Priority Block")] [Range(0, 255)] [SerializeField]
        private byte priority_background = 25;

        [Range(0, 255)] [SerializeField] private byte priority_UI = 50;
        [Range(0, 255)] [SerializeField] private byte priority_system = 75;
        [Range(0, 255)] [SerializeField] private byte priority_steps = 100;
        [Range(0, 255)] [SerializeField] private byte priority_auto = 125;
        [Range(0, 255)] [SerializeField] private byte priority_mehanicanism = 150;
        [Range(0, 255)] [SerializeField] private byte priority_talking = 175;
        [Range(0, 255)] [SerializeField] private byte priority_fire = 200;
        [Range(0, 255)] [SerializeField] private byte priority_damage = 225;

        private bool isSoundEnabled = true;

        private void Awake()
        {
            //Initialize
            _backgroundSource = GetComponent<AudioSource>(); //создаем отдельный сорс для бэкграунда
            _backgroundSource.priority = priority_background;
            _backgroundSource.loop = true;

            for (int i = 0; i < _sourceCurrent; i++)
                _defaultSource.Add(gameObject.AddComponent<AudioSource>());
        }

        public void SoundButtonInit(ButtonUISpriteChanger button)
        {
            if (_muteSoundButton != null)
            {
                Debug.LogError("?????? ???????????!");
                return;
            }

            _muteSoundButton = button;

            button.ChangeSprite(isSoundEnabled);
            button.GetComponent<Button>().onClick.AddListener(OnSoundSwitch);
        }

        public void BackgroundClipPlay(AudioClip clip)
        {
            _backgroundSource.clip = clip;
            _backgroundSource.Play();
        }
#if FALSE
        private IEnumerator SwapBackgroundSoundVolume()
        {
            while (_backgroundSource[_backgroundSource.Count - 1].volume < (_backgroundVolume - _backgroundVolume * 0.05f))
            {
                for (int i = 0; i < _backgroundSource.Count; i++)
                {
                    if (i == _backgroundSource.Count - 1)
                        _backgroundSource[i].volume =
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         Mathf.Lerp(_backgroundSource[i].volume, _backgroundVolume, Time.deltaTime);
                    else
                        _backgroundSource[i].volume = Mathf.Lerp(_backgroundSource[i].volume, 0, Time.deltaTime);
                }
                yield return new WaitForFixedUpdate();
            }

            _backgroundSource[_backgroundSource.Count - 1].volume = _backgroundVolume;

            while (_backgroundSource.Count > 1)
            {
                Destroy(_backgroundSource[0]);
                _backgroundSource.RemoveAt(0);
            }
        }
#endif
        public void LeaveScene()
        {
            _backgroundSource.clip = null;

            if (_muteSoundButton != null) _muteSoundButton.GetComponent<Button>().onClick.RemoveAllListeners();
            _muteSoundButton = null;


            for (int i = 0; i < _additionalSource.Count; ++i)
            {
                if (_additionalSource[i].loop)
                    Destroy(_additionalSource[i]);
            }

            _additionalSource.Clear();
        }

        private void SetPriority(AudioSource sr, SoundPriority pr)
        {
            switch (pr)
            {
                case SoundPriority.Background:
                    sr.priority = priority_background;
                    break;
                case SoundPriority.UI:
                    sr.priority = priority_UI;
                    break;
                case SoundPriority.System:
                    sr.priority = priority_system;
                    break;
                case SoundPriority.Steps:
                    sr.priority = priority_steps;
                    break;
                case SoundPriority.Auto:
                    sr.priority = priority_auto;
                    break;
                case SoundPriority.Mehanicanism:
                    sr.priority = priority_mehanicanism;
                    break;
                case SoundPriority.Talking:
                    sr.priority = priority_talking;
                    break;
                case SoundPriority.Fire:
                    sr.priority = priority_fire;
                    break;
                case SoundPriority.Damage:
                    sr.priority = priority_damage;
                    break;
            }
        }

        public void ClipPlay(AudioClip clip, Vector3 pos, SoundPriority pr = SoundPriority.Background)
        {
            AudioSource.PlayClipAtPoint(clip, pos);
        }
        
        public void ClipPlay(AudioClip clip, SoundPriority pr = SoundPriority.Background)
        {
            AudioSource sr = GetFree();
            sr.PlayOneShot(clip);

            SetPriority(sr, pr);
        }

        public void ClipPlay(AudioClip clip, float pitch, SoundPriority pr = SoundPriority.Background)
        {
            AudioSource sr = GetFree();
            sr.pitch = pitch;
            sr.PlayOneShot(clip);
            SetPriority(sr, pr);
        }

        //для объектов со своим источником звука
        public void ClipPlay(AudioClip clip, AudioSource _source, SoundPriority pr = SoundPriority.Background)
        {
            SetPriority(_source, pr);

            if (_additionalSource.Contains(_source))
            {
                _source.PlayOneShot(clip);
                return;
            }

            _additionalSource.Add(_source);
            if (!isSoundEnabled)
                _source.volume = 0;
            else
                _source.PlayOneShot(clip);
        }

        public void ClipLoopAndPlay(AudioClip clip, SoundPriority pr = SoundPriority.Background)
        {
            AudioSource sr = GetFree();
            sr.clip = clip;
            sr.loop = true;
            SetPriority(sr, pr);
            sr.Play();
        }

        public void ClipLoopAndPlay(AudioClip clip, AudioSource _source, SoundPriority pr = SoundPriority.Background)
        {
            _source.clip = clip;
            _source.loop = true;
            _source.Play();

            SetPriority(_source, pr);

            if (!_additionalSource.Contains(_source))
            {
                if (!isSoundEnabled)
                {
                    _source.volume = 0;
                }

                _additionalSource.Add(_source);
            }
        }

        private AudioSource GetFree()
        {
            for (int i = 0; i < _defaultSource.Count; ++i)
            {
                if (_defaultSource[i].clip == null)
                    return _defaultSource[i];
            }

            AudioSource newClip = gameObject.AddComponent<AudioSource>();
            _additionalSource.Add(newClip);

            return newClip;
        }

        public void ClearSound()
        {
            for (int i = 0; i < _defaultSource.Count; ++i)
            {
                if (_defaultSource[i] != null)
                {
                    _defaultSource[i].clip = null;
                }
            }
        }

        #region Controllers

        /// <summary>
        /// Только для кнопки
        /// </summary>
        public void OnSoundSwitch()
        {
            SetSound(!isSoundEnabled);
        }

        /// <summary>
        /// НЕ ДЛЯ КНОПОК
        /// </summary>
        /// <param name="value"></param>
        public void SetSound(bool value)
        {
            isSoundEnabled = value;

            _backgroundSource.volume = value ? _backgroundVolume : 0;

            float fValue = value ? 1 : 0;
            for (int i = 0; i < _defaultSource.Count; i++)
                _defaultSource[i].volume = fValue;

            for (int i = 0; i < _additionalSource.Count; i++)
                _additionalSource[i].volume = fValue;

            if (_muteSoundButton)
                _muteSoundButton.ChangeSprite(value);
        }

        #endregion
    }

    public enum SoundPriority
    {
        Empty,
        Background,
        Steps,
        Auto,
        Mehanicanism,
        Fire,
        Damage,
        UI,
        Talking,
        System
    }
}