# Tidal Serenade (JellyfishSkyCity): عيد الحب الأبيض 2026، الزرار والشباك الرسمي بس

**الحالة:** `enabled: true`، جدول سنوي من 10 مارس لحد 30 مارس.
- التاريخ رسمي، من `STR_LUAUI_26BSQRJ_RANK_TIME`: "Mar. 10 - 08:59 (US)/16:59 (EU), Mar. 30".
- هيدر الـ lua الصيني بيقول 3.10-3.20، وده بتاع الصين.
- **تنبيه:** شوف التقاطع مع DreamTheater (03-05..03-25) وCrisisSeries (03-04..03-24).

## اللي شغال
- **زرار Fgui 150410 ("Tidal Serenade"):** الضغطة عليه بتبعت `WhiteDay2026Main_ClientOpen` على msg 10871.
- **صفحة Highlight Events (`ActInfo[10871]`):** موجودة في `highlight_acts`، وزرار الدخول بتاعها (10511) بيفتح نفس الشباك.
- **الشباك الرسمي Fgui_26Bsqrj_Cxlg** (الكود في `Base\JellyfishSkyCity\JellyfishSkyCityUi.cs`):
  - بيتبعت عن طريق `InfiniteReceiverData` بـ InterfaceID 1087189.
  - Stellar Fuel (3355898) بيتحسب من الشنطة.
  - النقط = 0.
  - Chapter I مفتوح بـ 0%، وده النص الرسمي TIP_1. باقي الفصول مقفولة.
  - مفيش نقط حمرا.
- **الضغطات:**
  - الفصول بترد برسالة مهذبة.
  - الجزر: لو الـ Stellar Fuel أقل من 100، بيرد بالنص الرسمي "Insufficient Stellar Fuel...".
  - Heart Promise بيشيك على Tear Pearl (3600844).
  - جوايز النقط والرانكنج (`StageSybTemplate_OpenView</N>150411001` على msg 10768) بترد برسالة.
- **رسالة انتهاء الحدث:** رسمية (`tWhiteValentine2026 [35360][Text712]`).
- **اتجرّب live:** الشباك فتح بالـ 6 فصول بأسمائهم الرسمية، وبعدين اتقفل والمهرجان اتطفى.

## المهام والخرايط (لسه مقفولين)
- **المهام** 9640-9703 (64 مهمة): مفيش `quests.json`.
- **الخرايط** 1005622-1005637 (من MistIsle لحد SkyCity) مش متحلولة.
- **الدليل اللي اتدوّر عليه تاني (14-10-2026):**
  - مفيش أسماء ليهم في `GameMapEx.ini`.
  - مفيش block متتالي يركب.
  - مفيش ملف `2026love` أو ما يشبهه في `map\map` بتاع الكلاينت الأصلي، يعني خرايط الحدث مش متشحنة.
  - على الـ DMap اللي مش مربوطة، كل خريطة ركب عليها مرشحين عامين كتير، ماعدا SunkenReef (arenamap وhalloween01)، وده مش دليل قوي.
- **عشان كده مفيش صف في `maps.txt`.**
- **ناقص كمان:** NPC جوه الـ instance، وlookface/atk/def للوحوش.

## اللي مش موجود وليه
- **رحلة Star Sea** (`Fgui_26Bsqrj_Hhmx`، 30 خانة لكل فصل)، وأحداث الجزر، وسحب Heart Promise، وجوايز النقط، والرانكنج: الأحداث والجوايز والاحتمالات مش في الكلاينت.

## مشتق (derived)
- نص المستوى ("You need to reach Level 80 to join Tidal Serenade."). الأصل الرسمي "Level: Lv. 80+".
- رسالة Tear Pearl.
- رسايل "isn`t open on this server yet".
