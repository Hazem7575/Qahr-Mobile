# مهرجان التلألؤ (Twinkling Fest) - البحث عن شظايا التلألؤ: شغّال

**الحالة:** `enabled: true` بس الجدول `manual`. الرسمي بيخلص 14 يناير 2026 (مكتوب في وصف Aurora Crystal/Dew)، لكن تاريخ البداية مش موجود في أي ملف في الكلاينت. يعني الأونر هو اللي بيشغّله: `@festival on TwinklingShard`.

## إزاي يتلعب (لفل 80+، زي الرسمي)
- **الزرار:** زرار Twinkling Fest فوق (Fgui 150381)، وكمان صفحة Highlight Events رقم 10831.
- **الشباك:** هو شباك ChristmasMainUI الرسمي، وفيه 6 مباني:

| المبنى | المنطقة | الخريطة (doc) | المهام |
|---|---|---|---|
| Dawn Star | Joy | 1005569 (13154) | 9512-9514 |
| Stellar Court | لقاءات خاصة | — | مش موجودة على السيرفر |
| Stellar Spire | Hope | 1005565 (13150) | 9500-9502 |
| Star Nest | Wisdom | 1005568 (13153) | 9509-9511 |
| Stellar Bay | Will | 1005566 (13151) | 9503-9505 |
| Stellar Tower | Valor | 1005567 (13152) | 9506-9508 |

- **دخول منطقة:** تضغط على المبنى، فالمهمة الأولى بتاعته بتبدأ (رسمياً `BeginNpcId=0`) وبتنتقل للمنطقة. الباقي بيكمل لوحده (`next` + `autoAccept`).
- **كل منطقة مرة في اليوم:** المهام `repeat: daily`. ده مطابق للرسمي ("Completed searching %s/5 time(s) today"). المنطقة اللي خلصتها النهارده بتبقى رمادي.
- **Find Twinkling Shards:** بيختار منطقة عشوائي من اللي لسه ماخلصتهاش النهارده، ويحط عليها عمود نور. الحد 5 في اليوم.
- **الجايزة:** لما تلم الـ 5 شظايا في نفس الدورة بتاخد Wuqu Snow Box (3355342) مرة واحدة لكل شخصية. ده الرسمي: luauirewardshow 341006000، والنص Text133.
  - الشظية بتتحسب لما تخلص آخر مهمة في منطقتها: 9502 أو 9505 أو 9508 أو 9511 أو 9514.
  - بنعرف ده من سطور `QUEST` في `festivals_log.txt` اللي بعد بداية الدورة، ومن سجل المهام الحالي.
  - السجل بيتحفظ في `Data\TwinklingShard\claims.txt`.

## الرسمي والمشتق
- **رسمي:**
  - المهام ونصوصها وأماكن كل الـ NPCs (من `Questinfo.ini`).
  - الأشكال: بلوك npc.ini من 10419 لـ 10442، وNpcX 110234. أسماؤهم مطابقة لـ Questinfo.
  - الخرايط (maps.txt، ثقة A).
  - حياة ولفل الوحوش: GiantYeti 2,000,000 lv120، StellarPhantom 50,000، MadScholar 2,000,000 (من `monster.dat`).
  - جايزة الـ 5 شظايا، واللفل 80.
- **مشتق:**
  - **شكل الوحوش (lookface):** مش موجود في أي مصدر.
    - GiantYeti = SnowApe (103)
    - StellarPhantom = Phantom (363)
    - MadScholar = 978
  - **هجوم ودفاع الوحوش:** نفس قالب الوحوش لفل 120 (1400-1800، defence2 10000).
  - **الخرايط:** مشتركة مش instance خاص. الوحوش بتظهر لكل لاعب لوحده لما يقبل المهمة، ولمدة 30 دقيقة.
  - **الخطوات المكتوبة في `_deviation` جوه `quests.json`:**
    - GiantWave وWaves مالهمش شكل في الكلاينت:
      - GiantWave بقى ضغطتين على BreachPoint.
      - Waves بقى توصل للنقطة 139,168.
    - Scholar`sTest وFreely make a Dessert ألعاب مصغّرة، فبقوا كلام مع StellarScholar أو Dessert.
    - Questinfo كاتب DefenseDevice في 9508 عند 74,127، بس هو متحط في مكانه الرسمي 54,120.
- **من غير جوايز مهام:** Questinfo مافيهوش Prize للـ 15 مهمة.
  - الرسمي بيدّي Aurora Crystal/Dew لكل استكشاف (luauirewardshow 341001-341005).
  - دول بيتصرفوا في Aurora Fireworks، وده مش موجود على السيرفر، فماتحطوش.

## مش موجود (بيطلع رسالة بس)
- Stellar Court (لقاءات ImmortalityDivinity2025).
- Aurora Fireworks/المدافع، والنقط، والترتيب.
- Feeding Olaf وVault Palace.
- محل CP (Guide).
- NPCs توين سيتي (Suyi وFrostHighPriest): مالهمش مكان رسمي في أي ملف.

## الملفات
- `npcs.txt` (23 NPC من 8862001 لـ 8862046)
- `monsters\` (23472-23474)
- `quests.json`
- `rewards.json`
- الكود: `Base\TwinklingShard\TwinklingShardUi.cs`
- الإعدادات: `Data\TwinklingShard\twinklingshard.json`
