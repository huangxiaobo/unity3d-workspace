#!/usr/bin/env python
# -*- coding: utf-8 -*-
#======================================================================
#
# service.py - network service
#
# NOTE: for more information, please see the readme file.
#
#======================================================================
import sys
from gameevents import *
import player
import gl
import user
import database

#----------------------------------------------------------------------
# service id
#----------------------------------------------------------------------
ERROR_SERVICE = 0x0000
LOGIN_SERVICE = 0x0001
ENEMY_SERVICE = 0X0002
MAP_SERVICE   = 0X0003
PLAYER_SERVICE = 0x0004
CHAT_SERVICE = 0X0005
#----------------------------------------------------------------------
# service interface
#----------------------------------------------------------------------
class service (object):

    def __init__ (self, sid = 0):
        self.service_id = sid
        self.__command_map = {}

    def handle (self, msg, owner):
        cid = msg.cid
        if not cid in self.__command_map:
            raise Exception('bad command %s'%cid)
        f = self.__command_map[cid]
        return f(msg, owner)

    def register (self, cid, function):
        self.__command_map[cid] = function

    def registers (self, CommandDict):
        self.__command_map = {}
        for cid in CommandDict:
            self.register(cid, CommandDict[cid])
        return ERROR_SERVICE

    def exist(self, cid):
        print '_command_map.key:', self.__command_map.has_key(4097)
        if cid in self.__command_map.keys():
            return True
        else:
            return False

#----------------------------------------------------------------------
# dispatcher
#----------------------------------------------------------------------
class dispatcher (object):

    def __init__ (self):
        self.__service_map = {}

    def dispatch (self, msg, owner):
        sid = msg.sid
        if not sid in self.__service_map:
            raise Exception('bad service %d'%sid)
        svc = self.__service_map[sid]
        svc.handle(msg, owner)

    def register (self, sid, svc):
        self.__service_map[sid] = svc

    def getsid(self, cid):
        for key, val in self.__service_map.iteritems():
            if val.exist(cid):
                return key
        return ERROR_SERVICE


#----------------------------------------------------------------------
# testing case
#----------------------------------------------------------------------
class test_service(service):

    def __init__ (self, sid = 0):
        super(test_service, self).__init__(sid)
        commands = {
            10 : self.f1,
            20 : self.f2,
            30 : self.f3,
        }
        self.registers(commands)

    def f1 (self, msg, owner):
        print 'f1', msg, 'from', owner

    def f2 (self, msg, owner):
        print 'f2', msg, 'from', owner

    def f3 (self, msg, owner):
        print 'f3', msg, 'from', owner

class cobject(object):
    def __init__ (self, **argv):
        for key in argv: self.__dict__[key] = argv[key]
    def __str__ (self):
        text = []
        for k in self.__dict__:
            if k[:2] != '__': text.append('%s=%s'%(k, self.__dict__[k]))
        return ', '.join(text)





