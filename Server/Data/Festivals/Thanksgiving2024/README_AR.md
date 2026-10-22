# Thanksgiving Feast (Thanksgiving2024): الزرار والشباك الرسمي بس

**الحالة:** `enabled: true`، جدول سنوي من 19 نوفمبر لحد 2 ديسمبر. ده التاريخ الرسمي من `Cn_Res` `STR_FUI_24GANEN_TIME` ("Nov. 19, 2024 - Dec. 2, 2024").

## اللي شغال
- **زرار Fgui 150184 "Thanksgiving Feast":** بيظهر في الشريط اللي فوق وبيختفي لما المهرجان يقفل (اتجرب لايف).
- **الشباك الرسمي `Fui24GanenMain`** (msg 10486، الكود في `Base\Thanksgiving2024\Thanksgiving2024Ui.cs`):
  - فيه 3 أجزاء: Finale Feast وTurkey Refuge وWish of Fertility.
  - Points بتبان 0، لأن مفيش حاجة بتدّي Fertility Points.
  - Stage = -1، فبيظهر "The feast has ended" تحت Finale Feast، لأن القصة مش موجودة على السيرفر.
  - أي جزء بتدوس عليه بيرد برسالة مؤدبة.
  - `ThanksGiving2024View_Rule` بيفتح صفحة القواعد الرسمية `RuleText["10486-1"]`.
- **رسالة انتهاء الحدث:** نص رسمي من `tThanksGiving2024View_DelText[105][1]`.

## اللي مش موجود وليه
- قصة Finale Feast، وTurkey Refuge، وWish of Fertility (عبر السيرفرات)، والرانكنج، والمحلات. دي كلها سكربتات سيرفر رسمية مش متاحة لينا.
- مفيش مهام في `Questinfo`، ومفيش خرايط في `maps.txt`.

## مشتق (derived)
- أقصى قيمة لشريط النقط 120000. ده آخر مستوى في `pointsrankdata` `CONST_POINTS_AWARD_DATA[10486]`.
- Stage = -1.
- رسالة الليفل ورسايل "مش مفتوح".
- الإعدادات: `Infinity[DB]\Thanksgiving2024\thanksgiving2024.json`.
