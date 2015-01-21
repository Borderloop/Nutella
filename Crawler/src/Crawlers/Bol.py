from ftplib import FTP
from ConfigParser import SafeConfigParser

import time


class Crawler():
    def __init__(self):
        self.parseConfigFile()

    feedPath = ''
    ftpFilePath = ''

    host = ''
    user = ''
    password = ''

    # This procedure controls the main flow of the program. Due to a small crawler,
    # every functionality is in this procedure required for downloading the product feeds
    # is coded in this procedure.
    def main(self):
        # Login to HTTP
        ftp = FTP(host=self.host)
        ftp.login(self.user, self.password)

        # Read the file containing the names of all files that need to be downloaded.
        with open(self.ftpFilePath + 'bol.txt')as f:
            fileNames = f.readlines()

        for file in fileNames:
            print 'Crawling ' + file.strip()
            f = open(self.feedPath + file.strip(), 'w')

            tries = 0
            while True:
                try:
                    ftp.retrbinary('RETR %s' %file.strip(), f.write)
                except:
                    print 'Bol timed out'
                    tries += 1
                    time.sleep(7)

                    if tries == 20:
                        break

            f.close()
            print 'Done crawling ' + file.strip()

        ftp.quit()

    # Procedure to parse the config file
    def parseConfigFile(self):
        parser = SafeConfigParser()
        parser.read('C:/BorderSoftware/Boris/settings/boris.ini')
        self.feedPath = parser.get('Bol', 'feedpath')
        self.ftpFilePath = parser.get('General', 'ftpfilespath')

        self.host = parser.get('Bol', 'host')
        self.user = parser.get('Bol', 'user')
        self.password = parser.get('Bol', 'password').replace('"', '')