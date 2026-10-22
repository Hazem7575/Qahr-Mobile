# مسرح الأحلام: العقود اليومية (اتدمجت في DreamTheater)

**الحالة:** الفولدر ده فاضي ومقفول على طول (`enabled: false`، ومن غير icon). المحتوى كله شغال جوه `Festivals\DreamTheater`.

## ليه اتدمج
- في الرسمي ده **نفس الحدث**: White Day 2024 "Dream Theater"، وسكربتات السيرفر `MasterAndTheatre2024` و`tWhiteDay2024NPC`.
- ليه زرار واحد بس (`DlgWhiteDayBtn`)، وشباك واحد (`DlgWhiteDayMain`).
- المهام دي هي **عقود Agnes اليومية** جوه الشباك ده:
  - `TaskIndex = 8`: التنضيف (5752، و5796-5801). نص الشباك `luaUiRes[1466].main.TaskCenter[13]`.
  - `TaskIndex = 9`: مشاكل الممثلين (5753-5761). النص `TaskCenter[14]`.
  - المصدر: `tNpcShowTaskInfo` في `luaui\dialog\dlgwhiteday\dlgwhitedaymain`.
- عشان كده مفيش icon تاني هنا. لو اتعمل show/hide تاني لنفس الزرار هيتخانق مع DreamTheater.

## فين المحتوى
| الحاجة | المكان |
|---|---|
| المهام 16 (pools `DTClean` و`DTTrouble`، يومي، Wish Silver Coin) | `DreamTheater\quests.json` |
| الممثلين، وأماكن التنضيف، والركنين بتوع النضارة | `DreamTheater\npcs.txt`، على 12430 دلوقتي، و12431 الرسمي بعد الـ deploy (تحت) |
| "Cleaning..."، و"Searching..."، ومبارزة Jenkins، والشباك | `Base\DreamTheater\DreamTheaterModule.cs` و`DreamTheaterLua.cs` |

## الشباك بيعرض إيه حقيقي
- **Agnes في الشباك:**
  - بتعرض عقد التنضيف (8) لو مفيش واحد اتاخد أو خلص النهارده.
  - بعده عقد مشكلة ممثل (9).
  - بعدهم مفيش عقد (0).
  - الاستلام بيمشي من `MasterAndTheatre2024_SolveContracts` على QuestEngine.
- **نقطة "Enter Theater" الحمرا:** بتنور لما عقدين النهارده يخلصوا والعرض لسه متعملش.
- **عدد العملات:** عملات الفضة والدهب بتتعد من الشنطة.

## مستني الـ deploy: ماب المسرح الرسمي 12431
- دلوقتي المسرح على **12430**، وده الـ doc بتاعه.
- Questinfo بيحط الممثلين على **12431**، وعشان كده بيطلع 7 تحذيرات "not at its Questinfo.ini spot".
- اتجرّب التحويل لـ 12431 وطلع `map 12431 does not exist`، فاتعمله revert على طول.
  - السبب: الـ dll الشغال (19:54) مافيهوش `Base\FestivalMaps\FestivalMapTable.cs` (اتعمل 20:36).
- **بعد الـ deploy:**
  - شغّل `Backups_Systems\20261014_205740_TheaterAftermath\apply_12431_after_deploy.py` (عليه md5 guard).
  - وبعدين `reloadfestivals`.
- **حاجة derived:** في الرسمي التنضيف والنضارة في instance رقمها 1002561. عندنا هما جوه نفس النسخة الخاصة (نفس الـ doc ونفس الإحداثيات).

_backup: `Backups_Systems\20261014_205740_TheaterAftermath\`._
