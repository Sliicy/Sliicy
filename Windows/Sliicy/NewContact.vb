Imports System.Runtime.InteropServices

Public Class NewContact
    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function FindWindow(
  ByVal lpClassName As String,
  ByVal lpWindowName As String) As IntPtr
    End Function

    <DllImport("user32.dll", EntryPoint:="FindWindow", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function FindWindowByClass(
  ByVal lpClassName As String,
  ByVal zero As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", EntryPoint:="FindWindow", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function FindWindowByCaption(
  ByVal zero As IntPtr,
  ByVal lpWindowName As String) As IntPtr
    End Function

    Private Sub NewContact_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox2.Text = Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Environment.UserName)
    End Sub

    Private Sub NewContact_SizeChanged(sender As Object, e As EventArgs) Handles MyBase.SizeChanged
        Try
            Label1.Font = New Font(Label1.Font.Name, ClientSize.Height / 10, FontStyle.Bold, GraphicsUnit.Point)
            Label2.Font = New Font(Label2.Font.Name, ClientSize.Height / 10, FontStyle.Bold, GraphicsUnit.Point)
            TextBox1.Font = New Font(TextBox1.Font.Name, ClientSize.Height / 10, FontStyle.Bold, GraphicsUnit.Point)
            TextBox2.Font = New Font(TextBox2.Font.Name, ClientSize.Height / 10, FontStyle.Bold, GraphicsUnit.Point)
            Button1.Font = New Font(Button1.Font.Name, ClientSize.Height / 10, FontStyle.Bold, GraphicsUnit.Point)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub NewContact_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If (e.KeyCode = Keys.W OrElse e.KeyCode = Keys.Q) AndAlso e.Modifiers = Keys.Control Or (e.KeyCode = Keys.Escape AndAlso TextBox1.Text = Nothing) Then
            Close()
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

    Private Sub TextBox_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox1.KeyPress, TextBox2.KeyPress
        If e.KeyChar = Convert.ToChar(1) Then
            DirectCast(sender, TextBox).SelectAll()
            e.Handled = True
        End If
    End Sub

    Private Sub TextBox_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged, TextBox2.TextChanged
        Dim s1 = TextBox1.SelectionStart
        Dim s2 = TextBox2.SelectionStart
        TextBox1.Text = Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(TextBox1.Text)
        TextBox2.Text = Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(TextBox2.Text)
        TextBox1.SelectionStart = s1
        TextBox2.SelectionStart = s2
        If TextBox1.TextLength > 0 AndAlso TextBox2.TextLength > 0 Then
            Button1.Text = "OK"
        Else
            Button1.Text = "Cancel"
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If sender.Text = "Cancel" Then
            Close()
        ElseIf sender.text = "OK" Then
            Dim friendName As String = TextBox1.Text
            Dim myName As String = TextBox2.Text
            friendName = friendName.Replace("""", Nothing)
            myName = myName.Replace("""", Nothing)
            If My.Computer.FileSystem.FileExists(friendName & ".txt") Or My.Computer.FileSystem.FileExists(myName & ".slii") Then
                Dim d As DialogResult = MsgBox("Another contact with the same name was found. Do you want to overwrite the old one?", MsgBoxStyle.YesNoCancel + MsgBoxStyle.Exclamation + MsgBoxStyle.DefaultButton2, "Duplicate contact exists")
                If Not d = DialogResult.Yes Then
                    Exit Sub
                End If
            End If
            Main.Sliicy("create", myName, Login.mainMenuPassword,, friendName)
            MsgBox("2 files successfully made! Please note that " & friendName.Substring(0, friendName.Length - IO.Path.GetExtension(friendName).Length) & ".txt" & " is your OWN file. Do not give it out to anyone." & Environment.NewLine & myName.Substring(0, myName.Length - IO.Path.GetExtension(myName).Length) & ".slii" & " should only be given out to trusted individuals. Do not share this file over the internet! Please be responsible with it and adhere to local laws. Press OK to view this file now.", MsgBoxStyle.OkOnly + MsgBoxStyle.Information, "Contact created")
            Dim nWnd As IntPtr
            Dim ceroIntPtr As New IntPtr(0)
            Dim wndName As String
            wndName = Application.StartupPath.Split("\"c).Last
            nWnd = FindWindow(Nothing, wndName)
            If nWnd.Equals(ceroIntPtr) Then
                Shell("explorer /select, " & Application.StartupPath & "\" & myName & ".slii", AppWinStyle.NormalFocus)
            Else
                AppActivate(wndName)
                SendKeys.SendWait("~")
            End If
            Main.Show()
            Close()
        End If
    End Sub

    Private Sub TextBox1_Enter(sender As Object, e As EventArgs) Handles TextBox2.Enter, TextBox1.Enter
        sender.SelectAll()
    End Sub

    Private Sub Button1_MouseEnter(sender As Object, e As EventArgs) Handles Button1.MouseEnter
        Button1.BackColor = Main.secondaryColor
    End Sub

    Private Sub Button1_MouseLeave(sender As Object, e As EventArgs) Handles Button1.MouseLeave
        Button1.BackColor = Main.primaryColor
    End Sub
End Class