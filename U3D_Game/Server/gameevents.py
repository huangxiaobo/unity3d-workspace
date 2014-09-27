import sys
import gameheader
from gl import *

MSG_CS_LOGIN	= 0x1001
MSG_SC_CONFIRM	= 0x1002
MSG_SC_WAIT     = 0x1003
MSG_SC_START    = 0x1004

MSG_CS_MOVETO	= 0x2001
MSG_SC_MOVETO	= 0x2002

MSG_CS_CHAT		= 0x3001
MSG_SC_CHAT		= 0x3002

MSG_SC_ADDUSER	= 0x4001
MSG_SC_DELUSER	= 0x4002
MSG_CS_DELUSER  = 0x4003


MSG_SC_ENEMY_CREATE = 0X5003

MSG_SC_PLAYER_CREATE = 0X6001

MSG_SC_PLAYER_POS = 0X7001
MSG_CS_PLAYER_POS = 0X7002
MSG_SC_PLAYER_MOV = 0X7003
MSG_CS_PLAYER_MOV = 0X7004
MSG_SC_PLAYER_ROT = 0X7005
MSG_CS_PLAYER_ROT = 0X7006
MSG_SC_PLAYER_VEL = 0X7007
MSG_CS_PLAYER_VEL = 0X7008
MSG_SC_PLAYER_LIFE = 0X7009
MSG_CS_PLAYER_LIFE = 0X700A
MSG_SC_PLAYER_KILLCOUNT = 0X700B
MSG_CS_PLAYER_KILLCOUNT = 0X700C
MSG_SC_PLAYER_DAMAGE =  0X700D
MSG_CS_PLAYER_DAMAGE =  0X700E

MSG_CS_PLAYER_EXIT = 0x700F

MSG_SC_ENEMY_ADD  = 0X8001
MSG_CS_ENEMY_ADD  = 0X8002
MSG_SC_ENEMY_DIE  = 0X8003
MSG_CS_ENEMY_DIE  = 0X8004
MSG_CS_ENEMY_MOVE = 0x8005
MSG_SC_ENEMY_MOVE = 0X8006
MSG_SC_ENEMY_DAMAGE = 0X8007
MSG_CS_ENEMY_DAMAGE = 0X8008
MSG_SC_ENEMY_STATUS = 0X8009
MSG_CS_ENEMY_STATUS = 0X800A
MSG_CS_ENEMY_POS = 0x800B
MSG_SC_ENEMY_POS = 0X800C
MSG_CS_ENEMY_ROT = 0x800D
MSG_SC_ENEMY_ROT = 0X800E


MSG_SC_SHOOT_STATUS = 0X9001
MSG_CS_SHOOT_STATUS = 0X9002

MSG_SC_HA_PICK = 0XA001
MSG_CS_HA_PICK = 0XA002
MSG_SC_HA_CREATE = 0XA003
MSG_CS_HA_CREATE = 0XA004

class msg_cs_login(gameheader.lazy_header):
    def __init__ (self, name = '', password = -1):
        super (msg_cs_login, self).__init__ (MSG_CS_LOGIN)
        self.append_param('name', name, 's')
        self.append_param('password', password, 's')

class msg_sc_wait(gameheader.lazy_header):
    def __init__ (self, uid = 0):
        super (msg_sc_wait, self).__init__ (MSG_SC_WAIT)
        self.append_param('uid', uid, 'i')

class msg_sc_start(gameheader.lazy_header):
    def __init__ (self, uid = 0):
        super (msg_sc_start, self).__init__ (MSG_SC_START)
        self.append_param('uid', uid, 'i')


class msg_sc_confirm(gameheader.lazy_header):
    def __init__ (self, name = 0, uid = 0, result = 0):
        super (msg_sc_confirm, self).__init__ (MSG_SC_CONFIRM)
        self.append_param('name', name, 's')
        self.append_param('uid', uid, 'i')
        self.append_param('result', result, 'i')

class msg_cs_moveto(gameheader.lazy_header):
    def __init__ (self, x = 0, y = 0, z = 0):
        super (msg_cs_moveto, self).__init__ (MSG_CS_MOVETO)
        self.append_param('x', x, 'f')
        self.append_param('y', y, 'f')
        self.append_param('z', z, 'f')

class msg_sc_moveto(gameheader.lazy_header):
    def __init__ (self, uid = 0, x = 0, y = 0, z = 0):
        super (msg_sc_moveto, self).__init__ (MSG_SC_MOVETO)
        self.append_param('uid', uid, 'i')
        self.append_param('x', x, 'f')
        self.append_param('y', y, 'f')
        self.append_param('z', z, 'f')

