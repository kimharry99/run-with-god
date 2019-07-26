using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HitRange : MonoBehaviour
{
	private SpriteRenderer sr;
	private Collider2D col;

	private void Start()
	{
		sr = GetComponent<SpriteRenderer>();
		col = GetComponent<Collider2D>();
	}

	public WaitForSeconds Activate(float time)
	{
		sr.gameObject.SetActive(true);
		StartCoroutine(ActivateRoutine(time));
		return new WaitForSeconds(time);
	}

	private IEnumerator ActivateRoutine(float time)
	{
		sr.enabled = true;
		float t;
		for (t = 0; t < time; t += Time.deltaTime)
		{
			sr.enabled = t % 0.05f < 0.025f;
			yield return null;
		}
		ContactFilter2D filter = new ContactFilter2D();
		filter.SetLayerMask(1 << LayerMask.NameToLayer("Player"));
		List<Collider2D> results = new List<Collider2D>();
		col.OverlapCollider(filter, results);

		foreach (var result in results)
		{
			result.GetComponent<PlayerController>()?.GetDamaged();
		}

		sr.gameObject.SetActive(false);
	}
}
