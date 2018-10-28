using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace Isjhar.Library
{
	public class AudioManager : MonoBehaviour
	{
		[Range(0f, 1f)][SerializeField] private float _currentVolumeBGM = 1f;
		[Range(0f, 1f)][SerializeField] private float _currentVolumeSFX = 1f;
		[SerializeField] private AudioDetail[] _defaulAudios;
		[SerializeField] private bool _onBGM = true;
		[SerializeField] private bool _onSFX = true;
		[SerializeField] private AudioMixer _mixer = null;

		private float _volumeSFX = 1f;
		private float _volumeBGM = 1f;
		private Queue<AudioSource> _availableSources = new Queue<AudioSource>();
		private Dictionary<AudioSource, AudioDetail> _playingSources = new Dictionary<AudioSource, AudioDetail>();
		private Dictionary<string, AudioDetail> _defaultAudiosDict = new Dictionary<string, AudioDetail>();
		private List<AudioSource> _playingSourcesList = new List<AudioSource>();
		private List<AudioSource> _idleSources = new List<AudioSource>();
		private AudioSource _bgmSource;
		private AudioDetail _bgmAudio;
		private IEnumerator _fadeBGMTweener;
		private Dictionary<AudioSource, IEnumerator> _fadeSFXTweener = new Dictionary<AudioSource, IEnumerator>();
		private string _currentScene;
		private bool _isPaused = false;
		private bool _onApplicationPaused = false;
		private AudioMixerGroup _bgmGroup = null;
		private AudioMixerGroup _sfxGroup = null;

		public bool OnBGM
		{
			get
			{
				return _onBGM;
			}
			set
			{
				_onBGM = value;
			}
		}

		public bool OnSFX
		{
			get
			{
				return _onSFX;
			}
			set
			{
				_onSFX = value;
			}
		}

		public AudioSource BGMSource
		{
			get
			{
				return _bgmSource;
			}
		}

		protected virtual void Awake ()
		{
			_onBGM = true;
			_onSFX = true;
			_currentVolumeBGM = _volumeBGM;
			_bgmSource = gameObject.AddComponent<AudioSource>();
			_bgmSource.loop = true;
			_bgmSource.volume = 1f;
			_bgmSource.playOnAwake = false;
			_currentScene = SceneManager.GetActiveScene().name;

			_bgmGroup = _mixer.FindMatchingGroups("BGM")[0];
			_sfxGroup = _mixer.FindMatchingGroups("SFX")[0];
			InitDefaultAudioDict();
		}

		private void Update()
		{
			CheckUnusedSource();
			SetVolumeSFX(_currentVolumeSFX);
			SetVolumeBGM(_currentVolumeBGM);
		}

		private void OnApplicationPause(bool isPause)
		{
			_onApplicationPaused = isPause;
		}

		private void InitDefaultAudioDict()
		{
			for(int i = 0; i < _defaulAudios.Length; i++)
			{
				AudioDetail audio = _defaulAudios[i];
				if(audio != null && audio.Clip != null)
					_defaultAudiosDict.Add(audio.Clip.name, audio);
			}
		}

		private void CheckUnusedSource()
		{
			if(_isPaused || _onApplicationPaused)
				return;
			
			/// Check Unused audiosource
			for(int i = 0; i < _playingSourcesList.Count; i++)
			{
				AudioSource source = _playingSourcesList[i];
				if(!source.isPlaying)
				{
					_idleSources.Add(source);
				}
			}

			if(_idleSources.Count > 0)
			{
				for(int i = 0; i < _idleSources.Count; i++)
				{
					AudioSource idleSource = _idleSources[i];
					RemoveSource(idleSource);
					idleSource.clip = null;
					_availableSources.Enqueue(idleSource);
				}
				_idleSources.Clear();
			}
		}

		#region BGM
		public void PlayBGM(AudioDetail audio)
		{
			_bgmAudio = audio;
			_bgmSource.clip = _bgmAudio.Clip;
			_bgmSource.volume = _bgmAudio.Volume;
			_bgmSource.loop = _bgmAudio.Loop;
			_bgmSource.outputAudioMixerGroup = _bgmGroup;
			_bgmSource.Play();
		}

		public void PlayBGM(AudioDetail audio, float volume)
		{
			_bgmAudio = audio;
			_bgmSource.clip = _bgmAudio.Clip;
			_bgmSource.volume = _bgmAudio.Volume * volume;
			_bgmSource.loop = _bgmAudio.Loop;
			_bgmSource.outputAudioMixerGroup = _bgmGroup;
			_bgmSource.Play();
		}

		public void PlayBGM(string audioName)
		{
			AudioDetail audio = GetAudio(audioName);
			if(audio == null)
			{
				Debug.LogFormat("Failed playe bgm : {0}, asset not found", audioName);
				return;
			}
			PlayBGM(audio);
		}

		public void FadeBGM(float endValue, float duration, UnityAction OnCompleted = null)
		{
			if(_fadeBGMTweener != null)
			{
				StopCoroutine (_fadeBGMTweener);
				_fadeBGMTweener = null;
			}
			_fadeBGMTweener = DoFadeBGM (endValue, duration, () =>
			{ 
				OnBGMFadeCompeleted ();
				if (OnCompleted != null)
					OnCompleted ();
			});
			StartCoroutine (_fadeBGMTweener);
		}

		private float GetVolumeMixer(float volume)
		{
			return -80f + (80) * volume;
		}

		public void SetVolumeBGM(float volume)
		{
			if(_volumeBGM == volume)
				return;

			_volumeBGM = volume;
			_mixer.SetFloat("BGMVolume", GetVolumeMixer(volume));
		}

		public void StopBGM()
		{
			_bgmSource.Stop();
		}

		private void OnBGMFadeCompeleted()
		{
			_fadeBGMTweener = null;
		}

		private IEnumerator DoFadeBGM(float endValue, float duration, UnityAction OnCompleted = null)
		{
			float currentDuration = 0;
			float startValue = _bgmSource.volume;
			while (_bgmSource.volume < endValue)
			{
				_bgmSource.volume = Mathf.Lerp (startValue, endValue, currentDuration / duration);
				currentDuration += Time.deltaTime;
				yield return null;
			}
			if (OnCompleted != null)
				OnCompleted ();
		}
		#endregion

		#region SFX

		private AudioSource GetSource(AudioDetail audio)
		{
			foreach(KeyValuePair<AudioSource, AudioDetail> entry in _playingSources)
			{
				if(entry.Value == audio)
				{
					return entry.Key;
				}
			}
			return null;
		}

		private AudioSource GetAvailableSource(bool newSource)
		{
			AudioSource availableSource = null;
			if(newSource)
			{
				availableSource = gameObject.AddComponent<AudioSource>();
			}
			else
			{
				availableSource = _availableSources.Count > 0 ? _availableSources.Dequeue() : 
					gameObject.AddComponent<AudioSource>();
			}
			return availableSource;
		}

		private void OnSFXFadeCompleted()
		{
			
		}

		public AudioSource PlaySFX(AudioDetail audio, bool newSource = false)
		{
			if(audio == null || audio.Clip == null)
			{
				Debug.LogFormat("Failed Play SFX Audio Null");
				return null;
			}

			AudioSource availableSource = GetAvailableSource(newSource);
			availableSource.clip = audio.Clip;
			availableSource.volume = audio.Volume;
			availableSource.loop = audio.Loop;
			availableSource.playOnAwake = false;
			availableSource.outputAudioMixerGroup = _sfxGroup;
			AddSource(availableSource, audio);
			availableSource.time = 0;
			availableSource.Play();
			return availableSource;
		}

		public AudioSource PlaySFX(string audioName, bool newSource = false)
		{
			AudioDetail audio = GetAudio(audioName);
			if(audio == null)
			{
				Debug.LogFormat("Failed playe sfx : {0}, asset not found", audioName);
				return null;
			}
			return PlaySFX(audio, newSource);
		}

		public AudioSource PlaySFXAtSpesificTime(AudioDetail audio, float time, bool newSource = false)
		{
			AudioSource availableSource = GetAvailableSource(newSource);
			availableSource.clip = audio.Clip;
			availableSource.loop = audio.Loop;
			availableSource.time = time;
			availableSource.volume = audio.Volume;
			availableSource.playOnAwake = false;
			availableSource.outputAudioMixerGroup = _sfxGroup;
			AddSource(availableSource, audio);
			availableSource.Play();
			return availableSource;
		}

		public void PlaySFXAtSpesificTime(AudioSource source, AudioDetail audio, float time)
		{
			source.clip = audio.Clip;
			source.loop = audio.Loop;
			source.time = time;
			source.volume = audio.Volume;
			source.playOnAwake = false;
			source.outputAudioMixerGroup = _sfxGroup;
			source.Play();
		}

		public void StopSFX(AudioDetail audio)
		{
			AudioSource source = GetSource(audio);
			if(source != null)
				source.Stop();
		}

		public void SetVolumeSFX(float volume)
		{
			if(_volumeSFX == volume)
				return;

			_volumeSFX = volume;
			_mixer.SetFloat("SFXVolume", GetVolumeMixer(volume));
		}

		public void SetVolumeSFX(AudioDetail audio, float volume)
		{
			AudioSource source = GetSource(audio);
			if(source != null)
				source.volume = audio.Volume * volume;
		}

		public void FadeSFX(AudioSource source, float startValue, float endValue, float duration)
		{
			IEnumerator tween;
			if(_fadeSFXTweener.ContainsKey(source))
			{
				tween = _fadeSFXTweener[source];
				StopCoroutine (tween);
				_fadeSFXTweener.Remove(source);
				Debug.LogFormat("Tween For {0} Exist, Killl", source.clip.name);
			}
			source.volume = startValue;
			tween = DoFadeSFX (source, startValue, endValue, duration, () =>
			{
				_fadeSFXTweener.Remove (source); 
				Debug.LogFormat ("Fade Source {0} Completed", source.clip.name);
			});
			_fadeSFXTweener.Add(source, tween);
			StartCoroutine (tween);
		}

		public void StopAllSFX()
		{
			for(int i = 0; i < _playingSourcesList.Count; i++)
			{
				AudioSource source = _playingSourcesList[i];
				source.Stop();
			}
		}

		private IEnumerator DoFadeSFX(AudioSource source, float startValue, float endValue, float duration, UnityAction OnCompleted)
		{
			source.volume = startValue;
			float currentDuration = 0;
			while (source.volume < endValue)
			{
				source.volume = Mathf.Lerp (startValue, endValue, currentDuration / duration);
				currentDuration += Time.deltaTime;
				yield return null;
			}
			if (OnCompleted != null)
				OnCompleted ();
		}
		#endregion

		public AudioDetail GetAudio(string audioName)
		{
			if(_defaultAudiosDict.ContainsKey(audioName))
			{
				return _defaultAudiosDict[audioName];
			}
			return null;
		}

		public void Pause()
		{
			_isPaused = true;
			for(int i = 0; i < _playingSourcesList.Count; i++)
			{
				AudioSource source = _playingSourcesList[i];
				source.Pause();
			}
		}

		public void UnPause()
		{
			for(int i = 0; i < _playingSourcesList.Count; i++)
			{
				AudioSource source = _playingSourcesList[i];
				source.UnPause();
			}
			_isPaused = false;
		} 

		public void StopAll()
		{
			_isPaused = false;
			_bgmSource.Stop();
			for(int i = 0; i < _playingSourcesList.Count; i++)
			{
				AudioSource source = _playingSourcesList[i];
				source.Stop();
			}
		}

		public void SetOnBGM(bool on)
		{
			_onBGM = on;
			_currentVolumeBGM = _onBGM ? 1f : 0f;
		}

		public void SetOnSFX(bool on)
		{
			_onSFX = on;
			_currentVolumeSFX = _onSFX ? 1f : 0f;
		}

		public void AddSource(AudioSource source, AudioDetail detail)
		{
			_playingSources.Add(source, detail);
			_playingSourcesList.Add(source);
		}

		public void RemoveSource(AudioSource source)
		{
			_playingSources.Remove(source);
			_playingSourcesList.Remove(source);
		}
	}
}
