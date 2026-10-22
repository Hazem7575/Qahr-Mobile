r"""Official update mirror for Conquer Online Mobile 1.1.2.3 (keep TQ's updates on our own servers).

  python -I official_mirror.py <command> [options] [--json]

Commands
  check                 ask the official autopatch servers (TCP 9547) what comes after our version; no download
  fetch                 follow the official UPDATE chain and download every patch zip we do not have yet
                        (--from V start version, --deep also every overlapping range zip, --dry-run)
  analyse [NAME|all]    list a downloaded patch, compare it with our client and our patches, write reports\NAME.md
  republish NAME        build our own patch from an official one into the AccServer PatchDir
                        (--as N our version number, --exclude/--include, --overrides DIR, --cdn-url URL, --dry-run)
  cdn-fetch             mirror the first-start resource set (2.8 GB) from the official CDN (resume, CRC check)
                        (--only-index, --limit N, --threads N)
  cdn-verify            check every mirrored CDN file against the official CRC list
  cdn-serve             serve the CDN mirror over HTTP (GET/HEAD/Range) (--port 9550, --bind 0.0.0.0)
  status                one summary of everything above

Every command takes --json: the result is printed as one JSON object on the last line of stdout (for the
console). Paths: --root (default D:\Qahr Mobile\CDN), --patchdir (AccServer PatchDir),
--client (the stock APK resources). Details: D:\conquer\conquer-mobile\docs\OFFICIAL_MIRROR.md
"""
import argparse, concurrent.futures, datetime, hashlib, http.server, io, json, os, re, shutil, socket, socketserver
import sys, threading, time, urllib.error, urllib.parse, urllib.request, zipfile, zlib

# ------------------------------------------------------------------------------------------------- defaults
QAHR = r'D:\conquer\QahrMobile'
DEF_ROOT = r'D:\Qahr Mobile\CDN'
DEF_PATCHDIR = r'D:\Qahr Mobile\Updates'
COMOBILE = r'D:\conquer\_private\comobile'
DEF_CLIENT_APKS = [os.path.join(COMOBILE, 'work', n) for n in ('com.Tq.CQ2ClientAndroid.apk', 'install_time_1001.apk')]
BASE_VERSION = 5150                     # version.dat of the 1.1.2.3 APK

# assets/ConquerRes/autoupdate/ini/autoupdate.ini of the stock APK (184.175.115.38 no longer answers)
OFFICIAL_PATCH_SERVERS = [('8.208.98.42', 9547), ('47.253.102.27', 9547), ('184.175.115.38', 9547)]
# The UPDATE answers name one of these hosts; they serve the same files (same ETag), so any of them is a mirror.
OFFICIAL_PATCH_HOSTS = ['updata2.yddzf.99.com', 'update1.yddzf.99.com', '114.119.190.108']

# assets/ConquerRes/ini/autocheckc3.ini of the stock APK (the first-start resource downloader)
CDN_CHECK = ['http://updata2.yddzf.99.com/ydzf/android/zf/check/nextcheck',     # Verify_URL_CDN
             'http://update1.yddzf.99.com/ydzf/android/zf/check/nextcheck']     # Verify_URL
CDN_DATA = ['http://updata2.yddzf.99.com/ydzf/android/zf/data/nextdata',        # Data_URL_CDN
            'http://update1.yddzf.99.com/ydzf/android/zf/data/nextdata']        # Data_URL
CDN_PREFIX = 'ydzf/android/zf'          # the mirror keeps the official path layout under cdn\
# checkdownloadlist.dat entry -> folder under data/nextdata/<ver>/
CDN_SUBDIR = {'.pak': '', '.ogg': 'sound/', '.wav': 'sound/', '.mp4': 'video/'}

# Files we own on the phone: an official patch must never overwrite them.
OWN_FILES = {
    'server.dat': 'our signed server list (build_apk.py)',
    'server_local.dat': 'our signed server list (build_apk.py)',
    'autoupdate/ini/autoupdate.ini': 'points the autopatch at our AccServer',
    'ini/recommend.ini': 'our server is the recommended one',
    'ini/autocheckc3.ini': 'CDN address (ours once the CDN mirror is live)',
    'version.dat': 'written by republish for our version line',
    'res.dat': 'resource category version (CDN)',
}
# Path patterns that usually mean work on the server side.
SERVER_HINTS = [
    (r'^ini/itemtype/|^ini/itemtype\d*\.ini$', 'items: new/changed item ids -> server itemtype.txt'),
    (r'^ini/monster(type)?\.(dat|ini)$', 'monsters -> server monster.txt / Monsters'),
    (r'^ini/npc(x)?\.ini$', 'NPC looks/names -> server Npcs.txt / mobile_npclooks.txt'),
    (r'^ini/magictype(ex|op)?\.dat$|^ini/magiceffect\.dat$', 'skills -> server MagicType.txt / magictypeex.txt'),
    (r'^ini/gamemap\.dat$|^ini/gamemapex\.ini$|^map/', 'maps -> server maps/GameMapEx.ini'),
    (r'^ini/questinfo|^ini/luacfg/quest|^ini/luacfg/mainstory', 'quests -> server Questinfo.ini / QuestEngine'),
    (r'^ini/(shop|honorshop|goldenleagueshop|joybeanshop|exchange_shop|new_shop|outerwear)', 'shops -> server shop data'),
    (r'^ini/battlepass|^ini/luacfg/battlepass', 'battle pass -> server battlepass_*.txt'),
    (r'^ini/.*_type\.dat$|^ini/.*_cfg\.dat$|^ini/.*_config\.dat$', 'config table also loaded by the server (check the twin .txt in Data)'),
    (r'^script/|\.lua$|^ini/luaui/|^ini/luacfg/', 'Lua UI/logic: may send new messages (CNetMsgLua) -> check the packet logs'),
    (r'^fgui/', 'UI package (FairyGUI): new windows may need new server packets'),
]
CATEGORIES = [(r'^ini/itemtype', 'items'), (r'^ini/luacfg/|^ini/luaui/|^script/|\.lua$', 'lua'),
              (r'^ini_ar/|^ini_sp/', 'language ini'), (r'^ini/', 'ini/data'), (r'^map/', 'maps'),
              (r'^fgui/', 'ui (fgui)'), (r'^ani/', 'ani'), (r'^sound/', 'sound'), (r'^video/', 'video'),
              (r'\.(tpd|tpi)$', 'packages (tpd/tpi)'), (r'^font/', 'fonts'), (r'^ZFCategroy', 'cdn category'),
              (r'^c3/|\.c3$', 'models (c3)'), (r'^data/', 'data')]

UA = 'Mozilla/5.0 (Linux; Android 9) libcurl'
JSON_OUT = False


def log(*a):
    if True:                    # progress lines; with --json the result is the last line
        print(*a, flush=True)


def now():
    return datetime.datetime.now().replace(microsecond=0).isoformat()


def human(n):
    n = float(n or 0)
    for u in ('B', 'KB', 'MB', 'GB'):
        if n < 1024 or u == 'GB':
            return ('%d %s' % (n, u)) if u == 'B' else ('%.1f %s' % (n, u))
        n /= 1024


def norm(p):
    return p.replace('\\', '/').lstrip('/').lower()


def sha256_file(path):
    h = hashlib.sha256()
    with open(path, 'rb') as f:
        for b in iter(lambda: f.read(1 << 20), b''):
            h.update(b)
    return h.hexdigest()


def crc32_file(path):
    c = 0
    with open(path, 'rb') as f:
        for b in iter(lambda: f.read(1 << 20), b''):
            c = zlib.crc32(b, c)
    return c & 0xffffffff


