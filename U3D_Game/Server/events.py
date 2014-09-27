import sys
import header

MSG_CS_LOGIN	= 0x1001
MSG_SC_CONFIRM	= 0x2001

MSG_CS_MOVETO	= 0x1002
MSG_SC_MOVETO	= 0x2002

MSG_CS_CHAT		= 0x1003
MSG_SC_CHAT		= 0x2003

MSG_SC_ADDUSER	= 0x2004
MSG_SC_DELUSER	= 0x2005


class msg_cs_login(header.lazy_header):
	def __init__ (self, name = '', icon = -1):
		super (msg_cs_login, self).__init__ (MSG_CS_LOGIN)
		self.append_param('name', name, 's')
		self.append_param('icon', icon, 'i')

class msg_sc_confirm(header.lazy_header):
	def __init__ (self, uid = 0, result = 0):
		super (msg_sc_confirm, self).__init__ (MSG_SC_CONFIRM)
		self.append_param('uid', uid, 'i')
		self.append_param('result', result, 'i')

class msg_cs_moveto(header.lazy_header):
	def __init__ (self, x = 0, y = 0):
		super (msg_cs_moveto, self).__init__ (MSG_CS_MOVETO)
		self.append_param('x', x, 'i')
		self.append_param('y', y, 'i')

class msg_sc_moveto(header.lazy_header):
	def __init__ (self, uid = 0, x = 0, y = 0):
		super (msg_sc_moveto, self).__init__ (MSG_SC_MOVETO)
		self.append_param('uid', uid, 'i')
		self.append_param('x', x, 'i')
		self.append_param('y', y, 'i')

class msg_cs_chat(header.lazy_header):
	def __init__ (self, text = ''):
		super (msg_cs_chat, self).__init__ (MSG_CS_CHAT)
		self.append_param('text', text, 's')

class msg_sc_chat(header.lazy_header):
	def __init__ (self, uid = 0, text = ''):
		super (msg_sc_chat, self).__init__ (MSG_SC_CHAT)
		self.append_param('uid', uid, 'i')
		self.append_param('text', text, 's')

class msg_sc_adduser(header.lazy_header):
	def __init__ (self, uid = 0, name = '', x = 0, y = 0):
		super (msg_sc_adduser, self).__init__ (MSG_SC_ADDUSER)
		self.append_param('uid', uid, 'i')
		self.append_param('name', name, 's')
		self.append_param('x', x, 'i')
		self.append_param('y', y, 'i')

class msg_sc_deluser(header.lazy_header):
	def __init__ (self, uid = 0):
		super (msg_sc_deluser, self).__init__ (MSG_SC_DELUSER)
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


