import re
import math
from ConfigParser import SafeConfigParser

from Handlers import Database


class Parser():
    def __init__(self, soup, page, website, url):
        self.parseConfigFile()
        self.soup = soup
        self.page = page
        self.website = website
        self.url = url

    db = Database.Queries()
    urls = []
    disallowedCategories = []

    # Procedure that controls the main flow of the parser.
    def main(self):
        self.urls = []

        if self.page == 'home':
            self.parseHomePage()
        elif self.page == 'subCategory':
            self.parseSubCategoryPage()
        elif self.page == 'products':
            self.parseProductsPage()
        elif self.page == 'product':
            self.parseProductPage()

        return self.urls

    # Procedure to parse the config file
    def parseConfigFile(self):
        parser = SafeConfigParser()
        parser.read('C:/BorderSoftware/BorderBot/settings/borderbot.ini')
        self.disallowedCategories = parser.get('getgoods', 'disallowedcategories').split(',')

    # This procedure parses the home page
    def parseHomePage(self):
        categories = self.soup.find('div', {'id': 'mainNavigation'}).find_all('li')
        for category in categories:
            if category != categories[0] and category != categories[-1] and category != categories[-2] \
                    and category != categories[-3] and category != categories[-6]:
                self.urls.append(category.find('a')['href'])

    # This procedure parses the subcategory page
    def parseSubCategoryPage(self):
        categories = self.soup.find('div', {'id': 'mainNavigationSecondLevel'}).find_all('li')
        for category in categories:
            if category.find('a').text not in self.disallowedCategories:
                self.urls.append(category.find('a')['href'])

    # This procedure parses the products page, the page which enlists multiple products.
    def parseProductsPage(self):
        # If it's the first time the products page is crawled for a category, add pagination and return the new url.
        if self.url.find('?n=') == -1:
            self.url += '?n=48'
            self.urls.append(self.url)
            return

        products = self.soup.find('div', {'id': 'listing-1col'}).find_all('div', recursive=False)

        for product in products:
            if product.find('p', {'class': 'deliverable_3'}) is None:  # Product not available
                head = product.find('div', {'class': 'head'})
                productId = head.find('span').text
                productId = productId[productId.find(':')+1:]

                # We found a new product, get the URL to the product page to add it to the queue.
                if self.checkIfParsedBefore(productId, self.website) == ():
                    self.urls.append(head.find('a')['href'])
                else:  # Else the product is already parsed and we only need to get price and availability data.
                    price = product.find('span', {'class': 'pricenumber'}).text
                    price = self.cleanPrice(price)
                    availability = product.find('div', {'class': 'delivery_container'}).find('p').text.strip()

                    self.db.openConnection()
                    self.db.updateArticle(productId, self.website, price, availability)
                    self.db.closeConnection()

        self.checkForNextPage()

    # This procedure parses the product page. If it gets here it means it's a new product,
    # so it always saves the product which is parsed here to the database.
    def parseProductPage(self):
        ean = None
        sku = None
        productID = None

        if 'Dieser Artikel ist leider zur Zeit nicht' in str(self.soup) \
                or 'Dieser Artikel ist leider nicht mehr verf' in str(self.soup):  # Not available
            return

        title = self.soup.find('div', {'class': 'detailheader'}).find('h1').text

        data = self.soup.find('div', {'class': 'ordernumber_data'}).find_all('p')
        for item in data:
            if 'Artikel' in item.text:
                productID = item.text[item.text.find(':')+1:].strip()
            elif 'Hersteller' in item.text:
                sku = item.text[item.text.find(':')+1:].strip()
            elif 'EAN' in item.text:
                ean = item.text[item.text.find(':')+1:].strip()

        availability = self.soup.find('div', {'id': 'article_details'}).find('p').text.strip()

        try:
            price = self.soup.find('div', {'class': 'specialCategoryTextColor article_details_price'}).find('strong').text
        except AttributeError:  # Day deal
            price = self.soup.find('strong', {'class': 'liveshoppingprice'}).text
        price = self.cleanPrice(price)

        try:
            imageLocation = self.soup.find('a', {'id': 'zoom1'})['href']
        except TypeError:
            imageLocation = None

        self.db.openConnection()
        self.db.insertNewArticle(self.website, productID, title, price, self.url, availability=availability, ean=ean, sku=sku, imageLocation=imageLocation)
        self.db.closeConnection()


    # This procedure checks if a product has been parsed before.
    def checkIfParsedBefore(self, productID, website):
        self.db.openConnection()
        result = self.db.selectProduct(productID, website)
        self.db.closeConnection()

        return result

    # This method gets rid of all other characters in a price.
    def cleanPrice(self, price):
        price = price.strip().replace('.', '').replace(',', '.')
        non_decimal = re.compile(r'[^\d.]+')
        return non_decimal.sub('', price)

    # Procedure to check if there's a next page and if so, add it to the urls.
    def checkForNextPage(self):
        totalProducts = self.soup.find('div', {'class': 'article_page_and_count'}).text
        totalProducts = totalProducts[totalProducts.find('von')+3:totalProducts.find('Produkte')-1].strip()
        totalPages = int(math.ceil(int(totalProducts) / 48.0))

        try:
            currentPage = self.soup.find('a', {'class': 'navi on'}).text

            if self.url.find('p=') == -1:
                self.urls.append(self.url + '&p=' + str(int(currentPage) + 1))
            else:
                currentPage = self.url[self.url.find('p=')+2:]
                if int(currentPage) < int(totalPages):
                    url = self.url[:self.url.rfind('=')+1] + str(int(currentPage) + 1)
                    self.urls.append(url)
        except AttributeError:  # Only one page for this category.
            pass