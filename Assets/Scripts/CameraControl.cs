using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
	private float mRotationX = 30, mRotationY = 30;
	public float mSensitivityX = 1f;
	public float mSensitivityY = 1f;
	public float mCameraSpeed = 20f;
	private void Start() {
		mRotationX = GetComponent<Transform>().localEulerAngles.y;
		mRotationY = GetComponent<Transform>().localEulerAngles.x;
	}
	void Update () {
		if(Input.GetKey(KeyCode.W))
		{
			transform.Translate(Vector3.forward * mCameraSpeed * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.S))
		{
			transform.Translate(Vector3.back * mCameraSpeed * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.A))
		{
			transform.Translate(Vector3.left * mCameraSpeed * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.D))
		{
			transform.Translate(Vector3.right * mCameraSpeed * Time.deltaTime);
		}
		mRotationX += Input.GetAxis("Mouse X") * mSensitivityX;
		mRotationY -= Input.GetAxis("Mouse Y") * mSensitivityY;
		mRotationY = Mathf.Clamp(mRotationY, -ConstValues.kMaxPitch, ConstValues.kMaxPitch);
		transform.localEulerAngles = new Vector3(mRotationY, mRotationX, 0);
	}

}
