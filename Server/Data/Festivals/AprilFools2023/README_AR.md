# كذبة أبريل 2023 - Tricky Wit (متاهة الحيل) — AprilFools2023

**الحالة (2026-10-15، مرحلة COMPLETE):** المتاهة بقت بتتلعب كاملة.
- الجدول `manual` (قرار FestivalYears: النسخة الأحدث بس هي اللي بتشتغل سنوي). التاريخ الرسمي محفوظ في `_schedule_yearly` (03-28 → 04-10، `luaUiRes_EN["1286"].time`).
- **تشغيل يدوي:** `@festival on AprilFools2023`.
- ملفات الكلاينت الصيني هي متاهة هالوين 2022 (ERP516962)، والنص الإنجليزي هو كذبة أبريل 2023: Trick Maze وLyon وMad Clown.

## بيتلعب إزاي
1. **الزرار** `DlgHalloweenMazeMainBtn` ("Tricky Wit") → msg `10113` `HalloweenPage_MainView` → شباك Tricky Wit، والـ **Points حقيقية** (Trick Points).
2. **Lyon** (`8892001`، NpcX 105590) في Market تويين سيتي (1036, 203,193). عنده: Challenge Trick Maze / Dungeon Breaker / Tricker Rankings.
3. **المدخل** ("Floor N Entrance"، نص رسمي `tHalloweenPage_DelText`):
   - الدور الأول بيتفتح ببلاش. باقي الأدوار محتاجة **Maze Key** (3341223).
   - Explore، و**Reset** بـ 20 Maze Pearl، و**Mop it up** بـ 21 Maze Pearl (للدور اللي غلبت مهرجه قبل كده).
   - **لازم تدخل من تويين سيتي أو الـ Market** (رسالة `[105][2]` الرسمية)، ولفل 80.
4. **خريطة المتاهة الرسمية** `DlgHalloweenMazeMainUi`: فيها 3 layouts × 19 أوضة والبوابات 100-900، بالظبط زي `halloweenmazeconfig`. الأوضة بتتفتح لما تبقى جنب البوابة أو جنب أوضة عديتها.
5. **الأوض.** القتال بيحصل في **نسخة خاصة** من NightmareMaze 11414:

   | الأوضة | الجايزة (رسمي) |
   |---|---|
   | Room | 3-10 Trick Amulet Fragments |
   | Corridor | مفيش |
   | Magic Chess | 8-15 Trick Amulets، وزيادة 5/10/20 لو جمعت 3/4/5 Ritual Blessings |
   | Forbidden Area | 5-8 Trick Amulets، وClown Key أول مرة في كل دور، وOrdeal Light، وبيكشف الـ Secret Passage |
   | Mystery Room | Clown Box: واحدة من 5/5/10/10/20/20/30/30/50 Trick Amulets بـ Clown Key، وبعدها Crystal Device لما تكسره بيفك ختم أوضة المهرج |
   | Clown Room | 10-15 Trick Amulets، وقناع المهرج بتاع الدور، وأول مرة بتاخد soul fragment |

   - **Magic Chess:** فيها Tricky Dice (النرد والـ Perfect Dice)، والـ Soldier، والـ Gun Carrier، والـ Ritual Blessings. وفي الآخر بتختار مربع من 4 مربعات الخروج، وفيهم King وQueen وPatriarch وKnight.
6. **Mad Clown:** لما تجمع 9 fragments تقدر تتحداه. الجايزة 50 Trick Amulets، و10 Joker Knight Fragments، و6 Playful Player Fragments. مرة واحدة بس، وبيتعمل إعلان للسيرفر كله.
7. **Dungeon Breaker:** كليك يمين على Trick Amulet، أو "Dungeon Breaker" من Lyon أو من الـ sub menu. بيصرف Trick Amulets ويديك كنز عشوائي + Trick Points.
8. **جوايز النقط** (`FoolsDayPointsAndRankConfig["10113"]`، رسمي): 500 / 1500 / 2500 / 5000 / 10000 / 30000 / 50000 / 77777، وبعد كده كل 5000.
   - الاستلام على msg 10311 `HalloweenNpc2023_ClimbStageReward`.
   - بيشتغل بس لو الشباك اتفتح من Tricky Wit في آخر 10 دقايق، لأن الشباك ده مشترك مع Halloween 2023.
