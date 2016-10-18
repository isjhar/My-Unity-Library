using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace Isjhar.Library
{
	public abstract class DropZone : MonoBehaviour, IDropHandler 
	{
		#region IDropHandler implementation
		public abstract void OnDrop (PointerEventData eventData);
		#endregion
	}
}