### Version 3.1.7
- [x] Improve Serializer

### Verison 3.1.6
- [x] Changes to Default Serializer
- [x] Json Class, Made some things static 
- [x] Remove more debugging from release build
- [x] Update SimpleLog4.NET

### Version 3.1.5
- [x] Cleanup

### Version 3.1.4
- [x] Improvements to `ServerTimeClient`

### Version 3.1.3
- [x] BinanceClientOptions: Add `LogPath` and `LogLevel`
- [x] BinanceSocketClientOptions: Add `LogPath` and `LogLevel`
- [x] OrderBookOptions: Add `LogPath` and `LogLevel`
- [x] Logging is now fully Optional and must be enabled by setting `LogPath`
- [x] Logged messages will also print to the `Console` when `LogPath` is set
- [x] KlineInterval Enum now includes the time in milliseconds for each period

### Version 3.1.2
- [x] Fixed Spot MiniTicker subscription having swapped base/quote volume properties
- [x] Changed MobileNumber type from long to string, fixing GetSubAccountStatusAsync deserialization when no phone number is defined
- [x] Removed some rate limiter related stuff
- [x] Updated SetProxy to support Socks5
