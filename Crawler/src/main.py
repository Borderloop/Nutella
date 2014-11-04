from Crawlers import TradeTracker
from Crawlers import FeedCrawler
#from Crawlers import TradeDoubler
from Crawlers import Belboon
from threading import Thread
from Crawlers import Daisycon

t = Thread(target=TradeTracker.Crawler().main)
t.start()

t = Thread(target=FeedCrawler.Crawler().main)
t.start()

t = Thread(target=Belboon.Crawler().main)
t.start()

t = Thread(target=Daisycon.Crawler().main)
t.start()