Imports Skender.Stock.Indicators
Public Class Script
    Public Shared Sub S_priority()
        Dim Priority() As Integer
        ReDim Priority(Binance.asset.Count - 1)
        For Each m As MARKET In Binance.market
            'CCI
            Dim CCI As IEnumerable(Of CciResult) = Indicator.GetCci(m.Charts, 20)
            If CCI.LastOrDefault().Cci IsNot Nothing Then
                If CCI.LastOrDefault().Cci > 200 Then
                    Priority(m.Base) += 1
                ElseIf CCI.LastOrDefault().Cci < -200 Then
                    Priority(m.Quote) += 1
                End If
            End If

            'RSI
            Dim RSI As IEnumerable(Of RsiResult) = Indicator.GetRsi(m.Charts, 14)
            If RSI.LastOrDefault().Rsi IsNot Nothing Then
                If RSI.LastOrDefault().Rsi > 80 Then
                    Priority(m.Base) += 1
                ElseIf RSI.LastOrDefault().Rsi < 20 Then
                    Priority(m.Quote) += 1
                End If
            End If

            'MACD
            Dim MACD As IEnumerable(Of MacdResult) = Indicator.GetMacd(m.Charts, 10, 26, 9)
            If MACD.LastOrDefault().Histogram IsNot Nothing Then
                If MACD.LastOrDefault().Histogram > 0 Then
                    Priority(m.Base) += 1
                Else
                    Priority(m.Quote) += 1
                End If
            End If

            'W%R
            Dim WilliamsR As IEnumerable(Of WilliamsResult) = Indicator.GetWilliamsR(m.Charts, 14)
            If WilliamsR.LastOrDefault().WilliamsR IsNot Nothing Then
                If WilliamsR.LastOrDefault().WilliamsR > -80 Then
                    Priority(m.Base) += 1
                ElseIf WilliamsR.LastOrDefault().WilliamsR < -20 Then
                    Priority(m.Quote) += 1
                End If
            End If

            Dim ind As Integer() = {5, 10, 20, 30, 50, 100, 200}
            For x = 0 To ind.Length - 1
                'EMA5 EMA10 EMA30 EMA50 EMA100 EMA200
                Dim EMA As IEnumerable(Of EmaResult) = Indicator.GetEma(m.Charts, ind(x))
                If EMA.LastOrDefault().Ema IsNot Nothing Then
                    If EMA.LastOrDefault().Ema < m.Price Then
                        Priority(m.Base) += 1
                    Else
                        Priority(m.Quote) += 1
                    End If
                End If

                'SMA5 SMA10 SMA30 SMA50 SMA100 SMA200
                Dim SMA As IEnumerable(Of SmaResult) = Indicator.GetSma(m.Charts, ind(x))
                If SMA.LastOrDefault().Sma IsNot Nothing Then
                    If SMA.LastOrDefault().Sma < m.Price Then
                        Priority(m.Base) += 1
                    Else
                        Priority(m.Quote) += 1
                    End If
                End If
            Next
        Next
        For n = 0 To Priority.Length - 1
            Priority(n) = Priority(n) * Binance.asset(n).Split
        Next
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
                Dim SMAlow As Decimal = (Indicator.GetSma(m.Charts, 10).LastOrDefault().Sma + m.PriceMin) \ 2
                Dim SMAhigh As Decimal = (Indicator.GetSma(m.Charts, 10).LastOrDefault().Sma + m.PriceMax) \ 2
                Dim base As ASSET = Binance.asset(m.Base)
                Dim quote As ASSET = Binance.asset(m.Quote)
                If quote.BalanceFree + quote.BalanceLending > quote.BalanceIdeal And base.BalanceFree + base.BalanceLending < base.BalanceIdeal Then
                    'BUY quote >to> base
                    If SMAlow > m.Price Then
                        If AppSetting.ActiveTrade Then
                            Binance.MBuy(m, 60)
                        Else
                            Binance.Log("TEST BUY : " & m.Name & "  Price : " & m.Price)
                        End If
                    ElseIf SMAhigh > m.Price And Quote.BalanceIdeal = 0 Then
                        If AppSetting.ActiveTrade Then
                            Binance.MBuy(m, 300)
                        Else
                            Binance.Log("TEST BUY : " & m.Name & "  Price : " & m.Price)
                        End If
                    End If
                End If
                If base.BalanceFree + base.BalanceLending > base.BalanceIdeal And quote.BalanceFree + quote.BalanceLending < quote.BalanceIdeal Then
                    'SELL base >to> quote
                    If SMAhigh < m.Price Then
                        If AppSetting.ActiveTrade Then
                            Binance.MSell(m, 60)
                        Else
                            Binance.Log("TEST SELL : " & m.Name & "  Price : " & m.Price)
                        End If
                    ElseIf SMAlow < m.Price And Base.BalanceIdeal = 0 Then
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