using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace ProductFeedReader
{
    public class Logger : StreamWriter
    {
        /// <summary>
        /// An array to store data.
        /// </summary>
        private int[] _data;

        /// <summary>
        /// A set containing all unique EAN numbers.
        /// </summary>
        private SortedSet<string> _eans;

        /// <summary>
        /// A set containing all unique SKU numbers.
        /// </summary>
        private SortedSet<string> _skus;

        /// <summary>
        /// A set containing all unique categories.
        /// </summary>
        private SortedSet<string> _cats;

        /// <summary>
        /// A Stopwatch for measuring the elapsed time (useful fo optimizing).
        /// </summary>
        private Stopwatch _sw;

        public bool Verbose { get; set; }

        public Logger(string logPath) : base(logPath) 
        {
            _sw = new Stopwatch();

            _data = new int[18];
            _eans = new SortedSet<string>();
            _skus = new SortedSet<string>();
            _cats = new SortedSet<string>();
        }

        public void StartStopwatch()
        {
            _sw.Start();
        }

        public void StopStopwatch()
        {
            _sw.Stop();
        }

        public override void Close()
        {
            //Stop the stopwatch, work is done.
            _sw.Stop();

            //Write all data to logfile.            
            base.WriteLine("Last scan: " + DateTime.Now.ToString("HH:mm:ss") + ".");
            base.WriteLine("Processing time: " + _sw.Elapsed);
            base.WriteLine(_data[17] + " products processed.");
            base.WriteLine(_data[0] + " products have a valid EAN, which is " + Math.Round((double)((double)_data[0] / (double)_data[17]) * 100, 2) + @"%");
            base.WriteLine(_data[1] + " products have a valid SKU, which is " + Math.Round((double)((double)_data[1] / (double)_data[17]) * 100, 2) + @"%");
            base.WriteLine(_data[2] + " products have a valid name, which is " + Math.Round((double)((double)_data[2] / (double)_data[17]) * 100, 2) + @"%");
            base.WriteLine(_data[3] + " products have a valid brand, which is " + Math.Round((double)((double)_data[3] / (double)_data[17]) * 100, 2) + @"%");
            base.WriteLine(_data[4] + " products have a valid category, which is " + Math.Round((double)((double)_data[4] / (double)_data[17]) * 100, 2) + @"%");
            base.WriteLine(_data[5] + " products have a valid price, which is " + Math.Round((double)((double)_data[5] / (double)_data[17]) * 100, 2) + @"%");
            base.WriteLine(_data[6] + " products have a valid url, which is " + Math.Round((double)((double)_data[6] / (double)_data[17]) * 100, 2) + @"%");
            base.WriteLine(_data[7] + " products have a valid currency, which is " + Math.Round((double)((double)_data[7] / (double)_data[17]) * 100, 2) + @"%");
            base.WriteLine(_data[8] + " products have a valid description, which is " + Math.Round((double)((double)_data[8] / (double)_data[17]) * 100, 2) + @"%");
            base.WriteLine(_data[9] + " products have a valid image, which is " + Math.Round((double)((double)_data[9] / (double)_data[17]) * 100, 2) + @"%");
            base.WriteLine(_data[10] + " products have a valid shipping cost, which is " + Math.Round((double)((double)_data[10] / (double)_data[17]) * 100, 2) + @"%");
            base.WriteLine(_data[11] + " products have a valid delivery time, which is " + Math.Round((double)((double)_data[11] / (double)_data[17]) * 100, 2) + @"%");
            base.WriteLine(_data[12] + " products have a valid stock value, which is " + Math.Round((double)((double)_data[12] / (double)_data[17]) * 100, 2) + @"%");
            base.WriteLine(_data[13] + " products have a valid last modified value, which is " + Math.Round((double)((double)_data[13] / (double)_data[17]) * 100, 2) + @"%");
            base.WriteLine(_data[14] + " products have a valid 'valid until' value, which is " + Math.Round((double)((double)_data[14] / (double)_data[17]) * 100, 2) + @"%");
            base.WriteLine(_data[15] + " products have an EAN and SKU value, which is " + Math.Round((double)((double)_data[15] / (double)_data[17]) * 100, 2) + @"%");
            base.WriteLine(_data[16] + " products have an SKU value and brand, which is " + Math.Round((double)((double)_data[16] / (double)_data[17]) * 100, 2) + @"%");
            base.WriteLine(_eans.Count + " products have a unique EAN, which is " + Math.Round((double)((double)_eans.Count / (double)_data[0]) * 100, 2) + @"% of all products with an EAN.");
            base.WriteLine(_skus.Count + " products have a unique SKU, which is " + Math.Round((double)((double)_skus.Count / (double)_data[1]) * 100, 2) + @"% of all products with an SKU.");
            base.WriteLine(_cats.Count + " products have a unique category, which is " + Math.Round((double)((double)_cats.Count / (double)_data[4]) * 100, 2) + @"% of all prodcts with a category.");         
            
            //Close the streamwriter.
            base.Close();

        }

        public void AddStats(List<Product> products)
        {
            _data[17] += products.Count;
            foreach (Product p in products)
            {
                if (!p.EAN.Equals(""))
                {
                    _data[0]++;
                    _eans.Add(p.EAN);
                }
                if (!p.SKU.Equals(""))
                {
                    _data[1]++;
                    _skus.Add(p.SKU);
                }
                if (!p.Name.Equals(""))
                {
                    _data[2]++;
                }
                if (!p.Brand.Equals(""))
                {
                    _data[3]++;
                }
                if (!p.Category.Equals(""))
                {
                    _data[4]++;
                    _cats.Add(p.Category);
                }
                if (!p.Price.Equals(""))
                {
                    _data[5]++;
                }
                if (!p.Url.Equals(""))
                {
                    _data[6]++;
                }
                if (!p.Currency.Equals(""))
                {
                    _data[7]++;
                }
                if (!p.Description.Equals(""))
                {
                    _data[8]++;
                }
                if (!p.Image.Equals(""))
                {
                    _data[9]++;
                }
                if (!p.DeliveryCost.Equals(""))
                {
                    _data[10]++;
                }
                if (!p.DeliveryTime.Equals(""))
                {
                    _data[11]++;
                }
                if (!p.Stock.Equals(""))
                {
                    _data[12]++;
                }
                if (!p.LastModified.Equals(""))
                {
                    _data[13]++;
                }
                if (!p.ValidUntil.Equals(""))
                {
                    _data[14]++;
                }
                if (!(p.EAN.Equals("")) && !(p.SKU.Equals("")))
                {
                    _data[15]++;
                }
                if (!(p.Brand.Equals("")) && !(p.SKU.Equals("")))
                {
                    _data[16]++;
                }
            }
        }
    }
}
