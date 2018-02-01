using UnityEngine;
using UnityEditor;
using System.Collections;
using ProBuilder2.MeshOperations;
using ProBuilder2.Common;

namespace ProBuilder2.Actions
{
	public class DetachDeleteFace : Editor
	{
		[MenuItem("Tools/" + pb_Constant.PRODUCT_NAME + "/Geometry/Detach Face Selection", false, pb_Constant.MENU_GEOMETRY + pb_Constant.MENU_GEOMETRY_FACE + 4)]
		public static void MenuDetachFace()
		{
			pb_Menu_Commands.MenuDetachFacesContext(pbUtil.GetComponents<pb_Object>(Selection.transforms));
		}

		[MenuItem("Tools/" + pb_Constant.PRODUCT_NAME + "/Geometry/Delete Face (Backspace)", false, pb_Constant.MENU_GEOMETRY + pb_Constant.MENU_GEOMETRY_FACE + 5)]
		public static void MenuDeleteFace()
		{
			pb_Menu_Commands.MenuDeleteFace(pbUtil.GetComponents<pb_Object>(Selection.transforms));
		}
	}
}