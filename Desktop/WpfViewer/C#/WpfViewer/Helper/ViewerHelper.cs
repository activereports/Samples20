using System;
using System.IO;

namespace ActiveReports.Samples.WPFViewer
{
	internal static class ViewerHelper
	{
		public static bool IsRpx(FileInfo fileName)
		{
			var extension = Path.GetExtension(fileName.FullName);
			return ".rpx".Equals(extension, StringComparison.InvariantCultureIgnoreCase);
		}

		public static bool IsRdf(FileInfo fileName)
		{
			var extension = Path.GetExtension(fileName.FullName);
			return ".rdf".Equals(extension, StringComparison.InvariantCultureIgnoreCase);
		}
		
		public static bool IsSnap(FileInfo fileName)
		{
			var extension = Path.GetExtension(fileName.FullName);
			return ".rdlx-snap".Equals(extension, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
