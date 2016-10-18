using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace Isjhar.Library
{
	public class SlotDropZone : DropZone 
	{
		#region implemented abstract members of DropZone

		public override void OnDrop (PointerEventData eventData)
		{
			Debug.LogFormat("OnDrop Slot Drop Zone {0} : {1}", eventData.pointerDrag.name, eventData.pointerDrag.tag);
			if(eventData.pointerDrag)
			{
				// spawp image
				DragHandler droppedItemDragHandler = eventData.pointerDrag.GetComponent<DragHandler>();

				if(droppedItemDragHandler != null && droppedItemDragHandler.enabled)
				{
					// if current item exist in this drop zone
					DragHandler myCurrentItem = transform.gameObject.GetComponentInChildren<DragHandler>();
					if(myCurrentItem != null)
					{
						myCurrentItem.transform.SetParent(droppedItemDragHandler.MyOldParent, false);
						ResetRectTransform(myCurrentItem.gameObject);	
					}
						
					droppedItemDragHandler.MyOldParent = transform;
					droppedItemDragHandler.transform.SetParent(transform, false);
					ResetRectTransform(eventData.pointerDrag);
				}
			}
		}

		#endregion

		private void ResetRectTransform(GameObject item)
		{
			RectTransform currentItemRect = item.GetComponent<RectTransform>();
			currentItemRect.anchorMin = new Vector2(0.5f, 0.5f);
			currentItemRect.anchorMax = new Vector2(0.5f, 0.5f);
			currentItemRect.anchoredPosition = new Vector2(0f, 0f);
			currentItemRect.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		}
	}
}