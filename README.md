# Binance Robot Code
Is a trading robot for Binance, developed in dot Net.

# Installation
For build the program you need to use .NET 2.1 SDK : <a href="https://dotnet.microsoft.com/download">Download</a>

The following packages are required :
- <a href="https://daveskender.github.io/Stock.Indicators/">Skender.Stock.Indicators</a>
- <a href="https://github.com/JKorf/Binance.Net">Binance.Net</a>

<pre>
dotnet add package Skender.Stock.Indicators
dotnet add package Binance.Net
</pre>

# Setup
Edit the AppSetting.vb file, to change the settings:
- ActiveTrade : If true, it will make real transactions.
- APIkey      : Your Binance API key.
- APIsecret   : Your Binance API secret.
- Asset       : Assets you want to trade on.
- Split       : Proportion you wish to have in your wallet of the corresponding asset.
- Period      : Period with which the charts must work. (1W, 3D, 1D, 12h, 8h, 6h, 4h, 2h, 1h, 30m, 15m, 5m, 3m, 1m)
- TimeChart   : Charts update rate in seconds.
- TimeTrade   : Transaction update rate in seconds.
- OrdersReset : Cancel all active orders.

Edit the Script.vb file, to change how the bot works, you can change the way indicators work and use your own strategy.

To start the program:
<pre>
dotnet run
</pre>

Application example:
<pre>
|             MARCKET              Price                High                 Low                  SMA                  |
|             ETHBTC               0.06911100           0.07031400           0.06900000           0                    |
|             BTCUSDT              45146.99000000       46218.12000000       44833.40000000       0                    |
|             ETHUSDT              3120.39000000        3239.00000000        3093.14000000        0                    |
========================================================================================================================
|             ASSET                Balance              Free                 Saving               Ideal                |
|             BTC                  0.00000000           0.00000000           0.00000000           0                    |
|             ETH                  0.00000000           0.00000000           0.00000000           0                    |
|             USDT                 0.00000000           0.00000000           0.00000000           0                    |
========================================================================================================================
Total Balance : 0.00000000 BTC
Active Trade  : True

12/08/2021 06:06:06 >  Account Type : Spot | Can Trade : True | Maker Commission : 0.1% | Taker Commission : 0.1%
</pre>
