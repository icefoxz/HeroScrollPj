#encoding : utf-8

from e_base import *

def export_json(xls, fn):
    f = create_file(fn)
    if f != None:
        reader = xls_reader.XLSReader()
        cfgs = reader.GetSheetByIndex(xls, 39, 2)
        if cfgs != None:
            f.write("{\n")
            s = "\t\"StoryIdTable\": [\n"
            for c in cfgs:
                ri = RowIndex(len(c))
                ss = "\t\t{\n"
                ss += "\t\t\t\"storyId\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"weight\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"storyType\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"jinBiR\": \"" + conv_str_bin(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"jingYanR\": \"" + conv_str_bin(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"YuQueR\": \"" + conv_str_bin(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"YuanBaoR\": \"" + conv_str_bin(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"JunTuanR\": \"" + conv_str_bin(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"Time\": \"" + conv_str_bin(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"WarId\": \"" + conv_int(c[ri.Next()]) + "\"\n"
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