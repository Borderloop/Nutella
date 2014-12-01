import logging
from time import strftime
from ConfigParser import SafeConfigParser
import os


# Check if the directory for the current day already exists. If not, create it.
def checkDir():
    newHandler = False
    curDate = strftime("%d-%m-%Y")
    logDir = parseConfigFile()

    if not os.path.exists(logDir + curDate):
        os.makedirs(logDir + curDate)
        newHandler = True

    return [logDir + curDate + '/', newHandler]


# Parses the config file to retrieve the log directory.
def parseConfigFile():
    parser = SafeConfigParser()
    parser.read('C:/BorderSoftware/BorderBot/settings/borderbot.ini')
    return parser.get('General', 'logdirectory')


# This procedure is called to log a request made to an server.
def logRequest(websiteName, reqTime, url, httpResponse, respTime, contentType):
    result = checkDir()
    directory = result[0]
    newHandler = result[1]

    requestLogger = logging.getLogger(websiteName)
    requestLogger.setLevel(logging.INFO)

    if not requestLogger.handlers:
        handler = logging.FileHandler(directory + websiteName + '.txt')
        handler.setLevel(logging.INFO)

        formatter = logging.Formatter('%(message)s')
        handler.setFormatter(formatter)
        requestLogger.addHandler(handler)

    if newHandler is True:
        requestLogger.removeHandler(handler)

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


def logError(websiteName, reqTime, url, error=None, httpResponse=None):
    result = checkDir()
    directory = result[0]
    newHandler = result[1]

    errorLogger = logging.getLogger(websiteName + 'Error')
    errorLogger.setLevel(logging.ERROR)

    if not errorLogger.handlers:
        handler = logging.FileHandler(directory + websiteName + 'Error.txt')
        handler.setLevel(logging.ERROR)

        formatter = logging.Formatter('%(message)s')
        handler.setFormatter(formatter)
        errorLogger.addHandler(handler)

    if newHandler is True:
        errorLogger.removeHandler(handler)

        handler = logging.FileHandler(directory + websiteName + 'Error.txt')
        handler.setLevel(logging.ERROR)

        formatter = logging.Formatter('%(message)s')
        handler.setFormatter(formatter)
        errorLogger.addHandler(handler)

    errorLogger.error('Time of request: ' + str(reqTime))
    errorLogger.error('Requested URL: ' + url)
    if httpResponse != None:
        errorLogger.error('HTTP Response: ' + str(httpResponse))
    if error != None:
        errorLogger.error('Error: ' + str(error))
    errorLogger.error('')


# This procedure is called to log an executed update or insert query.
def logQuery(com, vals, queryTime):
    result = checkDir()
    directory = result[0]
    newHandler = result[1]

    queryLogger = logging.getLogger('querys')
    queryLogger.setLevel(logging.INFO)

    if not queryLogger.handlers:
        handler = logging.FileHandler(directory + 'queries.txt')
        handler.setLevel(logging.INFO)

        formatter = logging.Formatter('%(message)s')
        handler.setFormatter(formatter)
        queryLogger.addHandler(handler)

    if newHandler is True:
        queryLogger.removeHandler(handler)

        handler = logging.FileHandler(directory + 'queries.txt')
        handler.setLevel(logging.INFO)

        formatter = logging.Formatter('%(message)s')
        handler.setFormatter(formatter)
        queryLogger.addHandler(handler)

    queryLogger.info('Time of query: ' + str(queryTime))
    queryLogger.info(com % tuple(vals))
    queryLogger.info('')