class msg_cs_chat(gameheader.lazy_header):
    def __init__ (self, uid = 0, text = ''):
        super (msg_cs_chat, self).__init__ (MSG_CS_CHAT)
        self.append_param('uid', uid, 'i')
        self.append_param('text', text, 's')

class msg_sc_chat(gameheader.lazy_header):
    def __init__ (self, uid = 0, text = ''):
        super (msg_sc_chat, self).__init__ (MSG_SC_CHAT)
        self.append_param('uid', uid, 'i')
        self.append_param('text', text, 's')

class msg_sc_adduser(gameheader.lazy_header):
    def __init__ (self, uid = 0, name = '', x = 0, y = 0):
        super (msg_sc_adduser, self).__init__ (MSG_SC_ADDUSER)
        self.append_param('uid', uid, 'i')
        self.append_param('name', name, 's')
        self.append_param('x', x, 'i')
        self.append_param('y', y, 'i')

class msg_sc_deluser(gameheader.lazy_header):
    def __init__ (self, uid = 0):
        super (msg_sc_deluser, self).__init__ (MSG_SC_DELUSER)
        self.append_param('uid', uid, 'i')

class msg_cs_deluser(gameheader.lazy_header):
    def __init__ (self, uid = 0):
        super (msg_cs_deluser, self).__init__ (MSG_CS_DELUSER)
        self.append_param('uid', uid, 'i')

class msg_cs_enemy_move(gameheader.lazy_header):
    def __init__(self, eid = 0, x = 0, y = 0, z = 0):
        super(msg_cs_enemy_move, self).__init__(MSG_CS_ENEMY_MOVE)
        self.append_param('eid', eid, 'i')
        self.append_param('x', x, 'f')
        self.append_param('y', y, 'f')
        self.append_param('z', z, 'f')

class msg_sc_player_create(gameheader.lazy_header):
    def __init__(self, uid = 0, x = 0, y = 0, z = 0):
        super(msg_sc_player_create, self).__init__(MSG_SC_PLAYER_CREATE)
        self.append_param('uid', uid, 'i')
        self.append_param('x', x, 'f')
        self.append_param('y', y, 'f')
        self.append_param('z', z, 'f')



class msg_sc_player_pos(gameheader.lazy_header):
    def __init__(self, uid = 0, x = 0, y = 0, z = 0):
        super(msg_sc_player_pos, self).__init__(MSG_SC_PLAYER_POS)
        self.append_param('uid', uid, 'i')
        self.append_param('x', x, 'f')
        self.append_param('y', y, 'f')
        self.append_param('z', z, 'f')

class msg_cs_player_pos(gameheader.lazy_header):
    def __init__(self, uid = 0, x = 0, y = 0, z = 0):
        super(msg_cs_player_pos, self).__init__(MSG_CS_PLAYER_POS)
        self.append_param('uid', uid, 'i')
        self.append_param('x', x, 'f')
        self.append_param('y', y, 'f')
        self.append_param('z', z, 'f')

class msg_sc_player_mov(gameheader.lazy_header):
    def __init__(self, uid = 0, x = 0, y = 0, z = 0):
        super(msg_sc_player_mov, self).__init__(MSG_SC_PLAYER_MOV)
        self.append_param('uid', uid, 'i')
        self.append_param('x', x, 'f')
        self.append_param('y', y, 'f')
        self.append_param('z', z, 'f')

class msg_cs_player_mov(gameheader.lazy_header):
    def __init__(self, uid = 0, x = 0, y = 0, z = 0):
        super(msg_cs_player_mov, self).__init__(MSG_CS_PLAYER_MOV)
        self.append_param('uid', uid, 'i')
        self.append_param('x', x, 'f')
        self.append_param('y', y, 'f')
        self.append_param('z', z, 'f')

class msg_sc_player_vel(gameheader.lazy_header):
    def __init__(self, uid = 0, x = 0, y = 0, z = 0):
        super(msg_sc_player_vel, self).__init__(MSG_SC_PLAYER_VEL)
        self.append_param('uid', uid, 'i')
        self.append_param('x', x, 'f')
        self.append_param('y', y, 'f')
        self.append_param('z', z, 'f')

class msg_cs_player_vel(gameheader.lazy_header):
    def __init__(self, uid = 0, x = 0, y = 0, z = 0):
        super(msg_cs_player_vel, self).__init__(MSG_CS_PLAYER_VEL)
        self.append_param('uid', uid, 'i')
        self.append_param('x', x, 'f')
        self.append_param('y', y, 'f')
        self.append_param('z', z, 'f')

