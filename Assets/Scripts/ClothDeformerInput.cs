using UnityEngine;

public class ClothDeformerInput : MonoBehaviour {
	public float force = 10f;
	public float forceOffset = 0.1f;
	
	void Update () {
		if (Input.GetMouseButton(0)) {
			HandleInput();
		}
	}

	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if (Physics.Raycast(inputRay, out hit)) {
			Debug.Log("hit!");

			
			// ClothDeformer deformer = hit.collider.GetComponent<ClothDeformer>();
			// // var sphere = GameObject.FindWithTag("Test Sphere");
			// // var sphereTransform = sphere.GetComponent<Transform>();
			// // sphereTransform.localScale = new Vector3(1, 1, 1);
			// if (deformer) {
				
			// 	Vector3 point = hit.point;
			// 	//point += hit.normal * forceOffset;
			// 	deformer.AddDeformingForce(point, force);
			// }
		}
	}
}