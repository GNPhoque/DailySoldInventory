using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.UI;
using MyBox;
using UnityEngine.Networking.Match;

namespace DailySoldInventory.Patches
{
	/// <summary>
	/// Sample Harmony Patch class. Suggestion is to use one file per patched class
	/// though you can include multiple patch classes in one file.
	/// Below is included as an example, and should be replaced by classes and methods
	/// for your mod.
	/// </summary>
	[HarmonyPatch(typeof(Checkout))]
	internal class ProductScannedPatches
	{
		public static Dictionary<string, int> soldProducts = new Dictionary<string, int>();

		/// <summary>
		/// Patches the Player Awake method with prefix code.
		/// </summary>
		/// <param name="__instance"></param>
		[HarmonyPatch(nameof(Checkout.ProductScanned))]
		[HarmonyPrefix]
		public static bool ProductScanned_Prefix(Product product, Checkout __instance)
		{
			DailySoldItemsSummarySMSMODPlugin.Log.LogInfo("In Player ProductScanned method Prefix.");
			string scannedProduct = product.ProductSO.LocalizedName.GetLocalizedString();
			DailySoldItemsSummarySMSMODPlugin.Log.LogInfo($"Adding {scannedProduct} to daily sold items list.");
			AddProductToSoldList(scannedProduct);
			return true;
		}

		/// <summary>
		/// Patches the Player Awake method with postfix code.
		/// </summary>
		/// <param name="__instance"></param>
		[HarmonyPatch(nameof(Checkout.ProductScanned))]
		[HarmonyPostfix]
		public static void Awake_Postfix(Checkout __instance)
		{
			DailySoldItemsSummarySMSMODPlugin.Log.LogInfo("In Player ProductScanned method Postfix.");
		}

		public static void AddProductToSoldList(string itemName)
		{
			if (soldProducts.ContainsKey(itemName))
			{
				soldProducts[itemName]++;
			}
			else soldProducts.Add(itemName, 1);
		}
	}

	[HarmonyPatch(typeof(DailyStatisticsManager))]
	internal class DailyStatisticsManagerPatch
	{
		static bool uiCreated = false;

		[HarmonyPatch(nameof(DailyStatisticsManager.Start))]
		[HarmonyPrefix]
		public static bool OnDayFinished_Prefix()
		{
			//UI STUFF HERE
			TestUI();
			//CreateDailySellsUI();
			return true;
		}

