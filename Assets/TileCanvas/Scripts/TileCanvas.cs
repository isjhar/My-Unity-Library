using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Isjhar.TileCanvas
{
	[ExecuteInEditMode]
	public class TileCanvas : MonoBehaviour 
	{
		[SerializeField] private int _tempRow;
		[SerializeField] private int _tempColumn;
		[SerializeField] private Sprite _selectedSprite = null;
		[SerializeField] private GameObject[,] _tiles = null;

		[SerializeField] private int _row = 0;
		[SerializeField] private int _column = 0;

		public Sprite SelectedSprite { get { return _selectedSprite; } }

		public int Row
		{
			get
			{
				return _row;
			}
		}

		public int Column
		{
			get
			{
				return _column;
			}
		}

		public GameObject[,] Tiles
		{
			get
			{
				return _tiles;
			}
			set
			{
				_tiles = value;
			}
		}

		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {

		}

		public void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.cyan;
			if(_column > 0)
				for (int x = 0; x <= _column; x++)
				{
					Gizmos.DrawLine (transform.position + new Vector3(x, 0f, 0f), transform.position + new Vector3(x, _row, 0f));
				}

			if(_row > 0)
				for (int y = 0; y <= _row; y++)
				{
					Gizmos.DrawLine (transform.position + new Vector3(0f, y, 0f), transform.position + new Vector3(_column, y, 0f));
				}
		}

		private void CheckTilesInited()
		{
			if (_tiles == null)
			{
				// Debug.Log ("Tiles Null, Create A New Tiles");
				// Create A New Tiles
				_tiles = new GameObject[_column, _row];
			}
		}

		public GameObject GetTile(Coord coord)
		{
			CheckTilesInited ();
			return _tiles [coord.X, coord.Y];
		}

		public void AddTile(Coord coord, GameObject tile)
		{
			CheckTilesInited ();
			_tiles [coord.X, coord.Y] = tile;
		}

		public void CheckTilesDefined()
		{
			if (_tiles == null)
			{
				_tiles = new GameObject[_row, _column];
				List<GameObject> deletedTiles = new List<GameObject> ();
				foreach (Transform child in transform)
				{
					Coord childCoord = Coord.Parse (child.name);
					try
					{
						_tiles[childCoord.X, childCoord.Y] = child.gameObject;
					}
					catch
					{
						deletedTiles.Add (child.gameObject);
					}
				}
				while (deletedTiles.Count > 0)
				{
					DestroyImmediate (deletedTiles[0]);
					deletedTiles.RemoveAt (0);
				}
			}
		}

		public void RecreateTiles(int row, int column)
		{
			GameObject[,] oldTiles = _tiles;
			_row = row;
			_column = column;
			_tiles = new GameObject[_column, _row];
			// ada tiles versi lama
			if (oldTiles == null)
				return;
			
			int copyBoundX = Mathf.Min (oldTiles.GetLength (0), _tiles.GetLength (0));
			int copyBoundY = Mathf.Min (oldTiles.GetLength (1), _tiles.GetLength (1));
			// reassign versi lama
			for (int x = 0; x < oldTiles.GetLength (0); x++)
			{
				for (int y = 0; y < oldTiles.GetLength (1); y++)
				{
					if (x < copyBoundX && y < copyBoundY)
					{
						_tiles [x, y] = oldTiles [x, y];
					}
					else
					{
						if (oldTiles [x, y] != null)
						{
							DestroyImmediate (oldTiles [x, y].gameObject);
							oldTiles[x, y] = null;
						}
					}
				}
			}
		}

		public struct Coord
		{
			private int _x;
			private int _y;

			public int X { get { return _x; } set { _x = value; } }
			public int Y { get { return _y; } set { _y = value; } }

			public Coord(int x, int y)
			{
				_x = x;
				_y = y;
			}

			public override string ToString ()
			{
				return string.Format ("[Coord: X={0}, Y={1}]", X, Y);
			}

			public static Coord Parse(string format)
			{
				string[] splitResult = format.Split (' ');
				try
				{
					string xString = splitResult[1].Substring (0, splitResult[1].Length - 1);
					string yString = splitResult[2].Substring (0, splitResult[2].Length - 1);
					int xValue = int.Parse (xString.Split ('=')[1]);
					int yValue = int.Parse (yString.Split ('=')[1]);
					return new Coord(xValue, yValue);
				}
				catch(System.Exception)
				{
					return new Coord (-1, -1);
				}
			}
		}
	}
}