def load_json(path, default):
    try:
        with open(path, 'r', encoding='utf-8') as f:
            return json.load(f)
    except (OSError, ValueError):
        return default


def save_json(path, obj):
    os.makedirs(os.path.dirname(path), exist_ok=True)
    tmp = path + '.tmp'
    with open(tmp, 'w', encoding='utf-8') as f:
        json.dump(obj, f, ensure_ascii=False, indent=1)
    os.replace(tmp, path)


class Ctx:
    def __init__(self, a):
        self.root = a.root
        self.patchdir = a.patchdir
        self.client = a.client
        self.patches = os.path.join(self.root, 'patches')
        self.reports = os.path.join(self.root, 'reports')
        self.cdn = os.path.join(self.root, 'cdn')
        self.manifest_path = os.path.join(self.root, 'manifest.json')
        self.cdn_manifest_path = os.path.join(self.root, 'cdn_manifest.json')
        self.overrides = os.path.join(self.root, 'overrides')
        for d in (self.patches, self.reports, self.cdn, self.overrides):
            os.makedirs(d, exist_ok=True)

    def manifest(self):
        m = load_json(self.manifest_path, None) or {}
        m.setdefault('base_version', BASE_VERSION)
        m.setdefault('patches', [])
        m.setdefault('checks', [])
        m.setdefault('republished', [])
        return m

    def save_manifest(self, m):
        save_json(self.manifest_path, m)
        self.save_console_manifest(m)

    def save_console_manifest(self, m):
        """PatchDir\\_official\\manifest.json, the page "official updates" of PatchConsole reads it
        (format: conquer-mobile\\docs\\PATCH_CONSOLE.md)."""
        checks = m.get('checks') or []
        latest = latest_version(m)
        for c in reversed(checks):
            if c.get('steps'):
                ups = [r for r in c['steps'] if 'UPDATE' in r]
                if ups:
                    nxt = parse_update(ups[-1])[1]
                    latest = max(latest, int(nxt) if nxt.isdigit() else 0)
                break
        rep = {r['official']: r for r in m.get('republished', [])}
        rows = []
        for p in m['patches']:
            r = rep.get(p['name'])
            live = bool(r) and os.path.exists(os.path.join(self.patchdir, '%04d.zip' % r['version']))
            rows.append({'ours': r['version'] if r else None, 'official': p['name'], 'kind': 'official',
                         'size': r['size'] if r else p.get('size'), 'sha256': r['sha256'] if r else p.get('sha256'),
                         'sourceUrl': p.get('url'), 'created': r['time'] if r else p.get('fetched'),
                         'status': 'published' if live else 'removed' if r else
                         'analysed' if p.get('analysed') else 'fetched',
                         'officialSize': p.get('size'), 'officialSha256': p.get('sha256'), 'from': p.get('from'),
                         'to': p.get('to'), 'publishedByTq': p.get('last_modified'), 'files': p.get('file_count'),
                         'report': os.path.join(self.reports, p['name'] + '.md')})
        save_json(os.path.join(self.patchdir, '_official', 'manifest.json'), {
            'latestOfficial': str(latest), 'checkedAt': checks[-1]['time'] if checks else None,
            'source': ', '.join('%s:%d' % s for s in OFFICIAL_PATCH_SERVERS), 'mirrorRoot': self.root,
            'patches': rows})


# ---------------------------------------------------------------------------------------- version check (TCP)
def parse_update(reply):
    """'UPDATE host path/5151-5171.zip' -> (url, next_version) exactly like AutoUpdateMgr::GetUpdateUrl."""
    joined = re.sub(r'\s+', '/', reply.strip())
    url = 'http://' + joined[7:]
    dot = url.rfind('.')
    nxt = url[dot - 4:dot] if dot >= 4 else ''
    return url, nxt


def query_chain(host, port, version, timeout=8.0, rounds=50):
    """Talk to an official autopatch server like the client: send the version, follow UPDATE answers."""
    steps, seen = [], set()
    s = socket.create_connection((host, port), timeout=timeout)
    s.settimeout(timeout)
    try:
        ver = str(version)
        for _ in range(rounds):
            s.sendall(ver.encode('ascii'))
            r = s.recv(255).decode('latin1').strip()
            st = {'sent': ver, 'reply': r}
            steps.append(st)
            if 'UPDATE' in r:
                url, nxt = parse_update(r)
                st.update(kind='UPDATE', url=url, next=nxt)
                if url in seen or not nxt.isdigit():
                    break
                seen.add(url)
                ver = nxt
                continue
            st['kind'] = 'READY' if 'READY' in r else 'FAIL' if 'FAIL' in r else 'OTHER'
            break
    finally:
        s.close()
    return steps


def ask_official(version, servers=None):
    errors = []
    for host, port in servers or OFFICIAL_PATCH_SERVERS:
        try:
            return host, port, query_chain(host, port, version), errors
        except OSError as e:
            errors.append('%s:%d %s' % (host, port, e))
    return None, None, [], errors


def patch_name(url):
    return os.path.splitext(url.rsplit('/', 1)[-1])[0]


def mirrors_for(url):
    """The same path on every known official patch host (first = the one the server named)."""
    m = re.match(r'http://([^/]+)/(.*)$', url)
    if not m:
        return [url]
    out = [url]
    for h in OFFICIAL_PATCH_HOSTS:
        u = 'http://%s/%s' % (h, m.group(2))
        if u not in out:
            out.append(u)
    return out


# ----------------------------------------------------------------------------------------------- HTTP download
def http_head(url, timeout=20):
    req = urllib.request.Request(url, method='HEAD', headers={'User-Agent': UA})
    with urllib.request.urlopen(req, timeout=timeout) as r:
        h = r.headers
        return {'size': int(h.get('Content-Length') or -1), 'etag': (h.get('ETag') or '').strip('"'),
                'last_modified': h.get('Last-Modified') or ''}


def download(urls, dest, size=None, progress=None, timeout=60, tries=4):
    """Resumable GET into dest (dest.part while running). Tries every mirror. Returns the url used."""
    part = dest + '.part'
    os.makedirs(os.path.dirname(dest), exist_ok=True)
    last = None
    for attempt in range(tries):
        for url in urls:
            have = os.path.getsize(part) if os.path.exists(part) else 0
            if size is not None and have > size:
                os.remove(part)
                have = 0
            if size is not None and have == size:
                os.replace(part, dest)
                return url
            headers = {'User-Agent': UA}
            if have:
                headers['Range'] = 'bytes=%d-' % have
            try:
                req = urllib.request.Request(url, headers=headers)
                with urllib.request.urlopen(req, timeout=timeout) as r:
                    if have and r.status != 206:          # server ignored Range: start again
                        have = 0
                    with open(part, 'ab' if have else 'wb') as f:
                        while True:
                            b = r.read(1 << 18)
                            if not b:
                                break
                            f.write(b)
                            have += len(b)
                            if progress:
                                progress(have)
                if size is None or have == size:
                    os.replace(part, dest)
                    return url
                last = 'short read %d/%d' % (have, size)
            except (urllib.error.URLError, OSError) as e:
                last = '%s: %s' % (url, e)
                if isinstance(e, urllib.error.HTTPError) and e.code == 416 and size is not None:
                    try:
                        os.remove(part)
                    except OSError:
                        pass
        time.sleep(2 + attempt * 3)
    raise IOError('download failed: %s' % last)


