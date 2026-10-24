# QixiRomance2022 — كيشي 2022 العالمي (Romantic Season) — مرحلة COMPLETE (2026-10-15)

**المصدر الرسمي:** `luaui\dialog\dlgqixifestival` + `luagui\1267.dat` (جداول الجوايز) + `luaUiRes[1266..1270]` + `Server_Key` (`tTanabataFestivalNpc`، `tQixiFestivalCross2022_DelText`، `tQiXiMemoryPart`) + itemtype.

## اللي شغال دلوقتي
- **الزرار والشباك الرئيسي** `DlgQiXiFestivalMain` ببيانات اللاعب الحقيقية: النقط، أنهي Lyra مفتوحة، الهارب اتصلّح ولا لأ، النقط الحمرا.
- **تصليح الـ Ethereal Harp** (Repair / Maintain / Maintain all): كل مرة بتاخد قطعة من خامة الجزء (Phoenix Wood / Fairy Flower / Whale Baleen / Mermaid Tear)، وبتدّي جايزة من قايمة الكلاينت الرسمية (`1267-preview-award`) + Love Points. الأجزاء بتتفتح بالترتيب.
- **العزف** (بعد ما الهارب يتصلّح): First Love / Sunny Day / Sweet Hugs بـ 1/3/10 Holy Plectrum (رسمي)، وأول First Love في اليوم ببلاش (رسمي). الجوايز من الـ pools الرسمية (`1267-award-pool`) والشباك بينوّر على الجايزة اللي كسبتها.
- **جوايز Love Points** (زرار Rewards في الشباك أو Weaver Girl): 3000/5000/10000/20000/30000/50000 بالجوايز الرسمية، وبعد 50000 كل 1600 نقطة = L1 Mythsoul + Apollo`s Blessing.
- **الترتيب المحلي** (Local / Global): أعلى 10، ولازم 5000 نقطة على الأقل (رسمي).
- **شباك الذكريات** بعدد Memory Stone الحقيقي في الشنطة.
- **NPCs في توين سيتي:** Weaver Girl (8872001) و Orphee (8872002).
  - Orphee بياخد الورد/الهدايا ويدّي خامات الهارب بالجدول الرسمي (9 Tulips/Jades = 1، 20 Orchids/Beer أو Lilies/Love Letters أو Kisses/Red Roses = 1، الـ 99 = 5، الـ 999 = 50، 99 Tulips/Jades = 11)، والحد 150 قطعة في اليوم (رسمي).
- **الأيتمز:** Weaver Girl`s Silk (30 = Love Vow (B)، 50 = Love Vow (Eternal))، Apollo`s Blessing (90 = Golden Plume (Guardian)، 60 = جناح Love Sound)، وفتح الصناديق: Ethereal Harp Repair Pack، Harp Performance Pack، Orphee Memory Pack، Love Vow/Love Vow (Eternal)/Golden Plume (Guardian) (1% Blessed)، Love Sound (الجناح، مرة واحدة بس).

## رسمي ولا مشتق
- **رسمي:** كل الأيتمز والجوايز والكميات، تكلفة الأغاني، العزف المجاني اليومي، جدول الورد والحد اليومي، مراحل Love Points، حد الترتيب، النصوص.
- **مشتق (في `Data\QixiRomance2022\qixiromance2022.json`):**
  - نسبة كل تصليح 4% (ده الرقم الافتراضي اللي في كود الكلاينت نفسه) = 25 تصليح للجزء.
  - النقط: 100 لكل تصليح/صيانة/Plectrum.
  - احتمالات الجوايز (weights)، وأقصى عدد في Maintain all = 50.
  - الخامة اللي Orphee بيدّيها = خامة الجزء اللي بتصلّحه (وبعد التصليح الكامل خامة عشوائية).
  - L1 Mythsoul = L1 Mythsoul Pack 3325756.
  - مكان Weaver Girl و Orphee في توين سيتي (374,440) و (377,447): الكلاينت مفيهوش مكانهم، اخترنا خلايا ماشية جنب منادين المهرجانات. Orphee شكله Musician (npc.ini 10324) لأن الكلاينت مفيهوش شكل لـ Orphee. Weaver Girl شكلها الرسمي NpcX 110081.

## مش موجود وليه
- **نجوم Lyra A-D و Star Beasts و Star Energy:** كروس سيرفر، والخرايط 11840-11843 ملهاش doc في GameMap.dat (الكلاينت معندوش ملف الخريطة).
- **دخول الذكريات (Memory instances):** الخرايط 11845-11852 برضه ملهاش doc. الشباك بيفتح، والدخول برسالة.
- **تبديل Animae بـ Holy Plectrums:** مش واضح أنهي item هو "P9/P10/P11 Anima" في itemtype، فمعملناهوش.
- **تحويل Love Points لنقط الورد:** محتاج سيستم ترتيب الورد.
- **لقب 50000 نقطة** [The Little Prince/The Rose]: مالوش id في title_type.txt.
- **جوايز الترتيب في آخر الحدث (بالبريد):** مش متعملة.

## الكود
- `Base\QixiRomance2022\QixiRomance2022Ui.cs` (الحدث)، `QixiKit.cs` (أدوات مشتركة لمجموعة Qixi)، `QixiHooks.cs` (الهوكين).
- هوك LuaPush (msg 10044) موجود من امبارح.
- هوكين جداد (سطر واحد لكل واحد):
  - `Game\MsgNpc\Procesor.cs` → `QixiKit.Hooks.OnNpc`.
  - `MY System\MsgItem\MsgItemUsuagePacket.cs` → `QixiKit.Hooks.UseItem`.
- حالة اللاعبين: `Data\QixiRomance2022\players.txt` (موسم لكل سنة).

## IDs محجوزة لمجموعة Qixi
- NPCs: `8872000-8872199` (2022 = 8872001-8872009).
- Monsters: `997000-997199`.
