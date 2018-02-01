using UnityEditor;
using UnityEngine;
using ProBuilder2.MeshOperations;
using System.Collections.Generic;
using System.Linq;
using ProBuilder2.Common;

namespace ProBuilder2.Actions
{
	public class EdgeSelection : Editor
	{

		[MenuItem("Tools/" + pb_Constant.PRODUCT_NAME + "/Selection/Edge Ring &r")]
		public static void MenuEdgeRing()
		{
			pb_Menu_Commands.MenuRingSelection(pbUtil.GetComponents<pb_Object>(Selection.transforms));
		}

		[MenuItem("Tools/" + pb_Constant.PRODUCT_NAME + "/Selection/Edge Loop &l")]
		public static void MenuEdgeLoop()
		{
			pb_Menu_Commands.MenuLoopSelection(pbUtil.GetComponents<pb_Object>(Selection.transforms));
		}
	}
}