using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace Isjhar.Library
{
	[RequireComponent(typeof(CanvasGroup))]
	public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{

		#region Non Static Field
		// non static field
		private Vector2 _startPosition;
		private Vector2 _offset;
		private Transform _myOldParent;
		private CanvasGroup _myCanvasGroup;


		private const string _draggingCanvasName = "Drag Canvas";
		#endregion


		#region Static Field
		// static field
		private static Transform _draggingCanvas;
		private static GameObject _itemDragged;
		#endregion

		#region Property
		public Transform MyOldParent
		{
			get
			{
				return _myOldParent;
			}
			set
			{
				_myOldParent = value;
			}
		}

		public static GameObject ItemDragged
		{
			get
			{
				return _itemDragged;
			}
			set
			{
				_itemDragged = value;
			}
		}
		#endregion

		private void Awake()
		{
			_draggingCanvas = GameObject.Find(_draggingCanvasName).transform;
			if(_draggingCanvas == null)
			{
				GameObject canvas = new GameObject();
				canvas.name = _draggingCanvasName;
				canvas.AddComponent<Canvas>();
				_draggingCanvas = canvas.transform;
			}
			_myCanvasGroup = GetComponent<CanvasGroup>();
		}

		/// <summary>
		/// Resets item the position.
		/// </summary>
		public void ResetPosition()
		{
			transform.SetParent(_myOldParent, false);
			transform.gameObject.GetComponent<RectTransform>().anchoredPosition = _startPosition;
		}

		#region IBeginDragHandler implementation
		public void OnBeginDrag (PointerEventData eventData)
		{
			if(_itemDragged == null)
			{
				// define this item dragged
				_itemDragged = gameObject;
				// reset rotation
				_itemDragged.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				// remember start position before drag
				_startPosition = transform.gameObject.GetComponent<RectTransform>().anchoredPosition;
				// remember old parent
				_myOldParent = transform.parent;
				// define offset
				_offset = (Vector2) transform.position - eventData.position;
				// change parent to canvas that responsible for item
				transform.SetParent(_draggingCanvas, false);
				// set block raycast false
				_myCanvasGroup.blocksRaycasts = false;
			}
		}
		#endregion

		#region IDragHandler implementation

		public void OnDrag (PointerEventData eventData)
		{
			if(_itemDragged == gameObject)
				transform.position = eventData.position + _offset;
		}

		#endregion

		#region IEndDragHandler implementation

		public void OnEndDrag (PointerEventData eventData)
		{
			_itemDragged = null;
			_myCanvasGroup.blocksRaycasts = true;
		}

		#endregion
	}
}