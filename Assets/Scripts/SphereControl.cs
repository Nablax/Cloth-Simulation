using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereControl : MonoBehaviour
{
	public float sensitivityX = 1f;
	public float sensitivityY = 1f;
	public float mSphereSpeed = 20f;
	void Update () {
		if(Input.GetKey(KeyCode.Z))
		{
			transform.Translate(Vector3.forward * mSphereSpeed * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.X))
		{
			transform.Translate(Vector3.back * mSphereSpeed * Time.deltaTime);
		}
        if(Input.GetKey(KeyCode.Q))
		{
			transform.Translate(Vector3.left * mSphereSpeed * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.E))
		{
			transform.Translate(Vector3.right * mSphereSpeed * Time.deltaTime);
		}
        if(Input.GetKey(KeyCode.R))
		{
			transform.Translate(Vector3.up * mSphereSpeed * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.F))
		{
			transform.Translate(Vector3.down * mSphereSpeed * Time.deltaTime);
		}
	}
}
