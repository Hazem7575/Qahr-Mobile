# مهرجان الربيع 2024 - سنة التنين (SpringFestival2024): الزرار والشباك الرسمي بس

**الحالة:** `enabled: true`، جدول سنوي من 1 فبراير لحد 21 فبراير. ده التاريخ الرسمي من الكلاينت (`actbtnconfig` `ActBtnConfig[18]` = "2024-02-01 .. 2024-02-21"، و`luaUiRes[1459].Time` = "Feb. 1 - Feb. 21").

## اللي شغال
- **زرار الـ lua الرسمي `DlgFestival2024BTN`** (luagui 1462، الصورة `2024qqcj_btn11`، مكتوب عليه "Dragon Gift")، بيظهر طول ما المهرجان شغال.
- **الشباك الرسمي `DlgFestivalMain2024Task`** (msg 10358، الكود في `Base\SpringFestival2024\SpringFestival2024Ui.cs`):
  - الزرار (`DlgFestival2024BTN_OpenUI` / `DlgFestival2024BTN_NewOpenUI`) بيبعت `LuckyPool2024_OpenLuckView`، والسيرفر بيفتح الشباك.
  - الشباك اسمه "Dragon Gift". المهام الأربعة (Exorcism، وDragon Blessing، وDragon Trial اللي هو تحدي أولاد التنين التسعة `Dlg24SpringChallenge`، وYear Beast) بتبان "Locked". لو دوست عليها الكلاينت نفسه بيقول نصه الرسمي.
  - Fortune Points (الدايرة اللي في النص) ونقط Auspicious الشخصية بيبانوا 0، والتنين نايم (الفقاعة الرسمية "Find the fragments to awaken the dragon!").
  - زرار القواعد (?) والوقت بيطلعوا من نصوص الكلاينت الرسمية.
  - لو دوست على التنين أو صندوق Fortune Pool بتطلعلك رسالة مؤدبة إنه مش مفتوح لسه.
- **رسالة انتهاء الحدث:** نصها رسمي من `Server_Key` `tSpringFestival2024Main[32466][Text122]`.

## اللي مش موجود وليه
- مفيش مهام في `Questinfo.ini` ومفيش خرايط في `maps.txt`.
- مهام Dragon Blessing Painting اليومية، وإيقاظ التنين والأمنيات (شباك `DlgFestivalMain2024Wish`: جوايزه السبعة والعشرة جداول سيرفر)، وتحدي أولاد التنين التسعة (الجوايز التسعة والاحتمالات على السيرفر)، وExorcism، وYear Beast، وFortune Pool والرانكنج: كلها سكربتات سيرفر رسمية (`tSpringFestival2024Main` و`tLuckyPool2024` و`tSpingYuan2023` و`tNianBeast2024`) وجوايزها مش موجودة في الكلاينت. مش هنألّف جوايز.
- العملات الرسمية موجودة في `itemtype` (Fortune 3347118، وDragon Aura 3347117، وWish Pearl 3600694)، بس الشباك الرئيسي مش بيعرض عددها (الجزء ده متعلّق في الكلاينت نفسه).
- `ActBtnConfig[18]` (زرار 1462 جوه قايمة الأنشطة) متعلّق كـ comment في الكلاينت ده، فاستخدمنا زرار الـ lua `DlgFestival2024BTN` الرسمي.

## مشتق (derived)
- أسماء الـ callbacks الستة اللي بنبعتها للشباك (`Head.Func`) اختيار السيرفر، من الأسماء الرسمية اللي في قوايم `Access` في الكلاينت (`SpringFestivalDailyActivities_EnterMap`، `SpingYuan2023_OpenNineDragon`، `NianBeast2024_OpenSL`، `NianBeast2024_GotoBoss`) + `SpringFestival2024Main_GiftClick` و`SpringFestival2024Main_DragonClick`.
- رسالة الليفل ("requires level 80") مشتقة من "Requirement: Level 80" الرسمية.
- السنة في نص الوقت هي السنة الحالية؛ اليوم والشهر رسميين.
