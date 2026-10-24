# Castle Startle / لغز القلعة (CastleMystery) - عيد الحب 2025

**الحالة:** `enabled: true`، `yearly` من **02-11 لـ 02-24** (رسمي: `EmoneySaleShop[33978]` Castle Key من 20250211 لـ 20250224، ونص الـ Castle Key "Expires ... Feb. 24, 2025").
- تشغيل يدوي: `@festival on CastleMystery`.
- **محتاج بيلد جديد:** ملف جديد `Base\CastleMystery\CastleStartle.cs` (لازم يتضاف للـ csproj)، و`CastleMysteryUi.cs` اتعدّل، و`Base\BandsTrouble\StoryEntry.cs`.
- **hooks جديدة (سطر واحد لكل واحد، عليهم tag `[CastleMystery 2026-10-15]`):**
  - `Game\MsgNpc\Procesor.cs`: `CastleStartle.OnItemDialog` (بعد SpringHooks.OnItemDialog) و`CastleStartle.OnNpc` (بعد SpringHooks.OnNpc).
  - `MY System\MsgItem\MsgItemUsuagePacket.cs`: `CastleStartle.OnUse` (بعد SpringHooks.OnUse).
  - نسخة احتياطية: `Backups_Systems\20261015_094714_CastleMystery_hooks\`.
- **الأرقام:** `Data\CastleMystery\castle.json`. الحالة: `Data\CastleMystery\state.json`.

## بيتلعب إزاي (لفل 80+)
1. **القصة 8422-8429** (زي ما كانت) في نسخة خاصة من 1003099 (doc 12814)، من زرار Fgui 10139 أو من Suyi "Head to Auter Castle." أو من قايمة ItemGet في الشباك (`ValentinesDay2025Story_EnterInsatnce`).
   - **جديد:** خطوة "Rampaging" بقت وحوش بجد:
     - **8428:** PhantomMaid بتطلع لما تقبل المهمة عند نقطة Questinfo (215,130) في نسختك.
     - **8429:** PhantomLord بيطلع عند (132,105).
     - لو الوحش اختفى أو مات بعيد، الـ NPC بتاع بداية المهمة بيدّيك "Progress" عشان يطلعه تاني.
   - الجوايز: 7 Castle Keys + Butler`s Token (Questinfo).
2. **Suyi [CastleStartle] في توين سيتي (362,440)** (شكل رسمي NpcX 110010، النصوص الرسمية `tLover2025Kua_DelText[33978]`):
   - "Head to Auter Castle." ← القصة.
   - "Obtain castle treasures." ← تستخدم Castle Key (1 أو 10).
   - "Buy Castle Keys." ← متجر الـ CP الرسمي: Castle Key بـ 20 CP، Fine Castle Key Box بـ 199 CP (أقصى 20)، Grand Castle Key Box بـ 999 CP (أقصى 5).
   - "Check ranking rewards." / "Visit Love Garden." ← رسالة (cross-server).
3. **كنوز القلعة:** كل Castle Key ← Chocolate Candy + 10 Castle Points (الجدول **مشتق** في `castle.json`، المتوسط ~57 Candy للمفتاح). كمان تقدر تستخدم الـ Castle Key من الشنطة.
4. **Jet [CastleStore] (373,434)** (NpcX 110011): متجر Chocolate Candy الرسمي `EmoneySaleShop[33979]` في `rewards.json`:
   - Preserved Flower بـ 1، UniversalRuneEssence (B) بـ 3، +1 Stone (B) بـ 3، 1KK Contribution Medal (B) بـ 8.
   - Romantic Parasol / Flute / String / Pet Box بـ 3600 (مرة واحدة لكل صندوق في كل run).
