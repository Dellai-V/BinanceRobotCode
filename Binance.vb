'https://github.com/Dellai-V/BinanceRobotCode
Imports Skender.Stock.Indicators
Imports Binance.Net

'dotnet add package Skender.Stock.Indicators
'dotnet add package Binance.Net

Public Class MARKET
    Public Name As String
    Public Base As Integer
    Public Quote As Integer
    Public MinPrice As Decimal
    Public MinVolume As Decimal
    Public MinNotional As Decimal

    Public Last As Date
    Public Volume As Decimal
    Public Price As Decimal
    Public PriceMin As Decimal
    Public PriceMax As Decimal
    Public Sma As Decimal


    Public Periodo As String
    Public Charts As New List(Of Quote)
End Class

Public Class ASSET
    Public Name As String
    Public Split As String
    Public Balance As Decimal
    Public BalanceFree As Decimal
    Public BalanceIdeal As Decimal
    Public BalanceLending As Decimal
    Public ToBTC As Decimal
End Class

Public Class API
    Public info As CryptoExchange.Net.Objects.WebCallResult(Of Objects.Spot.MarketData.BinanceExchangeInfo)
    Public client As BinanceClient = New BinanceClient()
    Public stato As Boolean = False
    Public APIkey As String
    Public APISecret As String
End Class

Public Class Binance
    Public Shared BTCtot As Decimal
    Public Shared api As New API
    Public Shared asset As New List(Of ASSET)
    Public Shared market As New List(Of MARKET)
#Region "START"
    Public Shared Sub LoadAPI()
        api.APIkey = AppSetting.APIkey
        api.APISecret = AppSetting.APIsecret
        api.stato = False
        If api.APIkey.Length = 64 And api.APISecret.Length = 64 Then
            api.client.SetApiCredentials(api.APIkey, api.APISecret)
            api.info = api.client.Spot.System.GetExchangeInfo()
            Dim accountInfo = api.client.General.GetAccountInfo()
            If accountInfo.Error Is Nothing Then
                api.stato = accountInfo.Data.CanTrade
                Log("Account Type : " & accountInfo.Data.AccountType.ToString & " | Can Trade : " & accountInfo.Data.CanTrade &
                    " | Maker Commission : " & accountInfo.Data.MakerCommission / 100 & "% | Taker Commission : " & accountInfo.Data.TakerCommission / 100 & "%")
                LoadVar()
            Else
                Log("ERROR API : wrong settings")
            End If
        Else
            Log("ERROR API : You have not configured the APIKey or APISecret")
        End If
    End Sub
    Private Shared Sub ResetVar()
        asset.Clear()
        market.Clear()
    End Sub
    Public Shared Sub LoadVar()
        ResetVar()
        If AppSetting.Asset.Count <> AppSetting.Split.Count Then
            Log("ERROR Setting : Asset and Split do not match")
            Exit Sub
        End If
        For x = 0 To AppSetting.Asset.Count - 1
            Dim a As New ASSET
            a.Name = AppSetting.Asset(x)
            a.Split = AppSetting.Split(x)
            asset.Add(a)
        Next
        For x As Integer = 0 To api.info.Data.Symbols.Count - 1
            For y = 0 To asset.Count - 1
                If asset(y).Name = api.info.Data.Symbols(x).BaseAsset Then
                    For z = 0 To asset.Count - 1
                        If asset(z).Name = api.info.Data.Symbols(x).QuoteAsset Then
                            Dim m As New MARKET
                            m.Name = api.info.Data.Symbols(x).Name
                            m.Base = y
                            m.Quote = z
                            m.MinNotional = api.info.Data.Symbols(x).MinNotionalFilter.MinNotional
                            m.MinVolume = api.info.Data.Symbols(x).LotSizeFilter.MinQuantity
                            m.MinPrice = api.info.Data.Symbols(x).PriceFilter.MinPrice
                            m.Periodo = AppSetting.Period
                            market.Add(m)
                        End If
                    Next
                End If
            Next
        Next
        StabilityTest()
        OHLC()
        GetBalance()
    End Sub
    Private Shared Sub StabilityTest()
        Dim x As Integer = market.Count
        Dim y As Integer = asset.Count
        If Not ((y * y) - y) / 2 = x Then
            Log("/!\ WARNING : For correct operation, some assets will cause problems because there is not a market list with all combinations")
        End If
    End Sub
