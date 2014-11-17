import requests
from bs4 import BeautifulSoup
from ConfigParser import SafeConfigParser
import Database
import re
import math
import time


class Crawler():
    def __init__(self, website, url):
        self.website = website
        self.url = url

    # These variables store the tags, elements and contents which are used to determine the page
    productsPage = ''
    subCategoryPage = ''
    productPage = ''

    soup = ''

    result = ''

    # Procedure to control the main flow of the crawler
    def main(self):
        self.headers = {
            'User-Agent': 'Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2049.0 Safari/537.36'
        }

        r = requests.get(self.url, headers=self.headers)
        data = r.text
        self.soup = BeautifulSoup(data, 'html.parser')
        self.checkPage()
        return self.result

    # Procedure to check which page of the website is being crawled
    def checkPage(self):

        if self.url[self.url.rfind('.'):].find('/') == -1:  # Home page
            self.result = Parser(self.soup, 'home', self.website, self.url).main()
            return

        # Identifiers contain tags, elements and contents.
        identifiers = dict('div', 'id', 'listing-1col',
                           'div', 'id', 'center',
                           'div', 'id', 'detailbox')

        for identifier in identifiers:
            if self.soup.find(identifiers[identifier][0], {identifiers[identifier][1]: identifiers[identifier][2]}) is not None:
                # If it's the product page, add extension to the url for more products per page.
                # Then crawl the site again.
                if identifier == 'products' and self.url.find('?n=') == -1:
                    self.url += '?n=48'
                    r = requests.get(self.url, headers=self.headers)
                    data = r.text
                    self.soup = BeautifulSoup(data, 'html.parser')

                self.result = Parser(self.soup, identifier, self.website, self.url).main()
                break


class Parser():
    def __init__(self, soup, page, website, url):
        self.soup = soup
        self.page = page
        self.website = website
        self.url = url

    db = Database.Queries()
    urls = []

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

    # This procedure parses the home page
    def parseHomePage(self):
        categories = self.soup.find('div', {'id': 'mainNavigation'}).find_all('li')
        for category in categories:
            if category != categories[0] and category != categories[-1] and category != categories[-2]:
                self.urls.append(category.find('a')['href'])

    # This procedure parses the subcategory page
    def parseSubCategoryPage(self):
        categories = self.soup.find('div', {'id': 'mainNavigationSecondLevel'}).find_all('li')
        for category in categories:
            self.urls.append(category.find('a')['href'])

    # This procedure parses the products page, the page which enlists multiple products.
    def parseProductsPage(self):
        products = self.soup.find('div', {'id': 'listing-1col'}).find_all('div', recursive=False)

        for product in products:
            title = product.find('a', {'class': 'title'})

            # We found a new product, get the URL to the product page to add it to the queue.
            if self.checkIfParsedBefore(title.text, self.website) is None:
                self.urls.append(title['href'])
            else:  # Else the product is already parsed and we only need to get price and availability data.
                price = self.soup.find('span', {'class': 'pricenumber'}).text
                price = self.cleanPrice(price)
                availability = self.soup.find('p', {'class': 'deliverable_1'}).text.strip()

                self.db.openConnection()
                self.db.updateArticle(title.text, self.website, price, availability)
                self.db.closeConnection()

        # Check if there's a next page and if so, add it to the urls.
        totalProducts = self.soup.find('div', {'class': 'article_page_and_count'}).text
        totalProducts = totalProducts[totalProducts.find('von')+3:totalProducts.find('Produkten')-1].strip()
        totalPages = int(math.ceil(int(totalProducts) / 48.0))
        currentPage = self.soup.find('a', {'class': 'navi on'}).text

        if int(currentPage) < int(totalPages):
            if self.url.find('p=') == -1:
                self.urls.append(self.url + 'p=' + str(int(currentPage) + 1))
            else:
                url = self.url[:self.url.rfind('=')+1] + str(int(currentPage) + 1)
                self.urls.append(url)

    # This procedure parses the product page. If it gets here it means it's a new product,
    # so it always saves the product which is parsed here to the database.
    def parseProductPage(self):
        title = self.soup.find('div', {'class': 'detailheader'}).find('h1').text
        brand = self.soup.find('div', {'class': 'PageHeader'}).text

        data = self.soup.find('div', {'class': 'ordernumber_data'}).find_all('p')
        ean = data[2].text[data[2].text.find(':')+1:].strip()
        sku = data[1].text[data[1].text.find(':')+1:].strip()
        availability = self.soup.find('p', {'class': 'deliverable_1'}).text.strip()

        price = self.soup.find('div', {'class': 'specialCategoryTextColor article_details_price'}).text
        price = self.cleanPrice(price)

        imageLocation = self.soup.find('a', {'id': 'zoom1'})['href']

        self.db.openConnection()
        self.db.insertNewArticle(title, self.website, brand, price, availability, self.url, ean=ean, sku=sku, imageLocation=imageLocation)
        self.db.closeConnection()


    # This procedure checks if a product has been parsed before.
    def checkIfParsedBefore(self, title, website):
        self.db.openConnection()
        result = self.db.selectTitle(title, website)
        self.db.closeConnection()

        return result

    # This method gets rid of all other characters in a price.
    def cleanPrice(self, price):
        price = price.strip().replace(',', '.')
        non_decimal = re.compile(r'[^\d.]+')
        return non_decimal.sub('', price)