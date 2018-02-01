using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Soomla.Store;

/// <summary>
/// This class defines our game's economy, which includes virtual goods, virtual currencies
/// and currency packs, virtual categories
/// </summary>
public class SoomlaAssets : IStoreAssets {
	
	/** Static Final Members **/
	
	
	
	public const string SOSIGES_X5_ITEM_ID = "sosiges_x5";
	public const string SOSIGES_X5_PRODUCT_ID = "sosiges_x5";
	
	public const string TESTID_ITEM_ID = "testID";
	public const string TESTID_PRODUCT_ID = "android.test.purchased";
	
	public const string FISH_2_ITEM_ID = "fish_2";
	public const string FISH_2_PRODUCT_ID = "fish_2";
	
	public const string FISH_3_ITEM_ID = "fish_3";
	public const string FISH_3_PRODUCT_ID = "fish_3";
	
	public const string FISH_4_ITEM_ID = "fish_4";
	public const string FISH_4_PRODUCT_ID = "fish_4";
	
	
	
	/** Virtual Currencies **/
	
	
	/** Virtual Currency Packs **/
	
	
	/** Virtual Goods **/
	
	public static VirtualGood SOSIGES_X5 = new SingleUseVG(
		"Some fish",					//name
		"Buy 50 fish",				//description
		SOSIGES_X5_ITEM_ID,				//item id
		new PurchaseWithMarket(SOSIGES_X5_PRODUCT_ID, 1)				//the way this virtual good is purchased
	);
	
	public static VirtualGood TESTID = new SingleUseVG(
		"TestPursh",					//name
		"description",				//description
		TESTID_ITEM_ID,				//item id
		new PurchaseWithMarket(TESTID_PRODUCT_ID, 0.99)				//the way this virtual good is purchased
	);
	
	public static VirtualGood FISH_2 = new SingleUseVG(
		"Busket of fish",					//name
		"Buy 150 fish",				//description
		FISH_2_ITEM_ID,				//item id
		new PurchaseWithMarket(FISH_2_PRODUCT_ID, 2)				//the way this virtual good is purchased
	);
	
	public static VirtualGood FISH_3 = new SingleUseVG(
		"Truck of fish",					//name
		"Buy 400 fish",				//description
		FISH_3_ITEM_ID,				//item id
		new PurchaseWithMarket(FISH_3_PRODUCT_ID, 5)				//the way this virtual good is purchased
	);
	
	public static VirtualGood FISH_4 = new SingleUseVG(
		"Sea of fish",					//name
		"Buy 1000 fish",				//description
		FISH_4_ITEM_ID,				//item id
		new PurchaseWithMarket(FISH_4_PRODUCT_ID, 10)				//the way this virtual good is purchased
	);
	
	
	/** Virtual Categories **/
	// The muffin rush theme doesn't support categories, so we just put everything under a general category.
	public static VirtualCategory GENERAL_CATEGORY = new VirtualCategory(
		"General", new List<string>(new string[] {SOSIGES_X5_ITEM_ID, TESTID_ITEM_ID, FISH_2_ITEM_ID, FISH_3_ITEM_ID, FISH_4_ITEM_ID})
	);
	
	/// <summary>
	/// see parent.
	/// </summary>
	public int GetVersion() {
		return 0;
	}
	
	/// <summary>
	/// see parent.
	/// </summary>
	public VirtualCurrency[] GetCurrencies() {
		return new VirtualCurrency[]{};
	}
	
	/// <summary>
	/// see parent.
	/// </summary>
	public VirtualCurrencyPack[] GetCurrencyPacks() {
		return new VirtualCurrencyPack[]{};
	}
	
	/// <summary>
	/// see parent.
	/// </summary>
	public VirtualGood[] GetGoods() {
		return new VirtualGood[]{SOSIGES_X5, TESTID, FISH_2, FISH_3, FISH_4};
	}
	
	/// <summary>
	/// see parent.
	/// </summary>
	public VirtualCategory[] GetCategories() {
		return new VirtualCategory[]{GENERAL_CATEGORY};
	}
	
}
