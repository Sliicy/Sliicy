Public Class Login
    Public mainMenuPassword As String

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If sender.Text = "Cancel" Then
            Close()
        ElseIf sender.text = "OK" Then
            Main.Show()
            Hide()
            mainMenuPassword = TextBox1.Text
            TextBox1.Clear()
        End If
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        If TextBox1.TextLength > 0 Then
            Button1.Text = "OK"
        Else
            Button1.Text = "Cancel"
        End If
    End Sub

    Private Sub TextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            Button1.PerformClick()
            e.SuppressKeyPress = True
            e.Handled = True
        ElseIf e.KeyCode = Keys.Escape Then
            e.SuppressKeyPress = True
            e.Handled = True
        End If
    End Sub

    Private Sub Form1_SizeChanged(sender As Object, e As EventArgs) Handles MyBase.SizeChanged
        Try
            Label1.Font = New Font(Label1.Font.Name, ClientSize.Height / 10, FontStyle.Bold, GraphicsUnit.Point)
            TextBox1.Font = New Font(TextBox1.Font.Name, ClientSize.Height / 10, FontStyle.Bold, GraphicsUnit.Point)
            Button1.Font = New Font(Button1.Font.Name, ClientSize.Height / 10, FontStyle.Bold, GraphicsUnit.Point)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Login_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If ((e.KeyCode = Keys.W OrElse e.KeyCode = Keys.Q) AndAlso e.Modifiers = Keys.Control) OrElse (e.KeyCode = Keys.Escape AndAlso TextBox1.Text = Nothing) Then
            Environment.Exit(0)
        End If
    End Sub

    Private Sub Button1_MouseEnter(sender As Object, e As EventArgs) Handles Button1.MouseEnter
        Button1.BackColor = Main.secondaryColor
        Button1.ForeColor = Color.White
    End Sub

    Private Sub Button1_MouseLeave(sender As Object, e As EventArgs) Handles Button1.MouseLeave
        Button1.BackColor = Color.White
        Button1.ForeColor = Color.Black
    End Sub
End Class