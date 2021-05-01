'https://github.com/Dellai-V/BinanceRobotCode
'https://daveskender.github.io/Stock.Indicators/docs/GUIDE.html
Imports Skender.Stock.Indicators
Public Class Script
    Public Shared Sub S_priority()
        Dim Priority() As Integer
        ReDim Priority(Binance.asset.Count - 1)
        For Each m As MARKET In Binance.market
            Dim UO As IEnumerable(Of UltimateResult) = Indicator.GetUltimate(m.Charts, 7, 14, 28)
            If UO.Last.Ultimate IsNot Nothing Then
                If UO.Last.Ultimate < 20 Then
                    Priority(m.Base) += 1 'buy
                ElseIf uo.Last.Ultimate > 80 Then
                    Priority(m.Quote) += 1 'sell
                End If
            End If
            'AO
            Dim AO As IEnumerable(Of AwesomeResult) = Indicator.GetAwesome(m.Charts, 5, 34)
            If AO.Last.Oscillator IsNot Nothing Then
                If AO.Last.Oscillator > 0 And AO(AO.Count - 2).Oscillator < AO.Last.Oscillator Then
                    Priority(m.Base) += 1 'buy
                ElseIf AO.Last.Oscillator < 0 And AO(AO.Count - 2).Oscillator > AO.Last.Oscillator Then
                    Priority(m.Quote) += 1 'sell
                End If
            End If
            'ADX
            Dim ADX As IEnumerable(Of AdxResult) = Indicator.GetAdx(m.Charts, 14)
            If ADX.Last.Adx IsNot Nothing Then
                If ADX.Last.Adx > 40 Then
                    Priority(m.Base) += 1 'buy
                ElseIf ADX.Last.Adx < 20 Then
                    Priority(m.Quote) += 1 'sell
                End If
            End If
            'STOCHASTIC OSCILLATOR %K
            Dim STOCH As IEnumerable(Of StochResult) = Indicator.GetStoch(m.Charts, 14, 3, 3)
            If STOCH.Last.Oscillator IsNot Nothing Then
                If STOCH.Last.Oscillator < 20 Then
                    Priority(m.Base) += 1 'buy
                ElseIf STOCH.Last.Oscillator > 80 Then
                    Priority(m.Quote) += 1 'sell
                End If
            End If
            'HMA
            Dim HMA As IEnumerable(Of HmaResult) = Indicator.GetHma(m.Charts, 9)
            If HMA.Last.Hma IsNot Nothing Then
                If HMA.Last.Hma < m.Price Then
                    Priority(m.Base) += 1 'buy
                Else
                    Priority(m.Quote) += 1 'sell
                End If
            End If
            'VWMA
            Dim VWAP As IEnumerable(Of VwapResult) = Indicator.GetVwap(m.Charts)
            If VWAP.Last.Vwap IsNot Nothing Then
                If VWAP.Last.Vwap < m.Price Then
                    Priority(m.Base) += 1 'buy
                Else
                    Priority(m.Quote) += 1 'sell
                End If
            End If
            'ICHIMOKU
            Dim ICHIMOKU As IEnumerable(Of IchimokuResult) = Indicator.GetIchimoku(m.Charts, 9, 26, 52)
            If ICHIMOKU.Last.TenkanSen IsNot Nothing Then
                If ICHIMOKU.Last.TenkanSen > ICHIMOKU.Last.KijunSen Then
                    Priority(m.Base) += 1 'buy
                Else
                    Priority(m.Quote) += 1 'sell
                End If
            End If

            'CCI
            Dim CCI As IEnumerable(Of CciResult) = Indicator.GetCci(m.Charts, 20)
            If CCI.Last.Cci IsNot Nothing Then
                If CCI.Last.Cci < -200 Then
                    Priority(m.Base) += 1 'buy
                ElseIf CCI.Last.Cci > 200 Then
                    Priority(m.Quote) += 1 'sell
                End If
            End If

            'RSI
            Dim RSI As IEnumerable(Of RsiResult) = Indicator.GetRsi(m.Charts, 14)
            If RSI.Last.Rsi IsNot Nothing Then
                If RSI.Last.Rsi < 20 Then
                    Priority(m.Base) += 1
                ElseIf RSI.Last.Rsi > 80 Then
                    Priority(m.Quote) += 1
                End If
            End If

            'MACD
            Dim MACD As IEnumerable(Of MacdResult) = Indicator.GetMacd(m.Charts, 10, 26, 9)
            If MACD.Last.Histogram IsNot Nothing Then
                If MACD.Last.Histogram > 0 Then
                    Priority(m.Base) += 1
                Else
                    Priority(m.Quote) += 1
                End If
            End If

            'W%R
            Dim WilliamsR As IEnumerable(Of WilliamsResult) = Indicator.GetWilliamsR(m.Charts, 14)
            If WilliamsR.Last.WilliamsR IsNot Nothing Then
                If WilliamsR.Last.WilliamsR < -80 Then
                    Priority(m.Base) += 1
                ElseIf WilliamsR.Last.WilliamsR > -20 Then
                    Priority(m.Quote) += 1
                End If
            End If

            Dim ind As Integer() = {5, 10, 20, 30, 50, 100, 200}
            For x = 0 To ind.Length - 1
                'EMA5 EMA10 EMA30 EMA50 EMA100 EMA200
                Dim EMA As IEnumerable(Of EmaResult) = Indicator.GetEma(m.Charts, ind(x))
                If EMA.Last.Ema IsNot Nothing Then
                    If EMA.Last.Ema < m.Price Then
                        Priority(m.Base) += 1
                    Else
                        Priority(m.Quote) += 1
                    End If
                End If

                'SMA5 SMA10 SMA30 SMA50 SMA100 SMA200
                Dim SMA As IEnumerable(Of SmaResult) = Indicator.GetSma(m.Charts, ind(x))
                If SMA.Last.Sma IsNot Nothing Then
                    If SMA.Last.Sma < m.Price Then
                        Priority(m.Base) += 1
                    Else
                        Priority(m.Quote) += 1
                    End If
                End If
            Next
        Next
        Dim pmin As Integer = 0
        For n = 0 To Priority.Length - 1
            Priority(n) = Priority(n) * Binance.asset(n).Split
            If Priority(n) < Priority(pmin) Then
                pmin = n
            End If
        Next
        Priority(pmin) = 0
        For n = 0 To Priority.Length - 1
            Priority(n) = (Priority(n) ^ 2)
        Next
        Dim prioTot As Integer = 0
        For n As Integer = 0 To Binance.asset.Count - 1
            prioTot += Priority(n)
        Next
        For n As Integer = 0 To Binance.asset.Count - 1
            Dim a As ASSET = Binance.asset(n)
            If a.ToBTC > 0 And Priority(n) > 0 Then
                a.BalanceIdeal = Math.Round(((Binance.BTCtot / prioTot) * Priority(n)) / a.ToBTC, 8)
            Else
                a.BalanceIdeal = 0
            End If
        Next
    End Sub
    Public Shared Sub StartTrade()
        If Binance.api.stato = True Then
            For Each m As MARKET In Binance.market

                Dim SMA As Decimal = Indicator.GetSma(m.Charts, 10).Last.Sma
                m.Sma = SMA
                Dim base As ASSET = Binance.asset(m.Base)
                Dim quote As ASSET = Binance.asset(m.Quote)
                If quote.BalanceFree + quote.BalanceLending > quote.BalanceIdeal And base.BalanceFree + base.BalanceLending < base.BalanceIdeal Then
                    'BUY quote >to> base
                    If SMA * 0.99 > m.Price Then
                        If AppSetting.ActiveTrade Then
                            Binance.MBuy(m, 60)
                        Else
                            Binance.Log("TEST BUY : " & m.Name & "  Price : " & m.Price)
                        End If
                    ElseIf SMA * 1.05 > m.Price And quote.BalanceIdeal = 0 Then
                        If AppSetting.ActiveTrade Then
                            Binance.MBuy(m, 300)
                        Else
                            Binance.Log("TEST BUY : " & m.Name & "  Price : " & m.Price)
                        End If
                    End If
                End If
                If base.BalanceFree + base.BalanceLending > base.BalanceIdeal And quote.BalanceFree + quote.BalanceLending < quote.BalanceIdeal Then
                    'SELL base >to> quote
                    If SMA * 1.01 < m.Price Then
                        If AppSetting.ActiveTrade Then
                            Binance.MSell(m, 60)
                        Else
                            Binance.Log("TEST SELL : " & m.Name & "  Price : " & m.Price)
                        End If
                    ElseIf SMA * 0.95 < m.Price And base.BalanceIdeal = 0 Then
                        If AppSetting.ActiveTrade Then
                            Binance.MSell(m, 300)
                        Else
                            Binance.Log("TEST SELL : " & m.Name & "  Price : " & m.Price)
                        End If
                    End If

                End If
            Next
        End If
    End Sub
End Class