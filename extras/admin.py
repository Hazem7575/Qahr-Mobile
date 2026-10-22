r"""Run a GameServer admin action on a player (Admin\AdminApi, 127.0.0.1, secret from Data\adminapi.txt), no restart.

  venv\Scripts\python.exe -I tools\admin.py <op> key=value ...
  e.g.  admin.py online
        admin.py teleport name=TrojanQ1 map=1002 x=640 y=400
        admin.py setlevel name=TrojanQ1 level=15
        admin.py learnskill name=TrojanQ1 skill=1045 level=4
        admin.py revive name=TrojanQ1
        admin.py runcommand name=TrojanQ1 command="@spell 1045 4"
Ops: AdminApi.cs Dispatch (online player search setlevel addexp money cps giveitem teleport revive learnskill
     runcommand reloadnpcs reloadmonsters reloadquests reloadall ...).
"""
import json, socket, sys

CFG = r'D:\conquer\QahrMobile\Data\adminapi.txt'

def cfg():
    d = {}
    for line in open(CFG, encoding='utf-8'):
        line = line.strip()
        if '=' in line and not line.startswith('#'):
            k, v = line.split('=', 1); d[k.strip()] = v.strip()
    return d

def call(op, **args):
    c = cfg()
    req = dict(args, secret=c['secret'], op=op)
    s = socket.create_connection(('127.0.0.1', int(c.get('port', 5899))), timeout=15)
    s.sendall((json.dumps(req) + '\n').encode())
    data = b''
    while not data.endswith(b'\n'):
        part = s.recv(65536)
        if not part: break
        data += part
    return json.loads(data.decode('utf-8'))

if __name__ == '__main__':
    args = {}
    for a in sys.argv[2:]:
        k, v = a.split('=', 1)
        args[k] = int(v) if v.lstrip('-').isdigit() else v
    print(json.dumps(call(sys.argv[1], **args), ensure_ascii=False, indent=1)[:3000])
