using FpsSurvive.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.UI
{
    public class EquipmentUI : MonoBehaviour
    {
		#region Variables
		private Equipment equip;

		public Transform mainWeaponSlotParent;
		public Transform consumWeaponSlotsParent;
		public Transform EquipSlotsParent;
		#endregion

		private void Awake()
		{
			equip = Equipment.Instance;
		}
	}
}