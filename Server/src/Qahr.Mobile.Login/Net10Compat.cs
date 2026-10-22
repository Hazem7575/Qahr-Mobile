// [net10] Compiled ONLY by Qahr.Mobile.Login.Net10.csproj (AccServer.csproj lists its files explicitly and does not include this one).
// MySql.Data (.NET Framework only) is replaced by MySqlConnector on .NET 10.
//
// NOTE: deliberately NO `global using MySqlConnector;` here. AccServer has its own Qahr.Mobile.Login.Database.MySqlCommand /
// MySqlReader wrappers, and Program.cs / AccountTable.cs / Server.cs import `using Qahr.Mobile.Login.Database;` at file level,
// so a global import of MySqlConnector makes `MySqlCommand` ambiguous (CS0104). The three files that touch the driver
// (DataHolder.cs, MySql\MySqlCommand.cs, MySql\MySqlReader.cs) pick the driver with `#if NET` instead.
// Empty placeholders so any leftover `using MySql.Data.MySqlClient;` / `using MySql.Data.Types;` still compiles.
namespace MySql.Data.MySqlClient { }
namespace MySql.Data.Types { }
