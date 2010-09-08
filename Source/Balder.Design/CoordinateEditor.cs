using System.Windows;
using Balder.Design.Controls;
using Microsoft.Windows.Design.PropertyEditing;

namespace Balder.Design
{
	
	public class CoordinateEditor : PropertyValueEditor
	{
		public CoordinateEditor(DataTemplate dataTemplate)
			: base(dataTemplate)
		{
			
		}

		public CoordinateEditor()
		{
			var coordinateEditControl = new FrameworkElementFactory(typeof (CoordinateEditorControl));
			var dataTemplate = new DataTemplate {VisualTree = coordinateEditControl};
			InlineEditorTemplate = dataTemplate;
		}
	}
}
