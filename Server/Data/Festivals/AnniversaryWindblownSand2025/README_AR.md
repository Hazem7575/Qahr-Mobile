# Wind-blown Sand (AnniversaryWindblownSand2025): العيد السنوي الـ 22 لـ CO، الزرار والشباك الرسمي بس

**الحالة:** `enabled: true`، جدول سنوي من 22 أبريل لحد 12 مايو. التاريخ رسمي من `Cn_Res` `STR_LUAUI_ANNIVERSARY_MAIN_TIME` "Apr. 22, 2025 - May 12, 2025".

> **ملحوظة:** زرار العيد السنوي التاني 10631 (Anniversary Invitation، من 13 مايو لحد 9 يونيو) ليه تاريخ مختلف، فاتعمل له فولدر شقيق `AnniversaryJubilee2025`. الاتنين بيستخدموا نفس الكود.

## اللي شغال
- **زرار Fgui 10611** (`lua_2025qqznq_btn5`) في الشريط اللي فوق. الضغطة بتبعت `WindBlownSand2025_OpenMainView` على msg 10611.
- **الشباك الرسمي `Anniversary25Main`** (`Anniversary25Main_OpenData`، الكود في `Base\AnniversaryWindblownSand2025\AnniversaryWindblownSand2025Ui.cs`):
  - بيفتح في Twin City أو Market بس (الرسالة الرسمية `[Sys][NoCity]`).
  - عدد Luminut (3600780) حقيقي من الشنطة، والنقط = 0.
- **رسايل الليفل 80 وانتهاء الحدث:** النص رسمي من `Server_Key` `tWindBlownSand2025_DelText`.

## اللي مش موجود (رسالة سيستم مهذبة، ومفيش أي جايزة)
- **Dune Search، West Journey (الـ 6 مراحل)، Anniversary Blessing، والمحل.**
- **الرانكنج والسحب والبركة** بيروحوا على msg تانية (DlgValentinesDay، 10510، DlgBless)، فسايبينهم لأصحابهم.

## مشتق (derived)
- حالة المراحل `{1,0,0,0,0,0,0}`، يعني Dune مفتوحة والباقي رمادي.
