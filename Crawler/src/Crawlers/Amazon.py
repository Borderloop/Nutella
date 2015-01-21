from amazonproduct import API
from amazonproduct.errors import InvalidParameterValue
from amazonproduct.errors import AWSError
from xml.etree.ElementTree import ElementTree, Element, SubElement

from ConfigParser import SafeConfigParser

from CrawlerHelpScripts import logger

import traceback
import time


class Crawler():
    def __init__(self):
        self.parseConfigFile()

    asinTxtPath = ''
    locales = []
    feedPath = ''

    api = ''

    log = logger.createLogger("AmazonLogger", "Amazon")

    def parseConfigFile(self):
        parser = SafeConfigParser()
        parser.read('C:/BorderSoftware/Boris/settings/boris.ini')
        self.asinTxtPath = parser.get('Amazon', 'asintxtpath')
        self.locales = parser.get('Amazon', 'locales').split(',')
        self.feedPath = parser.get('Amazon', 'feedpath')

    def main(self):
        # Amazon consists of multiple webshops from different countries.
        for locale in self.locales:
            productDataList = []

            self.api = API(locale=locale)
            products = self.loadProducts(locale)

            for product in products:
                if product != '' and product is not None and product[0] != '#':  # Comment or blank line.
                    # Product contains two elements: The ASIN and the shipping cost, divided by `:`.
                    product = product.split(':')
                    ASIN = product[0]

                    productData = self.gatherData(ASIN, locale)

                    if productData is not None:  # Something went wrong retrieving data.
                        productData["shipping_cost"] = product[1]

                        # Add the product data to a list so we can convert the list to xml once all products are parsed.
                        productDataList.append(productData)

                    time.sleep(2)

            self.writeXML(productDataList, locale)

    # This procedure loads products from the .txt file corresponding with the locale.
    def loadProducts(self, locale):
        f = open(self.asinTxtPath + locale + '.txt')
        products = f.read().splitlines()
        f.close()

        return products

    # This procedure makes the API call and retrieves all necessary data from the response
    def gatherData(self, ASIN, locale):
        productData = dict()

        tries = 0
        while True:
            try:
                result = self.api.item_lookup(ASIN, ResponseGroup='Large')
                break
            except InvalidParameterValue:  # ID doesn't exist for this locale
                return
            except AWSError:  # Product not accessible through API
                self.log.info('Not accessible through API: ' + ASIN + ' - Locale: ' + locale)
                return
            except Exception as e:
                print 'Amazon timed out'
                print e
                tries += 1
                time.sleep(7)

                if tries == 20:
                    return

        for item in result.Items.Item:
            productData["asin"] = item.ASIN.text
            productData["deep_link"] = item.DetailPageURL.text
            productData["image_large"] = item.ImageSets.ImageSet.LargeImage.URL.text
            productData["image_medium"] = item.ImageSets.ImageSet.MediumImage.URL.text
            productData["image_small"] = item.ImageSets.ImageSet.SmallImage.URL.text
            productData["ean"] = item.ItemAttributes.EAN.text
            productData["category"] = item.ItemAttributes.Binding.text
            productData["title"] = item.ItemAttributes.Title.text
            try:
                productData["currency"] = item.Offers.Offer.OfferListing.Price.CurrencyCode.text
            except AttributeError:
                self.log.info("No offer data available for: " + ASIN + ' - Locale: ' + locale)
                break
            productData["price"] = item.Offers.Offer.OfferListing.Price.FormattedPrice.text

            try:
                productData["color"] = item.ItemAttributes.Color.text
            except AttributeError:  # Some locales don't contain color.
                productData["color"] = ''
            try:
                productData["brand"] = item.ItemAttributes.Brand.text
            except AttributeError:  # Some products, like dvd's, don't have a brand.
                productData["brand"] = ''

            return productData

    # This procedure converts the product data to a xml file.
    def writeXML(self, productDataList, locale):
        root = Element('Products')

        for productData in productDataList:
            product = SubElement(root, 'Product')

            for data in productData:
                child = SubElement(product, data)

                child.text = productData[data]

        if locale == 'uk':
            locale = 'co.uk'

        ElementTree(root).write(self.feedPath + 'www.amazon.' + locale + '.xml', encoding='UTF-8')

        print 'Wrote XML for www.amazon.' + locale