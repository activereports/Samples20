using GrapeCity.ActiveReports.Design.Advanced;
using System;
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
			string reportName = "../../../../DemoReport.rdlx";
			DesignerForm df = new DesignerForm();
			df.Load += Df_Load;
			df.LoadReport(reportName);
			df.ExportViewerFactory = new ExportViewerFactory();
			df.SessionSettingsStorage = new SessionSettingsStorage();
			Application.Run(df);
		}

		private static void Df_Load(object sender, EventArgs e)
		{
			HelperForm helper = new HelperForm();
			helper.Show();
		}
	}
}
