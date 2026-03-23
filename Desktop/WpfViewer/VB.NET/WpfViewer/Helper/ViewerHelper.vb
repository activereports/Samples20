Imports System.IO

Public Class ViewerHelper
	Public Shared Function IsRpx(fileName As FileInfo) As Boolean
		Dim extension As Object = Path.GetExtension(fileName.FullName)
		Return ".rpx".Equals(extension)
	End Function

	Public Shared Function IsRdf(fileName As FileInfo) As Boolean
		Dim extension As Object = Path.GetExtension(fileName.FullName)
		Return ".rdf".Equals(extension)
	End Function

	Public Shared Function IsSnap(fileName As FileInfo) As Boolean
		Dim extension As Object = Path.GetExtension(fileName.FullName)
		Return ".rdlx-snap".Equals(extension)
	End Function
End Class
