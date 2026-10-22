# رحلة الأرنب اليشم (Lunar Voyage، منتصف الخريف 2024)

**الحالة:** `enabled: true`، الجدول `yearly` من 09-10 لـ 09-23. ده الميعاد الرسمي "Sept. 10 - Sept. 23" من `Cn_Res STR_FUI_ZHONGQIU2024_TIP_TIME`.

## الزرار مشترك
- زرار Fgui رقم **10459 "Lunar Voyage"** بتاع MidAutumnVillage. نفس الحدث الرسمي ERP592820.
- المهرجان ده **مالوش icon_show/icon_hide** عن قصد، عشان مايبقاش فيه أيقونة تانية.
- الشباك مابيردّش غير لما MidAutumnVillage يكون شغال. خلّي جدول الاتنين مع بعض.

## بيتلعب إزاي (لفل 80+)
- افتح Lunar Voyage، واختار منطقة، وبعدين "Go".
- الكود في `Base\JadeRabbit\JadeRabbitUi.cs`، ومتنادي من `LunarVoyage.cs`.
- لما تدوس Go: المهمة بتتقبل عند JadeRabbit، وبتنتقل للخريطة، وبتطلعلك رسالة الوصول الرسمية (MsgBox1).

| منطقة الشباك | المهمة | الخريطة الرسمية ← doc | المطلوب |
|---|---|---|---|
| 1 Rainbow Bay | 8066 | 1003074 ← 12599 | تكلّم الأرنب، وتلمّ Tulip وRose وLavender، وترجعله |
| 2 Ring Mountain | 8067 | 1003075 ← 12600 | تكلّم الأرنب، وتروح مكان FlameFox (59,59)، وترجعله |
| 6 Peak of Eternity | 8073 | 1003078 ← 12603 | تكلّم الأرنب، والإزازة، وبعدها النودلز، وترجعله |

- **الجايزة:** 3349968 Dewy Beauty Fragment ×1 لكل مهمة. ده الـ Prize الرسمي في Questinfo.
- كل مهمة مرة واحدة بس للشخصية. الرسمي بيقول: "Jade Rabbit has already completed the exploration of this area".
- علامة النهاية في الشباك (`Reward[1][1]=1`) بتظهر لما المهمة تخلص. اتجرّبت بالـ inject: Rainbow Bay بتبان مكتملة (لوتس دهبي).
- **المناطق 3-5** (Stormy Ocean وSummer Lake وGalaxy Palace): نصوص 8068-8072 موجودة في `Server_Key`، بس المهام نفسها مش في Questinfo. فبيطلعلك "not open".
- **ملحوظة مهمة من الكلاينت:** الشباك بيحسب القفل بنفسه (`fuizhongqiu2024mainproxy InitView`). المنطقة بتفضل رمادي لحد ما كل اللي قبلها يخلص.
  - يعني منطقة 6 مش هتتداس من الشباك طول ما 3-5 مش موجودين.
  - مهمة 8073 شغالة من السيرفر (GoTo 6)، بس اللاعب مش هيقدر يوصلها من الشباك.

## الدليل (رسمي)
- **أسماء المناطق:** `STR_FUI_ZHONGQIU2024_TIP_TITLE_1..6`.
- **النهايات:**
  - `STR_FGUI_150138_1_1` Dreamy Rainbow (رسم Rainbow Bay)
  - `2_1` Bound Foes (Flame Fox)
  - `6_1` Lunar Guardian (تنضيف الزبالة)
- **الخرايط:** `FestivalMaps\maps.txt` (A).
- **الأماكن:** `Questinfo.ini`، وكلها walkable.
  - 8067 FinishNpc الرسمي (361,545) برّه الخريطة (112×112)، فاستخدمنا نقطة الـ Intention (43,46).
- **الأشكال** (`npc.ini` بالاسم والتاج الرسمي):

| NPC | النوع في npc.ini |
|---|---|
| JadeRabbit | 4882 [CuteAdventurer] |
| YellowTulip / RedRose / Lavender | 4886 / 4887 / 4888 [RainbowBay] |
| DiscardedBottle / LeftoverNoodle | 4840 / 4841 [PeakOfEternity] |

- **النصوص:** `Server_Key tPlotEditorPlotEvent_DelText[8066/8067/8073]`.
- **الـ IDs:** NPCs المهرجان 8865001-8865023. اتغيّروا من 8852xxx لأن CastleMystery بيستخدم الرينج ده.

## فرق عن الرسمي (derived)
- **8067:** "Defeat FlameFox" بقت visit لمكان الخناقة (59,59).
  - السبب: FlameFox ليه ستاتس في `monster.dat` (22790، لفل 120، حياة 120000)، بس مالوش lookface في أي مصدر.
  - لو عايزها مقفولة بدل كده: حط `"engine": false` في `quests.json`.
- **بداية المهمة:** الرسمي بيديك المهمة أول ما توصل (BeginNpcId=0). هنا الشباك بيقبلها عند الأرنب، أو تقبلها بإيدك لو دوست عليه.

## اتجرّب
- **Harness بالـ dll الحقيقي على نسخة من الداتا (87/87):**
  - كل منطقة: Go، وبعدين الخطوات، وبعدين تسليم وجايزة واحدة.
  - التسليم التاني مابيدّيش حاجة.
  - الاستثناءات اترفضت: لفل 70، والسجن، والـ dynamic، واللاعب ميت، وأرقام غلط، والمهرجان مقفول.
