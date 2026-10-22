# مجد الملعب (Cloudsurge) - كأس العالم 2026، الشوط التاني

**الحالة:** `enabled: true`، سنوي من 30-06 لـ 20-07. التاريخ رسمي: هيدر Server_Key `[6.30-7.20]`، و`STR_FUI_ACTTIME_10900`.

## اللي شغال
- **زرار فوق رسمي Fgui `150448`:** الضغط عليه بيفتح الشباك الرسمي `Fgui_26Foot_Main` (Stage=1)، وفيه:
  - المدة.
  - `RaceDay` من جدول البطولة الرسمي.
  - صفحة Highlight Events `ActInfo[10900]`.
- **جديد (2026-10-15) - Vault Palace Visit (زرار 7):**
  - بينقلك لـ Vault Palace (خريطة 12818 ← doc 12019) جنب تمثال Pitch Glory الرسمي.
  - المكان من minimap الكلاينت `[GlobalPitchGlory(Windrise)]` (310,242). الخلية دي هي التمثال نفسه، فالوصول على (304,243) (مستنتج).
  - النقل من توين سيتي والمدن بس، ولو اللاعب عايش.
- **باقي الأزرار** بترد بالسبب بالظبط بدل "مش متاح" (`Reason()` في الكود).
- **الكود:** `Base\WorldCup2026\WorldCup2026Ui.cs`، وهو نفسه بيخدم الشوط الأول.

## ليه الباقي مش ممكن
- **أصناف كأس العالم 2026 مش موجودة** في itemtype بتاع الكلاينت ولا السيرفر: Field Whistle، وPitch Point Box، وCheer Coins، وCasual Coins، والجوايز (3356639+، و3600869).
  - الكلاينت أقدم من الحدث.
  - عشان كده Points & Ranking وPitch Store وGolden Boot مش ممكنين، لأن مفيش أصناف نديها أو ناخدها.
- **Cuju Tournament وPenalty Kick-Off وCuju Training وPitch Flag Capture:** بيتلعبوا على ملعب 13319 (`2026worldcup_new`).
  - الملعب ده موجود في `client - orignal\ini\GameMap.dat` بس.
  - `client_test\ini\GameMap.dat` مفيهوش ولا doc لنفس الملف، فلازم باتش للكلاينت.
- **Pitch Adventure / Cuju Dream (Albert`s Dream 13310):**
  - الخريطة نفسها ممكنة: `2024thx_new` = doc 12633 في client_test.
  - لكن خطوات القصة وجوايزها مش في الكلاينت (5 نصوص بس في `tWorldCup2026Task_DelText`).
- **Lucky Cheer:** رهان على ماتشات كأس العالم الحقيقية، cross-server، وبعملات مش موجودة.

## اقتراح للـ main (مش في فولدري، فماعدلتهوش)
- صفوف `FestivalMaps\maps.txt` (doc رسمي ← نفس الملف في client_test):
  - `13310 12633 13310 map/map/2024thx_new.7z B`
  - `13325 12633 13325 map/map/2024thx_new.7z B`
- **13319:** محتاج إضافة `2026worldcup_new` لـ `GameMap.dat` بتاع client_test.

## الكونفيج
`Infinity[DB]\WorldCup2026\worldcup2026.json`:
- `minLevel` (80 رسمي).
- `secondHalfTime` (رسمي).
- `firstHalfTime` (**مستنتج**).

## اتجرّب
Harness بالـ dll الحقيقي:
- النقل لـ 12818 من الشوطين.
- مفيش نقل لو الشوط مقفول، أو لو انت جوه أصلاً.
- Pitch Store بيرد بالسبب بس.
