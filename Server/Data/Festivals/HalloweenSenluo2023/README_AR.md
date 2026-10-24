# هالوين 2023 - Macabre Realm (HalloweenSenluo2023)

**الحالة:** `enabled: true` بجدول `manual` (اتعمل كده في FestivalYears: الهالوين الأحدث بس هو اللي سنوي). التشغيل: `@festival on HalloweenSenluo2023`.
المواعيد الرسمية 10.24 لـ 11.06 (`luaUiRes_EN[1435].time`، و`actbtnconfig #13`، و ERP559756).

## بيتلعب إزاي دلوقتي (لفل 80+)
1. **الزرار الرسمي** `DlgSenluoHalloweenMainBtn` بيفتح شباك الدخول `DlgSenluoHalloweenMainUi` بداتا حقيقية: Macabre Flame وMacabre Coin من الشنطة، وMacabre Points، وDanger Level، والمنطقة المختارة النهارده ووقت الريست.
2. **تختار منطقة واحدة في اليوم** (الرسمي: "you can choose one to challenge every day"):
   - **Death Sideroad (منطقة 1):** نسخة خاصة من الخريطة 12270. اللافتة `Blood-stained Signpost` (32115) فيها 3 ممرات. كل ممر فيه 6 Servants، وبعدهم بيطلع الـ corroded relic (Xuanyuan / Pangu / Shennong) وبيدي صندوقه الرسمي. بعد كده بيظهر Mysterious Crack: لو هو الصح بتطلع ومعاك Macabre Flames، ولو غلط بترجع للتقاطع (النصوص كلها رسمية).
   - **Blood Altar (منطقة 2):** نسخة خاصة من 12273 PeripheralZone. المدخل والمذابح الخمسة والمخرج من الـ minimap الرسمي. بتفتّش الجثث، وممكن يطلعلك:
     - Secret Macabre Bag أو Supply Pack.
     - Black Succubus بيهجم عليك.
     - شبح (Jess / Courie / Elis / Dilys) بكلامه الرسمي، وبيسيبلك هدية.
     - Secret Key. بيه بتفتح الـ Mysterious Crack (32166) وتعدّي المنطقة.
