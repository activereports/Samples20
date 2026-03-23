using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using GrapeCity.ActiveReports.Extensibility.Rendering;
using GrapeCity.ActiveReports.Extensibility.Rendering.Components;
using GrapeCity.ActiveReports.Drawing;
using GrapeCity.ActiveReports.Rendering;
using GrapeCity.ActiveReports.Rendering.Export;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using GrapeCity.ActiveReports.Drawing.Core;

namespace ActiveReports.Samples.Export.Rendering.PdfSharp
{
	/// <summary>
	/// PDF generator based on PDFsharp: http://www.pdfsharp.net/
	/// </summary>
	internal sealed class PdfGenerator : IGenerator, IDisposable
	{
		private static readonly ITextMetricsProvider GcMetricsProvider = new TextMetricsProvider(FontsFactory.Instance, new TextMetricsProvider.TextSettings());
		private static readonly IRenderersFactory GcRenderersFactory = new DocRenderersFactory();

		private readonly Stream _outputStream;

		private PdfDocument _document;
		private readonly IList<XGraphics> _graphics = new List<XGraphics>();
		private readonly PdfImagesFactory _images;
		private readonly PdfFontsFactory _fonts;
		private PdfInteractivityRenderer _interactivityRenderer;

		public PdfGenerator(Stream outputStream, bool embedFonts)
		{
			_outputStream = outputStream;
			_images = new PdfImagesFactory();
			_fonts = new PdfFontsFactory(embedFonts);
		}

		void IDisposable.Dispose()
		{
			foreach (var g in _graphics)
			{
				Debug.Assert(g.GraphicsStateLevel == 0);
				g.Dispose();
			}
			_graphics.Clear();
			if (_document != null)
			{
				_document.Save(_outputStream);
				_document.Dispose();
			}
			_document = null;
			((IDisposable)_images).Dispose();
		}

		public ITextMetricsProvider MetricsProvider => GcMetricsProvider;

		public IRenderersFactory RenderersFactory => GcRenderersFactory;

		public IInteractivityRenderer InteractivityRenderer => _interactivityRenderer;

		void IGenerator.Init(IReport report)
		{
			_document = new PdfDocument { Version = 17 };
			_interactivityRenderer = new PdfInteractivityRenderer(_document, _graphics);
		}

		IDrawingCanvas IGenerator.NewPage(int pageNumber, SizeF pageSize)
		{
			var page = new PdfPage(_document);
			page.Width = new XUnit(pageSize.Width / PdfConverter.TwipsPerPoint, XGraphicsUnit.Point);
			page.Height = new XUnit(pageSize.Height / PdfConverter.TwipsPerPoint, XGraphicsUnit.Point);
			_document.AddPage(page);
			var graphics = XGraphics.FromPdfPage(page);
			_graphics.Add(graphics);
			return new PdfContentGenerator(graphics, _images, _fonts);
		}
	}
}
