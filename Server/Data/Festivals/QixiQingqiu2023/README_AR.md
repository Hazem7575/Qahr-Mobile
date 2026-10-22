# عيد تشيشي 2023 العالمي - تل اليشم (QixiQingqiu2023)

**الحالة:** `enabled: true`، و`yearly` من 08-22 لـ 09-04. دي المواعيد الرسمية: ActBtnConfig[9] ActTime = "2023-08-22 00:00 2023-09-04 23:59" (والصيني كان 8.15-8.28).
- **تشغيل يدوي:** `@festival on QixiQingqiu2023`.
- **محتاج:** سيرفر فيه `Base\QixiQingqiu2023\QixiQingqiu2023Ui.cs`، وسطر الهوك في `LuaPush.cs` (msg 10272).
- **تنبيه:** المواعيد بتتداخل مع `QixiInterplane2024` (08-06 لـ 08-26). الرسمي إن كل واحد كان في سنة لوحده.

## بيتلعب إزاي (لفل 80+)
1. لما المهرجان يشتغل، بيظهر الزرار الرسمي `DlgChineseSaintValentinesDayBtn` ("Jade Hill Panaroma"، الصورة `2023qqqx_btn4`).
   - الزرار بيقرا اسم الدالة من `g_player_info`، فبنظهره بـ `Dlg_OpenUI` ومعاه `Head.Func[1] = 'QingQiuMainNpc_OpenMain'` (الاسم الرسمي، موجود في g_function.dat).
2. لما تدوس عليه، بيروح 10272، والسيرفر بيفتح الشباك الرسمي `DlgChineseSaintValentinesDayMain` ("Jade Hill"):
   - المدة "Aug. 22 - Sept. 4".
   - التلات صفحات: Peachland Life، وFox Tie، وFox Practice.
   - الـ 7 ذكريات (Story 1-7) كلها مقفولة. دي حالة البداية الرسمية.
3. أي زرار بيرد برسالة نظام مؤدبة، ومعاها عدد **Clue Bell** (3380015) و**Marital Stick** (3344212) و**Marital Fruit** (3600624) اللي في الشنطة (داتا حقيقية). الذكرى بتتقال باسمها الرسمي (Server_Key Story: Griffin Invasion …).

## الرسمي والمشتق
**رسمي:**
- الزرار والشباك وشكل الداتا (`luaui\dialog\dlgchinesesaintvalentinesday`، و`luagui\1399.dat`).
- الـ msgId 10272، و`QingQiuMainNpc_OpenMain`، و`QiXiPlay5_OpenQixiMain` (Fox Tie)، و`MassifDraw_DrawView` (Fox Practice).
- أسماء الـ 7 ذكريات، وأرقام العناصر، والمدة، واللفل 80.

**مشتق:**
- تكلفة فتح كل ذكرى المكتوبة تحتها ("X1 Open" = 1 Clue Bell). العدد الرسمي على السيرفر بس.
- أسماء الدوال `QingQiuMainNpc_OpenDaily` / `_GuideEnd` / `_Story` / `_EffectEnd`: دي بتتبعت من السيرفر للكلاينت، فالاسم الرسمي مش في الكلاينت.
- رسايل "مش متاح على السيرفر".
- جوايز كل ذكرى (Award) فاضية، لأنها على السيرفر بس.

## مش موجود (رسالة نظام، ومفيش أي جايزة بتتدي)
- خريطة Jade Hill: مش في `FestivalMaps\maps.txt` (الـ doc `qingqiu_new` مستخدم لـ CrisisSeries بس).
- Peachland Life (مهام NPC 31811، tQiXiPart2).
- الـ 7 ذكريات (PlotEditor 5561-5567، ومش في Questinfo).
- Fox Tie (التوفيق بين اللاعبين)، وFox Practice (السحب)، وJade Hill Points، والترتيب.