		static void TestUI()
		{
			GameObject canvasObject = GameObject.Find("Day Cycle Canvas").transform.GetChild(1).gameObject;
			canvasObject.SetActive(true);
			GameObject background = canvasObject.transform.GetChild(3).gameObject;
			for (int i = 5; i < 22; i++)
			{
				canvasObject.transform.GetChild(i).gameObject.SetActive(false);
			}

			// Create ScrollRect
			GameObject scrollRectObject = new GameObject("ScrollRect");
			RectTransform scrollRectTransform = scrollRectObject.AddComponent<RectTransform>();
			scrollRectTransform.SetParent(background.transform, false);
			scrollRectTransform.anchorMin = new Vector2(0, 0);
			scrollRectTransform.anchorMax = new Vector2(1, 1);
			scrollRectTransform.offsetMin = new Vector2(100, 100);
			scrollRectTransform.offsetMax = new Vector2(-100, -100);
			ScrollRect scrollRect = scrollRectObject.AddComponent<ScrollRect>();

			// Create ScrollRect content
			GameObject contentObject = new GameObject("Content");
			RectTransform contentRectTransform = contentObject.AddComponent<RectTransform>();
			contentRectTransform.SetParent(scrollRectObject.transform, false);
			GridLayoutGroup gridLayout = contentObject.AddComponent<GridLayoutGroup>();
			gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
			gridLayout.constraintCount = 2;
			gridLayout.cellSize = new Vector2(300, 100);

			// Calculate content size based on number of rows and up to 2 items per row
			int elementsCount = 20; // Change this to the actual number of rows
			int numRows = Mathf.CeilToInt((float)elementsCount / 2);
			float contentHeight = (elementsCount / 2) * (gridLayout.cellSize.y + gridLayout.spacing.y);
			float contentWidth = gridLayout.constraintCount * (gridLayout.cellSize.x + gridLayout.spacing.x) - gridLayout.spacing.x;
			contentRectTransform.sizeDelta = new Vector2(contentWidth, contentHeight);

			// Adjust ScrollRect properties
			scrollRect.content = contentRectTransform;
			scrollRect.viewport = scrollRectTransform;

			// Create vertical scrollbar
			GameObject verticalScrollbarObject = new GameObject("Scrollbar Vertical");
			RectTransform verticalScrollbarRectTransform = verticalScrollbarObject.AddComponent<RectTransform>();
			verticalScrollbarRectTransform.SetParent(scrollRectObject.transform, false);
			Scrollbar verticalScrollbar = verticalScrollbarObject.AddComponent<Scrollbar>();
			verticalScrollbar.direction = Scrollbar.Direction.BottomToTop;
			verticalScrollbarObject.AddComponent<CanvasRenderer>(); // Needed for Unity versions prior to 2019.3
			scrollRect.verticalScrollbar = verticalScrollbar;
			scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;

			// Create horizontal scrollbar
			GameObject horizontalScrollbarObject = new GameObject("Scrollbar Horizontal");
			RectTransform horizontalScrollbarRectTransform = horizontalScrollbarObject.AddComponent<RectTransform>();
			horizontalScrollbarRectTransform.SetParent(scrollRectObject.transform, false);
			Scrollbar horizontalScrollbar = horizontalScrollbarObject.AddComponent<Scrollbar>();
			horizontalScrollbar.direction = Scrollbar.Direction.LeftToRight;
			horizontalScrollbarObject.AddComponent<CanvasRenderer>(); // Needed for Unity versions prior to 2019.3
			scrollRect.horizontalScrollbar = horizontalScrollbar;
			scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;

			// Instantiate text elements in the grid
			for (int i = 0; i < elementsCount; i++)
			{
				GameObject newTextObject = new GameObject("Text");
				newTextObject.transform.SetParent(contentObject.transform, false);
				TextMeshProUGUI newText = newTextObject.AddComponent<TextMeshProUGUI>();
				newText.text = "Element " + i.ToString();
				newText.alignment = TextAlignmentOptions.Center;
			}
		}

