using FpsSurvive.AnimationParameter;
using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.UI
{
    public class InventoryUI : ItemUI
    {
		#region Variables
		private Inventory inventory;

		public Transform invenSlotPerant;

		private ItemSlot[] invenSlots;

		public ItemInfoUI infoUI;

		private int selectedSlotIndex = -1;
		#endregion

		private void Start()
		{
			inventory = Inventory.Instance;
			inventory.OnItemChanged += UpdateInventoryUI;
			invenSlots = invenSlotPerant.GetComponentsInChildren<ItemSlot>();

			infoUI = GetComponent<ItemInfoUI>();
		}

		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.I))
			{
				Toggle();
			}
		}

        protected override IEnumerator OpenUIAni()
        {
            isOpen = true;
            playerMove.enabled = false;
            playerNewInput.enabled = false;
            weaponManager.enabled = false;
            ani.SetBool(AniParameters.isOpen, true);

            yield return new WaitForSeconds(animatingTimeLenght);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

			infoUI.InfoOpen();
        }

        protected override IEnumerator CloseUIAni()
        {
			infoUI.InfoClose();

			yield return new WaitForSeconds(animatingTimeLenght);

            isOpen = false;
            ani.SetBool(AniParameters.isOpen, false);

            yield return new WaitForSeconds(animatingTimeLenght);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            playerMove.enabled = true;
            playerNewInput.enabled = true;
            weaponManager.enabled = true;
        }

        void UpdateInventoryUI()
		{
			for(int i = 0; i < invenSlots.Length; i++)
			{
				invenSlots[i].ResetItemSlot();
			}

			for(int i = 0; i < invenSlots.Length; i++)
			{
				if( i < inventory.items.Count)
				{
					invenSlots[i].SetItemSlot(inventory.items[i], i);
				}
			}
		}
	}
}