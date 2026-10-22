# Qahr Mobile — السيرفر

> **⚠️ تنويه: هذا المشروع تطوير خاص ومستقل لأغراض شخصية وتعليمية، ولا يتبع لعبة كونكر أونلاين الأصلية ولا شركة TQ Digital / NetDragon بأي شكل، ولا يمس سيرفراتها الرسمية أو لاعبيها أو حقوقها بأي صورة من الصور.**

سيرفر اللعبة وسيرفر الحسابات لـ **Qahr Online**: كونكر أونلاين موبايل، نسخة الكلاينت 1.1.2.3، والسيرفر مبني على سورس 7957.

> ⚠️ **الريبو فيه كلمات سر حقيقية وملفات لاعيبة، فلازم يفضل Private دايماً.**

---

## محتويات الريبو

| الفولدر أو الملف | فيه إيه |
|---|---|
| `Server/Qahr.Mobile.sln` | ملف المشروع، بتفتحه بالـ Visual Studio |
| `Server/src/Qahr.Mobile.Game` | سيرفر اللعبة |
| `Server/src/Qahr.Mobile.Login` | سيرفر الحسابات، وصفحة الشحن، وصفحة الـ WebHub |
| `Server/src/Qahr.Mobile.Shared/ServerConfig.ini` | **ملف الإعدادات الرئيسي** |
| `Server/Data` | داتا اللعبة كلها وملفات اللاعيبة |
| `database/qahr_mobile.sql.gz` | قاعدة البيانات |
| `Server/Tools` و`extras` | أدوات مساعدة (مش لازمة علشان السيرفر يشتغل) |

---

# طريقة التركيب

## الخطوة 1: نزّل البرامج دي

| البرنامج | تجيبه منين |
|---|---|
| **Visual Studio 2022 أو 2026** (Community مجاني) | https://visualstudio.microsoft.com. وأنت بتسطّبه، علّم على **.NET desktop development** |
| **.NET 10 SDK** | https://dotnet.microsoft.com/download/dotnet/10.0. اختار **SDK x64 لويندوز** |
| **MySQL** أو **MariaDB** | MariaDB 10.6 من https://mariadb.org/download، أو أي MySQL عندك |
| **Navicat** | علشان تفتح بيه قاعدة البيانات |
| **WinRAR** أو **7-Zip** | علشان تفك ضغط ملف القاعدة |

---

## الخطوة 2: نزّل السورس

1. افتح صفحة الريبو على GitHub.
2. اضغط على الزرار الأخضر **Code**، وبعدين **Download ZIP**.
3. فك الضغط في أي مكان، مثلاً: `D:\Qahr Mobile`. هيبقى عندك فولدر اسمه `Server` جواه.

> المكان اللي اختاره صاحب السيرفر هو `D:\Qahr Mobile\Server`. لو حطيته في نفس المكان ده بالظبط، هتعدي الخطوة 4 أسرع.

---

## الخطوة 3: قاعدة البيانات بالـ Navicat

1. **فك الضغط:** روح على فولدر `database`، ودوس كليك يمين على `qahr_mobile.sql.gz` واختار **Extract Here** (بالـ WinRAR أو 7-Zip). هيطلعلك ملف اسمه `qahr_mobile.sql`.
2. **الاتصال:** افتح **Navicat**، واضغط **Connection** ← **MySQL** (أو MariaDB)، واكتب:
   - Host: `localhost`
   - Port: `3306`
   - User name: `root`
   - Password: كلمة سر الـ root اللي اخترتها وأنت بتسطّب MySQL

   وبعدين **OK**.
3. **فتح الاتصال:** دوس دبل كليك على الاتصال اللي عملته علشان يفتح.
4. **استيراد القاعدة:** دوس **كليك يمين على اسم الاتصال** ← **Execute SQL File...**
   - اختار ملف `qahr_mobile.sql`.
   - اضغط **Start**، واستنى لحد ما يكتب **Finished successfully**.
5. **التأكد:** دوس كليك يمين على الاتصال ← **Refresh**. هتلاقي قاعدة اسمها **`qahr_mobile`** ظهرت وفيها الجداول.

> القاعدة فيها الحسابات، والشخصيات، وحاجات اللاعيبة، وباقات الشحن، وأخبار الـ WebHub.

---

## الخطوة 4: ظبط ملف الإعدادات (بالـ Notepad)

### الملف الأول: `Server\src\Qahr.Mobile.Shared\ServerConfig.ini`

دوس عليه كليك يمين ← **Open with** ← **Notepad**، وغيّر السطور دي بس:

**قسم `[Database]`**، بيانات الـ MySQL بتاعتك:
```ini
User=root
Password=كلمة سر الـ root بتاعتك
Name=qahr_mobile
```

**قسم `[Server]`**:
```ini
IP=اكتب هنا الـ IP اللي اللاعيبة هيدخلوا عليه
DataFolder=D:\Qahr Mobile\Server\Data\
CO2Folder=D:\Qahr Mobile\Server\Data\
```
- **IP:** لو بتجرب على نفس الجهاز اكتب `127.0.0.1`. لو على راوتر البيت اكتب IP الجهاز (هتلاقيه في إعدادات الشبكة). لو على VPS اكتب IP السيرفر.
- **DataFolder وCO2Folder:** مسار فولدر `Data` عندك، **ولازم ينتهي بـ `\`**. لو حطيت السيرفر في مكان تاني، اكتب مكانه أنت.

**قسم `[AutoPatch]`**:
```ini
PatchDir=D:\Qahr Mobile\Updates
```
اعمل فولدر فاضي بالاسم ده (أو أي اسم، واكتب مساره هنا).

احفظ الملف (**Ctrl + S**).

### الملف التاني: `Server\src\Qahr.Mobile.Game\bin\shell.ini`

افتحه بالـ Notepad وغيّر:
```ini
[ServerInfo]
AddresIP=نفس الـ IP اللي كتبته فوق

