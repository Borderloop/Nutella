''' This module crawls multiple affiliates, stored in the feed_urls.xlsx file'''

from openpyxl import load_workbook
import urllib
from CrawlerHelpScripts import logger
import time
import traceback

class Crawler():
    
    log = logger.createLogger("FeedCrawlerLogger", "FeedCrawler")
    
    #Import the excel file with websites
    wb=load_workbook(r'C:\Crawler\feed_urls.xlsx', use_iterators = True)
    ws=wb.get_sheet_by_name('Sheet1')
    
    prevName = "" #Used to check if the previous campaign is from the same company.
    x = 2 #Used to name files of companies that use multiple xml files
    
    def main(self):
        start_time = time.time()
        self.log.info(str(time.asctime( time.localtime(time.time()) ))+": Starting with FeedCrawler")
        
        self.gatherData()
        
        self.log.info(str(time.asctime( time.localtime(time.time()) ))+": Finished crawling FeedCrawler affiliates in: " + str((time.time() - start_time)))
    
    #Gathers the data needed for downloading and correct file name    
    def gatherData(self):
        #Iterate trough all rows
        for row in self.ws.iter_rows(row_offset=1):
            for cell in row:
                if cell.column == 'A':# #Column A contains the affiliate name.
                    affiliate = cell.value
                if cell.column == 'B': #Column B contains the name of the website.
                    website = cell.value
                elif cell.column == 'C': #Else if column is C, it contains the product feed download url. 
                    url = cell.value
                    
                    #If there's a semicolon, there are multiple product feeds for this company so save them all and add a number.
                    if url.find(';') != -1: 
                        urls = url.split(';')
                        x = 1
                        for url in urls:
                            websiteWithNumber = website + " " + str(x)
                            self.save(affiliate, url, websiteWithNumber)
                            x = x+1
                    else: #Else it's just one url, save this
                        self.save(affiliate,url, website)
                        
            
    #Downloads and saves the xml file under the correct name           
    def save(self,affiliate, url, website):
        #Download xml file and write it to a file
        self.log.info(str(time.asctime( time.localtime(time.time()) ))+": Saving xml file for: " + affiliate + " - " + website)
        
        #If the save failes, something is wrong with the file or directory name. Catch this error
        try:
            xmlFile = urllib.URLopener()
            xmlFile.retrieve(url, "C:/Crawler/Product Feeds/" + affiliate + "/" + website + ".xml")
            self.log.info(str(time.asctime( time.localtime(time.time()) ))+": Done saving xml file for: " + affiliate + " - " + website)
            
        except:
            self.log.error(str(time.asctime( time.localtime(time.time()) ))+ ": " + traceback.format_exc())
            self.log.info(str(time.asctime( time.localtime(time.time()) ))+": Failed saving xml file for: " + affiliate + " - " + website + ". See error log for more details")
        