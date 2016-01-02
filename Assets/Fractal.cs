using UnityEngine;
using System.Collections;

public class Fractal : MonoBehaviour {

	// set up 3D object visuals
	public Mesh mesh;
	public Material material;

	// fractal limits and sizes
	public int maxDepth;
	public float childScale;
	private float rotationSpeed;
	private int depth;

	// twists and turns
	public float maxRotationSpeed;
	public float maxTwist;

	// arrays to optimize child values
	private static Vector3[] childDirections =
	{
		Vector3.up, Vector3.right, Vector3.left, Vector3.forward, Vector3.back
	};

	private static Quaternion[] childOrientations =
	{
		Quaternion.identity,
		Quaternion.Euler (0f, 0f, -90f),
		Quaternion.Euler (0f, 0f, 90f),
		Quaternion.Euler (90f, 0f, 0f),
		Quaternion.Euler (-90f, 0f, 0f)

	};

	private Material[] materials;

	private void InitializeMaterials ()
	{
		materials = new Material[maxDepth + 1];
		for (int i = 0; i <= maxDepth; i++) {
			materials[i] = new Material(material);
			materials[i].color =
				Color.Lerp(Color.red, Color.blue, (float)i / maxDepth);
		}
	}

	private void Start ()
	{
		rotationSpeed = Random.Range (-maxRotationSpeed, maxRotationSpeed);
		transform.Rotate(Random.Range(-maxTwist,maxTwist), 0f, 0f);
		if (materials == null) {
			InitializeMaterials();
		}
		gameObject.AddComponent<MeshFilter> ().mesh = mesh;
		gameObject.AddComponent<MeshRenderer> ().material = materials[depth];
		if (depth < maxDepth) 
		{
			StartCoroutine(CreateChildren());
		}
	}

	/// <summary>
	/// Initialize based on parent fractal.
	/// </summary>
	/// <param name="parent">Parent.</param>
	private void Initialize (Fractal parent, int childIndex)
	{

		// update child variables to match the parent
		mesh = parent.mesh;
		materials = parent.materials;
		maxDepth = parent.maxDepth;
		depth = parent.depth + 1;
		childScale = parent.childScale;
		maxRotationSpeed = parent.maxRotationSpeed;
		maxTwist = parent.maxTwist;

		// places new object into parent's heirarchy
		transform.parent = parent.transform;

		// adjust child variables to be different from parent
		transform.localScale = Vector3.one * childScale;
		transform.localPosition = 
			childDirections[childIndex] * (childScale + childScale * childScale);
		Debug.Log ("Child scale is now " + childScale);
		transform.localRotation = 
			childOrientations[childIndex];
	}

	private void Update ()
	{
		transform.Rotate (0f, rotationSpeed * Time.deltaTime, 0f);
	}

	/// <summary>
	/// Creates the children.
	/// </summary>
	/// <returns>The children.</returns>
	private IEnumerator CreateChildren ()
	{
		for (int i = 0; i < childDirections.Length; i++) 
		{
			yield return new WaitForSeconds (Random.Range(0.1f,0.5f));
			new GameObject 
				("Fractal Child").AddComponent<Fractal> ().
					Initialize(this, i);
		}
	}
}
