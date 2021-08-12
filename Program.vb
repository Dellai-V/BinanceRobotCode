'https://github.com/Dellai-V/BinanceRobotCode
Imports System.Threading

Module Program
    Sub Main(args As String())
        Dim timeChart As DateTime = DateTime.Now
        Dim timeTrade As DateTime = DateTime.Now.AddSeconds(AppSetting.TimeTrade)

        Console.WriteLine("========== Binance Robot Code ==========")
        Console.WriteLine("Source : https://github.com/Dellai-V/BinanceRobotCode")
        Binance.LoadAPI()
        Binance.OHLC()
        If AppSetting.OrdersReset = True Then
            Binance.CancelOrders()
        End If
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
            Thread.Sleep(AppSetting.TimeChart)
        End While
    End Sub
End Module