[Database]
Location=D:\Qahr Mobile\Server\Data\
CO2FOLDER=D:\Qahr Mobile\Server\Data\
```
احفظ.

### الملف التالت: `Server\Data\mobile_webpages.txt`

افتحه بالـ Notepad، وغيّر الـ IP اللي في الرابط للـ IP بتاعك، واحفظ:
```
2 http://IP_بتاعك:9548/qahr/hub?uid={uid}
```

---

## الخطوة 5: افتح المشروع في Visual Studio

1. دوس دبل كليك على **`Server\Qahr.Mobile.sln`**، هيفتح في Visual Studio.
2. **البناء:** من فوق اضغط **Build** ← **Build Solution** (أو **Ctrl + Shift + B**)، واستنى لحد ما يكتب تحت **Build succeeded**.
   - لو قالك إن .NET 10 مش موجود، سطّب الـ **.NET 10 SDK** من الخطوة 1 وافتح الـ Visual Studio تاني.

---

## الخطوة 6: شغّل السيرفر

لازم تشغّل **سيرفر الحسابات الأول**، وبعده **سيرفر اللعبة**.

### الطريقة الأولى: من غير Visual Studio (الأسهل)

1. **سيرفر الحسابات:** افتح فولدر `Server\src\Qahr.Mobile.Login\bin\Debug`، ودوس دبل كليك على **`Qahr.Mobile.Login.exe`**.
2. **سيرفر اللعبة:** افتح فولدر `Server\src\Qahr.Mobile.Game\bin`، ودوس دبل كليك على **`Qahr.Mobile.Game.exe`**.
3. **اتأكد إنه اشتغل:** استنى لحد ما شاشة سيرفر اللعبة تخلص تحميل وتلاقي فيها `listening on port 5828`.

### الطريقة التانية: من Visual Studio (لو بتعدّل في الكود)

1. في نافذة **Solution Explorer**، دوس كليك يمين على **Qahr.Mobile.Login** ← **Debug** ← **Start New Instance**.
2. بعده، دوس كليك يمين على **Qahr.Mobile.Game** ← **Debug** ← **Start New Instance**.

> **مهم:** متقفلش شاشة سيرفر اللعبة بالـ X وفيه لاعيبة جوه. ابعت أمر الحفظ الأول (`@save`) أو استنى الحفظ التلقائي.

---

## الخطوة 7: افتح البورتات في الفايروول (لو اللاعيبة من برا الجهاز)

1. افتح **Start** واكتب **Windows Defender Firewall with Advanced Security** وافتحه.
2. اضغط على **Inbound Rules** من الشمال، وبعدين **New Rule...** من اليمين.
3. اختار **Port** ← **Next** ← **TCP**، وفي خانة **Specific local ports** اكتب:
   ```
   5828, 16100, 9547, 9548, 9549, 9550
   ```
4. **Next** ← **Allow the connection** ← **Next** ← علّم على الكل ← **Next**، وسمّيها **Qahr Mobile** ← **Finish**.

| البورت | بتاع إيه |
|---|---|
| 5828 | سيرفر اللعبة |
| 16100 | سيرفر الحسابات |
| 9548 | التسجيل، وصفحة الشحن، والـ WebHub |
| 9547 و9549 | التحديثات |
| 9550 | ملفات اللعبة (CDN) |

> لو السيرفر على راوتر بيت، لازم كمان تعمل **Port Forwarding** للبورتات دي من صفحة الراوتر لـ IP الجهاز.

---

## حاجات مهمة لازم تعرفها

- **الـ APK (اللعبة على الموبايل):** النسخة الحالية متظبطة على الـ IP **`26.31.69.47`**. لو السيرفر بقى على IP تاني، لازم الـ APK يتبني تاني بالـ IP الجديد. مشروع الكلاينت (ClientSource) مش في الريبو ده لأن حجمه كبير.
- **ملفات اللعبة اللي الموبايل بينزلها أول مرة (CDN):** حجمها حوالي 3 جيجا، ومش في الريبو. بتتنزل بأداة `Server\Tools\OfficialMirror`، وبتشتغل على بورت 9550.
- **إيميل استرجاع كلمة السر:** من قسم `[Mail]` في `ServerConfig.ini`. لو استخدمت Gmail، حط `Host=smtp.gmail.com` وإيميلك، و**كلمة سر تطبيق** من إعدادات حساب جوجل (مش كلمة سر الإيميل العادية). لو سبته فاضي، الخاصية دي هتبقى مقفولة بس.

---

## تعديل اللعبة من غير ما تلمس الكود

| عايز تغيّر إيه | فين |
|---|---|
| أماكن الـ NPC | `Server\Data\Npcs.txt` |
| حاجات المول | `Server\Data\new_shop_goods.txt` |
| Daily Packs | `Server\Data\mobile_dailygift.txt` |
| الكروت الأسبوعية والشهرية | `Server\Data\mobile_weekcard.txt` |
| محتوى صناديق Swap Shop | `Server\Data\swapshop_boxes.txt` |
| باقات الشحن وأسعارها وصورها | جدول **`recharge_packs`** في Navicat |
| طرق الدفع | جدول **`recharge_methods`** في Navicat |
| أخبار صفحة الـ WebHub | جدول **`web_hub`** في Navicat |

في **Navicat**: افتح قاعدة `qahr_mobile` ← **Tables**، ودوس دبل كليك على الجدول. عدّل في الخانة على طول، واضغط على علامة **✓** تحت علشان تحفظ. التعديلات دي بتظهر في اللعبة على طول، من غير ما تقفل السيرفر.
