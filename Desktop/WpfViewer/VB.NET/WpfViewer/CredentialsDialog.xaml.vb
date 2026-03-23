Imports System.Windows
Imports System.Windows.Input

Partial Public Class CredentialsDialog
	Inherits Window

	Public Property UserName As String
	Public Property Password As String
	Private Shared ReadOnly OKCommand As RoutedCommand = New RoutedCommand()
	Private Shared ReadOnly CancelCommand As RoutedCommand = New RoutedCommand()

	Public Sub New(dataSourceName As String, prompt As String)
		InitializeComponent()
		Title = "Credentials"
		CommandBindings.Add(New CommandBinding(OKCommand, AddressOf OKCommand_Executed))
		OKCommand.InputGestures.Add(New KeyGesture(Key.Enter))
		btnOk.Command = OKCommand
		CommandBindings.Add(New CommandBinding(CancelCommand, AddressOf CancelCommand_Executed))
		CancelCommand.InputGestures.Add(New KeyGesture(Key.Escape))
		btnCancel.Command = CancelCommand
		txtPrompt.Text = prompt
		Title = String.Format(Title, dataSourceName)
	End Sub

	Private Sub OKCommand_Executed(sender As Object, e As ExecutedRoutedEventArgs)
		UserName = txtUser.Text
		Password = txtPassword.Password
		DialogResult = True
		Close()
	End Sub

	Private Sub CancelCommand_Executed(sender As Object, e As ExecutedRoutedEventArgs)
		DialogResult = False
		Close()
	End Sub
End Class