3. **لما تعدّي المنطقة** بيتفتح شباك Macabre Realm (`DlgSenluoHalloweenGameUi`):
   - **4 قاعات** (West / North / South / East)، والـ Counts بتوعها Nicolas / Friss / Apada / Jupp. التحدي بـ Macabre Flame، وبيدخلك نسخة خاصة: موجة وحوش ← Elite Macabre Box ← الـ Count ← صندوقه الرسمي (Nicolas` Box …)، وفيه فرصة إنك تاخد الختم (seal).
   - **Mop-up:** للقاعة اللي عدّيتها قبل كده. بتاخد نفس الجايزة من غير قتال، ومن غير ما يزيد التقدّم (رسمي: msgtip4).
   - **تقدّم السيرفر كله:** أول عدّية في اليوم لكل لاعب بتزوّده. لما يوصل 100% ومعاك 5000 Macabre Points (رسمي) بتستلم جايزة القاعة مرة واحدة.
   - **أول واحد ياخد الختم:** اسمه وجيلده بيظهروا في تفاصيل القاعة، وبياخد Macabre Gold Box.
   - **Mighty Lord:** محتاج الـ 4 أختام وDestiny Card. فيه تحدي وmop-up. الـ Danger Level ماشي بالجدول الرسمي (L1 = 0-5 مرات ... L9 = 106+)، ولما يعلى بيشتغل الإفكت الرسمي.
   - **شراء Flame / Destiny Card بالـ CPs:** من زرار الـ +.
4. **Count Dracula** (8823001) في توين سيتي (400,380)، وكلامه رسمي:
   - Enter Macabre Realm (بيفتح الشباك).
   - Macabre Treasury: متجر بـ Macabre Coin، وهو `rewards.json`.
   - الرانكنج.
   - استلام صندوق الرانكنج (Local No.1 … No.7-10) من يوم 6 نوفمبر.
5. **الصناديق بتتفتح** (41 صندوق، `Data\HalloweenSenluo2023\boxes.json`). المحتوى من نص الأيتم الرسمي، زي:
   - Xuanyuan Box = 2 +3 Stone لمدة 5 أيام و20 Ginseng Fruit.
   - Iron / Silver / Gold Box = 320 / 1280 / 5120 عملة ونقطة.
6. **لو الشنطة مليانة:** الجايزة بتتحفظ وبتوصلك أول ما تفتح الشباك وفيه مكان.

## الرسمي والمشتق
**رسمي:**
- كل النصوص.
- أرقام الأيتمات ومحتوى الصناديق.
- خريطة 12273 بنقطها.
- الـ servants والوحوش: L120 بحياة 44000 / 64000 من `monster.dat`.
- جدول الـ Danger Level، وشرط الـ 5000 نقطة، ولفل 80.
- أشكال الـ NPCs (`npc.ini` / NpcX).

**مشتق (DERIVED):**
- **خريطة Death Sideroad = 12270.** هي نفس ملف المتاهة 2021qixi_new بتاع NightmareMaze (11886→11414)، وموجودة في بلوك هالوين 2023 جنب 12273. مفيش اسم رسمي ليها.
- **خرايط القاعات:** 12290 / 12286 / 12285 / 12292، وMighty Lord على 12283. دول من البلوك الرسمي 12275-12292 ("内围 挑战")، بس توزيعهم على القاعات مشتق. 12275 و12288 متسابين عشان السيرفر بيستخدمهم.
- **شكل كل الوحوش (lookface):** متاخد من وحوش السيرفر اللي في نفس الموضوع. Black Succubus والـ relics والـ Counts وMighty Lord ملهمش صف في `monster.dat`، فالستاتس بتاعتهم مشتقة من قالب اللفل.
- **مكان Count Dracula.**
- **كل الأرقام** في `Data\HalloweenSenluo2023\config.json` (ومكتوب جنب كل رقم إنه derived): أسعار الـ CPs، والـ flames بتاعة العدّية، ونسب الجثث والمفتاح، ونسبة الختم، وأيام فتح القاعات، و5% تقدّم، وأوزان الصناديق، وأسعار الخزنة.
- **جوايز القاعات على مستوى السيرفر:** Wedding / Rose / Wings / Gold Box.
- **صندوق Mighty Lord:** Iron لحد L3، وSilver لحد L6، وGold بعد كده.

## مش موجود وليه
- **القاعة جوّاها 5 مهام رسمياً** (Combat / Elite / Surprise / Puzzle / Boss). هنا القاعة موجة واحدة وبعدها الـ Count. الـ Surprise والـ Puzzle مالهمش أي داتا في الكلاينت.
- **زيارة Vault Palace اليومية (Vault Stela 32167):** مكانها مش معروف.
- **SpookyMerchant (Macabre Mall = CP Mall):** متجر الـ CPs بتاع الكلاينت.
- **جوايز الرانكنج:** الرسمي بيبعتها بالبريد بعد الحدث. هنا بتستلمها من Count Dracula.
- **حاجات مالهاش id عندنا وشلناها من الصناديق:**
  - Rare Yellow Rune Box (B) بتاع 2023.
  - Midnight Ordeal [Death] (base item).
  - تسريحات الشعر.
- **ميكس الـ fragments** (30/50 ← garment)، وWings Encore Box / Silent Wings / Demon Wings: نظام الأجنحة والتجميع مش متعمل.

## الأرقام المحجوزة
- **NPC:** من 8823001 لـ 8823399. فيهم Dracula 8823001، والـ cracks 8823010+، والجثث 8823100+، والمفتاح 8823200، وكراك الخروج 8823300. الـ IDs الرسمية 32115 / 32154-32157 / 32166 جوه النسخ الخاصة بس.
- **الوحوش:** من 14639 لـ 994716.

## الكود
- `Base\HalloweenSenluo2023\`: الملفات `HsKit.cs` و`HsData.cs` و`HsZones.cs` و`HsBoxes.cs` و`HalloweenSenluo2023Ui.cs`.
- **الهوكات:** `LuaPush` (10311، كان موجود)، و`Game\MsgNpc\Procesor.cs` (سطر واحد)، و`MY System\MsgItem\MsgItemUsuagePacket.cs` (سطر واحد).
- **الحالة:** `Data\HalloweenSenluo2023\players.json`.
