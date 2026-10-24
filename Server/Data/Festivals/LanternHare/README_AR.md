# الأرنب الضايع والفوانيس (Lost Hare & Lanterns): مقفول

**الحالة:** `enabled: false`. الفولدر فيه `festival.json` بس، ومفيش NPCs ولا وحوش ولا `quests.json`، عشان مانعملش محتوى من مصادر مش حقيقية.

## المهرجان ده إيه
6 مهام (TypeId 8) على 3 خرايط صغيرة للجنيّات:
- 4927 / 4929 / 4931: تساعد Lilac وBeryl وGinger يلاقوا LittleHare وLantern وLittle Fox.
- 4928 / 4930 / 4932: تطرد GeneralFlame اللي بيضايق الجنيّات.
- الجايزة: "Lilac's Blessing" و"Beryl's Blessing" و"Ginger's Blessing". دي نص بس (buff)، **مفيش item id**.

## ليه مقفول (اللي ناقص بالظبط)
- **الخرايط 11441 و11442 و11443** مش موجودة في `GameMap.dat` بتاع الكلاينت (لا client_test ولا الكلاينت الأصلي)، فاللاعب مش هيقدر يحمّلها.
  - دوّرت على doc بديل: جرّبت كل نقط الكويستات على كل DMap عندنا (radius 0). 11442 و11443 مفيش ولا خريطة أحداث بتطابقهم، غير الخرايط الضخمة العامة (texaswait وlineup) اللي بتطابق أي حاجة. يعني مفيش doc مؤكد.
- **الوحوش** LittleHare وLantern وLittle Fox وGeneralFlame مالهمش صف في `client_test\ini\monster.dat` (اتفك بالـ 3-Way)، ولا في `Monsters\` بتاع السيرفر، ولا في Hazem6609 أو zf.sql. يعني مفيش شكل ولا ستاتس.
- **الـ NPC 35871** (LostLittleHare / Lilac / Beryl / Ginger) مالوش mesh بالاسم في `npc.ini` ولا `NpcX.ini`.
- **الجايزة** buff بالنص بس. السيرفر مفيهوش نظام "Blessing" نديه، وماينفعش نخترع item مكانه.

## عشان يشتغل محتاج إيه
1. الـ doc الحقيقي للخرايط الـ 3 (capture من الرسمي)، أو قرار من الأونر بخريطة بديلة.
2. lookface وستاتس الوحوش الأربعة من مصدر رسمي.
3. mesh الـ NPC 35871.
4. قرار في الجايزة: item حقيقي بدل الـ Blessing.

_Agent 4 (13-10-2026). المصادر: `Data\Questinfo.ini`، و`client_test\ini\monster.dat` و`npc.ini` و`NpcX.ini` و`GameMap.dat`، و`tools\QuestAudit\maps_added.json`._
