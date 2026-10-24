# تجربة صقل الأسطورة (Gear Mythforging): مقفول

**الحالة:** `enabled: false`. فيه `festival.json` بس.

## المهرجان ده إيه
مهمة واحدة 4943 عند Richy في توين سيتي (1002، 351,473):
- **الهدف:** "Gear Mythforging [chk_quest 4943,1,1,1]"، يعني تعمل عملية Mythforging واحدة على عدّة.
- **الجايزة:** `40703081` L1 Ethereal [Armor]. موجود في `itemtype.txt`، وهو خامة Mythforge.

## ليه مقفول (اللي ناقص بالظبط)
- **السيرفر مفيهوش نظام Mythforging خالص.** مفيش كود (`grep mythforg` على السورس فاضي)، ولا handler للباكت اللي الكلاينت بيبعتها من واجهة الـ Mythforge، ولا event في محرك الكويستات.
  - `QuestEngineData.KnownEvents` فيها `bountyDone` بس. أي objective من نوع `event` بمفتاح زي `mythforge` هيتعلّم "not workable" وماحدش هيقدر يخلّصه.
- **ماينفعش نفكّها لـ talk بس،** لأن ده هيبقى جايزة ببلاش (خامة لنظام مش موجود).
- **Richy:** فيه mesh بالاسم في `NpcX.ini` (`[105132]`، Note `[Ambassador]`، يعني mesh = 1051320)، بس مش متحط في `Npcs.txt`.

## عشان يشتغل محتاج إيه
1. **نظام Mythforge في السيرفر:** باكت الواجهة (اعرف رقمها بالـ PacketLogger وانت بتفتح الواجهة في الكلاينت)، والخامات، والنسب.
2. **سطر واحد في الكود** بعد ما العملية تنجح: `QuestEngineCore.OnEvent(client, "mythforge")`، وتضيف `mythforge` لـ `KnownEvents`.
3. بعدها المهرجان يبقى:
   - NPC واحد Richy في `npcs.txt` (mesh 1051320 عند 1002 351,473، بعد فحص الـ walkability).
   - `quests.json` فيه objective `{"type":"event","event":"mythforge","slot":1}`، والجايزة 40703081.

_Agent 4 (13-10-2026). المصادر: `Data\Questinfo.ini`، و`itemtype.txt`، و`client_test\ini\NpcX.ini`، و`Base\QuestEngine\QuestEngineData.cs`._
