using System.Text;


namespace ActiveReports.Samples.Viewer
{
    static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

#if NET6_0_OR_GREATER
			ApplicationConfiguration.Initialize();
#else
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
#endif

			var applicationForm = new ViewerForm();

			if (args.Length > 0 && !string.IsNullOrEmpty(args[0]))
			{
				var file = new FileInfo(args[0]);
				if (file.Exists)
				{
					applicationForm.LoadDocument(file);
				}
			}


			Application.Run(applicationForm);
		}
	}
}
