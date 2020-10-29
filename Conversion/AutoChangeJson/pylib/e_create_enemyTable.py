#encoding : utf-8

from e_base import *

def export_json(xls, fn):
    f = create_file(fn)
    if f != None:
        reader = xls_reader.XLSReader()
        cfgs = reader.GetSheetByIndex(xls, 15, 2)
        if cfgs != None:
            f.write("{\n")
            s = "\t\"EnemyTable\": [\n"
            for c in cfgs:
                ri = RowIndex(len(c))
                ss = "\t\t{\n"
                ss += "\t\t\t\"enemyId\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"pos1\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"pos2\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"pos3\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"pos4\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"pos5\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"pos6\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"pos7\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"pos8\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"pos9\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"pos10\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"pos11\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"pos12\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"pos13\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"pos14\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"pos15\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"pos16\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"pos17\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"pos18\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"pos19\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"pos20\": \"" + conv_int(c[ri.Next()]) + "\"\n"
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