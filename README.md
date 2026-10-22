# Qahr Mobile — السيرفر

> **⚠️ تنويه: هذا المشروع تطوير خاص ومستقل لأغراض شخصية وتعليمية، ولا يتبع لعبة كونكر أونلاين الأصلية ولا شركة TQ Digital / NetDragon بأي شكل، ولا يمس سيرفراتها الرسمية أو لاعبيها أو حقوقها بأي صورة من الصور.**

سيرفر اللعبة وسيرفر الحسابات لـ **Qahr Online**: كونكر أونلاين موبايل، نسخة الكلاينت 1.1.2.3، والسيرفر مبني على سورس 7957.
الريبو ده نسخة كاملة زي ما هي من فولدر `D:\Qahr Mobile\Server`، ومعاها نسخة من قاعدة البيانات.

> ⚠️ **الريبو فيه كلمات سر حقيقية وملفات لاعيبة ولوجات، فلازم يفضل Private دايماً.**

---

## 1. محتويات الريبو

| المسار | المحتوى |
|---|---|
| `Server/Qahr.Mobile.sln` | ملف الـ Solution (.NET 10) |
| `Server/src/Qahr.Mobile.Game` | سورس سيرفر اللعبة، ومعاه `bin/` فيها السيرفر مبني وجاهز |
| `Server/src/Qahr.Mobile.Login` | سورس سيرفر الحسابات، وصفحات الشحن والـ WebHub، ومعاه `bin/Debug/` |
| `Server/src/Qahr.Mobile.Shared` | الكود المشترك، وملف الإعدادات المركزي `ServerConfig.ini` |
| `Server/Data` | داتا اللعبة كلها (حاجات، وحوش، خرايط، NPC، محلات، مهام، أحداث) وملفات اللاعيبة |
| `Server/scripts` | سكربتات التشغيل `run_live.ps1` و`run_cdn.ps1` |
| `Server/Tools` | أدوات السيرفر: MobileStudio وPatchConsole وOfficialMirror |
| `Server/libs` | مكتبات خارجية |
| `database/qahr_mobile.sql.gz` | نسخة كاملة من قاعدة MariaDB `qahr_mobile` |
| `extras/admin.py` | أداة الأدمن من سطر الأوامر (حفظ اللاعيبة، تليبورت، إعطاء حاجات، reload…) |
| `extras/requirements.txt` | مكتبات بايثون اللي أداة الـ CDN محتاجاها |

---

## 2. المطلوب على الجهاز

| البرنامج | النسخة | ملاحظات |
|---|---|---|
| **Windows** | 10 أو 11 أو Server 2019 وأحدث | السيرفر `net10.0-windows` |
| **.NET 10 SDK** | 10.0.x (اتجرب على 10.0.402) | من https://dotnet.microsoft.com/download/dotnet/10.0. لازم الـ SDK يبقى فيه **Windows Desktop Runtime** |
| **MariaDB** | 10.6 | ينفع MySQL 8 كمان |
| **Python** | 3.10 أو أحدث | محتاجه بس لأداة الأدمن وسيرفر الـ CDN |
| **Git** | أي نسخة | علشان تنسخ الريبو |

---

## 3. التشغيل خطوة بخطوة

### 3.1 انسخ الريبو

```powershell
git clone https://github.com/Hazem7575/Qahr-Mobile.git "D:\Qahr Mobile\Repo\Qahr-Mobile"
```

### 3.2 حط فولدر Server في مكانه

السيرفر متظبط إنه يشتغل من `D:\Qahr Mobile\Server`، وبيقرا الداتا من المسار `D:\conquer\QahrMobile\Data\`
(المسار ده مكتوب في `ServerConfig.ini` وفي `Server/src/Qahr.Mobile.Game/bin/shell.ini`).

```powershell
# 1) انسخ فولدر Server لمكانه
robocopy "D:\Qahr Mobile\Repo\Qahr-Mobile\Server" "D:\Qahr Mobile\Server" /E

