using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FpsSurvive.Player;
using FpsSurvive.Weapon;

namespace FpsSurvive.UI
{
    public class AmmoUI : MonoBehaviour
    {
		#region Variables
		public TextMeshProUGUI currentAmmoText;
		public TextMeshProUGUI invenAmmoText;
		public TextMeshProUGUI slotIndexText;

		private PlayerWeaponManager playerWeaponManager;
		private WeaponController weaponController;
		#endregion

		private void Start()
		{
			playerWeaponManager = FindObjectOfType<PlayerWeaponManager>();

			playerWeaponManager.OnSwitchToWeapon += WeaponSwitch;
		}

		private void WeaponSwitch(WeaponController _weaponController)
		{
			weaponController = _weaponController;

			slotIndexText.text = (playerWeaponManager.ActiveWeaponIndex + 1).ToString();
			currentAmmoText.text = weaponController.GetCurrentAmmo().ToString();
			//임시, 이후 인벤토리와 연동 필요
			invenAmmoText.text = weaponController.GetCarriedBullets().ToString();
		}

		private void Update()
		{
			currentAmmoText.text = weaponController.GetCurrentAmmo().ToString();
			//임시 지정
			invenAmmoText.text = weaponController.GetCarriedBullets().ToString();
		}
	}
}