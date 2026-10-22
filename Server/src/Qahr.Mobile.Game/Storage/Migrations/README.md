# Database migrations (Laravel-style)

This folder is the **single place** for the MySQL schema. You never hand-build tables again.

## What happens on boot (MySql / Dual backend only)
1. **CREATE DATABASE** — if the database in the connection string is missing, the server creates it
   (as long as the MySQL connection itself works). See `Migrator.EnsureDatabase()`.
2. **CREATE TABLES** — the core tables (`players`, binary blobs, social) are still created by the
   existing `*Storage.EnsureSchema()` (driven by `players_columns.csv`). This runs before step 3.
3. **MIGRATIONS** — every `*.sql` file here runs **once**, in filename order, and is recorded in the
   `schema_migrations` table. Already-applied files are skipped. See `Migrator.Run()`.

So if the whole database is lost: one boot rebuilds the database, the tables, and re-applies every
migration. Nothing manual.

## Adding a change later
Drop a new file named `00NN_short_description.sql` (keep the numbers increasing, zero-padded). Example:

```sql
-- 0002_add_last_login
ALTER TABLE `players` ADD COLUMN `LastLogin` DATETIME NULL;
CREATE INDEX `idx_lastlogin` ON `players` (`LastLogin`);
```

On the next server start it runs automatically and is remembered.

## Rules
- Files run in **filename order** (`0001_...`, `0002_...`, ...). Do not renumber an applied file.
- Prefer idempotent DDL (`CREATE TABLE IF NOT EXISTS`, `ADD COLUMN` on a column you know is new).
  A migration that fails is **not** recorded, so it retries on the next boot — fix the SQL and restart.
- One logical change per file. Separate statements with `;`. `--` and `#` lines are comments.
- MySQL auto-commits DDL, so a half-failing multi-statement DDL migration cannot roll back — split risky
  DDL into separate, individually-safe files.

## Shipping to another machine
The server finds this folder next to the exe or by searching up the source tree (same as
`players_columns.csv`). On a deployed server, copy this `Migrations` folder next to the exe
(e.g. `bin-net10\Migrations\`) so the `.sql` files travel with it.
