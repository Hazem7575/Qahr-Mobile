# Steed Legend (SpringFestival2026): مهرجان الربيع 2026، الزرار والشباك الرسمي بس

**الحالة:** `enabled: true`، جدول سنوي من 10 فبراير لحد 2 مارس.
- التاريخ ده رسمي، من نص الكلاينت `STR_FUI_ACTTIME_10850`: "Feb. 10 - 07:59 (US)/15:59 (EU), Mar. 2".
- الشباك نفسه بيكتب "Feb. 10 - 23:59, Feb. 28"، وده وقت جزء Steed Feeding بس.

## اللي شغال
- **زرار Fgui 150401:** بيظهر في الشريط اللي فوق، والضغطة عليه بتبعت `SpringFestivalX2025_OpenView` على msg 10850.
- **الشباك الرسمي `Fgui_26ChunJie_Qrcs`** (الكود في `Base\SpringFestival2026\SpringFestival2026Ui.cs`):
  - بيعرض مدة الحدث الرسمية.
  - Steed Points = 0، وBanishing Progress = 0/0 (السيرفر والشخصي)، ومفيش نقط حمرا. دي بيانات حقيقية، لأن مفيش حاجة على السيرفر بتدّي نقط أو تقدّم.
- **الخرايط الأربعة** (Cherry Inkstone Pool، وPetal Vale، وDecay Beast Lair، وGale Fortress): كل واحدة بترد برسالة إنها مش مفتوحة.
  - Decay Beast Lair بيشيك على عدد Purge Pearl (3355569) الحقيقي في الشنطة. لو مفيش، بيرد بالنص الرسمي: "You do not possess a Purge Pearl...".
- **صفحة Highlight Events رقم 10850 ("Steed Legend"):** موجودة في `actinfo` الرسمي، وزرار الدخول فيها بيفتح الشباك.
- **رسالة انتهاء الحدث ومستوى 80:** من `tSpringFestivalX2025[35242]`.

## اللي مش موجود وليه
- **مهام Petal Vale وGale Fortress اليومية (Cleansing Purge):** مالهاش quests في Questinfo، ومالهاش خرايط في `maps.txt`. الـ NPCs (Bloom Steed 35189، وEbony Steed 35213) سكريبت سيرفر رسمي مش موجود.
- **Decay Beast Lair:** لعبة الشبكة دي، الخريطة والحزم والجوايز بتاعتها بيانات سيرفر.
- **Steed Feeding (`SpringFestivalDraw`):** الاحتمالات الرسمية مكتوبة في الكلاينت (4%/10%/10%)، لكن جدول الكروت والجوايز نفسه مش موجود. عشان كده مش متعمل، ومفيش جوايز متألفة.
- **Steed's Gift، والرانكنج، وSteed Store، وجوايز صناديق التقدم الستة:** بيانات سيرفر. الضغطة عليهم بترد برسالة مهذبة.
- **الخانة `[2]` في الشباك (العملة اللي جنبها "+"):** مش بتتبعت، لأن الكلاينت مش بيقول هي أنهي item، والرقم بتاعها بيتحط كـ index لـ controller، فرقم غلط ممكن يبوّظ الشباك.

## مشتق (derived)
- نص رسالة المستوى ("You need to reach Level 80 to join Steed Legend.") ورسايل "isn`t open on this server yet".
- الجدول سنوي (02-10 لحد 03-02). الحدث الرسمي كان سنة 2026 بس، والسنة الصينية بتتغير كل سنة.
