using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.UI
{
    public class InventoryUI : ItemUI
    {
		#region Variables
		public Transform invenSlotPerant;

		private ItemSlot[] invenSlots;
		#endregion

		private void Start()
		{
			invenSlots = invenSlotPerant.GetComponentsInChildren<ItemSlot>();
		}

		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.I))
			{
				Toggle();
			}
		}
	}
}