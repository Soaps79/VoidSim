using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// ListView with InputFields.
	/// </summary>
	public class LVInputFields : ListViewCustom<LVInputFieldsComponent,LVInputFieldsItem>
	{
		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void SelectColoring(LVInputFieldsComponent component)
		{
			//disable select coloring
		}
	}
}