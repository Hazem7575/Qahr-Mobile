# الكريسماس العالمي 2022 - مصنع المعجزات (Christmas2022)

**الحالة:** `enabled: true`، و`yearly` من 12-20 لـ 01-04. دي المواعيد الرسمية من luaUiRes_EN["1323"].acttime = "Dec. 20 - Jan. 4" (سكريبت 2022).
- **تشغيل يدوي:** `@festival on Christmas2022`.
- **محتاج:** سيرفر فيه `Base\Christmas2022\Christmas2022Ui.cs` وسطر الهوك في `LuaPush.cs`.
- **تنبيه:** المواعيد بتتداخل كل سنة مع Christmas2023 وVillageRequests وTwinklingShard، والزراير الـ lua بتقع في نفس المكان فوق. الرسمي إن كل واحد كان في سنته.

## بيتلعب إزاي
1. لما المهرجان يشتغل بيظهر الزرار الرسمي `DlgChristmasBtn` ("Xmas Miracle") بأمر الكلاينت نفسه `DlgChristmas_OpenMainBtn`.
2. الدوسة بتروح 10153 `ChrismasFactory_OpenMainButton`، وبيفتح الشباك الرسمي `DlgChristmasMain` (Xmas Miracle Factory):
   - عدد **Xmas Wishing Star** (3600441) من الشنطة: داتا حقيقية.
   - حالة البداية الرسمية: خطوط الإنتاج التلاتة عطلانة (رسالة MainOpenTips)، مفيش هدايا، الدب لفل 0.

## الرسمي والمشتق
- **رسمي:** الزرار والشباك وشكل الداتا (`luaui\dialog\dlgchristmas`، `luagui\1323.dat`)، الـ msgId 10153، رقم العملة، الرسايل MsgBringOut1.
- **مشتق:** MakeState = 0 (أول إنتاج مجاني زي الـ guide)، ورسايل "مش متاح على السيرفر" بالإنجليزي.

## مش موجود (رسالة نظام، مفيش أي جايزة)
تصليح الخطوط، إنتاج الهدايا، Bear Wish، Miracle Delivery، الترتيب، Snowman Kingdom كروس سيرفر، طرق جمع النجوم. مفيش مهام Questinfo ولا خرايط.
