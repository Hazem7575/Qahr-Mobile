# Wonderland (أليس وعالم المراية) — April Fools 2024

**الحالة (2026-10-14):** `enabled: true`، سنوي **4/2 → 4/15** (رسمي: `luaUiRes["1475-3"].txt_time` = "Duration: Apr. 2 - Apr. 15").

## اللي شغّال دلوقتي (رسمي)
- **الزرار:** `DlgAliceFoolsDayMainBtn` (Dialog_Info `1475-btn`، مكتوب عليه "Wonderland"). `icon_show` / `icon_hide` في `festival.json`.
- **الضغطة:** msg `10376` (`tMsg_Info["DlgFoolsDay24"]`) `AprilFoolsDayPage2024_OpenMainPage` ← `Base\AliceRescue\AliceRescueUi.cs`.
- **الواجهة:** `DlgAliceFoolsDayMainUi_OpenViewDatas`. فيها بيانات **حقيقية** من الشنطة:
  - Dreamy Star `3347775`، وDream Pearl `3600702`، وTreasure Ticket `3347776` (نفس ترتيب `DlgAliceFoolsDay_AddItem` 1/2/3).
  - Points = 0، ومفيش red dots، وزرار الـ dragon مقفول (`Lock`).
- **شرط اللفل 80** (رسمي: `tAprilFoolsDayMainNpc_DelText[32750][Text114]`).
- **بعد ما الحدث يخلص:** النص الرسمي `[32749][Text122]` ("Evil Jabba has been defeated...").
- **Mirror World:** لو معاك أقل من 5 Dream Pearls بيطلع النص الرسمي `tCastleBeat2023_DelText[Sys][NoFire]`، ولو معاك يقولك إن عالم المراية مش مفتوح ومش بيخصم حاجة.

## اللي بيرد برسالة مهذبة (مفيش ليه داتا رسمية على السيرفر)
- استكشاف Mirror World (`CastleBeat2023_EnterMirror`): الأدوار، والـ partners المتقيدين، وMirror Chests.
- ضرب التنين (`AprilFoolKillDragon_OpenDragon`)، وجوايز النقط (`OpenStagePage`)، والترتيب (`OpenRankPage`).
- محل الـ Treasure Ticket (`AprilFoolsDayMainNpc_ExchangeShop`)، وMemory Garden CS (`AprilFoolsDayKua_EnterOS`)، وVault Palace (`AprilFoolsDayKua_XunLi`)، وTritar (`NpcFind 32750`).
- **مفيش أي خصم ولا أي جايزة.**

## المهام 5837-5841 (لسه مش شغالة)
5 مهام مخفية: "Rescue X" (Cheshire Cat وMarch Hare وAbsolon وFurry Beast وWhite Queen)، والجايزة Lovely Fragment `3347777` ×1.
- مالهاش NPC ولا خريطة ولا وحش: بتتفتح من جوه استكشاف Mirror World (`tCastleBeat2023`)، والسيرفر مفيهوش النظام ده.
- عشان تشتغل محتاج **نظام Mirror World كامل**: الأدوار، والـ fetter، والوحوش 21881-21899 و22406، والـ Mirror Chests، والـ Dreamy Stars. ده نظام كامل ناقص.
