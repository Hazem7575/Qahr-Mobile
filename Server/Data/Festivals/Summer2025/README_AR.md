# Sylvan Whisper (Summer2025): صيف 2025، الزرار والشباك الرسمي بس

**الحالة:** `enabled: true`، جدول سنوي من 28 يوليو لحد 10 أغسطس. ده التاريخ الرسمي للنسخة الإنجليزي (`Cn_Res` `STR_LUAUI_SUMMERVACATION_TIME` "July 28 - Aug. 10"). النسخة الصيني كانت 7.8-7.21.

## اللي شغال
- **زرار Fgui 150306** (`lua_2025qqsq_btn2`) في الشريط اللي فوق طول ما المهرجان شغال. الضغطة بتبعت `SummerDraw2025_OpenMainView` على msg 10707.
- **صفحة Highlight Events رقم 10707** ("Volley Garden") وزرار Enter بتاعها بيفتح نفس الشباك.
- **الشباك الرسمي `SummerVacation`** (`SummerVacation25_OpenData`، الكود في `Base\Summer2025\Summer2025Ui.cs`):
  - عدد Midsummer Seed (3354231) حقيقي من الشنطة.
  - مفيش ورد مزروع، والنقط = 0.
  - أيقونات الـ 4 أنواع ورد من `luauirewardshow` الرسمي (225001000..228001000).
- **Reward Preview:** بيفتح شباك `Summer25` الرسمي بجوايز الورد Lv1-3 الرسمية (sybids من `luauirewardshow` [225]..[230]).
- **Targeted Planting:** الاختيار بيترجع للشباك (UI بس، مفيش جايزة).
- **رسايل الليفل وانتهاء الحدث:** النص رسمي من `Server_Key` `tSummer2025Npc`.

## اللي مش موجود وليه (رسالة سيستم مهذبة، ومفيش أي جايزة)
- **الزراعة والحصاد:** البذور 3354230 و3354231 مش موجودة في itemtype بتاع الكلاينت ولا السيرفر، فمحدش يقدر يمتلكها. الرد: "You lack sufficient Midsummer Seeds."
- **Flickerwood، Woodland Watch، Undying Blight:** مالهمش maps ولا NPCs عندنا.
- **الرانكنج والمحلات.**

## ملحوظة على 150437
زرار Fgui 150437 (`lua_yfyh_btn7`) بيبعت برضه على 10707، بس الفانكشن بتاعته `SummerActive2023Page_OpenMainInterface` بتفتح شباك Dream Cruise بتاع Summer2023 (Set Sail 2026). مش تبع الحدث ده، فمش بنظهره هنا، والكود بتاعنا بيسيبه لصاحبه.

## مشتق (derived) في `Data\Summer2025\summer2025.json`
- `sowCost` = 1 و`quickCost` = 5: عدد البذور للزرعة. مأخوذ من بيانات الاختبار في الكلاينت.
- `woodlandWatchMax` = 3: عداد "Woodland Watch (0/3)".
- اختيار أيقونات Lv1 لأنواع الورد في Data[8].
