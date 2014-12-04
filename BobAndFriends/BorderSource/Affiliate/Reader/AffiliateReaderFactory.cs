﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BorderSource.Affiliate.File;
using System.IO;

namespace BorderSource.Affiliate.Reader
{
    public static class AffiliateReaderFactory
    {
        public static AffiliateReaderBase GetAppropriateReader(AffiliateFile file)
        {
            AffiliateReaderBase reader;
            switch(file.Name)
            {
                case "Bol": reader = new BolReader(); break;
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