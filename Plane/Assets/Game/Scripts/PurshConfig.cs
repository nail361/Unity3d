using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Soomla.Store {
	
	public class PurshConfig : IStoreAssets{
		
		public int GetVersion() {
			return 3;
		}
		
		public VirtualCurrency[] GetCurrencies() {
			return new VirtualCurrency[]{};
		}
		
		public VirtualGood[] GetGoods() {
			return new VirtualGood[] {POINTS};
		}
		
		public VirtualCurrencyPack[] GetCurrencyPacks() {
			return new VirtualCurrencyPack[] {};
		}
		
		public VirtualCategory[] GetCategories() {
			return new VirtualCategory[]{};
		}
		
		public const string POINTS_ID = "buy_points";
		
		public static VirtualGood POINTS = new SingleUseVG(
			"More points", // name
			"Give you 1000 points", // description
			"buy_1000_points", // item id
			new Soomla.Store.PurchaseWithMarket( POINTS_ID, 1 )
			);
		
	}
}