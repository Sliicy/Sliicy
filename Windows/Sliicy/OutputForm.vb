Public Class OutputForm
    Private Sub OutputForm_SizeChanged(sender As Object, e As EventArgs) Handles MyBase.SizeChanged
        Try
            Label1.Font = New Font(Label1.Font.Name, ClientSize.Height / 30, FontStyle.Bold, GraphicsUnit.Point)
            TextBox1.Font = New Font(TextBox1.Font.Name, ClientSize.Height / 40)
            Button1.Font = New Font(Button1.Font.Name, ClientSize.Height / 30, FontStyle.Bold, GraphicsUnit.Point)
            Button2.Font = New Font(Button2.Font.Name, ClientSize.Height / 30, FontStyle.Bold, GraphicsUnit.Point)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub OutputForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TextBox1.Text = Main.TextBox2.Text
        TextBox1.SelectionStart = 0
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        Dim TextBoxRect As Size = TextRenderer.MeasureText(sender.Text, sender.Font, New Size(sender.Width, Integer.MaxValue), TextFormatFlags.WordBreak Or TextFormatFlags.TextBoxControl)
        Try
            sender.ScrollBars = If(TextBoxRect.Height > sender.Height, ScrollBars.Vertical, ScrollBars.None)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Button_MouseEnter(sender As Object, e As EventArgs) Handles Button1.MouseEnter, Button2.MouseEnter
        sender.BackColor = Main.secondaryColor
    End Sub

    Private Sub Button_MouseLeave(sender As Object, e As EventArgs) Handles Button1.MouseLeave, Button2.MouseLeave
        sender.BackColor = Main.primaryColor
    End Sub
End Class