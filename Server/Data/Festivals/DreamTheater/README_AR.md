# مسرح الأحلام (DreamTheater)

**آخر تحديث:** 15-10-2026، مراجعة كاملة على 4 فيديوهات رسمية + لعب كامل على mohamed2.
- مقارنة الفيديو بالتفصيل: `tools\ProtoStudio\out\dreamtheater_video_diff.md`.
- المواصفة القديمة: `tools\Festivals\DreamTheater_OFFICIAL_AR.md`.

## بيشتغل إمتى
- الرسمي من 5 مارس لـ 25 مارس. هنا `manual`: `@festival on DreamTheater`.
- اللفل المطلوب 80.

## اللعب (زي الرسمي)
1. **Agnes في توين سيتي (370,437):** القصة الرسمية، وآخرها "Revive the theater," بيفتح شباك Dream Theater.
   - أول مرة الشباك بيتفتح بيطلع guide Agnes الرسمي (8 خطوات) مرة واحدة بس.
   - بعد كده Agnes: "Go to Dream Theater." (الشباك)، "Go to Swan Lake (locked)."، "View Theater Points and ranking rewards.".
2. **عقد واحد كل يوم من Agnes جوه الشباك** (TaskIndex الرسمي):
   - 1-7 = التجنيد بالترتيب: Irwin، Daisy، Oliver، Mia، Maryan، Jenkins، Lucky Kitty.
   - بعد ما الـ 7 يخلصوا: تنضيف (8) أو مشاكل الممثلين (9). اختيار اليوم **DERIVED** (يوم ويوم).
   - كل عملية تجنيد = عملة Wish Gold Coin. المجموع 7.
3. **العقد خلص ← العرض جاهز** (Enter Theater بتنور): Settlement رسمي.
   - النقط = نقط اللفل + 30 لكل ممثل (فيديو: 330 / 520 / 550).
   - Funds حسب اللفل: Lv1 100000، Lv3 200000. Lv2/4/5 **DERIVED** (+50000 لكل لفل).
   - الصفحة التانية: Wish Silver Coin = فضة اللفل + 1 لكل ممثل، وعدد العروض.
   - عرض واحد في اليوم.
4. **مهام الـ instance:** الدببة (5803)، الخيل (5744)، الـ Birdmen (5748) في نسخة خاصة.
   - لو خرجت قبل ما تخلص: كلّم الصنوبر / Oliver / Jenkins تاني وهترجع نفس النسخة.
5. **جوه المسرح:** الممثل بيظهر بس بعد ما يتجند. أماكن التنضيف والأركان بتظهر بس وقت المهمة بتاعتها.
6. **Wish Fountain:** فضة = أمنية (+20 نقطة)، دهب = 10 أماني (+200). بعد الإفكت بيطلع "You received" الرسمي.
7. **جوايز النقط:** الصفوف الرسمية 1000 → 120000 + كل 5000 (من `MidAutumn_Config[10369]`).
8. **الترتيب المحلي:** 30000 نقطة على الأقل. بيتقفل 25/3 الساعة 08:00. الجايزة بتيجي **بالبريد** (Dream Splendid/Starry/Glowy/Lucky Box).
9. **Fluffy:** محل الـ CP الرسمي + تبديل Twinkle. **Flori:** الورد/الهدايا والشهادات ← Twinkle، و P10/P9/P8 Anima ← 512/256/128 فضة.

## كل حاجة الحدث بيديها (لازم تشتغل لما تتستخدم)
| الحاجة | id | بتعمل إيه لما تدوس عليها | الـ handler |
|---|---|---|---|
| Wish Gold Coin | 3347476 | MsgBox رسمي "10 wishes... Go there now?" ← الـ Wish Fountain. بعد الحدث = 100 Chi | `DreamTheaterItems.UseCoin` |
| Wish Silver Coin | 3600697 | MsgBox "1 wish" ← الـ Fountain. بعد الحدث = 10 Chi | نفسه |
| Wish Twinkle Coin | 3347478 | رسالة التبديل عند Fluffy. بعد الحدث = 10 Chi | نفسه |
| Love Velvet | 3347321 | ديالوج رسمي: mount armors / garments / titles / تكملة بالـ CP / Chi | `VelvetDialog` |
| Beloved Bridal / Swan Sonata vouchers | 3347510-3347515 | بتدي الجارمنت / الـ mount armor | `CastleStartle.Vouchers` |
| Only/Sincere/True Love Title Box | 3347516-3347518 | التايتل (حسب النوع) | `OpenTitleBox` |
| Dream Splendid/Starry/Glowy/Lucky Box | 3347479-3347482 | المحتوى الرسمي من نص الـ item | `OpenRankBox` (theater.json `boxes`) |
| Elite / Deluxe Flower Pack | 3310215 / 3310216 | شهادات ورد عشوائية (30-200 / 300-2000، **DERIVED**) | `OpenFlowerPack` |
| حاجات الـ Fountain والجوايز (أحجار، Chi Liquor، Rune Essence، Awakening Packs، medals، فواكه، Parasol Scrap Box) | - | handlers السيرفر العادية للحاجات دي | - |

كل استخدام: المساحة الأول، بعدين الحاجة بتتاخد، بعدين التأثير، والشباك بيتحدّث. فيه throttle ضد الدبل كليك.

## الملفات
| الملف | فيه إيه |
|---|---|
| `theater.json` | كل الأرقام: اللفلات، الـ Fountain، الجوايز، الترتيب، الصناديق، الـ Love Velvet. كل حاجة **DERIVED** مكتوب جنبها |
| `quests.json` | التجنيد بقى من Agnes (beginNpc + prequest)، الـ instances، العقود اليومية |
| `rewards.json` | Fluffy و Flori والتحيات الرسمية |
| `npcs.txt` | 31 NPC. Maryan/Jenkins رجعوا NpcX 106146/106147 (الأشكال الرسمية) |
| `monsters\` | BlackBear 22400 (شكل 312 = باندا بروب أزرق زي الفيديو، **DERIVED**)، GoldArmBirdman 22395 (= Birdman الرسمي)، KnightJenkins 22396 |
| `Data\FestivalMaps\maps.txt` | 1002560 / 1002565 / 1002566 اتحلّوا |
| الكود | `Base\DreamTheater\DreamTheaterModule.cs` / `DreamTheaterLua.cs` / `DreamTheaterItems.cs` |
| hooks | `Procesor.cs` (OnNpc + OnItemDialog)، `MsgItemUsuagePacket.cs` (OnUseItem)، `RoleView.cs` (ظهور NPCs المسرح) |

## لسه ناقص (السبب)
- **Flower Language و Vault Palace و Swan Lake و 5795:** النصوص بس في الكلاينت، مكان الورد والموجات مش موجودين.
- **مشهد الأوبرا:** مفيش packet معروف بيشغّل سكربت الكلاينت.
- **الترتيب الإقليمي والفرق، Ebony Dream:** محتاجين أكتر من سيرفر.
- **حد 500 Twinkle في اليوم عند Flori:** ✅ اتعمل (2026-10-16). الكاب على تبديلات الهدايا/الورد (`gift_*`) بس — الشهادات (`*_cert`) مش محدودة زي الرسمي. العدّاد لكل لاعب محفوظ في EventStore (حدث "DreamTheater"، مفتاح `flori_twinkle`) وبيصفّر كل يوم. الكود في `FestivalManager.ExchangeCore`.
