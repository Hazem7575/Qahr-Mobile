# Anniversary Sail - الذكرى السنوية 2026

**الحالة:** `enabled: true`، سنوي من 21-04 لـ 11-05 (التاريخ رسمي: ERP660950 `[4.21-5.11]`، و Cn_Res `STR_FUI_ACTTIME_10883` و`STR_FUI_26ZNQCENTER_TIME`).

## اللي شغال
- زرار فوق رسمي Fgui `150428` (اسمه في الكلاينت "Anniversary Sail").
- الضغط عليه (msg 10883 `AnswerQuestion_OpenMain`) بيفتح الشباك الرسمي `Fui26ZnqCenter`: خريطة الموانئ السبعة، و Revival Academy، و Residential Area، و Naval Drill، و Luoxing Pagoda.
- صفحة Highlight Events `ActInfo[10883]` وزرار الدخول بتاعها.
- الكود: `Infinity [Sourse]\Base\Anniversary2026\Anniversary2026Ui.cs`.

## الداتا اللي في الشباك
- جوايز كل ميناء (reward-show ids `4300x000-2`) وأدوات شروط الفتح (`3356092`-`3356101`، زي Quanzhou Port Chart و Deep Dive Pill): **رسمية**. موجودة في luauirewardshow و itemtype بتوع الكلاينت، وأساميها مطابقة لـ Cn_Res.
- كل لاعب بيبدأ من الأول: Quanzhou Port (Exploring) والباقي مقفول، والنقط 0، لأن مفيش حاجة في السيرفر بتقدّم الحالة.
- أقصى نقط 99999999: **مستنتج** من sample الداتا بتاع الكلاينت (`pointsMax` في الكونفيج).

## الكويز
الكلاينت **مفيهوش بنك أسئلة** للكويز ده. `tAnswerQuestion` في Server_Key فيها النصوص والرسايل بس، والأسئلة داتا سيرفر. عشان كده Revival Academy بترد برسالة سيستم ومفيش أسئلة متألفة.

## اللي لسه مش موجود (بيرد برسالة سيستم)
الكويز، و Naval Drill، و Residential Area، و Stargazing في Luoxing Pagoda (Star Raft)، وتجنيد الأبطال وفتح الطرق والجوايز بتاعتها، والرانكنج. خريطة Anniversary Sail مش في `maps.txt`.

## الكونفيج
`Infinity[DB]\Anniversary2026\anniversary2026.json`: `minLevel` (80 رسمي) و`pointsMax` (**مستنتج**).
