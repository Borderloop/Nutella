import osa

url = "http://api.daisycon.com/publisher/soap/program/wsdl/"
cl = osa.Client(url)

print cl.service