class msg_sc_player_rot(gameheader.lazy_header):
    def __init__(self, uid = 0, rotx = 0, roty = 0):
        super(msg_sc_player_rot, self).__init__(MSG_SC_PLAYER_ROT)
        self.append_param('uid', uid, 'i')
        self.append_param('rotx', rotx, 'f')
        self.append_param('roty', roty, 'f')

class msg_cs_player_rot(gameheader.lazy_header):
    def __init__(self, uid = 0, rotx = 0, roty = 0):
        super(msg_cs_player_rot, self).__init__(MSG_CS_PLAYER_ROT)
        self.append_param('uid', uid, 'i')
        self.append_param('rotx', rotx, 'f')
        self.append_param('roty', roty, 'f')

class msg_sc_player_life(gameheader.lazy_header):
    def __init__(self, uid = 0, life = 0):
        super(msg_sc_player_life, self).__init__(MSG_SC_PLAYER_LIFE)
        self.append_param('uid', uid, 'i')
        self.append_param('life', life, 'i')

class msg_cs_player_life(gameheader.lazy_header):
    def __init__(self, uid = 0, life = 0):
        super(msg_cs_player_life, self).__init__(MSG_CS_PLAYER_LIFE)
        self.append_param('uid', uid, 'i')
        self.append_param('life', life, 'i')

class msg_sc_player_killcount(gameheader.lazy_header):
    def __init__(self, uid = 0, killcount = 0):
        super(msg_sc_player_killcount, self).__init__(MSG_SC_PLAYER_KILLCOUNT)
        self.append_param('uid', uid, 'i')
        self.append_param('killcount', killcount, 'i')

class msg_cs_player_killcount(gameheader.lazy_header):
    def __init__(self, uid = 0, killcount = 0):
        super(msg_cs_player_killcount, self).__init__(MSG_CS_PLAYER_KILLCOUNT)
        self.append_param('uid', uid, 'i')
        self.append_param('killcount', killcount, 'i')

class msg_sc_player_damage(gameheader.lazy_header):
    def __init__(self, uid = 0, eid = 0, damage = 0):
        super(msg_sc_player_damage, self).__init__(MSG_SC_PLAYER_DAMAGE)
        self.append_param('uid', uid, 'i')
        self.append_param('eid', eid, 'i')
        self.append_param('damage', damage, 'i')

class msg_cs_player_damage(gameheader.lazy_header):
    def __init__(self, uid = 0, eid = 0, damage = 0):
        super(msg_cs_player_damage, self).__init__(MSG_CS_PLAYER_DAMAGE)
        self.append_param('uid', uid, 'i')
        self.append_param('eid', eid, 'i')
        self.append_param('damage', damage, 'i')

class msg_sc_enemy_create(gameheader.lazy_header):
    def __init__(self, eid = 0, x = 0, y = 0, z = 0, life = 100, enemy_type = ENEMY_TYPE_NO):
        super(msg_sc_enemy_create, self).__init__(MSG_SC_ENEMY_CREATE)
        self.append_param('eid', eid, 'i')
        self.append_param('x', x, 'f')
        self.append_param('y', y, 'f')
        self.append_param('z', z, 'f')
        self.append_param('life', life, 'i')
        self.append_param('enemy_type', enemy_type, 'i')

class msg_sc_enemy_add(gameheader.lazy_header):
     def __init__(self, eid = 0, x = 0, y = 0, z = 0, life = 100, enemy_type = ENEMY_TYPE_NO):
        super(msg_sc_enemy_add, self).__init__(MSG_SC_ENEMY_ADD)
        self.append_param('eid', eid, 'i')
        self.append_param('x', x, 'f')
        self.append_param('y', y, 'f')
        self.append_param('z', z, 'f')
        self.append_param('life', life, 'i')
        self.append_param('enemy_type', enemy_type, 'i')

class msg_sc_enemy_pos(gameheader.lazy_header):
     def __init__(self, eid = 0, x = 0, y = 0, z = 0):
        super(msg_sc_enemy_pos, self).__init__(MSG_SC_ENEMY_POS)
        self.append_param('eid', eid, 'i')
        self.append_param('x', x, 'f')
        self.append_param('y', y, 'f')
        self.append_param('z', z, 'f')

class msg_cs_enemy_pos(gameheader.lazy_header):
     def __init__(self, eid = 0, x = 0, y = 0, z = 0):
        super(msg_cs_enemy_pos, self).__init__(MSG_CS_ENEMY_POS)
        self.append_param('eid', eid, 'i')
        self.append_param('x', x, 'f')
        self.append_param('y', y, 'f')
        self.append_param('z', z, 'f')

