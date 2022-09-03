using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Diamond : MonoBehaviour
{
    [SerializeField] private int scoreAmmount;
    [SerializeField] private DiamondText diamondTextPrefab;

	private void Awake() => scoreAmmount = Random.Range(1, Manager.Instance.DiamondMaxScoreAmmount + 1);

	private void OnTriggerEnter(Collider playerCollider)
	{
		Manager.Instance.OnDiamondCollected(scoreAmmount);
		var diamondText = Instantiate(diamondTextPrefab, transform.position, Quaternion.identity);
		diamondText.Text.text = $"+{scoreAmmount}";
		Destroy(gameObject);
	}
}
