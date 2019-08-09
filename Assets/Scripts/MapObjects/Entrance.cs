using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class Entrance : MonoBehaviour
{
	[SerializeField]
	private string nextSceneName;
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Player")
		{
			SceneManager.LoadScene(nextSceneName);
		}
	}
}
