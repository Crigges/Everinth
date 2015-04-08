using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {

	public float width = 32.0f;
	public float height = 32.0f;
	public float sceneWidth = 1600.0f;
	public float sceneHeight = 1600.0f;
	public Transform tilePrefab;
	public TileSet_C tileset;

	public Color color = Color.white;


	void OnDrawGizmos(){
		Vector3 pos = Camera.current.transform.position;
		Gizmos.color = this.color;

		for (float y = pos.y - sceneWidth; y<pos.y + sceneWidth; y=y + this.height) {
			Gizmos.DrawLine(new Vector3(-1000000.0f, Mathf.Floor(y/height)*this.height,0.0f),new Vector3(1000000.0f,Mathf.Floor(y/this.height)*this.height,0.0f));
		}

		for (float x = pos.x - sceneHeight; x<pos.x + sceneHeight; x=x + this.height) {
			Gizmos.DrawLine (new Vector3( Mathf.Floor(x/this.width)*this.width, -1000000.0f, 0.0f),new Vector3(Mathf.Floor(x/this.width)*this.width, 1000000.0f, 0.0f));
		}

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
