using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Threading;
using System.Text.RegularExpressions;
using BorderSource.Common;
using BorderSource.ProductAssociation;
using BorderSource.Loggers;

namespace BorderSource.Affiliate.Reader
{
    public class EffiliationLecteur : AffiliateReaderBase
    {
        public override string Name { get { return "Effiliation"; } }

        public override IEnumerable<List<Product>> ReadFromFile(string fichier)
        {
            // Initialiser le XmlValueReader et votrekeys clés
            using (XmlValueReader valeurLecteur = new XmlValueReader())
            {
                List<Product> produits = new List<Product>();
                string fichierUrl = Path.GetFileNameWithoutExtension(fichier).Split(null)[0].Replace('$', '/');

                valeurLecteur.ProductEnd = "product";
                valeurLecteur.AddKeys("ean", XmlNodeType.Element);
                valeurLecteur.AddKeys("name", XmlNodeType.Element);
                valeurLecteur.AddKeys("brand", XmlNodeType.Element);
                valeurLecteur.AddKeys("price", XmlNodeType.Element);
                valeurLecteur.AddKeys("currency", XmlNodeType.Element);
                valeurLecteur.AddKeys("url_product", XmlNodeType.Element);
                valeurLecteur.AddKeys("url_image", XmlNodeType.Element);
                valeurLecteur.AddKeys("merchant_category_name", XmlNodeType.Element);
                valeurLecteur.AddKeys("description", XmlNodeType.Element);
                valeurLecteur.AddKeys("delivery_time", XmlNodeType.Element);
                valeurLecteur.AddKeys("in_stock", XmlNodeType.Element);
                valeurLecteur.AddKeys("shipping_cost", XmlNodeType.Element);
                valeurLecteur.AddKeys("sku", XmlNodeType.Element);

                Product produit = new Product();

                valeurLecteur.CreateReader(fichier, new XmlReaderSettings { CloseInput = true });
                foreach (DualKeyDictionary<string, XmlNodeType, string> dkd in valeurLecteur.ReadProducts())
                {
                    produit.EAN = dkd["ean"][XmlNodeType.Element];
                    produit.SKU = dkd["sku"][XmlNodeType.Element];
                    produit.Title = dkd["name"][XmlNodeType.Element];
                    produit.Brand = dkd["brand"][XmlNodeType.Element];
                    produit.Price = dkd["price"][XmlNodeType.Element];
                    produit.Currency = dkd["currency"][XmlNodeType.Element];
                    produit.Url = dkd["url_product"][XmlNodeType.Element];
                    produit.Image_Loc = dkd["url_image"][XmlNodeType.Element];
                    produit.Category = dkd["merchant_category_name"][XmlNodeType.Element];
                    produit.Description = dkd["description"][XmlNodeType.Element];
                    produit.DeliveryCost = dkd["shipping_cost"][XmlNodeType.Element];
                    produit.DeliveryTime = dkd["delivery_time"][XmlNodeType.Element];
                    produit.Stock = dkd["in_stock"][XmlNodeType.Element];
                    produit.Affiliate = "Effiliation";
                    produit.FileName = fichier;
                    produit.Webshop = fichierUrl;
                    
                    //  Performer le SHA256 encryption parce que le Effliation ne donner pas une unique id
                    produit.AffiliateProdID = (produit.Title + produit.Webshop).ToSHA256();

                    produits.Add(produit);
                    produit = new Product();
                    if (produits.Count >= PackageSize)
                    {
                        yield return produits;
                        produits.Clear();
                    }
                }

                yield return produits;
            }
        }

        [Obsolete]
        public override IEnumerable<List<Product>> ReadFromDir(string dir)
        {
            Console.WriteLine("Tenté d'utiliser une vieille méthode");
            yield break;
        }
    }
}

