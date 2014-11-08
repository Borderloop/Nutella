using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorderSource.Common
{
    public class ProductValidation
    {
        public Product Product;
        public int ArticleNumberOfEanMatch = -1;
        public int ArticleNumberOfSkuMatch = -1;
        public int ArticleNumberOfExistingProduct = -1;
        public int CategoryId = -1;
        public int CountryId;


        public bool CategorySynonymExists = false;

        public  bool EanMatched
        {
            get
            {
                return ArticleNumberOfEanMatch != -1;
            }
        }

        public bool SkuMatched
        {
            get
            {
                return ArticleNumberOfSkuMatch != -1;
            }
        }
        public bool CategoryMatched
        {
            get
            {
                return CategoryId != -1;
            }
        }
        public bool ProductAlreadyExists
        {
            get
            {
                return ArticleNumberOfExistingProduct != -1;
            }
        }

        public bool IsAmbiguous()
        {
            return (EanMatched && SkuMatched) && (ArticleNumberOfEanMatch != ArticleNumberOfSkuMatch);
        }
    }
}