def zip_listing(path):
    out = []
    with zipfile.ZipFile(path) as z:
        for i in z.infolist():
            if i.is_dir():
                continue
            out.append({'path': i.filename.replace('\\', '/'), 'size': i.file_size, 'crc': '%08x' % i.CRC})
    return out


# ------------------------------------------------------------------------------------------------ commands
def cmd_check(ctx, a):
    m = ctx.manifest()
    have = latest_version(m)
    start = a.version or have
    host, port, steps, errors = ask_official(start)
    res = {'asked': start, 'server': host and '%s:%d' % (host, port), 'errors': errors, 'steps': steps,
           'known_latest': have}
    if steps:
        last = steps[-1]
        ups = [s for s in steps if s.get('kind') == 'UPDATE']
        res['official_latest'] = ups[-1]['next'] if ups else start
        res['new_patches'] = [{'name': patch_name(s['url']), 'url': s['url']} for s in ups
                              if not find_patch(m, patch_name(s['url']))]
        res['final'] = last.get('kind')
    m['checks'] = (m['checks'] + [{'time': now(), 'asked': start, 'server': res['server'],
                                   'steps': [s['reply'] for s in steps], 'errors': errors}])[-50:]
    ctx.save_manifest(m)
    for s in steps:
        log('  %s -> %s' % (s['sent'], s['reply']))
    for e in errors:
        log('  no answer: ' + e)
    if steps:
        log('official latest: %s, new patches for us: %d' % (res['official_latest'], len(res['new_patches'])))
    return res


def latest_version(m):
    vs = [int(p['to']) for p in m['patches'] if str(p.get('to', '')).isdigit()]
    return max(vs) if vs else int(m.get('base_version', BASE_VERSION))


def find_patch(m, name):
    for p in m['patches']:
        if p['name'] == name:
            return p
    return None


def fetch_one(ctx, m, url, sent, dry=False):
    name = patch_name(url)
    p = find_patch(m, name)
    dest = os.path.join(ctx.patches, name + '.zip')
    if p and os.path.exists(dest) and os.path.getsize(dest) == p['size']:
        log('  %s already here (%s)' % (name, human(p['size'])))
        return p, False
    urls = mirrors_for(url)
    info = None
    for u in urls:
        try:
            info = http_head(u)
            break
        except (urllib.error.URLError, OSError) as e:
            log('  HEAD %s: %s' % (u, e))
    if not info or info['size'] <= 0:
        raise IOError('no mirror has ' + name)
    log('  %s  %s  (%s, %s)' % (name, url, human(info['size']), info['last_modified']))
    if dry:
        return {'name': name, 'url': url, 'size': info['size'], 'dry_run': True}, False
    t0, last = time.time(), [0]

    def prog(n):
        if time.time() - last[0] > 5:
            last[0] = time.time()
            log('    %s / %s' % (human(n), human(info['size'])))
    used = download(urls, dest, info['size'], prog)
    sha = sha256_file(dest)
    try:
        files = zip_listing(dest)
        with zipfile.ZipFile(dest) as z:
            bad = z.testzip()
        ok = bad is None
    except zipfile.BadZipFile as e:
        files, ok, bad = [], False, str(e)
    rng = name.split('-')
    p = {'name': name, 'from': int(sent), 'to': int(rng[-1]) if rng[-1].isdigit() else None,
         'range_first': int(rng[0]) if rng[0].isdigit() else None,
         'url': url, 'mirrors': urls, 'downloaded_from': used, 'size': info['size'], 'etag': info['etag'],
         'last_modified': info['last_modified'], 'fetched': now(), 'sha256': sha, 'zip_ok': ok,
         'zip_error': bad, 'file_count': len(files), 'unpacked_size': sum(f['size'] for f in files),
         'version_dat': read_version_dat(dest), 'files': files}
    m['patches'] = [x for x in m['patches'] if x['name'] != name] + [p]
    m['patches'].sort(key=lambda x: (x.get('to') or 0, x['name']))
    save_json(os.path.join(ctx.patches, name + '.json'), p)
    log('    done in %ds, sha256 %s..., %d files, zip %s' % (time.time() - t0, sha[:16], len(files),
                                                            'OK' if ok else 'BAD ' + str(bad)))
    return p, True


def read_version_dat(path):
    try:
        with zipfile.ZipFile(path) as z:
            for n in z.namelist():
                if norm(n) == 'version.dat':
                    return z.read(n).decode('latin1').strip()
    except (zipfile.BadZipFile, OSError):
        pass
    return None


def cmd_fetch(ctx, a):
    m = ctx.manifest()
    start = a.version or latest_version(m)
    host, port, steps, errors = ask_official(start)
    if not steps:
        raise SystemExit('no official autopatch server answered: %s' % '; '.join(errors))
    todo = [(s['url'], s['sent']) for s in steps if s.get('kind') == 'UPDATE']
    if a.deep:
        # every version between base and the official latest: the overlapping range zips as well
        top = int(todo[-1][0] and patch_name(todo[-1][0]).split('-')[-1]) if todo else start
        for v in range(int(m['base_version']), top):
            try:
                st = query_chain(host, port, v)
            except OSError:
                continue
            if st and st[0].get('kind') == 'UPDATE' and all(u != st[0]['url'] for u, _ in todo):
                todo.append((st[0]['url'], st[0]['sent']))
    got, new = [], 0
    for url, sent in todo:
        p, fresh = fetch_one(ctx, m, url, sent, a.dry_run)
        got.append({k: p.get(k) for k in ('name', 'from', 'to', 'size', 'sha256', 'file_count', 'zip_ok')})
        new += fresh
        if fresh:
            ctx.save_manifest(m)
    m['last_fetch'] = {'time': now(), 'asked': start, 'server': '%s:%d' % (host, port),
                       'chain': [s['reply'] for s in steps]}
    ctx.save_manifest(m)
    log('fetch: %d patch(es) in the chain, %d new' % (len(todo), new))
    return {'asked': start, 'chain': [s['reply'] for s in steps], 'patches': got, 'new': new}


def cmd_status(ctx, a):
    m = ctx.manifest()
    cm = load_json(ctx.cdn_manifest_path, {})
    ps = [{k: p.get(k) for k in ('name', 'from', 'to', 'size', 'fetched', 'last_modified', 'file_count', 'zip_ok')}
          | {'report': os.path.exists(os.path.join(ctx.reports, p['name'] + '.md'))} for p in m['patches']]
    disk = dir_size(ctx.patches)
    cdn = {'version': cm.get('version'), 'files': len(cm.get('files', {})),
           'have': sum(1 for f in cm.get('files', {}).values() if f.get('ok')),
           'total_size': cm.get('total_size'), 'disk': dir_size(ctx.cdn), 'updated': cm.get('updated')}
    res = {'root': ctx.root, 'base_version': m['base_version'], 'latest_official_fetched': latest_version(m),
           'patches': ps, 'patches_disk': disk, 'last_check': (m['checks'] or [None])[-1],
           'republished': m['republished'], 'cdn': cdn}
    log('official patches: %d (%s), latest %s' % (len(ps), human(disk), res['latest_official_fetched']))
    for p in ps:
        log('  %-12s %s -> %s  %10s  %s' % (p['name'], p['from'], p['to'], human(p['size']), p['last_modified']))
    for r in m['republished']:
        log('  republished %s as %s (%s)' % (r['official'], r['version'], r['time']))
    log('cdn: version %s, %s/%s files, %s on disk' % (cdn['version'], cdn['have'], cdn['files'], human(cdn['disk'])))
    return res


