using UnityEngine;

public class MusicPlayerController : MonoBehaviour
{
	private AudioSource audioSource;
	private AudioClip dayClip, nightClip;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private void OnEnable()
	{
		TimeEvents.OnDayPhaseChanged += OnDayPhaseChanged;
	}

	private void OnDayPhaseChanged(DayPhase phase)
	{
		switch (phase)
		{
			case DayPhase.Morning:
				audioSource.clip = dayClip;
				audioSource.Play();
				break;
			case DayPhase.Noon:
				break;
			case DayPhase.Afternoon:
				break;
			case DayPhase.Evening:
				audioSource.clip = nightClip;
				audioSource.Play();
				break;
			case DayPhase.Midnight:
				break;
			case DayPhase.Dawn:
				break;
		}
	}
}
