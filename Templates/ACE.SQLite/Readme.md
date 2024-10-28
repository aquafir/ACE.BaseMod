* This uses SQLite as the EntityFramework provider which has a few [limitations](https://learn.microsoft.com/en-us/ef/core/providers/sqlite/limitations)
* You'll need to specify runtimes (done to reduce size for redundant SQLite providers) with one of these:
  * `<RuntimeIdentifier>win-x64</RuntimeIdentifier>`
  * `dotnet publish -c Release -r win-x64 --self-contained`



* Change language folders with:
  * `	<SatelliteResourceLanguages>en</SatelliteResourceLanguages>`





