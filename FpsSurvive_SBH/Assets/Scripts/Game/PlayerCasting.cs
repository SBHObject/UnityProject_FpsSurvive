using UnityEngine;
using TMPro;
using FpsSurvive.Player;

namespace FpsSurvive.Game
{
    public class PlayerCasting : MonoBehaviour
    {
        #region Variables
        private PlayerNewInput playerInput;
        public static float distanceToTarget;

        private float maxDistance = 2f;

        public LayerMask interactiveLayer = -1;

        public TextMeshProUGUI actionTextUI;
        #endregion

        private void Start()
        {
            playerInput = GetComponentInParent<PlayerNewInput>();
        }

        private void Update()
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, maxDistance, interactiveLayer) )
            {
                distanceToTarget = hit.distance;
                Interactive interactive = hit.collider.GetComponent<Interactive>();
                if (interactive)
                {
                    actionTextUI.text = interactive.SetActionText();
                    if(playerInput.GetInteractive())
                    {
                        interactive.DoAction();
                    }
                }
            }
            else
            {
                actionTextUI.text = "";
            }
        }
    }
}