using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace Isjhar.Library
{
	public class ForbidDropZone : DropZone
	{
		#region implemented abstract members of DropZone

		public override void OnDrop (PointerEventData eventData)
		{
			Debug.Log("OnDrop Forbid Drop Zone");
			if(eventData.pointerDrag)
			{
				DragHandler draggedItem = eventData.pointerDrag.GetComponent<DragHandler>();
				if(draggedItem != null && draggedItem.enabled)
					draggedItem.ResetPosition();
			}
		}
		#endregion
	}
}