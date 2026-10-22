# الأرض السرية للقوة الداخلية (InnerPowerSecretland)

**المالك:** Agent 3، بتاريخ 13-10-2026.
**المهام:** مهمة واحدة بس، 7108 "[Inner Power] Mountain Secretland". من TypeId 8 في `Questinfo.ini`.

## الحالة: مقفول (`enabled: false`)
- المهمة بتبدأ وبتخلص عند MysterySage رقم 23021. هو موجود أصلاً في `Npcs.txt` عند 1002 (378,383)، والـ mesh بتاعه 49290 = npc.ini NpcType4929 MysterySage. فمحتاجناش NPC للمهرجان.
- خطوتها الوحيدة: "Pass the inner power quest once".
- الخطوة دي محتاجة إشارة من كود دنجن القوة الداخلية، وهي `QuestEngineCore.OnEvent(client, "innerpower.secretland")`.
- المحرك النهارده مابيعرفش غير `bountyDone`. يعني لو المهمة اشتغلت، عمرها ما هتخلص.

## اللي ناقص عشان يشتغل
1. الكود (ER-6): في المكان اللي اللاعب بيخلّص فيه دنجن القوة الداخلية، ضيف السطر ده:
   `QuestEngineCore.OnEvent(client, "innerpower.secretland", 1)`
   وضيف الاسم `innerpower.secretland` في `QuestGrammar.KnownEvents`.
2. الجايزة: النص الرسمي بيقول "Inner Power Books" ومن غير رقم أيتم.
   - دلوقتي حاطين كرة خبرة واحدة مكانها، وده placeholder.
   - المرشحين من `itemtype.txt`: 3328940 Secretland Box و3322874 InnerPowerPack. اتأكد إنهم بيتفتحوا في السيرفر قبل ما تحطهم.
3. بعد كده: خلّي `enabled: true`.
   - الجدول دلوقتي weekly سبت وحد، وده placeholder.
   - المهمة `repeat: daily`.