class game_service(service):

    def __init__ (self, sid = 0):
        super(game_service, self).__init__(sid)
        commands = {
            MSG_CS_LOGIN: self.login,
            MSG_SC_CONFIRM:self.confirm,
            MSG_SC_ADDUSER:self.adduser,
            MSG_SC_DELUSER:self.deluser,
            MSG_CS_DELUSER:self.cs_deluser,

            MSG_SC_PLAYER_CREATE:self.create_player,

            MSG_CS_CHAT:self.cs_chat,
            MSG_SC_CHAT:self.sc_chat,

            MSG_SC_PLAYER_POS:self.sc_player_pos,
            MSG_CS_PLAYER_POS:self.cs_player_pos,

            MSG_SC_PLAYER_MOV:self.sc_player_mov,
            MSG_CS_PLAYER_MOV:self.cs_player_mov,

            MSG_SC_PLAYER_ROT:self.sc_player_rot,
            MSG_CS_PLAYER_ROT:self.cs_player_rot,

            MSG_SC_PLAYER_VEL:self.sc_player_vel,
            MSG_CS_PLAYER_VEL:self.cs_player_vel,

            MSG_CS_PLAYER_DAMAGE:self.cs_player_damage,


            MSG_CS_ENEMY_DAMAGE:self.cs_enemy_damage,

            MSG_CS_SHOOT_STATUS:self.cs_shoot_status,


        }
        self.registers(commands)


    def login(self, msg, owner):
        mcl = msg_cs_login().unmarshal(msg.data)
        # Allowed

        #密码确认
        if database.database.verify(mcl.name, mcl.password):
            new_user = gameworld.add_user(msg.client, mcl.name, mcl.password)
            #确认
            msc = msg_sc_confirm(new_user.name, new_user.uid, 1)
            msg.host.send(msg.client, msc.marshal())
        else:
            msc = msg_sc_confirm(mcl.name, -1, 0)
            msg.host.send(msg.client, msc.marshal())
            return


        #给玩家发送开始消息
        msb = msg_sc_start(new_user.uid)
        msg.host.send(new_user.conn, msb.marshal())

        pl = gameworld.create_player(new_user.conn, new_user.uid)

        gameworld.enter_world(pl)



    def adduser(self, msg, owner):
        #增加用户
       pass

    def confirm(self, msg, owner):
        pass

    def deluser(self, msg, owner):
        #删除场景中的对象
        print '删除玩家....uid: ', owner.uid
        uid = owner.uid
        gameworld.del_user(uid)

    def cs_deluser(self, msg, owner):
        mcd =  msg_cs_deluser().unmarshal(msg.data)
        gameworld.del_user(mcd.uid)


    def create_player(self, msg, owner):
        #向所有玩家发送创建消息
        #先将所有的玩家的信息在自己上面创建出来
        for player in gameworld.players.itervalues():
            msp = msg_sc_player_create(player.uid, player.position.x, player.position.y, player.position.z)
            msg.host.send(msg.client, msp.marshal())

    def cs_chat(self, msg, owner):
        #客户端发来的聊天
        print 'cs_chat: data: %r' % msg.data
        mcc = msg_cs_chat().unmarshal(msg.data)

        data = msg_sc_chat(uid = mcc.uid, text = mcc.text).marshal()
        for player in gameworld.players.itervalues():
            msg.host.send(player.conn, data)

    def sc_chat(self, msg, owner):
        #
        print 'sc_char: data: %r' % msg.data
        pass

    def sc_player_pos(self, msg, owner):
        #服务端发给客户端的位置消息
        pass

    def cs_player_pos(self, msg, owner):
        #客户端发给服务端的位置信息
        mcpp = msg_cs_player_pos().unmarshal(msg.data)

        gameworld.player_pos(mcpp.uid, mcpp.x, mcpp.y, mcpp.z)
        pass

    def sc_player_mov(self, msg, owner):
        #服务端发给客户端的移动消息
        pass

    def cs_player_mov(self, msg, owner):
        #客户端发给服务端的移动消息
        mcpm = msg_cs_player_mov().unmarshal(msg.data)

        #直接将消息转发出去
        mspm = msg_sc_player_mov(mcpm.uid, mcpm.x, mcpm.y, mcpm.z).marshal()
        for player in gameworld.players:
            if mcpm.uid != player.uid:
                msg.host.send(player.conn, mspm)

    def sc_player_vel(self, msg, owner):
        pass

    def cs_player_vel(self, msg, owner):
        mcpv = msg_cs_player_vel().unmarshal(msg.data)

                #直接将消息转发出去
        mspv = msg_sc_player_vel(mcpv.uid, mcpv.x, mcpv.y, mcpv.z).marshal()
        for player in gameworld.players:
            if mcpv.uid != player.uid:
                msg.host.send(player.conn, mspv)

    def sc_player_rot(self, msg, owner):
        #角色的旋转
        pass

    def cs_player_rot(self, msg, owner):
        #角色的旋转
        mcpr = msg_cs_player_rot().unmarshal(msg.data)

                #直接将消息转发出去
        mspr = msg_sc_player_rot(mcpr.uid, mcpr.rotx, mcpr.roty).marshal()
        for player in gameworld.players.itervalues():
            if mcpr.uid != player.uid:
                msg.host.send(player.conn, mspr)

        pass

    def sc_player_vel(self, msg, owner):
        pass

    def cs_player_vel(self, msg, owner):
        mspv = msg_sc_player_vel(mcpv.uid, mcpv.x, mcpv.y, mcpv.z).marshal()
        for player in gameworld.players:
            if mcpv.uid != player.uid:
                msg.host.send(player.conn, mspv)


    def cs_player_damage(self, msg, owner):
        #print '玩家造成伤害'
        mcpd = msg_cs_player_damage().unmarshal(msg.data)
        gameworld.player_apply_damage(mcpd.uid, mcpd.damage)


    def sc_enemy_add(self, msg, owner):
        #发送消息,创建怪物
        pass

    def cs_enemy_damage(self, msg, owner):
        #怪物伤害
        mced = msg_cs_enemy_damage().unmarshal(msg.data)
        gameworld.enemy_apply_damage(mced.eid, mced.uid, mced.damage)


    def cs_shoot_status(self, msg, owner):
        #print '开枪'
        mcst = msg_cs_shoot_status().unmarshal(msg.data)

        gameworld.player_shoot(mcst.uid,                                         \
                               mcst.ray_org_x, mcst.ray_org_y, mcst.ray_org_z,  \
                               mcst.ray_dir_x, mcst.ray_dir_y, mcst.ray_dir_z)

#全局分发函数
disp = dispatcher()
disp.register(0, game_service())


if __name__ == '__main__':
    disp = dispatcher()
    disp.register(0, test_service())

    msg1 = cobject(sid = 100, cid = 20)
    msg2 = cobject(sid = 100, cid = 30)
    errormsg1 = cobject(sid = 100, cid = 30)
    errormsg2 = cobject(sid = 200, cid = 20)
    disp.dispatch(msg1, 'client1')
    disp.dispatch(msg2, 'client2')
    disp.dispatch(errormsg1, 'client3')