9. **كليك يمين على الأيتمات:**
   - Trick Amulet Fragment: كل 5 بيبقوا Trick Amulet.
   - Maze Pearl Pack بيدي Maze Pearl.
   - الأقنعة التسعة: 10-15 Trick Amulets + الجايزة الرسمية لكل قناع.
   - Joker Knight Fragment: 30 بيبقوا Joker Knight (B) 191475، و50 بيبقوا Joker Knight (Eerie) 191485.
   - Playful Player Fragment: 30 بيبقوا لقب Playful Player دايم.

## IDs محجوزة (اتعمل grep على كل الـ Festivals وNpcs.txt وMonsters)
- **NPCs:** `8892000-8892099` لمجموعة كذبة أبريل كلها.
  - AprilFools2023: `8892001` Lyon.
  - virtual dialogs: `8892090-8892099` (مش NPCs حقيقية).
- **وحوش:** `992300-992399` لمجموعة كذبة أبريل كلها.
  - AprilFools2023: `23209-992320`.
    - Rascal وTickler وCoward.
    - ChessSoldier وChessQueen وChessPatriarch وChessKnight وChessKing.
    - ForbiddenMaster وCrystalDevice.
    - 9 مهرجين.
    - MadClown.

## رسمي ولا مشتق
- **رسمي:** كل النصوص، والـ layouts، وجوايز الأوض والصناديق والأقنعة، وجوايز النقط، وجايزة Mad Clown، وشروط المفاتيح، ونصوص الأيتمات.
  - **Rascal:** level 120 وlife 100000 من monster.dat الرسمي (23209).
- **مشتق (DERIVED):** القيم دي في `Infinity[DB]\AprilFools2023\config.json` وفي تعليق أول سطر من كل ملف في `monsters\*.ini`.
  - خريطة المتاهة: NightmareMaze 11414. أدوار Ordeal Maze الرسمية 11944-11952 مالهاش doc في أي GameMap.dat.
  - أماكن الأوض على الخريطة.
  - الـ looks والـ stats: مفيش look رسمي للوحوش دي. اخترت look 222 (وش مهرج) للمهرجين، و290/919/294/900/102 لقطع الشطرنج.
  - عدد الوحوش.
  - طول رقعة الشطرنج 20.
  - نسب Dungeon Breaker، و10 نقط لكل Trick Amulet.
- **مكان Lyon:** مشتق. Market (1036, 203,193)، والخلية walkable.

## مش موجود (وليه)
- **Trick Store** (تبديل Joy Shard): مفيش جدول أسعار رسمي في الكلاينت. بيطلع رسالة.
- **ترتيب global/regional:** محتاج cross-server. الترتيب المحلي شغال.
- **Joker Event (مقالب المهرج):** مالهوش أي بيانات.
- **ألقاب Playful Player الـ 30 و90 يوم:** مفيش timed titles على السيرفر، فالدايم بس هو اللي موجود.
- **Clown Weapon/Garment/Collection Boxes:** مش في Dungeon Breaker، لأن محتواها اختيارات ملابس وJoy Shard ومالهاش handler.

## ملفات وهوكات
- **الكود:**
  - `Base\AprilFools2023\FoolsKit.cs`: مشترك للمجموعة. فيه Store وItems وArena (نسخ خاصة + وحوش) وHooks.
  - `AprilFools2023Ui.cs`.
  - `AprilFools2023Items.cs`.
- **الهوكات (سطر واحد لكل واحد):**
  - `LuaPush.cs`: 10113 موجود. ضفت 10311 للاستلام.
  - `Procesor.cs`: `FoolsKit.Kit.OnDialog`.
  - `PoolProcesses.cs`: `FoolsKit.Kit.OnTick`.
  - `MsgItemUsuagePacket.cs` `UseItem`: `FoolsKit.Kit.OnUse`.
