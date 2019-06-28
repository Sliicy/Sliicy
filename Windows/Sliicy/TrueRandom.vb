Public Class TrueRandom
    ''''''''''''''''''''''''''''''''''''''''
    ''Use this class at will            ''''
    ''but please leave this note attached'''
    '' - Phill64 (Stack Overflow)       ''''
    ''''''''''''''''''''''''''''''''''''''''
    ''True Random
    '''''''''''''
    ''This random class uses 4 factors most likely to change:
    ''System Timer
    ''System Memory (RAM)
    ''CPU Usage
    ''Mouse Position
    ''''''''''''''''''
    ''Why is this class true random and the pre-packaged one isn't you ask?
    ''Because classic uses just the timer as a seed, and a pattern to generate from then on
    ''Whereas, this class only follows patterns for a limited time(long enough for new data to come in)
    ''and the fact it uses RAM and the Mouse Position means it is effected by unknown sources.
    ''the system clock spins circles, so it will of course create a pattern
    ''a user moving his mouse is COMLETELY unpredictable
    ''RAM is effected continuously in ways that can never be predicted(within reason)
    ''CPU performance is effected by everything running and even the weather! thus the CPU can also never be accurately predicted
    '''''''''''
    ''How did I test the results?
    '''''''''''
    ''by creating 2 classes of the classic Random
    ''and 2 classes of TrueRandom
    ''then making all 4 create a list of random numbers
    ''classic random generated 2 IDENTICAL lists every time i tested
    ''whereas the 2 TrueRandom classes would create 2 different lists
    '''''
    ''Conclusion:
    ''this class, created under all the same conditions returns different results
    ''that.. is truely random.

    'these variables make a temporary pattern,
    'long enough for data to change
    Private betTick As Double 'between ticks
    Private lastRAM As Decimal 'last value from memory
    Private tempPat As Integer 'small pattern iterater
    Public Function GetNext(Optional ByRef lower As Integer = 0, Optional ByRef upper As Integer = 2147483647) As Integer
        'get system timer
        betTick = (betTick + Now.Ticks) / 2  'between ticks, gradually move towards next tick of the system clock

        'get system RAM
        Dim RAM As Decimal 'where we'll put information taken from RAM
        Dim i As Integer
        For i = 0 To 40
            RAM = ((RAM + Threading.Thread.VolatileRead(betTick + i)) / 2) 'retrieve next byte and average with last
        Next
        RAM = Mid(((lastRAM + RAM) / 2).ToString, 5) 'average RAM just retrieved with last RAM retrieved
        lastRAM = RAM

        'apply timer, RAM, CPU, and Mouse
        tempPat += 1
        If tempPat > 20 Then tempPat = 1
        Dim v As Double = (RAM / tempPat) + ((betTick / 100) + CPUUsage() + (Cursor.Position.X / Cursor.Position.Y))
        Dim rand As Decimal = (v / 1000) - Math.Floor(v / 1000)

        'ratio the random decimal number to the required bounds
        Return lower + (rand * (upper - lower))
    End Function

    ''CPU Usage code from: http://www.theserverside.net/discussions/thread.tss?thread_id=26337
    Private Function CPUUsage() As Double
        Dim myCounter As Diagnostics.PerformanceCounter = New Diagnostics.PerformanceCounter With {
            .CategoryName = "Processor", .CounterName = "% Processor Time", .InstanceName = "_Total"}
        Return myCounter.RawValue
    End Function
End Class