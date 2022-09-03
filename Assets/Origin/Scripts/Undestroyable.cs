using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Undestroyable : MonoBehaviour
{
	private static bool checkedAsUndestroyable;
	private void Awake()
	{
		if (checkedAsUndestroyable)
		{
			Destroy(gameObject);
			return;
		}
		checkedAsUndestroyable = true;
		DontDestroyOnLoad(gameObject);
	}
}
