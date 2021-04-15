'https://github.com/Dellai-V/BinanceRobotCode
'ActiveTrade: If True, it will make real transactions.
'APIkey     : Your Binance API key.
'APIsecret  : Your Binance API secret.
'Asset      : Assets you want To trade On.
'Split      : Proportion you wish To have In your wallet Of the corresponding asset.
'Period     : Period with which the charts must work. (1W, 3D, 1D, 12h, 8h, 6h, 4h, 2h, 1h, 30m, 15m, 5m, 3m, 1m)
'TimeChart  : Charts update rate In seconds.
'TimeTrade  : Transaction update rate In seconds.
Public Class AppSetting

    Public Shared ActiveTrade As Boolean = False ' if true, automatic transactions will be active

    Public Shared APIkey As String = ""
    Public Shared APIsecret As String = ""

    Public Shared Asset() As String = {"BTC", "ETH"}
    Public Shared Split() As Integer = {100, 100}
    Public Shared Period As String = "1D"
    Public Shared TimeChart As Integer = 5 '5 sec
    Public Shared TimeTrade As Integer = 60 '1 min


    'Public Shared ActiveTrade As Boolean = False
    'Public Shared APIkey As String = ""
    'Public Shared APIsecret As String = ""
    'Public Shared Asset() As String = {"BTC", "ETH", "USDT", "BNB"}
    'Public Shared Split() As Integer = {100, 100, 0, 10}
    'Public Shared Period As String = "3D"
    'Public Shared TimeChart As Integer = 20
    'Public Shared TimeTrade As Integer = 180
End Class
