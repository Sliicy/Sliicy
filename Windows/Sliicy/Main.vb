Imports System.Text.RegularExpressions

Public Class Main
    Dim delim As String() = New String(0) {Environment.NewLine & Environment.NewLine}
    Dim space() As Char = {" "c, vbCr, vbLf}
    Dim contactPath As String = Nothing
    Dim fileBrowse As New OpenFileDialog With {.InitialDirectory = Application.StartupPath & "\", .Multiselect = False}
    Public primaryColor As Color = Color.FromArgb(81, 0, 148)
    Public secondaryColor As Color = Color.FromArgb(140, 0, 255)

    'The following are locations where in the password everything is stored.
    'Numbers are the first item (location 0) followed by decimals.
    'IPv4 starts at 2 and has 4 namespaces (ex: 192.168.0.1) so the next item starts at 6.
    'This helps prevent discrepancies later in the code.
    'To update the number locations, only this area needs changing.
    Const const_Numbers = 0
    Const const_Decimals = 1
    Const const_IPv4 = 2 'Maximum is 4 places such as 192.168.0.1
    Const const_IPv6 = 6 'Maximum is 8 places such as 0.0.0.0.0.0.0.0
    Const numSign = 14 'Maximum is 12 numbers which are 1234567890.-
    Const symSign = 26 'Maximum is 33 symbols which are ,./|\;':"[]{}<>?-=_+`~!@#$%^&*() and [space]
    Const letterSign = 59 'Maximum is 52 letters which are A-Z and a-z
    Const const_CountOrder = 111 'Maximum is 256 words currently
    Const const_NumSignOrder = const_CountOrder
    Const const_CountTotal = 367 'Maximum is 256 words currently
    Const countSymbol = 623 'Maximum is 32 unique symbols in foundSymbols, 9 total which are .,;!?()+= 
    Const const_Total = 654 'Maximum allocation of space needed for everything besides the wordlists
    Const const_Types = const_Total + 1 'Start position of the wordlists

    Public Function ModIt(x As Long, m As Long)
        Dim r As Long = x Mod m
        Return If(r < 0, r + m, r)
    End Function

    Public Function RandomizeStrings(arr As String()) As String()
        Dim _random As New TrueRandom
        Dim list As New List(Of KeyValuePair(Of Integer, String))()
        ' Add all strings from array.
        ' ... Add new random int each time.
        For Each s As String In arr
            list.Add(New KeyValuePair(Of Integer, String)(_random.[GetNext](), s))
        Next
        ' Sort the list by the random number.
        Dim sorted = From item In list Order By item.Key
        ' Allocate new string array.
        Dim result As String() = New String(arr.Length - 1) {}
        ' Copy values to array.
        Dim index As Integer = 0
        For Each pair As KeyValuePair(Of Integer, String) In sorted
            result(index) = pair.Value
            index += 1
        Next
        ' Return copied array.
        Return result
    End Function

    Public Function CheckForAlphaCharacters(ByVal stringToCheck As String)
        For i = 0 To stringToCheck.Length - 1
            If Char.IsLetter(stringToCheck.Chars(i)) Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If TextBox1.TextLength = Nothing AndAlso Clipboard.GetText.Length < 131072 Then
            TextBox1.Text = Clipboard.GetText
        End If
    End Sub

    Private Sub Main_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If ((e.KeyCode = Keys.W OrElse e.KeyCode = Keys.Q) AndAlso e.Modifiers = Keys.Control) Or (e.KeyCode = Keys.Escape AndAlso TextBox1.Text = Nothing) Then
            Environment.Exit(0)
        End If
        If My.Computer.Keyboard.CtrlKeyDown = True Then
            If e.KeyCode = Keys.S Then
                Button1.PerformClick()
            ElseIf e.KeyCode = Keys.E Then
                Button2.PerformClick()
            ElseIf e.KeyCode = Keys.D Then
                Button3.PerformClick()
            ElseIf e.KeyCode = Keys.N Then
                Button4.PerformClick()
            ElseIf e.KeyCode = Keys.J Then
                Button5.PerformClick()
            ElseIf e.KeyCode = Keys.P Then
                Button6.PerformClick()
            ElseIf e.KeyCode = Keys.L Then
                Button7.PerformClick()
            End If
        End If
        If e.KeyCode = Keys.F1 Then
            Dim lineSplit = My.Resources.WordList_Dictionary.Split(delim, StringSplitOptions.None)
            MsgBox("Find out more at sliicy.com" & Environment.NewLine & "Version " & Application.ProductVersion & Environment.NewLine & "Total number of WordLists: " & lineSplit.Count - 1, MsgBoxStyle.OkOnly + MsgBoxStyle.Information, "Sliicy")
        End If
    End Sub

    Private Sub Main_SizeChanged(sender As Object, e As EventArgs) Handles MyBase.SizeChanged
        Try
            Button2.Font = New Font(Button2.Font.Name, ClientSize.Height / 20, FontStyle.Bold, GraphicsUnit.Point)
            Button3.Font = New Font(Button3.Font.Name, ClientSize.Height / 20, FontStyle.Bold, GraphicsUnit.Point)
            Button1.Font = New Font(Button1.Font.Name, ClientSize.Height / 40, FontStyle.Bold, GraphicsUnit.Point)
            Button4.Font = New Font(Button4.Font.Name, ClientSize.Height / 40, FontStyle.Bold, GraphicsUnit.Point)
            Button5.Font = New Font(Button5.Font.Name, ClientSize.Height / 40, FontStyle.Bold, GraphicsUnit.Point)
            Button6.Font = New Font(Button6.Font.Name, ClientSize.Height / 40, FontStyle.Bold, GraphicsUnit.Point)
            Button7.Font = New Font(Button7.Font.Name, ClientSize.Height / 40, FontStyle.Bold, GraphicsUnit.Point)
            TextBox1.Font = New Font(TextBox1.Font.Name, ClientSize.Height / 20)
            TextBox2.Font = New Font(TextBox2.Font.Name, ClientSize.Height / 20)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Main_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        If Login.Visible = False Then Environment.Exit(0)
    End Sub

    Private Sub Button_MouseEnter(sender As Object, e As EventArgs) Handles Button7.MouseEnter, Button6.MouseEnter, Button5.MouseEnter, Button4.MouseEnter, Button3.MouseEnter, Button2.MouseEnter, Button1.MouseEnter
        sender.BackColor = secondaryColor
    End Sub

    Private Sub Button_MouseLeave(sender As Object, e As EventArgs) Handles Button7.MouseLeave, Button6.MouseLeave, Button5.MouseLeave, Button4.MouseLeave, Button3.MouseLeave, Button2.MouseLeave, Button1.MouseLeave
        sender.BackColor = primaryColor
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        fileBrowse.Title = "Select the contact to speak with"
        fileBrowse.Filter = Nothing
        Dim d As DialogResult = fileBrowse.ShowDialog()
        If d = DialogResult.OK Then
            If fileBrowse.FileName.ToLower.EndsWith(".slii") Then
                MsgBox("Selected contact isn't in a chat yet! Please first join the chat!", MsgBoxStyle.OkOnly + MsgBoxStyle.Critical, "Error")
                Exit Sub
            End If
            contactPath = fileBrowse.FileName
            TextBox1.Select()
            Button1.Text = IO.Path.GetFileNameWithoutExtension(fileBrowse.FileName)
        Else
            Exit Sub
        End If
    End Sub

    Private Sub Dencrypt_Click(sender As Object, e As EventArgs) Handles Button2.Click, Button3.Click
        If contactPath = Nothing Then
            MsgBox("Please pick a valid contact file!", MsgBoxStyle.OkOnly + MsgBoxStyle.Exclamation, "Error")
            Exit Sub
        Else
            contactPath = contactPath.Replace("""", Nothing)
            If Not My.Computer.FileSystem.FileExists(contactPath) Then 'Catch invalid name (maybe the user forgot to add .txt)
                contactPath = contactPath & ".txt"
                If Not My.Computer.FileSystem.FileExists(contactPath) Then 'Catch invalid path
                    MsgBox("The contact can't be read. Make sure it can be accessed!", MsgBoxStyle.OkOnly + MsgBoxStyle.Exclamation, "Contact can't be read")
                    Exit Sub
                End If
            End If
            If TextBox1.Text = Nothing Then Exit Sub
            Dim messageString As String = TextBox1.Text
            If sender Is Button3 Then
                Sliicy("decrypt", contactPath, Login.mainMenuPassword, messageString,,, False)
            ElseIf sender Is Button2 Then
                Sliicy("encrypt", contactPath, Login.mainMenuPassword, messageString,,, False)
            End If
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        NewContact.Show()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        MsgBox("Please select the .slii file that your friend shared with you.", MsgBoxStyle.OkOnly + MsgBoxStyle.Information, "Join")
        fileBrowse.Title = "Select the .slii file to join"
        fileBrowse.Filter = "Sliicy Files|*.slii"
        Dim d As DialogResult = fileBrowse.ShowDialog()
        If d = DialogResult.OK Then
            contactPath = fileBrowse.FileName
            contactPath = contactPath.Replace("""", Nothing)
            If Not contactPath.Contains(".slii") Then
                contactPath = contactPath & ".slii"
            End If
            If Not My.Computer.FileSystem.FileExists(contactPath) Then
                MsgBox("Can't find or access the requested contact!", MsgBoxStyle.OkOnly + MsgBoxStyle.Critical, "Error")
                Exit Sub
            End If
            If My.Computer.FileSystem.FileExists(contactPath.ToString.Replace(".slii", ".txt")) Then
                Dim d2 As DialogResult = MsgBox("The contact " & contactPath.ToString.Replace(".slii", ".txt") & " already exists! If you proceed, the old one will be overwritten! Continue?", MsgBoxStyle.YesNoCancel + MsgBoxStyle.Exclamation + MessageBoxDefaultButton.Button2, "Duplicate contact exists")
                If Not d2 = DialogResult.Yes Then Exit Sub
            End If
            Sliicy("join", contactPath, Login.mainMenuPassword)
            Dim d3 As DialogResult = MsgBox("Successfully joined the conversation! Would you like to load the contact now?", MsgBoxStyle.YesNoCancel + MsgBoxStyle.Information, "Success")
            If d3 = DialogResult.Yes Then
                contactPath = fileBrowse.FileName.Replace(".slii", ".txt")
                TextBox1.Select()
                Button1.Text = IO.Path.GetFileNameWithoutExtension(fileBrowse.FileName)
            End If
        Else
            Exit Sub
        End If
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim d As DialogResult = MsgBox("Do you want to change the password used to login?", MsgBoxStyle.YesNoCancel + MsgBoxStyle.Question + MessageBoxDefaultButton.Button2, "Change Password?")
        If d = DialogResult.Yes Then
            ChangePassword.ShowDialog()
        End If
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Login.Show()
        Close()
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged, TextBox2.TextChanged
        If TextBox2.ForeColor = Color.Red Then TextBox2.ForeColor = Color.Black
    End Sub

    Private Sub TextBox_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox1.KeyPress, TextBox2.KeyPress
        If e.KeyChar = Convert.ToChar(1) Then
            DirectCast(sender, TextBox).SelectAll()
            e.Handled = True
        End If
    End Sub

    Private Sub TextBox_Enter(sender As Object, e As EventArgs) Handles TextBox1.Enter, TextBox2.Enter
        sender.SelectAll
    End Sub

    Public Sub Sliicy(command As String, user As String, password As String, Optional message As String = Nothing, Optional friendName As String = Nothing, Optional newPassword As String = Nothing, Optional readMode As Boolean = False)

        'The main wordlist that will hold all the words together (from the contact file's dictionary):
        Dim wordList1 As New List(Of List(Of String))

        'A dual-purpose wordlist (when creating a wordlist, used as the shuffler; when (en/de)crypting, used to sort and save to the contact file:
        Dim wordList2 As New List(Of List(Of String))

        'A list of words that didn't exist in the contact file will fill up here (and will be added to the contact file):
        Dim addedWordList As New List(Of String)

        'Phrase will be the string that is the answer to the encryption or decryption. It will be adding word after word from the message:
        Dim phrase As String = Nothing

        'A text format of the password found in the first line of the contact file's (a bunch of numbers) will be this:
        Dim masterPassword As String

        'A list format of the password found in the first line of the contact file:
        Dim passwordChunk As New List(Of String)

        '2 lists of every number from 0 to 255 or 65535, used to define as an IP address:
        Dim IPv4 As New List(Of Integer)(Enumerable.Range(0, 255).ToList())
        Dim IPv6 As New List(Of Integer)(Enumerable.Range(0, 65535).ToList())

        If password.Length = 0 Then
            Exit Sub
        End If

        'Get the numerical value of the user's personal password:
        Dim charCount = 1
        Dim sumTotal As Integer = 0
        For Each c As Char In password
            sumTotal = Val(sumTotal + AscW(c) * charCount)
            If Char.IsLetter(c) Then
                charCount = charCount + 1
            End If
        Next
        If charCount Mod 2 > 0 Then
            sumTotal = -sumTotal
        End If
        password = sumTotal * charCount

        'Check if contact file came from a different OS and make it compatible with Windows:
        If user.Length > 0 AndAlso Not command.ToLower = "create" Then 'fix !
            If Not IO.File.ReadAllText(user).Contains(Environment.NewLine) Then
                Dim newCRLF() As String = IO.File.ReadAllLines(user)
                My.Computer.FileSystem.DeleteFile(user, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                IO.File.WriteAllLines(user, newCRLF, System.Text.Encoding.UTF8)
            End If
        End If

        'Start checking the arguments for Sliicy (check the lowercase of the command for compatibility):
        If command.ToLower = "create" Then

            'Check if the contact name, password, or friend name is blank:
            If user.Length = 0 OrElse friendName.Length = 0 Then
                'Console.ForegroundColor = ConsoleColor.Red
                'Console.WriteLine("Error: Nothing can be left empty.")
                Exit Sub
            End If

            'Generate a new password from scratch with a random set of numbers:
            Dim newPass As String = Nothing
            Dim random As New TrueRandom

            'Create a list of strings based on Sliicy's Wordlist Dictionary embedded in Resources:
            Dim lineSplit = My.Resources.WordList_Dictionary.Split(delim, StringSplitOptions.None)

            'For every item inside the wordlist:
            For i = 0 To lineSplit.Count - 1
                'Add that item to WordList1:
                wordList1.Add(New List(Of String)(lineSplit(i).Split(Environment.NewLine.ToCharArray, StringSplitOptions.RemoveEmptyEntries)))
            Next

            'randomList will hold every number from 0 to wordList1's maximum size:
            Dim randomList As New List(Of String)()
            For i = 0 To wordList1.Count - 1
                randomList.Add(i)
            Next

            'randomList is shuffled:
            randomList = RandomizeStrings(randomList.ToArray).ToList

            'Instead of generically going from 0 to the max, this will go out of order purposely to further make harder to crack:
            For Each item In randomList

                'Only if the list isn't supposed to be at the bottom (Sliicy is part of that list):
                If Not wordList1(item).Contains("Sliicy") Then

                    'Add a random item from WordList1 to WordList2:
                    wordList2.Add(RandomizeStrings(wordList1(item).ToArray).ToList)
                End If
            Next

            'Create an array from A-z for encrypting all new words:
            Dim abcs As New List(Of String)()
            For Each c As Char In "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-"
                abcs.Add(c)
            Next

            'Insert this ABC array at the beginning:
            wordList2.Insert(0, New List(Of String)(RandomizeStrings(abcs.ToArray).ToList))

            'Create a new list called lastList that will store the proper nouns:
            Dim lastList As New List(Of String)()
            For i = 0 To wordList1.Count - 1
                If wordList1(i).Contains("Sliicy") Then
                    lastList.AddRange(wordList1(i))
                End If
            Next

            'Add the missing proper noun list to the end:
            wordList2.Add(New List(Of String)(RandomizeStrings(lastList.ToArray).ToList))

            'For each number in the line split including the total number of password segments used (see the constants listed at top of code):
            For i = 0 To lineSplit.Count + const_Total + 10000

                'Pick a random number from -100000 to 100000 for the new password:
                newPass = newPass & Math.Ceiling(random.GetNext(-100000, 100000)) & ";"
            Next

            'If the first number is a zero, keep repeating the process again (because it can compromise Sliicy):
            Do While newPass.Substring(0, 2) = "0;"
                newPass = Math.Ceiling(random.GetNext(-100000, 100000)) & ";" & newPass.Substring(2)
            Loop

            'If there are any other zeros, keep repeating the process again (because it can compromise Sliicy):
            Do While newPass.Contains(";0")
                newPass = newPass.Replace(";0", ";" & Math.Ceiling(random.GetNext(-100000, 100000)))
            Loop

            'Remove the last character of the new password (it's an extra semicolon):
            newPass = newPass.Substring(0, newPass.Length - 1)

            'Make a final list that will have break points between each wordlist:
            Dim finalList As New List(Of String)

            'For every wordlist:
            For i = 0 To wordList2.Count - 1

                'For every sub-wordlist:
                For j = 0 To wordList2(i).Count - 1

                    'Add all words to the final list
                    finalList.Add(wordList2(i)(j))
                Next

                'Add blank space (this will separate one wordlist from another wordlist, like nouns from verbs):
                finalList.Add(Nothing)
            Next

            'Remove the last empty string of final list:
            finalList.RemoveAt(finalList.Count - 1)

            'Finally, write all changes to the contact file just created saved as a .slii (this file should be shared securely), and not encrypted by any password (until they join it):
            IO.File.WriteAllText(user.Substring(0, user.Length - IO.Path.GetExtension(user).Length) & ".slii", newPass & Environment.NewLine & Environment.NewLine & String.Join(Environment.NewLine, finalList.ToArray))

            'Now it's time to encrypt the user's own contact file used by their personal password:
            Dim answer As String = Nothing

            'For each of the numbers found in the contact file's first line:
            For Each pass As String In newPass.Split({";"c}, StringSplitOptions.RemoveEmptyEntries)
                'Add the user's password and the current number found in the contact file's first line to the answer:
                answer = answer & Val(pass) + Val(password) & ";"
            Next

            'Write all changes from the answer to the user's own contact file saved as a .txt file (NOT to be shared with anyone), and encrypted by the user's own password:
            IO.File.WriteAllText(friendName.Substring(0, friendName.Length - IO.Path.GetExtension(friendName).Length) & ".txt", answer.Substring(0, answer.Length - 1) & Environment.NewLine & Environment.NewLine & String.Join(Environment.NewLine, finalList.ToArray))

            'If Sliicy was asked to let the user join a conversation:
        ElseIf command.ToLower = "join" Then

            'Check if the contact or password is blank:
            If user.Length = 0 OrElse password.Length = 0 Then
                'Console.ForegroundColor = ConsoleColor.Red
                'Console.WriteLine("Error: The user or password cannot be empty.")
                Exit Sub
            End If

            'Remove redundant quotes from contact file location supplied:
            user = user.Replace("""", Nothing)

            'Get the entire contact file into a split string:
            Dim lineSplit = IO.File.ReadAllText(user).Split(delim, StringSplitOptions.None)

            'Temporary holding string:
            Dim answer As String = Nothing

            'Using the numerical value of the user's password, every number is added to this number to encrypt it:
            For Each passFound As String In lineSplit(0).Split({";"c}, StringSplitOptions.RemoveEmptyEntries)
                'Encrypt Passwords
                answer = answer & Val(passFound) + Val(password) & ";"
            Next

            'Set the first string of the line split to the numbers just encrypted:
            lineSplit(0) = answer.Substring(0, answer.Length - 1)

            'It's important that the wordlists have a separating blank line between the start and end of each wordlist:
            'This contact file is encrypted with the user's password. If this gets into the wrong hands, it still will be VERY hard to guess each number:
            IO.File.WriteAllText(user.ToString.Replace(".slii", ".txt"), String.Join(Environment.NewLine & Environment.NewLine, lineSplit.ToArray), System.Text.Encoding.UTF8)

            'Delete the old contact file ending in a .slii (no longer needed and a major security flaw if not deleted):
            My.Computer.FileSystem.DeleteFile(user, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)

            'If Sliicy received an encrypt or decrypt request:
        ElseIf command.ToLower = "encrypt" OrElse command.ToLower = "decrypt" Then

            'Detect if any required fields are missing (ie: no message):
            If user.Length = 0 OrElse message.Length = 0 Then
                'Console.ForegroundColor = ConsoleColor.Red
                'Console.WriteLine("Error: The user or password cannot be empty.")
                Exit Sub
            End If

            'Check if the message doesn't have any words:
            If command.ToLower = "encrypt" AndAlso Not CheckForAlphaCharacters(message) Then
                MsgBox("The message must contain at least 1 word for security purposes.", MsgBoxStyle.OkOnly + MsgBoxStyle.Critical, "Error")
                Exit Sub
            End If

            'Clean up message by removing leading and following spaces:
            message = message.TrimStart(" ").TrimEnd(" ")

            'Remove double spacing:
            Do While message.Contains("  ")
                message = message.Replace("  ", " ")
            Loop

            'Remove periods from numbers (Sliicy sees them as decimals and will drop them!):
            If message.Contains(".") Then

                'redFlag will tell detect if any number has a trailing dot:
                Dim redFlag = 0
                Dim countChars = 0
                Dim locationOfDot As New List(Of Integer)({})
                For Each c As Char In message
                    If c = "." AndAlso redFlag = 1 AndAlso countChars = message.Count - 1 Then
                        locationOfDot.Add(countChars)
                        redFlag = 0
                    ElseIf IsNumeric(c) Then
                        redFlag = 1
                    ElseIf c = "." AndAlso redFlag = 1 Then
                        redFlag = 2
                    ElseIf c = "." AndAlso redFlag = 2 Then
                        locationOfDot.Add(countChars - 1)
                        redFlag = 0
                    ElseIf Not IsNumeric(c) AndAlso c <> "." AndAlso redFlag = 2 Then
                        locationOfDot.Add(countChars - 1)
                        redFlag = 0
                    ElseIf Not IsNumeric(c) AndAlso c <> "." Then
                        redFlag = 0
                    End If
                    countChars = countChars + 1
                Next

                'Remove any dots connecting to numbers starting backwards:
                If locationOfDot.Count > 0 Then
                    For i = locationOfDot.Count - 1 To 0 Step -1
                        message = message.Insert(locationOfDot(i), " ")
                    Next
                End If
            End If

            'Split contact file into sections of lineSplits():
            Dim lineSplit As String()
            Try
                lineSplit = IO.File.ReadAllText(user).Split(delim, StringSplitOptions.None)
            Catch ex As Exception
                'Console.ForegroundColor = ConsoleColor.Red
                'Console.WriteLine("Error: The user or password cannot be empty.")
                Exit Sub
            End Try

            'Set the Master Password to the first line of contact:
            masterPassword = lineSplit(0)

            'Decrypt the Master Password based on the numerical value of the user's own password:
            'Computation will temporarily be the string builder (it will hold our results until we're done):
            Dim computation As String = Nothing

            'For every number between the semicolons found in the Master Password:
            For Each passFind As String In masterPassword.Split({";"c}, StringSplitOptions.RemoveEmptyEntries)

                'Decrypt current number by the user's password, then append a semicolon:
                computation = computation & (Val(passFind) - Val(password)) & ";"
            Next

            'Finally re-add all the decrypted numbers back to the Master Password (since they were encrypted and unreadable before):
            masterPassword = computation.Substring(0, computation.Length - 1)

            'Add each wordlist found from the contact to WordLists 1 & 2 (start at index 2 because index 0 is the numbers, and index 1 are the letters):
            For i = 2 To lineSplit.Count - 1
                wordList1.Add(New List(Of String)(lineSplit(i).Split(space, StringSplitOptions.RemoveEmptyEntries)))
                wordList2.Add(New List(Of String)(lineSplit(i).Split(space, StringSplitOptions.RemoveEmptyEntries)))
            Next

            'Create a list dedicated to letter-based encryption for new words (based on 1 because that is where the letters are located):
            Dim letterList As New List(Of String)(lineSplit(1).Split(space, StringSplitOptions.RemoveEmptyEntries))

            'Determine if encrypting or decrypting:
            If command.ToLower = "decrypt" Then

                'Flip the value from negative to positive and vice versa for every number in the first line of the contact file:
                For Each passFound As String In masterPassword.Split({";"c}, StringSplitOptions.RemoveEmptyEntries)

                    'If the currently checked number is negative:
                    If passFound.Contains("-") Then

                        'Remove the negative symbol:
                        passwordChunk.Add(passFound.Substring(1))
                    Else

                        'Add a negative symbol:
                        passwordChunk.Add("-" & passFound)
                    End If
                Next
            Else

                'Encryption was requested by the user; make a list of numbers called "passwordChunk" from each decrypted number found in the contact file:
                For Each passFound As String In masterPassword.Split({";"c}, StringSplitOptions.RemoveEmptyEntries)
                    passwordChunk.Add(passFound)
                Next
            End If

            'Remove fancy quotation marks (’ ‘ “ ” becomes ' and " respectively):
            message = message.Replace("’", "'").Replace("‘", "'").Replace(ChrW(&H201C), """").Replace(ChrW(&H201D), """")

            'These will hold the values needed to prevent forgery:
            Dim signature As Integer = Nothing
            Dim verifySignature As Integer = Nothing

            'Applies the last number of the message to the verifySignature variable if decrypting, and then trims it from the message:
            Try
                If command.ToLower = "decrypt" Then
                    verifySignature = message.Substring(message.LastIndexOf(" "), message.Length - message.LastIndexOf(" ")).TrimStart(" ").TrimEnd(" ")
                    message = message.Substring(0, message.LastIndexOf(" "))
                End If
            Catch ex As Exception
                'Console.ForegroundColor = ConsoleColor.Red
                'Console.WriteLine(Environment.NewLine & "Error: The message must end with a signature.")
                Exit Sub
            End Try

            'This will constantly add or subtract each word's position from the next word's position to create the unique signature:
            Dim flipPositiveNegative As Boolean = True

            'Count the number of words in the message:
            Dim occurrences As Integer = (message.Length - message.Replace(" ", String.Empty).Length) + 1

            'Warn the user if too many words exist in the message that will be sent:
            If occurrences > 256 AndAlso Not command.ToLower = "decrypt" Then
                Dim d As DialogResult = MsgBox("The message has more than 256 words. Any extra words won't have a unique encryption. Continue?", MsgBoxStyle.YesNoCancel + MsgBoxStyle.Question + MessageBoxDefaultButton.Button2, "Unique Encryption")
                If Not d = DialogResult.Yes Then
                    Exit Sub
                End If
            End If

            'Starting point of Count Order slots:
            Dim countOrder = const_CountOrder

            'Starting point of signatures of numbers:
            Dim numSignOrder = const_CountOrder

            'Starting point of Count Total Words slots:
            Dim countTotal = const_CountTotal

            'Strength represents how strong the requested message is from being cracked:
            Dim strength As Double = 1

            'Define a list of characters to find in the message (CAN'T USE A COLON because IPv6 colons can expand or contract; also can't use - for numerous reasons):
            Dim foundSymbols As New List(Of String)({".", ",", ";", "!", "?", "(", ")", "+", "="}) '9 total symbol types

            'Get the total number of all occurrences of symbols found:
            Dim total As Integer
            For i = 0 To foundSymbols.Count - 1
                total = total + message.Length - message.Replace(foundSymbols(i), String.Empty).Length
            Next

            '32 was the decided limit of how much punctuation on average there can be in messages:
            If total > 32 Then total = 32

            'For each of the delimiters found in the message, ignore them and focus on the text itself. Delimiters are various symbols:
            'Dim delimiters = New List(Of String)() From {"	", " ", "~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "=", "+", ",", "<", ">", "/", "?", ";", """", "[", "{", "]", "}", "\", "|", Environment.NewLine}

            'Make a pattern used for ignoring the delimiters:
            Dim pattern As String = "([^A-z=0-9\.:']|" & Regex.Escape(Environment.NewLine) & ")" 'String.Join("|", delimiters.[Select](Function(d) System.Text.RegularExpressions.Regex.Escape(d)).ToArray()) + ")"

            'For every word in the message being sent:
            For Each word As String In message.Split({" "c}, StringSplitOptions.RemoveEmptyEntries)

                'If the word is blank then move on (maybe the user employed double spacing):
                If word = Nothing Then Continue For

                'Make a pattern used to find website URLs:
                Dim re As Text.RegularExpressions.Regex = New Text.RegularExpressions.Regex("http(s)?://([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?")

                'Find out if the word is actually a website URL:
                For Each m As Text.RegularExpressions.Match In re.Matches(word)

                    'Try to see if the word is indeed a website URL. If this fails, then it wasn't a website URL:
                    Do
                        Try

                            'Find the Hostname (ie: 8-8-8-8.google.com) of the website URL:
                            Dim hostName As Net.IPHostEntry = Net.Dns.GetHostEntry((GetDomain.GetDomainFromUrl(m.Value)))

                            'Get the actual IP address (ie: 192.168.1.1) from that Hostname:
                            Dim ip As Net.IPAddress() = hostName.AddressList

                            'Determine if this IP address is actually an IPv6 (ie: 2001:4860:4860::8888) and not a regular IP address (ie: 192.168.1.1):
                            'The first index (0) is used because the address is stored there:
                            If ip(0).ToString.Contains(":") Then

                                'Replace the word in the message with this IPv6 address along with brackets (brackets indicate it is IPv6, not IPv4):
                                message = message.Replace(word, "[" & ip(0).ToString & "]")
                            Else

                                'Nope, it actually is a regular IPv4 address (ie: 192.168.1.1). Replace the word in the message with this IPv4:
                                message = message.Replace(word, ip(0).ToString)
                            End If

                            'If the word WASN'T a website URL, keep calm and carry on:
                        Catch ex As Exception
                            Dim d As DialogResult = MsgBox("No internet available. Internet is needed to convert the URL into an IP.", MsgBoxStyle.AbortRetryIgnore + MsgBoxStyle.Exclamation + MessageBoxDefaultButton.Button2, "No Internet")
                            If d = DialogResult.Retry Then
                                Continue Do
                            ElseIf d = DialogResult.Abort Then
                                Exit Sub
                            End If
                        End Try
                        Exit Do
                    Loop
                Next

                'Every word will add to the total count of words in the message:
                If countTotal < const_CountTotal + 256 Then countTotal = countTotal + 1
            Next
            'If the above code found website URLs, they have now been replaced with their actual IP address counterparts!

            'Make another pattern checker to check if the message now has any IP addresses which need to encrypt or decrypt (ie: 192.168.1.1 -> 1.2.3.4):
            Dim output As String() = System.Text.RegularExpressions.Regex.Split(message, pattern)
            
            'Make a list that will have ALL words:
            Dim bigListOfAllWords As New List(Of String)()

            'Fill the Big List with a HUGE amount of ALL words found in the contact file:
            For i = 0 To wordList1.Count - 1
                For j = 0 To wordList1(i).Count - 1
                    bigListOfAllWords.Add(wordList1(i)(j))
                Next
            Next

            If output.Count = 0 Then
                'Console.ForegroundColor = ConsoleColor.Red
                'Console.WriteLine("Error: The user or password cannot be empty.")
                Exit Sub
            End If

            'For each word in the message:
            For Each word In output

                'Ignore all spaces:
                If word = Nothing Then Continue For

                'Any dots and colons found in the word will help us know if the address is IPv4 or IPv6:
                Dim dotFound = 0
                Dim colonFound = 0

                'If the word actually is a number (ie: 5, 3.14, -70), it is dealt with separately:
                If IsNumeric(word) OrElse (command.ToLower = "decrypt" AndAlso word.Contains(".") = True AndAlso word.Last = "-") Then
                    If word.ToLower.Contains("e") Then
                        MsgBox("The message cannot contain any exponents.", MsgBoxStyle.OkOnly + MsgBoxStyle.Critical, "Error")
                        Exit Sub
                    End If
                    Dim negativeDecimal As Boolean = False
                    If word.Contains(".") = True AndAlso word.Last = "-" Then
                        negativeDecimal = True
                        word = word.Substring(0, word.Length - 1)
                    End If
                    If Decimal.TryParse(word, Nothing) = False Then
                        MsgBox("The message contains semi-numeric words that can't be evaluated. Please consider fixing any vague numbers.", MsgBoxStyle.OkOnly + MsgBoxStyle.Critical, "Error")
                        Exit Sub
                    End If
                    If word.Contains(".") Then
                        word = word.TrimEnd("0")
                        If word.Last = "." Then word = word & "0"
                    End If
                    Dim originalPhrase As Decimal = word
                    Dim cryptedPhrase As String = Decimal.Add(originalPhrase, passwordChunk.Item(const_Numbers) + Val(passwordChunk.Item(countOrder)) + Val(passwordChunk.Item(countTotal)) + Val(passwordChunk.Item(countSymbol + total)))
                    If originalPhrase.ToString.Contains(".") Then
                        Dim originalAnswer() As String = originalPhrase.ToString.Split(".")
                        Dim cryptedAnswer() As String = originalPhrase.ToString.Split(".")
                        cryptedAnswer(1) = StrReverse(cryptedAnswer(1))
                        cryptedAnswer(0) = Val(cryptedAnswer(0)) + passwordChunk.Item(const_Numbers) + Val(passwordChunk.Item(countOrder)) + Val(passwordChunk.Item(countTotal)) + Val(passwordChunk.Item(countSymbol + total))
                        'Reversed so .07 becomes 70, and -90 becomes .09-:
                        If negativeDecimal Then
                            cryptedAnswer(1) = StrReverse(Val(cryptedAnswer(1)) + -(Val(passwordChunk.Item(const_Decimals)) + Val(passwordChunk.Item(countOrder)) + Val(passwordChunk.Item(countTotal)) + Val(passwordChunk.Item(countSymbol + total)))).Replace("-", Nothing)
                        Else
                            cryptedAnswer(1) = StrReverse(Val(cryptedAnswer(1)) + Val(passwordChunk.Item(const_Decimals)) + Val(passwordChunk.Item(countOrder)) + Val(passwordChunk.Item(countTotal)) + Val(passwordChunk.Item(countSymbol + total)))
                        End If
                        cryptedPhrase = cryptedAnswer(0) & "." & cryptedAnswer(1)
                    End If
                    If cryptedPhrase.ToLower.Contains("e") Then
                        MsgBox("The message cannot contain any exponents or very large numbers or decimals.", MsgBoxStyle.OkOnly + MsgBoxStyle.Critical, "Error")
                        Exit Sub
                    End If
                    phrase = phrase & cryptedPhrase
                    Dim signature2 As Integer = Nothing
                    Dim flipPositiveNegative2 As Boolean = True
                    If command.ToLower = "encrypt" Then
                        For Each c As Char In Decimal.Parse(word).ToString
                            Dim temp As Long = Nothing
                            If IsNumeric(c) Then
                                temp = Val(passwordChunk.Item(numSign + Integer.Parse(c))) + Val(passwordChunk.Item(numSignOrder))
                            ElseIf c = "-" Then
                                temp = Val(passwordChunk.Item(numSign + 10)) + Val(passwordChunk.Item(numSignOrder))
                            ElseIf c = "." Then
                                temp = Val(passwordChunk.Item(numSign + 11)) + Val(passwordChunk.Item(numSignOrder))
                            End If
                            If signature2 = Nothing Then
                                signature2 = temp
                            Else
                                If flipPositiveNegative2 Then
                                    signature2 = signature2 + temp
                                Else
                                    signature2 = signature2 - temp
                                End If
                            End If
                            numSignOrder = numSignOrder + 1
                            flipPositiveNegative2 = Not flipPositiveNegative2
                        Next
                        numSignOrder = const_CountOrder
                        'The signature will be empty the first time. Here is where it is assigned:
                        If signature = Nothing Then
                            signature = signature2
                        Else
                            'If positive, add the new word's positional number by positive, or else subtract it:
                            If flipPositiveNegative Then
                                signature = signature + signature2
                            Else
                                signature = signature - signature2
                            End If
                        End If

                        'Flip the value of manipulating the signature by + or - as being True or False respectively:
                        flipPositiveNegative = Not flipPositiveNegative

                    ElseIf command.ToLower = "decrypt" Then
                        For Each c As Char In cryptedPhrase
                            Dim temp As Long = Nothing
                            If IsNumeric(c) Then
                                temp = -Val(passwordChunk.Item(numSign + Integer.Parse(c))) - Val(passwordChunk.Item(numSignOrder))
                            ElseIf c = "-" Then
                                temp = -Val(passwordChunk.Item(numSign + 10)) - Val(passwordChunk.Item(numSignOrder))
                            ElseIf c = "." Then
                                temp = -Val(passwordChunk.Item(numSign + 11)) - Val(passwordChunk.Item(numSignOrder))
                            End If
                            If signature2 = Nothing Then
                                signature2 = temp
                            Else
                                If flipPositiveNegative2 Then
                                    signature2 = signature2 + temp
                                Else
                                    signature2 = signature2 - temp
                                End If
                            End If
                            numSignOrder = numSignOrder + 1
                            flipPositiveNegative2 = Not flipPositiveNegative2
                        Next
                        numSignOrder = const_CountOrder
                        If signature = Nothing Then
                            signature = signature2
                        Else
                            'If positive, add the new word's positional number by positive, or else subtract it:
                            If flipPositiveNegative Then
                                signature = signature + signature2
                            Else
                                signature = signature - signature2
                            End If
                        End If

                        'Flip the value of manipulating the signature by + or - as being True or False respectively:
                        flipPositiveNegative = Not flipPositiveNegative
                    End If

                    'The word is in fact an IPv4:
                ElseIf Net.IPAddress.TryParse(word, Nothing) = True AndAlso word.Contains(":") = False Then

                    'Count number of dots found (since IPv4 addresses only have dots):
                    Dim dotCount = 0
                    For Each c As Char In word
                        If c = "." Then dotCount += 1
                    Next

                    'If there are 3 dots found (ie: 192.168.1.1):
                    If dotCount = 3 Then

                        'Iterating this code 4 times total. This helps keep track:
                        Dim tempCount = 0

                        'For every number (4 total in an IPv4) found in the word determined to be an IPv4:
                        For Each item As Integer In word.Split(New Char() {"."c})

                            'If the number in the contact password located at [1 + how many times this code ran] is a positive number:
                            'Positive and negative results are split so that the program doesn't go out of bounds:
                            Dim cryptedPhrase As Integer
                            If Val(passwordChunk.Item(const_IPv4 + tempCount) + Val(passwordChunk.Item(countOrder)) + Val(passwordChunk.Item(countTotal)) + Val(passwordChunk.Item(countSymbol + total))) >= 0 Then
                                cryptedPhrase = (item + passwordChunk.Item(const_IPv4 + tempCount) + Val(passwordChunk.Item(countOrder)) + Val(passwordChunk.Item(countTotal)) + Val(passwordChunk.Item(countSymbol + total))) Mod IPv4.Count
                            Else
                                Dim answer As Long = item + ModIt(passwordChunk.Item(const_IPv4 + tempCount) + Val(passwordChunk.Item(countOrder)) + Val(passwordChunk.Item(countTotal)) + Val(passwordChunk.Item(countSymbol + total)), IPv4.Count)
                                If answer >= IPv4.Count Then answer = answer - IPv4.Count
                                cryptedPhrase = IPv4.Item(answer)
                            End If
                            phrase = phrase & cryptedPhrase & "."

                            Dim signature2 As Integer = Nothing
                            Dim flipPositiveNegative2 As Boolean = True
                            If command.ToLower = "encrypt" Then
                                For Each c As Char In item.ToString
                                    Dim temp As Long = Nothing
                                    temp = Val(passwordChunk.Item(numSign + Integer.Parse(c))) + Val(passwordChunk.Item(numSignOrder))
                                    If signature2 = Nothing Then
                                        signature2 = temp
                                    Else
                                        If flipPositiveNegative2 Then
                                            signature2 = signature2 + temp
                                        Else
                                            signature2 = signature2 - temp
                                        End If
                                    End If
                                    numSignOrder = numSignOrder + 1
                                    flipPositiveNegative2 = Not flipPositiveNegative2
                                Next
                                numSignOrder = const_CountOrder
                                'The signature will be empty the first time. Here is where it is assigned:
                                If signature = Nothing Then
                                    signature = signature2
                                Else
                                    'If positive, add the new word's positional number by positive, or else subtract it:
                                    If flipPositiveNegative Then
                                        signature = signature + signature2
                                    Else
                                        signature = signature - signature2
                                    End If
                                End If

                                'Flip the value of manipulating the signature by + or - as being True or False respectively:
                                flipPositiveNegative = Not flipPositiveNegative

                            ElseIf command.ToLower = "decrypt" Then
                                For Each c As Char In cryptedPhrase.ToString
                                    Dim temp As Long = Nothing
                                    temp = -Val(passwordChunk.Item(numSign + Integer.Parse(c))) - Val(passwordChunk.Item(numSignOrder))
                                    If signature2 = Nothing Then
                                        signature2 = temp
                                    Else
                                        If flipPositiveNegative2 Then
                                            signature2 = signature2 + temp
                                        Else
                                            signature2 = signature2 - temp
                                        End If
                                    End If
                                    numSignOrder = numSignOrder + 1
                                    flipPositiveNegative2 = Not flipPositiveNegative2
                                Next
                                numSignOrder = const_CountOrder
                                If signature = Nothing Then
                                    signature = signature2
                                Else
                                    'If positive, add the new word's positional number by positive, or else subtract it:
                                    If flipPositiveNegative Then
                                        signature = signature + signature2
                                    Else
                                        signature = signature - signature2
                                    End If
                                End If

                                'Flip the value of manipulating the signature by + or - as being True or False respectively:
                                flipPositiveNegative = Not flipPositiveNegative
                            End If

                            tempCount = tempCount + 1
                        Next
                        phrase = phrase.Substring(0, phrase.Length - 1)
                        Continue For
                    End If

                    'The word is in fact an IPv6:
                ElseIf Net.IPAddress.TryParse(word, Nothing) = True AndAlso word.Contains(":") Then

                    Dim colonCount As Integer = 0
                    For Each c As Char In word
                        If c = ":" Then colonCount += 1
                    Next
                    If colonCount >= 2 AndAlso colonCount <= 7 Then
                        Dim phrase0 As String = Nothing
                        Dim neededZeros = 9 - word.Split(":").Length
                        For Each item In word.Split(New Char() {":"c}) 'Converts to longhand decimal:
                            If item = Nothing Then
                                If neededZeros = 1 Then
                                    phrase0 = phrase0 & ":0"
                                ElseIf neededZeros = 2 Then
                                    phrase0 = phrase0 & ":0:0"
                                ElseIf neededZeros = 3 Then
                                    phrase0 = phrase0 & ":0:0:0"
                                ElseIf neededZeros = 4 Then
                                    phrase0 = phrase0 & ":0:0:0:0"
                                ElseIf neededZeros = 5 Then
                                    phrase0 = phrase0 & ":0:0:0:0:0"
                                ElseIf neededZeros = 6 Then
                                    phrase0 = phrase0 & ":0:0:0:0:0:0"
                                ElseIf neededZeros = 7 Then
                                    phrase0 = phrase0 & ":0:0:0:0:0:0:0"
                                End If
                            Else
                                phrase0 = phrase0 & ":" & Convert.ToInt32(item, 16)
                            End If
                        Next
                        Dim phrase2 = Nothing
                        Dim tempCount = 0

                        'Add substring 1 because of a : at the beginning:
                        For Each item As Integer In phrase0.Substring(1).Split(New Char() {":"c})

                            'Cannot add Val(passwordChunk.Item(countSymbol + total)) because the IPv6 colons can always change:
                            Dim cryptedPhrase As Integer
                            If Val(passwordChunk.Item(const_IPv6 + tempCount) + Val(passwordChunk.Item(countOrder)) + Val(passwordChunk.Item(countTotal))) >= 0 Then
                                cryptedPhrase = (item + passwordChunk.Item(const_IPv6 + tempCount) + Val(passwordChunk.Item(countOrder)) + Val(passwordChunk.Item(countTotal))) Mod IPv6.Count
                            Else
                                Dim answer As Long = item + ModIt(passwordChunk.Item(const_IPv6 + tempCount) + Val(passwordChunk.Item(countOrder)) + Val(passwordChunk.Item(countTotal)), IPv6.Count)
                                If answer >= IPv6.Count Then answer = answer - IPv6.Count
                                cryptedPhrase = IPv6.Item(answer)
                            End If
                            phrase2 = phrase2 & cryptedPhrase & ":"

                            Dim signature2 As Integer = Nothing
                            Dim flipPositiveNegative2 As Boolean = True
                            If command.ToLower = "encrypt" Then
                                For Each c As Char In item.ToString
                                    Dim temp As Long = Nothing
                                    temp = Val(passwordChunk.Item(numSign + Integer.Parse(c))) + Val(passwordChunk.Item(numSignOrder))
                                    If signature2 = Nothing Then
                                        signature2 = temp
                                    Else
                                        If flipPositiveNegative2 Then
                                            signature2 = signature2 + temp
                                        Else
                                            signature2 = signature2 - temp
                                        End If
                                    End If
                                    numSignOrder = numSignOrder + 1
                                    flipPositiveNegative2 = Not flipPositiveNegative2
                                Next
                                numSignOrder = const_CountOrder
                                'The signature will be empty the first time. Here is where it is assigned:
                                If signature = Nothing Then
                                    signature = signature2
                                Else
                                    'If positive, add the new word's positional number by positive, or else subtract it:
                                    If flipPositiveNegative Then
                                        signature = signature + signature2
                                    Else
                                        signature = signature - signature2
                                    End If
                                End If

                                'Flip the value of manipulating the signature by + or - as being True or False respectively:
                                flipPositiveNegative = Not flipPositiveNegative

                            ElseIf command.ToLower = "decrypt" Then
                                For Each c As Char In cryptedPhrase.ToString
                                    Dim temp As Long = Nothing
                                    temp = -Val(passwordChunk.Item(numSign + Integer.Parse(c))) - Val(passwordChunk.Item(numSignOrder))
                                    If signature2 = Nothing Then
                                        signature2 = temp
                                    Else
                                        If flipPositiveNegative2 Then
                                            signature2 = signature2 + temp
                                        Else
                                            signature2 = signature2 - temp
                                        End If
                                    End If
                                    numSignOrder = numSignOrder + 1
                                    flipPositiveNegative2 = Not flipPositiveNegative2
                                Next
                                numSignOrder = const_CountOrder
                                If signature = Nothing Then
                                    signature = signature2
                                Else
                                    'If positive, add the new word's positional number by positive, or else subtract it:
                                    If flipPositiveNegative Then
                                        signature = signature + signature2
                                    Else
                                        signature = signature - signature2
                                    End If
                                End If

                                'Flip the value of manipulating the signature by + or - as being True or False respectively:
                                flipPositiveNegative = Not flipPositiveNegative
                            End If

                            tempCount = tempCount + 1
                        Next
                        Dim phrase3 = Nothing
                        For Each item In phrase2.Substring(0, phrase2.Length - 1).Split(New Char() {":"c}) 'Converts to shorthand hex:
                            phrase3 = phrase3 & ":" & Hex(item)
                        Next
                        phrase0 = phrase3.ToString.Substring(1).ToLower
                        Do
                            If phrase0.Contains(":0:0:0:0:0:0:") Then
                                phrase0 = Replace(phrase0, ":0:0:0:0:0:0:", "::", , 1)
                                Exit Do
                            ElseIf phrase0.Contains(":0:0:0:0:0:") Then
                                phrase0 = Replace(phrase0, ":0:0:0:0:0:", "::", , 1)
                                Exit Do
                            ElseIf phrase0.Contains(":0:0:0:0:") Then
                                phrase0 = Replace(phrase0, ":0:0:0:0:", "::", , 1)
                                Exit Do
                            ElseIf phrase0.Contains(":0:0:0:") Then
                                phrase0 = Replace(phrase0, ":0:0:0:", "::", , 1)
                                Exit Do
                            ElseIf phrase0.Contains(":0:0:") Then
                                phrase0 = Replace(phrase0, ":0:0:", "::", , 1)
                            End If
                        Loop While False
                        phrase = phrase & phrase0
                        Continue For
                    End If

                    'The word in the message is not a number or IP address (assuming it is now a word):
                Else

                    'Count the number of dots found (since dots were excluded from the delimiter):
                    Dim amountOfDots = word.Count(Function(c As Char) c = ".")
                    If word.Contains(".") Then
                        If word.Substring(0, 1) = "." Then
                            dotFound = 1
                        Else
                            dotFound = 2
                        End If
                        word = word.Replace(".", Nothing)
                    End If

                    If command.ToLower = "encrypt" AndAlso word.Contains(":::") Then
                        MsgBox("Sliicy uses "":::"" to distinguish between new words and regular words. Your message cannot contain three (3) consecutive colons.", MsgBoxStyle.OkOnly + MsgBoxStyle.Critical, "Triple Colon Error")
                        Exit Sub
                    End If

                    'Count the number of colons found (since colons were excluded from the delimiter):
                    Dim amountOfColons = word.Count(Function(c As Char) c = ":")

                    'If there is a colon in the word:
                    If word.Contains(":") Then

                        'If the word is a new word and also additional colons:
                        If word.Contains(":::") AndAlso word.Split(":").Length - 1 > 3 Then
                            If word.Last = ":" Then
                                colonFound = 2
                            Else
                                colonFound = 1
                            End If

                            'If the word only has 3 colons:
                        ElseIf word.Contains(":::") AndAlso word.Split(":").Length - 1 = 3 Then
                            colonFound = 0

                            'If the word has colons in general:
                        Else
                            If word.Substring(0, 1) = ":" Then
                                colonFound = 1
                            Else
                                colonFound = 2
                            End If
                        End If
                        If Not word.Contains(":::") Then
                            word = word.Replace(":", Nothing)
                        End If
                    End If

                    'Replace 'an' with 'a' (stripped down when encrypted):
                    If word.ToLower = "an" Then word = "a"

                    'Check if the word is misspelled (ie: ive instead of I've) categorically by its character length, and fix the spelling:
                    If word.Length = 2 Then
                        word = Replace(word, "im", "I'm", 1,, CompareMethod.Text)
                    ElseIf word.Length = 3 Then
                        word = Replace(word, "ive", "I've", 1,, CompareMethod.Text)
                        word = Replace(word, "hed", "he'd", 1,, CompareMethod.Text)
                        word = Replace(word, "hes", "he's", 1,, CompareMethod.Text)
                        word = Replace(word, "itd", "it'd", 1,, CompareMethod.Text)
                    ElseIf word.Length = 4 Then
                        word = Replace(word, "aint", "ain't", 1,, CompareMethod.Text)
                        word = Replace(word, "cant", "can't", 1,, CompareMethod.Text)
                        word = Replace(word, "dont", "don't", 1,, CompareMethod.Text)
                        word = Replace(word, "isnt", "isn't", 1,, CompareMethod.Text)
                        word = Replace(word, "itll", "it'll", 1,, CompareMethod.Text)
                        word = Replace(word, "lets", "let's", 1,, CompareMethod.Text)
                        word = Replace(word, "shes", "she's", 1,, CompareMethod.Text)
                        word = Replace(word, "weve", "we've", 1,, CompareMethod.Text)
                        word = Replace(word, "whos", "who's", 1,, CompareMethod.Text)
                        word = Replace(word, "wont", "won't", 1,, CompareMethod.Text)
                        word = Replace(word, "yall", "y'all", 1,, CompareMethod.Text)
                        word = Replace(word, "youd", "you'd", 1,, CompareMethod.Text)
                    ElseIf word.Length = 5 Then
                        word = Replace(word, "arent", "aren't", 1,, CompareMethod.Text)
                        word = Replace(word, "didnt", "didn't", 1,, CompareMethod.Text)
                        word = Replace(word, "hadnt", "hadn't", 1,, CompareMethod.Text)
                        word = Replace(word, "hasnt", "hasn't", 1,, CompareMethod.Text)
                        word = Replace(word, "thats", "that's", 1,, CompareMethod.Text)
                        word = Replace(word, "theyd", "they'd", 1,, CompareMethod.Text)
                        word = Replace(word, "wasnt", "wasn't", 1,, CompareMethod.Text)
                        word = Replace(word, "youll", "you'll", 1,, CompareMethod.Text)
                        word = Replace(word, "youre", "you're", 1,, CompareMethod.Text)
                        word = Replace(word, "youve", "you've", 1,, CompareMethod.Text)
                    ElseIf word.Length = 6 Then
                        word = Replace(word, "doesnt", "doesn't", 1,, CompareMethod.Text)
                        word = Replace(word, "havent", "haven't", 1,, CompareMethod.Text)
                        word = Replace(word, "mustve", "must've", 1,, CompareMethod.Text)
                        word = Replace(word, "mustnt", "mustn't", 1,, CompareMethod.Text)
                        word = Replace(word, "theres", "there's", 1,, CompareMethod.Text)
                        word = Replace(word, "theyll", "they'll", 1,, CompareMethod.Text)
                        word = Replace(word, "theyre", "they're", 1,, CompareMethod.Text)
                        word = Replace(word, "theyve", "they've", 1,, CompareMethod.Text)
                        word = Replace(word, "werent", "weren't", 1,, CompareMethod.Text)
                        word = Replace(word, "wheres", "where's", 1,, CompareMethod.Text)
                    ElseIf word.Length = 7 Then
                        word = Replace(word, "couldve", "could've", 1,, CompareMethod.Text)
                        word = Replace(word, "couldnt", "couldn't", 1,, CompareMethod.Text)
                        word = Replace(word, "mightve", "might've", 1,, CompareMethod.Text)
                        word = Replace(word, "nobodys", "nobody's", 1,, CompareMethod.Text)
                        word = Replace(word, "wouldve", "would've", 1,, CompareMethod.Text)
                        word = Replace(word, "wouldnt", "wouldn't", 1,, CompareMethod.Text)
                    ElseIf word.Length = 8 Then
                        word = Replace(word, "shouldve", "should've", 1,, CompareMethod.Text)
                        word = Replace(word, "shouldnt", "shouldn't", 1,, CompareMethod.Text)
                        word = Replace(word, "someones", "someone's", 1,, CompareMethod.Text)
                    ElseIf word.Length = 9 Then
                        word = Replace(word, "everyones", "everyone's", 1,, CompareMethod.Text)
                        word = Replace(word, "somebodys", "somebody's", 1,, CompareMethod.Text)
                    ElseIf word.Length = 10 Then
                        word = Replace(word, "everybodys", "everybody's", 1,, CompareMethod.Text)
                    End If

                    'Create a variable that will tell us if the word was found yet in the wordlists:
                    Dim found = False

                    Do
                        If word.Length = 1 AndAlso Not Char.IsLetter(word) Then Exit Do
                        Dim countTypes = const_Types
                        For inspectedGroup = 0 To wordList1.Count - 1
                            For inspectedWord = 0 To wordList1(inspectedGroup).Count - 1
                                If wordList1(inspectedGroup)(inspectedWord).ToLower = word.ToLower Then

                                    'If encryption was requested:
                                    If command.ToLower = "encrypt" Then

                                        'The signature will be empty the first time. Here is where it is assigned:
                                        If signature = Nothing Then

                                            'Set signature to the index of the current word in the big list of words:
                                            signature = Enumerable.Range(0, bigListOfAllWords.Count).Where(Function(f) bigListOfAllWords(f).ToLower = word.ToLower).ToArray(0)
                                        Else

                                            'If positive, add the new word's positional number by positive, or else subtract it:
                                            If flipPositiveNegative Then
                                                signature = signature + Enumerable.Range(0, bigListOfAllWords.Count).Where(Function(f) bigListOfAllWords(f).ToLower = word.ToLower).ToArray(0)
                                            Else
                                                signature = signature - Enumerable.Range(0, bigListOfAllWords.Count).Where(Function(f) bigListOfAllWords(f).ToLower = word.ToLower).ToArray(0)
                                            End If
                                        End If

                                        'Flip the value of manipulating the signature by + or - as being True or False respectively:
                                        flipPositiveNegative = Not flipPositiveNegative
                                    End If
                                    Dim addDots = Nothing
                                    For i = 0 To amountOfDots - 1
                                        addDots = addDots & "."
                                    Next
                                    Dim addColons = Nothing
                                    For i = 0 To amountOfColons - 1
                                        addColons = addColons & ":"
                                    Next
                                    Dim ss1 = Nothing
                                    Dim ss2 = Nothing
                                    If dotFound = 1 Then
                                        ss1 = ss1 & addDots
                                    ElseIf dotFound = 2 Then
                                        ss2 = ss2 & addDots
                                    End If
                                    If colonFound = 1 Then
                                        ss1 = ss1 & addColons
                                    ElseIf colonFound = 2 Then
                                        ss2 = ss2 & addColons
                                    End If
                                    Dim cryptedPhrase As String = Nothing
                                    Try
                                        If Val(passwordChunk.Item(countTypes)) + Val(passwordChunk.Item(countOrder) + Val(passwordChunk.Item(countTotal)) + Val(passwordChunk.Item(countSymbol + total))) >= 0 Then
                                            cryptedPhrase = wordList1(inspectedGroup).Item(Val((inspectedWord) + Val(passwordChunk.Item(countTypes)) + Val(passwordChunk.Item(countOrder)) + Val(passwordChunk.Item(countTotal)) + Val(passwordChunk.Item(countSymbol + total))) Mod wordList1(inspectedGroup).Count)

                                            'Determines if the word was UPPERCASE, Normal, lowercase or neither:
                                            If word.ToUpper(Globalization.CultureInfo.InvariantCulture) = word Then
                                                phrase = phrase & ss1 & cryptedPhrase.ToUpper & ss2
                                            Else
                                                phrase = phrase & ss1 & cryptedPhrase & ss2
                                            End If

                                        Else 'Negative Password (like -5)
                                            Dim answer As Long = inspectedWord + ModIt(Val(passwordChunk.Item(countTypes)) + Val(passwordChunk.Item(countOrder)) + Val(passwordChunk.Item(countTotal)) + Val(passwordChunk.Item(countSymbol + total)), wordList1(inspectedGroup).Count)
                                            If answer >= wordList1(inspectedGroup).Count Then
                                                answer = answer - wordList1(inspectedGroup).Count
                                            End If
                                            cryptedPhrase = wordList1(inspectedGroup).Item(answer)

                                            'Determines if the word was UPPERCASE, Normal, lowercase or neither:
                                            If word.ToUpper(Globalization.CultureInfo.InvariantCulture) = word Then
                                                phrase = phrase & ss1 & cryptedPhrase.ToUpper & ss2
                                            Else
                                                phrase = phrase & ss1 & cryptedPhrase & ss2
                                            End If
                                        End If
                                    Catch ex As Exception
                                        MsgBox("The selected contact is either corrupt or not a contact.", MsgBoxStyle.OkOnly + MsgBoxStyle.Critical, "Incorrect file chosen")
                                        Exit Sub
                                    End Try

                                    'Word was found:
                                    found = True

                                    If command.ToLower = "decrypt" Then

                                        If signature = Nothing Then

                                            signature = Enumerable.Range(0, bigListOfAllWords.Count).Where(Function(f) bigListOfAllWords(f).ToLower = cryptedPhrase.ToLower).ToArray(0)
                                        Else

                                            'If positive, add the new word's positional number by positive, or else subtract it:
                                            If flipPositiveNegative Then
                                                signature = signature + Enumerable.Range(0, bigListOfAllWords.Count).Where(Function(f) bigListOfAllWords(f).ToLower = cryptedPhrase.ToLower).ToArray(0)
                                            Else
                                                signature = signature - Enumerable.Range(0, bigListOfAllWords.Count).Where(Function(f) bigListOfAllWords(f).ToLower = cryptedPhrase.ToLower).ToArray(0)
                                            End If
                                        End If

                                        'Flip the value of manipulating the signature by + or - as being True or False respectively:
                                        flipPositiveNegative = Not flipPositiveNegative
                                    End If

                                    'How strong is this encryption:
                                    strength = strength * wordList1(inspectedGroup).Count
                                    If Not readMode AndAlso Not command.ToLower = "decrypt" Then
                                        wordList2(inspectedGroup).Remove(word.ToLower)
                                        wordList2(inspectedGroup).Insert(0, word)
                                    End If
                                    Exit Do
                                End If
                            Next
                            countTypes = countTypes + 1
                        Next
                    Loop While False
                    If Not found Then
                        If word.Length > 1 AndAlso word <> Environment.NewLine Then

                            'Colonized will be true if ::: is in the word:
                            Dim colonized = False
                            If word.Contains(":::") Then colonized = True
                            word = word.Replace(":::", Nothing)
                            Dim cryptedPhrase As String = Nothing
                            Dim temp2 As Integer = 0
                            For Each c As Char In word
                                For i = 0 To letterList.Count - 1
                                    If letterList(i) = c Then
                                        If Val(passwordChunk.Item(const_CountOrder + temp2) + Val(passwordChunk.Item(countTotal)) + Val(passwordChunk.Item(countSymbol + total))) >= 0 Then
                                            cryptedPhrase = cryptedPhrase & letterList(Val(i + Val(passwordChunk.Item(const_CountOrder + temp2)) + Val(passwordChunk.Item(countTotal)) + Val(passwordChunk.Item(countSymbol + total))) Mod letterList.Count)
                                        Else 'Negative Password (like -5)
                                            Dim answer As Long = i + ModIt(Val(passwordChunk.Item(const_CountOrder + temp2)) + Val(passwordChunk.Item(countTotal)) + Val(passwordChunk.Item(countSymbol + total)), letterList.Count)
                                            If answer >= letterList.Count Then
                                                answer = answer - letterList.Count
                                            End If
                                            cryptedPhrase = cryptedPhrase & letterList(answer)
                                        End If
                                        If temp2 < 256 Then temp2 = temp2 + 1
                                    End If
                                Next
                            Next
                            Dim addDots = Nothing
                            Dim addColons = Nothing
                            If dotFound = 0 AndAlso colonFound = 0 Then
                                If command.ToLower = "encrypt" Then
                                    phrase = phrase & ":::" & cryptedPhrase
                                Else
                                    phrase = phrase & cryptedPhrase
                                End If
                            Else
                                For j = 0 To amountOfDots - 1
                                    addDots = addDots & "."
                                Next
                                If colonized Then
                                    For j = 0 To amountOfColons - 4 '-1 and -3 because of :::
                                        addColons = addColons & ":"
                                    Next
                                Else
                                    For j = 0 To amountOfColons - 1
                                        addColons = addColons & ":"
                                    Next
                                End If
                                If dotFound = 1 Then
                                    If command.ToLower = "encrypt" Then
                                        phrase = phrase & addDots & ":::" & cryptedPhrase
                                    Else
                                        phrase = phrase & addDots & cryptedPhrase
                                    End If
                                ElseIf dotFound = 2 Then
                                    If command.ToLower = "encrypt" Then
                                        phrase = phrase & ":::" & cryptedPhrase & addDots
                                    Else
                                        phrase = phrase & cryptedPhrase & addDots
                                    End If
                                End If
                                If colonFound = 1 Then
                                    If command.ToLower = "encrypt" Then
                                        phrase = phrase & addColons & ":::" & cryptedPhrase
                                    Else
                                        phrase = phrase & addColons & cryptedPhrase
                                    End If
                                ElseIf colonFound = 2 Then
                                    If command.ToLower = "encrypt" Then
                                        phrase = phrase & ":::" & cryptedPhrase & addColons
                                    Else
                                        phrase = phrase & cryptedPhrase & addColons
                                    End If
                                End If
                            End If
                            Dim signature2 As Integer = Nothing
                            Dim flipPositiveNegative2 As Boolean = True
                            If command.ToLower = "encrypt" Then

                                'Add new unknown word to custom dictionary in contact file:
                                addedWordList.Add(word)

                                'Add new unknown word to the end of the big list of words:
                                bigListOfAllWords.Add(word)

                                For Each c As Char In word.ToString
                                    Dim temp As Long = Nothing
                                    Dim characterStart As Integer = 0
                                    If c.ToString.ToUpper = c Then
                                        characterStart = AscW(c) - 65
                                    Else
                                        characterStart = AscW(c) - 71
                                    End If
                                    temp = Val(passwordChunk.Item(letterSign + characterStart)) + Val(passwordChunk.Item(numSignOrder))
                                    If signature2 = Nothing Then
                                        signature2 = temp
                                    Else
                                        If flipPositiveNegative2 Then
                                            signature2 = signature2 + temp
                                        Else
                                            signature2 = signature2 - temp
                                        End If
                                    End If
                                    numSignOrder = numSignOrder + 1
                                    flipPositiveNegative2 = Not flipPositiveNegative2
                                Next
                                numSignOrder = const_CountOrder
                                'The signature will be empty the first time. Here is where it is assigned:
                                If signature = Nothing Then
                                    signature = signature2
                                Else
                                    'If positive, add the new word's positional number by positive, or else subtract it:
                                    If flipPositiveNegative Then
                                        signature = signature + signature2
                                    Else
                                        signature = signature - signature2
                                    End If
                                End If

                                'Flip the value of manipulating the signature by + or - as being True or False respectively:
                                flipPositiveNegative = Not flipPositiveNegative

                            ElseIf command.ToLower = "decrypt" Then

                                'Add new unknown word to custom dictionary in contact file:
                                addedWordList.Add(cryptedPhrase)

                                'Add new unknown word to the end of the big list of words:
                                bigListOfAllWords.Add(cryptedPhrase)

                                For Each c As Char In cryptedPhrase.ToString
                                    Dim temp As Long = Nothing
                                    Dim characterStart As Integer = 0
                                    If c.ToString.ToUpper = c Then
                                        characterStart = AscW(c) - 65
                                    Else
                                        characterStart = AscW(c) - 71
                                    End If
                                    temp = -Val(passwordChunk.Item(letterSign + characterStart)) - Val(passwordChunk.Item(numSignOrder))
                                    If signature2 = Nothing Then
                                        signature2 = temp
                                    Else
                                        If flipPositiveNegative2 Then
                                            signature2 = signature2 + temp
                                        Else
                                            signature2 = signature2 - temp
                                        End If
                                    End If
                                    numSignOrder = numSignOrder + 1
                                    flipPositiveNegative2 = Not flipPositiveNegative2
                                Next
                                numSignOrder = const_CountOrder
                                If signature = Nothing Then
                                    signature = signature2
                                Else
                                    'If positive, add the new word's positional number by positive, or else subtract it:
                                    If flipPositiveNegative Then
                                        signature = signature + signature2
                                    Else
                                        signature = signature - signature2
                                    End If
                                End If

                                'Flip the value of manipulating the signature by + or - as being True or False respectively:
                                flipPositiveNegative = Not flipPositiveNegative
                            End If
                            addedWordList = addedWordList.Distinct(StringComparer.CurrentCultureIgnoreCase).ToList
                            bigListOfAllWords = bigListOfAllWords.Distinct(StringComparer.CurrentCultureIgnoreCase).ToList
                        Else
                            Do
                                Dim temp As Long = Nothing
                                Dim charTemp As Integer = Nothing
                                If word = "`" Then
                                    charTemp = 0
                                ElseIf word = "~" Then
                                    charTemp = 1
                                ElseIf word = "!" Then
                                    charTemp = 2
                                ElseIf word = "@" Then
                                    charTemp = 3
                                ElseIf word = "#" Then
                                    charTemp = 4
                                ElseIf word = "$" Then
                                    charTemp = 5
                                ElseIf word = "%" Then
                                    charTemp = 6
                                ElseIf word = "^" Then
                                    charTemp = 7
                                ElseIf word = "&" Then
                                    charTemp = 8
                                ElseIf word = "*" Then
                                    charTemp = 9
                                ElseIf word = "(" Then
                                    charTemp = 10
                                ElseIf word = ")" Then
                                    charTemp = 11
                                ElseIf word = "_" Then
                                    charTemp = 12
                                ElseIf word = "+" Then
                                    charTemp = 13
                                ElseIf word = "-" Then
                                    charTemp = 14
                                ElseIf word = "=" Then
                                    charTemp = 15
                                ElseIf word = "{" Then
                                    charTemp = 16
                                ElseIf word = "}" Then
                                    charTemp = 17
                                ElseIf word = "|" Then
                                    charTemp = 18
                                ElseIf word = "[" Then
                                    charTemp = 19
                                ElseIf word = "]" Then
                                    charTemp = 20
                                ElseIf word = "\" Then
                                    charTemp = 21
                                ElseIf word = ":" Then
                                    charTemp = 22
                                ElseIf word = """" Then
                                    charTemp = 23
                                ElseIf word = ";" Then
                                    charTemp = 24
                                ElseIf word = "'" Then
                                    charTemp = 25
                                ElseIf word = "<" Then
                                    charTemp = 26
                                ElseIf word = ">" Then
                                    charTemp = 27
                                ElseIf word = "?" Then
                                    charTemp = 28
                                ElseIf word = "," Then
                                    charTemp = 29
                                ElseIf word = "." Then
                                    charTemp = 30
                                ElseIf word = "/" Then
                                    charTemp = 31
                                ElseIf word = " " Then
                                    charTemp = 32
                                Else
                                    Exit Do
                                End If
                                If command.ToLower = "encrypt" Then
                                    temp = Val(passwordChunk.Item(symSign + charTemp)) + Val(passwordChunk.Item(numSignOrder))
                                ElseIf command.ToLower = "decrypt" Then
                                    temp = -Val(passwordChunk.Item(symSign + charTemp)) - Val(passwordChunk.Item(numSignOrder))
                                End If
                                If signature = Nothing Then
                                    signature = temp
                                Else
                                    If flipPositiveNegative Then
                                        signature = signature + temp
                                    Else
                                        signature = signature - temp
                                    End If
                                End If
                                flipPositiveNegative = Not flipPositiveNegative
                            Loop While False
                            If dotFound = 0 AndAlso colonFound = 0 Then
                                phrase = phrase & word
                            Else
                                Dim addDots = Nothing
                                For i = 0 To amountOfDots - 1
                                    addDots = addDots & "."
                                Next
                                If dotFound = 1 Then
                                    phrase = phrase & addDots & word
                                ElseIf dotFound = 2 Then
                                    phrase = phrase & word & addDots
                                End If
                                Dim addColons = Nothing
                                For i = 0 To amountOfColons - 1
                                    addColons = addColons & ":"
                                Next
                                If colonFound = 1 Then
                                    phrase = phrase & addColons & word
                                ElseIf colonFound = 2 Then
                                    phrase = phrase & word & addColons
                                End If
                            End If
                        End If
                    End If
                End If
                If countOrder < 268 Then countOrder = countOrder + 1
            Next

            'Autocorrect the crypted message with 'a' and 'an':
            If phrase.Substring(0, 2).ToLower = "a " AndAlso (phrase.Substring(2, 1).ToLower = "a" OrElse phrase.Substring(2, 1).ToLower = "e" OrElse phrase.Substring(2, 1).ToLower = "i" OrElse phrase.Substring(2, 1).ToLower = "o" OrElse phrase.Substring(2, 1).ToLower = "u") Then
                phrase = "An" & phrase.Substring(1)
            End If

            'First look for capitalized characters:
            phrase = Replace(phrase, " a A", " an A")
            phrase = Replace(phrase, " a E", " an E")
            phrase = Replace(phrase, " a I", " an I")
            phrase = Replace(phrase, " a O", " an O")
            phrase = Replace(phrase, " a U", " an U")

            'Look for the rest of the characters:
            phrase = System.Text.RegularExpressions.Regex.Replace(phrase, " a a", " an a", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            phrase = System.Text.RegularExpressions.Regex.Replace(phrase, " a e", " an e", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            phrase = System.Text.RegularExpressions.Regex.Replace(phrase, " a i", " an i", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            phrase = System.Text.RegularExpressions.Regex.Replace(phrase, " a o", " an o", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            phrase = System.Text.RegularExpressions.Regex.Replace(phrase, " a u", " an u", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            phrase = phrase.Substring(0, 1).ToUpper & phrase.Substring(1)

            'Console.ForegroundColor = ConsoleColor.Green
            'If phrase = Nothing Then
            'Console.WriteLine(Environment.NewLine & "No possible combinations for being cracked!" & Environment.NewLine)
            'Else
            'Dim trailingS = "s"
            'If strength = 1 Then trailingS = Nothing
            'Console.WriteLine(Environment.NewLine & strength.ToString("N0") & " possible combination" & trailingS & " for being cracked!" & Environment.NewLine)
            'End If
            'Console.ForegroundColor = ConsoleColor.Yellow
            TextBox2.Text = Nothing
            If command.ToLower = "decrypt" Then
                'Console.WriteLine(Environment.NewLine & "The calculated signature: " & signature)
                TextBox2.Text = Environment.NewLine & "The calculated signature: " & signature
                'Verify the authorship (signature) of the message:
                If signature = verifySignature Then
                    'Console.ForegroundColor = ConsoleColor.Green
                    'Console.WriteLine("The signature of message: " & verifySignature)
                    TextBox2.Text = TextBox2.Text & Environment.NewLine & "The message signature: " & verifySignature
                    'Console.WriteLine(Environment.NewLine & "The signatures appear PERFECT, however be mindful if the sentence sounds jarbled up (signs of tampering)!")
                Else
                    'Console.ForegroundColor = ConsoleColor.Red
                    'Console.WriteLine("The signature of message: " & verifySignature)
                    TextBox2.ForeColor = Color.Red
                    TextBox2.Text = TextBox2.Text & Environment.NewLine & "The message signature: " & verifySignature
                    Dim d As DialogResult = MsgBox("The signature was forged or corrupted! It is HIGHLY likely that the message was tampered with. Proceed anyways?", MsgBoxStyle.YesNoCancel + MsgBoxStyle.Exclamation + MessageBoxDefaultButton.Button2, "Warning")
                    If Not d = DialogResult.Yes Then
                        MsgBox("The decryption was aborted. Nothing was changed.", MsgBoxStyle.OkOnly + MsgBoxStyle.Information, "Aborted")
                        Exit Sub
                    End If
                    'Console.WriteLine(Environment.NewLine & "WARNING!!! The signature was forged or corrupted! It is HIGHLY likely that the message was tampered with. Proceed anyways?")
                    'Console.WriteLine(Environment.NewLine & "Y or N")
                    'Console.ForegroundColor = ConsoleColor.White
                    'Dim response = Console.ReadLine
                    'If Not (response.ToLower = "y" Or response.ToLower = "yes") Then
                    'Console.WriteLine("Decryption aborted! No dictionaries were altered!")
                    'Exit Sub
                    'End If
                End If
                'Console.WriteLine("Decrypted message:" & Environment.NewLine)
                'Console.ForegroundColor = ConsoleColor.DarkGray
            Else
                'Console.WriteLine("Encrypted message (send to your friend):" & Environment.NewLine)
                'Console.ForegroundColor = ConsoleColor.Green

                'Add the signature of the message to the encrypted message:
                phrase = phrase & " " & signature
            End If

            'Write the message (encrypted or decrypted):
            TextBox2.Text = phrase.TrimStart(" ").TrimEnd(" ") & Environment.NewLine & TextBox2.Text
            'Console.WriteLine(phrase.TrimStart(" ").TrimEnd(" ") & Environment.NewLine)
            'Console.ForegroundColor = ConsoleColor.White
            If command.ToLower = "decrypt" AndAlso Not readMode Then

                Dim d As DialogResult = OutputForm.ShowDialog
                If Not d = DialogResult.Yes Then
                    MsgBox("The decryption was aborted. Nothing was changed.", MsgBoxStyle.OkOnly + MsgBoxStyle.Information, "Aborted")
                    Exit Sub
                End If
                'Console.ForegroundColor = ConsoleColor.Yellow
                'Console.WriteLine(Environment.NewLine & "Does this message look like something your friend would say? (If the above decrypted message makes no sense, either a hacker gave you the message or your dictionaries are out of sync!)")
                'Console.WriteLine(Environment.NewLine & "Y or N")
                'Console.ForegroundColor = ConsoleColor.White
                'Dim response = Console.ReadLine
                'If Not (response.ToLower = "y" Or response.ToLower = "yes") Then
                'Console.WriteLine("Decryption aborted! No dictionaries were altered!")
                'Exit Sub
                'End If
            End If

            'Copy the message to the clipboard:
            If phrase <> Nothing Then My.Computer.Clipboard.SetText(phrase)
            'Console.WriteLine(Environment.NewLine & "Message copied to clipboard!")
            If Not readMode Then
                If command.ToLower = "decrypt" Then
                    Dim delimiters2 = New List(Of String)() From {" ", "~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "=", "+", ",", "<", ".", ">", "/", "?", ";", ":", """", "[", "{", "]", "}", "\", "|", Environment.NewLine}
                    Dim pattern2 As String = "(" + String.Join("|", delimiters2.[Select](Function(d) System.Text.RegularExpressions.Regex.Escape(d)).ToArray()) + ")"
                    Dim output2 As String() = System.Text.RegularExpressions.Regex.Split(phrase, pattern2)
                    For Each word In output2
                        For l = 0 To wordList1.Count - 1
                            For m = 0 To wordList1(l).Count - 1
                                If wordList1(l)(m).ToLower = word.ToLower Then
                                    wordList2(l).Remove(word.ToLower)
                                    wordList2(l).Insert(0, word)
                                End If
                            Next
                        Next
                    Next
                End If
                Dim final As String = Nothing
                For i = 0 To wordList2.Count - 1
                    wordList2(i) = wordList2(i).Distinct(StringComparer.CurrentCultureIgnoreCase).ToList()
                    final = final & Environment.NewLine & Environment.NewLine & String.Join(Environment.NewLine, wordList2(i).ToArray())
                Next
                final = final.Substring(4)
                If addedWordList.Count > 0 Then
                    addedWordList = addedWordList.Distinct(StringComparer.CurrentCultureIgnoreCase).ToList()
                    final = final & Environment.NewLine & String.Join(Environment.NewLine, addedWordList.ToArray)
                End If
                My.Computer.FileSystem.WriteAllText(user, lineSplit(0) & Environment.NewLine & Environment.NewLine & lineSplit(1) & Environment.NewLine & Environment.NewLine & final, False, System.Text.Encoding.UTF8)
            End If
        ElseIf command.ToLower = "change" Then
            If user.Length = 0 Or newPassword.Length = 0 Then
                Exit Sub
            End If
            charCount = 0
            sumTotal = 0
            For Each c As Char In newPassword
                sumTotal = Val(sumTotal + AscW(c) * charCount)
                If Char.IsLetter(c) Then
                    charCount = charCount + 1
                End If
            Next
            If charCount Mod 2 > 0 Then
                sumTotal = -sumTotal
            End If
            newPassword = sumTotal * charCount
            If My.Computer.FileSystem.DirectoryExists(user) Then
                Dim d As DialogResult = MsgBox("All text files inside """ & user & """ may be modified! Proceed?", MsgBoxStyle.YesNoCancel + MsgBoxStyle.Exclamation + MessageBoxDefaultButton.Button2, "Warning")
                If Not d = DialogResult.Yes Then Exit Sub
                For Each file As String In IO.Directory.GetFiles(user)

                    'Check if the contact is a text file
                    If file.Contains(".txt") Then

                        'Split each contact in half. One half are the numbers and the other are the wordlists
                        Dim lineSplit = IO.File.ReadAllText(file).Split(delim, StringSplitOptions.None)
                        'Check if each contact is indeed from Sliicy (if it contains semicolons and the word "Sliicy")
                        If IO.File.ReadAllText(file).Contains(";") AndAlso IO.File.ReadAllText(file).Contains("Sliicy") Then
                            'Set the Master Password to the numbers (the first half of the file)
                            masterPassword = lineSplit(0)
                            Dim computation As String = Nothing
                            For Each passwordFound As String In masterPassword.Split({";"c}, StringSplitOptions.RemoveEmptyEntries)
                                'Decrypt and Encrypt Passwords:
                                computation = computation & (Val(passwordFound) - Val(password) + Val(newPassword)) & ";"
                            Next

                            'Trim the last extra semicolon:
                            lineSplit(0) = computation.Substring(0, computation.Length - 1)
                            'Write all changes to each contact found (new numbers and untouched wordlists)
                            IO.File.WriteAllText(file, String.Join(Environment.NewLine & Environment.NewLine, lineSplit.ToArray), System.Text.Encoding.UTF8)
                        End If
                    End If
                Next
            ElseIf My.Computer.FileSystem.FileExists(user) Then

                'Split the contact file in half. One half are the numbers and the other are the wordlists
                Dim lineSplit = IO.File.ReadAllText(user).Split(delim, StringSplitOptions.None)
                If IO.File.ReadAllText(user).Contains(";") AndAlso IO.File.ReadAllText(user).Contains("Sliicy") Then
                    'Set the Master Password to the numbers (the first half of the file)
                    masterPassword = lineSplit(0)
                    Dim computation As String = Nothing
                    For Each passwordFound As String In masterPassword.Split({";"c}, StringSplitOptions.RemoveEmptyEntries)
                        'Decrypt and Encrypt Passwords:
                        computation = computation & (Val(passwordFound) - Val(password) + Val(newPassword)) & ";"
                    Next

                    'Trim the last extra semicolon:
                    lineSplit(0) = computation.Substring(0, computation.Length - 1)
                    'Write all changes to the contact (new numbers and untouched wordlists)
                    IO.File.WriteAllText(user, String.Join(Environment.NewLine & Environment.NewLine, lineSplit.ToArray), System.Text.Encoding.UTF8)
                End If
            Else
                'Whatever was tried didn't work. Maybe because the contact can't be written to, found, etc...
                'Console.ForegroundColor = ConsoleColor.Red
                'Console.WriteLine(Environment.NewLine & "The contact or folder " & user & " doesn't exist!")
            End If
        Else
            'Console.ForegroundColor = ConsoleColor.Red
            'Console.WriteLine(Environment.NewLine & "Unknown command! Valid commands are: change, create, decrypt, encrypt, or join.")
        End If
    End Sub
End Class
