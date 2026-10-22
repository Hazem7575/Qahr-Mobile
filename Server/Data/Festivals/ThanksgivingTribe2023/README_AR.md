# عيد الشكر 2023 - Tribe Fight (ThanksgivingTribe2023)

**الحالة:** `enabled: true`، و`yearly` من 11-21 لـ 12-04. دي المواعيد الرسمية: luaUiRes_EN[1445].txt_time2 = "Nov. 21 - Dec. 4"، وActBtnConfig[14] = 2023-11-21 لـ 2023-12-04 (ERP561753).
- **تشغيل يدوي:** `@festival on ThanksgivingTribe2023`.
- **محتاج:** سيرفر فيه `Base\ThanksgivingTribe2023\ThanksgivingTribe2023Ui.cs`، وسطر الهوك في `LuaPush.cs` (msg 10319).
- **تنبيه:** المواعيد بتتداخل مع `Thanksgiving2024` لو الاتنين `enabled` بمواعيد سنوية. الرسمي إن كل واحد كان في سنة لوحده. الزرارين مختلفين (ده lua، وبتاع 2024 Fgui 10486).

## بيتلعب إزاي (لفل 80+)
1. لما المهرجان يشتغل، بيظهر الزرار الرسمي `DlgThanksgivingTribeMainBtn` (Thanksgiving Feast) بأمر الكلاينت نفسه `DlgThanksgivingTribeMainBtn_OpenMainBtn`.
2. لما تدوس عليه، بيروح 10319 `Thanksgiving2023Richman_OpenView`، وده بيفتح الشباك الرسمي `DlgThanksgivingTribeMainUi` (Tribe Fight). الشباك بيعرض:
   - عدد **Hunting Crystal** (3346119) و**Tribe Gold Coin** (3346120) و**Tribe Silver Coin** (3346121) و**Treasure Compass** (3346122) اللي في الشنطة.
   - عدد **Stellar Runestone** (3600664) و**Lunar Runestone** (3346118).
   - **Hunting Points = 0**، ونقط القبيلتين (Dusk وDawn) = 0، لأن محدش سلّم كريستال على السيرفر ده.
   - العد التنازلي لحد نهاية المهرجان.

## الرسمي والمشتق
**رسمي:**
- الزرار والشباك وشكل الداتا (`luaui\dialog\thanksgiving23`، و`luagui\1445.dat`).
- الـ msgId 10319 (`tMsg_Info["Thanksgiving23"]`).
- العملات من `Dialog_Info["1445-MapData"].PropData` وitemtype.
- الشرط: ActBtnConfig[14] NeedLevel = 80.

**مشتق:**
- رسايل "مش متاح على السيرفر".
- رسالة النهاية.

## مش موجود (رسالة نظام، ومفيش أي حاجة بتتاخد أو بتتدي)
- رمي الـ Runestones والتحرك على اللوحة: جوايز الخانات على السيرفر الرسمي بس.
- تسليم Hunting Crystals للقبيلة.
- Tribe Treasure Hunt.
- Exchange Store (Timo).
- Point Reward.
- Rankings.
- Arena (CS).

مفيش مهام في Questinfo، والخرايط مش معروفة ومش موجودة في maps.txt.