#End Region
#Region "Balance"
    Public Shared Sub GetBalance()
        LendingBalance()
        If api.stato = True Then
            Try
                Dim info = api.client.General.GetAccountInfo()
                If info.Error Is Nothing Then
                    For Each a As ASSET In asset
                        For each D in info.Data.Balances
                            If D.Asset = a.Name Then
                                a.Balance = D.Total
                                a.BalanceFree = D.Free
                                Exit For
                            End If
                        Next
                    Next
                    CalcoloBTC()
                End If
            Catch ex As Exception
                Log("ERROR Balances : " & ex.Message)
            End Try
        End If
    End Sub
    Private Shared Sub LendingBalance()
        If api.stato = True Then
            Try
                Dim info = api.client.Lending.GetLendingAccount()
                If info.Error Is Nothing Then
                    For Each a As ASSET In asset
                        For Each D In info.Data.PositionAmounts
                            If D.Asset = a.Name Then
                                a.BalanceLending = D.Amount
                            End If
                        Next
                    Next
                End If
            Catch ex As Exception
                Log("ERROR Balances : " & ex.Message)
            End Try
        End If
    End Sub
    Private Shared Sub CalcoloBTC()
        If api.stato = True Then
            asset(0).ToBTC = 1 'Valore base 
            For Each m As MARKET In market
                If m.Quote = 0 And m.Price > 0 Then
                    asset(m.Base).ToBTC = m.Price
                End If
                If m.Base = 0 And m.Price > 0 Then
                    asset(m.Quote).ToBTC = 1 / m.Price
                End If
            Next
            BTCtot = 0
            For Each a As ASSET In asset
                BTCtot += (a.Balance + a.BalanceLending) * a.ToBTC
            Next
        End If
    End Sub
#End Region
#Region "CHARTS"
    Public Shared Sub OHLC() 'Aggiorna grafici
        For Each m As MARKET In market
            Try
                Dim trade_stream As CryptoExchange.Net.Objects.WebCallResult(Of IEnumerable(Of Interfaces.IBinanceKline))
                If m.Last = Nothing Then
                    trade_stream = api.client.Spot.Market.GetKlines(m.Name, Interval(m.Periodo))
                Else
                    trade_stream = api.client.Spot.Market.GetKlines(m.Name, Interval(m.Periodo), startTime:=m.Last)
                    m.Charts.RemoveAt(m.Charts.Count - 1)
                End If
                For x As Integer = 0 To trade_stream.Data.Count - 1
                    m.Last = trade_stream.Data(x).OpenTime
                    m.Price = trade_stream.Data(x).Close
                    m.PriceMax = trade_stream.Data(x).High
                    m.PriceMin = trade_stream.Data(x).Low
                    m.Volume = trade_stream.Data(x).BaseVolume

                    Dim qu As New Quote
                    qu.Date = trade_stream.Data(x).OpenTime
                    qu.Open = trade_stream.Data(x).Open
                    qu.Close = trade_stream.Data(x).Close
                    qu.High = trade_stream.Data(x).High
                    qu.Low = trade_stream.Data(x).Low
                    qu.Volume = trade_stream.Data(x).BaseVolume
                    m.Charts.Add(qu)
                Next
            Catch ex As Exception
                Log("ERROR CHART " & m.Name & " : " & ex.Message)
                m.Last = Nothing
                m.Charts.Clear()
            End Try
        Next
    End Sub
    Private Shared Function Interval(ByRef i As String) As Enums.KlineInterval
        Select Case i
            Case "1W"
                Return Enums.KlineInterval.OneWeek
            Case "3D"
                Return Enums.KlineInterval.ThreeDay
            Case "1D"
                Return Enums.KlineInterval.OneDay
            Case "12h"
                Return Enums.KlineInterval.TwelveHour
            Case "8h"
                Return Enums.KlineInterval.EightHour
            Case "6h"
                Return Enums.KlineInterval.SixHour
            Case "4h"
                Return Enums.KlineInterval.FourHour
            Case "2h"
                Return Enums.KlineInterval.TwoHour
            Case "1h"
                Return Enums.KlineInterval.OneHour
            Case "30m"
                Return Enums.KlineInterval.ThirtyMinutes
            Case "15m"
                Return Enums.KlineInterval.FifteenMinutes
            Case "5m"
                Return Enums.KlineInterval.FiveMinutes
            Case "3m"
                Return Enums.KlineInterval.ThreeMinutes
            Case "1m"
                Return Enums.KlineInterval.OneMinute
            Case Else
                Return Enums.KlineInterval.ThreeDay
        End Select
    End Function
