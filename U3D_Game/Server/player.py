#!/usr/bin/env python
# -*- coding: utf-8 -*-
import copy
from mymath.vector3 import *


class Player:
    def __init__(self, conn = 0, uid = 0, position = Vector3(0, 0, 0)):
        self.conn = conn        #网络连接
        self.uid = uid
        self.position = position
        self.life = 100

        self.dead = False

        self.killcount = 0

    def update(self):
        pass

    def apply_damage(self, damage):
        print 'player apply_damage: ', damage
        if damage <= 0:
            return
        self.life -= damage

        if self.life < 0:
            self.life = 0
            dead = True

    def is_death(self):
        return self.life == 0