5. **Lord Anthony (365,445)** (npc.ini 10029): الحكايات الرسمية (14 موضوع، 5 كل يوم)، وبعد الحكاية بيدّيك Castle Key (النصوص الرسمية ADNTip). مرة في اليوم (مشتق).
6. **الصناديق من الشنطة:**
   - Fine Castle Key Box = 11 مفتاح، Grand = 55 (نص الـ item الرسمي).
   - Romantic Parasol / Flute / String Box: تختار واحد من الأسماء اللي في وصف الصندوق الرسمي.
   - Romantic Pet Box: تختار من Panda Ponpon / Dragon Ruyi / Doggy Pudding (نص Suyi الرسمي Text117، وصف الصندوق نفسه فاضي).
   - **Dream Note (Valentine2023)** بيتعامل هنا كمان: 25 ← Twinkle Waltz (B)، و50 ← Twinkle Waltz (Brilliance) (النص الرسمي `tValentineDay2022Npc[3340595]`).

## الرسمي والمشتق
- **رسمي:** القصة والجوايز وأماكن الـ NPCs جوه القلعة (Questinfo)، أسماء/لفل/HP الـ Phantoms (monster.dat 22966/22967)، أشكال Suyi وJet وAnthony، نصوصهم، أسعار وحدود متجر الـ CP، متجر Jet كله، محتوى الصناديق، المواعيد.
- **مشتق:**
  - **شكل الـ Phantoms وهجومهم** (`monsters\*.ini`، ids 22966/22967): PhantomMaid = look 261 (روح رمادي بهدوم طويلة)، PhantomLord = look 260 (لورد بدرع غامق وعباية حمرا). الهجوم والدفاع من قالب bosses لفل 135 بتوع CrisisSeries. مفيش فيديو رسمي اتلقى. غيّر الـ lookface براحتك.
  - جدول كنوز المفتاح، Castle Points، مفتاح Anthony مرة في اليوم، أماكن الـ NPCs في توين سيتي (الأماكن الرسمية مش في الكلاينت؛ اتشيّكت walkable على الـ DMap الرسمي).
- **الـ ids:** وحوش 22966-22967 (من بلوك مجموعتي 996300-996399)، NPCs 8871011-8871013 (بلوك CastleMystery 8871001-8871013).

## مش موجود وليه
- **Love Garden** (Love Sprite، Preserved Flowers، Heaven/Celestial Dragon): أرض cross-server (`Lover2025Kua_EnterKuaLand`). سيرفر واحد بس.
- **الترتيب وجوايزه:** cross-server.
- **شباك السحب الرسمي 2025** (`DlgValentinesDayMainUI` بـ DlgID): داتا الـ DlgID بتاعة 2025 مش في الكلاينت، فالكنوز بتمشي من Suyi ومن الشنطة.
- **دخول Secret Chamber والـ cellar للـ Butler** (Jet Text151): مفيش داتا للأوض دي.
- "Avoid the freeze trap" و"Handle ChamberCrisis": أحداث جوه الأوض مش وحوش (زي ما كانت: الوصول للنقطة الرسمية).

## الاختبار
- harness بالـ GameServer.dll الحقيقي (temp) على نسخة من الداتا: **49 PASS، 0 FAIL**.
- اتجرّب: 8422-8429 كلهم workable، 8428 بيتقبل ويدخل نسخة ويطلع PhantomMaid مرة واحدة والقتل بيتحسب؛ متجر الـ CP (من غير CP، option مش معروض، تكرار الـ option، حد الـ 20، لفل 70)؛ الصناديق (11 / 55 مفتاح، من غير صندوق، اختيار مش معروض)؛ الكنوز (0 / سالب / 11 مفتاح، من غير مفتاح، لفل 70)؛ مفتاح Anthony (مرتين في نفس اليوم، إعادة الـ option، اليوم اللي بعده)؛ Dream Note؛ إعادة تحميل الـ state؛ المهرجان مقفول.
- live: `reloadfestivals` ← CastleMystery valid، 3 NPCs، 2 monster types، 8 quests، 8 exchanges. الكود الجديد محتاج restart.