def dir_size(d):
    t = 0
    for dp, _, fn in os.walk(d):
        for f in fn:
            try:
                t += os.path.getsize(os.path.join(dp, f))
            except OSError:
                pass
    return t



# ------------------------------------------------------------------------------------------------- analyse
TQ_KEY = None


def tq_decrypt(d, seed=9527):
    """Mobile ini .dat (CMyEncryptFile, see comobile tools/tqdat.py)."""
    global TQ_KEY
    if TQ_KEY is None:
        k, s = bytearray(128), seed
        for i in range(128):
            s = (s * 214013 + 2531011) & 0xffffffff
            k[i] = (s >> 16) & 0xff
        TQ_KEY = bytes(k)
    o = bytearray(len(d))
    for i, b in enumerate(d):
        b ^= TQ_KEY[i & 0x7f]
        r = i & 7
        o[i] = ((b >> r) | (b << (8 - r))) & 0xff
    return bytes(o)


def as_text(name, data):
    """Plain text of a client table (.dat decrypted when it is encrypted). None = binary."""
    def printable(t):
        sample = t[:4096]
        return bool(sample) and sum(32 <= c < 127 or c in (9, 10, 13) or c >= 0x80 for c in sample) > 0.95 * len(sample)
    if printable(data):
        return data.decode('utf-8', 'replace')
    if name.endswith('.dat') and len(data) < 30_000_000:
        t = tq_decrypt(data)
        if printable(t):
            return t.decode('utf-8', 'replace')
    return None


class Client:
    """The stock 1.1.2.3 resources (assets/ConquerRes of the APK splits), read straight from the APKs."""
    def __init__(self, apks, cache):
        self.apks = [p for p in apks if os.path.exists(p)]
        stamp = [[p, os.path.getsize(p), int(os.path.getmtime(p))] for p in self.apks]
        c = load_json(cache, {})
        if c.get('stamp') != stamp:
            files = {}
            for p in self.apks:
                with zipfile.ZipFile(p) as z:
                    for i in z.infolist():
                        if i.filename.startswith('assets/ConquerRes/') and not i.is_dir():
                            files[norm(i.filename[len('assets/ConquerRes/'):])] = [p, i.filename, i.file_size,
                                                                                   '%08x' % i.CRC]
            c = {'stamp': stamp, 'files': files}
            save_json(cache, c)
        self.files = c['files']
        self._zips = {}

    def read(self, path):
        e = self.files.get(norm(path))
        if not e:
            return None
        z = self._zips.get(e[0]) or self._zips.setdefault(e[0], zipfile.ZipFile(e[0]))
        return z.read(e[1])


def our_patches(patchdir):
    """({path: [(version, crc, title, official)]}, [patch info]) for every NNNN.zip in the AccServer PatchDir."""
    out, vers = {}, []
    if not os.path.isdir(patchdir):
        return out, vers
    for f in sorted(os.listdir(patchdir)):
        m = re.match(r'^(\d{4})\.zip$', f)
        if not m:
            continue
        meta = load_json(os.path.join(patchdir, '_meta', m.group(1) + '.json'), {})
        title = meta.get('Title') or ''
        vers.append({'version': int(m.group(1)), 'title': title, 'official': meta.get('Official')})
        ours_files = {norm(x.get('Path', '')) for x in meta.get('Files', []) if x.get('Status') == 'ours'}
        try:
            with zipfile.ZipFile(os.path.join(patchdir, f)) as z:
                for i in z.infolist():
                    n = norm(i.filename)
                    if i.is_dir() or n == 'version.dat':
                        continue
                    official = bool(meta.get('Official')) and n not in ours_files
                    out.setdefault(n, []).append((int(m.group(1)), '%08x' % i.CRC, title, official))
        except zipfile.BadZipFile:
            pass
    return out, vers


def category(path):
    for rx, c in CATEGORIES:
        if re.search(rx, path):
            return c
    return 'other'


def ids_atat(text, fields=(0,)):
    out = {}
    for line in text.splitlines():
        parts = line.split('@@')
        if len(parts) > max(fields) and parts[0].strip().lstrip('-').isdigit():
            key = tuple(parts[i].strip() for i in fields)
            out[key if len(fields) > 1 else key[0]] = line.strip()
    return out


def ids_sections(text):
    out, cur, body = {}, None, []
    for line in text.splitlines():
        m = re.match(r'^\s*\[([^\]]+)\]', line)
        if m:
            if cur is not None:
                out[cur] = '\n'.join(body)
            k = m.group(1).strip()
            cur, body = (str(int(k)) if k.isdigit() else k), []
        elif cur is not None:
            body.append(line.strip())
    if cur is not None:
        out[cur] = '\n'.join(body)
    return out


def server_ids(kind):
    """Ids the game server already has (QahrMobile Data folder)."""
    d = os.path.join(QAHR, 'Data')

    def rd(*p):
        try:
            with open(os.path.join(d, *p), 'r', encoding='utf-8', errors='replace') as f:
                return f.read()
        except OSError:
            return ''
    if kind == 'item':
        return set(ids_atat(rd('itemtype.txt'))) | set(ids_atat(rd('ItemtypeSub.txt')))
    if kind == 'monster':
        return set(ids_sections(rd('monster.txt'))) | set(ids_atat(rd('monster.txt')))
    if kind == 'magic':
        return set(ids_atat(rd('MagicType.txt'), (1, 2)))
    if kind == 'map':
        s = set(ids_sections(rd('GameMapEx.ini')))
        if os.path.isdir(os.path.join(d, 'maps')):
            s |= {os.path.splitext(f)[0] for f in os.listdir(os.path.join(d, 'maps'))}
        return s
    return set()


# client table -> (kind, parser) for the "new ids" check
ID_TABLES = [
    (r'^ini/itemtype/[^/]+\.ini$|^ini/itemtype\d*\.ini$', 'item', lambda t: ids_atat(t)),
    (r'^ini/monstertype\.ini$', 'monster', lambda t: ids_atat(t)),
    (r'^ini/monster\.dat$', 'monster', lambda t: ids_sections(t)),
    (r'^ini/magictype\.dat$', 'magic', lambda t: ids_atat(t, (1, 2))),
    (r'^ini/gamemapex\.ini$', 'map', lambda t: ids_sections(t)),
    (r'^ini/npc\.ini$', 'npc look', lambda t: ids_sections(t)),
]
SERVER_KINDS = ('item', 'monster', 'magic', 'map')


