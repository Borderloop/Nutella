import logging
import os

'''
This class constructs the logger for every module requesting it. It takes 2 parameters:
logName assigns the name to the logger, fileName assigns the name used by the log files.
'''


def createLogger(logName, fileName):
    os.chdir('C:\BorderSoftware\Boris\log')
    log = logging.getLogger(logName)
    log.setLevel(logging.DEBUG)
    formatter = logging.Formatter('[%(levelname)s] %(message)s')
    handler_stream = logging.StreamHandler()
    handler_stream.setFormatter(formatter)
    handler_stream.setLevel(logging.ERROR)
    log.addHandler(handler_stream)
    handler_file = logging.FileHandler('%sInfo.log' % fileName)
    handler_file2 = logging.FileHandler('%sErrors.log' % fileName)
    handler_file.setFormatter(formatter)
    handler_file2.setFormatter(formatter)
    handler_file2.setLevel(logging.ERROR)
    log.addHandler(handler_file)
    log.addHandler(handler_file2)
    
    return log
