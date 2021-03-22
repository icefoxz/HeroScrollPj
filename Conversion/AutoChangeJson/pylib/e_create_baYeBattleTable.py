#encoding : utf-8

from e_base import *

def export_json(xls, fn):
    f = create_file(fn)
    if f != None:
        reader = xls_reader.XLSReader()
        cfgs = reader.GetSheetByIndex(xls, 36, 2)
        if cfgs != None:
            f.write("{\n")
            s = "\t\"BaYeBattleTable\": [\n"
            for c in cfgs:
                ri = RowIndex(len(c))
                ss = "\t\t{\n"
                ss += "\t\t\t\"id\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"level0\": \"" + conv_str_bin(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"level1\": \"" + conv_str_bin(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"level2\": \"" + conv_str_bin(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"level3\": \"" + conv_str_bin(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"level4\": \"" + conv_str_bin(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"level5\": \"" + conv_str_bin(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"level6\": \"" + conv_str_bin(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"level7\": \"" + conv_str_bin(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"level8\": \"" + conv_str_bin(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"level9\": \"" + conv_str_bin(c[ri.Next()]) + "\"\n"
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