# 2) اعمل الاختصار (Junction) اللي السيرفر بيقرا الداتا منه
mkdir D:\conquer
cmd /c mklink /J "D:\conquer\QahrMobile" "D:\Qahr Mobile\Server"
```

لو حابب تحطه في مكان تاني، غيّر المسارات دي:
- `ServerConfig.ini`: `[Server] DataFolder` و`CO2Folder` و`[AutoPatch] PatchDir`
- `Server/src/Qahr.Mobile.Game/bin/shell.ini`: `[Database] Location` و`CO2FOLDER`
- `Server/scripts/*.ps1`: المسارات اللي في أول كل ملف

### 3.3 قاعدة البيانات

```powershell
# فك ضغط النسخة
tar -xzf database\qahr_mobile.sql.gz      # أو: gzip -d -k database/qahr_mobile.sql.gz

# اعمل المستخدم (الاسم وكلمة السر نفس اللي في ServerConfig.ini [Database])
mysql -u root -p -e "CREATE USER 'hazem'@'localhost' IDENTIFIED BY '<نفس كلمة السر اللي في ServerConfig.ini>'; GRANT ALL ON qahr_mobile.* TO 'hazem'@'localhost'; FLUSH PRIVILEGES;"

# استرجع القاعدة (النسخة بتعمل قاعدة qahr_mobile لوحدها)
mysql -u root -p < database\qahr_mobile.sql
```

القاعدة فيها كل الجداول: الحسابات، والشخصيات، و`player_blobs` (الحاجات والبريد والمهارات…)، والمول (`recharge_packs` و`recharge_methods`)، والـ WebHub (`web_hub`)، وحالة الأحداث والكروت.

### 3.4 ملف الإعدادات `ServerConfig.ini`

الملف في `Server/src/Qahr.Mobile.Shared/ServerConfig.ini`، وسيرفر اللعبة وسيرفر الحسابات الاتنين بيقروه.

| القسم / المفتاح | المعنى |
|---|---|
| `[Server] IP` | **الـ IP اللي اللاعيبة بيدخلوا عليه** (حالياً `26.31.69.47`). غيّره لـ IP الجهاز الجديد |
| `[Server] GamePort` = 5828 | بورت سيرفر اللعبة |
| `[Server] LoginPort` = 16100 | بورت سيرفر الحسابات |
| `[Server] MobileRegisterPort` = 9548 | التسجيل، واسترجاع كلمة السر، وصفحة الشحن `/qahr/shop`، والـ WebHub `/qahr/hub` |
| `[AutoPatch] HttpPort` = 9549، و`AutoPatchPort` = 9547 | تحديثات اللعبة (PatchDir = فولدر التحديثات) |
| `[Database]` | الاتصال بـ MySQL |
| `[Mail]` | إيميل استرجاع كلمة السر (Gmail: `Host=smtp.gmail.com`، وكلمة سر تطبيق من حساب جوجل). لو فاضي، الخاصية مقفولة |
| `[AdminApi] AllowWrites` | يسمح لأداة الأدمن بأوامر الكتابة |

الاتنين دول لازم يتعدّلوا لما تغيّر الـ IP:
- `Server/src/Qahr.Mobile.Game/bin/shell.ini`: `[ServerInfo] AddresIP`
- `Server/Data/mobile_webpages.txt`: رابط صفحة الـ WebHub (فيه الـ IP)

### 3.5 البناء (Build)

ملفات `bin/` الموجودة في الريبو مبنية وجاهزة. لو عدّلت في الكود:

```powershell
cd "D:\Qahr Mobile\Server"
dotnet build src\Qahr.Mobile.Game\Qahr.Mobile.Game.csproj -c Debug
dotnet build src\Qahr.Mobile.Login\Qahr.Mobile.Login.csproj
```

- سيرفر اللعبة بيطلع في `src\Qahr.Mobile.Game\bin\Qahr.Mobile.Game.exe`
- سيرفر الحسابات بيطلع في `src\Qahr.Mobile.Login\bin\Debug\Qahr.Mobile.Login.exe`

### 3.6 التشغيل

بالترتيب ده:

```powershell
# 1) سيرفر الحسابات (Login + التسجيل + صفحة الشحن + WebHub + التحديثات)
Start-Process "D:\Qahr Mobile\Server\src\Qahr.Mobile.Login\bin\Debug\Qahr.Mobile.Login.exe" -WorkingDirectory "D:\Qahr Mobile\Server\src\Qahr.Mobile.Login\bin\Debug"

# 2) سيرفر اللعبة: وضع التطوير (أي تعديل في الكود بيتطبق وهو شغال)
powershell -ExecutionPolicy Bypass -File "D:\Qahr Mobile\Server\scripts\run_live.ps1"
#    أو شغّل الـ exe مباشرة:
#    Start-Process "D:\Qahr Mobile\Server\src\Qahr.Mobile.Game\bin\Qahr.Mobile.Game.exe" -WorkingDirectory "D:\Qahr Mobile\Server\src\Qahr.Mobile.Game\bin"

# 3) سيرفر ملفات اللعبة CDN (بورت 9550)
powershell -ExecutionPolicy Bypass -File "D:\Qahr Mobile\Server\scripts\run_cdn.ps1"
```

- `run_live.ps1` بيحفظ اللاعيبة، ويقفل السيرفر القديم، ويبني، ويشغّل بـ `dotnet watch`. اللوجات في `D:\Qahr Mobile\Logs\watch.log`.
- لو عايز تقفل الـ CDN: `run_cdn.ps1 -Stop`. ولو عايز تشوف حالته: `run_cdn.ps1 -Status`.

### 3.7 البورتات اللي لازم تتفتح في الفايروول

| البورت | الخدمة |
|---|---|
| 5828 | سيرفر اللعبة |
| 16100 | سيرفر الحسابات |
| 9547 و9549 | التحديثات |
| 9548 | التسجيل، وصفحة الشحن، والـ WebHub |
| 9550 | ملفات اللعبة (CDN) |
| 5897 | أداة الأدمن (محلي بس، **ما تفتحوش للنت**) |

```powershell
New-NetFirewallRule -DisplayName "Qahr Mobile" -Direction Inbound -Protocol TCP -LocalPort 5828,16100,9547,9548,9549,9550 -Action Allow
```

---

## 4. حاجات مش جوه الريبو ومطلوبة

| الحاجة | ليه مش موجودة | تجيبها إزاي |
|---|---|---|
| **CDN** (حوالي 3 جيجا، ملفات اللعبة الرسمية اللي الموبايل بينزلها أول مرة) | حجمها كبير على GitHub، وهي ملفات TQ الرسمية | `python -I Server\Tools\OfficialMirror\official_mirror.py cdn-fetch --threads 8` بينزلها في `D:\Qahr Mobile\CDN` (محتاج `pip install -r extras\requirements.txt`) |
| **فولدر التحديثات** `D:\Qahr Mobile\Updates` | فيه ملفات أكبر من 100 ميجا | `official_mirror.py fetch` و`republish`، أو من PatchConsole |
| **APK اللعبة ومشروع الكلاينت** (ClientSource) | حجمه 3.2 جيجا، والـ APK أكبر من حد GitHub | الكلاينت الحالي متظبط على IP `26.31.69.47`. لو غيّرت الـ IP، لازم الـ APK يتبني تاني من ClientSource بالـ IP الجديد |

---

## 5. أداة الأدمن

`extras/admin.py` بتكلم سيرفر اللعبة على `127.0.0.1`، والسر والبورت بتاخدهم من `Server/Data/adminapi.txt`.
حطها في `D:\conquer\_private\comobile\tools\admin.py`، لأن `run_live.ps1` بيدوّر عليها هناك وبيشغّلها بـ `D:\conquer\_private\comobile\venv\Scripts\python.exe`. أو عدّل المسارين اللي في `run_live.ps1`.

```powershell
python -I extras\admin.py online
python -I extras\admin.py save all=1
python -I extras\admin.py teleport name=test22 map=1002 x=430 y=378
python -I extras\admin.py reloadnpcs        # reloadshops / reloaditems / reloadmonsters / reloadall
```

---

## 6. ملفات مهمة للتعديل من غير ما تلمس الكود

| الملف أو الجدول | يتحكم في |
|---|---|
| `Data/Npcs.txt` | أماكن الـ NPC (بعدها `reloadnpcs`) |
| `Data/new_shop_goods.txt` | حاجات المول (بعدها `reloadshops`) |
| `Data/mobile_dailygift.txt` | Daily Packs |
| `Data/mobile_weekcard.txt` | الكروت الأسبوعية والشهرية |
| `Data/swapshop_boxes.txt` | محتوى صناديق Swap Shop |
| `Data/mobile_webpages.txt` | رابط صفحة الـ WebHub |
| جدول `recharge_packs` و`recharge_methods` | باقات الشحن وطرق الدفع |
| جدول `web_hub` | أخبار صفحة الـ WebHub |
