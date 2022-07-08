### Version 6.0.5.0
- [x] Add `UriClient` - A Static Client that manages Uris for Requests
- [x] Add Feature [`ChangeEndpoint`](https://github.com/HypsyNZ/BinanceAPI.NET/wiki/Base-Client-Settings#select-api-controller) to `BaseClient`
- [x] Remove `BaseAddress` from `ClientOptions
- [x] `WebSockets` will now use the other `Streams` based on selected [`Endpoint`](https://github.com/HypsyNZ/BinanceAPI.NET/wiki/Base-Client-Settings#select-api-controller)
- [x] Improve [`Tests`](https://github.com/HypsyNZ/BinanceAPI.NET/tree/main/API-Test/Tests)
- [x] Update [`Examples`](https://github.com/HypsyNZ/BinanceAPI.NET/wiki)
- [x] Remove some legacy code

### Version 6.0.4.9
- [x] Close `WebSocket` properly

### Version 6.0.4.8
- [x] `WebSocket` Speed Improvements
- [x] Remove `SemaphoreLite`
- [x] Remove `IncomingKbps`

### Version 6.0.4.5
- [x] Fix a small inconsistency in `Order Update`

### Version 6.0.4.4
- [x] Socket Changes - Please [see the example](<https://github.com/HypsyNZ/BinanceAPI.NET/wiki/Simple-Socket-Example>) for more information
- [x] This update makes ALL `Subscriptions` connect automatically again.
- [x] You can subscribe to the `StatusChanged` event before they connect automatically.
- [x] Add `Lost` to `Connection Status`
- [x] Add `Closed` to `Connection Status`
- [x] Remove `ConnectionLost` event
- [x] Remove `ConnectionClosed` event

### Version 6.0.4.3
- [x] ~`UpdateSubscriptions` are now manually connected using [`sub.ConnectAsync()`](<https://github.com/HypsyNZ/BinanceAPI.NET/blob/c02c8c712abbca2e528daf316e65ca0b95067b90/API-Test/API-Test.cs#L171>)~

### Version 6.0.4.2
- [x] This release has multiple breaking changes that you should be able to fix in about 10 seconds.
- [x] Improve `Connected` indicator for `UserSubscriptions`
- [x] Rename `Reconnecting` to `Connecting`
- [x] Improve `Connecting` indicator for `UserSubscriptions`
- [x] Improve `Disconnected` indicator for `UserSubscriptions`
- [x] Rename `BinanceClient` to `BinanceClientHost`
- [x] Rename `BinanceSocketClient` to `SocketClientHost`
- [x] Rename `Options` to their respective new names
- [x] Rename `Sockets` for clarity
- [x] Expose Event `StatusChanged` for `UserSubscriptions`
- [x] Expose Event `StatusChanged` for `BaseSocketClient`
- [x] Merge some `Options`
- [x] Improve `Socket` connection flow
- [x] Add Enum `Connection Status`
- [x] Renames multiple things to clarify intended usage
- [x] Fixes misc bugs

### Version 6.0.4.0
- [x] Another refactor of `WebSocket` for some small gains
- [x] Fix a `Task` that wasn't Awaited correctly
- [x] Another pass on legacy code

### Version 6.0.3.8
- [x] Some Cleanup

### Version 6.0.3.6
- [x] Fixes bug in User Subscriptions caused by an [error](https://github.com/dotnet/runtime/blob/7cbf0a7011813cb84c6c858ef19acb770daa777e/src/libraries/Common/src/System/Net/WebSockets/ManagedWebSocket.cs#L525) in the `.NET Runtime`

### Version 6.0.3.5
- [x] Added Symbol Filter: `NOTIONAL`

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
