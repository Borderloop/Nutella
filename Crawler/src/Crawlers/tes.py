import suds

url = "http://ws.webgains.com/aws.php"

#Create client and authenticate
client = suds.client.Client(url)
print client

print client.service.getPrograms("affiliate@borderloop.com", "Hoppa123", 12)