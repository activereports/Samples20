using GrapeCity.ActiveReports.Design.Advanced;
using System.Text;

namespace ActiveReports.Samples.TestDesignerPro
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

			string reportName = "../../../../../../Reports/CustomTileProvider.rdlx";
			DesignerForm df = new DesignerForm();
			df.SessionSettingsStorage = new SessionSettingsStorage();
			df.ExportViewerFactory = new ExportViewerFactory();
			df.Load += Df_Load;
			df.LoadReport(reportName);
			Application.Run(df);
		}

		private static void Df_Load(object sender, EventArgs e)
		{
			HelperForm helper = new HelperForm();
			helper.Show();
		}
	}
}