class msg_sc_enemy_rot(gameheader.lazy_header):
     def __init__(self, eid = 0, rotx = 0.0, roty = 0.0, rotz = 0.0):
        super(msg_sc_enemy_rot, self).__init__(MSG_SC_ENEMY_ROT)
        self.append_param('eid', eid, 'i')
        self.append_param('rotx', rotx, 'f')
        self.append_param('roty', roty, 'f')
        self.append_param('rotz', rotz, 'f')

class msg_cs_enemy_rot(gameheader.lazy_header):
     def __init__(self, eid = 0, rotx = 0.0, roty = 0.0, rotz = 0.0):
        super(msg_cs_enemy_rot, self).__init__(MSG_CS_ENEMY_ROT)
        self.append_param('eid', eid, 'i')
        self.append_param('rotx', rotx, 'f')
        self.append_param('roty', roty, 'f')
        self.append_param('rotz', rotz, 'f')

class msg_sc_enemy_die(gameheader.lazy_header):
     def __init__(self, eid = 0):
        super(msg_sc_enemy_die, self).__init__(MSG_SC_ENEMY_DIE)
        self.append_param('eid', eid, 'i')

class msg_sc_enemy_damage(gameheader.lazy_header):
     def __init__(self, eid = 0, uid = 0, damage =100):
        super(msg_sc_enemy_damage, self).__init__(MSG_SC_ENEMY_DAMAGE)
        self.append_param('eid', eid, 'i')
        self.append_param('uid', uid, 'i')
        self.append_param('damage', damage, 'i')


class msg_cs_enemy_damage(gameheader.lazy_header):
     def __init__(self, eid = 0, uid = 0, damage =100):
        super(msg_cs_enemy_damage, self).__init__(MSG_CS_ENEMY_DAMAGE)
        self.append_param('eid', eid, 'i')
        self.append_param('uid', uid, 'i')
        self.append_param('damage', damage, 'i')


class msg_sc_shoot_status(gameheader.lazy_header):
     def __init__(self, uid = 0, ray_org_x = 0.0, ray_org_y = 0.0, ray_org_z = 0.0, \
                  ray_dir_x = 0.0, ray_dir_y = 0.0, ray_dir_z = 0.0):
        super(msg_sc_shoot_status, self).__init__(MSG_SC_SHOOT_STATUS)
        self.append_param('uid', uid, 'i')
        self.append_param('ray_org_x', ray_org_x, 'f')
        self.append_param('ray_org_y', ray_org_y, 'f')
        self.append_param('ray_org_z', ray_org_z, 'f')
        self.append_param('ray_dir_x', ray_dir_x, 'f')
        self.append_param('ray_dir_y', ray_dir_y, 'f')
        self.append_param('ray_dir_z', ray_dir_z, 'f')

class msg_cs_shoot_status(gameheader.lazy_header):
     def __init__(self, uid = 0, ray_org_x = 0.0, ray_org_y = 0.0, ray_org_z = 0.0, \
                  ray_dir_x = 0.0, ray_dir_y = 0.0, ray_dir_z = 0.0):
        super(msg_cs_shoot_status, self).__init__(MSG_CS_SHOOT_STATUS)
        self.append_param('uid', uid, 'i')
        self.append_param('ray_org_x', ray_org_x, 'f')
        self.append_param('ray_org_y', ray_org_y, 'f')
        self.append_param('ray_org_z', ray_org_z, 'f')
        self.append_param('ray_dir_x', ray_dir_x, 'f')
        self.append_param('ray_dir_y', ray_dir_y, 'f')
        self.append_param('ray_dir_z', ray_dir_z, 'f')


class msg_sc_ha_create(gameheader.lazy_header):
     def __init__(self, hid = 0, x = 0, y = 0, z = 0):
        super(msg_sc_ha_create, self).__init__(MSG_SC_HA_CREATE)
        self.append_param('hid', hid, 'i')
        self.append_param('x', x, 'f')
        self.append_param('y', y, 'f')
        self.append_param('z', z, 'f')

class msg_sc_ha_pick(gameheader.lazy_header):
     def __init__(self, hid = 0, uid = 0):
        super(msg_sc_ha_pick, self).__init__(MSG_SC_HA_PICK)
        self.append_param('hid', hid, 'i')
        self.append_param('uid', uid, 'i')




if __name__ == '__main__':
    p1 = msg_cs_login()
    p1.name = 'PLAYER1'
    p1.icon = 10
    data = p1.marshal()
    print repr(data)
    p2 = msg_cs_login()
    p2.unmarshal(data)
    print p2.name, p2.icon


