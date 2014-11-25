import Queue
from ConfigParser import SafeConfigParser
import robotexclusionrulesparser
import urlparse

websiteQueue = Queue.Queue()
penaltyList = []
websites = ''

# Parse the config file for all websites
parser = SafeConfigParser()
parser.read('C:/BorderSoftware/BorderBot/settings/borderbot.ini')
websites = parser.get('General', 'websites')
multiplier = float(parser.get('General', 'multiplier'))
agentName = parser.get('General', 'agentname')
robotsRefreshRate = int(parser.get('General', 'robotsrefreshrate'))

robots = dict()

# Fill the queues before the start of the program
websites = websites.split(';')
for website in websites:  # We need all homepages in the url queues at the start.
    urlQueue = Queue.Queue()
    urlQueue.put(website)
    websiteQueue.put([website, urlQueue, [2, 2, 2, 2, 2]])

    # Parse robots file
    name = website[website.find('.')+1:website.rfind('.')]
    parser = robotexclusionrulesparser.RobotFileParserLookalike()
    parser.set_url(urlparse.urljoin(website, 'robots.txt'))
    parser.read()
    parser.modified()
    robots[name] = parser