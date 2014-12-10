import urllib
from ConfigParser import SafeConfigParser
from openpyxl import load_workbook
import time
import gzip
import os
import traceback

from CrawlerHelpScripts import logger


class Crawler():
    def __init__(self):
        self.parseConfigFile()

    feedPath = ''
    xmlPath = ''
    user = ''
    password = ''
    sid = ''

    log = logger.createLogger("LinkshareLogger", "Linkshare")

    # This procedure controls the main flow of the program.
    def main(self):
        self.gatherData()

    # Procedure to parse the config file
    def parseConfigFile(self):
        parser = SafeConfigParser()
        parser.read('C:/BorderSoftware/Boris/settings/boris.ini')

        self.feedPath = parser.get('Linkshare', 'feedpath')
        self.xmlPath = parser.get('Linkshare', 'xmlpath')
        self.user = parser.get('Linkshare', 'user')
        self.password = parser.get('Linkshare', 'password')
        self.sid = parser.get('Linkshare', 'sid')

    # This procedure gathers the advertiser id and website url, needed for downloading and saving
    # the file.
    def gatherData(self):

        wb = load_workbook(filename=self.xmlPath, use_iterators=True)
        ws = wb.get_sheet_by_name('Sheet1')

        for row in ws.iter_rows(row_offset=1):
            for cell in row:
                # Affiliate id is in column A, the corresponding url in B. Get both and save it
                if cell.column == 'A':

                    try:
                        affiliateID = str(int(cell.value))
                        websiteURL = ws['B' + str(cell.row)].value
                    except TypeError:
                        break

                    self.save(affiliateID, websiteURL)

    # Downloads and saves the files under the correct name
    def save(self, affiliateID, websiteURL):
        print 'Saving ' + websiteURL

        # If the save fails, something is wrong with the file or directory name. Catch this error
        try:
            zipFile = urllib.URLopener()

            zipFile.retrieve('ftp://' + self.user + ':' + self.password + '@aftp.linksynergy.com/' + affiliateID + '_' +
                             self.sid + '_mp.txt.gz', self.feedPath + websiteURL + ".txt.gz")

            self.readZipFile(websiteURL)
            print websiteURL + 'saved.'
        except Exception as e:
            self.log.error(str(time.asctime(time.localtime(time.time()))) + ": " + str(e))
            self.log.info(str(time.asctime(time.localtime(time.time()))) + ": Failed:" + websiteURL)

    # This procedure is used to read zip files and extract their contents and afterwards delete the zip file.
    def readZipFile(self, website):
        data = ''

        zipFile = gzip.open(self.feedPath + website + '.txt.gz')
        csvFile = open(self.feedPath + website + '.csv', 'a')

        for line in zipFile:
            csvFile.write(line)

        zipFile.close()
        csvFile.close()

        os.remove(self.feedPath + website + '.txt.gz')