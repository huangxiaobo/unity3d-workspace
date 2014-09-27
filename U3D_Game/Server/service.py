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
		return 0



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


if __name__ == '__main__':
	disp = dispatcher()
	disp.register(100, test_service())

	msg1 = cobject(sid = 100, cid = 20)
	msg2 = cobject(sid = 100, cid = 30)
	errormsg1 = cobject(sid = 100, cid = 30)
	errormsg2 = cobject(sid = 200, cid = 20)
	disp.dispatch(msg1, 'client1')
	disp.dispatch(msg2, 'client2')
	disp.dispatch(errormsg1, 'client3')

