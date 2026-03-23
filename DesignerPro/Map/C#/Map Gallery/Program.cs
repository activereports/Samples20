

using System.Text;

namespace ActiveReports.Samples.MapGallery
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

#if NET6_0_OR_GREATER
			ApplicationConfiguration.Initialize();
#else
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
#endif

			Application.Run(new ReportsForm());
		}
	}
}
