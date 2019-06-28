Public Class ChangePassword
    Dim contactPath As String

    Private Sub ChangePassword_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If (e.KeyCode = Keys.W OrElse e.KeyCode = Keys.Q) AndAlso e.Modifiers = Keys.Control Or (e.KeyCode = Keys.Escape AndAlso TextBox1.Text = Nothing) Then
            Close()
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Button1.Text = "OK" Then
            Dim d As DialogResult = MsgBox("Note: This changes your PERSONAL password used to login to Sliicy." & Environment.NewLine & "Sliicy doesn't store your password ANYWHERE. If you forget this password, it will take a VERY LONG TIME to ever recover it! Proceed?", MsgBoxStyle.YesNoCancel + MsgBoxStyle.Exclamation + MessageBoxDefaultButton.Button2, "Change Password")
            If Not d = DialogResult.Yes Then Exit Sub
            'Console.WriteLine(Environment.NewLine & "Please specify the path to the folder that contains all of your contacts:")
            'Console.WriteLine(Environment.NewLine & "(Example: """ & My.Computer.FileSystem.SpecialDirectories.Desktop & "\Sliicy Contacts\"")")
            contactPath = contactPath.Replace("""", Nothing)
            If Not My.Computer.FileSystem.DirectoryExists(contactPath) Then 'Catch invalid path
                Exit Sub
            End If
            Dim newPass As String = TextBox1.Text
            Dim checkPass As String = TextBox2.Text
            If checkPass = newPass Then
                Main.Sliicy("change", contactPath, Login.mainMenuPassword,,, newPass)
                MsgBox("Password was successfully changed!")
                Login.mainMenuPassword = TextBox2.Text
                Close()
            End If
        Else
            Close()
        End If
    End Sub

    Private Sub TextBox_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged, TextBox2.TextChanged
        If TextBox1.Text = TextBox2.Text AndAlso TextBox1.TextLength > 0 Then
            Button1.Text = "OK"
        Else
            Button1.Text = "Cancel"
        End If
    End Sub

    Private Sub TextBox_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox1.KeyPress, TextBox2.KeyPress
        If e.KeyChar = Convert.ToChar(1) Then
            DirectCast(sender, TextBox).SelectAll()
            e.Handled = True
        End If
    End Sub

    Private Sub TextBox_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyDown, TextBox2.KeyDown
        If e.KeyCode = Keys.Enter Then
            If sender Is TextBox1 Then
                TextBox2.Select()
            ElseIf sender Is TextBox2 Then
                Button1.PerformClick()
            End If
            e.SuppressKeyPress = True
            e.Handled = True
        End If
    End Sub

    Private Sub ChangePassword_SizeChanged(sender As Object, e As EventArgs) Handles MyBase.SizeChanged
        Try
            Label1.Font = New Font(Label1.Font.Name, ClientSize.Height / 10, FontStyle.Bold, GraphicsUnit.Point)
            Label2.Font = New Font(Label2.Font.Name, ClientSize.Height / 10, FontStyle.Bold, GraphicsUnit.Point)
            TextBox1.Font = New Font(TextBox1.Font.Name, ClientSize.Height / 10, FontStyle.Bold, GraphicsUnit.Point)
            TextBox2.Font = New Font(TextBox2.Font.Name, ClientSize.Height / 10, FontStyle.Bold, GraphicsUnit.Point)
            Button1.Font = New Font(Button1.Font.Name, ClientSize.Height / 10, FontStyle.Bold, GraphicsUnit.Point)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub ChangePassword_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MsgBox("Please select the folder containing all your contacts.", MsgBoxStyle.OkOnly + MsgBoxStyle.Information, "Change Password")
        Dim fileBrowse As New FolderBrowserDialog With {
        .SelectedPath = Application.StartupPath & "\",
        .Description = "Select the folder containing all contacts"}
        Dim d2 As DialogResult = fileBrowse.ShowDialog()
        If d2 = DialogResult.OK Then
            contactPath = fileBrowse.SelectedPath
        Else
            Close()
        End If
    End Sub

    Private Sub Button1_MouseEnter(sender As Object, e As EventArgs) Handles Button1.MouseEnter
        Button1.BackColor = Main.secondaryColor
    End Sub

    Private Sub Button1_MouseLeave(sender As Object, e As EventArgs) Handles Button1.MouseLeave
        Button1.BackColor = Main.primaryColor
    End Sub
End Class