def analyse_patch(ctx, client, ours, p):
    path = os.path.join(ctx.patches, p['name'] + '.zip')
    rows, conflicts, hints, tables = [], [], {}, []
    srv_cache = {}
    with zipfile.ZipFile(path) as z:
        for i in z.infolist():
            if i.is_dir():
                continue
            n = norm(i.filename)
            crc = '%08x' % i.CRC
            base = client.files.get(n)
            st = 'new' if not base else ('same' if base[3] == crc else 'changed')
            own = [x for x in ours.get(n, []) if not x[3]]
            r = {'path': i.filename.replace('\\', '/'), 'size': i.file_size, 'crc': crc, 'status': st,
                 'category': category(n), 'base_size': base[2] if base else None}
            if n in OWN_FILES or own:
                why = OWN_FILES.get(n) or 'changed by our patch %s' % ', '.join('%d (%s)' % (v, t) for v, _, t, _ in own)
                r['conflict'] = why
                conflicts.append({'path': r['path'], 'why': why, 'status': st})
            for rx, h in SERVER_HINTS:
                if st != 'same' and re.search(rx, n):
                    hints.setdefault(h, []).append(r['path'])
            for rx, kind, parse in ID_TABLES:
                if st == 'same' or not re.search(rx, n):
                    continue
                new_t = as_text(n, z.read(i))
                old_raw = client.read(n) if base else b''
                old_t = as_text(n, old_raw) if old_raw else ''
                if new_t is None:
                    continue
                a_new, a_old = parse(new_t), parse(old_t or '')
                added = sorted(set(a_new) - set(a_old), key=str)
                changed = sorted((k for k in set(a_new) & set(a_old) if a_new[k] != a_old[k]), key=str)
                removed = sorted(set(a_old) - set(a_new), key=str)
                missing = []
                if kind in SERVER_KINDS:
                    if kind not in srv_cache:
                        srv_cache[kind] = server_ids(kind)
                    missing = [k for k in added if k not in srv_cache[kind]]
                tables.append({'file': r['path'], 'kind': kind, 'added': len(added), 'changed': len(changed),
                               'removed': len(removed), 'added_ids': [fmt_id(k) for k in added[:400]],
                               'missing_on_server': [fmt_id(k) for k in missing[:400]],
                               'missing_count': len(missing),
                               'added_names': {fmt_id(k): row_name(a_new[k]) for k in added[:400]}})
            rows.append(r)
    cats = {}
    for r in rows:
        c = cats.setdefault(r['category'], {'new': 0, 'changed': 0, 'same': 0, 'size': 0})
        c[r['status']] += 1
        c['size'] += r['size']
    return {'name': p['name'], 'from': p.get('from'), 'to': p.get('to'), 'size': p.get('size'),
            'last_modified': p.get('last_modified'), 'version_dat': p.get('version_dat'),
            'files': len(rows), 'new': sum(r['status'] == 'new' for r in rows),
            'changed': sum(r['status'] == 'changed' for r in rows),
            'same': sum(r['status'] == 'same' for r in rows), 'categories': cats, 'conflicts': conflicts,
            'server_hints': hints, 'tables': tables, 'rows': rows}


def fmt_id(k):
    return '/'.join(k) if isinstance(k, tuple) else str(k)


def row_name(line):
    if '@@' in line:
        for x in line.split('@@')[1:5]:
            if x and not x.lstrip('-').isdigit():
                return x
        return ''
    m = re.search(r'(?m)^Name=(.*)$', line)
    return m.group(1).strip() if m else ''


def write_report(ctx, an, ours_versions):
    L = []
    w = L.append
    w('# Official patch %s' % an['name'])
    w('')
    w('- Official version line: %s -> %s (version.dat in the zip: %s)' % (an['from'], an['to'], an['version_dat']))
    w('- Published by TQ: %s, zip %s, %d files' % (an['last_modified'], human(an['size']), an['files']))
    w('- Compared with the stock 1.1.2.3 client (APK, version %d) and our patches: %s' % (
        BASE_VERSION, ', '.join('%d' % v['version'] for v in ours_versions) or 'none'))
    w('- Generated %s by official_mirror.py analyse' % now())
    w('')
    w('## Summary')
    w('')
    w('| category | new | changed | same as APK | size |')
    w('|---|---:|---:|---:|---:|')
    for c, v in sorted(an['categories'].items(), key=lambda x: -x[1]['size']):
        w('| %s | %d | %d | %d | %s |' % (c, v['new'], v['changed'], v['same'], human(v['size'])))
    w('')
    w('Total: %d new, %d changed, %d unchanged. A patch zip can only add or overwrite files, never remove.'
      % (an['new'], an['changed'], an['same']))
    w('')
    w('## Conflicts with our own files')
    w('')
    if an['conflicts']:
        w('`republish` leaves these out unless `--include <path>` is given:')
        w('')
        for c in an['conflicts']:
            w('- `%s` (%s): %s' % (c['path'], c['status'], c['why']))
    else:
        w('None.')
    w('')
    w('## Server-side work')
    w('')
    if not an['tables'] and not an['server_hints']:
        w('Nothing found.')
    for t in an['tables']:
        w('### %s (%s)' % (t['file'], t['kind']))
        w('')
        w('- rows added %d, changed %d, removed %d' % (t['added'], t['changed'], t['removed']))
        if t['kind'] in SERVER_KINDS:
            w('- **missing on our server: %d**' % t['missing_count'])
            if t['missing_on_server']:
                show = t['missing_on_server'][:80]
                w('  ' + ', '.join(('%s %s' % (k, t['added_names'].get(k, ''))).strip() for k in show)
                  + (' ...' if t['missing_count'] > len(show) else ''))
        elif t['added_ids']:
            w('- added: ' + ', '.join(('%s %s' % (k, t['added_names'].get(k, ''))).strip() for k in t['added_ids'][:60]))
        w('')
    if an['server_hints']:
        w('### Areas touched')
        w('')
        for h, paths in sorted(an['server_hints'].items(), key=lambda x: -len(x[1])):
            w('- %s: %d file(s), e.g. %s' % (h, len(paths), ', '.join('`%s`' % x for x in paths[:6])))
        w('')
    w('Protocol: a patch zip only carries resources. libCQ2ClientEn.so (packet ids, struct layouts) changes only')
    w('with a new APK. New Lua/fgui files can still use messages the server does not answer yet: test the new')
    w('windows and look for `[Mobile] no layout` lines in watch.log.')
    w('')
    w('## Files')
    w('')
    w('| status | path | size | APK size |')
    w('|---|---|---:|---:|')
    order = {'new': 0, 'changed': 1, 'same': 2}
    for r in sorted(an['rows'], key=lambda r: (order[r['status']], r['path'])):
        w('| %s%s | `%s` | %s | %s |' % (r['status'], ' **conflict**' if r.get('conflict') else '', r['path'],
                                         human(r['size']), human(r['base_size']) if r['base_size'] else ''))
    out = os.path.join(ctx.reports, an['name'] + '.md')
    with open(out, 'w', encoding='utf-8') as f:
        f.write('\n'.join(L) + '\n')
    return out


def cmd_analyse(ctx, a):
    m = ctx.manifest()
    names = [p['name'] for p in m['patches']] if a.name in (None, 'all') else [a.name]
    client = Client(a.client, os.path.join(ctx.root, 'client_index.json'))
    ours, vers = our_patches(ctx.patchdir)
    res = []
    for n in names:
        p = find_patch(m, n)
        if not p:
            raise SystemExit('unknown patch %s (fetch it first)' % n)
        an = analyse_patch(ctx, client, ours, p)
        rep = write_report(ctx, an, vers)
        save_json(os.path.join(ctx.reports, n + '.json'), an)
        p['analysed'] = now()
        log('%s: %d files (%d new, %d changed, %d same), %d conflicts -> %s' % (
            n, an['files'], an['new'], an['changed'], an['same'], len(an['conflicts']), rep))
        for t in an['tables']:
            log('   %s (%s): +%d ~%d -%d%s' % (t['file'], t['kind'], t['added'], t['changed'], t['removed'],
                                              (', %d missing on our server' % t['missing_count'])
                                              if t['kind'] in SERVER_KINDS else ''))
        short = {k: v for k, v in an.items() if k != 'rows'}
        short['report'] = rep
        res.append(short)
    ctx.save_manifest(m)
    return {'patches': res}



# ----------------------------------------------------------------------------------------------- republish
def match_any(path, pats):
    n = norm(path)
    for p in pats:
        if norm(p) == n or n.startswith(norm(p).rstrip('/') + '/'):
            return True
        try:
            if re.search(p, n, re.I):
                return True
        except re.error:
            pass
    return False


