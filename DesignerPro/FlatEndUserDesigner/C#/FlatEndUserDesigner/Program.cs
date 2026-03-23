using GrapeCity.ActiveReports.Design.Advanced;
using System.Text;

namespace ActiveReports.Samples.FlatUserDesigner
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

			var designerForm = new DesignerForm
			{
				ExportViewerFactory = new ExportViewerFactory(),
				SessionSettingsStorage = new SessionSettingsStorage()
			};
			Application.Run(designerForm);
		}
	}
}
