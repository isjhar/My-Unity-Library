using UnityEngine;
using System.Collections;

namespace UnityEngine.EventSystems
{
	public abstract class DropZone : MonoBehaviour, IDropHandler 
	{
		#region IDropHandler implementation
		public abstract void OnDrop (PointerEventData eventData);
		#endregion
	}
}