# Love Cycles (Qixi2025): عيد تشيشي 2025، الزرار والشباك الرسمي بس

**الحالة:** `enabled: true`، جدول سنوي من 26 أغسطس لحد 15 سبتمبر.
- التاريخ ده رسمي، من `STR_FGUI_25QIXI_SSYC_TIME`: "Aug. 26 - 08:59 (US)/16:59 (EU), Sept. 15".
- **تنبيه:** الجدول بيتقاطع مع SwordSaga (Qixi 2026، من 08-11 لحد 08-31) في الأيام من 26 لحد 31 أغسطس. لو عايز Qixi واحد بس زي الرسمي، خلّي واحد فيهم manual.

## اللي شغال
- **زرار Fgui 150313 ("Love Cycles"):** الضغطة عليه بتبعت `QixiMainNpc2025_OpenView` على msg 10726.
- **الشباك الرسمي `Fgui_25Qixi_Xjyy`** (الكود في `Base\Qixi2025\Qixi2025Ui.cs`):
  - Worldly Points = 0، ومفيش نقط حمرا، والعالم الحالي "Worldly Test". دي بيانات حقيقية، لأن مفيش حاجة بتدّي نقط أو وحوش.
- **المداخل التلاتة:**
  - Worldly Test: رسالة إنه مش مفتوح.
  - Rebuild Bridge: بيشيك على Starway Sand (3353900) الحقيقي في الشنطة. لو مفيش، بيرد بالنص الرسمي "Insufficient Starway Sands...".
  - Fate Beast: رسالة إنه مش مفتوح.
- **رسالة انتهاء الحدث:** رسمية (`tQixiMainNpc2025[34490][Text122]`).

## اللي مش موجود وليه
- **Worldly Dream (7 عوالم بمهامها):** instances سكريبت سيرفر، مالهاش quests في Questinfo ومالهاش خرايط في `maps.txt`.
- **تربية الوحوش وتناسخها والـ Handbook، وسحب Magpie Bridge، والرانكنج، وWorldly Store، وجوايز المراحل:** جداول الاحتمالات والجوايز مش موجودة في الكلاينت، عشان كده بترد برسالة مهذبة.
- **مفيش صفحة Highlight:** مفيش `ActInfo[10726]` في الكلاينت.

## مشتق (derived)
- نص رسالة المستوى ("You need to reach Level 80 to join Love Cycles.") ورسايل "isn`t open on this server yet".
- الاسم الإنجليزي "Love Cycles" مأخوذ من صورة الزرار ومن فئة الأيتمز `Love Cycles` في itemtype.
