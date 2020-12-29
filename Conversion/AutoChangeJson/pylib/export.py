#encoding : utf-8

import os
import sys
import shutil
import time

import traceback, multiprocessing, functools
from multiprocessing import Pool,cpu_count,Manager

import e_res
import e_base

#py_file

import e_create_playerInitialTable
import e_create_assetTable
import e_create_playerLevelTable
import e_create_upGradeTable
import e_create_classTable
import e_create_heroTable
import e_create_soldierTable
import e_create_towerTable
import e_create_trapTable
import e_create_spellTable
import e_create_cityLevelTable
import e_create_warTable
import e_create_warChestTable
import e_create_pointTable
import e_create_battleEventTable
import e_create_enemyTable
import e_create_enemyUnitTable
import e_create_storyTable
import e_create_storyRTable
import e_create_testTable
import e_create_testRTable
import e_create_supplyTable
import e_create_encounterTable
import e_create_shoppingTable
import e_create_choseWarTable
import e_create_guideTable
import e_create_knowledgeTable
import e_create_rcodeTable
import e_create_tiLiStoreTable
import e_create_enemyBOSSTable
import e_create_stringTextTable
import e_create_numParametersTable
import e_create_jiBanTable
import e_create_shiLiTable
import e_create_baYeDiTuTable
import e_create_baYeShiJianTable
import e_create_baYeBattleTable
import e_create_baYeRenWuTable


taskList = (

	('/Hero.xlsx', e_create_playerInitialTable, '/PlayerInitialTable.json'),
	('/Hero.xlsx', e_create_assetTable, '/AssetTable.json'),
	('/Hero.xlsx', e_create_playerLevelTable, '/PlayerLevelTable.json'),
	('/Hero.xlsx', e_create_upGradeTable, '/UpGradeTable.json'),
	('/Hero.xlsx', e_create_classTable, '/ClassTable.json'),
	('/Hero.xlsx', e_create_heroTable, '/HeroTable.json'),
	('/Hero.xlsx', e_create_soldierTable, '/SoldierTable.json'),
	('/Hero.xlsx', e_create_towerTable, '/TowerTable.json'),
	('/Hero.xlsx', e_create_trapTable, '/TrapTable.json'),
	('/Hero.xlsx', e_create_spellTable, '/SpellTable.json'),
	('/Hero.xlsx', e_create_cityLevelTable, '/CityLevelTable.json'),
	('/Hero.xlsx', e_create_warTable, '/WarTable.json'),
	('/Hero.xlsx', e_create_warChestTable, '/WarChestTable.json'),
	('/Hero.xlsx', e_create_pointTable, '/PointTable.json'),
	('/Hero.xlsx', e_create_battleEventTable, '/BattleEventTable.json'),
	('/Hero.xlsx', e_create_enemyTable, '/EnemyTable.json'),
	('/Hero.xlsx', e_create_enemyUnitTable, '/EnemyUnitTable.json'),
	('/Hero.xlsx', e_create_storyTable, '/StoryTable.json'),
	('/Hero.xlsx', e_create_storyRTable, '/StoryRTable.json'),
	('/Hero.xlsx', e_create_testTable, '/TestTable.json'),
	('/Hero.xlsx', e_create_testRTable, '/TestRTable.json'),
	('/Hero.xlsx', e_create_supplyTable, '/SupplyTable.json'),
	('/Hero.xlsx', e_create_encounterTable, '/EncounterTable.json'),
	('/Hero.xlsx', e_create_shoppingTable, '/ShoppingTable.json'),
	('/Hero.xlsx', e_create_choseWarTable, '/ChoseWarTable.json'),
	('/Hero.xlsx', e_create_guideTable, '/GuideTable.json'),
	('/Hero.xlsx', e_create_knowledgeTable, '/KnowledgeTable.json'),
	('/Hero.xlsx', e_create_rcodeTable, '/RcodeTable.json'),
	('/Hero.xlsx', e_create_tiLiStoreTable, '/TiLiStoreTable.json'),
	('/Hero.xlsx', e_create_enemyBOSSTable, '/EnemyBOSSTable.json'),
	('/Hero.xlsx', e_create_stringTextTable, '/StringTextTable.json'),
	('/Hero.xlsx', e_create_numParametersTable, '/NumParametersTable.json'),
	('/Hero.xlsx', e_create_jiBanTable, '/JiBanTable.json'),
	('/Hero.xlsx', e_create_shiLiTable, '/ShiLiTable.json'),
	('/Hero.xlsx', e_create_baYeDiTuTable, '/BaYeDiTuTable.json'),
	('/Hero.xlsx', e_create_baYeShiJianTable, '/BaYeShiJianTable.json'),
	('/Hero.xlsx', e_create_baYeBattleTable, '/BaYeBattleTable.json'),
	('/Hero.xlsx', e_create_baYeRenWuTable, '/BaYeRenWuTable.json'),

)





def trace_unhandled_exceptions(func):
	@functools.wraps(func)
	def wrapped_func(*args, **kwargs):
		try:
			func(*args, **kwargs)
		except:
			# trace back.print_exc()
			raise Exception(traceback.format_exc())
	return wrapped_func

dependTaskList = (

)

@trace_unhandled_exceptions
def ResWork(db_path, ep_path, resList):
	worktask = [
		
	]
	for v in worktask:
		v[1].export_json(db_path + v[0], ep_path + v[2], resList)
		print(db_path + v[0])

@trace_unhandled_exceptions
def loadTb(start, end, dbpath, eppath):
	for x in range(start, end):
		taskList[x][1].export_json(dbpath + taskList[x][0], eppath + taskList[x][2])
		print(dbpath + taskList[x][0])

def export_json(db_path, ep_path, resList):
	e_base.prepair_path(ep_path)
	starttime = time.time()

	p = Pool()
	t_len = len(taskList)
	d_len = len(dependTaskList)

	def err_cb(exc_msg):
		print(exc_msg)
		p.terminate()

	for i in range(d_len):
		p.apply_async(dependTaskList[i], args=(db_path, ep_path), error_callback = err_cb)

	p.apply_async(ResWork, args=(db_path, ep_path, resList), error_callback = err_cb)

	average = 15
	start = 0
	end = 0
	while True:
		start = end
		if start == t_len:
			break

		end = start + average
		if end >= t_len:
			end = t_len
		#print(start, end)
		p.apply_async(loadTb, args=(start,end,db_path,ep_path), error_callback = err_cb)

	p.close()
	p.join()
	print("load client db time %0.2f" %(time.time() - starttime))

def verfiy_lua(ep_path):
	return True

def export_bin(db_path, ep_path):
	pass

def verfiy_bin(ep_path):
	return True

if __name__ == '__main__':
	db_path = sys.argv[1]
	ep_path = sys.argv[2]

	db_path = db_path.rstrip("\\")
	db_path = db_path.rstrip("/")

	ep_path = ep_path.rstrip("\\")
	ep_path = ep_path.rstrip("/")

	resList = Manager().list([dict(), dict(), dict(), dict(), dict(), dict(), dict(), dict(), dict()])
	print('开始导出客户端数据... ...')
	export_json(db_path, ep_path, resList)
	export_bin(db_path, ep_path)

	print('数据导出完成, 开始验证数据完整性... ...')
	if not verfiy_lua(ep_path):
		print('数据存在错误，查看日志了解详情')
	else:
		print('数据未发现异常')

	print('导出资源数据... ...')
	e_res.export_json('../rescpy_compress.ini', True, resList)
	e_res.export_json('../rescpy_uncompress.ini', False, resList)
	print('导出完成')