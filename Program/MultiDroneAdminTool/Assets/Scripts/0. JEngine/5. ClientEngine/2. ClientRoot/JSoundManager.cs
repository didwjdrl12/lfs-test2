using System.Collections.Generic;
using UnityEngine;


namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JSoundManager
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JSoundManager : MonoBehaviour
    {
        public static JSoundManager Instance;

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public static string _folder = "SharedData/Sound/";


        #region 변수
        private IDictionary<string, AudioClip> _bgms = new Dictionary<string, AudioClip>();
        private IDictionary<string, AudioClip> _effectSounds = new Dictionary<string, AudioClip>();
        private IDictionary<string, AudioClip> _envBgms = new Dictionary<string, AudioClip>();
        public AudioSource _bgmAudio;
        public AudioSource _bgmAudio2;
        public AudioSource _envAudio;
        public AudioSource _envAudio2;
        public AudioSource _envAudio3;
        public AudioSource _effectAudio;

        private bool _fadeInOutBGM;
        private float _fadeInOutVolume;
        private float _envSoundTime;
        private float _envSoundPlayTime;
        private string _envSoundName;

        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 메인 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region 메인 함수
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Awake()
        {
            Instance = this;

            JLogger.Write("InitSound");

            _bgmAudio = gameObject.AddComponent<AudioSource>();
            _envAudio = gameObject.AddComponent<AudioSource>();
            _envAudio2 = gameObject.AddComponent<AudioSource>();
            _envAudio3 = gameObject.AddComponent<AudioSource>();
            _bgmAudio2 = gameObject.AddComponent<AudioSource>();
            _effectAudio = gameObject.AddComponent<AudioSource>();

#if !NET_SERVER
            LoadBGM("/BGM/MainBGM");
            LoadBGM("/BGM/BattleBGM");
#endif
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void StopAll()
        {
            StopBGM();
            StopEnvBGM();
            StopEffect();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Update()
        {
            if (_fadeInOutBGM)
            {
                var speed = 0.5f;
                _bgmAudio.volume = Mathf.Min(1f, _bgmAudio.volume + Time.deltaTime * speed);
                _bgmAudio2.volume = Mathf.Max(0f, _bgmAudio2.volume - Time.deltaTime * speed);

                if (_bgmAudio.volume >= _fadeInOutVolume)
                {
                    _fadeInOutBGM = false;
                    _bgmAudio2.Stop();
                }
            }

            if (_envSoundName != null)
            {
                if (Time.time - _envSoundPlayTime > _envSoundTime)
                    PlayEnvBGM3(_envSoundName, _envSoundTime);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // 볼륨 조절
        private float _volume_ = 1.0f;
        public float _volume
        {
            get { return _volume_; }
            set
            {
                _volume_ = value;
                _bgmAudio.volume = _volume_;
                _envAudio.volume = _volume_;
                _envAudio2.volume = _volume_;
                _envAudio3.volume = _volume_;
                _effectAudio.volume = _volume_;
            }
        }

        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // BGM 사운드
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region BGM 사운드
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private bool LoadBGM(string name)
        {
            AudioClip clip = null;
            //if(PbcGameSetup.Instance._downLoadAsset)
            //	clip = BcAssetBundleManager.Instance.FindAssetBundle("Sound").Load(name, typeof(AudioClip)) as AudioClip;
            //else
            clip = (AudioClip)Resources.Load(_folder + "BGM/" + name);
            if (clip == null)
                return false;
            _bgms.Add(name, clip);

            return true;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private string _prevBgmName = "";
        public void PlayBGM(string name, bool loop = true, bool fadeInOut = false)
        {
            if (!JBase._sound)
                return;

            if (false == _bgms.ContainsKey(name))
            {
                if (false == LoadBGM(name))
                    return;
            }

            if (_prevBgmName == name)
                return;

            Debug.Log("PlayBGM " + name);

            _prevBgmName = name;

            if (_bgmAudio.clip != null && fadeInOut)
            {
                _bgmAudio2.clip = _bgmAudio.clip;
                _bgmAudio2.volume = _bgmAudio.volume;
                _bgmAudio2.time = _bgmAudio.time;
                _bgmAudio2.Play();
            }


            AudioClip bgm = _bgms[name];
            _bgmAudio.Stop();
            _bgmAudio.clip = bgm;
            _bgmAudio.loop = loop;
            _bgmAudio.volume = 0.1f; //volume * SsWindow_Admin.Instance.GetAdminVolume();
            if (!fadeInOut)
                _bgmAudio.volume = _volume;
            _bgmAudio.Play();

            _fadeInOutBGM = fadeInOut;
            _fadeInOutVolume = _volume;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void StopBGM()
        {
            Debug.Log("StopBGM");
            _bgmAudio.Stop();
            _prevBgmName = "";
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 환경 사운드
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region 환경 사운드
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public AudioClip PlayEnvBGM(string name, float volumeScale = 1f)
        {
            if (!JBase._sound)
                return null;

            var sound = getEnvSound(name);
            if (sound == null)
                return null;
            _envAudio.Stop();
            _envAudio.clip = sound;
            _envAudio.loop = true;
            _envAudio.volume = _volume * volumeScale;// *SsWindow_Admin.Instance.GetAdminVolume();
            _envAudio.Play();

            return sound;
        }
        public AudioClip PlayEnvBGM2(string name, float volumeScale = 1f)
        {
            if (!JBase._sound)
                return null;

            var sound = getEnvSound(name);
            if (sound == null)
                return null;
            _envAudio2.Stop();
            _envAudio2.clip = sound;
            _envAudio2.loop = true;
            _envAudio2.volume = _volume * volumeScale;// *SsWindow_Admin.Instance.GetAdminVolume();
            _envAudio2.Play();

            return sound;
        }
        public AudioClip PlayEnvBGM3(string name, float time, float volumeScale = 1f)
        {
            if (!JBase._sound)
                return null;

            var sound = getEnvSound(name);
            if (sound == null)
                return null;
            _envAudio3.Stop();
            _envAudio3.clip = sound;
            _envAudio3.loop = false;
            _envAudio3.volume = _volume * volumeScale;// *SsWindow_Admin.Instance.GetAdminVolume();
            _envAudio3.Play();

            _envSoundName = name;
            _envSoundTime = time;
            _envSoundPlayTime = Time.time;

            return sound;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private AudioClip getEnvSound(string name)
        {
            if (false == _envBgms.ContainsKey(name))
            {
                AudioClip clip = null;
                //if (PbcGameSetup.Instance._downLoadAsset)
                //	clip = BcAssetBundleManager.Instance.FindAssetBundle("Sound").Load(name, typeof(AudioClip)) as AudioClip;
                //else
                clip = (AudioClip)Resources.Load(_folder + "Env/" + name);
                if (clip == null)
                    return null;
                Debug.Log("Load EnvSound:" + name);
                _envBgms.Add(name, clip);
            }

            return _envBgms[name];
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void StopEnvBGM()
        {
            _envAudio.Stop();
            _envAudio2.Stop();
            _envAudio3.Stop();
            _envSoundName = null;
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 이펙트 사운드
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region 이펙트 사운드
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public AudioClip PlayEffect(string name, float volumeScale = 1f)
        {
            if (!JBase._sound)
                return null;

            //Debug.Log(name);

            var sound = getEffectSound(name);
            if (sound == null)
                return null;
            _effectAudio.volume = _volume * volumeScale;// *SsWindow_Admin.Instance.GetAdminVolume();
            _effectAudio.PlayOneShot(sound, _volume * volumeScale);

            return sound;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private AudioClip getEffectSound(string name)
        {
            if (false == _effectSounds.ContainsKey(name))
            {
                AudioClip clip = null;
                //if (PbcGameSetup.Instance._downLoadAsset)
                //	clip = BcAssetBundleManager.Instance.FindAssetBundle("Sound").Load(name, typeof(AudioClip)) as AudioClip;
                //else
                clip = (AudioClip)Resources.Load(_folder + "Effect/" + name);
                if (clip == null)
                    return null;
                //Debug.Log("Load EffectSound:" + name);
                _effectSounds.Add(name, clip);
            }

            return _effectSounds[name];
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void StopEffect()
        {
            _effectAudio.Stop();
        }
        #endregion

    }

}

