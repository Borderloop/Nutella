'''
Created on 24 jul. 2014

@author: Marnik
'''
from Tkinter import *
from Crawlers import Zanox, WebsiteParser
from datetime import datetime
from threading import Timer, Thread

text = "Welcome"


class GUI():
        
    def createGUI(self): #Create the GUI with the buttons
        root = Tk()
        root.title("Crawler")
        f1 = Frame(root)
        
        #Set buttons and label
        b1 = Button(f1, text="Force start", width=15, command=self.createCrawlerThread)
        b2 = Button(f1, text="Parse websites", width=15, command=self.createWebsiteParserThread)
        self.l = Label(root, text=text, pady=10)
        
        #Pack all buttons and label
        b1.pack(side=LEFT, padx=10, pady=10)
        b2.pack(side=LEFT, padx=10, pady=10)
        
        f1.pack()
        self.l.pack()
        
        thread = Thread(target = self.runTimer)
        root.after(500, thread.start())
        root.mainloop()
        
        
    def runTimer(self): #This method checks for specific times to run the crawler
        
        while True:
            x=datetime.today()
            if x.hour < 8:#If it's before 08:00, set hour to run crawler to 08:00 and day to current day
                self.hour = 8
                self.day = x.day
            elif x.hour >= 8 and x.hour < 13:#Else if it's between 08:00 and 13:00, set hour to run crawler to 13PM and day to current day
                self.hour = 13
                self.day = x.day
            elif x.hour >= 13 and x.hour < 19:#Else if it's between 13:00 and 19:00, set hour to run crawler to 19:00 and day to current day
                self.hour = 19
                self.day = x.day
            else: #Else, run crawler 08:00 the next day
                self.hour = 8
                self.day = x.day+1
            
            y=x.replace(day=self.day, hour=self.hour, minute=00, second=00, microsecond=0)
            
            delta_t = y-x
            secs=delta_t.seconds+1
            
            self.l['text'] = 'Standby. Next run on: ' + str(self.hour) + ":00"
            
            t1 = Timer(secs, self.runCrawler)
            t1.start()
            t1.join()
    
        
    def runCrawler(self):
        self.l['text'] = 'Crawler busy with Zanox'
        thread = Zanox.Thread()
        thread.start()
        thread.join()
        
        self.l['text'] = 'Standby. Next run on: ' + str(self.hour) + ":00"
        
    #If the parse websites button is clicked, run the website parser.    
    def parseWebsites(self):
        self.l['text'] = 'Parsing websites'
        thread = WebsiteParser.Thread()
        thread.start()
        thread.join()
        
        self.l['text'] = 'Standby. Next run on: ' + str(self.hour) + ":00"
        
    def createCrawlerThread(self):
        thread = Thread(target = self.runCrawler)
        thread.start()
    
    def createWebsiteParserThread(self):
        thread = Thread(target = self.parseWebsites)
        thread.start()