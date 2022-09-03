using TMPro;
using UnityEngine;

public class DiamondText : MonoBehaviour
{
	[SerializeField] private Animator animator;
	[SerializeField] private TextMeshPro text;


	private void Awake() => animator.Play("DiamondCollected-Text");
	private void Update() => transform.rotation = Camera.main.transform.rotation;


	public void DestroyObject() => Destroy(gameObject);
	public TextMeshPro Text => text;
}
