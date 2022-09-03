using System.Collections;
using UnityEngine;

public class Step : MonoBehaviour
{
	[SerializeField] private bool generateBeforeRunning;
	[SerializeField] private bool shouldGenerateSteps;
	[SerializeField] private bool shouldGenerateDiamond;
	[SerializeField] private Transform[] stepGenerationPoints;
	[SerializeField] private Transform diamondGenerationPoint;
	[SerializeField] private Step stepPrefab;
	[SerializeField] private Diamond diamondPrefab;

	private Diamond newDiamond;

	// to prevent the accidentally generated straight lines by random 
	private static int similarityCounter;
	private static int previousIndex;
	// --------------------------------------------------------------

	private void Update() // only generate once and only if the game is running
	{
		if (shouldGenerateSteps && (Manager.Instance.State == GameState.Running || generateBeforeRunning))
		{
			shouldGenerateSteps = false;
			StartCoroutine(GenerateSteps());
		}
		if (shouldGenerateDiamond && (Manager.Instance.State == GameState.Running || generateBeforeRunning))
		{
			shouldGenerateDiamond = false;
			GenerateDiamond();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("StepsDestroyer"))
			StartCoroutine(AutoDestory());
	}

	private void GenerateDiamond()
	{
		if (Random.Range(0.0f, 1.0f) < Manager.Instance.DiamondsPercentage)
			newDiamond = Instantiate(diamondPrefab, diamondGenerationPoint.position, Quaternion.AngleAxis(90, Vector3.right));
	}

	private IEnumerator GenerateSteps()
	{
		yield return new WaitForSeconds(Manager.Instance.StepGenerateTime);
		var currentIndex = Random.Range(0, stepGenerationPoints.Length);
		var newStep = Instantiate(stepPrefab, transform.parent);
		newStep.shouldGenerateSteps = true;
		newStep.shouldGenerateDiamond = true;

		similarityCounter = currentIndex == previousIndex ? similarityCounter + 1 : 0;
		currentIndex = similarityCounter <= Manager.Instance.StraightLineLength ? currentIndex : (currentIndex + 1) % stepGenerationPoints.Length;
		previousIndex = currentIndex;

		newStep.transform.position = stepGenerationPoints[currentIndex].position;
	}

	private IEnumerator AutoDestory()
	{
		if (newDiamond != null) Destroy(newDiamond.gameObject);
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
		GetComponent<Rigidbody>().AddForce(Physics.gravity);
		yield return new WaitForSeconds(Manager.Instance.StepDestroyTime);
		Destroy(gameObject);
	}
}
