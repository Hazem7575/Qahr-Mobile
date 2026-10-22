# عيد تشيشي 2024 "Interplane" (QixiInterplane2024): الزرار والشباك الرسمي بس

**الحالة:** `enabled: true`، جدول سنوي من 6 أغسطس لحد 26 أغسطس. ده التاريخ الرسمي من الكلاينت (`Cn_Res` `STR_FUI_24QIXI_MAIN_TIME` = "Aug. 6 - Aug. 26").

## اللي شغال
- **زرار Fgui 150111** في الشريط اللي فوق (`fguibtnconfig`، الصورة `luahdshouna_btn4`، مكتوب عليه "Romance Planet")، بيظهر طول ما المهرجان شغال.
- **الشباك الرسمي `Fui24QixiTravel`** (عنوانه "Milky Way") (msg 10448، الكود في `Base\QixiInterplane2024\QixiInterplane2024Ui.cs`):
  - التلات مجرّات (Romance / Soulmate / Eternity Starfield) بتبان "Not open yet." لأن الرحلة مش موجودة على السيرفر.
  - النقط (Starry Points) بتبان 0، لأن مفيش حاجة بتدّي نقط.
  - زرار القواعد (?) بيفتح نص القواعد الرسمي من الكلاينت (`luaUiRes[10448].Rule`).
  - لو دوست على مجرة أو السيف (Excalibur) أو الرانكنج بتطلعلك رسالة مؤدبة إنه مش مفتوح لسه.
- **رسايل الليفل (80) وانتهاء الحدث:** نصها رسمي من `Server_Key` (`tQixi2024Part5[Msg][Nolevel]` و`tQixiMainNpc2024[33197][Text122]`).

## اللي مش موجود وليه
- مفيش مهام في `Questinfo.ini` ومفيش خرايط في `maps.txt`.
- رحلة المجرات (الرحلة الثنائية والفردية، الأحداث الرومانسية، البوصلة)، وتشكيل السيف، والنقط والرانكنج: كلها سكربتات سيرفر رسمية (`tInterplanePlay` و`tQixi2024Part5` و`tQixiMainNpc2024`) وجوايزها مش موجودة في الكلاينت. مش هنألّف جوايز.
- الـ NPC الرسمي Sword Deity (33197) وخريطة Vault Palace مش موجودين على السيرفر.

## مشتق (derived)
- `Effect=1` (نور السيف بتاع أول مرحلة) في بيانات فتح الشباك، لأن محدش بيشكّل السيف هنا. ممكن يتغيّر من `Infinity[DB]\QixiInterplane2024\qixiinterplane2024.json` (`swordEffect` 1-3، اختياري).
