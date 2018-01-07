using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

namespace UIWidgets
{
	/// <summary>
	/// AudioPlayer.
	/// Play single AudioClip.
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class AudioPlayer : MonoBehaviour
	{
		/// <summary>
		/// Slider to display play progress.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("progress")]
		protected Slider Progress;
		
		/// <summary>
		/// Play button.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("playButton")]
		protected Button PlayButton;
		
		/// <summary>
		/// Pause button.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("pauseButton")]
		protected Button PauseButton;
		
		/// <summary>
		/// Stop button.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("stopButton")]
		protected Button StopButton;
		
		/// <summary>
		/// Toggle button.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("toggleButton")]
		protected Button ToggleButton;
		
		AudioSource source;
		
		/// <summary>
		/// AudioSource to play AudioClip.
		/// </summary>
		public AudioSource Source {
			get {
				if (source==null)
				{
					source = GetComponent<AudioSource>();
				}
				return source;
			}
		}

		bool isInited = false;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init AudioPlayer and attach listeners.
		/// </summary>
		public virtual void Init()
		{
			if (isInited)
			{
				return ;
			}
			isInited = true;

			if (PlayButton!=null)
			{
				PlayButton.onClick.AddListener(Play);
			}
			if (PauseButton!=null)
			{
				PauseButton.onClick.AddListener(Pause);
			}
			if (StopButton!=null)
			{
				StopButton.onClick.AddListener(Stop);
			}
			if (ToggleButton!=null)
			{
				ToggleButton.onClick.AddListener(Toggle);
			}

			if (Progress!=null)
			{
				Progress.onValueChanged.AddListener(SetCurrentTimeSample);
			}

		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (PlayButton!=null)
			{
				PlayButton.onClick.RemoveListener(Play);
			}
			if (PauseButton!=null)
			{
				PauseButton.onClick.RemoveListener(Pause);
			}
			if (StopButton!=null)
			{
				StopButton.onClick.RemoveListener(Stop);
			}
			if (ToggleButton!=null)
			{
				ToggleButton.onClick.RemoveListener(Toggle);
			}
			if (Progress!=null)
			{
				Progress.onValueChanged.RemoveListener(SetCurrentTimeSample);
			}
		}

		/// <summary>
		/// Set playback position in seconds.
		/// </summary>
		/// <param name="time">Playback position in seconds.</param>
		public virtual void SetTime(float time)
		{
			if (Source.clip!=null)
			{
				Source.time = Mathf.Min(time, Source.clip.length);
			}
		}

		/// <summary>
		/// Set playback position in PCM samples.
		/// </summary>
		/// <param name="timesample">Playback position in PCM samples.</param>
		protected virtual void SetCurrentTimeSample(float timesample)
		{
			SetCurrentTimeSample(Mathf.RoundToInt(timesample));
		}

		/// <summary>
		/// Set playback position in PCM samples.
		/// </summary>
		/// <param name="timesample">Playback position in PCM samples.</param>
		public virtual void SetCurrentTimeSample(int timesample)
		{
			if (Source.clip!=null)
			{
				Source.timeSamples = Mathf.Min(timesample, Source.clip.samples - 1);
			}
		}

		/// <summary>
		/// Set AudioClip to play.
		/// </summary>
		/// <param name="clip">AudioClip to play.</param>
		public virtual void SetAudioClip(AudioClip clip)
		{
			if (Source.isPlaying)
			{
				Source.Stop();
				Source.clip = clip;
				Source.Play();
			}
			else
			{
				Source.clip = clip;
			}
			Update();
		}

		/// <summary>
		/// Play specified AudioClip.
		/// </summary>
		/// <param name="clip">AudioClip to play.</param>
		public virtual void Play(AudioClip clip)
		{
			Source.Stop();
			Source.clip = clip;
			Source.Play();
		}

		/// <summary>
		/// Play current AudioClip.
		/// </summary>
		public virtual void Play()
		{
			if (Source.timeSamples >= (Source.clip.samples - 1))
			{
				Source.timeSamples = 0;
			}
			Source.Play();
			Update();
		}

		/// <summary>
		/// Pauses playing current AudioClip.
		/// </summary>
		public virtual void Pause()
		{
			Source.Pause();
			Update();
		}

		/// <summary>
		/// Stops playing current AudioClip.
		/// </summary>
		public virtual void Stop()
		{
			Source.Stop();
			Update();
		}

		/// <summary>
		/// Pauses current AudioClip, if it's playing, otherwise unpaused.
		/// </summary>
		public virtual void Toggle()
		{
			if (Source.isPlaying)
			{
				Source.Pause();
				Update();
			}
			else
			{
				Play();
			}
		}

		/// <summary>
		/// Update buttons state and playing progress.
		/// </summary>
		protected virtual void Update()
		{
			if (PlayButton!=null)
			{
				PlayButton.gameObject.SetActive(!Source.isPlaying);
			}
			if (PauseButton!=null)
			{
				PauseButton.gameObject.SetActive(Source.isPlaying);
			}
			if (StopButton!=null)
			{
				StopButton.gameObject.SetActive(Source.isPlaying);
			}

			if (Progress!=null)
			{
				Progress.wholeNumbers = true;
				Progress.minValue = 0;
				if (Source.clip!=null)
				{
					Progress.maxValue = Source.clip.samples;
					Progress.value = Source.timeSamples;
				}
				else
				{
					Progress.maxValue = 100;
					Progress.value = 0;
				}
			}
		}
	}
}