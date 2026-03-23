Imports System
Imports System.IO
Imports System.Windows
Imports System.Windows.Controls
Imports System.Xml
Imports GrapeCity.ActiveReports
Imports GrapeCity.ActiveReports.Data
Imports GrapeCity.ActiveReports.PageReportModel

Public Class MainWindow

	Dim CurrentFileLocation As String = System.AppDomain.CurrentDomain.BaseDirectory & "..\..\..\..\..\..\Reports\"

	Public Sub New()
		InitializeComponent()
		Icon = New System.Windows.Media.Imaging.BitmapImage(New Uri("..\\..\\..\\..\\resources\\app.ico", UriKind.Relative))
	End Sub

	Private Sub InitializeCustomCommand(resources As ResourceDictionary)
		If resources Is Nothing Then
			Throw New ArgumentNullException(NameOf(resources))
		End If
		resources("MyCommand") = New MyCommand()
	End Sub

	Private Sub btnPreview_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
		Dim _report As ComboBoxItem = DirectCast(CmbListBox.SelectedItem, ComboBoxItem)

		'If the 'Add Custom Button' checkbox is checked load custom menu of Wpf Viewer 
		If chkCustomButton.IsChecked = True Then
			Dim langDictPath As String = "DefaultWPFViewerTemplates.xaml"
			Dim langDictUri As New Uri(langDictPath, UriKind.Relative)
			ResourceTheme.Source = langDictUri
			InitializeCustomCommand(ResourceTheme)
		End If

		'If the 'Add Custom Button' checkbox is not checked, clear the resource dictionary values.
		If chkCustomButton.IsChecked = False Then
			ResourceTheme.Clear()
		End If

		'Load Selected Report
		LoadReport(New FileInfo(Path.Combine(CurrentFileLocation, _report.Content.ToString())))
	End Sub

	Private Sub LoadReport(file As FileInfo)
		Try

			If ViewerHelper.IsRdf(file) OrElse ViewerHelper.IsSnap(file) Then
				ReportViewer.LoadDocument(file.FullName)
			ElseIf ViewerHelper.IsRpx(file) Then
				Dim sectionReport = New SectionReport()
				AddHandler sectionReport.LocateCredentials, AddressOf OnLocateCredentialsSection
				Dim settings = New XmlReaderSettings With {
					.DtdProcessing = DtdProcessing.Prohibit,
					.XmlResolver = Nothing
				}
				Using reader = XmlReader.Create(file.FullName, settings)
					sectionReport.LoadLayout(reader, New System.Collections.ArrayList())
				End Using
				ReportViewer.LoadDocument(sectionReport)
			Else
				Dim pageReport = New PageReport(file)
				AddHandler pageReport.Document.LocateCredentials, AddressOf OnLocateCredentials
				ReportViewer.LoadDocument(pageReport.Document)
			End If
		Catch ex As Exception
			ReportViewer.HandleError(ex)
		End Try
	End Sub

	Private Sub OnLocateCredentialsSection(sender As Object, args As GrapeCity.ActiveReports.Chart.Designer.LocateCredentialsEventArgs)
		Dim pageArgs = ToLocateCredentialsEventArgs(args)

		OnLocateCredentials(sender, pageArgs)

		args.UserName = pageArgs.UserName
		args.Password = pageArgs.Password
	End Sub

	Private Function ToLocateCredentialsEventArgs(args As GrapeCity.ActiveReports.Chart.Designer.LocateCredentialsEventArgs) As LocateCredentialsEventArgs
		Dim dataSource = args.DataSource
		Dim connectionString As String = Nothing
		Dim dataProvider As String = Nothing

		If TypeOf dataSource Is DbDataSource Then
			connectionString = DirectCast(dataSource, DbDataSource).ConnectionString
			dataProvider = DirectCast(dataSource, DbDataSource).ProviderName
		End If

		Dim pageDataSource = New DataSource() With {
		.Name = args.Name,
		.ConnectionProperties = New ConnectionProperties With {
			.Prompt = args.PromptText,
			.ConnectString = connectionString,
			.DataProvider = dataProvider
		}
	}

		Return New LocateCredentialsEventArgs(pageDataSource, args.ReportPath, args.PromptText)
	End Function

	Friend Sub OnLocateCredentials(ByVal sender As Object, ByVal args As LocateCredentialsEventArgs)
		If LocateCredentialsEvent IsNot Nothing Then
			RaiseEvent LocateCredentials(sender, args)
		Else
			Dim connectionProperties As ConnectionProperties = args.DataSource.ConnectionProperties
			If connectionProperties Is Nothing Then
				Return
			End If
			Dim dataSourceName = If(args.DataSource IsNot Nothing, args.DataSource.Name, String.Empty)
			Dim prompt As String = connectionProperties.Prompt

			Dim credential = GetCredentials(New GrapeCity.ActiveReports.PageReportModel.DataSource With {
			.Name = dataSourceName,
			.ConnectionProperties = New ConnectionProperties With {
				.prompt = prompt,
				.ConnectString = connectionProperties.ConnectString,
				.DataProvider = connectionProperties.DataProvider,
				.IntegratedSecurity = connectionProperties.IntegratedSecurity
			}
		})

			args.UserName = credential.UserName
			args.Password = credential.Password
		End If
	End Sub

	Friend Event LocateCredentials As LocateCredentialsEventHandler

	Friend Structure Credentials
		Public Property UserName As String

		Public Property Password As String

		Public Sub New(name As String, password As String)
			Me.UserName = name
			Me.Password = password
		End Sub
	End Structure

	Private Function GetCredentials(dataSource As DataSource) As Credentials
		Dim credentials As Credentials? = New Credentials()

		Dim prompt As String = dataSource.ConnectionProperties.Prompt
		If String.IsNullOrEmpty(prompt) Then
			Return If(credentials, New Credentials(String.Empty, String.Empty))
		End If

		Dim dataProvider As String = dataSource.ConnectionProperties.DataProvider
		If String.IsNullOrEmpty(dataProvider) Then
			Return If(credentials, New Credentials(String.Empty, String.Empty))
		End If

		credentials = RequestCredentials(dataSource, prompt)
		Return credentials.Value
	End Function

	Private Function RequestCredentials(ByVal dataSource As DataSource, ByVal prompt As String) As Credentials?
		Dim credentials As Credentials = New Credentials(String.Empty, String.Empty)
		Dim requestFunction = Sub()
								  Dim credentialsDialog = New CredentialsDialog(dataSource.Name, prompt)
								  If credentialsDialog.ShowDialog() = True Then credentials = New Credentials(credentialsDialog.UserName, credentialsDialog.Password)
							  End Sub
		Application.Current.Dispatcher.Invoke(requestFunction)
		Return credentials
	End Function

	Private Sub CmbListBox_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles CmbListBox.SelectionChanged
		Dim _selecteditem As ComboBoxItem = DirectCast(CmbListBox.SelectedItem, ComboBoxItem)

		If CmbListBox.SelectedIndex = 0 Then
			If chkCustomButton IsNot Nothing Then
				chkCustomButton.IsEnabled = False
			End If
			btnPreview.IsEnabled = False
		Else
			chkCustomButton.IsEnabled = True
			btnPreview.IsEnabled = True
		End If
	End Sub
End Class
