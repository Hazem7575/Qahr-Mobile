-- 0003_drop_normalized_binary
-- The binary stores (items / mail / profs / spells / hairface / titles) now live byte-exact in
-- player_blobs (migration 0002) and load from there. The old normalized mirror tables are no longer
-- created (EnsureSchema stopped making them) or maintained (saves write only the blob), so drop them:
-- smaller DB + faster saves (no per-save parse and 7 upserts). The blob holds all of this data.
DROP TABLE IF EXISTS `player_items`;
DROP TABLE IF EXISTS `player_item_perfection`;
DROP TABLE IF EXISTS `player_mail`;
DROP TABLE IF EXISTS `player_profs`;
DROP TABLE IF EXISTS `player_spells`;
DROP TABLE IF EXISTS `player_hairface_storage`;
DROP TABLE IF EXISTS `player_title_equips`;
