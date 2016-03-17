using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class FreeDropZone : DropZone
{
	#region implemented abstract members of DropZone

	public override void OnDrop (PointerEventData eventData)
	{
		Debug.Log("OnDrop Free Drop Zone");
		if(eventData.pointerDrag)
		{
			DragHandler draggedItem = eventData.pointerDrag.GetComponent<DragHandler>();
			if(draggedItem != null && draggedItem.enabled)
				draggedItem.transform.SetParent(transform, true);
		}
	}
	#endregion


}
