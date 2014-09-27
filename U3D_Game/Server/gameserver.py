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
from netstream import *
import gameservice
import user
from gameevents import *
import gl

if __name__ == '__main__':


    #服务器
    host = nethost(8)
    host.startup(2000)
    host.settimer(5000)
    print 'service startup at port', host.port

    gl.gameworld.host = host        #地图主机

    client_map = {}

    while True:
        #time.sleep(0.01)


        host.process()

        # server side testing case:
        event, wparam, lparam, data = host.read()
        if event < 0: continue
        #print 'event=%d wparam=%xh lparam=%xh data="%s"'%(event, wparam, lparam, data)

        if event == NET_DATA:
            cid = struct.unpack('H',  data[0:2])[0]
            #print '消息类型: ', hex(cid)
            msg = gameservice.cobject(sid = 0, cid = cid,  data = data, host = host, client = wparam)
            gameservice.disp.dispatch(msg, gl.gameworld.find_user_by_conn(wparam))
        elif event ==NET_NEW:
            #用户的连接
            #忽略
            pass
        elif event == NET_LEAVE:
            #用户离开
            msg = gameservice.cobject(sid = 0, cid = MSG_SC_DELUSER,  data = data, host = host, client = wparam)
            u = gl.gameworld.find_user_by_conn(wparam)
            if u:
                gameservice.disp.dispatch(msg, u)
            pass
        elif event == NET_TIMER:
            #定时器
            pass