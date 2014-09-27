#!/usr/bin/env python
# -*- coding: utf-8 -*-
import copy
from mymath.vector3 import *

UID = [0]


class User:
    def __init__(self, conn = 0, name = '', password = ''):
        self.conn = conn        #网络连接
        self.name = name
        self.password = password
        self.uid = UID[0] +1
        UID[0] += 1




