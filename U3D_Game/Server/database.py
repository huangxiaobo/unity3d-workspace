#!/usr/bin/env python
# -*- coding: utf-8 -*-

#模拟数据库

class DataBase:
    def __init__(self):
        self.data = {
            'netease0' : '163',
            'netease1' : '163',
            'netease2' : '163',
        }

    def verify(self, username, password):
        if username in self.data.iterkeys():
            if password == self.data[username]:
                return True
            else:
                return False
        else:
            return False


database = DataBase()