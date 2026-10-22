using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet JiangHuStatusCreate(this ServerSockets.Packet stream, string Name
            , byte Stage = 0, byte Talent = 0, uint Timer = 0, ulong StudyPoints = 0, uint FreeTimeToday =0
            , byte FreeTimeTodeyUsed = 0, uint RoundBuyPoints = 0, ICollection<Role.Instance.JiangHu.Stage> array = null)
        {
            stream.InitWriter();

            stream.Write(Name, 32);
            stream.Write((byte)((Stage == 0) ? 1 : Stage));
            stream.Write(Talent);
            stream.Write(Timer);
            stream.Write((byte)0);
            stream.Write(StudyPoints);
            stream.Write(FreeTimeToday);
            stream.Write(9999999);
            stream.Write(FreeTimeTodeyUsed);
            stream.Write(RoundBuyPoints);

            if (array != null)
            {
                foreach (var obj in array)
                {
                    if (obj.Activate)
                    {
                        foreach (var star in obj.ArrayStars)
                        {
                            stream.Write(star.UID);
                        }
                    }
                }
            }

            stream.Finalize(GamePackets.MsgOwnKongfuImproveSummaryInfo);

            return stream;
        }
    }
}
/*Byte: 226 uShort: 226 UInt: 159121634  Offset = 0
Byte: 124 uShort: 2428 UInt: 1299712380  Offset = 2
Byte: 9 uShort: 30729 UInt: 1766684681  Offset = 3
Byte: 120 uShort: 19832 UInt: 1684622712  Offset = 4
Byte: 77 uShort: 26957 UInt: 1868851533  Offset = 5
Byte: 105 uShort: 25705 UInt: 7300201  Offset = 6
Byte: 100 uShort: 28516 UInt: 28516  Offset = 7
Byte: 111 uShort: 111 UInt: 111  Offset = 8
Byte: 9 uShort: 1289 UInt: 1073743113  Offset = 36
Byte: 5 uShort: 5 UInt: 1111490565  Offset = 37
Byte: 64 uShort: 16960 UInt: 1000000  Offset = 39
Byte: 66 uShort: 3906 UInt: 184553282  Offset = 40
Byte: 15 uShort: 15 UInt: 487260175  Offset = 41
Byte: 11 uShort: 7435 UInt: 7435  Offset = 43
Byte: 29 uShort: 29 UInt: 29  Offset = 44
Byte: 10 uShort: 10 UInt: 10  Offset = 51
Byte: 127 uShort: 38527 UInt: 9999999  Offset = 55
Byte: 150 uShort: 39062 UInt: 39062  Offset = 56
Byte: 152 uShort: 152 UInt: 152  Offset = 57
Byte: 14 uShort: 1294 UInt: 84804878  Offset = 64
Byte: 5 uShort: 3589 UInt: 235212293  Offset = 65
Byte: 14 uShort: 1294 UInt: 84804878  Offset = 66
Byte: 5 uShort: 3589 UInt: 235212293  Offset = 67
Byte: 14 uShort: 1294 UInt: 84804878  Offset = 68
Byte: 5 uShort: 3589 UInt: 235212293  Offset = 69
Byte: 14 uShort: 1294 UInt: 84804878  Offset = 70
Byte: 5 uShort: 3589 UInt: 235212293  Offset = 71
Byte: 14 uShort: 1294 UInt: 84804878  Offset = 72
Byte: 5 uShort: 3589 UInt: 235212293  Offset = 73
Byte: 14 uShort: 1294 UInt: 84804878  Offset = 74
Byte: 5 uShort: 3589 UInt: 235212293  Offset = 75
Byte: 14 uShort: 1294 UInt: 84804878  Offset = 76
Byte: 5 uShort: 3589 UInt: 235212293  Offset = 77
Byte: 14 uShort: 1294 UInt: 84804878  Offset = 78
Byte: 5 uShort: 3589 UInt: 201657861  Offset = 79
Byte: 14 uShort: 1294 UInt: 101451022  Offset = 80
Byte: 5 uShort: 3077 UInt: 201722885  Offset = 81
Byte: 12 uShort: 1548 UInt: 84674060  Offset = 82
Byte: 6 uShort: 3078 UInt: 201657350  Offset = 83
Byte: 12 uShort: 1292 UInt: 84673804  Offset = 84
Byte: 5 uShort: 3077 UInt: 201657349  Offset = 85
Byte: 12 uShort: 1292 UInt: 84673804  Offset = 86
Byte: 5 uShort: 3077 UInt: 201657349  Offset = 87
Byte: 12 uShort: 1292 UInt: 84673804  Offset = 88
Byte: 5 uShort: 3077 UInt: 201657349  Offset = 89
Byte: 12 uShort: 1292 UInt: 84673804  Offset = 90
Byte: 5 uShort: 3077 UInt: 201657349  Offset = 91
Byte: 12 uShort: 1292 UInt: 84673804  Offset = 92
Byte: 5 uShort: 3077 UInt: 201657349  Offset = 93
Byte: 12 uShort: 1292 UInt: 84673804  Offset = 94
Byte: 5 uShort: 3077 UInt: 201657349  Offset = 95
Byte: 12 uShort: 1292 UInt: 101451020  Offset = 96
Byte: 5 uShort: 3077 UInt: 33950725  Offset = 97
Byte: 12 uShort: 1548 UInt: 100795916  Offset = 98
Byte: 6 uShort: 518 UInt: 33948166  Offset = 99
Byte: 2 uShort: 1538 UInt: 84018690  Offset = 100
Byte: 6 uShort: 518 UInt: 33882630  Offset = 101
Byte: 2 uShort: 1282 UInt: 84018434  Offset = 102
Byte: 5 uShort: 517 UInt: 33882629  Offset = 103
Byte: 2 uShort: 1282 UInt: 84018434  Offset = 104
Byte: 5 uShort: 517 UInt: 33882629  Offset = 105
Byte: 2 uShort: 1282 UInt: 84018434  Offset = 106
Byte: 5 uShort: 517 UInt: 33882629  Offset = 107
Byte: 2 uShort: 1282 UInt: 84018434  Offset = 108
Byte: 5 uShort: 517 UInt: 33882629  Offset = 109
Byte: 2 uShort: 1282 UInt: 84018434  Offset = 110
Byte: 5 uShort: 517 UInt: 33882629  Offset = 111
Byte: 2 uShort: 1282 UInt: 84018434  Offset = 112
Byte: 5 uShort: 517 UInt: 33882629  Offset = 113
Byte: 2 uShort: 1282 UInt: 84018434  Offset = 114
Byte: 5 uShort: 517 UInt: 251986437  Offset = 115
Byte: 2 uShort: 1282 UInt: 101647618  Offset = 116
Byte: 5 uShort: 3845 UInt: 252055301  Offset = 117
Byte: 15 uShort: 1551 UInt: 101647887  Offset = 118
Byte: 6 uShort: 3846 UInt: 252055302  Offset = 119
Byte: 15 uShort: 1551 UInt: 101647887  Offset = 120
Byte: 6 uShort: 3846 UInt: 252055302  Offset = 121
Byte: 15 uShort: 1551 UInt: 101647887  Offset = 122
Byte: 6 uShort: 3846 UInt: 252055302  Offset = 123
Byte: 15 uShort: 1551 UInt: 101647887  Offset = 124
Byte: 6 uShort: 3846 UInt: 252055302  Offset = 125
Byte: 15 uShort: 1551 UInt: 101647887  Offset = 126
Byte: 6 uShort: 3846 UInt: 252055302  Offset = 127
Byte: 15 uShort: 1551 UInt: 101647887  Offset = 128
Byte: 6 uShort: 3846 UInt: 252055302  Offset = 129
Byte: 15 uShort: 1551 UInt: 101647887  Offset = 130
Byte: 6 uShort: 3846 UInt: 252055302  Offset = 131
Byte: 15 uShort: 1551 UInt: 84870671  Offset = 132
Byte: 6 uShort: 3846 UInt: 17108742  Offset = 133
Byte: 15 uShort: 1295 UInt: 100730127  Offset = 134
Byte: 5 uShort: 261 UInt: 17170693  Offset = 135
Byte: 1 uShort: 1537 UInt: 83953153  Offset = 136
Byte: 6 uShort: 262 UInt: 17105158  Offset = 137
Byte: 1 uShort: 1281 UInt: 83952897  Offset = 138
Byte: 5 uShort: 261 UInt: 17105157  Offset = 139
Byte: 1 uShort: 1281 UInt: 83952897  Offset = 140
Byte: 5 uShort: 261 UInt: 251986181  Offset = 141
Byte: 1 uShort: 1281 UInt: 101647617  Offset = 142
Byte: 5 uShort: 3845 UInt: 17174277  Offset = 143
Byte: 15 uShort: 1551 UInt: 100730383  Offset = 144
Byte: 6 uShort: 262 UInt: 17170694  Offset = 145
Byte: 1 uShort: 1537 UInt: 100730369  Offset = 146
Byte: 6 uShort: 262 UInt: 17170694  Offset = 147
Byte: 1 uShort: 1537 UInt: 100730369  Offset = 148
Byte: 6 uShort: 262 UInt: 17170694  Offset = 149
Byte: 1 uShort: 1537 UInt: 100730369  Offset = 150
Byte: 6 uShort: 262 UInt: 201720070  Offset = 151
Byte: 1 uShort: 1537 UInt: 84674049  Offset = 152
Byte: 6 uShort: 3078 UInt: 201657350  Offset = 153
Byte: 12 uShort: 1292 UInt: 84673804  Offset = 154
Byte: 5 uShort: 3077 UInt: 201657349  Offset = 155
Byte: 12 uShort: 1292 UInt: 84673804  Offset = 156
Byte: 5 uShort: 3077 UInt: 201657349  Offset = 157
Byte: 12 uShort: 1292 UInt: 84673804  Offset = 158
Byte: 5 uShort: 3077 UInt: 201657349  Offset = 159
Byte: 12 uShort: 1292 UInt: 84673804  Offset = 160
Byte: 5 uShort: 3077 UInt: 201657349  Offset = 161
Byte: 12 uShort: 1292 UInt: 84673804  Offset = 162
Byte: 5 uShort: 3077 UInt: 201657349  Offset = 163
Byte: 12 uShort: 1292 UInt: 84673804  Offset = 164
Byte: 5 uShort: 3077 UInt: 201657349  Offset = 165
Byte: 12 uShort: 1292 UInt: 84673804  Offset = 166
Byte: 5 uShort: 3077 UInt: 201657349  Offset = 167
Byte: 12 uShort: 1292 UInt: 84673804  Offset = 168
Byte: 5 uShort: 3077 UInt: 33885189  Offset = 169
Byte: 12 uShort: 1292 UInt: 84018444  Offset = 170
Byte: 5 uShort: 517 UInt: 33882629  Offset = 171
Byte: 2 uShort: 1282 UInt: 84018434  Offset = 172
Byte: 5 uShort: 517 UInt: 33882629  Offset = 173
Byte: 2 uShort: 1282 UInt: 84018434  Offset = 174
Byte: 5 uShort: 517 UInt: 33882629  Offset = 175
Byte: 2 uShort: 1282 UInt: 84018434  Offset = 176
Byte: 5 uShort: 517 UInt: 33882629  Offset = 177
Byte: 2 uShort: 1282 UInt: 84018434  Offset = 178
Byte: 5 uShort: 517 UInt: 33882629  Offset = 179
Byte: 2 uShort: 1282 UInt: 84018434  Offset = 180
Byte: 5 uShort: 517 UInt: 33882629  Offset = 181
Byte: 2 uShort: 1282 UInt: 84018434  Offset = 182
Byte: 5 uShort: 517 UInt: 33882629  Offset = 183
Byte: 2 uShort: 1282 UInt: 84018434  Offset = 184
Byte: 5 uShort: 517 UInt: 33882629  Offset = 185
Byte: 2 uShort: 1282 UInt: 84018434  Offset = 186
Byte: 5 uShort: 517 UInt: 235209221  Offset = 187
Byte: 2 uShort: 1282 UInt: 101582082  Offset = 188
Byte: 5 uShort: 3589 UInt: 235277829  Offset = 189
Byte: 14 uShort: 1550 UInt: 101582350  Offset = 190
Byte: 6 uShort: 3590 UInt: 235277830  Offset = 191
Byte: 14 uShort: 1550 UInt: 101582350  Offset = 192
Byte: 6 uShort: 3590 UInt: 235277830  Offset = 193
Byte: 14 uShort: 1550 UInt: 101582350  Offset = 194
Byte: 6 uShort: 3590 UInt: 235277830  Offset = 195
Byte: 14 uShort: 1550 UInt: 101582350  Offset = 196
Byte: 6 uShort: 3590 UInt: 235277830  Offset = 197
Byte: 14 uShort: 1550 UInt: 101582350  Offset = 198
Byte: 6 uShort: 3590 UInt: 235277830  Offset = 199
Byte: 14 uShort: 1550 UInt: 101582350  Offset = 200
Byte: 6 uShort: 3590 UInt: 235277830  Offset = 201
Byte: 14 uShort: 1550 UInt: 101582350  Offset = 202
Byte: 6 uShort: 3590 UInt: 235277830  Offset = 203
Byte: 14 uShort: 1550 UInt: 101582350  Offset = 204
Byte: 6 uShort: 3590 UInt: 17174022  Offset = 205
Byte: 14 uShort: 1550 UInt: 100730382  Offset = 206
Byte: 6 uShort: 262 UInt: 17170694  Offset = 207
Byte: 1 uShort: 1537 UInt: 100730369  Offset = 208
Byte: 6 uShort: 262 UInt: 17170694  Offset = 209
Byte: 1 uShort: 1537 UInt: 100730369  Offset = 210
Byte: 6 uShort: 262 UInt: 17170694  Offset = 211
Byte: 1 uShort: 1537 UInt: 100730369  Offset = 212
Byte: 6 uShort: 262 UInt: 17170694  Offset = 213
Byte: 1 uShort: 1537 UInt: 100730369  Offset = 214
Byte: 6 uShort: 262 UInt: 17170694  Offset = 215
Byte: 1 uShort: 1537 UInt: 100730369  Offset = 216
Byte: 6 uShort: 262 UInt: 17170694  Offset = 217
Byte: 1 uShort: 1537 UInt: 100730369  Offset = 218
Byte: 6 uShort: 262 UInt: 17170694  Offset = 219
Byte: 1 uShort: 1537 UInt: 100730369  Offset = 220
Byte: 6 uShort: 262 UInt: 17170694  Offset = 221
Byte: 1 uShort: 1537 UInt: 100730369  Offset = 222
Byte: 6 uShort: 262*/