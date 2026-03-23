using System.Collections.Generic;
using System.Drawing;
using GrapeCity.ActiveReports.Rendering.Export;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace ActiveReports.Samples.Export.Rendering.PdfSharp
{
	internal class PdfInteractivityRenderer: IInteractivityRenderer
	{
		private readonly PdfDocument _document;
		private readonly IDictionary<string, PdfOutline> _outlines = new Dictionary<string, PdfOutline>();
		private readonly IList<XGraphics> _graphics;

		public PdfInteractivityRenderer(PdfDocument document, IList<XGraphics> graphics)
		{
			_document = document;
			_graphics = graphics;
		}
		
		public void AddOutline(string key, string parentKey, int targetPage, RectangleF targetArea, string name)
		{
			var outline = new PdfOutline(name, _document.Pages[targetPage - 1])
			{
				PageDestinationType = PdfPageDestinationType.Xyz,
				Left = targetArea.X / PdfConverter.TwipsPerPoint,
				Top = targetArea.Y / PdfConverter.TwipsPerPoint
			};
			
			if (_outlines.TryGetValue(parentKey, out var parentOutline))
				parentOutline.Outlines.Add(outline);
			else
				_document.Outlines.Add(outline);
			_outlines[key] = outline;
		}

		public void AddBookmark(string key, int sourcePage, RectangleF sourceArea, int targetPage, RectangleF targetArea)
		{
			var sourceRect = _graphics[sourcePage].Transformer.WorldToDefaultPage(XRect.FromLTRB(sourceArea.Left, sourceArea.Top, sourceArea.Right, sourceArea.Bottom));
			_document.Pages[sourcePage].AddDocumentLink(new PdfRectangle(sourceRect), targetPage);
		}

		public void UrlGoTo(string link, int sourcePage, RectangleF sourceArea)
		{
			_document.Pages[sourcePage - 1].AddWebLink(new PdfRectangle(PdfConverter.Convert(sourceArea)), link);
		}

		public void DrillthroughGoTo(string reportName, IDictionary<string, object> parameters, int sourcePage, RectangleF sourceArea)
		{ }

		public void AddToggle(string key, int sourcePage, RectangleF sourceArea)
		{ }

		public void AddSorting(string key, int sourcePage, RectangleF sourceArea)
		{ }
	}
}