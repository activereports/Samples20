using GrapeCity.ActiveReports.Design.Advanced;
using System.Text;

namespace ActiveReports.Samples.Radar
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
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

			string _reportName = @"..\..\..\..\..\..\Report\Radar.rdlx";
			DesignerForm df = new DesignerForm();
			df.SessionSettingsStorage = new SessionSettingsStorage();
			df.ExportViewerFactory = new ExportViewerFactory();
			df.LoadReport(_reportName);
			Application.Run(df);
		}
	}
}
