# BruceLeePreheat — تمهيد صحوة محاربي التنين (Beast Scroll)

**المصدر الرسمي:** الكلاينت `luaui\dialog\dlgawakenbrucelee` (سكربت 2022-10-08) + `luaUiRes[1300]` + `Server_Key` (`tXiaoLongPreheat_DelText`, `tLongPreheating_DelText`).

## اللي شغال
- **الزرار:** `DlgAwakenBruceLeeBtn` — `icon_show` = `DlgAwakenBruceLee_OpenMainBtn` (من غير عداد)، `icon_hide` = `LuaUIMgr_HideDialog`.
- **الضغطة:** msg `10127` `XiaoLongPreheat_MainView` → النافذة الرسمية **Beast Scroll** (`DlgAwakenBruceLeeEntrance`).
- **الوحوش:** الوحش رقم i (Dragon، Kunpeng، Suanni) بيتفتح من يوم i في المهرجان. **مشتق** من النصوص الرسمية "unlocked tomorrow / the day after tomorrow"؛ يوم 1 = يوم ما المهرجان اشتغل على السيرفر (لو السيرفر اترستر، العد بيبدأ من جديد).
- **هدية الـ7 أيام:** بتفضل زي ما هي (مفيش حاجة تتاخد) — الجوايز مكتوبة في الكلاينت بس الأيتمز bound/بوقت وقواعد التسجيل على السيرفر، فمفيش تسليم.
- لفل أقل من 80 → "Please upgrade to level 80 first." (رسمي).

## مش موجود على السيرفر (رسالة مهذبة، مفيش جوايز ولا خصم)
- Beast Trials (Wind Plain / Sea of Sinking Ship / Zhenwu Temple)، استلام هدية الـ7 أيام، أمنيات Celestial Pool (Dragon Scales)، Peach Village.

## الجدول
- **manual**: الكلاينت مفيهوش تاريخ بداية/نهاية، بس "Peach Village will be accessible at 8:00 of Feb. 14". غالبًا الـ7 أيام اللي قبل 14 فبراير (2023) — مش مؤكد. التشغيل: `@festival on BruceLeePreheat`.

## الكود
- `Infinity [Sourse]\Base\BruceLeePreheat\BruceLeePreheatUi.cs` + سطر واحد في `LuaPush.HandleCallback` (msg 10127).
- كونفيج اختياري: `Infinity[DB]\BruceLeePreheat\bruceleepreheat.json` `{festival, minLevel}`.