def cmd_republish(ctx, a):
    m = ctx.manifest()
    if not a.name:
        raise SystemExit('republish needs the official patch name, e.g. 5151-5171')
    p = find_patch(m, a.name)
    if not p:
        raise SystemExit('unknown patch %s (fetch it first)' % a.name)
    src = os.path.join(ctx.patches, p['name'] + '.zip')
    ver = int(a.as_version or p.get('to') or 0)
    if not 1000 <= ver <= 9998:
        raise SystemExit('bad version %r (4 digits; 9999 is reserved by the client)' % ver)
    existing = sorted(int(f[:4]) for f in os.listdir(ctx.patchdir) if re.match(r'^\d{4}\.zip$', f)) \
        if os.path.isdir(ctx.patchdir) else []
    if existing and ver <= existing[-1] and not (a.force and ver in existing):
        raise SystemExit('version %d is not above our latest patch %d (phones would never get it); use --as %d'
                         % (ver, existing[-1], existing[-1] + 1))
    dest = os.path.join(ctx.patchdir, '%04d.zip' % ver)
    if os.path.exists(dest) and not a.force:
        raise SystemExit('%s exists (use --force)' % dest)
    ours, _ = our_patches(ctx.patchdir)
    own = {n for n, l in ours.items() if any(not x[3] for x in l)}
    ovdir = a.overrides or ctx.overrides
    overrides = {}
    if os.path.isdir(ovdir):
        for dp, _, fn in os.walk(ovdir):
            for f in fn:
                full = os.path.join(dp, f)
                overrides[norm(os.path.relpath(full, ovdir))] = full
    if a.cdn_url:
        overrides['ini/autocheckc3.ini'] = None                      # generated below
    kept, skipped, files = [], [], []
    tmp = dest + '.tmp'
    zout = None
    if not a.dry_run:
        os.makedirs(ctx.patchdir, exist_ok=True)
        zout = zipfile.ZipFile(tmp, 'w', zipfile.ZIP_DEFLATED, compresslevel=6)
    try:
        with zipfile.ZipFile(src) as zin:
            for i in zin.infolist():
                if i.is_dir():
                    continue
                n = norm(i.filename)
                why = None
                if n == 'version.dat':
                    why = 'replaced by our version.dat'
                elif n in overrides:
                    why = 'replaced by overrides'
                elif match_any(n, a.exclude):
                    why = 'excluded (--exclude)'
                elif (n in OWN_FILES or n in own) and not match_any(n, a.include):
                    why = OWN_FILES.get(n) or 'our own patch changes it'
                if why:
                    skipped.append({'path': i.filename, 'why': why})
                    continue
                name = i.filename.replace('\\', '/')
                kept.append(name)
                files.append({'Path': name, 'Size': i.file_size, 'Sha256': '', 'Status': 'official'})
                if zout:
                    # stored entries stay stored (already compressed media), the rest deflated
                    zi = zipfile.ZipInfo(name, i.date_time)
                    zi.compress_type = i.compress_type if i.compress_type in (0, 8) else 8
                    with zin.open(i) as fi, zout.open(zi, 'w') as fo:
                        shutil.copyfileobj(fi, fo, 1 << 20)
        for n, full in sorted(overrides.items()):
            if full is None:
                data = autocheck_ini(a.cdn_url).encode('ascii')
            else:
                with open(full, 'rb') as f:
                    data = f.read()
            files.append({'Path': n, 'Size': len(data), 'Sha256': hashlib.sha256(data).hexdigest(), 'Status': 'ours'})
            if zout:
                zout.writestr(n, data)
        if zout:
            zout.writestr('version.dat', ('%04d' % ver).encode('ascii'))
            zout.close()
            zout = None
            with zipfile.ZipFile(tmp) as zt:
                if zt.testzip() is not None:
                    raise IOError('the new zip does not test OK')
            os.replace(tmp, dest)
    finally:
        if zout:
            zout.close()
        if os.path.exists(tmp):
            os.remove(tmp)
    res = {'official': p['name'], 'version': ver, 'zip': dest, 'kept': len(kept), 'skipped': skipped,
           'overrides': sorted(overrides), 'dry_run': a.dry_run}
    if not a.dry_run:
        size, sha = os.path.getsize(dest), sha256_file(dest)
        meta = {'Version': ver,
                'Title': a.title or ('Official update %s' % p['name']),
                'Notes': 'Official TQ patch %s (published %s) republished by official_mirror.py: %d official files, '
                         '%d left out (%s), %d of ours.' % (
                             p['name'], p.get('last_modified'), len(kept), len(skipped),
                             ', '.join(sorted({s['path'] for s in skipped}))[:300] or '-', len(overrides)),
                'Created': now(), 'Author': os.environ.get('USERNAME', ''), 'Sha256': sha, 'Size': size,
                'Files': files, 'Kind': 'official', 'Official': p['name']}
        save_json(os.path.join(ctx.patchdir, '_meta', '%04d.json' % ver), meta)
        try:
            with open(os.path.join(ctx.patchdir, '_meta', 'operations.log'), 'a', encoding='utf-8') as f:
                f.write('%s\tنشر رسمي\t%s\tالتحديث الرسمي %s اتنشر كـ %04d (%d ملف، %s)\n' % (
                    now().replace('T', ' '), meta['Author'], p['name'], ver, len(files), human(size)))
        except OSError:
            pass
        m['republished'] = [r for r in m['republished'] if r['version'] != ver] + [
            {'official': p['name'], 'version': ver, 'time': now(), 'size': size, 'sha256': sha,
             'skipped': [s['path'] for s in skipped]}]
        ctx.save_manifest(m)
        res.update(size=size, sha256=sha)
    log('%s -> %s: %d official files kept, %d skipped, %d ours%s' % (
        p['name'], dest, len(kept), len(skipped), len(overrides), ' (dry run)' if a.dry_run else ''))
    for s in skipped:
        log('   skipped %s: %s' % (s['path'], s['why']))
    return res


def autocheck_ini(base):
    base = base.rstrip('/')
    return ('[config]\r\nVerify_URL={0}/check/nextcheck\r\nVerify_URL_CDN={0}/check/nextcheck\r\n\r\n'
            'Data_URL={0}/data/nextdata\r\nData_URL_CDN={0}/data/nextdata\r\nCdnValid=1\r\n').format(base)



# ----------------------------------------------------------------------------------------------- CDN mirror
# The first-start downloader (CC3NetControlImpl / "Download module" in libCQ2ClientEn.so) reads
# ini/autocheckc3.ini and asks, under <Verify_URL>/ = check/nextcheck/ and <Data_URL>/ = data/nextdata/:
#   check/nextcheck/version.dat                     resource version (5150)
#   check/nextcheck/[<ver>/]checkdownloadlist.7z    every file: name[date][size][0xhash][ver]
#   data/nextdata/<ver>/ZFCategroy.7z               the category (ResCategroy.xml, Res_*.xml, pak_index.dat)
#   data/nextdata/<ver>/checkdownloadlist.7z        the pak subset
#   data/nextdata/<ver>/<name>.pak, sound/<name>.ogg|.wav, video/<name>.mp4
# The hash column is TQ's own 32-bit file hash (not CRC32/Adler); the client checks it itself. We check size,
# and keep a SHA-256 of what we stored.
CDN_INDEX = ['check/nextcheck/version.dat', 'check/nextcheck/checkdownloadlist.7z',
             'check/nextcheck/{v}/version.dat', 'check/nextcheck/{v}/checkdownloadlist.7z',
             'data/nextdata/{v}/ZFCategroy.7z', 'data/nextdata/{v}/checkdownloadlist.7z']


