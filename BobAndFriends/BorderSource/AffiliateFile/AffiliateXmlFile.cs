using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BorderSource.AffiliateReader;

namespace BorderSource.AffiliateFile
{
    public class AffiliateXmlFile : AffiliateFileBase
    {
        public override AffiliateReaderBase GetReader()
        {
            AffiliateReaderBase reader;
            switch(this.Name)
            {
                case "Affilinet": reader = new AffilinetReader(); break;
                case "Belboon": reader = new BelboonReader(); break;
                case "CommissionJunction": reader = new CommissionJunctionReader(); break;
                case "Daisycon": reader = new DaisyconReader(); break;
                case "TradeDoubler": reader = new TradeDoublerReader(); break;
                case "Tradetracker": reader = new TradeTrackerReader(); break;
                case "Webgains": reader = new WebgainsReader(); break;
                case "Zanox": reader = new ZanoxReader(); break;
                case "BorderBot": reader = new BorderBotReader(); break;
                default: reader = null; break;
            }
            return reader;
        }
    }
}
