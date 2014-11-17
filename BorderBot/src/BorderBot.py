import Queue
from ConfigParser import SafeConfigParser
import GetGoods
from threading import Thread

websiteQueue = Queue.Queue()
penaltyQueue = Queue.Queue()
websites = ''

# Parse the config file for all websites
parser = SafeConfigParser()
parser.read('C:/BorderSoftware/Boris/settings/borderbot.ini')
websites = parser.get('General', 'websites')

# Fill the queues before the start of the program
websites = websites.split(';')
for website in websites:  # We need all homepages in the url queues at the start.
    urlQueue = Queue.Queue()
    urlQueue.put(website)
    websiteQueue.put([website, urlQueue])


class BorderBot():

    def runThread(self):
        while websiteQueue.empty() == False:
            self.main(websiteQueue.get())

    # Controls the main flow of the BorderBot
    def main(self, websiteItem):
        website = websiteItem[0]
        url = websiteItem[1].get()

        result = GetGoods.Crawler(website, url).main()

        # URLs are returned if the result does not equal None
        if result is not None:
            websiteItem = self.addToQueue(result, websiteItem)

        websiteQueue.put(websiteItem)

    # This procedure adds URLS returned by a thread to the corresponding queue.
    def addToQueue(self, result, websiteItem):
        for url in result:
            websiteItem[1].put(url)
        return websiteItem


class T(Thread):
    def __init__(self):
        Thread.__init__(self)

    def run(self):
        BorderBot().runThread()

T().start()