def cdn_local(ctx, rel):
    return os.path.join(ctx.cdn, *(CDN_PREFIX + '/' + rel).split('/'))


def http_get_small(urls, timeout=30):
    last = None
    for u in urls:
        try:
            with urllib.request.urlopen(urllib.request.Request(u, headers={'User-Agent': UA}), timeout=timeout) as r:
                return r.read(), u
        except (urllib.error.URLError, OSError) as e:
            last = '%s: %s' % (u, e)
    raise IOError(last)


def cdn_urls(rel):
    bases = CDN_CHECK if rel.startswith('check/') else CDN_DATA
    sub = rel.split('/', 2)[2]
    return ['%s/%s' % (b, urllib.parse.quote(sub)) for b in bases]     # a few sound names contain spaces


def read_7z_member(path, member_suffix):
    try:
        import py7zr
    except ImportError:
        raise SystemExit('py7zr is needed (pip install py7zr in the venv)')
    import tempfile
    out = {}
    with tempfile.TemporaryDirectory() as td:
        with py7zr.SevenZipFile(path) as z:
            z.extractall(td)
        for dp, _, fn in os.walk(td):
            for f in fn:
                if f.lower().endswith(member_suffix.lower()):
                    full = os.path.join(dp, f)
                    with open(full, 'rb') as fh:
                        out[os.path.relpath(full, td).replace('\\', '/')] = fh.read()
    return out


def parse_downloadlist(text):
    out = []
    for line in text.splitlines():
        m = re.match(r'^(.+?)\[(\d+)\]\[(\d+)\]\[(0x[0-9a-fA-F]+)\]\[(\d+)\]\s*$', line.strip())
        if m:
            out.append({'name': m.group(1), 'date': m.group(2), 'size': int(m.group(3)),
                        'hash': m.group(4).lower(), 'ver': m.group(5)})
    return out


def cdn_rel(e):
    ext = os.path.splitext(e['name'])[1].lower()
    return 'data/nextdata/%s/%s%s' % (e['ver'], CDN_SUBDIR.get(ext, ''), e['name'])


def cmd_cdn_fetch(ctx, a):
    cm = load_json(ctx.cdn_manifest_path, {}) or {}
    data, used = http_get_small(['%s/version.dat' % b for b in CDN_CHECK])
    ver = data.decode('latin1').strip()
    if not ver.isdigit():
        raise SystemExit('odd CDN version.dat: %r' % data[:20])
    log('official CDN resource version %s (%s)' % (ver, used))
    index = {}
    for t in CDN_INDEX:
        rel = t.format(v=ver)
        dest = cdn_local(ctx, rel)
        try:
            body, u = http_get_small(cdn_urls(rel))
        except IOError as e:
            log('  index %s: not there (%s)' % (rel, e))
            continue
        os.makedirs(os.path.dirname(dest), exist_ok=True)
        old = open(dest, 'rb').read() if os.path.exists(dest) else None
        with open(dest, 'wb') as f:
            f.write(body)
        index[rel] = {'size': len(body), 'sha256': hashlib.sha256(body).hexdigest(), 'changed': old != body}
        log('  index %-45s %9s%s' % (rel, human(len(body)), '  CHANGED' if old is not None and old != body else ''))
    lst = []
    for rel in ('check/nextcheck/%s/checkdownloadlist.7z' % ver, 'check/nextcheck/checkdownloadlist.7z'):
        if rel in index:
            for _, body in read_7z_member(cdn_local(ctx, rel), 'checkdownloadlist.dat').items():
                lst = parse_downloadlist(body.decode('latin1'))
            if lst:
                break
    if not lst:
        raise SystemExit('no checkdownloadlist from the CDN')
    entries = {cdn_rel(e): e for e in lst}
    # packs named only in the category xml (should be none)
    zrel = 'data/nextdata/%s/ZFCategroy.7z' % ver
    if zrel in index:
        for n, body in read_7z_member(cdn_local(ctx, zrel), '.xml').items():
            for u in re.findall(r'URL="([^"]+)"', body.decode('utf-8', 'replace')):
                r = 'data/nextdata/%s/%s' % (ver, u)
                if r not in entries:
                    entries[r] = {'name': u, 'size': None, 'hash': None, 'ver': ver, 'date': ''}
    files = cm.get('files', {})
    todo = []
    for rel, e in sorted(entries.items()):
        dest = cdn_local(ctx, rel)
        f = files.get(rel, {})
        ok = os.path.exists(dest) and e['size'] is not None and os.path.getsize(dest) == e['size'] \
            and f.get('hash') == e['hash'] and f.get('ok')
        files[rel] = dict(f, size=e['size'], hash=e['hash'], date=e['date'], ok=bool(ok))
        if not ok:
            todo.append(rel)
    total = sum(e['size'] or 0 for e in entries.values())
    cm.update(version=ver, updated=now(), total_size=total, index=index, files=files,
              layout='%s/{check/nextcheck,data/nextdata/<ver>}' % CDN_PREFIX)
    save_json(ctx.cdn_manifest_path, cm)
    need = sum(entries[r]['size'] or 0 for r in todo)
    log('%d files (%s); have %d, to download %d (%s)' % (len(entries), human(total), len(entries) - len(todo),
                                                         len(todo), human(need)))
    if a.only_index or a.dry_run:
        return {'version': ver, 'files': len(entries), 'total_size': total, 'todo': len(todo), 'todo_size': need,
                'index': index}
    if a.limit:
        todo = todo[:a.limit]
    lock = threading.Lock()
    done = {'n': 0, 'bytes': 0, 'fail': []}
    t0 = time.time()

    def one(rel):
        e = entries[rel]
        dest = cdn_local(ctx, rel)
        try:
            download(cdn_urls(rel), dest, e['size'], timeout=60)
            sha = sha256_file(dest)
            with lock:
                files[rel].update(ok=True, sha256=sha, fetched=now())
                done['n'] += 1
                done['bytes'] += e['size'] or 0
                if done['n'] % 50 == 0:
                    el = max(1, time.time() - t0)
                    log('  %d/%d  %s  %.1f MB/s' % (done['n'], len(todo), human(done['bytes']),
                                                    done['bytes'] / el / 1048576))
                    save_json(ctx.cdn_manifest_path, cm)
        except Exception as ex:                                 # keep going, report at the end
            with lock:
                done['fail'].append('%s: %s' % (rel, ex))
    with concurrent.futures.ThreadPoolExecutor(max_workers=max(1, a.threads)) as ex:
        list(ex.map(one, todo))
    cm['updated'] = now()
    save_json(ctx.cdn_manifest_path, cm)
    log('cdn-fetch: %d downloaded (%s) in %ds, %d failed' % (done['n'], human(done['bytes']), time.time() - t0,
                                                            len(done['fail'])))
    for f in done['fail'][:20]:
        log('   FAIL ' + f)
    return {'version': ver, 'files': len(entries), 'total_size': total, 'downloaded': done['n'],
            'bytes': done['bytes'], 'failed': done['fail'], 'disk': dir_size(ctx.cdn)}


