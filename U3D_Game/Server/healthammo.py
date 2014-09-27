#!/usr/bin/env python
# -*- coding: utf-8 -*-
import copy
from mymath.vector3 import *
from gl import *

#装备
PACKAGE_HEALTH = 1
PACKAGE_AMMO = 2

HEALTHAMMO_DATA = [
    [PACKAGE_HEALTH, Vector3(-20.82831, 1, 11.36966)],
    [PACKAGE_HEALTH, Vector3(17.73441 , 1, -2.349801)],
]

class HealthAmmo:
    def __init__(self, hid = 0, ha_type = PACKAGE_HEALTH, position = None):

        self.hid = hid

        self.ha_ypte = ha_type

        self.position = position




