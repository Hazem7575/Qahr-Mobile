# Fool's Paradise (AprilFools2025): كدبة أبريل 2025، الزرار والشباك الرسمي بس

**الحالة:** `enabled: true`، جدول سنوي من 1 أبريل لحد 14 أبريل. ده التاريخ الرسمي من الكلاينت (`Cn_Res` `STR_FUI_25FOOLSDAY_TIME` "Apr. 1 - Apr. 14").

> **ملحوظة:** AliceRescue هو كدبة أبريل 2024، حدث تاني، ومتلمسش. لو الاتنين شغالين في نفس الوقت هيظهر زرارين.

## اللي شغال
- **زرار Fgui 150260 "Merry Fool Detective":** بيظهر في الشريط اللي فوق طول ما المهرجان شغال.
- **الشباك الرسمي `FoolsDayMain`** (`Fui_FoolsDayMain_OpenData`، msg 10568، الكود في `Base\AprilFools2025\AprilFools2025Ui.cs`):
  - بيفتح في Twin City أو Market بس (الرسالة الرسمية `[Sys][NoCity]`).
  - عدد Fate Coin (3600774) حقيقي من الشنطة.
  - Detective Points = 0 (مفيش حاجة بتدّي نقط).
- **زرار "+" بتاع العملة:** بيفتح قايمة طرق الحصول الرسمية (`Fui_FoolsDayLottery_PropAccess`).
- **رسايل الليفل وانتهاء الحدث:** النص رسمي من `Server_Key` `tAprilFools2025MainPage_DelText`.

## اللي مش موجود وليه (رسالة سيستم مهذبة، ومفيش أي جايزة)
- **Go to Fool's Paradise:** القصة (`tPlotEditor`) مالهاش maps ولا NPCs عندنا.
- **Masked Jester / Pleasure Vault (Cross-server):** محتاج سيرفرات متعددة.
- **Wonder Box:** جدول الجوايز في السيرفر الرسمي بس. الطلبات بتيجي على msg 10510 (`DrawClient_*</N>10568`). سطر الـ hook بتاعنا قبل سطر VillageRequests، وبياخد act 10568 بس.
- **الرانكنج، وJoyful Gift Shop.**

## مشتق (derived)
- تقدم القصة "Quest (0/8)". الـ 8 هي عدد صور القصة في الشباك، والعدد الرسمي في السيرفر.
