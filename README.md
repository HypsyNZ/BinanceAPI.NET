# BinanceAPI.NET [![](https://buildstats.info/nuget/BinanceAPI.NET)](https://www.nuget.org/packages/BinanceAPI.NET)

To get started with examples or find out about new features or changes, read the [Wiki](<https://github.com/HypsyNZ/BinanceAPI.NET/wiki>)

[![](https://user-images.githubusercontent.com/54571583/173321360-737e4e55-0e46-40aa-ac4e-0ac01875ce96.png)](https://github.com/HypsyNZ/BinanceAPI.NET/wiki)


# What Endpoints are Supported?

Supports all Basic Endpoints of the [Binance API](https://binance-docs.github.io/apidocs/spot/en/#change-log), Including some that aren't in the list below

| Feature 	| Support | Websocket |
|---------------|---------|-----------|
| Spot 		| Full 	  | Yes	|
| Margin	| Full 	  | Yes |
| Isolated 	| Full 	  | Yes |
| Account	| Full 	  | Yes |
| Symbols	| Full 	  | Yes |
| Order Book    | Full    | [Yes](<https://binance-docs.github.io/apidocs/spot/en/#how-to-manage-a-local-order-book-correctly>) |
| Trades        | Full    | Yes |
| Order Updates | Full    | [Yes](<https://binance-docs.github.io/apidocs/spot/en/#payload-order-update>) |
| Tickers       | Full    | [Yes](<https://binance-docs.github.io/apidocs/spot/en/#websocket-market-streams>) |
| User Data Streams | Full | [Yes](<https://binance-docs.github.io/apidocs/spot/en/#user-data-streams>) |
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