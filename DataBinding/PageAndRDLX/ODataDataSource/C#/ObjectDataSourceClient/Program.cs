using System.Text;

namespace ActiveReports.Samples.ODataDataSource
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
			Application.Run(new MainForm());
		}
	}
}
