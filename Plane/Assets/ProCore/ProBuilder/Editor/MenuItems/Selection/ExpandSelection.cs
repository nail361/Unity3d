using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ProBuilder2.Common;
using ProBuilder2.MeshOperations;
using ProBuilder2.Math;

public class ExpandSelection : Editor
{

	// [MenuItem("Tools/" + pb_Constant.PRODUCT_NAME + "/Selection/Grow Selection %&g", true, pb_Constant.MENU_SELECTION + 1)]
	[MenuItem("Tools/" + pb_Constant.PRODUCT_NAME + "/Selection/Grow Selection &g", true, pb_Constant.MENU_SELECTION + 2)]
	[MenuItem("Tools/" + pb_Constant.PRODUCT_NAME + "/Selection/Shrink Selection Plane &g", true, pb_Constant.MENU_SELECTION + 2)]
	public static bool VerifySelectionCommand()
	{
		return pb_Editor.instanceIfExists != null;
	}

	[MenuItem("Tools/" + pb_Constant.PRODUCT_NAME + "/Selection/Grow Selection &g", false, pb_Constant.MENU_SELECTION + 1)]
	public static void MenuGrowSelection()
	{
		pb_Menu_Commands.MenuGrowSelection(pbUtil.GetComponents<pb_Object>(Selection.transforms));
	}

	[MenuItem("Tools/" + pb_Constant.PRODUCT_NAME + "/Selection/Shrink Selection &#g", false, pb_Constant.MENU_SELECTION + 1)]
	public static void MenuShrinkSelection()
	{
		pb_Menu_Commands.MenuShrinkSelection(pbUtil.GetComponents<pb_Object>(Selection.transforms));
	}
}
