using UnityEngine;

namespace FinalAssignment.EndlessMarble
{
	public class CamFollow : MonoBehaviour
	{
		[SerializeField] private Transform target;
		[SerializeField] private float relativeHeigth = 10.0f;
		[SerializeField] private float zDistance = 5.0f;
		[SerializeField] private float dampSpeed = 2;

		void Update()
		{
			Vector3 newPos = target.position + new Vector3(0, relativeHeigth, -zDistance);
			transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * dampSpeed);
		}
	}

}