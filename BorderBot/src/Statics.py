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

# Parse the config file for all websites
configParser = SafeConfigParser()
configParser.read('C:/BorderSoftware/BorderBot/settings/borderbot.ini')
websites = configParser.get('General', 'websites')
multiplier = float(configParser.get('General', 'multiplier'))
agentName = configParser.get('General', 'agentname')
robotsRefreshRate = int(configParser.get('General', 'robotsrefreshrate'))
identifiersPath = configParser.get('General', 'identifierspath')


# Fill the queues before the start of the program
websites = websites.split(';')
for website in websites:  # We need all homepages in the url queues at the start.
    urlQueue = Queue.Queue()
    urlQueue.put(website)
    websiteQueue.put([website, urlQueue, [2, 2, 2, 2, 2]])

    # Parse robots file
    name = website[website.find('.')+1:website.rfind('.')]
    robotParser = robotexclusionrulesparser.RobotFileParserLookalike()
    robotParser.set_url(urlparse.urljoin(website, 'robots.txt'))
    robotParser.read()
    robotParser.modified()
    robots[name] = robotParser

    # Get identifiers dictionary file
    identifier = pickle.load(open(identifiersPath + name + ".p", "rb"))
    identifiers[name] = identifier


def parseMultiplier():
    configParser.read('C:/BorderSoftware/BorderBot/settings/borderbot.ini')
    return float(configParser.get('General', 'multiplier'))