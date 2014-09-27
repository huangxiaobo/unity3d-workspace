#!/usr/bin/python
# -*- coding: utf-8 -*
##############################################################
'''
¹¤¾ßº¯Êý
'''

HEADER = u'\033[95m'
OKBLUE = u'\033[94m'
OKGREEN = u'\033[92m'
WARNING = u'\033[93m'
FAIL = u'\033[91m'
ENDC = u'\033[0m'
BOLD = u"\033[1m"

def disable():
    HEADER = u''
    OKBLUE = u''
    OKGREEN = u''
    WARNING = u''
    FAIL = u''
    ENDC = u''

_debug = False

def infog( msg):
    if _debug:
        print OKGREEN + msg + ENDC

def info( msg):
    if _debug:
        print OKBLUE + msg + ENDC

def warn( msg):
    if _debug:
        print WARNING + msg + ENDC

def err( msg):
    if _debug:
        print FAIL + msg + ENDC

def print_caller(func):
    def func_wrap(*args, **kwargs):
        infog(u'===> %s' % func.func_name)
        return func(*args, **kwargs)
    return func_wrap;

##############################################################
