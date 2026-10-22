# Castle Startle: أزمات قلاع Green وSmith وMiller (White Day 2025)

**الحالة:** `enabled: true`، الجدول سنوي من 03-04 لـ 03-24 (الرسمي: `STR_LUAUI_25BSQRJ_GBJQ_HDSJ` = "Mar. 4 - Mar. 24").
الأونر يقدر يشغّله بإيده: `@festival on CrisisSeries`.

## اللاعب بيلعبه إزاي
1. يدوس على زرار **Castle Startle** فوق (Fgui `150249`). يفتح الشباك الرسمي `Fgui_25Bsqrj_Main`، وفيه 3 قلاع.
2. يختار قلعة، فتفتح لوحة المغامرة (`Fgui_25Bsqrj_Jxmx`) عند المرحلة الحالية.
3. يدوس "Move forward 1 step"، فتتقبل مهمة boss المرحلة ويتنقل لنسخة خاصة بيه من خريطة الـ instance الرسمية. الـ boss بيبقى واقف عند نقطة Questinfo.
4. بعد ما يقتل الـ boss يفتح الشباك تاني (أو يدوس Finish في لوحة المهام)، فالمهمة تخلص ويرجع Twin City.
5. كل قلعة فيها 3 مراحل، وبعد التالتة تبقى متحررة (State 2, 100%). مينفعش يبدأ قلعة تانية غير لما يخلّص القلعة اللي شغال فيها (`STR_LUAUI_25BSQRJ_MAIN_RULE_3`).

| القلعة | المرحلة | المهمة | الـ boss | الخريطة الرسمية (doc) |
|---|---|---|---|---|
| Green | 1 | 8740 | WickedFrieze | 1003997 (12796) |
| Green | 2 | 8746 | BlazeDemon | 1004003 (12802) |
| Green | 3 | 8751 | Demonstar | 1003992 (12791) |
| Smith | 1 | 8757 | GiantRaven | 1003992 (12791) |
| Smith | 2 | 8762 | DarkDeity + NPC DemonHeart | 1003996 (12795) |
| Smith | 3 | 8767 | DemonMatthew | 1003997 (12796) |
| Miller | 1 | 8773 | BarbieBunny | 1003992 (12791) |
| Miller | 2 | 8778 | DemonCharlie | 1004001 (12800) |
| Miller | 3 | 8783 | SkyboneDrake | 1004000 (12799) |

## إيه الرسمي وإيه المشتق
- **رسمي:**
  - نص المهام وأماكن الـ boss من `Questinfo.ini`. كل النقط اتشيّكت إنها walkable على الـ DMap.
  - اسم ولفل وحياة كل boss من `monster.dat` بتاع الكلاينت (الأنواع 23007/23012/23018/23022/23026/23032/23038/23043/23048، كلهم Boss=1).
  - الـ lookface من `fgui_25bsqrj_jxmxdata` `BossId` (299، 976، 166 / 439، 191، 184 / 285، 291، 150). ترتيبهم هو نفس ترتيب الـ 9 bosses في `monster.dat`، وDemonstar=166 بيطابق "Devil - Demonstar".
  - NPC الـ DemonHeart: `npc.ini NpcType10059` (mesh 100590).
  - الزرار والشباك والـ callbacks من luaui. الـ Chicken Bun هو 3352187 (`luauirewardshow 136001000`)، والعداد اللي في الشباك هو العدد اللي في الشنطة.
- **مشتق (في `Infinity[DB]\CrisisSeries\crisis.json` وفي `monsters\*.ini`):**
  - هجوم ودفاع الـ bosses: attack 2500-3500، defence2 4000.
  - الأماكن اللي يبدأ منها التحدي (المدن بس، ومش من جوه نسخة).
  - مكان الدخول جوه النسخة (12 خطوة من الـ boss) والخروج لـ TC (439,478).
  - اللوحة بتفتح على الخانة اللي قبل الـ boss، لأن مفيش داتا رسمية لأحداث الخانات الـ 18 اللي قبله.
  - مهمة 8762: الـ kill في Questinfo اسمه "DemonHeart"، والـ boss الحقيقي هو Dark Deity (23026، ونص المهمة بيقول كده). الـ Demon Heart نفسه NPC.
- **الـ ids:** الأنواع 23007-23048 (نطاق المهرجانات)، والـ NPC هو 8878001. الكلاينت بيستقبل الـ type id ده، فممكن حجم الـ boss ميطلعش زي zoom 200 الرسمي، لأن صف `monster.dat` مربوط بالـ id الرسمي.
- **مفيش جوايز:** `Prize=` فاضي في Questinfo لكل الـ 45 مهمة.

## اللي مقفول ومش شغال وليه
- **34 مهمة** (`engine: false`) وحوشها مالهاش lookface رسمي في أي مصدر: Rogue، Watchman، Gatekeeper، SinisterDealer، DemonSoldier، DemonWarden، Thug، FlameSoldier، CastleSentinel، CaptainSentinel، Demon`sMinion، EvilSprite، MadRobot، SandMonster، SeaSerpent، ShadowWyrm، DarkPuppet، Kachu، MinKachu، SwampCreeper، RockBugSwarm، Ruffian، DemonRyan، DarkWraith، PhantomLizard، Blood-eyedWolf، CastleWarden، HemovoreBat، FleshWraith، ChaoticCreation، BoneKnight، ToxicBird، EnchantedArt. ومعاهم كمان خطوات القصة 8756 و8758. السبب مكتوب في `_why` في `quests.json`.
- **8742 و8761:** التسليم عند HattoriHanzo على 10820، والخريطة دي مش محلولة في `maps.txt`.
- **مش موجودين:** 8736 و8752 و8768 (أول كل سلسلة) مش موجودين في Questinfo.
- **بترد برسالة بس:** صناديق اللوحة، ووحوش اللوحة، وصفحة الجوايز، وتحدي Castle Spirit (DarkOverlord 23062 ماله lookface)، والهدية اليومية من الـ Chicken Buns (الكمية مش معروفة)، وسحب Spirit Shards.

_اتعمل بواسطة CrisisSeries agent (14-10-2026). اللي كان هنا قبل كده (Agent 5) محفوظ في `Backups_Systems\20261014_210535_CrisisSeries\`._
