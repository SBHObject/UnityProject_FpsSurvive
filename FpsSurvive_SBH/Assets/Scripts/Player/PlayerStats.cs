using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FpsSurvive.Utility;

namespace FpsSurvive.Player
{
    public class PlayerStats : PersistantSingleton<PlayerStats>
    {
		#region Variables
		private int gold;

		public int Gold 
		{
			get { return gold; }
			set {  gold = value; } 
		}

		public bool IsDeath { get; set; }

		#endregion
	}
}