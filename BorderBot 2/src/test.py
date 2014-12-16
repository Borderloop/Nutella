f = open('C:/BorderSoftware/BorderBot2/urls/rakuten.txt', 'r')

urls = f.read().splitlines()

for url in urls:
    print url

f.close()