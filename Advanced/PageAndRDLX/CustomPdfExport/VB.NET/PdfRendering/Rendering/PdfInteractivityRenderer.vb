Imports System.Drawing
Imports GrapeCity.ActiveReports.Rendering.Export
Imports PdfSharp.Drawing
Imports PdfSharp.Pdf

Friend Class PdfInteractivityRenderer
    Implements IInteractivityRenderer

    Private ReadOnly _document As PdfDocument
    Private ReadOnly _outlines As IDictionary(Of String, PdfOutline) = New Dictionary(Of String, PdfOutline)()
    Private ReadOnly _graphics As IList(Of XGraphics)

    Public Sub New(document As PdfDocument, graphics As IList(Of XGraphics))
        _document = document
        _graphics = graphics
    End Sub

    Public Sub AddOutline(key As String, parentKey As String, targetPage As Integer, targetArea As RectangleF, name As String) Implements IInteractivityRenderer.AddOutline
        Dim outline As New PdfOutline(name, _document.Pages(targetPage - 1)) With {
            .PageDestinationType = PdfPageDestinationType.Xyz,
            .Left = targetArea.X / PdfConverter.TwipsPerPoint,
            .Top = targetArea.Y / PdfConverter.TwipsPerPoint
        }

        Dim parentOutline As PdfOutline = Nothing
        If _outlines.TryGetValue(parentKey, parentOutline) Then
            parentOutline.Outlines.Add(outline)
        Else
            _document.Outlines.Add(outline)
        End If
        _outlines(key) = outline
    End Sub

    Public Sub AddBookmark(key As String, sourcePage As Integer, sourceArea As RectangleF, targetPage As Integer, targetArea As RectangleF) Implements IInteractivityRenderer.AddBookmark
        Dim sourceRect As XRect = _graphics(sourcePage).Transformer.WorldToDefaultPage(XRect.FromLTRB(sourceArea.Left, sourceArea.Top, sourceArea.Right, sourceArea.Bottom))
        _document.Pages(sourcePage).AddDocumentLink(New PdfRectangle(sourceRect), targetPage)
    End Sub

    Public Sub UrlGoTo(link As String, sourcePage As Integer, sourceArea As RectangleF) Implements IInteractivityRenderer.UrlGoTo
        _document.Pages(sourcePage - 1).AddWebLink(New PdfRectangle(PdfConverter.Convert(sourceArea)), link)
    End Sub

    Public Sub DrillthroughGoTo(reportName As String, parameters As IDictionary(Of String, Object), sourcePage As Integer, sourceArea As RectangleF) Implements IInteractivityRenderer.DrillthroughGoTo
    End Sub

    Public Sub AddToggle(key As String, sourcePage As Integer, sourceArea As RectangleF) Implements IInteractivityRenderer.AddToggle
    End Sub

    Public Sub AddSorting(key As String, sourcePage As Integer, sourceArea As RectangleF) Implements IInteractivityRenderer.AddSorting
    End Sub
End Class