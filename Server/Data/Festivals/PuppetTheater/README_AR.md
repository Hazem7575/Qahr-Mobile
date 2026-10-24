# Strings of Terror (PuppetTheater): هالوين 2025 — كامل (2026-10-15)

**الحالة:** `enabled: true`، الجدول سنوي (auto) من 28 أكتوبر لحد 17 نوفمبر (رسمي، `STR_LUAUI_25HALLOWEENMAIN_TIME`).
- **تنبيه:** بيتقاطع مع Halloween2024 وHalloweenSenluo2023 لو هما كمان yearly.

## اللي شغال
- **الزرار 150348** بيفتح الشباك الرسمي Halloween25Main بداتا حقيقية:
  - SP وSoulthread من الشنطة، وTerror Points.
  - فتح المراحل، ونسبة الاستكشاف، وتقييم البوس S/A/B.
  - الـ SP بيرجع 150 كل يوم (رسمي).
- **الاستكشاف:** 10 مناطق على اللوحة الرسمية 8×8 (`Fgui_25Wsj_Zgz`)، 5 بالنهار و5 Dark.
  - أسامي الأحداث ونصوصها رسمية: صناديق، مواجهات، حكايات الـ NPC (GBJQ)، القطع الأثرية، البوابات والآليات والحراس.
  - كل خطوة بتصرف SP، وكل منطقة بتفتح اللي بعدها.
  - لما توصل الـ End بيطلعلك "Explore again / Challenge boss".
- **Challenge boss:** في المناطق اللي ليها قصة، المهام الرسمية 9186-9201 بتشتغل الأول، بـ NPCs رسمية في نسخ خاصة من الخرايط 1004398-1004403. بعد كده بوس في نسخة خاصة، وتقييمه بالوقت.
- **Puppet Home:** 5 دمى (Papet + 4 أرواح بتصحى لما تخلص مناطق النهار 2-5).
  - تختار جايزة لكل دمية من القوايم الرسمية 301001-301003.
  - كل UPG بيصرف Soulthreads وبيدي Terror Points.
- **Terror Points:** مكافآت عند حدود معينة، وترتيب محلي (StageSybTemplate 10768001/002).
- **Puppet Finale:** مرة في اليوم، Evil Marionette، ولما HP بتاعه يخلص بيبقى Nightmare Puppet.
  - أول تحدي بيدي 2 Soulthreads، وأعلى damage بياخد 2 Soulthreads وصندوق 1D Solar Steel Box (رسمي).
- **NPCs:**
  - في تويين سيتي: Suyi 8893001 (القصة والقايمة وتحويل SP وزيارة Vault Palace)، وElise 8893002 (Puppet Store بأسعار CP رسمية)، وStrange Spectator 8893003.
  - في Vault Palace 12818: Broken Puppet 8893004 (Puppet Adventure: 3 أرواح، 3 قطع، والتسليم 1/2/3).
- **الصناديق بتتفتح بالمحتوى الرسمي:** Soulthread Boxes 46/93/187، Random Soulthread Pack، SP Awakening Pack، Contribution Pack، Terror Box، Mirage Shade/Marionette (T)، Violet Dreamkey Boxes.
- **الحماية من الاستغلال:**
  - كل حاجة بتتعمل على السيرفر: lock لكل لاعب، وthrottle، والتحقق من مكان الـ NPC، ومساحة الشنطة قبل الخصم.
  - مفيش جايزة بتتاخد مرتين (صناديق اللوحة، أول قتل للبوس، الحدود، المغامرة، الـ Finale).
  - كله بيتسجل في `Data\PuppetTheater\log.txt`.

## رسمي ولا مشتق
- **رسمي:**
  - النصوص (`texts.json` من Server_Key وCn_Res).
  - الـ items وأسعار المتجر، وقوايم الجوايز، ومحتوى الصناديق.
  - الـ 16 مهمة ونقطها، وأسامي/looks الـ NPCs (NpcX 110165/110127/110201 و110187-110209).
  - MikesPuppet وMerchantPuppet (monster.dat 23358/23355: lv120، life 50000).
