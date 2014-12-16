import re
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
        elif self.page == 'products':
            self.parseProductsPage()
        elif self.page == 'product':
            self.parseProductPage()

        return self.urls

    # Procedure to parse the config file
    def parseConfigFile(self):
        parser = SafeConfigParser()
        parser.read('C:/BorderSoftware/BorderBot/settings/borderbottemp2.ini')
        self.disallowedCategories = parser.get('hknumeric', 'disallowedcategories').split(',')

    # This procedure parses the home page
    def parseHomePage(self):
        categories = self.soup.find('ul', {'id': 'menu-custom'}).find_all('li', recursive=False)
        for category in categories:
            if category.find('a').text not in self.disallowedCategories:
                self.urls.append(category.find('a')['href'])

    # This procedure parses the products page, the page which enlists multiple products.
    def parseProductsPage(self):
        products = self.soup.find('ul', {'id': 'product_list'}).find_all('li', recursive=False)

        for product in products:
            title = product.find('h5').text

            if product.find('div', {'class', 'reconditionne_ribbon'}) is None:  # Skip refurbished articles.
                 # We found a new product, get the URL to the product page to add it to the queue.
                if self.checkIfParsedBefore(title, self.website) == ():
                    self.urls.append(product.find('a')['href'])
                else:  # Else the product is already parsed and we only need to get price and availability data.
                    price = product.find('span', {'class': 'price'}).text
                    price = self.cleanPrice(price)

                    try:
                        availability = product.find('span', {'class': 'dispo'}).text.strip()
                    except AttributeError:
                        availability = product.find('span', {'class': 'nondispo'}).text.strip()

                    self.db.openConnection()
                    self.db.updateArticle(title, self.website, price, availability)
                    self.db.closeConnection()

        self.checkForNextPage()

    # This procedure parses the product page. If it gets here it means it's a new product,
    # so it always saves the product which is parsed here to the database.
    def parseProductPage(self):
        title = self.soup.find('div', {'id': 'top_titles'}).find('h1').text
        productID = self.soup.find('p', {'id': 'product_reference'}).find('span').text
        imageLocation = self.soup.find('span', {'id': 'view_full_size'}).find('img')['src']
        category = self.soup.find('div', {'class': 'breadcrumb_inset'}).find_all('a')[-1].text
        price = self.soup.find('span', {'id': 'our_price_display'}).text

        price = self.cleanPrice(price)
        ean = productID

        try:
            availability = self.soup.find('span', {'class': 'dispo'}).text.strip()
        except AttributeError:
            availability = None

        if productID != '':
            self.db.openConnection()
            self.db.insertNewArticle(self.website, productID, title, price, self.url, availability=availability, category=category, ean=ean, imageLocation=imageLocation)
            self.db.closeConnection()

    # This procedure checks if a product has been parsed before.
    def checkIfParsedBefore(self, title, website):
        self.db.openConnection()
        result = self.db.selectProductByTitle(title, website)
        self.db.closeConnection()

        return result

    # This method gets rid of all other characters in a price.
    def cleanPrice(self, price):
        price = price.strip().replace('.', '').replace(',', '.')
        non_decimal = re.compile(r'[^\d.]+')
        return non_decimal.sub('', price)

    # Procedure to check if there's a next page and if so, add it to the urls.
    def checkForNextPage(self):
        try:
            nextPage = self.soup.find('li', {'id': 'pagination_next'}).find('a')['href']
            url = self.url[:self.url.find('/en/')] + nextPage
            self.urls.append(url)
        except TypeError:  # No next page
            pass