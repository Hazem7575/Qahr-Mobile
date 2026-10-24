# عيد الحب 2023: Starlight Recital (Valentine2023)

**الحالة:** `enabled: true`، `yearly` من 02-14 لـ 02-27 (الرسمي: `luaUiRes_EN[1334].ActTime` = "Feb. 14 - Feb. 27").
- تشغيل يدوي: `@festival on Valentine2023`.
- **محتاج بيلد جديد للسيرفر:** `Base\Valentine2023\Valentine2023Ui.cs` (نفس الملف، اتوسّع، مفيش ملف جديد للـ csproj). سطر الهوك في `LuaPush.cs` موجود من قبل.
- **الأرقام:** `Data\Valentine2023\recital.json` (السيرفر بيعيد قراية الملف لو اتغيّر). الحالة: `Data\Valentine2023\state.json`.

## بيتلعب إزاي (لفل 80+، الشباك الرسمي DlgLoverDayMain)
1. **أرواح الآلات (4 من 6 كل يوم Recital):** تدوس على الروح، فالروح بتطلع جنبك في توين سيتي ("Piano Spirit made a gorgeous appearance!"). لو مش في توين سيتي: النص الرسمي "Please go to Twin City to join in the Romance Feast."
   - لما تقتلها: 3 Performance Inspiration + 1 Dream Note + 100 نقطة، ورسالة KillBossTip الرسمية.
   - الكريديت لصاحب الروح بس، ولازم يكون هو أكتر واحد ضرب فيها وقريب منها. لو حد تاني قتلها، صاحبها يقدر يتحداها تاني.
   - الروح بتختفي بعد 10 دقايق لو متقتلتش.
2. **النوت العشرة (Magic Book):** كل نوتة بتاخد 1 Performance Inspiration (بتتشال الأول)، وبتنوّر نوتة عشوائية:
   - كل نوتة: 1 Dream Note + 100 نقطة.
   - جايزة النوتة حسب مكانها (رسمي، `tRewardItemInfo`): 1 مفيش، 2-4 Stone 730004، 5-7 RadiantStarStone 3009002، 8-9 Awakening Spirit x300، 10 PermanentStone 723694.
   - لو المؤشر وقف في منطقة Extra: +100 لـ 500 نقطة (رسمي).
3. **Time Watch (Time Shift):** بعد ما الأرواح الأربعة تموت (النص الرسمي Trans[1])، بيكلّف 50 Time Power ويبدأ يوم Recital جديد: أرواح جديدة ونوت مطفية. النقط بتفضل.
   - Time Power: الحد 100 (رسمي). البداية 50، وبيزيد 5 كل ساعة (مشتق).
   - لو الطاقة ناقصة وفي شنطتك Time Berry (3600449)، بتتستخدم أوتوماتيك (1 = 1 نقطة، رسمي)، بحد 20 في اليوم (مشتق).

## الرسمي والمشتق
- **رسمي:** الزرار والشباك والـ calls والردود، أسماء الأرواح ونصوصها، رسايل الخطأ (NoMap، NoMonster، LackArrow، KillAll، LackTimeEnergy، Limit)، جوايز النوت، الـ Extra 100-500، Time Power 100 / 50.
- **مشتق (مكتوب في `recital.json`):** عدد الـ Inspiration والـ Dream Notes والنقط لكل روح ونوتة، شحن الطاقة، حد الـ Berries، مدة الروح، واختيار الأرواح الأربعة.
- **وحوش الأرواح (DERIVED، مفيش صف ليهم في monster.dat الرسمي):** `monsters\*.ini`، لفل 100، 300000 HP، الهجوم والدفاع من قالب لفل 100، `Boss=1` عشان السيرفر يحسب الضرر لكل لاعب.

  | الروح | id | lookface | السبب |
  |---|---|---|---|
  | PianoSpirit | 996301 | 145 | سيدة بفستان أبيض ودهبي (الحكيمة) |
  | ViolinSpirit | 996302 | 459 | سيدة بفستان أحمر ودهبي (لطيفة ومبتسمة) |
  | CelloSpirit | 996303 | 175 | سيدة نبيلة (الأخت الكبيرة) |
  | FluteSpirit | 996304 | 457 | بنت بفستان أبيض وأخضر (متحمسة) |
  | ClarinetSpirit | 996305 | 213 | روح زرقا (منعزلة) |
  | HornSpirit | 996306 | 470 | روح وردي وبنفسجي (بتفرّح) |

  - مفيش فيديو رسمي للحدث ده لقيته، فالأشكال اتختارت بالموضوع. غيّر `lookface` في الـ ini براحتك. كل الأشكال المختارة ليها ملف حركة (motion) في c3.wdb.
- **مكان القتال (مشتق):** في الرسمي الروح بتطلع في instance (نصوص EnterIns). الـ instance دي مش معروفة، فالروح بتطلع جنبك في توين سيتي.

## مش موجود وليه
- **Golden Masquerade** (نص الليل: الأقنعة وModern Plaza وKeen Break Dancer): ماب Modern Plaza مش متحلولة في `maps.txt`. الـ Time Shift بيعدّيها ويبدأ يوم Recital جديد على طول.
- **جوايز Ardor Points والترتيب:** جدول الجوايز مش في الكلاينت. الزرار بيقولك نقطك بس.
- **تبديل Dream Notes:** **بقى شغال** من الشنطة (استخدم الـ Dream Note): 25 ← Twinkle Waltz (B)، 50 ← Twinkle Waltz (Brilliance) (نص `tValentineDay2022Npc[3340595]`). الكود في `Base\CastleMystery\CastleStartle.cs` (hook استخدام الـ items بتاع مجموعة Valentine). تحويلها لـ Chi Points أو شرا Dream Notes بالـ CP مش موجودين.
- **Love Wish (Love Candy):** شباك الـ draw بتاع CastleMystery، والداتا بتاعته مش في الكلاينت.

## الـ ids
- **محجوز لمجموعتي (Valentine2023 / CastleMystery / CrisisSeries / JellyfishSkyCity):** وحوش **996300-996399**.
- Valentine2023 = 996301-996306. مفيش NPCs جديدة.

## الاختبار
- harness بالـ GameServer.dll الحقيقي (بيلد في temp) على نسخة من الداتا: **45 PASS، 0 FAIL**.
- اتجرّب: المهرجان مقفول (101)، لفل 70 (102)، index غلط/سالب/كبير/مش النهارده، برة توين سيتي، جوه نسخة خاصة، روحين في نفس الوقت، قتل من حد تاني، صاحبها بعيد، روح ماتت مترجعش، sweep مرتين من غير جايزة تانية، انتهاء الـ 10 دقايق، نوتة من غير Inspiration، سبام، النوتة الـ 11، Time Shift قبل الأرواح ومن غير طاقة، حد الـ Berries، شحن الطاقة وسقفها، شنطة مليانة، وإعادة تحميل الـ state.
- live: `reloadfestivals` بيقول Valentine2023 valid و6 monster types. الكود الجديد محتاج restart (الـ main بيعمل deploy).
