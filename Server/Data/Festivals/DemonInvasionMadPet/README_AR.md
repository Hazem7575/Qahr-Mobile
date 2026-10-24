# Hewing Laurel (قطع شجرة الغار) + فقاعات الكوابيس: Mid-Autumn 2023

**الحالة (2026-10-15، Phase COMPLETE):** شجرة الغار ورحلة Mystic Travel ومراحل الـ Cultivation كلهم بيتلعبوا من الشباك. التشغيل يدوي: `@festival on DemonInvasionMadPet`. التاريخ الرسمي 9/26 لـ 10/9 محفوظ في `_schedule_yearly`.

**المصدر الرسمي:**
- الكلاينت: `luaui\dialog\dlgmidautumnfestival\dlgfelllaurel*` و`dlg23midautumntravel*`، و`Dialog_Info 1425/1410`.
- النصوص: `luaUiRes[1425]` و`[1410]` و`[1273][10295]`.
- `MidAutumn_Config[10295]`.

## اللي شغال
1. **الشباك الرئيسي** (`DlgFellLaurelUi`): بيعرض Cultivation والمرحلة وعدد Lunar Spirit Pill وGiant Pill من الشنطة، ومحاولات Divine Axe، والـ hewer (Chang'e لحد Hou Yi، بيتغير مع المراحل).
2. **Daily Sign-in Pack:** أول ما تفتح الشباك كل يوم بتاخد 10 Lunar Spirit Pills. المصدر رسمي (`itemget1[1]`)، والعدد مشتق.
3. **قطع الشجرة ×1 / ×10:**
   - كل ضربة بتاخد حباية واحدة، وبتديك كنز من "Normal Hewing Rewards" الرسمي و15 Cultivation.
   - وإنت في Divine Axe الضربة بتاخد محاولة بدل الحباية، وبتديك كنز من "Divine Axe Hewing Rewards" و45 Cultivation.
   - الجدولين والنقط رسميين. المساحة بتتشيك قبل الخصم.
4. **Divine Axe:** بتستخدم Giant Pills من شباك `DlgFellLaurelUse` الرسمي، وكل حباية بمحاولة واحدة (مشتق).
5. **مراحل الـ Cultivation (44 مرحلة، `tForce` الرسمي):** فيه معاينة للجايزة الجاية (`NextYurang`) واستلام (`RankAwardget`). كل مرحلة مرة واحدة.
   - **مشتق:** محتوى الجوايز. الكلاينت بيستلمه من السيرفر، فاخترته من أيتمز جداول القطع الرسمية، وبيكبر كل مجموعة مراحل.
6. **جوايز الـ Cultivation (`DlgMidAutumnRank` ActivityId 10295):** الحدود الرسمية 500 … 80000، وبعدها كل 2000 لحد 500 مرة. والأيتمز الرسمية (`Rank.Total`). الاستلام على msg 10295 (`MidAutumnKua_Phase`).
7. **Mystic Travel (`Dlg23MidautumnTravel`):**
   - كل رحلة بتاخد 2 strength (رسمي)، وفيه ×1 و×10.
   - الجوايز من "Travel Rewards" الرسمي، ومعاها 10 Cultivation (مشتق).
   - كل يوم بتاخد 20 strength (مشتق)، والـ Mooncake بيرجع 2 strength (مشتق). الـ Mooncakes بتطلع من قطع الشجرة (رسمي).
8. **الترتيب:** أعلى 10 محليًا من داتا اللاعيبة الحقيقية.
9. **الصناديق** (`MidAutumnBoxes.cs`): الأيتمز اللي بتطلع من الجوايز ومكتوب فيها "Open to get…" بقت بتتفتح، والمحتوى من وصف الأيتم الرسمي.

   | الصندوق | المحتوى |
   |---|---|
   | +2 Stone (B) | `730002` |
   | +3 Stone (B) | `730003` |
   | Bright Star Stone | `3009001` لمدة يومين |
   | Radiant Star Stone | `3009002` لمدة يومين |
   | 3KK Contribution Medal (B) | `3322760` لمدة 7 أيام |
   | 1KK Contribution Medal (B) | `3314199` لمدة 7 أيام |
   | Twilight Star Stone | `3009000` لمدة يومين |
   | Permanent Stone Scrap (B) | `3314553` |
   | +5 Stone (B) | `730005` لمدة يومين |
   | Medium Awakening Bag | واحد عشوائي من الـ 10 المكتوبين |

## اللي ناقص وليه
- **فقاعات الأحلام والمهام 5580 / 5581:** الـ instances `1002422/1002424/1002425` مالهاش doc مؤكد في `FestivalMaps\maps.txt` (UNRESOLVED). الوحوش كمان KingAsura مالوش داتا، وWickedFlora مالوش lookface. الفقاعات مش بتظهر، والزرار بيرد برسالة.
- **Moon Devourer (CS)، وVault Palace، ومحل Moon Exchange، وCP Mall:**
  - الأولى cross-server.
  - Vault Palace نظام تاني.
  - المحلات مالهاش جدول أيتمز في الكلاينت.
  - التلاتة بيردوا برسالة. Silver Moonstone وGolden Moonflower بيتجمعوا لحد ما المحل يتعمل.
- **أحداث الكائنات السماوية في Mystic Travel (القصص والقتال):** الأحداث بتيجي من السيرفر ومش موجودة في الكلاينت.
- **Giant Pills:** مصدرها الرسمي CP Mall وAnima exchange. مضافوش هنا عشان مفيش سعر رسمي.

## الأمان
- الـ uid من السيرفر.
- كل الأعداد بتتشيك: ×1 أو ×10 بس، وGiant Pills وMooncakes لازم تكون في الشنطة، ولا رقم سالب ولا رقم كبير.
- الخصم قبل التسليم، والمساحة قبل الخصم.
- المراحل والـ stages بتتسجل قبل الجايزة، فمينفعش تتكرر بالـ relog أو بتكرار الباكت.
- فيه throttle. الحالة بتتحفظ في `Data\DemonInvasionMadPet\players.json`، والكونفيج في `config.json`.

## الكود
- **الملفات:**
  - `Base\DemonInvasionMadPet\DemonInvasionMadPetUi.cs`
  - `Base\DemonInvasionMadPet\MidAutumnBoxes.cs` (جديد).
  - بيستخدم `Base\MidAutumnReunion2022\HolidayKit.cs`.
- **الهوكس:**
  - سطر في `LuaPush.HandleCallback` (msg 10052 + 10295).
  - سطر في `MsgItemUsuagePacket.UseItem` للصناديق.