- **مشتق (DERIVED):**
  - الأرقام كلها في `Data\PuppetTheater\config.json`: شكل اللوحات، SP لكل خطوة (10)، كميات الـ Soulthread، تكلفة الـ UPG (10)، حدود النقط، HP الـ Finale، المغامرة.
  - **الخرايط 1004398-1004403** → doc 12473 (2019halloween_new). الخريطة الرسمية 13074-13079 مش موجودة في أي GameMap.dat، و2019halloween_new هي الخريطة الوحيدة اللي بطابع هالوين وكل نقط Questinfo بتاعة الـ 6 خرايط walkable عليها.
  - **looks/stats الوحوش 993301-993318:** مذكورة في أول سطر من كل ini.
  - **أماكن الـ NPCs:** مفيش إحداثيات رسمية.
  - **مهمة 9192:** بتخلص عند Clues، لأن الـ NPC الرسمي على خريطة تانية.

## مش موجود (وليه)
- **Finale والترتيب cross-server:** محتاجين سيرفرات تانية، فالترتيب محلي.
- **جوايز الترتيب بالبريد:** مفيش جدول رسمي ليها.
- **هالة String of Fate وتسريحة Mirage:** مالهمش item في السيرفر.
- **عرض أسلحة Anima (302002):** مفيش طريقة شراء رسمية معروفة.

## قايمة الـ items (Mandatory item checklist)
كل item المهرجان بيديه أو بيبيعه، والـ handler بتاعه. الهاندلرز كلها في `PuppetNpcs.UseItem` من خلال `FoolsKit.Kit.OnUse`. بتتأكد من الكمية، وبتشيل الـ item بعد ما تتأكد إن في مكان في الشنطة، وبعدين بتدي الجايزة وتسجلها وتحدّث الشبابيك المفتوحة.

| Item | المصدر | لما تستخدمه |
|---|---|---|
| SP 3354702 | يومي 150، SP Awakening Pack، زيارة Vault Palace | عملة بتتحسب من الشنطة. كليك يمين بيفتح شباك المهرجان، وكل خطوة استكشاف بتصرف 10 |
| Soulthread 3600813 | اللوحة، البوس، الـ Finale، المغامرة، الصناديق | عملة. كليك يمين بيفتح Puppet Home ("Use to train puppets") |
| SP Awakening Pack 3354672 | المتجر (28 CP، وزرار "+" بتاع SP)، حد النقط | 50 SP + Awakening Spirit (رسمي) |
| Random Soulthread Pack 3354671 | المتجر (95 CP، وزرار "+" بتاع Soulthread)، حد النقط | 3/4/8/100 Soulthreads (رسمي، النسب مشتقة) |
| Small/Medium Soulthread Box 3354668/3354669 (والـ Grand 3354670) | حدود النقط | 46/93/187 Soulthreads (رسمي) |
| Terror Box 3354679 | حدود النقط | 4 Shadow Runes / 50 Awakening Spirits / 1 DragonFruit (اختيار الـ 10KK Medal اتشال) |
| 1D Solar Steel Box 3354673 | أعلى damage في الـ Finale | اختيار: 2 Solar Steel (1-handed) أو 1 (2-handed)، يوم واحد، bound |
| Papet 4330005 | المتجر 30999 | الحيوان الأليف، بيشتغل من نظام الـ Collection الموجود |
| Mirage Shade (T) 3354681 / Mirage Marionette (T) 3354683 | المتجر 2999 / 18999 | لبس 194145 / 194155 بـ bless 1% |
| Violet Dreamkey (1H)/(2H) Box 3354689/3354690 | المتجر 12999 / 8999 | 2× 360468 / 1× 350230 بـ bless 1% |
| +3 Stone (B) 3346829 | تدريب الدمى | +3 Stone 730003، خمس أيام، bound |
| Tiger/Leopard Chi Liquor 3314207/3321077 | تدريب الدمى | +1000 / +100 Chi Points (رسمي) |
| Awakening Spirit، Night Oracle، Shadow Rune، Moon/Ginseng/DragonFruit، UniversalRuneEssence، Senior/Free Training Pill | التدريب / الصناديق | خامات بتشتغل في الأنظمة الموجودة (Eonspirit، Beasts، Runes، JiangHu) |

