# Dragonfest (DragonSaga): الزرار والشباك الرسمي بس

**الحالة:** `enabled: true`، جدول سنوي من 15 سبتمبر لحد 5 أكتوبر. ده التاريخ الرسمي من الكلاينت (`Cn_Res` `STR_FGUI_26QQSS_TIME`).

## اللي شغال
- **زرار Fgui 150475 "Dragonfest":** بيظهر في الشريط اللي فوق طول ما المهرجان شغال.
- **الشباك الرسمي `Fgui_26Qqss_Main`** (msg 10978، الكود في `Base\DragonSaga\DragonSagaUi.cs`):
  - فيه 4 مدن: Diamond March، وDragon Vault، وDragon Arena، وPrimeval Battlefield.
  - النقط بتبان 0، لأن مفيش حاجة بتدّي نقط.
  - لو دوست على مدينة بتطلعلك رسالة إنها مش مفتوحة على السيرفر لسه.
- **صفحة Highlight Events رقم 10978:** موجودة في `actinfo` بتاع الكلاينت الرسمي، وزرار الدخول فيها بيفتح نفس الشباك.
- **رسايل الليفل وانتهاء الحدث:** النص بتاعها رسمي، من `Server_Key` `tZhongQiu2026Npc[37341]`.

## اللي مش موجود وليه
- **مهام القصة 9942-9960:**
  - الـ NPCs بتاعة الـ instance (أنواع 10738-10747 زي BrightShard وAltar وTaintedSpring) والوحوش (24309-24314 زي LavaMonster وPurgatoryBrute) موجودين في الكلاينت الرسمي الأحدث بس، مش في `client_test`.
  - ومفيش lookface ليهم في أي مكان.
  - جوايز المهام 3600877 و3357159 مش موجودة في أي `itemtype`.
- **باقي الحدث:** Dragon March (CS)، وAncient Arena، وDragon Arena، والرانكنج، وDragon Vault. ده كله سكربتات سيرفر رسمية مش متاحة لينا.

## مشتق (derived)
- ترتيب المدن جه من `STR_FGUI_26QQSS_MAIN_BTN_1..4`.
- أسماء الـ callbacks اللي بنبعتها للمدن (`ZhongQiu2026Npc_City</N>n` و`ZhongQiu2026Npc_OpenRank`) اختيار السيرفر، لأن الكلاينت بيرجّع اللي بيوصله وخلاص.
- الحد الأقصى للنقط 50000 جه من `tExShop[10978][UnLock][2]`.
