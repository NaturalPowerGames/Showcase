using UnityEngine;

public class SFXPlayerController : MonoBehaviour
{
	private AudioSource audioSource;
	[SerializeField]
	private AudioClip[] animalClips;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private void OnEnable()
	{
		AnimalEvents.OnAnimalSoundRequested += OnAnimalSoundRequested;
	}

	private void OnAnimalSoundRequested(AnimalType animalType)
	{
		audioSource.PlayOneShot(animalClips[(int)animalType]);
	}
}