#End Region
#Region "Trade"
    Public Shared Sub CancelOrders(Optional Symbol As String = Nothing)
        Try
            If Symbol = Nothing Then
                For x = 0 To market.Count - 1
                    api.client.Spot.Order.CancelAllOpenOrders(market(x).Name)
                Next
            Else
                api.client.Spot.Order.CancelAllOpenOrders(Symbol)
            End If
        Catch ex As Exception
            Log("ERROR Cancel Order : " & ex.Message)
        End Try
    End Sub
    Public Shared Sub MSell(ByVal m As MARKET, Optional ByVal Div As Integer = 10000) 'base >to> quote
        For x = 0 To 10000
            Dim Volume As Decimal = Mdecimal((BTCtot / asset(m.Base).ToBTC) / (Div + x), m.MinVolume)
            If Volume > asset(m.Base).BalanceFree And Volume < asset(m.Base).BalanceLending + asset(m.Base).BalanceFree And asset(m.Base).BalanceLending > asset(m.Base).BalanceIdeal Then
                LeftQ(asset(m.Base).Name, Math.Round(asset(m.Base).BalanceLending - asset(m.Base).BalanceIdeal, 7))
                Exit For
            ElseIf Volume > m.MinNotional / m.Price And asset(m.Quote).BalanceFree + (Volume * m.Price) + asset(m.Quote).BalanceLending <= asset(m.Quote).BalanceIdeal And asset(m.Base).BalanceFree + asset(m.Base).BalanceLending - Volume >= asset(m.Base).BalanceIdeal And asset(m.Base).BalanceFree > Volume Then
                Try
                    Dim ordine = api.client.Spot.Order.PlaceOrder(m.Name, Enums.OrderSide.Sell, Enums.OrderType.Market, Mdecimal(Volume, m.MinVolume))
                    Log("SELL : " & ordine.Data.Symbol & " Volume : " & ordine.Data.Quantity & " Price : " & m.Price & " ID : " & ordine.Data.OrderId)
                    GetBalance()
                    Exit For
                Catch ex As Exception
                    Log("ERROR SELL : " & m.Name & "  |  Volume : " & Volume & " /!\ " & ex.Message)
                    Exit For
                End Try
            End If
        Next
    End Sub
    Public Shared Sub MBuy(ByVal m As MARKET, Optional ByVal Div As Integer = 10000) 'quote >to> base    |  Base/quote
        For x = 0 To 10000
            Dim Volume As Decimal = Mdecimal((BTCtot / asset(m.Base).ToBTC) / (Div + x), m.MinVolume)
            If Volume * m.Price > asset(m.Quote).BalanceFree And Volume * m.Price < asset(m.Quote).BalanceLending + asset(m.Quote).BalanceFree And asset(m.Quote).BalanceLending > asset(m.Quote).BalanceIdeal Then
                LeftQ(asset(m.Quote).Name, Math.Round(asset(m.Quote).BalanceLending - asset(m.Quote).BalanceIdeal, 7))
                Exit For
            ElseIf Volume > m.MinNotional / m.Price And asset(m.Base).BalanceFree + Volume + asset(m.Base).BalanceLending <= asset(m.Base).BalanceIdeal And asset(m.Quote).BalanceFree + asset(m.Quote).BalanceLending - (Volume * m.Price) >= asset(m.Quote).BalanceIdeal And asset(m.Quote).BalanceFree > (Volume * m.Price) Then
                Try
                    Dim ordine = api.client.Spot.Order.PlaceOrder(m.Name, Enums.OrderSide.Buy, Enums.OrderType.Market, Mdecimal(Volume, m.MinVolume))
                    Log("BUY : " & ordine.Data.Symbol & " Volume : " & ordine.Data.Quantity & " Price : " & m.Price & " ID : " & ordine.Data.OrderId)
                    GetBalance()
                    If m.Quote = 0 Then
                        api.client.Spot.Order.PlaceOrder(m.Name, Enums.OrderSide.Sell, Enums.OrderType.Limit, Mdecimal(Volume * 0.8, m.MinVolume), price:=Mdecimal(m.Price * 1.1, m.MinPrice))
                    End If
                    Exit For
                Catch ex As Exception
                    Log("ERROR BUY : " & m.Name & "  |  Volume : " & Volume & " /!\ " & ex.Message)
                    Exit For
                End Try
            End If
        Next
    End Sub
    Private Shared Sub LeftQ(ByVal n As String, ByVal volume As Decimal)
        Try
            Dim info = api.client.Lending.GetFlexibleProductPosition(n)
            If info.Error Is Nothing Then
                For Each D In info.Data
                    If D.Asset = n And D.FreeAmount > volume Then
                        Dim left = api.client.Lending.RedeemFlexibleProduct(D.ProductId, volume, Enums.RedeemType.Fast)
                        Log("REDEEM : " & D.ProductId & "  Volume : " & volume)
                    Else
                        Dim left = api.client.Lending.RedeemFlexibleProduct(D.ProductId, D.FreeAmount, Enums.RedeemType.Fast)
                        Log("REDEEM : " & D.ProductId & "  Volume : " & D.FreeAmount)
                    End If
                Next
            End If
        Catch ex As Exception
            Log("ERROR REDEEM : " & n & "  /!\ " & ex.Message)
        End Try
    End Sub
    Private Shared Function Mdecimal(ByVal vol As Decimal, ByVal v As Decimal) As Decimal
        Dim dec As Integer = 0
        Dim i As Decimal = 0.5
        For x As Integer = 0 To 10
            If Not v < 1 Then
                Exit For
            Else
                v = v * 10
                dec += 1
                i = i / 10
            End If
        Next
        Return Math.Round(vol - i, dec)
    End Function
