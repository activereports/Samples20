Imports PdfSharp.Pdf
Imports PdfSharp.Drawing
Imports GrapeCity.ActiveReports.Rendering.Export
Imports GrapeCity.ActiveReports.Rendering
Imports GrapeCity.ActiveReports.Drawing
Imports GrapeCity.ActiveReports.Extensibility.Rendering.Components
Imports GrapeCity.ActiveReports.Extensibility.Rendering
Imports System.IO
Imports System.Drawing
Imports GrapeCity.ActiveReports.Drawing.Core

''' <summary>
''' PDF generator based on PDFsharp: http://www.pdfsharp.net/
''' </summary>
Friend NotInheritable Class PdfGenerator
	Implements IGenerator
	Implements IDisposable
	Private Shared ReadOnly GcMetricsProvider As ITextMetricsProvider = New TextMetricsProvider(FontsFactory.Instance, New TextMetricsProvider.TextSettings())
	Private Shared ReadOnly GcRenderersFactory As IRenderersFactory = New DocRenderersFactory()

	Private ReadOnly _outputStream As Stream

	Private _document As PdfDocument
	Private ReadOnly _graphics As IList(Of XGraphics) = New List(Of XGraphics)()
	Private ReadOnly _images As PdfImagesFactory
	Private ReadOnly _fonts As PdfFontsFactory
	Private _interactivityRenderer As PdfInteractivityRenderer

	Public Sub New(outputStream As Stream, embedFonts As Boolean)
		_outputStream = outputStream

		_images = New PdfImagesFactory()
		_fonts = New PdfFontsFactory(embedFonts)
	End Sub

	Sub Dispose() Implements IDisposable.Dispose
		For Each g As XGraphics In _graphics
			Debug.Assert(g.GraphicsStateLevel = 0)
			g.Dispose()
		Next
		_graphics.Clear()
		If _document IsNot Nothing Then
			_document.Save(_outputStream)
			_document.Dispose()
		End If
		_document = Nothing
		CType(_images, IDisposable).Dispose()
	End Sub

	Public ReadOnly Property MetricsProvider() As ITextMetricsProvider Implements IGenerator.MetricsProvider

		Get
			Return GcMetricsProvider
		End Get
	End Property

	Public ReadOnly Property RenderersFactory() As IRenderersFactory Implements IGenerator.RenderersFactory
		Get
			Return GcRenderersFactory
		End Get
	End Property
	
	Public ReadOnly Property InteractivityRenderer As IInteractivityRenderer Implements IGenerator.InteractivityRenderer
		Get
			Return _interactivityRenderer
		End Get
	End Property

	Sub Init(report As IReport) Implements IGenerator.Init
		_document = New PdfDocument() With {
				.Version = 17
			}
		_interactivityRenderer = New PdfInteractivityRenderer(_document, _graphics)
	End Sub

	Function NewPage(pageNumber As Integer, pageSize As SizeF) As IDrawingCanvas Implements IGenerator.NewPage
		Dim page As New PdfPage(_document)
		page.Width = New XUnit(pageSize.Width / PdfConverter.TwipsPerPoint, XGraphicsUnit.Point)
		page.Height = New XUnit(pageSize.Height / PdfConverter.TwipsPerPoint, XGraphicsUnit.Point)
		_document.AddPage(page)
		Dim graphics = XGraphics.FromPdfPage(page)
		_graphics.Add(graphics)
		Return New PdfContentGenerator(graphics, _images, _fonts)
	End Function
	
End Class
