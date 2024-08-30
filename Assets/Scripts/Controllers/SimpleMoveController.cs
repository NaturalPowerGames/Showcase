using UnityEngine;
using DG.Tweening;
using Animancer;

public class SimpleMoveController : MonoBehaviour, IAnimalInteractor
{
	[Header("Settings")]
	[SerializeField]
	private bool automatic;
	[SerializeField]
	private bool interactable;
	[SerializeField]
	private bool rotateOnArrivalToMatchTransform;
	[SerializeField]
	private Transform pointA, pointB;
	[SerializeField]
	private float waitBetweenMovement = 10;
	private float nextMoveTime;
	private bool movingToB, inMovement;
	[Header("Animations")]
	[SerializeField]
	private AnimationClip idle, walk;
	private AnimancerComponent animancerComponent;
	[SerializeField]
	private AnimalType animalType;
	private void Awake()
	{
		animancerComponent = GetComponent<AnimancerComponent>();
		movingToB = false;
	}

	private void Update()
	{
		if (Time.time >= nextMoveTime && automatic)
		{
			if (movingToB)
			{
				RotateAndMove();
			}
			else
			{
				RotateAndMove();
			}
			nextMoveTime = Time.time + waitBetweenMovement;
		}
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
			RotateAndMove();
		}
	}

	private void RotateAndMove()
	{
		if (inMovement) return;
		inMovement = true;
		movingToB = !movingToB;
		Transform destination = movingToB ? pointB : pointA;
		Vector3 direction = (destination.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(direction);

		transform.DORotateQuaternion(lookRotation, 0.25f)
				 .SetEase(Ease.InOutQuad)
				 .OnComplete(() => MoveToPoint(destination));
	}

	private void MoveToPoint(Transform destination)
	{
		PlayWalkAnimation();
		transform.DOMove(destination.position, 3f)
				 .SetEase(Ease.InOutQuad)
				 .OnComplete(OnArrival);
	}

	private void OnArrival()
	{
		inMovement = false;
		PlayIdleAnimation();
		float baseRotation = movingToB ? pointB.eulerAngles.y : pointA.eulerAngles.y;
		float randomYRotation = Random.Range(baseRotation - 60, baseRotation + 60);
		Vector3 randomRotation = new Vector3(transform.eulerAngles.x, randomYRotation, transform.eulerAngles.z);

		transform.DORotate(randomRotation, 0.25f).SetEase(Ease.InOutQuad);
	}

	private void PlayWalkAnimation()
	{
		animancerComponent.Play(walk);
	}

	private void PlayIdleAnimation()
	{
		animancerComponent.Play(idle);
	}

	public AnimalType AnimalType()
	{
		return animalType;
	}
}
