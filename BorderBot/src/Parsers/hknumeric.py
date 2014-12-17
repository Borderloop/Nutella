import re
from ConfigParser import SafeConfigParser
import codecs

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
        with codecs.open('C:/BorderSoftware/BorderBot/settings/borderbot.ini', 'r', encoding='utf-8') as f:
            parser.readfp(f)
        self.disallowedCategories = parser.get('hknumeric', 'disallowedcategories')

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
            self.urls.append(product.find('a')['href'])

        self.checkForNextPage()

    # This procedure parses the product page.
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

        # Product hasn't been parsed before, insert it.
        if self.checkIfParsedBefore(productID, self.website.replace('/en/', '')) == () and productID != '':
            self.db.openConnection()
            self.db.insertNewArticle(self.website.replace('/en/', ''), productID, title, price, self.url, availability=availability, category=category, ean=ean, imageLocation=imageLocation)
            self.db.closeConnection()
        else:  # Known product, update only
            self.db.openConnection()
            self.db.updateArticle(productID, self.website.replace('/en/', ''), price)
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
        try:
            nextPage = self.soup.find('li', {'id': 'pagination_next'}).find('a')['href']
            url = self.url[:self.url.find('/en/')] + nextPage
            self.urls.append(url)
        except (TypeError, AttributeError):  # No next page
            pass