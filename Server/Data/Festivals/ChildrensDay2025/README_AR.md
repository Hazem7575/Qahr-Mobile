# Dream Park (ChildrensDay2025): يوم الطفل 2025، كامل

**الحالة:** `enabled: true`، سنوي من 27 مايو لحد 9 يونيو (رسمي: `STR_FUI_CHILD_TXT_31`). لفل 80+ (رسمي).

## بيتلعب إزاي
1. **زرار Dream Park (Fgui 150288)** فوق ← الشباك الرسمي `ChildrenDay` بداتا حقيقية من حالة اللاعب.
2. **التجديد (Renovate x1 / x10)** للمباني التلاتة (Dessert Shop، Ferris Wheel، Carousel):
   - كل Starwish Stone = 50 Dream Point (رسمي) + 20 Moonglass + باكيت المبنى (رسمي من `luauirewardshow 19x001001/002`).
   - 7 جودات من Rundown لـ Legendary. آخر جودة عند 1050 نقطة للمبنى (رسمي `ACHIEVE_1`).
3. **أحداث مفاجئة** (علامة الاستفهام على المبنى): 9 أحداث رسمية بقرارين.
   - القرار 1: بياخد 1 Starwish ويدّي 10~60 Moonglass و50 نقطة.
   - القرار 2: ببلاش ويدّي 5 Moonglass.
4. **تشغيل الحديقة (Op. Rwd.)** مرة في اليوم: Moonglass حسب جودة المباني + 1 Starwish. بعدها عداد لحد نص الليل.
5. **النقط والرانكنج** (زرار النقط فوق على الشمال):
   - مراحل النقط رسمية: كل 1000 (لحد 999 مرة)، و5000، و10000، و20000، و40000، و60000، و80000، و120000.
   - مكافآت المباني لما توصل 1050.
   - جوايز الرانك المحلي الرسمية (Local No.x Dream Box)، والرانك المحلي بيظهر في الشات (الحد الأدنى رسمي: 20000 نقطة).
6. **Vault Palace** (زرار المبنى 5، أو طريقة الحصول 2):
   - بينقلك للخريطة 12818 (doc 12019).
   - فيها Flora، و3 Starlight Blessings: Saluting Gun وFirework وBalloon.
   - لما تولّع التلاتة بتاخد Starwish Pack، مرة في اليوم.
7. **NPCs في توين سيتي:**

| NPC | المكان | بيعمل إيه |
|---|---|---|
| **Suyi** | (375,441) | الشرح ونتيجة النهارده، وفتح الشباك، و**Dream Store** و**VIP Dream Store** (بيفتح عند 15000 نقطة) |
| **Prince Puff** | (378,446) | محل الـ CPs |

   - **Dream Store** فيه Anima Exchange الرسمي: P9/P10/P11 Anima ← Small/Medium/Grand Starwish Box.
   - **محل Prince Puff:** Starwish Pack بـ 100 CPs، وصناديق Sweet Dream / Tiara / Concerto.
8. **الصناديق بتتفتح:**
   - Starwish Pack/Box (3-100 / 42 / 85 / 175).
   - Dessert / Carousel / Ferris Wheel / Dream Material Pack.
   - Local No.x Dream Box.
   - Sweet Dream Box (T/B) وSweet Tiara Box وConcerto Boxes.
   - 30 Sweet Fragment ← Sweet Dream، و30 Concerto Fragment ← Candy Concerto 2H.

## رسمي ولا مستنتج
- **رسمي:**
  - كل النصوص، والـ IDs، والـ 50 نقطة، و15000، و20000، و1050، وحدود المراحل.
  - جداول المحلات بالأسعار والحدود (`Infinity[DB]\ChildrensDay2025\stores.json` = `EmoneySaleShop.lua` 342501/342502/34250).
  - محتوى الصناديق (من وصف itemtype)، وأشكال الـ NPCs (NpcX 110102/110103/110104، وnpc.ini 10181-10183).
- **مستنتج** (كله في `Infinity[DB]\ChildrensDay2025\childrensday2025.json` وتقدر تغيّره):
  - حدود الجودات بين 0 و1050، وإيراد كل جودة، ونسبة الأحداث (15%) وجوايزها.
  - مكافآت مراحل النقط والمباني (من أصناف الحدث بس)، وأوزان الباكيتات العشوائية.
  - عدد الفراجمنتس (30)، وجايزة الـ Starlight Blessing (Starwish Pack).
  - أماكن الـ NPCs (الكلاينت مفيهوش إحداثيات؛ كل الخلايا walkable).
  - حدود الشراء بتتعد لكل شخصية في كل دورة مهرجان.
- **IDs:** NPCs المهرجان 8866000-8866099 محجوزة لـ ChildrensDay2025 (المستخدم: 8866001-003 و8866011-013).

## اللي مش ممكن وليه
- **Dream Quests:** خريطة Dream Park والـ 7 instances (Spooky House، Into Flame...) مش موجودين في أي GameMap.dat عندنا، فالزرار بيرد برسالة.
- **الرانك العالمي:** cross-server، فبيرد برسالة. صناديق Global مش بتتفتح لنفس السبب.
- **توزيع جوايز الرانك المحلي آخر الحدث:** محتاج hook لنهاية المهرجان (مش موجود في FestivalManager).
- **Sweet Dream Hairstyle (T)** بيتدّى مع Sweet Tiara Box بس مش بيتفتح (نظام الشعر).
- **View statues** في Vault Palace.

## الكود
- **ملفات جديدة للـ csproj:**
  - `Base\ChildrensDay2025\DreamParkConfig.cs`
  - `Base\ChildrensDay2025\DreamParkState.cs`
  - `Base\ChildrensDay2025\DreamParkItems.cs`
  - `Base\ChildrensDay2025\DreamParkNpcs.cs`
  - `ChildrensDay2025Ui.cs` كان موجود واتعدّل.
- **Hooks:**
  - `LuaPush.cs` (msg 10664، موجود من قبل).
  - `Game\MsgNpc\Procesor.cs`: سطر واحد قبل `FestivalManager.OnNpc`.
  - `MsgItemUsuagePacket.UseItem`: سطر واحد.
- **اتجرّب:** harness بالـ dll الحقيقي **61/61**.
  - رفض كل الاستغلال: أرقام غلط، وإعادة الطلب، وشنطة مليانة، وبعيد عن الـ NPC، وحد الشراء، وVIP قبل 15000، وCPs مش كفاية، ومرتين في اليوم.
  - live: الـ 6 NPCs اتحطوا على السيرفر (on ثم auto).
