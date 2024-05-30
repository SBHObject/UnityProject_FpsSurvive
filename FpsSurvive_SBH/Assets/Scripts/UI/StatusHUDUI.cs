using FpsSurvive.Player;
using FpsSurvive.Weapon;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FpsSurvive.UI
{
    public class StatusHUDUI : MonoBehaviour
    {
        #region Variables
        [Tooltip("Ammo UI")]
        public TextMeshProUGUI currentAmmoText;
        public TextMeshProUGUI invenAmmoText;
        public TextMeshProUGUI slotIndexText;

        [Tooltip("Health Bar UI")]
        public Image healthBar;
        public TextMeshProUGUI healthText;

        private PlayerWeaponManager playerWeaponManager;
        private WeaponController weaponController;
        private Health health;
        #endregion

        private void Start()
        {
            playerWeaponManager = FindObjectOfType<PlayerWeaponManager>();
            health = playerWeaponManager.GetComponent<Health>();

            playerWeaponManager.OnSwitchToWeapon += WeaponSwitch;
            
        }

        private void WeaponSwitch(WeaponController _weaponController)
        {
            weaponController = _weaponController;

            slotIndexText.text = (playerWeaponManager.ActiveWeaponIndex + 1).ToString();
            currentAmmoText.text = weaponController.GetCurrentAmmo().ToString();
            invenAmmoText.text = weaponController.GetCarriedBullets().ToString();
        }

        private void Update()
        {
            if (weaponController != null)
            {
                currentAmmoText.text = weaponController.GetCurrentAmmo().ToString();
                invenAmmoText.text = weaponController.GetCarriedBullets().ToString();
            }
            else
            {
                currentAmmoText.text = "";
                invenAmmoText.text = "";
			}
            UpdateHealthUI();
        }

        private void UpdateHealthUI()
        {
            healthBar.fillAmount = health.GetHealthRatio();
            healthText.text = health.CurrentHealth.ToString() + "/" + health.MaxHealth.ToString();
        }
    }
}