#End Region
#Region "LOG"
    Public Shared Sub Console_App()
        Console.Clear()
        Console.WriteLine("{0,-13} {1,-20} {2,-20} {3,-20} {4,-20} {5,-20} {6,-1}", "|", "MARCKET", "Price", "High", "Low", "SMA", "|")
        For Each m As MARKET In market
            Console.WriteLine("{0,-13} {1,-20} {2,-20} {3,-20} {4,-20} {5,-20} {6,-1}", "|", m.Name, m.Price, m.PriceMax, m.PriceMin, m.Sma, "|")
        Next
        Console.WriteLine("========================================================================================================================")
        Console.WriteLine("{0,-13} {1,-20} {2,-20} {3,-20} {4,-20} {5,-20} {6,-1}", "|", "ASSET", "Balance", "Free", "Saving", "Ideal", "|")
        For Each a As ASSET In asset
            Console.WriteLine("{0,-13} {1,-20} {2,-20} {3,-20} {4,-20} {5,-20} {6,-1}", "|", a.Name, a.Balance, a.BalanceFree, a.BalanceLending, a.BalanceIdeal, "|")
        Next
        Console.WriteLine("========================================================================================================================")
        For Each a As ASSET In asset
            Console.WriteLine("Total Balance : " & Math.Round(BTCtot, 8) & " " & a.Name)
            Exit For
        Next
        Console.WriteLine("Active Trade  : " & AppSetting.ActiveTrade)
        Console.WriteLine("")
        For n = 0 To LOG_text.Count - 1
            Console.WriteLine(LOG_text(n))
        Next
    End Sub
    Public Shared LOG_text As New List(Of String)
    Public Shared Sub Log(ByRef text As String)
        LOG_text.Add(DateTime.UtcNow.ToUniversalTime & " >  " & text)
        If LOG_text.Count > 25 Then
            LOG_text.RemoveAt(0)
        End If
    End Sub
#End Region
End Class