def cmd_cdn_verify(ctx, a):
    cm = load_json(ctx.cdn_manifest_path, {})
    if not cm.get('files'):
        raise SystemExit('no CDN mirror yet (cdn-fetch)')
    bad, missing, ok = [], [], 0
    for rel, f in sorted(cm['files'].items()):
        p = cdn_local(ctx, rel)
        if not os.path.exists(p):
            missing.append(rel)
            continue
        if f.get('size') is not None and os.path.getsize(p) != f['size']:
            bad.append('%s: size %d != %d' % (rel, os.path.getsize(p), f['size']))
            f['ok'] = False
            continue
        if f.get('sha256') and not a.dry_run and sha256_file(p) != f['sha256']:
            bad.append('%s: sha256 changed' % rel)
            f['ok'] = False
            continue
        ok += 1
    save_json(ctx.cdn_manifest_path, cm)
    log('cdn-verify: %d ok, %d missing, %d bad' % (ok, len(missing), len(bad)))
    for b in bad[:30]:
        log('   BAD ' + b)
    return {'ok': ok, 'missing': len(missing), 'missing_first': missing[:30], 'bad': bad}


class CdnHandler(http.server.BaseHTTPRequestHandler):
    """GET/HEAD of files under the mirror root, Range: bytes=N- / N-M -> 206. Nothing else."""
    root = None
    logf = None
    server_version = 'QahrCDN/1.0'
    protocol_version = 'HTTP/1.1'

    def log_message(self, fmt, *args):
        pass

    def _log(self, code, size):
        line = '%s\t%s\t%s\t%s\t%d\t%s\n' % (now(), self.client_address[0], self.command, self.path, code, size)
        with CdnHandler.lock:
            with open(CdnHandler.logf, 'a', encoding='utf-8') as f:
                f.write(line)

    def _resolve(self):
        p = urllib.parse.unquote(self.path.split('?', 1)[0])
        full = os.path.realpath(os.path.join(self.root, *[x for x in p.split('/') if x not in ('', '.', '..')]))
        if not full.startswith(os.path.realpath(self.root) + os.sep) or not os.path.isfile(full):
            return None
        return full

    def do_HEAD(self):
        self.do_GET(head=True)

    def do_GET(self, head=False):
        full = self._resolve()
        if not full:
            body = b'not found'
            self.send_response(404)
            self.send_header('Content-Length', str(len(body)))
            self.end_headers()
            if not head:
                self.wfile.write(body)
            self._log(404, 0)
            return
        size = os.path.getsize(full)
        start, end = 0, size - 1
        rng = self.headers.get('Range')
        code = 200
        if rng:
            m = re.match(r'bytes=(\d*)-(\d*)$', rng.strip())
            if m and (m.group(1) or m.group(2)):
                if m.group(1):
                    start = int(m.group(1))
                    end = int(m.group(2)) if m.group(2) else size - 1
                else:
                    start = max(0, size - int(m.group(2)))
                if start >= size:
                    self.send_response(416)
                    self.send_header('Content-Range', 'bytes */%d' % size)
                    self.send_header('Content-Length', '0')
                    self.end_headers()
                    self._log(416, 0)
                    return
                end = min(end, size - 1)
                code = 206
        n = end - start + 1
        self.send_response(code)
        self.send_header('Content-Type', 'application/octet-stream')
        self.send_header('Accept-Ranges', 'bytes')
        self.send_header('Content-Length', str(n))
        self.send_header('Last-Modified', self.date_time_string(os.path.getmtime(full)))
        if code == 206:
            self.send_header('Content-Range', 'bytes %d-%d/%d' % (start, end, size))
        self.end_headers()
        sent = 0
        if not head:
            try:
                with open(full, 'rb') as f:
                    f.seek(start)
                    while sent < n:
                        b = f.read(min(1 << 18, n - sent))
                        if not b:
                            break
                        self.wfile.write(b)
                        sent += len(b)
            except (ConnectionError, OSError):
                pass
        self._log(code, sent)


class ThreadingHTTP(socketserver.ThreadingMixIn, http.server.HTTPServer):
    daemon_threads = True
    allow_reuse_address = True


def cmd_cdn_serve(ctx, a):
    import urllib.parse  # noqa: F401  (used by the handler)
    CdnHandler.root = ctx.cdn
    CdnHandler.logf = os.path.join(ctx.root, 'cdn_access.log')
    CdnHandler.lock = threading.Lock()
    srv = ThreadingHTTP((a.bind, a.port), CdnHandler)
    log('serving %s on http://%s:%d/  (client base: http://<IP>:%d/%s)  log: %s' % (
        ctx.cdn, a.bind, a.port, a.port, CDN_PREFIX, CdnHandler.logf))
    try:
        srv.serve_forever()
    except KeyboardInterrupt:
        pass
    return {'stopped': True}


# ------------------------------------------------------------------------------------------------------ main
COMMANDS = {'check': 'cmd_check', 'fetch': 'cmd_fetch', 'analyse': 'cmd_analyse', 'analyze': 'cmd_analyse',
            'republish': 'cmd_republish', 'cdn-fetch': 'cmd_cdn_fetch', 'cdn-verify': 'cmd_cdn_verify',
            'cdn-serve': 'cmd_cdn_serve', 'status': 'cmd_status'}


def main(argv=None):
    global JSON_OUT
    for st in (sys.stdout, sys.stderr):
        try:
            st.reconfigure(encoding="utf-8", errors="replace")
        except (AttributeError, ValueError):
            pass
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument('command', choices=sorted(COMMANDS))
    ap.add_argument('name', nargs='?', help='patch name (analyse/republish), e.g. 5151-5171')
    ap.add_argument('--root', default=DEF_ROOT)
    ap.add_argument('--patchdir', default=DEF_PATCHDIR)
    ap.add_argument('--client', action='append', default=None, help='stock APK split(s) with assets/ConquerRes')
    ap.add_argument('--json', action='store_true', help='print the result as JSON on the last line')
    ap.add_argument('--version', type=int, help='check/fetch: the version to send (default: latest fetched)')
    ap.add_argument('--deep', action='store_true', help='fetch: also every overlapping range zip')
    ap.add_argument('--dry-run', action='store_true')
    ap.add_argument('--as', dest='as_version', type=int, help='republish: our version number (default: official end)')
    ap.add_argument('--exclude', action='append', default=[], help='republish: path or regex to leave out')
    ap.add_argument('--include', action='append', default=[], help='republish: keep this conflicting path anyway')
    ap.add_argument('--overrides', default=None, help='republish: folder whose files replace/add (default root/overrides)')
    ap.add_argument('--cdn-url', default=None, help='republish: also send ini/autocheckc3.ini pointing at this CDN base')
    ap.add_argument('--title', default=None)
    ap.add_argument('--force', action='store_true', help='republish: overwrite an existing NNNN.zip')
    ap.add_argument('--only-index', action='store_true', help='cdn-fetch: only the version/category/list files')
    ap.add_argument('--limit', type=int, default=0, help='cdn-fetch: at most N files (test)')
    ap.add_argument('--threads', type=int, default=4)
    ap.add_argument('--port', type=int, default=9550)
    ap.add_argument('--bind', default='0.0.0.0')
    a = ap.parse_args(argv)
    a.client = a.client or DEF_CLIENT_APKS
    JSON_OUT = a.json
    ctx = Ctx(a)
    try:
        res = globals()[COMMANDS[a.command]](ctx, a)
        ok = True
    except SystemExit as e:
        if not a.json:
            raise
        res, ok = {'error': str(e)}, False
    except Exception as e:                                   # the console wants an answer, not a traceback
        if not a.json:
            raise
        res, ok = {'error': '%s: %s' % (type(e).__name__, e)}, False
    if a.json:
        print(json.dumps({'ok': ok, 'command': a.command, 'result': res}, ensure_ascii=False))
    return 0 if ok else 1


if __name__ == '__main__':
    sys.exit(main())
