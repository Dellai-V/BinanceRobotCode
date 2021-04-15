# Binance Robot Code
Is a trading robot for Binance, developed in dot Net compatible with Windows, Linux and MacOS

# Installation
For build the program you need to use .NET 5.0 SDK : <a href="https://dotnet.microsoft.com/download">Download</a>

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

Edit the Script.vb file, to change how the bot works, you can change the way indicators work and use your own strategy.

To start the program:
<pre>
dotnet run
</pre>




