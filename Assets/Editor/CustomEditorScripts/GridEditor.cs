using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;


[CustomEditor(typeof(Grid))]
public class GridEditor : Editor {

	Grid grid;

	private int oldIndex = 0;


	void OnEnable(){
		grid = (Grid)target;
	}

	[MenuItem("Assets/Create/TileSet")]
	static void CreateTileSet(){
		var asset = ScriptableObject.CreateInstance<TileSet_C>();
		var path = AssetDatabase.GetAssetPath(Selection.activeObject);

		if (string.IsNullOrEmpty(path)) {
			path = "Assets";
		} else {
			if(Path.GetExtension(path) != ""){
				path = path.Replace(Path.GetFileName(path),"");
			}
			else{
				path = path + "/";
			}
		}

		var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "TileSet.asset");
		AssetDatabase.CreateAsset (asset, assetPathAndName);
		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
		asset.hideFlags = HideFlags.DontSave;
	}


	public override void OnInspectorGUI(){
		grid.width = CreateSlider ("Width", grid.width);
		grid.height = CreateSlider ("Height", grid.height);

		if (GUILayout.Button ("Open Grid Window")) {	//executed if clicked
			GridWindow window  = (GridWindow)EditorWindow.GetWindow (typeof(GridWindow));
			window.Init();
		}

		//Tile Prefab
		EditorGUI.BeginChangeCheck();
		var newTilePrefab = (Transform)EditorGUILayout.ObjectField("Tile Prefab",grid.tilePrefab, typeof(Transform),false);
		if(EditorGUI.EndChangeCheck()){
			grid.tilePrefab = newTilePrefab;
			Undo.RecordObject(target,"Grid changed");
		}

		//Tile Map
		EditorGUI.BeginChangeCheck();
		var newTileset = (TileSet_C)EditorGUILayout.ObjectField ("Tileset", grid.tileset, typeof(TileSet_C), false);
		if(EditorGUI.EndChangeCheck()){
			grid.tileset = newTileset;
			Undo.RecordObject(target,"Grid Changed");
		}

		if (grid.tileset != null) {
			EditorGUI.BeginChangeCheck();
			var names = new string[grid.tileset.prefabs.Length];
			var values = new int[names.Length];

			for(int i = 0; i<names.Length;i++){
				names[i] = grid.tileset.prefabs[i] != null ? grid.tileset.prefabs[i].name : "";
				values[i] = i;
			}

			var index = EditorGUILayout.IntPopup("Select Tile",oldIndex,names,values);
			if(EditorGUI.EndChangeCheck()){
				Undo.RecordObject(target,"Grid Changed");
				if(oldIndex != index){
					oldIndex = index;
					grid.tilePrefab = grid.tileset.prefabs[index];
					float width = grid.tilePrefab.GetChild(0).GetComponent<Renderer>().bounds.size.x;
					float height = grid.tilePrefab.GetChild(0).GetComponent<Renderer>().bounds.size.y;

					grid.width = width;
					grid.height = height;
				}
			}
		}
	}

	private float CreateSlider(string labelName, float sliderPosition){
		GUILayout.BeginHorizontal();
		GUILayout.Label("Grid " + labelName);
		sliderPosition = EditorGUILayout.Slider (sliderPosition, 1f, 100f, null);
		GUILayout.EndHorizontal();

		return sliderPosition;
	}

	void OnSceneGUI(){
		int controlid = GUIUtility.GetControlID (FocusType.Passive); 
		Event e = Event.current;
		Ray ray = Camera.current.ScreenPointToRay (new Vector3 (e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
		Vector3 mousePos = ray.origin;
		if (e.isMouse && e.type == EventType.MouseDown) {
			GUIUtility.hotControl = controlid;
			e.Use();

			GameObject gameObject;
			Transform prefab = grid.tilePrefab;

			if(prefab){
				Undo.IncrementCurrentGroup();
				gameObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab.gameObject);
				Vector3 aligned = new Vector3(Mathf.Floor(mousePos.x/grid.width)*grid.width + (grid.width/2.0f),Mathf.Floor(mousePos.y/grid.height)*grid.height + (grid.height/2.0f),0.0f);
				gameObject.transform.position = aligned;
				gameObject.transform.parent = grid.transform;
				Undo.RegisterCreatedObjectUndo(gameObject,"Create " + gameObject.name);
				                              
			}
		}
		if (e.isMouse && e.type == EventType.MouseUp) {
			GUIUtility.hotControl = 0;
		}
	}

}
