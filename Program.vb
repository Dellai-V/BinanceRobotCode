'https://github.com/Dellai-V/Binance-Robot-Wallet
Module Program
    Sub Main(args As String())
        Dim timeChart As DateTime = DateTime.Now
        Dim timeTrade As DateTime = DateTime.Now.AddSeconds(AppSetting.TimeTrade)

        Console.BackgroundColor = ConsoleColor.Gray
        Console.ForegroundColor = ConsoleColor.Black

        Console.WriteLine("___ Binance Robot Wallet ___")
        Console.WriteLine("Source : https://github.com/Dellai-V/Binance-Robot-Wallet")
        Binance.LoadAPI()
        Binance.OHLC()
        Script.S_priority()
        Binance.Console_App()

        While True
            If timeChart < DateTime.Now Then
                Binance.OHLC()
                Binance.Console_App()
                timeChart = DateTime.Now.AddSeconds(AppSetting.TimeChart)
            End If
            If timeTrade < DateTime.Now Then
                Binance.GetBalance()
                Script.S_priority()
                Script.StartTrade()
                timeTrade = DateTime.Now.AddSeconds(AppSetting.TimeTrade)
            End If
        End While
    End Sub
End Module
