'https://github.com/Dellai-V/BinanceRobotCode
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
