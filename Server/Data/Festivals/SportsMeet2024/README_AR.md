# دورة ألعاب كونكر الرياضية 2024 (SportsMeet2024): الزرار والشباك الرسمي

**ده مهرجان موسمي مش نشاط دايم:** في الكلاينت مكتوب ERP593055 "حدث أغسطس - CO Sports Games (8.06-8.19)"، وكل أدواته بتنتهي يوم 19 أو 22 أغسطس 2024.
**الحالة:** `enabled: true`، جدول سنوي من 6 أغسطس لحد 19 أغسطس (`Cn_Res` `STR_FUI_SPORTS_TIME` و`tCoSportNpc2024[33251][Text114]`).

## اللي شغال
- **زرار Fgui 150124** في الشريط اللي فوق (`fguibtnconfig`، الصورة `luahdshouna_btn5`).
- **الشباك الرسمي `FuiSportsMain` "CO Sports"** (msg 10453، الكود في `Base\SportsMeet2024\SportsMeet2024Ui.cs`):
  - العملات من الشنطة بأعداد حقيقية: Entry Ticket (3349636) وMedal of Honor (3349640).
  - الـ 4 بافات من الشنطة بأعداد حقيقية: Precision Glasses / Pocket Watch / Climbing Claw / Underwater Accelerator (3349676-3349679).
  - نقط الرياضة = 0، والمرحلة الجاية 500.
- **صفحة جوايز المراحل** (PointsRank 10453): الـ 13 مرحلة وجوايزها من الكلاينت، وكلها "مش متاحة" لأن النقط 0.
- السباقات الأربعة (Horse Racing، Precision Shooting، Synchronized Swimming، Sky Climbing)، والرانكنج، وClan War، والـ Sports Park اللي بين السيرفرات، والـ battle pass، والمتجر، واليانصيب: كلها بتطلع رسالة مؤدبة، ومفيش أي جايزة بتتدي.
- رسايل انتهاء الحدث والليفل والدخول بين السيرفرات رسمية من `Server_Key`.

## اللي مش موجود وليه
- مفيش مهام في `Questinfo.ini` ومفيش خرايط في `maps.txt`.
- السباقات واليانصيب والرانكنج كلها سكربتات سيرفر رسمية (`tCoSportShoot` و`tAugustSportRank` و`tSportsMeetKua2024` و`tCOSportClimbingDraw` و`tCoSportNpc2024`). نسب اليانصيب مش موجودة في الكلاينت، فمش هنألّفها.

## مشتق (derived)
- العملتين اللي فوق على اليمين اخترناهم Entry Ticket وMedal of Honor، لأن قواعد الحدث بتقول إنهم العملات الأساسية. الكلاينت مش بيحدد ترتيبهم.
- ترتيب البافات الأربعة ماشي على ترتيب Athletics Buff Box (3349701).
