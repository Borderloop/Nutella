'''
Created on 17 jan. 2014

@author: Marnik
'''
import time
from datetime import date

#This class calculates the time it will take for a product to be available again.
class TimeCalculator():
    
    def __init__(self, date):
        self.date = date
        self.months = ('januari', 'februari', 'maart', 'april', 'mei', 'juni', 'juli', 'augustus', 'september', 'oktober', 'november', 'december')
        
    def caluclateTime(self):
        #First calculate the current date and convert it to a date type.
        curTime = time.strftime("%d/%m/%Y")
        curTime = curTime.split("/")
        curDate = date(int(curTime[2]), int(curTime[1]), int(curTime[0]))
        
        #Next, calculate the number for the date on which the product will be available. Example: Januari must become 01
        self.date = self.date.split() #Split the day from month first
        availabilityMonth = self.months.index(self.date[1]) +1

        #If the current date is 11(december) and the availabilitydate is lower, it means it is in the next year.
        if (availabilityMonth < int(curTime[1])) or (availabilityMonth <= int(curTime[1]) and int(self.date[0]) <= int(curTime[0])):
            availabilityYear = int(curTime[2]) + 1
        else:
            availabilityYear = curTime[2]
        
        availabilityDate = date(int(availabilityYear), int(availabilityMonth), int(self.date[0]))
        
        #Calculate the difference between the two dates. Then return only the days.
        dateDifference = availabilityDate - curDate
        dateDifference = str(dateDifference).split()
        return dateDifference[0]
        