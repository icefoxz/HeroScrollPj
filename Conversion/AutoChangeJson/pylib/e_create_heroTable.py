#encoding : utf-8

from e_base import *

def export_json(xls, fn):
    f = create_file(fn)
    if f != None:
        reader = xls_reader.XLSReader()
        cfgs = reader.GetSheetByIndex(xls, 5, 2)
        if cfgs != None:
            f.write("{\n")
            s = "\t\"HeroTable\": [\n"
            for c in cfgs:
                ri = RowIndex(len(c))
                ss = "\t\t{\n"
                ss += "\t\t\t\"id\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"name\": \"" + conv_str_bin(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"intro\": \"" + conv_str_bin(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"rarity\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"price\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"classes\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"powers\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"damage\": \"" + conv_str_bin(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"hp\": \"" + conv_str_bin(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"hpr\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"dod\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"def\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"cri\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"criDamage\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"huixin\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"huixinDamage\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"icon\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"tag1\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"tag2\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"chestZyBox\": \"" + conv_int(c[ri.Next()]) + "\",\n"
                ss += "\t\t\t\"chestBxBox\": \"" + conv_int(c[ri.Next()]) + "\"\n"
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