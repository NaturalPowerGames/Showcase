using UnityEngine;
using DG.Tweening;
using Animancer;

public class SimpleMoveController : MonoBehaviour
{
	[SerializeField]
	private bool rotateOnArrivalToMatchTransform;
	[SerializeField]
	private Transform pointA, pointB;
	[SerializeField]
	private float waitBetweenMovement = 10;
	private float nextMoveTime;
	private bool movingToB;
	[SerializeField]
	private AnimationClip idle, walk;
	private AnimancerComponent animancerComponent;

	private void Awake()
	{
		animancerComponent = GetComponent<AnimancerComponent>();
		movingToB = true;
	}

	void Update()
	{
		if (Time.time >= nextMoveTime)
		{
			if (movingToB)
			{
				RotateAndMove(pointB);
			}
			else
			{
				RotateAndMove(pointA);
			}

			nextMoveTime = Time.time + waitBetweenMovement;
			movingToB = !movingToB;
		}
	}
	private void RotateAndMove(Transform destination)
	{
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
}
