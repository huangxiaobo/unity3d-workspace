#!/usr/bin/env python
# -*- coding: utf-8 -*-

import random
from mymath.vector3 import *
import gameevents
import user
import enemy
import player
import struct
import gametimer
import healthammo

import database

#  world

class World:
    def __init__(self):
        self.pos = Vector3(0, 0, 0) #//世界坐标原点
        self.width = 50
        self.length = 50

        self.users_map = {}         #用户

        self.enemies_map = {}       #怪物

        self.pick_package = {}      #可拾取的包

        self.players = {}       #玩家

        self.host = None        #主机


        self.init_world()

        #定时器,定时刷新敌人
        self.timer_update_enemeies = gametimer.Timer(self.update, [], 0.1)
        self.timer_update_enemeies.start()

    def init_world(self):
        self.init_enemy()

        self.init_package()

    def init_enemy(self):
        self.add_enemy()

    def init_package(self):
        self.pick_package.clear()
        for hid, data in enumerate(healthammo.HEALTHAMMO_DATA):
            self.pick_package[hid] = healthammo.HealthAmmo(hid, data[0], data[1])



    def update(self):

        self.update_enemies()

        self.world_check_pack_package()


    def add_enemy(self):
        self.enemies_map.clear()     #清除整个敌人
        for id, data in enumerate(enemy.ENEMY_DATA):
            self.enemies_map[id] = (enemy.Enemy(id, data[0], data[1], data[2]))

    def update_enemey_rot(self):
        for (eid, en) in self.enemies_map.iteritems():
            if en.enemy_type == enemy.ENEMY_TYPE_NR:
                #更新旋转
                for pl in self.players.itervalues():
                    mser = gameevents.msg_sc_enemy_rot(en.eid, 0.0, 1.0, 0.0)
                    self.host.send(pl.conn, mser.marshal())

    def update_enemy_pos(self):
        pass

    def update_enemies(self):
        for (eid, en) in self.enemies_map.iteritems():
            if en.enemy_type == enemy.ENEMY_TYPE_NR:
                self.enemy_update(eid)

        #for (eid, en) in self.enemies_map.iteritems():
        for eid in self.enemies_map.keys():
            if self.enemies_map[eid].enemy_type == enemy.ENEMY_TYPE_AI:
                self.enemy_update(eid)
        for eid in self.enemies_map.keys():
            if self.enemies_map[eid].death():
                self.enemy_notify_die(eid)

    def enemy_update(self, eid):
        #en =
        en = self.enemies_map[eid]

        if en.enemy_type == enemy.ENEMY_TYPE_NR:
            #self.enemy_apply_rot(en.eid)
            return

        minDist = 1000
        target = None
        for pl in self.players.itervalues():
            d = en.position.get_distance_to(pl.position)
            if d < minDist:
                minDist = d
                target = pl

        if target is None or (not en.can_see_target(pl)):
            return

        if en.can_attack(pl): #可以攻击
            #更新 player and enemy
            self.player_apply_damage(pl.uid, en.attack_value)
            self.enemy_apply_damage(en.eid, pl.uid, en.life)      #自爆类型的怪物
        else:
            #只能看到
            vec = Vector3(pl.position - en.position).normalize()
            en.move(vec)
            self.enemy_apply_mov(en.eid)

    def enemy_apply_rot(self, eid):
         #更新旋转
         for pl in self.players.itervalues():
            mser = gameevents.msg_sc_enemy_rot(eid, 0.0, 1.0, 0.0)
            self.host.send(pl.conn, mser.marshal())

    def enemy_apply_mov(self, eid):
        en = self.enemies_map[eid]
        for pl in self.players.itervalues():
            msep = gameevents.msg_sc_enemy_pos(en.eid, en.position.x, en.position.y, en.position.z)
            self.host.send(pl.conn, msep.marshal())

    def enemy_apply_damage(self, eid, uid, damage):
        if eid not in self.enemies_map.iterkeys():
            print 'enemy_apply_damage error'
            return
        print '敌人', eid , '受到', uid, '攻击!'
        self.enemies_map[eid].life -= damage
        if self.enemies_map[eid].death():
            print '敌人', eid, '死亡', ' uid: ', uid
            self.players[uid].killcount += 1
            self.enemy_notify_die(eid)
            self.player_notify_killcount(uid)
        else:
            self.enemy_notify_damage(eid, damage)

    def enemy_notify_damage(self, eid, damage):
        msed = gameevents.msg_sc_enemy_damage(eid, damage).marshal()
        for pl in self.players.itervalues():
            self.host.send(pl.conn, msed)

    def enemy_notify_die(self, eid):
        if not self.enemies_map.has_key(eid):   #可能已经被删除   定时器的原因
            return
        msed = gameevents.msg_sc_enemy_die(eid).marshal()
        for pl in self.players.itervalues():
            self.host.send(pl.conn, msed)
        #删除怪物

        del(self.enemies_map[eid])

    def create_player(self, conn = 0, uid = 0):
        print '创建玩家控制角色...uid: ', uid
        position = Vector3(random.uniform(-2, 2), 1.8, random.uniform(-2, 2))
        p = player.Player(conn, uid = uid, position = position)
        self.players[uid] = p
        return p


    def enter_world(self, commer):
        #玩家进入地图开始初始化地图信息
        #创建怪物
        #创建角色

        #向所有玩家发送创建消息
        #先将所有的玩家的信息在自己上面创建出来
        print '进入地图... uid:', commer.uid
        #在其他玩家上创建自己
        for p1 in self.players.itervalues():
            if (p1 != commer):
                msp = gameevents.msg_sc_player_create(commer.uid, commer.position.x, commer.position.y, commer.position.z)
                self.host.send(p1.conn, msp.marshal())

        #将其他玩家创建到自己客户端上
        for p2 in self.players.itervalues():
            mspc = gameevents.msg_sc_player_create(p2.uid, p2.position.x, p2.position.y, p2.position.z)
            self.host.send(commer.conn, mspc.marshal())

        #创建敌人
        print '创建敌人: ', len(self.enemies_map)
        for e in self.enemies_map.itervalues():
            msec = gameevents.msg_sc_enemy_create(e.eid, e.position.x, e.position.y, e.position.z, e.life, e.enemy_type)
            self.host.send(commer.conn, msec.marshal())

        #创建可以拾取装备
        print '创建可拾取装备'
        for ha in self.pick_package.itervalues():
            mshc = gameevents.msg_sc_ha_create(hid = ha.hid, x = ha.position.x, y = ha.position.y, z = ha.position.z)
            self.host.send(commer.conn, mshc.marshal())


    def add_user(self, conn = 0, name = '', password = ''):
        if conn == 0:
            return
        new_user = user.User(conn, name, password)
        self.users_map[new_user.uid] = new_user
        print '创建新角色...uid: ', new_user.uid, ' 在线人数:', len(self.users_map)
        return new_user

    def find_user(self, uid):
        if uid in self.users_map.iterkeys():
            return self.users_map[uid]
        else:
            return None

    def find_user_by_conn(self, conn = None):
        for u in self.users_map.itervalues():
            if u.conn == conn:
                return u
        return None

    def del_user(self, uid = 0):
        if uid in self.players.iterkeys():
            del self.players[uid]

        #删除用户
        if uid in self.users_map.iterkeys():
            del(self.users_map[uid])

        #广播
        data = msd = gameevents.msg_sc_deluser(uid = uid).marshal()
        for player in self.players.itervalues():
            self.host.send(player.conn, data)

        text = 'player ' + str(uid) + ' leave game.'
        data = gameevents.msg_sc_chat(uid = uid, text = text).marshal()
        for player in self.users_map.itervalues():
            self.host.send(player.conn, data)


        print '剩余玩家数: ', len(self.players)
    def player_rot(self, uid = 0):
        pass

    def player_pos(self, uid = 0, x = 0, y = 0, z = 0):
        #直接将消息转发出去
        if uid not in self.users_map.iterkeys():
            return
        self.players[uid].position = Vector3(x, y, z)

        mspp = gameevents.msg_sc_player_pos(uid, x, y, z).marshal()
        for player in self.players.itervalues():
            if uid != player.uid:
                self.host.send(player.conn, mspp)

    def player_apply_damage(self, uid, damage):
        #print '玩家伤害: ', damage, ' uid:', uid
        if uid not in self.players.iterkeys():
            print 'player_apply_damage error. uid: ', uid
            return

        pl = self.players[uid]
        pl.life -= damage
        self.player_nofity_life(pl.uid)

    def player_nofity_life(self, uid):
        #更新某个玩家的生命值
        pl = self.players[uid]
        mspl = gameevents.msg_sc_player_life(uid, pl.life).marshal()
        for player in self.players.itervalues():
            self.host.send(player.conn, mspl)

    def player_shoot(self,uid,ray_org_x, ray_org_y, ray_org_z,ray_dir_x, ray_dir_y, ray_dir_z):
        #开枪
        msst = gameevents.msg_sc_shoot_status()
        msst.uid = uid
        msst.ray_org_x = ray_org_x
        msst.ray_org_y = ray_org_y
        msst.ray_org_z = ray_org_z

        msst.ray_dir_x = ray_dir_x
        msst.ray_dir_y = ray_dir_y
        msst.ray_dir_z = ray_dir_z

        data = msst.marshal()
        for (k, v) in self.users_map.iteritems():
            if k != uid:
                self.host.send(v.conn, data)

    def player_notify_killcount(self, uid):
        mspk = gameevents.msg_sc_player_killcount(uid = uid, killcount= self.players[uid].killcount)
        self.host.send(self.users_map[uid].conn, mspk.marshal())


    def health_ammo_create(self, hid):
        if not self.pick_package.has_key(hid):
            return
        ha = self.pick_package[hid]

        mshc = gameevents.msg_sc_ha_create(hid = hid, x = ha.position.x, y = ha.position.y, z = ha.position.z)
        for u in self.users_map.itervalues():
            self.host.send(u.conn, mshc.marshal())

    def health_ammo_del(self, hid):
        if not self.pick_package.has_key(hid):
            return
        ha = self.pick_package[hid]

        mshc = gameevents.msg_sc_ha_create(hid = hid, x = ha.position.x, y = ha.position.y, z = ha.position.z)
        for u in self.users_map.itervalues():
            self.host.send(u.conn, mshc.marshal())


    def health_ammo_notify_pick(self, hid, uid):#被uid拾取
        print 'health_ammo_notify_pick......'
        mshp = gameevents.msg_sc_ha_pick(hid = hid, uid = uid).marshal()

        for pl in self.players.itervalues():
            self.host.send(pl.conn, mshp)


    def world_check_pack_package(self):
        #检查是否有可拾取的装备
        for pl in self.players.itervalues():
            for ha in self.pick_package.itervalues():
                dist = Vector3(pl.position - ha.position).get_length()
                if dist < 0.5:
                    #拾取
                    if ha.ha_type == healthammo.PACKAGE_HEALTH:
                        print '装备被拾取.........'
                        pl.life += 10;
                        self.health_ammo_notify_pick(ha.hid, pl.uid)
                        self.player_nofity_life(pl.uid, pl.life)
                        return
                    else:
                        #
                        pass

