from ftplib import FTP
from ConfigParser import SafeConfigParser


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
            ftp.retrbinary('RETR %s' %file.strip(), f.write)
            f.close()

        ftp.quit()

    # Procedure to parse the config file
    def parseConfigFile(self):
        parser = SafeConfigParser()
        parser.read('C:/BorderSoftware/Boris/settings/boris.ini')
        self.feedPath = parser.get('Bol', 'feedpath')
        self.ftpFilePath = parser.get('General', 'ftpfilespath')

        self.host = parser.get('Bol', 'host')
        self.user = parser.get('Bol', 'user')
        self.password = parser.get('Bol', 'password')