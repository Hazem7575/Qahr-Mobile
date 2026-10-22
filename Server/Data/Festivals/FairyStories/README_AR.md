# حكايات الجنيّات (Lily / Rose / Orchid): مقفول

**الحالة:** `enabled: false`. فيه `festival.json` بس.

## المهرجان ده إيه
6 مهام مخفية (`HideInTaskList=1`)، كل جنيّة ليها instance:
- **Lily (1001852):**
  - 5007: تكلّم Lily وتقتل 30 MirageSprite.
  - 5008: تكلّم GriffiniaFairy وتقتل InfernoSprite.
- **Rose (1001853):**
  - 5010: تبعت شهب (Meteors) لـ RoseFairy.
  - 5011: تبعت ورد لـ RoseFairy.
- **Orchid (1001854):**
  - 5013: تمسح ToxicSprite.
  - 5014: تمسح HugeToxicSprite.
- **الجايزة:** فاضية في `Questinfo.ini` لكل الست مهام.

## ليه مقفول (اللي ناقص بالظبط)
- **الخرايط:** 1001852-1001854 instances رسمية من غير doc في الكلاينت.
  - جرّبت نقط الكويستات على كل DMap عندنا. Lily بتطابق 25 خريطة، وRose بتطابق 74، وOrchid أكتر من 70.
  - مفيش بلوك أرقام متتالية في `GameMap.dat` يحدد الـ doc، زي ما لقيته لمهرجان 2023.
  - النتيجة: مفيش خريطة أساس مؤكدة.
- **الوحوش:**

| الوحش | `monster.dat` الكلاينت | السيرفر |
|---|---|---|
| MirageSprite | 3116، لفل 135، حياة 184,444 | مفيش |
| InfernoSprite | 3117، Boss، لفل 140، حياة 10,666,666 | مفيش |
| ToxicSprite | مفيش | مفيش |
| HugeToxicSprite | مفيش | مفيش |

  - `monster.dat` فيه Name وLevel وMaxLife وBoss بس، **من غير lookface ولا هجوم ولا دفاع**. ماينفعش نعمل منه `[cq_monstertype]` حقيقي.
- **الـ NPCs:**
  - موجودين بالاسم: GriffiniaFairy (`npc.ini` 1312 `[GlobalValentine]`)، وLittleOrchid (3827 `[FlowerFairy]`)، وLily (أكتر من نوع، فمش مؤكد).
  - RoseFairy مالهاش mesh.
- **"Send meteors / roses":** ممكن تتعمل `collect` بـ Meteor 1088001 (`take: true`). بس لوحدها ومن غير جايزة مش مهرجان.

## عشان يشتغل محتاج إيه
1. الـ doc الحقيقي للـ 3 instances.
2. lookface وستاتس كاملة للأربع وحوش.
3. mesh لـ RoseFairy.
4. قرار من الأونر في الجايزة، لأن الرسمي مالوش جايزة.

_Agent 4 (13-10-2026). المصادر: `Infinity[DB]\Questinfo.ini`، و`client_test\ini\monster.dat` (3-Way) و`npc.ini` و`NpcX.ini` و`GameMap.dat`، وفحص الـ walkability على `Infinity[DB]\map\map\*.DMap`._
