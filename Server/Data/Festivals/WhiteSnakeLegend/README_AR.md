# أسطورة الثعبان الأبيض (Bless All - White Snake Mark): شغّال

**الحالة:** `enabled: true`.
- **الجدول:** `yearly` من 01-21 لـ 02-10. دي نفس مواعيد الرسمي (Cn_Res `STR_LUAUI_25CHUNJIE_MAIN_HDSJ`: من 21 يناير لـ 10 فبراير 2025).
- **تشغيل يدوي:** `@festival on WhiteSnakeLegend`.

## إزاي يتلعب (لفل 80+)
- **الزرار:** زرار Bless All فوق (Fgui 150235)، وبيفتح شباك Fgui_25ChunJie_Main الرسمي.
- **المدخل الأول "White Snake Mark":**
  - تضغط عليه، فمهمة 8350 بتبدأ (رسمياً `BeginNpcId=0`) وبتنتقل للثعبان الأبيض على 1003905. الخريطة دي بتتبعت للكلاينت كـ doc 12734 (2025tsf_new).
  - المهام من 8351 لـ 8358 بتكمل لوحدها (`next` + `autoAccept`).
  - الشباك بيعرض "Story Rewards x/9"، وعدد Fortune Knack اللي في الشنطة.
- **الجوايز:** Fortune Knack (3351698) واحدة في كل مهمة من الـ 7 اللي فيها Prize في Questinfo. المهام 8354 و8356 مالهمش جايزة. كل ده مرة واحدة لكل شخصية.
- **المداخل 2 و3 و4** (Green Snake / Bless All / Nian)، والسحب، والنقط، وBlessing Pool: مش موجودين على السيرفر. بيطلع رسالة بس، ومفيش جوايز مخترعة.

## الـ NPCs (`npcs.txt` من 8863001 لـ 8863006، في الأماكن الرسمية، وكلها walkable)

| الاسم | الشكل | المصدر | الثقة |
|---|---|---|---|
| WhiteSnake | 1100010 | NpcX 110001 [WhiteSnakeMark] | رسمي |
| XuXian | 1100020 | NpcX 110002 [WhiteSnakeMark] | رسمي |
| EarthDeity | 33740 | npc.ini 3374 [WhiteSnakeMark] | رسمي |
| WonderLingzhi | 36330 | npc.ini 3633 Herb | ⚠️ proxy |
| TraceOfMark | 35660 | npc.ini 3566 TrailSpot | ⚠️ proxy |
| BottomOfLeifengTower | 100550 | npc.ini 10055 SearchingPoint | ⚠️ proxy |

الثلاثة الـ proxy مالهمش أي entry في الكلاينت.

## الوحوش
- **حياة ولفل (رسمي من `monster.dat`):** Nightfiend 24238 (50,000، lv120) وSnakeIllusion 24242 (200,000، lv120).
- **مشتق:**
  - الشكل: Nightfiend = WhiteFiend 226، وSnakeIllusion = SnakeKing 404.
  - الهجوم والدفاع: قالب لفل 120.
- **الظهور:** كل وحش بيظهر للاعب لوحده لما يقبل المهمة (8353 و8358)، ولمدة 30 دقيقة.

## الفروق (مكتوبة في `_deviation`)
- FormidableEmergence وFragmentEvil وMarkIllusion مالهمش صف في `monster.dat` (أحداث scripted). كل واحد منهم بقى "روح للنقطة الرسمية".
- الخريطة مشتركة مش instance خاص.

## الملفات
- `npcs.txt`
- `monsters\`
- `quests.json`
- `rewards.json`
- الكود: `Base\WhiteSnakeLegend\WhiteSnakeLegendUi.cs`
- الإعدادات: `Infinity[DB]\WhiteSnakeLegend\whitesnakelegend.json`
