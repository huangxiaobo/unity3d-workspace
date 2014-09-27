#!/usr/bin/env python
# -*- coding: utf-8 -*-

from mymath.vector3 import *

#敌人的类型
ENEMY_TYPE_NR = 1       #普通敌人 不会主动攻击
ENEMY_TYPE_AI = 2       #智能敌人,能主动攻击


#装备
PACKAGE_HEALTH = 1
PACKAGE_AMMO = 2

#敌人
ENEMY_DATA = [
    #位置, 血量, 类型
    [Vector3(7.792586, 1, -6.273241), 100, ENEMY_TYPE_NR],
    [Vector3(-17.14053, 1, -6.273241), 100, ENEMY_TYPE_AI],
    [Vector3(-7.908734, 1, 9.428045), 100, ENEMY_TYPE_NR],
    [Vector3(14.84364, 1, 12.40837), 100, ENEMY_TYPE_NR],
]

class Enemy:
    def __init__(self, eid = '', position = Vector3(0, 0, 0), life = 100, enemy_type = ENEMY_TYPE_NR):
        #eneymy id
        self.eid = eid
        #positon
        self.position = position

        self.life = life

        self.enemy_type = enemy_type
        #可见范围
        self.see_range = 10

        self.attack_range = 0.5

        self.attack_value = 5

    def update(self):
        pass

    def death(self):
        if self.life <= 0:
            return True
        else:
            return False

    def move(self, mov):
        if mov is not None:
            self.position += mov

    def can_see_target(self, target):
        v = Vector3(target.position - self.position).get_length()
        #print '距离:  ', v
        if v > self.see_range:
            return False
        else:
            return True

    def can_attack(self, target):
        if self.enemy_type == ENEMY_TYPE_NR:
            return False

        v = Vector3(target.position - self.position).get_length()

        if v > self.attack_range:
            return False
        else:
            return True

    def attack(self, target):
        if self.can_attack(self, target):
            self.life = 0
            target.apply_damage(self.attack_value)