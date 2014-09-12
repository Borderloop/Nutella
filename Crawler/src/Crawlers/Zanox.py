import suds
from suds.xsd.doctor import ImportDoctor, Import

url = "https://api.zanox.com/wsdl/2011-03-01"

# WSDL fails to import schema, import this and create client.
imp = Import('http://schemas.xmlsoap.org/soap/encoding/')
imp.filter.add('http://api.daisycon.com/publisher/soap//program/')
d = ImportDoctor(imp)

client = suds.client.Client(url, doctor=d) 

print client