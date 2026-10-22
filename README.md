# Qahr Mobile - Server

Game and login server for Qahr Mobile (Conquer Online Mobile client, 7957-based).
This repository is a full, as-is copy of the live `D:\Qahr Mobile\Server` folder plus a database dump.

**This repository contains real passwords, config secrets, player files and logs. It must stay private.**

## Layout

| Path | Contents |
|------|----------|
| `Server/Qahr.Mobile.sln` | Visual Studio solution (.NET 10) |
| `Server/src/Qahr.Mobile.Game` | Game server source (including `bin/` with the built server, logs, PacketLogs, Exceptions, server logs) |
| `Server/src/Qahr.Mobile.Login` | Login (account) server source (including `bin/`) |
| `Server/src/Qahr.Mobile.Shared` | Shared code and the real `ServerConfig.ini` (central config read by both servers) |
| `Server/Data` | Game data and live state: items, monsters, maps, NPCs, shops, quests, events, Users/Players files, storages, houses, logs, `adminapi.txt`, `Agreed.txt` |
| `Server/scripts` | Start scripts (`run_live.ps1`, `run_cdn.ps1`) |
| `Server/libs` | Third-party libraries (MySql.Data) |
| `Server/Tools` | Server-side helper tools |
| `database/qahr_mobile.sql.gz` | MariaDB 10.6 dump of the `qahr_mobile` database (routines and triggers included) |

Only `obj/` and `.vs/` folders (build cache) are left out.

## Configuration

`Server/src/Qahr.Mobile.Shared/ServerConfig.ini` is the live config with its real values.
Check `[Server] IP`, `DataFolder` and `CO2Folder` when running on another machine.

## Restoring the database

The dump creates the `qahr_mobile` database itself.

```sh
gunzip -k database/qahr_mobile.sql.gz
mysql -u <user> -p < database/qahr_mobile.sql
```
