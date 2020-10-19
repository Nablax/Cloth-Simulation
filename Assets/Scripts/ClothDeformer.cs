using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ClothDeformer : MonoBehaviour {

	public float ks = 1000, kd = 230, nodeMass = 10, dragParam = 0.02f;
	private float oriStringSize, maxStringSize, vMax;
	private int xSize, ySize, updateRate = 5;
	Mesh deformingMesh;
	Vector3[] originalVertices, displacedVertices;
	Vector3[] vertexVelocities, vertexAccelerations;
	Vector3[] dragForce;
	void Start () {
		deformingMesh = GetComponent<MeshFilter>().mesh;
		originalVertices = deformingMesh.vertices;
		displacedVertices = new Vector3[originalVertices.Length];
		for (int i = 0; i < originalVertices.Length; i++) {
			displacedVertices[i] = originalVertices[i];
		}
		vertexVelocities = new Vector3[originalVertices.Length];
		vertexAccelerations = new Vector3[originalVertices.Length];
		xSize = GetComponent<ClothGenerator>().xSize;
		ySize = GetComponent<ClothGenerator>().ySize;
		oriStringSize = GetComponent<ClothGenerator>().stringLen;
		maxStringSize = oriStringSize * 1.5f;
		dragForce = new Vector3[xSize * ySize * 2];
		vMax = 2 * Mathf.Sqrt(ConstValues.kGravity * ySize * maxStringSize);
		//aMax = 2 * ConstValues.kGravity;
	}
	void FixedUpdate () {
		for(int time = 0; time < updateRate; time++){
			for(int i = 0; i < vertexAccelerations.Length; i++){
				vertexAccelerations[i] = new Vector3(0, -ConstValues.kGravity, 0);
			}
			for(int i = 0; i <= xSize; i++){
				for(int j = 0; j <= ySize; j++){
					UpdateVertex(j, i);
				}
			}
			for(int i = 0; i <= xSize; i++){
				fixedPoint(0, i);
			}
			// fixedPoint(0, 0);
			// fixedPoint(0, xSize);
			for(int i = 0; i < vertexAccelerations.Length; i++){
				integration(i);
			}
			deformingMesh.vertices = displacedVertices;
			gameObject.GetComponent<MeshCollider>().sharedMesh = deformingMesh;
		}
		deformingMesh.RecalculateNormals();
	}

	private int coordinateToIndex(int j, int i){
		if(i < 0 || j < 0 || i > xSize || j > ySize){
			return -1;
		}
		return (ySize - j) * (xSize + 1) + i;
	}
	private void fixedPoint(int j, int i){
		var coord = coordinateToIndex(j, i);
		if(coord < 0){
			return;
		}
		vertexAccelerations[coord] = new Vector3(0, 0, 0);
		vertexVelocities[coord] = new Vector3(0, 0, 0);
	}
	Vector3 computeTriangleDragForce(int node1, int node2, int node3){
		if(node1 < 0 || node2 < 0 || node3 < 0){
			return new Vector3(0, 0, 0);
		}

		var nStar = Vector3.Cross(displacedVertices[node2] - displacedVertices[node1], displacedVertices[node3] - displacedVertices[node1]);
		var v = (vertexVelocities[node1] + vertexVelocities[node1] + vertexVelocities[node1]) / 3;
		if(Vector3.Magnitude(nStar) < Mathf.Epsilon){
			return new Vector3(0, 0, 0);
		}
		//Debug.Log(nStar);
		//return -0.25f * dragParam * Vector3.Magnitude(v) * Vector3.Dot(v, nStar) / 100 * nStar;
		return -0.25f * dragParam * Vector3.Magnitude(v) * Vector3.Dot(v, nStar) / Vector3.Magnitude(nStar) * nStar;
	}
	Vector3 computeVertexDragForce(int j, int i){
		if(j > ySize || i > xSize || j < 0 || i < 0){
			return new Vector3(0, 0, 0);
		}
		int middleNode = coordinateToIndex(j, i);
		int topNode = coordinateToIndex(j - 1, i);
		int leftNode = coordinateToIndex(j, i - 1);
		int rightNode = coordinateToIndex(j, i + 1);
		int botNode = coordinateToIndex(j + 1, i + 1);
		return computeTriangleDragForce(middleNode, leftNode, topNode) + computeTriangleDragForce(middleNode, leftNode, botNode)+
		computeTriangleDragForce(middleNode, topNode, rightNode) + computeTriangleDragForce(middleNode, rightNode, botNode);
	}
	private void computeSpringForce(int j, int i, int jNext, int iNext, float oriStringSize){
		if(jNext > ySize || iNext > xSize || j < 0 || i < 0 || iNext < 0 || jNext < 0){
			return;
		}
	
		int nextCoord = coordinateToIndex(jNext, iNext);
		int curCoord = coordinateToIndex(j, i);
		var curStringSize = Vector3.Magnitude(displacedVertices[nextCoord] - displacedVertices[curCoord]);
		// Debug.Log(
		// 	"curN: " + curCoord + 
		// 	" nextN: " + nextCoord + 
		// 	" curY: " + displacedVertices[curCoord].y + 
		// 	" nextY: " + displacedVertices[nextCoord].y +
		// 	" strLen: " + curStringSize);
		if(curStringSize <= 0){
			return;
		}
		var curStringDir = Vector3.Normalize(displacedVertices[nextCoord] - displacedVertices[curCoord]);
		var topVel = Vector3.Dot(curStringDir, vertexVelocities[curCoord]);
		var botVel = Vector3.Dot(curStringDir, vertexVelocities[nextCoord]);
		var stringForce = curStringDir * (-ks * (curStringSize - oriStringSize) - kd * (botVel - topVel));
		if(nodeMass < 0){
			nodeMass = 1;
		}
		var stringAcc = stringForce / nodeMass;
		var dragForce = computeVertexDragForce(j, i);
		vertexAccelerations[curCoord] += dragForce / nodeMass;

		// print("v: " + vertexVelocities[coordinateToIndex(j, i)]);
		// print("drag:" + dragForce);
		vertexAccelerations[curCoord] -= stringAcc;
		vertexAccelerations[nextCoord] += stringAcc;
	}
	void shrinkVector(ref Vector3 inVec, float bound){
		var tmpLen = Vector3.Magnitude(inVec);
		if(tmpLen <= bound){
			return;
		}
		inVec = Vector3.Normalize(inVec) * bound;
	}
	private void integration(int curNode){
		float dt = Time.fixedDeltaTime / updateRate * 2;
		//shrinkVector(vertexAccelerations[curNode], aMax);
		displacedVertices[curNode] += (vertexVelocities[curNode]  + 0.5f * vertexAccelerations[curNode] * dt) * dt;
		vertexVelocities[curNode] += vertexAccelerations[curNode] * dt;
	}
	void sphereCollision(int j, int i, string tag){
		float bounceBackParam = 0.5f;
		var sphereTransform = GameObject.FindWithTag("Test Sphere").GetComponent<Transform>();
		var curNode = coordinateToIndex(j, i);
		var toCenterVec = sphereTransform.position - displacedVertices[curNode];
		var r = sphereTransform.localScale.x / 2 + 1.5f;

		var toCenterLen = Vector3.Magnitude(toCenterVec);

		if(toCenterLen < r && toCenterLen > 0){
			//Debug.Log(r);
			toCenterVec = Vector3.Normalize(toCenterVec);
			displacedVertices[curNode] = sphereTransform.position - toCenterVec * r;
			var accProjToCentor = Vector3.Dot(vertexAccelerations[curNode], toCenterVec);
			if(accProjToCentor > 0){
				vertexAccelerations[curNode] -= accProjToCentor * toCenterVec * (1 + bounceBackParam);
			}
		}
	}
	void UpdateVertex (int j, int i) {
		computeSpringForce(j, i, j + 1, i, oriStringSize);
		computeSpringForce(j, i, j, i + 1, oriStringSize);

		computeSpringForce(j, i, j + 1, i + 1, 1.4142f * oriStringSize);
		computeSpringForce(j, i, j + 1, i - 1, 1.4142f * oriStringSize);
		sphereCollision(j, i, "Test Sphere");
	}

	public void AddDeformingForce (Vector3 point, float force) {

	}

	void AddForceToVertex (int i, Vector3 point, float force) {

	}
}