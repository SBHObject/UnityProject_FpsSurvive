using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FpsSurvive.Player
{
    [CreateAssetMenu(fileName = "New ItemDatabase", menuName = "Inventory/Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        public ItemObject[] itemObjects;

		public void OnValidate()
		{
			for(int i = 0; i < itemObjects.Length; i++)
			{
				itemObjects[i].data.itemId = i;
			}
		}
	}
}