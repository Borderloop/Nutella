import Statics

from threading import Thread
import time
import importlib

from Crawlers import getgoods


class BorderBot():

    def runThread(self):
        while Statics.websiteQueue.empty() is False:
            self.main(Statics.websiteQueue.get())

    # Controls the main flow of the BorderBot
    def main(self, websiteItem):
        website = websiteItem[0]
        url = websiteItem[1].get()

        name = website[website.find('.')+1:website.rfind('.')]

        if Statics.robots[name].can_fetch(Statics.agentName, url) is True:  # Don't crawl if the website blocked our bot.

            crawler = importlib.import_module('Crawlers.' + name)
            result = crawler.Crawler(website, url).main()

            # URLs are returned if the result does not equal None, add them to the queue.
            if result[0] is not None:
                websiteItem = self.addToQueue(result[0], websiteItem)

            timeResult = self.calculateAvgTime(result[1], websiteItem[2])
            timeInterval = timeResult[0]
            websiteItem[2] = timeResult[1]
            print 'Timeinterval: ' + str(timeInterval)
            crawlDelay = Statics.robots[name].get_crawl_delay(Statics.agentName)

            # Respect the crawl-delay if present
            if crawlDelay is None:
                # If this raises an error, the websiteItem doesn't have a time interval yet so add it.
                try:
                    websiteItem[3] = time.time() + timeInterval
                except IndexError:
                    websiteItem.append(time.time() + timeInterval)
            else:
                try:
                    websiteItem[3] = time.time() + crawlDelay
                except IndexError:
                    websiteItem.append(time.time() + crawlDelay)

            Statics.penaltyList.append(websiteItem)

    # This procedure adds URLS returned by a thread to the corresponding queue.
    def addToQueue(self, result, websiteItem):
        for url in result:
            websiteItem[1].put(url)
        return websiteItem

    # This procedure calculates the time interval, which determines how long a website will stay
    # in the penalty queue, and thus prevent from being crawled. This is to prevent a server overload.
    def calculateAvgTime(self, iterationTime, times):
        # Replace last time in times list with the most recent iteration time.
        times.pop()
        times = [iterationTime] + times

        totalTime = 0
        for time in times:
            totalTime += time

        averageTime = totalTime / 5
        return [averageTime * Statics.multiplier, times]


class T(Thread):
    def __init__(self):
        Thread.__init__(self)

    def run(self):
        BorderBot().runThread()

# Check the penalty list for expired items once each second
while True:
    for item in Statics.penaltyList:
        if time.time() > item[3]:
            Statics.websiteQueue.put(item)
            Statics.penaltyList.remove(item)

    # Start a new thread if the website queue is not empty to achieve maximum performance.
    if Statics.websiteQueue.empty() is False:
        T().start()

    # Check if the robots should be parsed again
    for robot in Statics.robots:
        age = int(time.time() - Statics.robots[robot].mtime())
        if age > Statics.robotsRefreshRate:
            Statics.robots[robot].read()
            Statics.robots[robot].modified()

    time.sleep(1)