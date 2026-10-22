# عيد ميلاد كونكر 2024 "Anniversary Cake Gala" (AnniversaryOtherworld2024): الزرار والشباك الرسمي

**الحالة:** `enabled: true`، جدول سنوي من 23 أبريل لحد 13 مايو. التاريخ رسمي من الكلاينت (`luaUiRes_EN[1482].main.ActTime` = "Apr. 23 - May 13"، وأدوات الحدث بتنتهي 13 مايو 2024).

## اللي شغال
- **زرار lua `Dlg24AnniversaryBtn`** (Dialog_Info `1482-btn`)، بيظهر بنداء السيرفر الرسمي `Dlg24Anniversary_ShowBtn` وبيستخبى بـ `LuaUIMgr_HideDialog`.
- **الشباك الرسمي `Dlg24AnniversaryMainUi`** (msg 10389، الكود في `Base\AnniversaryOtherworld2024\AnniversaryOtherworld2024Ui.cs`):
  - عدد الـ Super Gourd (3600703) الحقيقي من الشنطة.
  - نقط الحفلة (Gala Points) = 0، لأن مفيش حاجة على السيرفر بتديها.
  - الـ 7 أماكن (Gala Gourmet، Verdant Forest، Bakery، Lounge، Coastal Area، Candy Valley (CS)، Sea Ruins): لو دوست على أي واحد بتطلعلك رسالة مؤدبة إنه مش مفتوح لسه.
  - الكيك، الرانكنج، المتجر، التبديل، Vault Palace، معسكر الوحوش: رسالة مؤدبة، ومفيش أي جايزة بتتدي.
- **رسالة انتهاء الحدث** رسمية: `tAnniversary2024Npc[36841][Text122]`.

## اللي مش موجود وليه
- مفيش مهام في `Questinfo.ini` ومفيش خرايط في `maps.txt`.
- مسك الوحوش وتربيتها والمخبز ونقط الحفلة كلها سكربتات سيرفر رسمية (`tAnniversary2024Npc` و`tCultivateMonsterCamp`)، وجداولها مش موجودة في الكلاينت. مش هنألّف جوايز.
- الـ NPCs الرسمية (Maisie 36841، Baking Master 36842، Theodore 36846) وخرايطهم مش موجودين على السيرفر.

## مشتق (derived)
- كل الأماكن السبعة بتتبعت `State=2` (مفتوحة) عشان كل ضغطة يبقى ليها رد. حالة البداية الرسمية مش موجودة في الكلاينت.
- رسالة الليفل ("Please come back after reaching level 80.") مكتوبة بإيدينا، لكن شرط لفل 80 نفسه رسمي.
