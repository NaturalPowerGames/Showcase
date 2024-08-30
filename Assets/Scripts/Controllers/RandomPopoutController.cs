using UnityEngine;
using DG.Tweening;

public class RandomPopoutController : MonoBehaviour, IAnimalInteractor
{
    public Transform popoutLocationsParent;
    private Transform[] popoutLocations;

	[SerializeField]
	private float popoutTime, hideTime;
	private float popoutCountdown, hideCountdown;
	[SerializeField]
	private float interactionTime;
	[SerializeField]
	private bool automatic, interactable;
	private bool hidden;
	private Renderer[] renderers;

	private void Awake()
	{
        popoutLocations = new Transform[popoutLocationsParent.childCount];
        for (int i = 0; i < popoutLocationsParent.childCount; i++)
		{
            popoutLocations[i] = popoutLocationsParent.GetChild(i);
		}
		renderers = GetComponentsInChildren<Renderer>();
	}

	private void Update()
	{
		if(interactionTime > 0)
		{
			interactionTime -= Time.deltaTime;
		}
		else
		{
			if(interactable)
			return;
		}
		if (!automatic && interactionTime < 0) return;
		if (hidden)
		{
			if (popoutCountdown > 0)
			{
				popoutCountdown -= Time.deltaTime;
			}
			else
			{
				popoutCountdown = popoutTime;
				PopOut();
			}
		}
		else
		{
			if (hideCountdown > 0)
			{
				hideCountdown -= Time.deltaTime;
			}
			else
			{
				hideCountdown = hideTime;
				Hide();
			}
		}
	}


	private void PopOut()
	{
		hidden = false;
		int randomPopoutLocation = UnityEngine.Random.Range(0, popoutLocations.Length);
		Vector3 chosenLocation = popoutLocations[randomPopoutLocation].position;
		Vector3 position = transform.position;
		position.x = chosenLocation.x;
		position.z = chosenLocation.z;
		transform.position = position;
		ToggleVisuals(true);
		transform.DOMoveY(chosenLocation.y, 0.5f);
	}

	private void Hide()
	{
		hidden = true;
		float hideDestinationY = transform.position.y - 0.5f;
		transform.DOMoveY(hideDestinationY, 0.5f).OnComplete(() => ToggleVisuals(false)) ;
	}


	public void ToggleVisuals(bool active)
	{
		foreach (Renderer renderer in renderers)
		{
			renderer.gameObject.SetActive(active);
		}
	}

	public void OnInteract()
	{
		interactionTime += 10;
	}
}
