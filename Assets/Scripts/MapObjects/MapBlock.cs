using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBlock : MonoBehaviour
{
	public Transform leftJoint, rightJoint, startPoint;

	PlayerPrefs prefs;
	public void ConnectNextTo(MapBlock block)
	{
		transform.position = block.transform.position + block.rightJoint.localPosition - leftJoint.localPosition;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		if (leftJoint != null)
			Gizmos.DrawWireSphere(leftJoint.position, 1);
		if (rightJoint != null)
			Gizmos.DrawWireSphere(rightJoint.position, 1);
		if (startPoint != null)
			Gizmos.DrawWireSphere(startPoint.position, 1);
	}
#endif
}
