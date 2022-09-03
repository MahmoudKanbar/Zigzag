using UnityEngine;

public class Player : MonoBehaviour
{
	private Vector3 movingDirection;
	private Rigidbody rigidBody;

	private void Awake()
	{
		movingDirection = Vector3.left;
		rigidBody = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (!Input.GetMouseButtonDown(0)) return;
		if (Manager.Instance.State != GameState.Running) return;
		Manager.Instance.OnPlayerSwap();
		if (movingDirection == Vector3.forward) movingDirection = Vector3.left;
		else movingDirection = Vector3.forward;
	}

	private void FixedUpdate()
	{
		if (Manager.Instance.State == GameState.Running)
			rigidBody.velocity = Manager.Instance.PlayerSpeed * movingDirection;
	}

	private void LateUpdate()
	{
		if (Manager.Instance.State == GameState.Running)
			Manager.Instance.CameraCenter.position = transform.position;
		else if (Manager.Instance.State == GameState.Over)
			Manager.Instance.StepsDestroyer.position += Manager.Instance.PlayerSpeed * Time.deltaTime * 
				Manager.Instance.StepsDestroyer.forward;
	}
}
