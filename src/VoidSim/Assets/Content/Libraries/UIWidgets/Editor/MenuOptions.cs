using UnityEditor;

namespace UIWidgets.TMProSupport {
	public static class MenuOptions {
		#region Collections
		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Collections/Combobox", false, 1000)]
		public static void CreateCombobox()
		{
			Utilites.CreateWidgetFromAsset("ComboboxTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Collections/ComboboxIcons", false, 1010)]
		public static void CreateComboboxIcons()
		{
			Utilites.CreateWidgetFromAsset("ComboboxIconsTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Collections/ComboboxIconsMultiselect", false, 1020)]
		public static void CreateComboboxIconsMultiselect()
		{
			Utilites.CreateWidgetFromAsset("ComboboxIconsMultiselectTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Collections/DirectoryTreeView", false, 1030)]
		public static void CreateDirectoryTreeView()
		{
			Utilites.CreateWidgetFromAsset("DirectoryTreeViewTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Collections/FileListView", false, 1040)]
		public static void CreateFileListView()
		{
			Utilites.CreateWidgetFromAsset("FileListViewTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Collections/ListView", false, 1050)]
		public static void CreateListView()
		{
			Utilites.CreateWidgetFromAsset("ListViewTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Collections/ListViewInt", false, 1060)]
		public static void CreateListViewInt()
		{
			Utilites.CreateWidgetFromAsset("ListViewIntTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Collections/ListViewHeight", false, 1070)]
		public static void CreateListViewHeight()
		{
			Utilites.CreateWidgetFromAsset("ListViewHeightTMPro");
		}

		//[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Collections/ListViewGameObjects", false, 1080)]
		public static void CreateListViewGameObjects()
		{
			Utilites.CreateWidgetFromAsset("ListViewGameObjects");
		}
		
		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Collections/ListViewIcons", false, 1090)]
		public static void CreateListViewIcons()
		{
			Utilites.CreateWidgetFromAsset("ListViewIconsTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Collections/ListViewPaginator", false, 1100)]
		public static void CreateListViewPaginator()
		{
			Utilites.CreateWidgetFromAsset("ListViewPaginatorTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Collections/TreeView", false, 1110)]
		public static void CreateTreeView()
		{
			Utilites.CreateWidgetFromAsset("TreeViewTMPro");
		}
		#endregion

		#region Containers
		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Containers/Accordion", false, 2000)]
		public static void CreateAccordion()
		{
			Utilites.CreateWidgetFromAsset("AccordionTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Containers/Tabs", false, 2010)]
		public static void CreateTabs()
		{
			Utilites.CreateWidgetFromAsset("TabsTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Containers/TabsLeft", false, 2020)]
		public static void CreateTabsLeft()
		{
			Utilites.CreateWidgetFromAsset("TabsLeftTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Containers/TabsIcons", false, 2030)]
		public static void CreateTabsIcons()
		{
			Utilites.CreateWidgetFromAsset("TabsIconsTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Containers/TabsIconsLeft", false, 2040)]
		public static void CreateTabsIconsLeft()
		{
			Utilites.CreateWidgetFromAsset("TabsIconsLeftTMPro");
		}
		#endregion

		#region Dialogs
		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Dialogs/DatePicker", false, 3000)]
		public static void CreateDatePicker()
		{
			Utilites.CreateWidgetFromAsset("DatePickerTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Dialogs/Dialog Template", false, 3010)]
		public static void CreateDialog()
		{
			Utilites.CreateWidgetFromAsset("DialogTemplateTMPro");
		}

		//!!!
		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Dialogs/FileDialog", false, 3020)]
		public static void CreateFileDialog()
		{
			Utilites.CreateWidgetFromAsset("FileDialogTMPro");
		}

		//!!!
		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Dialogs/FolderDialog", false, 3030)]
		public static void CreateFolderDialog()
		{
			Utilites.CreateWidgetFromAsset("FolderDialogTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Dialogs/Notify Template", false, 3040)]
		public static void CreateNotify()
		{
			Utilites.CreateWidgetFromAsset("NotifyTemplateTMPro");
		}

