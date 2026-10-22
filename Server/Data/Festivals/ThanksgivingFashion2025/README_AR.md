# Top Master Campaign (ThanksgivingFashion2025): عيد الشكر 2025 "Fashion Star"، الزرار والشباك الرسمي بس

**الحالة:** `enabled: true`، جدول سنوي من 25 نوفمبر لحد 15 ديسمبر. ده التاريخ الرسمي من الكلاينت (`Cn_Res` `STR_LUAUI_DARENXIU_TIME_TXT` "Nov. 25 - Dec. 15, 2025"). الأحداث الفرعية مفتوحة من 25 نوفمبر لحد 8 ديسمبر.

> **ملحوظة:** Thanksgiving2024 (19 نوفمبر لحد 2 ديسمبر) وThanksgivingTribe2023 سنين تانية. لو الاتنين شغالين في نفس الوقت هيظهر زرارين.

## اللي شغال
- **زرار Fgui 150361 "Top Master":** بيظهر في الشريط اللي فوق طول ما المهرجان شغال.
- **الشباك الرسمي `TalentShow`** (`Fui_TalentShow_Open`، msg 10789 `tMsg_Info["DaRenXiu"]`، الكود في `Base\ThanksgivingFashion2025\ThanksgivingFashion2025Ui.cs`):
  - صفحة Main Hall (`com_rk`).
  - عدد Skyreach Stone (3600820) حقيقي من الشنطة.
  - Skyreach Points = 0.
  - الأماكن الستة بتعرض وقت الفتح الرسمي "Open Time: Nov. 25 - Dec. 8".

## اللي مش موجود وليه (رسالة سيستم مهذبة، ومفيش أي جايزة)
- **Splendor Pavilion وPinnacle Tower وLand of Fortune:** جداول الجوايز في السيرفر الرسمي بس.
- **Skyreach Hunt:** خريطة Skyreach Realm مش موجودة في `maps.txt`.
- **الرانكنج وArena (تصويت Top Master):** محتاجين سيرفرات متعددة.
- **Skyreach Vault (المتجر).**

## مشتق (derived)
- الحد الأقصى لشريط النقط (`pointsMax`، القيمة الافتراضية 1، يعني الشريط فاضي). جدول المراحل الرسمي مش موجود في الكلاينت.
- الـ sybid المستخدم لعرض Skyreach Stone هو 308001001 من `luauirewardshow`.
