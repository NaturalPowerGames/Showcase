using UnityEngine;

public class InteractionTransmitterController : MonoBehaviour, IAnimalInteractor
{
	[SerializeField]
	private GameObject[] targets;

	public void OnInteract()
	{
		for (int i = 0; i < targets.Length; i++)
		{
			targets[i].GetComponent<IAnimalInteractor>().OnInteract();
		}
	}

	private void OnMouseDown()
	{
		OnInteract();
	}
}
