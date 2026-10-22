# مغامرة الذكرى السنوية 2023 - Anniversary Adventure (MayAdventure2023)

**الحالة (15-10-2026، مرحلة COMPLETE):** الشباك الرسمي كله شغال بجد.
- **التشغيل:** `manual` (`@festival on MayAdventure2023`)، لأن الذكرى السنوية الأحدث بس (Anniversary2026) هي اللي بتشتغل سنوي. المواعيد الرسمية 05-16 لـ 05-29 محفوظة في `_schedule_yearly`.
- **الشرط:** ريبورن تاني (رسمي).

## بيتلعب إزاي
1. **الزرار** `DlgMayAdventureMainBtn` (اسمه Adventure) بيفتح الشباك الرسمي `DlgMayAdventureMainUi`.
2. **اللوحة:** 4 خرايط × 20 خانة، وأنواع الخانات رسمية من `luagui 1380 MapData` (عادية / علامة / كنز).
   - **Adventure x1** بياخد Courage Token واحد، و**x5** بياخد 5. لازم تكون في Twin City، ومش في تيم ولا بوث ولا اسمك أزرق (الرسايل رسمية من `tAnniversaryAdventure_DelText`).
   - كل خطوة = مغامرة: **30 Courage Points** (رسمي) وجايزة من جوايز المعاينة الرسمية (`1380-2-other`): Courage Pearl، Courage Medal، Awakening Spirit، PermanentStone، +4 Stone، Holy Whale، Courage Pack.
   - بعد ما الحركة تخلص بيظهر شباك الجوايز الرسمي (`DlgRewardFourteen`)، ولما تدوس OK بيرجعك للوحة.
   - مكانك على اللوحة ونقطك متسجلين، وبيبدأوا من الأول مع كل تشغيلة جديدة للمهرجان.
3. **Treasure Hunt:** بيسألك "Use 1 Courage Pearl?"، ولو وافقت بياخد Pearl ويديك supplies (Courage Medal 1-3، أو Courage Pack، أو Awakening Spirits)، و+10 نقط.
   - شباك السحب الأصلي بتاع 2023 الكلاينت حوّله لعيد الشكر 2023، فاستخدمنا رسالة تأكيد وشباك الجوايز الرسمي.
4. **Exchange Store** (`DlgMayAdventureShop`، صفحتين): بتدفع Courage Medals وتاخد +3/+5/+7/+8 Stone (B) (الأحجار دي اسمها في لوا الشوب نفسه)، وPermanentStone، وCourage Pack، وAwakening Spirit، وHoly Whale. لكل صنف حد للشخص في التشغيلة.
5. **Courage Rankings:**
   - زرار الجوايز بيفتح المعاينة الرسمية (`1379-rank-item[10238]`).
   - "View Courage Rankings" بيعرض أعلى 10 ومركزك.
   - **جوايز الترتيب رسمية** (No.1 / 2-3 / 4-6 / 7-10: صناديق الأجنحة، Mythsoul L6، Lunar Lottery Tickets، Universal Rune Essence). بتتجمد أول ما تشغيلة جديدة تبدأ، وبتتصرف مرة واحدة أول ما اللاعب يفتح الشباك (لو مفيش مكان في الشنطة بتستنى).
6. **علامة + جنب العملات (الطرق الرسمية لجمع Courage Tokens):**
   - Daily Sign-in Pack: مرة في اليوم.
   - 100 / 200 Active Points Pack: مرة في اليوم لكل واحدة (بنقرا الـ ActivityPoints الحقيقية).
   - CP Mall: رسالة إن صناديق الشجاعة مش بتتباع هنا.
7. **الصناديق** (لو حد معاه): +Stone / Star / Stone / Awakening / Contribution Courage Box، وRandom Pearl Pack، وCourage Pack. بتتفتح بالمحتوى المكتوب في نص الأيتم الرسمي. الملف: `Infinity[DB]\MayAdventure2023\boxes.json`.

## الرسمي والمشتق
**رسمي:** شكل اللوحة وأنواع الخانات، تكلفة 1/5 توكن، 30 نقطة لكل مغامرة، الشروط ورسايلها، أصناف الجوايز (المعاينة ونصوص الأيتمز ولوا الشوب)، جوايز وشرايح الترتيب، محتوى الصناديق الثابتة.

**مشتق (في `Infinity[DB]\MayAdventure2023\mayadventure.json`):** أوزان وكميات الجوايز، حد 60 خطوة في اليوم، أسعار وحدود الشوب، 10 نقط لكل سحب، توكنز الـ Sign-in (2) والـ Active Points (3 و5)، وأوزان الصناديق العشوائية. كوبونات الـ CPs بتاعة May Sale 2023 اتشالت لأن المهرجان ده مش موجود عندنا.

## مش ممكن ولي
- **مشاهد المغامرات** (Zombie Master، Ghost Fire House، ...): هي المهام 5924-5933 ومالهاش ماب في الكلاينت، فالمغامرة بتخلص فورًا بجايزتها.
- **Ahriman Battle وLava Land:** خرايطهم مش في `FestivalMaps\maps.txt`، وLava Land كمان cross-server.
- **الـ NPCs** Ortner (31610) وJessica (31611): أماكنهم الرسمية مش معروفة، فما اتحطوش. مفيش NPC أو وحوش جديدة.

## الملفات
- **الكود:** `Base\MayAdventure2023\MayAdventure2023Ui.cs` و`MayAdventureCore.cs` و`FestivalBoxOpen.cs` و`AnnivKit.cs` (أدوات مشتركة لمجموعة الذكرى السنوية).
- **الحالة:** `Infinity[DB]\MayAdventure2023\players.txt` و`ranks.txt`.
- **الهوكات:** سطر LuaPush (msg 10236) زي ما هو، وسطر جديد في `MsgItemUsuagePacket.cs` لفتح الصناديق.
