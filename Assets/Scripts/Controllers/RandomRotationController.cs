using UnityEngine;
using DG.Tweening;

public class RandomRotationController : MonoBehaviour, IAnimalInteractor
{
	public float maxRotationAngle = 45f;
	public float maxInterval = 2f;
	private float nextRotationTime;
	[Header("Settings")]
	[SerializeField]
	private bool automatic;
	[SerializeField]
	private bool interactable;
	[SerializeField]
	private AnimalType animalType;

	private void Start()
	{
		nextRotationTime = Time.time + maxInterval;
	}

	private void Update()
	{
		if (Time.time >= nextRotationTime && automatic)
		{
			Rotate();
			nextRotationTime = Time.time + UnityEngine.Random.Range(0, maxInterval);
		}
	}

	private void Rotate()
	{
		transform.DORotate(new Vector3(0f, UnityEngine.Random.Range(0, maxRotationAngle), 0f), 0.5f);
	}

	private void OnMouseDown()
	{
		OnInteract();
	}

	public void OnInteract()
	{
		if (interactable)
		{
			AnimalEvents.OnAnimalSoundRequested?.Invoke(animalType);
			Rotate();
		}
	}

	public AnimalType AnimalType()
	{
		return animalType;
	}
}
