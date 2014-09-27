#!/usr/local/bin/python
# -*- coding: utf-8 -*-
#======================================================================
#
# netstream.py - network data stream operation interface
#
# NOTE: The Replacement of TcpClient
#
#======================================================================

import socket
import select
import struct
import time
import sys
import errno
from netstream import *
from utility import *
from events import *


#----------------------------------------------------------------------
# testing case
#----------------------------------------------------------------------
if __name__ == '__main__':
	sock = netstream(8)
	last = time.time()
	sock.connect('127.0.0.1', 2000)
	print sock.status()
	print 'error: ', sock.error()
	print NET_STATE_ESTABLISHED
	#result = sock.send('Hello, world !!')
	#print 'send result: ',  result
	stat = 0
	last = time.time()
	sock.nodelay(0)
	sock.nodelay(1)
	while 1:
		time.sleep(1.1)
		sock.process()
		# client side testing case:
		if stat == 0:
			if sock.status() == NET_STATE_ESTABLISHED:
				stat = 1
				#sock.send('Hello, world !!')
				last = time.time()
		elif stat == 1:
			if time.time() - last >= 3.0:
				msg = msg_cs_login('netease', 1).marshal()
				print msg_cs_login('neta', 23).hfmt;
				print msg_cs_login('neta', 23).bfmt;
				print 'msg: %r' % msg
				print 'umsg; ', msg_cs_login().unmarshal(msg)
				#sock.send('VVVV')
				sock.send(msg)
				stat = 2
		elif stat == 2:
			if time.time() - last >= 5.0:
				sock.send('exit')
				stat = 3


