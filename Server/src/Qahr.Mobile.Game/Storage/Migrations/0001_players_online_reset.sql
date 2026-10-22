-- 0001_players_online_reset
-- First migration. The players.Online flag is reconciled live by the server every ~15s, but make sure
-- every existing row starts from a clean 0 the moment this feature ships (no stale "online" after an
-- old crash). Safe to run once; recorded in schema_migrations so it never runs again.
UPDATE `players` SET `Online` = 0 WHERE `Online` <> 0;
