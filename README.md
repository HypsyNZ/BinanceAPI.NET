# Whats Supported?

Supports all Basic Features of the [Binance API](https://binance-docs.github.io/apidocs/spot/en/#change-log), Including some that aren't in the list below

| Feature 	| Support | Websocket |
|---------------|---------|-----------|
| Spot 		| Full 	  | Yes	|
| Margin	| Full 	  | Yes |
| Isolated 	| Full 	  | Yes |
| Account	| Full 	  | Yes |
| Symbols	| Full 	  | Yes |
| Order Book    | Full    | Yes |
| Trades        | Full    | Yes |
| User Data Streams | Full | Yes |
| Lending 	| Full 	  | - 	|
| Fiat 		| History | - 	|

Some features aren't supported, examples include

| Feature 	| Support |
|---------------|-------|
| SubAccounts 	| No 	| 
| Futures	| No 	| 
| Swaps 	| No 	|
| Options	| No 	|
| Mining        | No    |
| Brokerage     | No    |
| NFT Related | No |


# New Options

Several options have been removed because they are no longer required, Below are the new `Options` you can configure.

#### `BinanceClientOptions`

| Field | Interval | Usage |
|-------|----------|-------|
|AutoTimestampTime | 125ms | The interval at which to sync the time automatically|
|AutoTimestampStartTime | 2000ms | The delay before the internal timer starts and begins synchronizing the server time|
|ServerTimeSyncType | x | Set to Aggressive by Default (125ms)|
|LogPath | x | Where to store the Log file for all BinanceClients
|LogLevel| x | Log Level for the Logger for all BinanceClients

#### `BinanceSocketClientOptions`

| Field | Type| Usage |
|-------|----------|-------|
|LogPath | string | Where to store the Log file for all SocketClients
|LogLevel| LogLevel | Log Level for the Logger for all SocketClients

#### `BinanceSocketClient Options`

| Field | Type | Usage |
|-------|----------|-------|
|LogPath | string | Where to store the Log file for all SocketClients
|LogLevel| LogLevel | Log Level for the Logger for all SocketClients

#### `OrderBookOptions`
| Field | Type | Usage |
|-------|----------|-------|
|LogPath | string | Where to store the Log file for all OrderBooks
|LogLevel| LogLevel | Log Level for the Logger for all OrderBooks


# Server Time Client

The `ServerTimeClient` is used to update the server time and the timestamp offset to be used for all requests and is created when you create your first `BinanceClient`. You can also use the `ServerTimeClient` to access the current `ServerTime` for your own purposes.

| Field | Type | Usage |
|-------|----------|-------|
|`ServerTimeClient.ServerTime`|DateTime | The current Server Time|
|`ServerTimeClient.UsedWeight`| int | The used request weight this minute|
|`ServerTimeClient.CalculatedTimeOffset` |double | The current calculated offset |
|`ServerTimeClient.Exists` | bool |True if the `ServerTimeClient` should be running |

You must create at least one `BinanceClient` so the `ServerTimeClient` can start.

# Logging

Loggers are automatically created the first time you create a `Client` only if the `LogPath` option is `Set`

All messages now use `nullable` so if you don't set a `LogPath` in your `Options` all logging in the library will be skipped, eg.

```cs
ClientLog?.Info("message");
SocketLog?.Warning("message");
```

All messages are also logged to the `Console` but again; only if the `LogPath` was set.

What this means is in `Release Mode` you should avoid setting `LogPath` unless you want users to receive logs from the library
