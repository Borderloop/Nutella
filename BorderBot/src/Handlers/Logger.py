import logging
from time import strftime
from ConfigParser import SafeConfigParser
import os


# Check if the directory for the current day already exists. If not, create it.
def checkDir():
    curDate = strftime("%d-%m-%Y")
    logDir = parseConfigFile()

    if not os.path.exists(logDir + curDate):
        os.makedirs(logDir + curDate)

    return logDir + curDate + '/'


# Parses the config file to retrieve the log directory.
def parseConfigFile():
    parser = SafeConfigParser()
    parser.read('C:/BorderSoftware/BorderBot/settings/borderbot.ini')
    return parser.get('General', 'logdirectory')


# This procedure is called to log a request made to an server.
def logRequest(websiteName, reqTime, url, httpResponse, respTime, contentType):
    directory = checkDir()

    requestLogger = logging.getLogger(websiteName)
    requestLogger.setLevel(logging.INFO)
    #logging.basicConfig(level=logging.INFO)
    #logging.getLogger("requests").setLevel(logging.WARNING)

    if not requestLogger.handlers:
        handler = logging.FileHandler(directory + websiteName + '.txt')
        handler.setLevel(logging.INFO)

        formatter = logging.Formatter('%(message)s')
        handler.setFormatter(formatter)
        requestLogger.addHandler(handler)

    requestLogger.info('Time of request: ' + str(reqTime))
    requestLogger.info('Requested URL: ' + url)
    requestLogger.info('HTTP Response: ' + str(httpResponse))
    requestLogger.info('Response time: ' + str(respTime))
    requestLogger.info('Content-Type: ' + contentType)
    requestLogger.info('')


# This procedure is called to log an executed update or insert query.
def logQuery(com, vals, queryTime):
    directory = checkDir()

    queryLogger = logging.getLogger('querys')
    queryLogger.setLevel(logging.INFO)

    if not queryLogger.handlers:
        handler = logging.FileHandler(directory + 'queries.txt')
        handler.setLevel(logging.INFO)

        formatter = logging.Formatter('%(message)s')
        handler.setFormatter(formatter)
        queryLogger.addHandler(handler)

    queryLogger.info('Time of query: ' + str(queryTime))
    queryLogger.info(com % tuple(vals))
    queryLogger.info('')