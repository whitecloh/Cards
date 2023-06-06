using OneLine;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Cards.ScriptableObjects
{
	[CreateAssetMenu(fileName = "NewCardPackConfiguration", menuName = "CardConfigs/Card Pack Configuration")]
	public class CardPackConfiguration : ScriptableObject
	{
		private bool _isConstruct;

		public SideType _sideType;

		[SerializeField, OneLine(Header = LineHeader.Short)]
		private CardPropertiesData[] _cards = null;

		public IEnumerable<CardPropertiesData> UnionProperties(IEnumerable<CardPropertiesData> array)
		{
			_isConstruct = false;
			TryToContruct();

			return array.Union(_cards);
		}

		private void TryToContruct()
		{
			if (_isConstruct) return;


			for(int i = 0; i < _cards.Length; i++)
			{
				_cards[i].Cost = (ushort)((_cards[i].Id)/100);
			}

			_isConstruct = true;
		}
	}
}