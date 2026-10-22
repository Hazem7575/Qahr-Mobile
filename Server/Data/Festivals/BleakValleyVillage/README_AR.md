# Celebration Realm (BleakValleyVillage): الذكرى السنوية 2023

**الحالة (15-10-2026، مرحلة COMPLETE):** المهرجان بقى بيتلعب بجد.
- **التشغيل:** `manual` (`@festival on BleakValleyVillage`)، لأن الذكرى السنوية الأحدث بس (Anniversary2026) هي اللي بتشتغل سنوي.
- **المواعيد الرسمية** (04-25 لـ 05-15) محفوظة في `_schedule_yearly`.
- **الشرط:** لفل 80 (رسمي).

## بيتلعب إزاي
1. **الزرار** `Dlg23AnniversaryBtn` ("Celebration Realm") بيفتح خريطة Anniversary Zone الرسمية، وفيها بياناتك الحقيقية: النقط، ومستوى النار، والـ Grit Points، وعدد الـ Spark Logs.
2. **Anniversary Bonfire:**
   - كل ضغطة بتاخد 45 Spark Log وبتديك 30 Anniversary Points (رسمي).
   - مستويات النار 1..5 عند 30/210/960/2040/3540 نقطة (رسمي).
   - كل مستوى بيفتح مناطق (رسمي من OpenTips): Viper Swamp عند L1، وBleak Valley عند L2، وFiery Desert وAnnihilation Vortex عند L3، وCollapsed Debris عند L4، وTimeless Shrine عند L5.
   - كل ضغطة فيها فرصة Faith Crystal، والفرصة بتتضاعف فوق 20000 نقطة (المضاعفة رسمية، والنسبة مشتقة).
3. **Celebration Monument:** بيفتح شباك جوايز النقط التراكمية الرسمي (`DlgMidAutumnRank`، ActivityId 10218):
   - العتبات 1000/3000/5000/10000/30000/45000/60000/100000، وبعدها جايزة كل 3000.
   - الجوايز رسمية من `MidAutumn_Config[10218]`.
   - "View Rankings" بيعرض أعلى 10.
4. **Grit Points:**
   - +1 كل 30 دقيقة، و+200 كل يوم الساعة 0:00 (رسمي).
   - أول ما المهرجان يبدأ معاك 200 (مشتق).
   - Anniversary Bread (1/2/100) بتزود Grit (رسمي).
5. **Dragon Remains (رحلة Dragonette):**
   - تبعته رحلة (120 دقيقة، مشتق)، ولما يرجع بيظهر شباك Search Result الرسمي بصورة من الـ 20 صورة الرسمية.
   - بيجيب Spark Logs وأحيانًا Faith Crystal.
   - استلام الجايزة بياخد 1 Grit Point (القاعدة 3 الرسمية).
6. **Bleak Valley (الماب 12100، ودي `sary02_new` doc 11203):** الزرار "Go" بيوديك قرية Foison.
   - **Reputation Officer** (عند 187,222 من الـ minimap الرسمي، والـ id بتاعه 8876001):
     - "Accept the quest" بياخد 100 Grit (رسمي) وبيديك مهمة من قرويين القرية (مهمة واحدة لكل قروي في اليوم).
     - "View Reputation" بيوريك جوايز السمعة الرسمية: 50/100/150/200/300 Spark Logs عند 50/100/150/200/300 سمعة.
     - "Buy Log Sale Packs" (الباك فيه 40-50 Log حسب نص الأيتم الرسمي)، والخصم بيزيد مع مستوى السمعة.
     - "Return to Twin City".
   - **المهام 7653-7667 شغالة.** النصوص والأهداف والـ NPCs وأماكنهم كلها رسمية من Questinfo. لما تخلص مهمة بتاخد 40 Spark Log و10 سمعة (الرقمين مشتقين).
   - **الوحوش** Blackcock وWeasel وWildMonkey (المهام 7659-7661) بتطلع في أماكنها الرسمية وبتتقتل.
   - **Exit** (207,233) بيرجعك Twin City.
7. **الصناديق:**
   - Jade Serpent Box وAnaconda Pack (50 Grit)، وLucky Debris Pack (100 Grit).
   - Log Sale Pack، وDaily Challenge Box، وSuper Dominator Box.
   - كلهم بيتفتحوا بالمحتوى المكتوب في نص الأيتم الرسمي.

## الـ IDs المحجوزة (مجموعة الذكرى السنوية)
- **NPC:** البلوك 8876000-8876999. المستخدم: 8876001 Reputation Officer، و8876002 Exit.
- **الوحوش:** البلوك 997100-997199. المستخدم: 997101 Blackcock، و997102 Weasel، و997103 WildMonkey.
- **NPCs المهام:** IDs رسمية 36556-36574 (في `npcs.txt`).

## الرسمي والمشتق
**رسمي:**
- كل الأرقام المكتوبة فوق إنها رسمية.
- IDs الـ NPCs وأماكنهم، ونصوص المهام وأهدافها.
- الأيتمز وجوايزها، وشبابيك الكلاينت.

**مشتق** (في `Infinity[DB]\BleakValleyVillage\bleakvalley.json` و`quests.json` و`spawns.json`):
- Grit البداية، ونسبة Faith Crystal، والسمعة لكل مهمة (10)، وجايزة المهمة (40 Log).
- سعر Log Sale Pack وخصمه وحده اليومي.
- رحلة Dragonette (الوقت والجوايز)، ونسبة Silver Key.
- **أشكال وإحصائيات الوحوش:** مفيش ليهم صف في monster.dat ولا look في الكلاينت، فاخترنا: Blackcock = FlyingRooster 304، وWeasel = FireRat 202، وWildMonkey = EvilMonkey 156. الإحصائيات من قالب لفل 80.
- **أشكال الـ NPCs:** الـ meshes PROXY، لأن القرويين مش موجودين في npc.ini. الـ VillageHead وHoneysuckle وOsmanthus وTrap بنفس الاسم بالظبط.

## مش ممكن وليه
- **باقي المناطق** (Anniversary Forest، Viper Swamp، Fiery Desert، Collapsed Debris، Timeless Shrine، Dimensional Portal، Annihilation Vortex، Ascension Trail): خرايطهم مش في `FestivalMaps\maps.txt` (مش في GameMap.dat)، فبترد برسالة.
- **Crest Treasury (المتجر):** بضاعته داتا سيرفر مش موجودة في الكلاينت.
- **Supply Officer / Planning Officer** في Twin City: أماكنهم الرسمية مش معروفة. يعني تبديل Faith Crystal بالهدوم مش موجود.
- **انتخاب السفير وتحدي الأبراج:** cross-server.

## الملفات والهوكات
- **الكود:** `Base\BleakValleyVillage\BleakValleyVillageUi.cs` و`BleakValleyCore.cs`. الكود ده بيستخدم `Base\MayAdventure2023\AnnivKit.cs` و`FestivalBoxOpen.cs`.
- **هوكات:**
  - سطر LuaPush لـ msg 10217 (موجود من قبل).
  - سطر LuaPush جديد لـ msg 10218.
  - Reputation Officer وExit بيتحطوا تلقائي طول ما المهرجان شغال.
- **الحالة:** `Infinity[DB]\BleakValleyVillage\players.txt`.
