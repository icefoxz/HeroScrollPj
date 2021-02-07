#encoding : utf-8

from e_base import *

def export_json(xls, fn):
    f = create_file(fn)
    if f != None:
        reader = xls_reader.XLSReader()
        cfgs = reader.GetSheetByIndex(xls, 2, 2)
        if cfgs != None:
            f.write("{\n")
            s = "\t\"PlayerLevelTable\": [\n"
            for c in cfgs:
                ri = RowIndex(len(c))
                ss = "\t\t{\n"
                ss += "\t\t\t\"level\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"exp\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"heroLimit\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"companionLimit\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"homeHp\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"yuanZheng\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"unLockShiLi\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"BaYeCombat\": \"" + conv_str_bin(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"BaYeNonCombat\": \"" + conv_str_bin(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"BaYeBattle\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"BaYeJunTuan\": \"" + conv_str_bin(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"BaYeBattleLevel\": \"" + conv_int(c[ri.Next()]) + "\"\n"
                ss += "\t\t},\n"
                s += ss
            s = s[:-2]
            s += "\n"
            s += "\t]\n"
            s += "}"
            f.write(s)
        else:
            print('sheed %s get failed.' % 'cfg')
        f.close()
def export_bin(xls, fn):
    pass