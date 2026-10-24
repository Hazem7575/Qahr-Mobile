# مغامرة الصيف (Summer Beach Adventure 2022): شغّال (COMPLETE 15-10-2026)

**الحالة:** `enabled: true`.
- **الجدول:** `manual`. الرسمي إن نسخة الصيف الأحدث بس (Summer2025) هي اللي بتشتغل بمواعيدها كل سنة. المواعيد الرسمية بتاعة النسخة دي (07-05 لـ 07-18) محفوظة في `_schedule_yearly`.
- **تشغيل يدوي:** `@festival on SummerWhales` (وبعدها `@festival auto SummerWhales` يرجّع الجدول).

## إزاي يتلعب (لفل 80+)
- **الزرار:** Summer Adventure فوق (DlgSummerBtn الرسمي). بيفتح الشباك الرسمي DlgSummerMain.
  - الزرار بيبعت الـ Func بتاعه خام (`</F>Func`). اسم الـ Func الرسمي مش موجود في الكلاينت، فاخترنا **`SummerActivePage_MainView`** (نفس عيلة `SummerActivePage_RankView` و`_SynthesisView` بتوع الشباك).
- **الدخول:** زرار Summer Beach في الشباك (`SummerNPC2022_EnterMap`) من توين سيتي أو من Summer Beach بس (النص الرسمي). بينقلك جنب Cassio (172,153).
- **Cassio** (30727، 172,149) بيدّي الطلبات، وعنده "Back to Twin City".

| المهمة | المطلوب | الحالة |
|---|---|---|
| 5113 Scallop Beach | تدوس على Scallop (90,223) تاخد Scallop `3336088` (لحد 5)، وتأكّلهم لـ Cassio | شغّالة |
| 5114 Stranded Whales | تنقذ StrandedWhale (100,245) ×3 | شغّالة |
| 5115 Whale Training | WhaleTraining (220,341) | شغّالة |
| 5111 Whalers` Gear | تقتل 5 Whaler عند (148,29) | شغّالة (15-10-2026) |
| 5112 Whale Hunter | تقتل 15 WhaleHunter عند (148,29) | شغّالة (15-10-2026) |

- **الجايزة:** Wish Conch `3600409` ×1 لكل مهمة (رسمي من Questinfo). مرة واحدة للشخصية.
- **الشباك:** `Lock` بيبقى 2 (Cool Waterfall وWhale Vortex شكلهم مفتوح) لما تخلّص 3 طلبات، زي النص الرسمي "Complete 3 Cassio`s requests". `Guide` = 0 لحد ما تاخد أول طلب.

## رسمي ولا مشتق
- **رسمي:** المهام والنصوص والجوايز والنقط (Questinfo)، الـ meshes (npc.ini)، المواعيد، شرط اللفل 80، الدخول من توين سيتي / Summer Beach، كل رسايل الشباك (`tSummerActivePage_DelText[105]`).
- **الخريطة:** 11829 SummerBeach = doc 12116 `2014vipholiday` (maps.txt، ثقة B). كل النقط الرسمية walkable.
- **مشتق:**
  - مكان الوصول (172,153)، وcooldown الـ Scallop (1.5 ث)، واسم الـ Func. كلهم في `Data\SummerWhales\summerwhales.json`.
  - "Back to Twin City" عند Cassio (زي باقي المهرجانات).
  - **WhaleTraining 30733:** مفيش mesh بالاسم ده. حطّينا Russ `[OldFisherman]` (npc.ini 4434) لأن نص 5115 بيقول "مع الصياد". PROXY.
  - **StrandedWhale 30728:** شكل Cassie (4429، أول واحد من الحيتان الخمسة `[StrandedWhale]`).
  - **Scallop:** NPC واحد بس في النقطة الرسمية (8857001).

