### Version 6.0.3.4
- [x] Exchange Info: `cancelReplaceAllowed`
- [x] Exchange Info: `allowTrailingStop`
- [x] Exchange Info: `EXCHANGE_MAX_NUM_ICEBERG_ORDERS`

### Version 6.0.3.3
- [x] If you call `ServerTimeClient.Start` it will automatically wait until the first timestamp is produced

### Version 6.0.3.1
- [x] Fixed a bug that was stopping the `ServerTimeClient` from starting
- [x] Simplified Creation of `ServerTimeClient`
- [x] You can specify a `CancellationToken` for the `BinanceClient` that is waiting for start.

### Version 6.0.3.0
- [x] Replace `TimeClient`
- [x] Improvements to `Time Sync`
- [x] You can now `Start` and `Stop` the `ServerTimeClient`
- [x] Add `WaitForStart()` method to `ServerTimeClient`
- [x] Add `CorrectionCount` to `ServerTimeClient`
- [x] Add `MissedPingCount` to `ServerTimeClient`
- [x] Add `GuesserAttemptCount` to `ServerTimeClient`
- [x] Add `GuessOffset` to `ServerTimeClient`
- [x] Add `RemoteTime` to `ServerTimeClient`
- [x] Add `PingTicks` to `ServerTimeClient`
- [x] You can trigger the `Guesser()` manually with `ServerTimeClient.Guesser()`
- [x] Update Test - Please have test output if you have issues

### Version 6.0.2.2
- [x] Small Improvement to `WebSocketClient`
- [x] Fix Repo Name

### Version 6.0.1.1
- [x] Add Package [`SemaphoreLight`](https://www.nuget.org/packages/SemaphoreLite.NET/)
- [x] Language Version `10` 

### Version 6.0.0
- [x] Refactor
- [x] Remove Nearly All Interfaces
- [x] Remove `IRequest`
- [x] Remove `IRequestFactory`
- [x] Remove `IRestClient`
- [x] Remove `IBinanceClient`
- [x] Refactored `BinanceClient`
- [x] Remove `ISocketClient`
- [x] Remove `IBinanceSocketClient`
- [x] Remove `BinanceSocketClient`
- [x] Remove `Exchange Name`
- [x] Move `WebSocketClient`
- [x] Enhance `SocketClient`
- [x] Add Bool: `IsClosing` to `WebSocketClient`
- [x] Expose Underlying Socket
- [x] Fix Encoding
- [x] Remove `Origin`
- [x] Remove `Websocket Headers`
- [x] Remove `Websocket Cookies`
- [x] Remove `WebsocketFactory`
- [x] Remove `Websocket`
- [x] Remove `IWebsocket`
- [x] Remove `IWebsocketFactory`
- [x] Enhance `WebSocketClient`
- [x] `Socket` is now the `WebSocketClient`
- [x] Fix `IWebsocket`
- [x] Expose `[UpdateSubscription].Connection`
- [x] Expose `[UpdateSubscription].Connection.Socket`
- [x] Expose `[UpdateSubscription].Connection.Socket.LastActionTime`
- [x] Expose `[UpdateSubscription].Connection.Socket.Reconnecting`
- [x] Expose `[UpdateSubscription].Connection.Socket.IsOpen`
- [x] Expose `[UpdateSubscription].Connection.Socket.IsClosed`
- [x] It is now easier to manage reconnection yourself from application code
- [x] Updated Test to Demonstrate checking the `LastActionTime` of a `UpdateSubscription`

### Version 4.0.7
- [x] Fix a bug in Reconnect Attempt

### Version 4.0.6
- [x] Fix a bug in Reconnect Attempt

### Version 4.0.5
- [x] Improvements to `Reconnecting`
- [x] You can now call `ReconnectAsync()` on your `Subscriptions`

### Version 4.0.4
- [x] Socket Changes - Improved Send/Recv Loop

### Version 4.0.3
- [x] Significantly Improved Time Sync
- [x] Provide Secure Strings directly
- [x] Fix a bug in Request Timestamps
- [x] Fix a bug in `Client Creation`
- [x] Fix a bug in `Default Client Options`
- [x] EventHandler
- [x] Average Ping based sync adjustments
- [x] Update [`ServerTimeClient`](<https://i.imgur.com/sNhE3UV.png>)
- [x] Add `TimeClient`
- [x] Add Client Option: `TimeLogPath`
- [x] Add Client Option: `Poll Rate`
- [x] ServerTimeClient: `AveragePing`
- [x] ServerTimeClient: `ServerTimeUpdated` Event - Occurs when Server Time is updated from the server.
- [x] ServerTimeClient: `CancelPoll` - Cancels Average Polling
- [x] ServerTimeClient: `WidePing` - True if your Min/Max ping is currently high.
