-- 0002_player_blobs
-- DB-only storage: the authoritative reload source for each player's binary stores
-- (items / mail / profs / spells / hairface / titles), stored byte-for-byte as the exact .bin
-- content so deposit chains and every struct field round-trip perfectly. The normalized
-- player_items / player_mail / ... tables remain as a read-only mirror for SQL queries.
-- `store` = the source folder name (PlayersItems, PlayersMail, PlayersProfs, PlayersSpells, HairfaceStorage, ...).
CREATE TABLE IF NOT EXISTS `player_blobs` (
  `uid` INT UNSIGNED NOT NULL,
  `store` VARCHAR(32) NOT NULL,
  `data` LONGBLOB NOT NULL,
  `updated_at` DATETIME NOT NULL,
  PRIMARY KEY (`uid`, `store`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
