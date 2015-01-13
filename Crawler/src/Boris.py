import os
import glob
from ConfigParser import SafeConfigParser
from threading import Thread

from Crawlers import TradeTracker, FeedCrawler, Effiliation, JacobElektronik
from Crawlers import Belboon
from Crawlers import Daisycon
from Crawlers import Bol
from Crawlers import LDLC
from Crawlers import Linkshare
from Crawlers import PepperjamNetwork
from Crawlers import Amazon


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

            while True:
                try:
                    for f in files:
                        os.remove(f)
                    break
                except WindowsError as e:
                    print e
                    raw_input("The above product feed is opened by a program, possibly Sublime. "
                              "Close the feed and press any key to continue")


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

    t = Thread(target=Effiliation.Crawler().main)
    t.start()

    t = Thread(target=LDLC.Crawler().main)
    t.start()

    t = Thread(target=Linkshare.Crawler().main)
    t.start()

    t = Thread(target=JacobElektronik.Crawler().main)
    t.start()

    t = Thread(target=PepperjamNetwork.Crawler().main)
    t.start()

    t = Thread(target=Amazon.Crawler().main)
    t.start()

main()