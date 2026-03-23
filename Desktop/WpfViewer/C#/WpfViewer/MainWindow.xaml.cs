using GrapeCity.ActiveReports;
using GrapeCity.ActiveReports.Data;
using GrapeCity.ActiveReports.PageReportModel;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace ActiveReports.Samples.WPFViewer
{
	public partial class MainWindow : Window
	{
		static readonly string CurrentFileLocation = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\..\..\..\Reports\";

		public MainWindow()
		{
			InitializeComponent();
			Icon = new System.Windows.Media.Imaging.BitmapImage(new Uri("..\\..\\..\\..\\resources\\app.ico", UriKind.Relative));
		}

		private void InitializeCustomCommand(ResourceDictionary resources)
		{
			if (resources == null) throw new ArgumentNullException(nameof(resources));

			resources["MyCommand"] = new MyCommand();
        }

		private void btnPreview_Click(object sender, RoutedEventArgs e)
		{
			ComboBoxItem _report = (ComboBoxItem)CmbListBox.SelectedItem;
			//If the 'Add Custom Button' checkbox is checked load custom menu of Wpf Viewer 
			if (chkCustomButton.IsChecked == true)
			{
				string langDictPath = "DefaultWPFViewerTemplates.xaml";
				Uri langDictUri = new Uri(langDictPath, UriKind.Relative);

                ResourceTheme.Source = langDictUri;
				InitializeCustomCommand(ResourceTheme);
            }
			//If the 'Add Custom Button' checkbox is not checked, clear the resource dictionary values.
			if (chkCustomButton.IsChecked == false)
			{
				ResourceTheme.Clear();
			}

			//Load Selected Report
			LoadReport(new FileInfo(Path.Combine(CurrentFileLocation, _report.Content.ToString())));
		}

		private void LoadReport(FileInfo file)
		{
			try
			{
				if (ViewerHelper.IsRdf(file) || ViewerHelper.IsSnap(file))
				{
					ReportViewer.LoadDocument(file.FullName);
				}
				else if (ViewerHelper.IsRpx(file))
				{
					var sectionReport = new SectionReport();
					sectionReport.LocateCredentials += OnLocateCredentialsSection;
					var settings = new XmlReaderSettings
					{
						DtdProcessing = DtdProcessing.Prohibit,
						XmlResolver = null
					};
					using (var reader = XmlReader.Create(file.FullName, settings))
						sectionReport.LoadLayout(reader, out System.Collections.ArrayList error);
					ReportViewer.LoadDocument(sectionReport);
				}
				else
				{
					var pageReport = new PageReport(file);
					pageReport.Document.LocateCredentials += OnLocateCredentials;
					ReportViewer.LoadDocument(pageReport.Document);
				}
			}
			catch (Exception ex)
			{
				ReportViewer.HandleError(ex);
			}
		}

		private void OnLocateCredentialsSection(object sender, GrapeCity.ActiveReports.Chart.Designer.LocateCredentialsEventArgs args)
		{
			var pageArgs = ToLocateCredentialsEventArgs(args);

			OnLocateCredentials(sender, pageArgs);

			args.UserName = pageArgs.UserName;
			args.Password = pageArgs.Password;
		}

		private LocateCredentialsEventArgs ToLocateCredentialsEventArgs(GrapeCity.ActiveReports.Chart.Designer.LocateCredentialsEventArgs args)
		{
			var dataSource = args.DataSource;
			string connectionString = null;
			string dataProvider = null;
			if (dataSource is DbDataSource)
			{
				connectionString = ((DbDataSource)dataSource).ConnectionString;
				dataProvider = ((DbDataSource)dataSource).ProviderName;
			}
			var pageDataSource = new DataSource()
			{
				Name = args.Name,
				ConnectionProperties = new ConnectionProperties
				{
					Prompt = args.PromptText,
					ConnectString = connectionString,
					DataProvider = dataProvider
				},
			};

			return new LocateCredentialsEventArgs(pageDataSource, args.ReportPath, args.PromptText);
		}

		private void OnLocateCredentials(object sender, LocateCredentialsEventArgs args)
		{
			if (LocateCredentials != null)
				LocateCredentials(sender, args);
			else
			{
				ConnectionProperties connectionProperties = args.DataSource.ConnectionProperties;
				if (connectionProperties == null)
					return;
				var dataSourceName = args.DataSource != null ? args.DataSource.Name : string.Empty;
				string prompt = connectionProperties.Prompt;

				var credential = GetCredentials(new DataSource
				{
					Name = dataSourceName,
					ConnectionProperties = new ConnectionProperties
					{
						Prompt = prompt,
						ConnectString = connectionProperties.ConnectString,
						DataProvider = connectionProperties.DataProvider,
						IntegratedSecurity = connectionProperties.IntegratedSecurity
					},
				});

				args.UserName = credential.UserName;
				args.Password = credential.Password;
			}
		}

		private event LocateCredentialsEventHandler LocateCredentials;

		private struct Credentials
		{
			public string UserName { get; private set; }

			public string Password { get; private set; }

			public Credentials(string name, string password) : this()
			{
				UserName = name;
				Password = password;
			}
		}

		private Credentials GetCredentials(DataSource dataSource)
		{
			Credentials? credentials;
			credentials = new Credentials();

			// Prohibit locate credential if locating credential from designer and credential request's count is more than one.
			// It can be happen on locating credential in subreport.
			string prompt = dataSource.ConnectionProperties.Prompt;
			if (string.IsNullOrEmpty(prompt))
				return credentials ?? new Credentials(string.Empty, string.Empty);

			// Prohibit locate credentials if report is blank or data provider is null
			string dataProvider = dataSource.ConnectionProperties.DataProvider;
			if (string.IsNullOrEmpty(dataProvider))
				return credentials ?? new Credentials(string.Empty, string.Empty);

			credentials = RequestCredentials(dataSource, prompt);

			return credentials.Value;
		}

		private Credentials? RequestCredentials(DataSource dataSource,
			string prompt)
		{
			Credentials credentials = new Credentials(string.Empty, string.Empty);

			Application.Current.Dispatcher.Invoke(() =>
			{
				var credentialsDialog = new CredentialsDialog(dataSource.Name, prompt);
				if (credentialsDialog.ShowDialog() == true)
					credentials = new Credentials(credentialsDialog.UserName, credentialsDialog.Password);
			});
			return credentials;
		}

		private void CmbListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (CmbListBox.SelectedIndex == 0)
			{
				if (chkCustomButton != null)
				{
					chkCustomButton.IsEnabled = false;
				}
				btnPreview.IsEnabled = false;
			}
			else
			{
				chkCustomButton.IsEnabled = true;

				btnPreview.IsEnabled = true;
			}
		}
	}
}