		//!!!
		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Dialogs/PickerBool", false, 3050)]
		public static void CreatePickerBool()
		{
			Utilites.CreateWidgetFromAsset("PickerBoolTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Dialogs/PickerIcons", false, 3060)]
		public static void CreatePickerIcons()
		{
			Utilites.CreateWidgetFromAsset("PickerIconsTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Dialogs/PickerInt", false, 3070)]
		public static void CreatePickerInt()
		{
			Utilites.CreateWidgetFromAsset("PickerIntTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Dialogs/PickerString", false, 3080)]
		public static void CreatePickerString()
		{
			Utilites.CreateWidgetFromAsset("PickerStringTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Dialogs/Popup", false, 3090)]
		public static void CreatePopup()
		{
			Utilites.CreateWidgetFromAsset("PopupTMPro");
		}
		#endregion

		#region Input
		//!!!
		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Input/Autocomplete", false, 3980)]
		public static void CreateAutocomplete()
		{
			Utilites.CreateWidgetFromAsset("AutocompleteTMPro");
		}

		//!!!
		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Input/AutocompleteIcons", false, 3990)]
		public static void CreateAutocompleteIcons()
		{
			Utilites.CreateWidgetFromAsset("AutocompleteIconsTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Input/ButtonBig", false, 4000)]
		public static void CreateButtonBig()
		{
			Utilites.CreateWidgetFromAsset("ButtonBigTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Input/ButtonSmall", false, 4010)]
		public static void CreateButtonSmall()
		{
			Utilites.CreateWidgetFromAsset("ButtonSmallTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Input/Calendar", false, 4020)]
		public static void CreateCalendar()
		{
			Utilites.CreateWidgetFromAsset("CalendarTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Input/CenteredSlider", false, 4030)]
		public static void CreateCenteredSlider()
		{
			Utilites.CreateWidgetFromAsset("CenteredSlider");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Input/CenteredSliderVertical", false, 4040)]
		public static void CreateCenteredSliderVertical()
		{
			Utilites.CreateWidgetFromAsset("CenteredSliderVertical");
		}

				[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Input/ColorPicker", false, 4050)]
		public static void CreateColorPicker()
		{
			Utilites.CreateWidgetFromAsset("ColorPicker");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Input/ColorPickerRange", false, 4060)]
		public static void CreateColorPickerRange()
		{
			Utilites.CreateWidgetFromAsset("ColorPickerRange");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Input/RangeSlider", false, 4070)]
		public static void CreateRangeSlider()
		{
			Utilites.CreateWidgetFromAsset("RangeSlider");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Input/RangeSliderFloat", false, 4080)]
		public static void CreateRangeSliderFloat()
		{
			Utilites.CreateWidgetFromAsset("RangeSliderFloat");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Input/RangeSliderVertical", false, 4090)]
		public static void CreateObject()
		{
			Utilites.CreateWidgetFromAsset("RangeSliderVertical");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Input/RangeSliderFloatVertical", false, 4100)]
		public static void CreateRangeSliderFloatVertical()
		{
			Utilites.CreateWidgetFromAsset("RangeSliderFloatVertical");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Input/Spinner", false, 4110)]
		public static void CreateSpinner()
		{
			Utilites.CreateWidgetFromAsset("Spinner");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Input/SpinnerFloat", false, 4120)]
		public static void CreateSpinnerFloat()
		{
			Utilites.CreateWidgetFromAsset("SpinnerFloat");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Input/Switch", false, 4130)]
		public static void CreateSwitch()
		{
			Utilites.CreateWidgetFromAsset("Switch");
		}
		#endregion

		//!!!
		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/AudioPlayer", false, 5000)]
		public static void CreateAudioPlayer()
		{
			Utilites.CreateWidgetFromAsset("AudioPlayer");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/Progressbar", false, 5010)]
		public static void CreateProgressbar()
		{
			Utilites.CreateWidgetFromAsset("ProgressbarTMPro");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/ScrollRectPaginator", false, 5020)]
		public static void CreateScrollRectPaginator()
		{
			Utilites.CreateWidgetFromAsset("ScrollRectPaginator");
		}

		[MenuItem("GameObject/UI/UIWidgets with TextMeshPro/ScrollRectNumericPaginator", false, 5030)]
		public static void CreateScrollRectNumericPaginator()
		{
			Utilites.CreateWidgetFromAsset("ScrollRectNumericPaginatorTMPro");
		}
	}
}