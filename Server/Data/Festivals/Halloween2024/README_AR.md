# Macabre Realm (Halloween2024): الزرار والشباك الرسمي بس

**الحالة:** `enabled: true`، جدول سنوي من 22 أكتوبر لحد 4 نوفمبر (من تعليق الكلاينت `ERP597236 全球万圣活动（10.22-11.4）`).
- **تنبيه:** الشباك الإنجليزي نفسه (`Cn_Res` `STR_LUAUI_HALLOWEEN24_TIME`) مكتوب فيه "Oct. 22 - Nov. 11". لو عايز تمشي على النص الإنجليزي، غيّر `end` لـ `11-11`.
- **تنبيه تاني:** فيه مهرجان `HalloweenSenluo2023` (هالوين 2023، زرار lua تاني) جدوله 10-24 لـ 11-06، يعني الاتنين هيشتغلوا مع بعض. الرسمي هالوين واحد في السنة، فشغّل واحد بس.

## اللي شغال
- **زرار Fgui 150175 "Macabre Realm":** بيظهر في الشريط اللي فوق طول ما المهرجان شغال (اتجرب لايف).
- **الشباك الرسمي `HalloweenMain` "Macabre Thrills"** (msg 10482، الكود في `Base\Halloween2024\Halloween2024Ui.cs`):
  - بيتفتح بـ `Fgui_CommandKey` `Cmd_HalloweenMain_Open` وبعدها `Cmd_HalloweenMain_UpdateData`.
  - Thrill Points بتبان 0، لأن مفيش حاجة على السيرفر بتدّيها.
  - زرار القواعد (؟) بيفتح صفحة القواعد الرسمية `RuleText["10482_1"]` (اتجربت لايف).
  - Witch House وGrimloft وSilent Hill وMidnight Duel وDevil Valley (CS) والرانكنج: كل واحد بيرد برسالة مؤدبة إنه مش مفتوح.
- **رسالة الليفل ورسالة انتهاء الحدث:** النص رسمي من `Server_Key` `tHalloweenNpc2024_DelText`.

## اللي مش موجود وليه
- كل محتوى الحدث (تربية الوحوش، Grimloft، Silent Hill، Midnight Duel، Devil Valley عبر السيرفرات) سكربتات سيرفر رسمية مش موجودة عندنا، ومفيش NPCs ولا خرايط ليه في `maps.txt`.
- **المهام 5924-5933 (Nightmare) مش متضافة:** مفيش أي دليل إنها تبع 2024.
  - هي `tHalloweenNightmare` (Jack O`Lanterns وOccult Realm)، وموجودة في أقسام قديمة من `Questinfo` (رقم [1438] وطالع).
  - شباك 2024 و`tHalloweenNpc2024` مابيشيروش ليها خالص.

## مشتق (derived)
- الرسايل المؤدبة بتاعة "مش مفتوح على السيرفر".
- `Task={0,0}` و`Reds={}`.
- الإعدادات: `Data\Halloween2024\halloween2024.json` (minLevel 80 رسمي).
