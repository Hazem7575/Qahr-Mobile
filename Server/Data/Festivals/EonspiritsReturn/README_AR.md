# عودة أرواح الأزل (Return of Eonspirits): مقفول

**الحالة:** `enabled: false`. ده حدث بيتلعب من **واجهة في الكلاينت**، مش من NPCs.

## المهرجان ده إيه
4 مهام لفل 30+، كلها `BeginNpcId=0` و`FinishNpcId=0`. الوصف بيقول: "Quests interrupted can be resumed on the Eonspirit Advent interface".

| المهمة | المطلوب |
|---|---|
| 5825 | تزور Conqueror Xiang Yu |
| 7884 | تكسر Deity Chain بفاس عشان تفك ختم روح المحارب |
| 7885 | تروح Eonspirit Palace تدوّر على باقي الأرواح |
| 7886 | Soul Assimilation |

- مفيش جايزة في `Questinfo.ini`.
- المهمة رقم 2 مش موجودة أصلًا في الملف (الأرقام بتنط من 5825 لـ 7884).

## ليه مقفول (اللي ناقص بالظبط)
- **مفيش ولا token** (`[npc]` / `[mob]` / `[map]`) في أي سطر Intention. يعني مفيش NPC ولا مكان ولا وحش رسمي نبني عليه. كل خطوة `chk_quest` بتعلّمها الواجهة نفسها.
- **السيرفر عنده نظام أيتمات الـ Eonspirit:** `GameClient.EonspiritSystem`، والـ positions 221-223 في `Enums.cs`، و`MY System\Yuanshen`، و`EonSpiritRank.txt`.
  - **بس مفيش "Eonspirit Advent":** لا الواجهة، ولا القصة، ولا Xiang Yu، ولا Eonspirit Palace.
- محرك الكويستات يعرف `bountyDone` و`useItem:<id>` بس. الخطوات دي محتاجة events من النظام.
  - "استخدم الفاس" ممكن تبقى `useItem`، بس رقم الفاس ومكان السلسلة مش معروفين.

## عشان يشتغل محتاج إيه
1. **باكتس واجهة Eonspirit Advent:** هتلاقيها بالـ PacketLogger وانت بتفتح الواجهة في الكلاينت. وبعدين handler في السيرفر.
2. **events في الكود** عند كل خطوة، مثلًا:
   - `QuestEngineCore.OnEvent(client, "eonspirit.xiangyu")`
   - `"eonspirit.chain"`
   - `"eonspirit.palace"`
   - `"eonspirit.assimilate"`
   - وتتضاف كلها لـ `KnownEvents`.
3. بعدها `quests.json` فيه objectives من نوع `event` بالمفاتيح دي. وخلّي بالك: المهام ملهاش جايزة رسمية.

_Agent 4 (13-10-2026). المصادر: `Data\Questinfo.ini`، والسورس (`Client\GameClient.cs` و`Enums.cs` و`MY System\Yuanshen`)._
