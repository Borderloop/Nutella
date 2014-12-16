import Queue
from ConfigParser import SafeConfigParser
import robotexclusionrulesparser
import urlparse
import cPickle as pickle

websiteQueue = Queue.Queue()
penaltyList = []
websites = ''
identifiers = dict()
robots = dict()
javascriptCrawler = dict()

# Parse the config file for all websites
configParser = SafeConfigParser()
configParser.read('C:/BorderSoftware/BorderBot2/settings/borderbot.ini')
websites = configParser.get('General', 'websites')
multiplier = float(configParser.get('General', 'multiplier'))
agentName = configParser.get('General', 'agentname')
robotsRefreshRate = int(configParser.get('General', 'robotsrefreshrate'))
urlFilesPath = configParser.get('General', 'urlfilespath')

# Keep track of when to shut down the program.
amountOfWebsites = 0
websitesFinished = 0
crawlerFinished = False

# Fill the queues before the start of the program.
# Add all urls listed in the websites' .txt to the url queue, and then add that queue to the websites queue.
websites = websites.split(';')

for website in websites:
    name = website[website.find('.')+1:website.rfind('.')]

    urlQueue = Queue.Queue()
    urlsFile = open(urlFilesPath + name + '.txt', 'r')

    urls = urlsFile.read().splitlines()

    for url in urls:
        urlQueue.put(url)
    urlsFile.close()

    websiteQueue.put([website, urlQueue, [2, 2, 2, 2, 2]])

    # Parse robots file
    robotParser = robotexclusionrulesparser.RobotFileParserLookalike()
    robotParser.set_url(urlparse.urljoin(website, 'robots.txt'))
    robotParser.read()
    robotParser.modified()
    robots[name] = robotParser

    # Get the javascript crawler value. Either True or False
    javascriptCrawler[name] = configParser.get(name, 'javascriptcrawler')

    amountOfWebsites += 1


def parseMultiplier():
    configParser.read('C:/BorderSoftware/BorderBot2/settings/borderbot.ini')
    return float(configParser.get('General', 'multiplier'))