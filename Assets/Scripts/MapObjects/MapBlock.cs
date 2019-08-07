using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBlock : MonoBehaviour
{
	public Transform leftJoint, rightJoint, startPoint;

	PlayerPrefs prefs;
	public void ConnectNextTo(MapBlock block)
	{
		transform.position = block.transform.position + rightJoint.localPosition - leftJoint.localPosition;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(leftJoint.position, 1);
		Gizmos.DrawWireSphere(rightJoint.position, 1);
		Gizmos.DrawWireSphere(startPoint.position, 1);
	}
#endif
}
