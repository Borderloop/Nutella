'''
Created on 25 mrt. 2014

@author: Marnik
'''
import requests
from decimal import *

class Converter():
    
    def convert(self, a, c):
        # a = the input currency, b = the currency to convert to, c = the amount(value) to be converted.
        b = 'EUR'
        
        #Get json result with currencyrate
        url = ('http://rate-exchange.appspot.com/currency?from=%s&to=%s&q=1') % (a, b)
        result = requests.get(url)
        price = result.json()['v'] * float(c) #multiply the currency rate with the price to get the correct price

        #Make sure there are 2 digits after the '.'
        TWOPLACES = Decimal(10) ** -2
        price = Decimal(price).quantize(TWOPLACES)
        return price