- **اتشالوا من الجوايز:** Contribution Pack 3354678 والـ Contribution Medals، وClass Awakening Box 3341018. مفيش نظام شغال ليهم على السيرفر لسه. الـ handler بتاع Contribution Pack موجود لو حد إداهولك من حتة تانية.

## ملفات وهوكات
- **ملفات جديدة (اتضافت للـ csproj):** `Base\PuppetTheater\PuppetTheaterGame.cs` و`PuppetNpcs.cs`. الملف `PuppetTheaterUi.cs` اتكتب من جديد.
- **الهوكات:**
  - سطر في `FoolsKit.Kit.OnDialog`.
  - سطر في `FoolsKit.Kit.OnUse`.
  - LuaPush 10768 موجود من قبل.

## تعديلات الفيديو الرسمي (2026-10-15، الدفعات 1 و2)
- **اللوحات:**
  - Jester's Maze وPuppet Garden وMenagerie Factory رسمية من الفيديو.
  - المناطق 4 و5 ومراحل الـ Dark من 6 لـ 10 مشتقة، وكل لوحة ليها شكل مختلف. الأبواب على لوحات الـ Dark بس.
  - الخلية اللي جنب خلية مستكشفة بتكشف صورتها واسمها (state=1, eventState=0)، زي الفيديو، والصناديق بتستخدم event 10 "Chest".
  - خلية الـ NPC بتبدأ مهمة القصة.
- **البوس والـ Finale:** بقوا معركة دُمى بالأدوار جوه شباك الكلاينت الرسمي FuiPetDaily، وفيه عداد الجولات وزرار Skip.
  - الأعداء: Spider Queen وFox Marionette... وArchon (ENEMYNAME 35-44)، والـ Evil Marionette (52).
  - التقييم: S لو في 3 جولات أو أقل، A لو في 6 أو أقل، وB لحد 10.
  - HP الـ Evil Marionette على مستوى السيرفر = 250000.
  - **مشتق:** الـ stats ومعادلة الضرر.
- **Puppet Home:** Soulthread واحد لكل UPG والزرار بيصرفهم كلهم، و50 نقطة لكل UPG، والـ 5 دمى صاحيين من أول يوم، وكل دمية ليها 8 جوايز رسمية.
- **Octave Bell (3354663):** بييجي من أول مرة تغلب البوس، ومن النقط ومن المستويات. التبديل في متجر Elise بالـ Octave Bells:
  - Mirage Shade (B) بـ 30.
  - Mirage Marionette (B) بـ 60.
  - Mirage Marionette بـ 80.
  - Papet بـ 100.
- **شباك المكافآت الرسمي (EnsemblePointRank 10768001-004):**
  - النقط: Terror Box كل 1000 نقطة، والحدود من 279001.
  - المستوى: من 288 لـ 291.
  - الاستكشاف: من 294 لـ 297.
  - الترتيب المحلي: صناديق Puppeteer Box من 287، وبتتاخد من 11-17. لازم 20000 نقطة على الأقل.
- **Vault Palace:**
  - Orion (8893005): "View statues" مرة في اليوم بتدي SP، و"Puppet Adventure".
  - SilkSpecter 23390 وWeepWood 23392 على الخريطة المشتركة، وكل واحد بيدي قطعة.
  - التمثال الذهبي (8893006) بيدي القطعة التالتة.
  - التلات قطع بيدوا Puppet Select Box (3600816).
- **Items جديدة شغالة:**
  - Puppet Select Box.
  - 1D Violet Dreamkey Box.
  - Terror Weapon Pack.
  - Puppeteer Boxes.
  - Contribution Medals: بتزوّد Nobility Contribution، وصناديقها 3346822-24 بتتفتح.
- **مش موجود:**
  - صندوق الـ Avatar Frame والـ Emoji، لأن مفيش نظام ليهم على السيرفر، فالاختيار ده بيترفض.
  - Butcher Knife، لأن مالوش item.
  - ترتيب Global/Regional، لأنه محتاج cross-server.
  - خريطة Puppet Garden الزرقا مش في الكلاينت، فهي لسه 2019halloween_new.
  - Thousand-tailFox، لأن مالوش NPC type.
