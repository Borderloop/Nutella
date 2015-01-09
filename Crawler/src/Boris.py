import os
import glob
from ConfigParser import SafeConfigParser
from threading import Thread

from Crawlers import TradeTracker, Effiliation
from Crawlers import FeedCrawler
from Crawlers import Belboon
from Crawlers import Daisycon
from Crawlers import Bol
from Crawlers import Wehkamp
from Crawlers import LDLC
from Crawlers import Linkshare


global productFeedsPath


def main():
    parseConfigFile()
    emptyDirectories()
    startCrawlers()


# Procedure to parse the config file
def parseConfigFile():
    parser = SafeConfigParser()
    parser.read('C:/BorderSoftware/Boris/settings/boris.ini')
    global productFeedsPath
    productFeedsPath = parser.get('General', 'productfeedspath')


# This procedure empties all the product feed folders.
def emptyDirectories():

    productFeedPaths = [x[0] for x in os.walk(productFeedsPath)]

    for path in productFeedPaths:
        if path != productFeedPaths[0] and 'BorderBot' not in path and "Rene's Toppertjes" not in path:
            files = glob.glob(path + '/*')
            for f in files:
                os.remove(f)


# This procedure starts all crawlers.
def startCrawlers():
    t = Thread(target=TradeTracker.Crawler().main)
    t.start()

    t = Thread(target=FeedCrawler.Crawler().main)
    t.start()

    t = Thread(target=Belboon.Crawler().main)
    t.start()

    t = Thread(target=Daisycon.Crawler().main)
    t.start()

    t = Thread(target=Bol.Crawler().main)
    t.start()

    t = Thread(target=Wehkamp.Crawler().main)
    t.start()

    t = Thread(target=Effiliation.Crawler().main)
    t.start()

    t = Thread(target=LDLC.Crawler().main)
    t.start()

    t = Thread(target=Linkshare.Crawler().main)
    t.start()

main()