## الوحوش (15-10-2026)
- **Whaler** (id `20122`) و**WhaleHunter** (id `20123`) في `monsters\`. بيطلعوا من `spawns.json` في المربع 140..158 × 22..36 حوالين النقطة الرسمية 148,29 (6 Whaler + 10 WhaleHunter، كل 30 ثانية).
- **رسمي:** الاسم، ولفل 120، والـ HP (64000 / 44000)، من `monster.dat` 20122 / 20123.
- **مشتق (DERIVED):**
  - **الشكل:** Whaler = lookface 401 (Robber)، وWhaleHunter = 201 (Thief). مفيش شكل رسمي في أي مصدر في الكلاينت، ومالقيتش فيديو للحدث، فاخترت أقرب شكل: "بشر مسلحين".
  - **الهجوم والدفاع:** من قالب لفل 120 (WaterHades 3617).
  - **مفيش drops**، لأن الجايزة الرسمية Wish Conch بس.
- **5111:** "Defeat Whaler and destroy 5 Harpoons" ليها عدّاد واحد بس (5) في Questinfo. مفيش Harpoon في الكلاينت، فكل Whaler بتقتله = Harpoon. ده مشتق.
- **بلوك الأرقام:** الوحوش من `20122` لـ `995599` لمجموعتي كلها (SummerWhales / Summer2023 / BandsTrouble / Summer2025 / SportsMeet2024 / ArenaSports). الـ NPCs في `88570xx`.

## Whale Wish (السحب) — شغّال بعد الريستارت
- **الفتح:** زرار Whale Wish في الشباك بيفتح الشباك الرسمي `DlgValentinesDayMainUI` بـ DlgID `1255-Darw`.
- **الصلاة:**
  - صلاة واحدة بـ **49 Wish Conch**، و10 صلوات بـ **490**.
  - كل صلاة بتدي: **1 Ambergris** + **49 Favor** + أيتم واحد من العجلة.
- **رسمي:** التكلفة، والـ Ambergris، والـ Favor (نص DrawInfo)، والـ 20 أيتم بتوع العجلة، والـ 8 جوايز بتوع المراحل، وكل الرسايل. كلهم من `valentinesdayinfo` و`luauires_en`.
- **مشتق** (في `Data\SummerWhales\summerwhales.json`):
  - أوزان العجلة، حسب لون الخانة: أصفر 5، أحمر 25، أبيض 50.
  - عدد الصلوات لكل مرحلة: 10، 20، 30، 50، 80، 120، 160، 200.
  - `favorMax` = 9800.
- **ملاحظة:** خانة 14 (`194825`) مش موجودة في itemtype بتاعنا، فعمرها ما بتطلع.
- **جايزة المرحلة:** مرة واحدة للشخصية. بتتسجّل في `Data\SummerWhales\players.json` **قبل** ما الأيتم يتدي.

## تحويل الـ Ambergris — شغّال بعد الريستارت
- **Garment:**
  - التكلفة 15 Ambergris، والنتيجة Silver Bubble (Sunny) (B) `199325`.
  - النسبة بتزيد 0.5% كل 10 مرات فشل، وبترجع تاني بعد النجاح. ده رسمي.
  - **نسبة البداية 5% مشتقة.**
  - لو فشلت، بتاخد حاجة من دول (رسمي): 500 Chi، أو Silver Bubble (B)، أو Permanent Stone (B)، أو Big Permanent Stone (B)، أو 25-100 Universal Rune Essence (B).
- **Accessory:** 10 Ambergris بتديك Cool Surfboard `3337923`.
- **الأيتمز** (أول ما تدوس عليها بيفتح لها dialog):
  - **Ambergris** (`3337922`):
    - تحويل.
    - فك ربط الـ garment بـ 299.
    - Chi: 500 لكل واحدة.
    - الشراء بالـ CP مقفول، لأن السعر الرسمي مش معروف.
  - **Cool Surfboard** (`3337923`): تختار 2-Handed `350189` أو 1-Handed `360401`، لمدة 7 أيام، وبـ 1% bless.
  - **Whale Spirit** (`3337924`):
    - **30** Whale Spirit بيعملوا Whale King (Holy) `200774`. الرقم 30 مشتق.
    - Chi: 1000 لكل واحدة.

## مش موجود على السيرفر (رسالة بس، مفيش جوايز)
- **الترتيب، وCool Waterfall، وWhale Vortex/Whale Waters:** دول cross-server، والخرايط بتاعتهم مش موجودة على السيرفر.
- **متجر Wish Conch (ExchangeNpc):** مالوش جدول في الكلاينت.

## الكود
- `Base\SummerWhales\SummerWhalesUi.cs`.
- `SummerWhalesWish.cs` (جديد).
- `FestPlayerStore.cs` (جديد، ومشترك مع باقي مهرجانات المجموعة).
- Hooks:
  - `LuaPush.HandleCallback`: `OnRaw` (النداءات الخام).
  - `Procesor.cs` قبل `FestivalManager.OnNpc`: الـ Scallop.
  - `Procesor.cs` قبل البحث عن الـ NPC: `SummerWhalesWish.OnItemDialog` (dialogs الأيتمز). اتضاف 15-10.
  - `MsgItemUsuagePacket.UseItem`: `SummerWhalesWish.OnUse`. اتضاف 15-10.
- **Harness:** `scratchpad\swh2`، والنتيجة **77/77 PASS**. اتجرّب فيه:
  - السبون.
  - 5111/5112.
  - السحب، بأعداد غلط وspam.
  - شنطة مليانة، ولفل 70.
  - المراحل مرتين، وكمان بعد إعادة التحميل من الديسك.
  - التحويل.
  - الأيتمز.

_اتعمل 14-10-2026. المصادر: `client - orignal\ini\Questinfo.ini`، و`npc.ini`، و`monster.dat`، وluaui `dlgsummeractivities` + `luauires_en`، و`Server_Key` (tSummerActivePage / tSummerActive2022Cross)، و`FestivalMaps\maps.txt`._
