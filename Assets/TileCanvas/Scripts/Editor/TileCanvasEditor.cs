using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Isjhar.TileCanvas
{
	[CustomEditor(typeof(TileCanvas))]
	public class TileCanvasEditor : Editor 
	{
		public enum MouseState
		{
			Down, Idle
		}

		public enum PaintState
		{
			Draw, Erase
		}

		public enum AltState
		{
			Idle, Hold
		}

		private TileCanvas _targetScript;
		private TileCanvas.Coord _currentMousePositionCoord;
		private MouseState _mouseState;
		private AltState _altState;
		private SerializedProperty _tempRowProp;
		private SerializedProperty _tempColumnProp;
		private SerializedProperty _rowProp;
		private SerializedProperty _columnProp;
		private SerializedProperty _selectedSpriteProp;
		private bool isDirty = false;
		private Sprite _drawSprite;

		[SerializeField] private PaintState _paintState;


		private void OnEnable()
		{
			_targetScript = (TileCanvas) target;
			_currentMousePositionCoord = new TileCanvas.Coord ();
			_mouseState = MouseState.Idle;
			_altState = AltState.Idle;
			_paintState = PaintState.Draw;

			_tempRowProp = serializedObject.FindProperty ("_tempRow");
			_tempColumnProp = serializedObject.FindProperty ("_tempColumn");
			_selectedSpriteProp = serializedObject.FindProperty ("_selectedSprite");
			_rowProp = serializedObject.FindProperty ("_row");
			_columnProp = serializedObject.FindProperty ("_column");

			if (_paintState == PaintState.Draw)
			{
				_drawSprite = _targetScript.SelectedSprite;
			}
			else if (_paintState == PaintState.Erase)
			{
				_drawSprite = null;
			}
			_targetScript.CheckTilesDefined ();
		}

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (_selectedSpriteProp);
			if (EditorGUI.EndChangeCheck ())
			{
				_drawSprite = (Sprite)_selectedSpriteProp.objectReferenceValue;
			}
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (_tempRowProp, new GUIContent ("Row"));
			EditorGUILayout.PropertyField (_tempColumnProp, new GUIContent ("Column"));	
			if (EditorGUI.EndChangeCheck ())
			{
				if (_tempRowProp.intValue != _targetScript.Row || _tempColumnProp.intValue != _targetScript.Column)
					isDirty = true;
				else
					isDirty = false;
			}

			GUI.enabled = isDirty;
			if (GUILayout.Button ("Apply Changed"))
			{
				_targetScript.RecreateTiles (_tempRowProp.intValue, _tempColumnProp.intValue);
				SceneView.RepaintAll ();
				isDirty = false;
			}
			GUI.enabled = true;
			EditorGUILayout.LabelField ("Draw Mode", EditorStyles.boldLabel);
			GUI.enabled = !(_paintState == PaintState.Draw);
			if (GUILayout.Button ("Draw"))
			{
				_drawSprite = _targetScript.SelectedSprite;
				_paintState = PaintState.Draw;
			}

			GUI.enabled = !(_paintState == PaintState.Erase);
			if (GUILayout.Button ("Erase"))
			{
				_drawSprite = null;
				_paintState = PaintState.Erase;
			}

			serializedObject.ApplyModifiedProperties ();
		}

		private void OnSceneGUI()
		{
			Event current = Event.current;
			int controlId = GUIUtility.GetControlID (FocusType.Passive);
			Vector2 mousePosition = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition).origin;
			TileCanvas.Coord selectedCoord = GetCoordByMousePosition (mousePosition);

			// get input alt
			if (current.isKey)
			{
				if (current.type == EventType.KeyDown && current.keyCode == KeyCode.LeftAlt && _altState == AltState.Idle)
				{
					_altState = AltState.Hold;
				}	
				else if (current.type == EventType.keyUp && current.keyCode == KeyCode.LeftAlt && _altState == AltState.Hold)
				{
					_altState = AltState.Idle;
				}	
			}
			if (_altState == AltState.Hold)
				return;
			
			switch (current.type)
			{
				case EventType.MouseMove:
					break;
				case EventType.MouseDrag:
					if (_mouseState == MouseState.Down)
					{
						if (IsCoordOutOfBound (selectedCoord))
						{
							// Fail Paint, Out Of Bound
							// Debug.LogErrorFormat ("Fail Paint Tile At Coord {0}, Out of Bound", selectedCoord);	
							return;
						}

						DrawTile (selectedCoord, _drawSprite);
						Event.current.Use ();
					}
					break;
				case EventType.MouseDown:
					if (Event.current.isMouse && Event.current.type == EventType.MouseDown)
					{
						_mouseState = MouseState.Down;

						if (IsCoordOutOfBound (selectedCoord))
						{
							// Fail Paint Tile Out Of Bound
							// Debug.LogErrorFormat ("Fail Paint Tile At Coord {0}, Out of Bound", selectedCoord);	
							return;
						}

						DrawTile (selectedCoord, _drawSprite);
						Event.current.Use ();
					}
					break;
				case EventType.MouseUp:
					if (_mouseState == MouseState.Down)
					{
						_mouseState = MouseState.Idle;
					}
					break;
				case EventType.Layout:
					HandleUtility.AddDefaultControl (controlId);
					break;
			}
		}

		private TileCanvas.Coord GetCoordByMousePosition(Vector2 mousePosition)
		{
			Vector2 direction = mousePosition - (Vector2) _targetScript.transform.position;
			_currentMousePositionCoord.X = Mathf.CeilToInt (direction.x) - 1;
			_currentMousePositionCoord.Y = Mathf.CeilToInt (direction.y) - 1;
			return _currentMousePositionCoord;
		}

		private Vector3 GetPositionByCoord(TileCanvas.Coord coord)
		{
			return new Vector3 (coord.X + 0.5f, coord.Y + 0.5f, _targetScript.transform.position.z);
		}

		private bool IsCoordOutOfBound(TileCanvas.Coord coord)
		{
			return coord.X < 0 || coord.Y < 0 || coord.X >= _targetScript.Column || coord.Y >= _targetScript.Row;
		}

		private void DrawTile(TileCanvas.Coord coord, Sprite sprite)
		{
			GameObject tileAtSelectedCoord = _targetScript.GetTile (coord);
			if (sprite != null)
			{
				if (tileAtSelectedCoord != null)
				{
					SpriteRenderer tileSpriteRenderer = tileAtSelectedCoord.GetComponent <SpriteRenderer> ();
					if (tileSpriteRenderer.sprite == sprite)
					{
						// Fail Paint Sprite Same
						// Debug.LogErrorFormat ("Fail Paint Tile At Coord {0}, Sprite Same", coord);	
						return;
					}
					else
					{
						// Replace Sprite
						// Debug.LogFormat ("Replace Sprite At {0}", coord);	
						tileSpriteRenderer.sprite = sprite;
					}
				}
				else
				{
					// Create A New Tile
					// Debug.LogFormat ("Create A New Tile At {0}", coord);	
					tileAtSelectedCoord = new GameObject ();
					tileAtSelectedCoord.name = coord.ToString ();
					tileAtSelectedCoord.transform.position = GetPositionByCoord (coord);
					SpriteRenderer tileSpriteRenderer = tileAtSelectedCoord.AddComponent <SpriteRenderer> ();
					tileSpriteRenderer.sprite = sprite;
					tileAtSelectedCoord.transform.parent = _targetScript.transform;
					_targetScript.AddTile (coord, tileAtSelectedCoord);
				}
			}
			else
			{
				if (tileAtSelectedCoord != null)
				{
					// Delete Tile
					// Debug.LogFormat ("Delete Tile At {0}", coord);
					DestroyImmediate (tileAtSelectedCoord);
					_targetScript.AddTile (coord, null);
				}
			}
		}

		[MenuItem("GameObject/2D Object/Tile Canvas")]
		private static void CreateTileCanvasGameObject()
		{
			GameObject tileCanvasGameObject = new GameObject ("Tile Canvas");
			tileCanvasGameObject.AddComponent <TileCanvas>();
			Selection.activeGameObject = tileCanvasGameObject;
		}
	}
}