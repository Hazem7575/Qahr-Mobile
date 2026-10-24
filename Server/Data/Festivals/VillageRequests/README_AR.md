# الكريسماس العالمي 2024: طلبات أهل Doomwoods: شغّال (جزئيًا)

**الحالة:** `enabled: true`.
- **الجدول:** `yearly` من 12-17 لـ 01-06. دي المواعيد الرسمية (هيدر ملفات الكلاينت: "全球圣诞活动 12.17-1.6").
- **تشغيل يدوي:** `@festival on VillageRequests` (وبعدها `@festival auto VillageRequests` يرجّع الجدول).

## إزاي يتلعب (لفل 80+)
- **الزرار:** Global Christmas فوق (Fgui `10510` الرسمي). بيفتح الشباك الرسمي ChristmasMain، وفيه 3 حاجات:
  - **البوابة (Doom Portal):** مفتوحة.
  - **الـ Converter وشجرة الكريسماس:** مقفولين.
- **البوابة:** شغّالة من توين سيتي بس، زي النص الرسمي "Please return to Twin City (Market)".
  - بتشغّل تأثير البوابة الرسمي، وبعدها بتدخّلك **نسخة خاصة بيك** من Doomwoods.
  - النسخة: instance `Doomwoods`، خريطة 1003831 = doc 12696، مدتها 60 دقيقة.
  - بتوصل قدام Lillian، زي النص الرسمي: "Go ask the person ahead".
- **جوه Doomwoods:** الـ 14 مواطن الرسميين `[WoodsCitizen]` واقفين في نقطهم الرسمية. Lillian عندها "Back to Twin City".

| المهمة | المطلوب | الحالة |
|---|---|---|
| 8247 Missing Necklace | Lillian ← SunflowerCluster ×3 ← Lillian | شغّالة (يومية) |
| 8254 Tasty Fruit | Jean ← TastyFruit ← Jean | شغّالة (يومية) |
| 8255 Fresh Vegetable | Scott ← FreshVegetable ← Scott | شغّالة (يومية) |
| 8248، 8250، 8252، 8256، 8257، 8258، 8259، 8260 | قتل وحوش | مقفولة |
| 8249، 8251، 8253 | Photo / Rebee`sRelic / Lost&Found | مقفولة |

- **الجايزة:** Wishflake Blessing Pack `3351462` ×1 (رسمي من Questinfo).
- **يومية:** النص الرسمي بيقول "Every day, citizens will come here to make wishes".

## رسمي ولا مشتق
- **رسمي:**
  - المهام والنصوص والنقط والجوايز (Questinfo).
  - الـ meshes: كل المواطنين `[WoodsCitizen]` في npc.ini (Lillian 1317، Jeana 1319، Thompson 1320، Scott 1889، Mia 3325، Seymour 3326، Ramon 3327، David 4220، Kura 4221، Jean 4672، Leo 4915، Kallir 4916، Zach 4917، Lucy 4918).
  - الشباك ونداءاته، ورسايل البوابة (`tChristmasDraw2024_DelText`)، والمواعيد، وشرط اللفل 80.
- **الخريطة:** 1003831 Doomwoods = 12696 `2024xmas_new` (maps.txt، ثقة B). كل النقط walkable، ما عدا TastyFruit (96,131): ده object على خلية مقفولة، وجنبه خليتين بيتمشي عليهم.
- **مشتق:**
  - مكان الوصول (181,46)، ومدة النسخة 60 دقيقة، ومكان الخروج، ودخول Market 1036، ومهلة البوابة. الإعدادات في `Data\VillageRequests\villagerequests.json`، والـ instance في `quests.json`.
  - **SunflowerCluster:** شكل Sunflower (npc.ini 1318، الـ id اللي بعد Lillian على طول). PROXY.
  - **TastyFruit 1888 / FreshVegetable 4219:** بالاسم، وأرقامهم جنب مواطنين Doomwoods.
  - **الـ greeting:** هو نص الطلب الرسمي (IntentionDesp).

## مش موجود (ليه)
- **الوحوش:** SnowFowl وWickedSpirit وCooChicken وFuriousGhost ليهم لفل وحياة بس في `monster.dat`، من غير lookface ولا هجوم. وRebee وFearedGhost مالهمش صف خالص.
- **Photo وRebee`sRelic وLost&Found:** مالهمش mesh في أي كلاينت.
- **الـ Material Converter وأمنيات/سحب شجرة الكريسماس ونقط الكريسماس والترتيب:** رسالة بس، ومفيش جوايز.
- **Wishflake Blessing Pack:** مالوش script فتح على السيرفر لسه. النص الرسمي: "Opening it for free will grant you 100 Chi Points/1 Wishflake. Paying 54 CPs ... random 10-50 Wishflakes". محتاج قرار من الأونر.

## الكود
- `Base\VillageRequests\VillageRequestsUi.cs`.
- Hook في `LuaPush.HandleCallback` (msg 10510).
- **الدخول للنسخة:** الـ QuestEngine بيدخّل الـ instances من NPC teleport أو quest action بس. عشان كده البوابة بتنادي `EnterInstance` بالـ reflection، من غير ما نعدّل `QuestEngine.cs`. لو حد غيّر اسمها أو الـ signature بتاعها، البوابة هتطلّع "Doomwoods isn`t open yet" بس.

_اتعمل 14-10-2026. المصادر: `client - orignal\ini\Questinfo.ini`، و`npc.ini`، و`monster.dat`، وluaui `christmasmain` / `christmas24` / `christmas24func` / `fguibtnconfig`، و`Server_Key` (tChristmasDraw2024 / tChrismas2024Main / tChristmas2024Wish)، و`FestivalMaps\maps.txt`._
