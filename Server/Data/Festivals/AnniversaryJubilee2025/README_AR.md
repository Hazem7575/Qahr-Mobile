# Anniversary Invitation (AnniversaryJubilee2025): احتفال العيد السنوي 2025 (النسخة الإنجليزي)

**الحالة:** `enabled: true`، جدول سنوي من 13 مايو لحد 9 يونيو. التاريخ رسمي من `Cn_Res` `STR_LUAUI_25JUBILEEMAIN_TIME` (ERP627502 【英文征服】).
- **Celebration Stage:** من 13 لحد 26 مايو.
- **Carnival Stage:** من 27 مايو لحد 9 يونيو.

الكود بتاعه في `Base\AnniversaryWindblownSand2025\AnniversaryWindblownSand2025Ui.cs`، وهو شقيق `AnniversaryWindblownSand2025`.

## اللي شغال
- **زرار Fgui 10631** (`lua2025znqtitle_btn`). الضغطة بتبعت `AnniversaryCarnival_CelebrationAttend` على msg 10631.
- **الشباك الرسمي `JubileeMain`** (`Fui_JubileeMain_OpenData`):
  - أعداد Wish Butterfly (3352961) وCelebration Medal (3352962) وAnniversary Point (3353247) وAnniv Coin (3353040) حقيقية من الشنطة. الأيقونات فاضية لأن الحاجات دي مالهاش sybid في `luauirewardshow` بتاع الكلاينت.
  - المرحلة 1 أو 2 بتتحسب من التاريخ الرسمي.
- **Anniversary Guide:** بيفتح شباك `JubileeRule` الرسمي بالنص الرسمي.
- **رسايل الليفل 80 وانتهاء الحدث:** النص رسمي من `tAnniversaryCarnival_DelText`.

## اللي مش موجود (رسالة سيستم مهذبة، ومفيش أي جايزة)
- **الأنشطة:** Recall، Rivalry، Wish، Silver Carnival.
- **باقي الأزرار:** Sign-in، Token، Promotion.
- **المحل، والتبديل، ومكافآت النقط.**
- **الرانكنج:** بيروح على msg 10139 بتاعة CastleMystery.

## مشتق (derived)
- حالة الأنشطة الأربعة = 0 (مش مفتوحة).