		private static void CreateDailySellsUI()
		{
			if (uiCreated) return;

			//Mock data
			ProductScannedPatches.AddProductToSoldList("test");
			ProductScannedPatches.AddProductToSoldList("test1");
			ProductScannedPatches.AddProductToSoldList("test2");
			ProductScannedPatches.AddProductToSoldList("test3");
			ProductScannedPatches.AddProductToSoldList("test4");
			ProductScannedPatches.AddProductToSoldList("test5");
			ProductScannedPatches.AddProductToSoldList("test6");
			ProductScannedPatches.AddProductToSoldList("test7");
			ProductScannedPatches.AddProductToSoldList("test8");
			ProductScannedPatches.AddProductToSoldList("test9");
			ProductScannedPatches.AddProductToSoldList("test10");
			ProductScannedPatches.AddProductToSoldList("test11");
			ProductScannedPatches.AddProductToSoldList("test11");
			ProductScannedPatches.AddProductToSoldList("test11");
			ProductScannedPatches.AddProductToSoldList("test11");
			ProductScannedPatches.AddProductToSoldList("test11");
			ProductScannedPatches.AddProductToSoldList("test11");
			ProductScannedPatches.AddProductToSoldList("test12");
			ProductScannedPatches.AddProductToSoldList("test13");
			ProductScannedPatches.AddProductToSoldList("test14");
			ProductScannedPatches.AddProductToSoldList("test15");
			ProductScannedPatches.AddProductToSoldList("test16");
			ProductScannedPatches.AddProductToSoldList("test17");
			ProductScannedPatches.AddProductToSoldList("test18");
			ProductScannedPatches.AddProductToSoldList("test19");
			ProductScannedPatches.AddProductToSoldList("test20");
			ProductScannedPatches.AddProductToSoldList("test21");
			ProductScannedPatches.AddProductToSoldList("test22");
			ProductScannedPatches.AddProductToSoldList("test23");
			ProductScannedPatches.AddProductToSoldList("test24");

			DailySoldItemsSummarySMSMODPlugin.Log.LogInfo("Creating daily report UI...");

			//Copy daily summary page
			GameObject dailySummaryUI = GameObject.Find("Day Cycle Canvas").transform.GetChild(1).gameObject;
			GameObject dailySellsUI = GameObject.Instantiate(dailySummaryUI, dailySummaryUI.transform.parent);

			GameObject scrollGO = dailySellsUI.transform.GetChild(3).gameObject;
			GameObject viewportGO = new GameObject("Viewport");
			viewportGO.transform.SetParent(scrollGO.transform);
			GameObject contentGO = new GameObject("Sold Items");
			contentGO.transform.SetParent(viewportGO.transform);

			//RectTransform
			RectTransform rt = viewportGO.AddComponent<RectTransform>();
			rt.pivot = new Vector2(0f, 1f);
			rt.anchorMin = Vector2.zero;
			rt.anchorMax = Vector2.one;
			rt.anchoredPosition = Vector2.zero;

			rt = contentGO.AddComponent<RectTransform>();
			rt.pivot = new Vector2(0f, 1f);
			rt.anchorMin = Vector2.zero;
			rt.anchorMax = Vector2.one;
			rt.anchoredPosition = Vector2.zero;

			//Add scroll
			ScrollRect sr = (ScrollRect)scrollGO.AddComponent(typeof(ScrollRect));
			scrollGO.AddComponent(typeof(Mask));
			sr.viewport = viewportGO.GetComponent<RectTransform>();
			sr.content = contentGO.GetComponent<RectTransform>();
			sr.horizontal = false;

			//Add grid layout in copy
			ContentSizeFitter csf = contentGO.AddComponent<ContentSizeFitter>();
			csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
			GridLayoutGroup grid = (GridLayoutGroup)contentGO.AddComponent(typeof(GridLayoutGroup));
			grid.childAlignment = TextAnchor.UpperCenter;
			grid.cellSize = new Vector2(contentGO.GetComponent<RectTransform>().sizeDelta.x / 2, 100);
			grid.padding = new RectOffset(0, 0, 0, 0);
			grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;

			//Delete unused objects
			int[] childrenToDelete = new int[] { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 };	// 4=title 9=total customer
			for (int i = 0; i < childrenToDelete.Length; i++)
			{
				DailySoldItemsSummarySMSMODPlugin.Log.LogInfo("Destroying item "+i);
				GameObject.Destroy(dailySellsUI.transform.GetChild(childrenToDelete[i]).gameObject);
			}

			//GameObject.Destroy(dailySellsUI.transform.GetChild(9).gameObject.transform.GetChild(0).gameObject);
			//GameObject uiListItem = dailySellsUI.transform.GetChild(9).gameObject;

			TextMeshProUGUI titleText = dailySellsUI.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
			titleText.text = "Items Sold Today";

			GameObject uiListItem = new GameObject("soldProduct", typeof(RectTransform), typeof(TextMeshProUGUI));
			//dailySellsUI.transform.GetChild(9).gameObject.SetActive(false);
			bool textAlignmentRight = true;
			foreach (var item in ProductScannedPatches.soldProducts)
			{
				GameObject inst = GameObject.Instantiate(uiListItem, contentGO.transform);

				//inst.AddComponent<LayoutElement>();

				RectTransform rect = inst.GetComponent<RectTransform>();
				rect.anchoredPosition = Vector2.zero;
				rect.SetWidth(0);
				rect.SetHeight(0);

				TextMeshProUGUI text = inst.GetComponent<TextMeshProUGUI>();
				text.enabled = true;
				text.font = titleText.font; 
				text.fontMaterial = titleText.fontMaterial;
				text.fontSize = 20;
				text.alignment = TextAlignmentOptions.Center;
				text.text = $"{item.Key} : {item.Value}";
				text.enableWordWrapping=false;
				textAlignmentRight = !textAlignmentRight;
			}
			dailySellsUI.SetActive(true);
			uiCreated = true;
		}
	}
}