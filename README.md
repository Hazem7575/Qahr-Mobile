# Qahr Mobile - Server

Game and login server for Qahr Mobile (Conquer Online Mobile client, 7957-based).

## Layout

| Path | Contents |
|------|----------|
| `Server/Qahr.Mobile.sln` | Visual Studio solution (.NET 10) |
| `Server/src/Qahr.Mobile.Game` | Game server |
| `Server/src/Qahr.Mobile.Login` | Login (account) server |
| `Server/src/Qahr.Mobile.Shared` | Shared code and `ServerConfig.ini` (central config read by both servers) |
| `Server/Data` | Game data (items, monsters, maps, NPCs, shops, quests, events) |
| `Server/scripts` | Start scripts (`run_live.ps1`, `run_cdn.ps1`) |
| `Server/libs` | Third-party libraries (MySql.Data) |
| `database` | MySQL/MariaDB dump of the `qahr_mobile` database |

Player save files and log files from `Server/Data` are not included; player data lives in the database.

## Configuration

`Server/src/Qahr.Mobile.Shared/ServerConfig.ini` is committed with all secrets removed. Before running, fill in:

- `[Database] Password`
- `[Transfer] Key` and `Salt` (must be the same for game and login server)
- `[Mail] Password` (only if password recovery by e-mail is used)

Also check `[Server] IP`, `DataFolder` and `CO2Folder` for your machine.
The admin API needs `Server/Data/adminapi.txt` (copy `adminapi.sample.txt` and set a secret); it is not in the repo.

## Restoring the database

The dump was made from MariaDB 10.6 and creates the `qahr_mobile` database itself.

```sh
gunzip -k database/qahr_mobile.sql.gz
mysql -u <user> -p < database/qahr_mobile.sql
```

**The dump contains player accounts and character data. This repository must stay private.**
